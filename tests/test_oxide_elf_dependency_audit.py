"""Synthetic ELF fixtures: native DT_NEEDED metadata and SHA-gated APK ZIPs."""
import hashlib
from io import BytesIO
import struct
import tempfile
import unittest
import warnings
from unittest.mock import patch

from tools import oxide_elf_dependency_audit as audit_module
from pathlib import Path
from zipfile import ZipFile

from tools.oxide_elf_dependency_audit import (
    ELFMetadataError, audit_dependency_metadata, parse_aarch64_dynamic,
    parse_aarch64_dynamic_streaming,
)


def fixture_elf(needed=("libc.so", "lib_burst_0_0.so"), soname="libfoo.so"):
    """Small section-header-free ELF64 LE AArch64 with PT_LOAD and PT_DYNAMIC."""
    blob = bytearray(0x400)
    blob[:16] = b"\x7fELF\x02\x01\x01" + b"\0" * 9
    struct.pack_into("<HHI", blob, 16, 3, 183, 1)
    struct.pack_into("<QQ", blob, 32, 64, 0)
    struct.pack_into("<HHHHH", blob, 52, 64, 56, 2, 64, 0)
    struct.pack_into("<IIQQQQQQ", blob, 64, 1, 5, 0, 0x1000, 0, 0x400, 0x420, 0x1000)
    strings = b"\x00" + b"\x00".join(
        x.encode("ascii") for x in (list(needed) + ([soname] if soname else []))) + b"\x00"
    blob[0x200:0x200 + len(strings)] = strings
    dynamic = [(5, 0x1200), (10, len(strings))]
    dynamic.extend((1, strings.find(x.encode() + b"\x00")) for x in needed)
    if soname:
        dynamic.append((14, strings.find(soname.encode() + b"\x00")))
    dynamic.append((0, 0))
    assert len(dynamic) * 16 <= 0x100
    struct.pack_into("<IIQQQQQQ", blob, 120, 2, 6, 0x100, 0x1100, 0,
                     len(dynamic) * 16, len(dynamic) * 16, 8)
    for i, (tag, value) in enumerate(dynamic):
        struct.pack_into("<qQ", blob, 0x100 + i * 16, tag, value)
    return bytes(blob)


class ElfMetadataTests(unittest.TestCase):
    def test_dt_needed_and_soname_with_stripped_sections(self):
        result = parse_aarch64_dynamic(fixture_elf())
        self.assertEqual(result["dt_needed"], ["libc.so", "lib_burst_0_0.so"])
        self.assertEqual(result["dt_soname"], "libfoo.so")

    def test_bounded_stream_returns_the_same_dynamic_dependencies(self):
        elf = fixture_elf()
        result = parse_aarch64_dynamic_streaming(BytesIO(elf), len(elf))
        self.assertEqual(result, parse_aarch64_dynamic(elf))

    def test_stream_rejects_file_unbacked_dynamic_strings(self):
        elf = bytearray(fixture_elf())
        struct.pack_into("<Q", elf, 0x100 + 8, 0x1410)
        with self.assertRaisesRegex(ELFMetadataError, "AMBIGUOUS_OR_UNBACKED_STRING_TABLE"):
            parse_aarch64_dynamic_streaming(BytesIO(elf), len(elf))

    def test_empty_needed(self):
        self.assertEqual(parse_aarch64_dynamic(fixture_elf(needed=()))["dt_needed"], [])

    def test_bad_machine(self):
        elf = bytearray(fixture_elf())
        struct.pack_into("<H", elf, 18, 62)
        with self.assertRaisesRegex(ELFMetadataError, "EXPECTED_AARCH64"):
            parse_aarch64_dynamic(bytes(elf))

    def test_dynamic_string_vaddr_not_file_backed(self):
        elf = bytearray(fixture_elf())
        struct.pack_into("<Q", elf, 0x100 + 8, 0x1410)
        with self.assertRaisesRegex(ELFMetadataError, "AMBIGUOUS_OR_UNBACKED_STRING_TABLE"):
            parse_aarch64_dynamic(bytes(elf))

    def test_program_header_outside_file(self):
        elf = bytearray(fixture_elf())
        struct.pack_into("<Q", elf, 32, 0x500)
        with self.assertRaisesRegex(ELFMetadataError, "PROGRAM_HEADERS_OUTSIDE_FILE"):
            parse_aarch64_dynamic(bytes(elf))

    def test_string_index_out_of_range(self):
        elf = bytearray(fixture_elf())
        struct.pack_into("<Q", elf, 0x100 + 2 * 16 + 8, 65535)
        with self.assertRaisesRegex(ELFMetadataError, "DYNAMIC_STRING_OFFSET_OUT_OF_RANGE"):
            parse_aarch64_dynamic(bytes(elf))

    def test_dynamic_terminator_missing(self):
        elf = bytearray(fixture_elf())
        struct.pack_into("<q", elf, 0x100 + 5 * 16, 1)
        with self.assertRaisesRegex(ELFMetadataError, "DYNAMIC_TERMINATOR_MISSING"):
            parse_aarch64_dynamic(bytes(elf))


class PinnedZipDependencyTests(unittest.TestCase):
    def setUp(self):
        self.folder = tempfile.TemporaryDirectory()
        self.addCleanup(self.folder.cleanup)
        self.root = Path(self.folder.name)

    def _write_zip(self, entries, name="small.apk"):
        path = self.root / name
        with ZipFile(path, "w") as archive:
            for entry, data in entries:
                archive.writestr(entry, data)
        return {name: hashlib.sha256(path.read_bytes()).hexdigest()}

    def test_reports_dependencies_without_dlopen_causation(self):
        hashes = self._write_zip([
            ("lib/arm64-v8a/lib_burst_generated.so",
             fixture_elf(("libc.so", "liblog.so"), "lib_burst_generated.so")),
            ("lib/arm64-v8a/libfoo.so", fixture_elf(("lib_burst_0_0.so",))),
        ])
        result = audit_dependency_metadata(self.root, hashes)
        self.assertEqual(result["status"], "ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_READ_ONLY_VERIFIED")
        self.assertEqual(result["arm64_libraries_inspected"], 2)
        self.assertEqual(result["lookup_names"]["lib_burst_0_0.so"]["libraries_declaring_dt_needed"],
                         ["lib/arm64-v8a/libfoo.so"])
        self.assertEqual(result["lookup_names"]["liblib_burst_0_0.so"]["libraries_declaring_dt_needed"], [])
        self.assertEqual(result["failed_dlopen_causation"], "NOT_ESTABLISHED")
        self.assertFalse(result["original_gameplay_verified"])

    def test_oversized_library_is_explicitly_uninspected(self):
        hashes = self._write_zip([
            ("lib/arm64-v8a/libnormal.so", fixture_elf(("libc.so",))),
            ("lib/arm64-v8a/liboversized.so", fixture_elf(("libc.so",)) + b"\x00" * 256),
        ])
        with patch.object(audit_module, "MAX_ELF_BYTES", 1100), \
             patch.object(audit_module, "MAX_STREAMABLE_ELF_BYTES", 1200):
            result = audit_dependency_metadata(self.root, hashes)
        self.assertEqual(result["status"],
                         "PARTIAL_ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_VERIFIED")
        self.assertEqual(result["arm64_libraries_inspected"], 1)
        self.assertFalse(result["all_packaged_arm64_libraries_examined"])
        self.assertEqual(len(result["large_libraries_not_examined"]), 1)
        self.assertEqual(result["large_libraries_not_examined"][0]["path"],
                         "lib/arm64-v8a/liboversized.so")
        self.assertEqual(result["failed_dlopen_causation"], "NOT_ESTABLISHED")

    def test_over_memory_cap_library_is_streamed_with_complete_coverage(self):
        hashes = self._write_zip([
            ("lib/arm64-v8a/libnormal.so", fixture_elf(("libc.so",))),
            ("lib/arm64-v8a/liblarge.so", fixture_elf(("lib_burst_0_0.so",)) + b"\x00" * 256),
        ])
        with patch.object(audit_module, "MAX_ELF_BYTES", 1100), \
             patch.object(audit_module, "MAX_STREAMABLE_ELF_BYTES", 2048):
            result = audit_dependency_metadata(self.root, hashes)
        self.assertEqual(result["status"],
                         "ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_READ_ONLY_VERIFIED")
        self.assertEqual(result["arm64_libraries_inspected"], 2)
        self.assertTrue(result["all_packaged_arm64_libraries_examined"])
        self.assertEqual(len(result["large_libraries_examined_by_bounded_stream"]), 1)
        self.assertEqual(result["lookup_names"]["lib_burst_0_0.so"]["libraries_declaring_dt_needed"],
                         ["lib/arm64-v8a/liblarge.so"])
        self.assertIsNone(next(lib["elf_sha256"] for lib in result["libraries"]
                               if lib["path"].endswith("liblarge.so")))

    def test_pinned_sha_mismatch_blocks_before_archive_read(self):
        hashes = self._write_zip([("lib/arm64-v8a/libfoo.so", fixture_elf())])
        hashes["small.apk"] = "0" * 64
        result = audit_dependency_metadata(self.root, hashes)
        self.assertEqual(result["status"], "BLOCKED_UNVERIFIED_OR_INCOMPLETE_ELF_INPUT")
        self.assertIn("HASH_MISMATCH", result["reason"])

    def test_corrupt_elf_inside_verified_archive_rejected(self):
        hashes = self._write_zip([("lib/arm64-v8a/libfoo.so", b"not ELF")])
        result = audit_dependency_metadata(self.root, hashes)
        self.assertEqual(result["status"], "BLOCKED_UNVERIFIED_OR_INCOMPLETE_ELF_INPUT")
        self.assertIn("EXPECTED_ELF64_LE", result["reason"])

    def test_duplicate_zip_member_rejected(self):
        path = self.root / "small.apk"
        with warnings.catch_warnings():
            warnings.simplefilter("ignore", UserWarning)
            with ZipFile(path, "w") as archive:
                archive.writestr("lib/arm64-v8a/libfoo.so", fixture_elf())
                archive.writestr("lib/arm64-v8a/libfoo.so", fixture_elf())
        result = audit_dependency_metadata(
            self.root, {path.name: hashlib.sha256(path.read_bytes()).hexdigest()})
        self.assertEqual(result["status"], "BLOCKED_UNVERIFIED_OR_INCOMPLETE_ELF_INPUT")
        self.assertIn("DUPLICATE_ZIP_MEMBER", result["reason"])


if __name__ == "__main__":
    unittest.main()
