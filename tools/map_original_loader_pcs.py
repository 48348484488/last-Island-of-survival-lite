#!/usr/bin/env python3
"""Read-only ELF crash-PC mapping of the unchanged original Oxide libloader.so.

Maps tombstone virtual PC offsets to ELF PT_LOAD file positions and records
bounded disassembly evidence. No game code, signatures, protection or
runtime state are modified. Does not infer why a SIGILL/SI_TKILL occurred.
"""
import argparse
import hashlib
import json
import struct
import zipfile
from pathlib import Path

SHA = "c272ff0c10833dc6404559b7817d49faff637f5b417d569ee69c75e9ebd2c114"
PCS = (0x830BEC, 0x830D98)
MAX_LIBRARY = 32 * 1024 * 1024

def load_original_lib(apk_paths):
    for apk in apk_paths:
        with zipfile.ZipFile(apk) as z:
            for entry in z.infolist():
                if entry.filename.endswith("/libloader.so") and entry.file_size <= MAX_LIBRARY:
                    binary = z.read(entry)
                    digest = hashlib.sha256(binary).hexdigest()
                    if digest != SHA:
                        raise ValueError("Original libloader.so SHA mismatch: "+digest)
                    return binary, {"source_apk":Path(apk).name, "entry":entry.filename,
                                    "sha256":digest}
    raise FileNotFoundError("Expected original ARM64 libloader.so not present in supplied APKs")

def segments(binary):
    if len(binary) < 64 or binary[:4] != b"\x7fELF":
        raise ValueError("Not ELF")
    if binary[4] != 2 or binary[5] != 1:
        raise ValueError("Expected ELF64 little endian")
    machine = struct.unpack_from("<H",binary,18)[0]
    if machine != 183:
        raise ValueError("Expected original AArch64 ELF, e_machine="+str(machine))
    phoff = struct.unpack_from("<Q",binary,32)[0]
    phentsize, count = struct.unpack_from("<HH", binary,54)
    if phentsize < 56 or count > 256 or phoff + count * phentsize > len(binary):
        raise ValueError("Invalid program headers")
    out = []
    for i in range(count):
        offset = phoff + i * phentsize
        typ,flags,foff,vaddr,paddr,filesz,memsz,align = struct.unpack_from("<IIQQQQQQ",binary,offset)
        if typ != 1:
            continue
        if foff+filesz > len(binary) or filesz > memsz:
            raise ValueError("Invalid PT_LOAD")
        out.append({"file_offset":foff,"vaddr":vaddr,"filesz":filesz,
                    "memsz":memsz,"flags":flags})
    if not out:
        raise ValueError("No PT_LOAD segment")
    return out

def inspect(binary, pcs=PCS):
    loads = segments(binary)
    regions = []
    for pc in pcs:
        seg = next((s for s in loads if s["vaddr"] <= pc < s["vaddr"]+s["filesz"]),None)
        if not seg:
            raise ValueError("Tombstone PC 0x%x is not in file-backed PT_LOAD"%pc)
        file_offset = seg["file_offset"] + (pc - seg["vaddr"])
        if file_offset % 4:
            raise ValueError("Unaligned AArch64 PC")
        start = max(seg["file_offset"], file_offset-32)
        end = min(seg["file_offset"]+seg["filesz"], file_offset+36)
        data=binary[start:end]
        regions.append({"pc":hex(pc),"file_offset":hex(file_offset),
                        "hex_window":data.hex(),"window_first_file_offset":hex(start),
                        "instruction_at_pc":binary[file_offset:file_offset+4].hex(),
                        "segment_flags":seg["flags"]})
    return {"status":"ORIGINAL_SIGNED_ARM64_ELF_PC_MAPPING_PASS",
            "original_libloader_sha256":hashlib.sha256(binary).hexdigest(),
            "target_pcs":regions,
            "play_integrity_verdict":"NOT_MEASURED",
            "security_cause":"NOT_IDENTIFIED",
            "limits":["PC mapping is not a proof that the CPU executed an illegal instruction.",
                      "SI_TKILL is a directed signal, not necessarily an unknown opcode.",
                      "No original APK was modified and no protective checks were bypassed."]}

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("apk",nargs="+",type=Path)
    p.add_argument("--out",required=True,type=Path)
    options=p.parse_args()
    binary,source=load_original_lib(options.apk)
    result=inspect(binary)
    result.update(source)
    options.out.mkdir(parents=True, exist_ok=True)
    (options.out/"original_loader_pc_map.json").write_text(json.dumps(result,indent=2)+"\n")
    print(json.dumps(result,indent=2))

if __name__=="__main__":
    main()
