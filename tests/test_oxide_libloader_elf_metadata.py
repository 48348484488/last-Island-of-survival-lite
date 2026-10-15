"""Small synthetic ELF64 fixture validates read-only ARM64 loader PC mapping."""
import json
import struct
import tempfile
import unittest
from pathlib import Path
from zipfile import ZipFile

from tools.oxide_libloader_elf_metadata import inspect_elf, inspect_original_archives


def elf_fixture(machine=183, exec_flag=5):
    blob = bytearray(0x3000)
    blob[:16] = b"\x7fELF\x02\x01\x01" + b"\0"*9
    struct.pack_into("<HHI", blob, 16, 3, machine, 1)
    struct.pack_into("<QQ", blob, 32, 0x40, 0x200)
    struct.pack_into("<HHHHH", blob, 54, 56, 1, 64, 3, 2)
    struct.pack_into("<IIQQQQQQ", blob, 0x40,
                     1, exec_flag, 0x1000, 0x700000, 0, 0x2000, 0x2000, 4096)
    names = b"\0.text\0.shstrtab\0"
    blob[0x500:0x500 + len(names)] = names
    struct.pack_into("<IIQQQQIIQQ", blob, 0x200 + 64,
                     1, 1, 6, 0x700000, 0x1000, 0x2000, 0, 0, 16, 0)
    struct.pack_into("<IIQQQQIIQQ", blob, 0x200 + 128,
                     7, 3, 0, 0, 0x500, len(names), 0, 0, 1, 0)
    return bytes(blob)


class OriginalLibloaderMetadataTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.folder = Path(self.temp.name)

    def archives(self, blob):
        with ZipFile(self.folder / "base.apk", "w") as z:
            z.writestr("lib/arm64-v8a/libloader.so", blob)
        with ZipFile(self.folder / "split_config.arm64_v8a.apk", "w") as z:
            z.writestr("META-INF/list.txt", "test")

    def test_pc_offsets_are_mapped_to_executable_elf64_section(self):
        data = inspect_elf(elf_fixture())
        self.assertEqual(data["elf_machine"], "AArch64")
        self.assertEqual(data["load_segment_count"], 1)
        for pc in data["tombstone_pc_mappings"]:
            self.assertTrue(pc["mapped_load_segment"])
            self.assertTrue(pc["load_segment_executable"])
            self.assertEqual(pc["section_name"], ".text")
            self.assertTrue(pc["section_executable"])
        self.assertEqual(data["tombstone_pc_mappings"][0]["elf_file_offset"], 0x1a3c)

    def test_only_metadata_returned_for_original_archive(self):
        self.archives(elf_fixture())
        data = inspect_original_archives(self.folder)
        self.assertEqual(data["status"], "NATIVE_ARM64_LIBLOADER_PCS_MAPPED_TO_EXECUTABLE_SEGMENTS")
        self.assertEqual(data["original_apk"], "base.apk")
        self.assertFalse(data["original_apk_or_library_modified"])
        self.assertFalse(data["original_gameplay_verified"])
        self.assertEqual(data["signal_causal_caller"], "UNKNOWN")
        self.assertNotIn("binary_contents", data)
        self.assertEqual(data["original_zip_entry"], "lib/arm64-v8a/libloader.so")

    def test_wrong_architecture_is_rejected(self):
        with self.assertRaisesRegex(ValueError, "EXPECTED_AARCH64_ELF"):
            inspect_elf(elf_fixture(machine=62))

    def test_non_executable_segment_is_not_called_code(self):
        self.archives(elf_fixture(exec_flag=4))
        data = inspect_original_archives(self.folder)
        self.assertEqual(data["status"], "LIBLOADER_PC_MAPPING_INCOMPLETE")
        self.assertFalse(data["tombstone_pc_mappings"][0]["load_segment_executable"])

    def test_missing_split_is_blocked(self):
        with ZipFile(self.folder / "base.apk", "w") as z:
            z.writestr("lib/arm64-v8a/libloader.so", elf_fixture())
        with self.assertRaises(FileNotFoundError):
            inspect_original_archives(self.folder)

    def test_duplicate_loader_is_rejected(self):
        self.archives(elf_fixture())
        with ZipFile(self.folder / "split_config.arm64_v8a.apk", "w") as z:
            z.writestr("lib/arm64-v8a/libloader.so", elf_fixture())
        with self.assertRaisesRegex(ValueError, "EXPECTED_ONE_UNCHANGED_LIBLOADER"):
            inspect_original_archives(self.folder)

    def test_malformed_elf_header_is_rejected(self):
        with self.assertRaises(ValueError):
            inspect_elf(b"")
        bad = bytearray(elf_fixture())
        struct.pack_into("<Q", bad, 32, 0xFFFFFFFF)
        with self.assertRaisesRegex(ValueError, "ELF_HEADERS_OUT_OF_RANGE"):
            inspect_elf(bad)


if __name__ == "__main__":
    unittest.main()
