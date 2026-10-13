#!/usr/bin/env python3
"""Read-only AArch64 disassembly of original libloader.so crash locations."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import zipfile

TARGETS=(0x830bec,0x830d98)
EXPECTED_SHA="c272ff0c10833dc6404559b7817d49faff637f5b417d569ee69c75e9ebd2c114"

def to_offset(data,address):
    if not data.startswith(b"\x7fELF") or data[4]!=2 or data[5]!=1:
        raise ValueError("Expected ELF64 little endian")
    if struct.unpack_from("<H",data,18)[0]!=183:
        raise ValueError("Expected ARM64 machine")
    phoff=struct.unpack_from("<Q",data,32)[0]
    phentsize,phnum=struct.unpack_from("<HH",data,54)
    for i in range(phnum):
        p=phoff+i*phentsize
        if p+56>len(data):raise ValueError("Truncated headers")
        t,flags,offset,vaddr,paddr,filesz,memsz,align=struct.unpack_from("<IIQQQQQQ",data,p)
        if t==1 and vaddr<=address<vaddr+filesz:
            dest=offset+address-vaddr
            if dest>=len(data):raise ValueError("Offset exceeds ELF")
            return dest
    raise ValueError("Address not mapped to PT_LOAD")

def run(apk,outdir):
    from capstone import Cs,CS_ARCH_ARM64,CS_MODE_ARM
    with zipfile.ZipFile(apk) as z:
        data=z.read("lib/arm64-v8a/libloader.so")
    sha=hashlib.sha256(data).hexdigest()
    if sha!=EXPECTED_SHA:raise ValueError("Unexpected libloader SHA256 "+sha)
    decoder=Cs(CS_ARCH_ARM64,CS_MODE_ARM)
    records=[]
    report=["# ARM64 libloader disassembly (original APK)","",
            "Library SHA256: "+sha,
            "Crash frames from the Android emulator: 0x830BEC and 0x830D98.",
            "Static instruction decoding cannot prove the cause of a SIGILL or predict real-device stability.",""]
    for addr in TARGETS:
        start=(addr-192)&~3
        offset=to_offset(data,start)
        raw=data[offset:offset+384]
        rec=[]
        report.extend(["## Around 0x%08X"%addr,""])
        for x in decoder.disasm(raw,start):
            marker="  <-- observed crash PC" if x.address==addr else ""
            rec.append({"address":hex(x.address),"bytes":x.bytes.hex(),"mnemonic":x.mnemonic,"op_str":x.op_str})
            report.append("    0x%08x  %-10s %s%s"%(x.address,x.mnemonic,x.op_str,marker))
        report.append("")
        records.append({"crash_address":hex(addr),"elf_file_offset":offset,"instructions":rec})
    outdir.mkdir(parents=True,exist_ok=True)
    (outdir/"LOADER_DISASM.md").write_text("\n".join(report))
    (outdir/"loader_disasm.json").write_text(json.dumps({"sha256":sha,"regions":records},indent=2)+"\n")
    print("\n".join(report))
    return 0

if __name__=="__main__":
    p=argparse.ArgumentParser()
    p.add_argument("--apk",type=Path,default=Path("split_config.arm64_v8a.apk"))
    p.add_argument("--out",type=Path,default=Path("loader-disasm"))
    a=p.parse_args()
    raise SystemExit(run(a.apk,a.out))
