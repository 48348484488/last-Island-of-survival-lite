#!/usr/bin/env python3
"""Read-only ELF metadata for the *unchanged* Oxide ARM64 libloader.so.

Only inspect existing original APK ZIP entries. No binary is changed, repacked,
patched, executed, disassembled, or uploaded. Tombstone PC offsets are used
solely to locate ELF segment/section and do NOT establish security causation.
"""
import argparse
import hashlib
import json
import struct
from pathlib import Path
from zipfile import ZipFile

PCS = (0x700A3C, 0x700BE8)
ORIGINALS = ("base.apk", "split_config.arm64_v8a.apk")


def inspect_elf(blob):
    if len(blob) < 64 or blob[:6] != b"\x7fELF\x02\x01":
        raise ValueError("EXPECTED_LITTLE_ENDIAN_ELF64")
    if struct.unpack_from("<H", blob, 18)[0] != 183:
        raise ValueError("EXPECTED_AARCH64_ELF")
    e_type = struct.unpack_from("<H", blob, 16)[0]
    if e_type != 3:
        raise ValueError("EXPECTED_ELF_ET_DYN")
    phoff, shoff = struct.unpack_from("<QQ", blob, 32)
    phentsize, phnum, shentsize, shnum, shstrndx = struct.unpack_from("<HHHHH", blob, 54)
    if (phentsize < 56 or shentsize < 64 or shstrndx >= shnum
            or phoff + phnum * phentsize > len(blob)
            or shoff + shnum * shentsize > len(blob)):
        raise ValueError("ELF_HEADERS_OUT_OF_RANGE")

    segments = []
    for i in range(phnum):
        fields = struct.unpack_from("<IIQQQQQQ", blob, phoff + i * phentsize)
        t, flags, offset, vaddr, _, filesz, memsz, align = fields
        if t == 1:  # PT_LOAD
            if offset + filesz > len(blob):
                raise ValueError("ELF_LOAD_FILE_RANGE_INVALID")
            segments.append({
                "vaddr": vaddr, "memsz": memsz, "offset": offset,
                "filesz": filesz, "flags": "".join(
                    bit for flag, bit in ((4, "R"), (2, "W"), (1, "X")) if flags & flag),
                "align": align,
            })

    sections = []
    raw_sections = []
    for i in range(shnum):
        raw_sections.append(struct.unpack_from(
            "<IIQQQQIIQQ", blob, shoff + i * shentsize))
    table = raw_sections[shstrndx]
    _, _, _, _, table_offset, table_size, *_ = table
    if table_offset + table_size > len(blob):
        raise ValueError("ELF_SECTION_NAMES_INVALID")
    strings = blob[table_offset:table_offset + table_size]

    for x in raw_sections:
        index, section_type, flags, vaddr, offset, size, _, _, align, _ = x
        end = strings.find(b"\0", index)
        if not 0 <= index < len(strings) or end < 0:
            raise ValueError("ELF_SECTION_NAME_OUT_OF_RANGE")
        name = strings[index:end].decode("ascii", errors="replace")
        sections.append({
            "name": name, "address": vaddr, "size": size,
            "file_offset": offset, "type": section_type,
            "executable": bool(flags & 0x4), "align": align,
        })

    locations = []
    for pc in PCS:
        match = next((s for s in segments if s["vaddr"] <= pc < s["vaddr"] + s["memsz"]), None)
        section = next((s for s in sections if s["address"] <= pc < s["address"] + s["size"]), None)
        locations.append({
            "tombstone_pc": f"0x{pc:08x}",
            "mapped_load_segment": match is not None,
            "load_segment_executable": "X" in match["flags"] if match else False,
            "elf_file_offset": (match["offset"] + pc - match["vaddr"]) 
            if match and pc - match["vaddr"] < match["filesz"] else None, 
            "file_backed_executable": bool(match and "X" in match["flags"] 
                and pc - match["vaddr"] < match["filesz"] 
                and section and section["executable"]),
            "section_name": section["name"] if section else None,
            "section_executable": section["executable"] if section else None,
        })

    return {
        "elf_machine": "AArch64",
        "elf_type": "ET_DYN",
        "elf_size_bytes": len(blob),
        "elf_sha256": hashlib.sha256(blob).hexdigest(),
        "load_segment_count": len(segments),
        "has_init_array_section": any(s["name"] == ".init_array" for s in sections),
        "has_init_section": any(s["name"] == ".init" for s in sections),
        "tombstone_pc_mappings": locations,
    }


def inspect_original_archives(directory):
    directory = Path(directory)
    matches = []
    for apk_name in ORIGINALS:
        path = directory / apk_name
        if not path.is_file():
            raise FileNotFoundError(f"MISSING_ORIGINAL_APK: {apk_name}")
        with ZipFile(path) as archive:
            candidates = [x for x in archive.namelist()
                          if x.startswith("lib/") and x.endswith("/libloader.so")]
            for entry in candidates:
                info = inspect_elf(archive.read(entry))
                info["original_apk"] = apk_name
                info["original_zip_entry"] = entry
                matches.append(info)
    if len(matches) != 1:
        raise ValueError(f"EXPECTED_ONE_UNCHANGED_LIBLOADER: {len(matches)}")
    result = matches[0]
    executable = all(x["file_backed_executable"] for x in result["tombstone_pc_mappings"])
    result.update({
        "status": ("NATIVE_ARM64_LIBLOADER_PCS_MAPPED_TO_EXECUTABLE_SEGMENTS"
                   if executable else "LIBLOADER_PC_MAPPING_INCOMPLETE"),
        "signal_origin": "SI_TKILL",
        "signal_causal_caller": "UNKNOWN",
        "security_trigger": "UNKNOWN",
        "original_apk_or_library_modified": False,
        "original_gameplay_verified": False,
        "note": "ELF metadata maps guest PCs but cannot identify why SIGILL was signaled.",
    })
    return result


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--directory", type=Path, default=Path("."))
    parser.add_argument("--output", type=Path, default=Path("reports/libloader-elf-metadata.json"))
    args = parser.parse_args()
    result = inspect_original_archives(args.directory)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["status"] == "NATIVE_ARM64_LIBLOADER_PCS_MAPPED_TO_EXECUTABLE_SEGMENTS" else 2


if __name__ == "__main__":
    raise SystemExit(main())
