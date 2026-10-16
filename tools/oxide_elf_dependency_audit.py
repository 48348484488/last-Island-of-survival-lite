#!/usr/bin/env python3
"""Read-only ELF PT_DYNAMIC / DT_NEEDED metadata from pinned original ARM64 splits.

Does not extract files to disk, execute, load, alter, sign, or disassemble code.
Only the three smaller original APK splits are inspected, never the asset pack.
The presence or absence of DT_NEEDED cannot establish why the app exited.
"""
import argparse
import hashlib
import json
import struct
from pathlib import Path
from zipfile import BadZipFile, ZipFile

if __package__:
    from .oxide_native_library_inventory import EXPECTED_PINNED_HASHES, sha256_file
else:
    from oxide_native_library_inventory import EXPECTED_PINNED_HASHES, sha256_file

MAX_ELF_BYTES = 160 * 1024 * 1024  # in-memory cap
MAX_STREAMABLE_ELF_BYTES = 512 * 1024 * 1024  # bounded streaming cap
MAX_DYNAMIC_STRING_BYTES = 32 * 1024 * 1024  # only metadata strings in memory
MAX_DYNAMIC_ENTRIES = 32768
LOOKUPS = ("lib_burst_generated.so", "liblib_burst_0_0.so", "lib_burst_0_0.so")


class ELFMetadataError(ValueError):
    """Unexpected ELF metadata or unverified original APK."""


def _check_span(blob, offset, length, label):
    if offset < 0 or length < 0 or offset > len(blob) or length > len(blob) - offset:
        raise ELFMetadataError(label)


def parse_aarch64_dynamic(blob):
    """Inspect only program headers, dynamic tags and file-backed strings."""
    if len(blob) < 64 or blob[:6] != b"\x7fELF\x02\x01" or blob[6] != 1:
        raise ELFMetadataError("EXPECTED_ELF64_LE")
    if struct.unpack_from("<H", blob, 18)[0] != 183:
        raise ELFMetadataError("EXPECTED_AARCH64")
    if struct.unpack_from("<H", blob, 16)[0] != 3:
        raise ELFMetadataError("EXPECTED_SHARED_OBJECT")
    phoff = struct.unpack_from("<Q", blob, 32)[0]
    phentsize, phnum = struct.unpack_from("<HH", blob, 54)
    if phentsize < 56 or not (1 <= phnum <= 4096):
        raise ELFMetadataError("BAD_PROGRAM_HEADERS")
    _check_span(blob, phoff, phnum * phentsize, "PROGRAM_HEADERS_OUTSIDE_FILE")

    loads, dynamics = [], []
    for i in range(phnum):
        t, flags, off, va, _, file_sz, mem_sz, align = struct.unpack_from(
            "<IIQQQQQQ", blob, phoff + i * phentsize)
        if file_sz > mem_sz:
            raise ELFMetadataError("SEGMENT_FILE_SIZE_EXCEEDS_MEMORY")
        if t in (1, 2) and file_sz:
            _check_span(blob, off, file_sz, "SEGMENT_OUTSIDE_FILE")
        if t == 1:
            loads.append((va, off, file_sz, mem_sz))
        elif t == 2:
            dynamics.append((off, file_sz))
    if len(dynamics) != 1 or not loads:
        raise ELFMetadataError("EXPECTED_SINGLE_DYNAMIC_AND_LOAD_SEGMENTS")
    dynamic_off, dynamic_size = dynamics[0]
    if not dynamic_size or dynamic_size % 16 or dynamic_size // 16 > MAX_DYNAMIC_ENTRIES:
        raise ELFMetadataError("BAD_DYNAMIC_SEGMENT_SIZE")

    tags = {}
    terminator = False
    for offset in range(dynamic_off, dynamic_off + dynamic_size, 16):
        tag, value = struct.unpack_from("<qQ", blob, offset)
        if tag == 0:
            terminator = True
            break
        if tag in (1, 5, 10, 14):
            tags.setdefault(tag, []).append(value)
    if not terminator:
        raise ELFMetadataError("DYNAMIC_TERMINATOR_MISSING")
    if len(tags.get(5, [])) != 1 or len(tags.get(10, [])) != 1:
        raise ELFMetadataError("DYNAMIC_STRING_TABLE_METADATA_MISSING")
    string_va = tags[5][0]
    size = tags[10][0]
    if size == 0 or size > MAX_ELF_BYTES:
        raise ELFMetadataError("BAD_DYNAMIC_STRING_TABLE_SIZE")

    mapped = []
    for vaddr, file_offset, file_size, _ in loads:
        if vaddr <= string_va < vaddr + file_size and size <= vaddr + file_size - string_va:
            mapped.append(file_offset + string_va - vaddr)
    if len(mapped) != 1:
        raise ELFMetadataError("AMBIGUOUS_OR_UNBACKED_STRING_TABLE")
    table_start = mapped[0]
    _check_span(blob, table_start, size, "DYNAMIC_STRING_TABLE_OUTSIDE_FILE")
    strings = blob[table_start:table_start + size]

    def get_string(index):
        if index >= len(strings):
            raise ELFMetadataError("DYNAMIC_STRING_OFFSET_OUT_OF_RANGE")
        stop = strings.find(b"\x00", index)
        if stop < 0:
            raise ELFMetadataError("DYNAMIC_STRING_NOT_TERMINATED")
        try:
            return strings[index:stop].decode("utf-8", "strict")
        except UnicodeDecodeError as exc:
            raise ELFMetadataError("DYNAMIC_STRING_NOT_UTF8") from exc

    needed = [get_string(i) for i in tags.get(1, [])]
    sonames = [get_string(i) for i in tags.get(14, [])]
    if len(sonames) > 1:
        raise ELFMetadataError("MULTIPLE_SONAME_ENTRIES")
    return {"dt_needed": needed, "dt_soname": sonames[0] if sonames else None}



def parse_aarch64_dynamic_streaming(source, file_size):
    """Read headers, dynamic tags and needed names with bounded seek/read.

    ZipExtFile may decompress skipped regions internally. It does NOT load the
    entire .so in memory or execute any native code.
    """
    if not (64 <= file_size <= MAX_STREAMABLE_ELF_BYTES):
        raise ELFMetadataError("STREAM_ELF_SIZE_OUT_OF_RANGE")

    def read_at(offset, length):
        if offset < 0 or length < 0 or offset > file_size or length > file_size - offset:
            raise ELFMetadataError("STREAM_ELF_READ_OUT_OF_RANGE")
        if length > MAX_DYNAMIC_STRING_BYTES:
            raise ELFMetadataError("STREAM_ELF_READ_OVER_MEMORY_LIMIT")
        source.seek(offset)
        result = source.read(length)
        if len(result) != length:
            raise ELFMetadataError("STREAM_ELF_SHORT_READ")
        return result

    header = read_at(0, 64)
    if header[:6] != b"\x7fELF\x02\x01" or header[6] != 1:
        raise ELFMetadataError("EXPECTED_ELF64_LE")
    if struct.unpack_from("<H", header, 18)[0] != 183:
        raise ELFMetadataError("EXPECTED_AARCH64")
    if struct.unpack_from("<H", header, 16)[0] != 3:
        raise ELFMetadataError("EXPECTED_SHARED_OBJECT")
    phoff = struct.unpack_from("<Q", header, 32)[0]
    phentsize, phnum = struct.unpack_from("<HH", header, 54)
    if phentsize < 56 or not (1 <= phnum <= 4096):
        raise ELFMetadataError("BAD_PROGRAM_HEADERS")
    phdata = read_at(phoff, phnum * phentsize)
    loads, dynamics = [], []
    for i in range(phnum):
        t, flags, off, va, _, file_sz, mem_sz, align = struct.unpack_from(
            "<IIQQQQQQ", phdata, i * phentsize)
        if file_sz > mem_sz or off + file_sz > file_size:
            raise ELFMetadataError("SEGMENT_OUTSIDE_FILE")
        if t == 1:
            loads.append((va, off, file_sz))
        elif t == 2:
            dynamics.append((off, file_sz))
    if len(dynamics) != 1 or not loads:
        raise ELFMetadataError("EXPECTED_SINGLE_DYNAMIC_AND_LOAD_SEGMENTS")
    dynamic_offset, dynamic_size = dynamics[0]
    if not dynamic_size or dynamic_size % 16 or dynamic_size // 16 > MAX_DYNAMIC_ENTRIES:
        raise ELFMetadataError("BAD_DYNAMIC_SEGMENT_SIZE")
    dyn = read_at(dynamic_offset, dynamic_size)
    tags, terminator = {}, False
    for offset in range(0, len(dyn), 16):
        tag, value = struct.unpack_from("<qQ", dyn, offset)
        if tag == 0:
            terminator = True
            break
        if tag in (1, 5, 10, 14):
            tags.setdefault(tag, []).append(value)
    if not terminator:
        raise ELFMetadataError("DYNAMIC_TERMINATOR_MISSING")
    if len(tags.get(5, [])) != 1 or len(tags.get(10, [])) != 1:
        raise ELFMetadataError("DYNAMIC_STRING_TABLE_METADATA_MISSING")
    string_va, string_size = tags[5][0], tags[10][0]
    if not (0 < string_size <= MAX_DYNAMIC_STRING_BYTES):
        raise ELFMetadataError("BAD_DYNAMIC_STRING_TABLE_SIZE")
    mapped = [off + string_va - va for va, off, count in loads
              if va <= string_va < va + count and string_size <= va + count - string_va]
    if len(mapped) != 1:
        raise ELFMetadataError("AMBIGUOUS_OR_UNBACKED_STRING_TABLE")
    strings = read_at(mapped[0], string_size)

    def string_at(index):
        if index >= len(strings):
            raise ELFMetadataError("DYNAMIC_STRING_OFFSET_OUT_OF_RANGE")
        end = strings.find(b"\x00", index)
        if end < 0:
            raise ELFMetadataError("DYNAMIC_STRING_NOT_TERMINATED")
        try:
            return strings[index:end].decode("utf-8", "strict")
        except UnicodeDecodeError as exc:
            raise ELFMetadataError("DYNAMIC_STRING_NOT_UTF8") from exc

    names = [string_at(index) for index in tags.get(1, [])]
    sonames = [string_at(index) for index in tags.get(14, [])]
    if len(sonames) > 1:
        raise ELFMetadataError("MULTIPLE_SONAME_ENTRIES")
    return {"dt_needed": names, "dt_soname": sonames[0] if sonames else None}


def _checked_apks(directory, expected):
    result = {}
    for apk_name, digest in expected.items():
        path = directory / apk_name
        if not path.is_file():
            raise ELFMetadataError("ORIGINAL_APK_MISSING:" + apk_name)
        actual = sha256_file(path)
        if actual != digest:
            raise ELFMetadataError("ORIGINAL_APK_HASH_MISMATCH:" + apk_name)
        result[apk_name] = {"sha256": actual, "size_bytes": path.stat().st_size}
    return result


def audit_dependency_metadata(directory, expected_hashes=EXPECTED_PINNED_HASHES):
    """Fail closed before native ZIP reads unless all three pinned hashes match."""
    directory = Path(directory)
    try:
        original_apks = _checked_apks(directory, expected_hashes)
        libraries = []
        skipped_libraries = []
        streamed_libraries = []
        for apk_name in expected_hashes:
            with ZipFile(directory / apk_name, "r") as archive:
                entries = archive.infolist()
                names = [x.filename for x in entries]
                if len(names) != len(set(names)):
                    raise ELFMetadataError("DUPLICATE_ZIP_MEMBER")
                for member in entries:
                    if not (member.filename.startswith("lib/arm64-v8a/")
                            and member.filename.endswith(".so")):
                        continue
                    if member.file_size > MAX_STREAMABLE_ELF_BYTES:
                        skipped_libraries.append({
                            "apk": apk_name, "path": member.filename,
                            "uncompressed_size_bytes": member.file_size,
                            "reason": "EXCEEDS_PER_LIBRARY_STREAMING_CAP",
                        })
                        continue
                    if member.file_size > MAX_ELF_BYTES:
                        with archive.open(member, "r") as stream:
                            meta = parse_aarch64_dynamic_streaming(stream, member.file_size)
                        streamed_libraries.append({
                            "apk": apk_name, "path": member.filename,
                            "uncompressed_size_bytes": member.file_size,
                        })
                        digest = None  # No complete binary loaded or hashed.
                    else:
                        with archive.open(member, "r") as stream:
                            elf = stream.read(MAX_ELF_BYTES + 1)
                        if len(elf) != member.file_size or len(elf) > MAX_ELF_BYTES:
                            raise ELFMetadataError("ELF_MEMBER_SIZE_INCORRECT")
                        meta = parse_aarch64_dynamic(elf)
                        digest = hashlib.sha256(elf).hexdigest()
                    libraries.append({
                        "apk": apk_name, "path": member.filename,
                        "elf_sha256": digest,
                        "dt_soname": meta["dt_soname"],
                        "dt_needed": meta["dt_needed"],
                    })
        if not libraries:
            raise ELFMetadataError("NO_ARM64_ELF_LIBRARIES")
        library_names = [x["path"].rsplit("/", 1)[-1] for x in libraries]
        if len(library_names) != len(set(library_names)):
            raise ELFMetadataError("DUPLICATE_NATIVE_LIBRARY_BASENAME")
        found = {name: [lib["path"] for lib in libraries
                         if lib["path"].rsplit("/", 1)[-1] == name]
                 for name in LOOKUPS}
        users = {name: [lib["path"] for lib in libraries if name in lib["dt_needed"]]
                 for name in LOOKUPS}
        return {
            "status": ("PARTIAL_ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_VERIFIED"
                       if skipped_libraries else
                       "ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_READ_ONLY_VERIFIED"),
            "original_apk_sha256_verified": original_apks,
            "arm64_libraries_inspected": len(libraries),
            "libraries": libraries,
            "large_libraries_not_examined": skipped_libraries,
            "large_libraries_examined_by_bounded_stream": streamed_libraries,
            "individual_elf_hash_complete": len(streamed_libraries) == 0,
            "all_packaged_arm64_libraries_examined": not skipped_libraries,
            "lookup_names": {name: {
                "packaged_paths": found[name],
                "libraries_declaring_dt_needed": users[name],
            } for name in LOOKUPS},
            "all_four_splits_inspected": False,
            "large_asset_pack_inspected": False,
            "other_runtime_sources_inspected": False,
            "original_apks_modified": False,
            "original_gameplay_verified": False,
            "security_trigger": "UNKNOWN",
            "failed_dlopen_causation": "NOT_ESTABLISHED",
            "note": "DT_NEEDED entries are ELF loader declarations; dlopen calls can request "
                    "additional names. No mandatory dependency or app-exit cause inferred.",
        }
    except (ELFMetadataError, BadZipFile, OSError, ValueError) as exc:
        return {"status": "BLOCKED_UNVERIFIED_OR_INCOMPLETE_ELF_INPUT",
                "reason": str(exc), "original_apks_modified": False,
                "original_gameplay_verified": False}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--directory", type=Path, default=Path("."))
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = audit_dependency_metadata(args.directory)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["status"] in (
        "ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_READ_ONLY_VERIFIED",
        "PARTIAL_ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_VERIFIED",
    ) else 2


if __name__ == "__main__":
    raise SystemExit(main())
