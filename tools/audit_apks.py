#!/usr/bin/env python3
"""Read-only split APK inspection. No APK code is executed. Python stdlib only."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import zipfile

NAMES = ("base.apk","split_config.arm64_v8a.apk","split_config.xxhdpi.apk","split_UnityDataAssetPack.apk")
TARGETS = {"libil2cpp.so","libunity.so","libloader.so","global-metadata.dat","catalog.bin","RuntimeInitializeOnLoads.json"}
MARKERS = (b"Oxide.PlayerManager",b"AreaOfInterestController",b"PlayerManager",b"NetworkIdentity",b"Mirror.NetworkClient",b"libil2cpp.sym.so",b"Oxide.Cheats.Core")

def sha256_file(path):
    h=hashlib.sha256()
    with open(path,"rb") as f:
        for b in iter(lambda:f.read(8*1024*1024),b""): h.update(b)
    return h.hexdigest()

def parse_stringpool(b,p):
    typ,header,size=struct.unpack_from("<HHI",b,p)
    if typ!=1 or header<28 or size<header or p+size>len(b): raise ValueError("bad stringpool")
    count,_,flags,start,_=struct.unpack_from("<IIIII",b,p+8)
    if count>100000 or header+count*4>size: raise ValueError("bad string count")
    utf8=bool(flags&0x100)
    def l8(pos):
        a=b[pos];pos+=1
        if a&0x80: return ((a&127)<<8)|b[pos],pos+1
        return a,pos
    def l16(pos):
        a=struct.unpack_from("<H",b,pos)[0];pos+=2
        if a&0x8000: return ((a&32767)<<16)|struct.unpack_from("<H",b,pos)[0],pos+2
        return a,pos
    out=[]
    for i in range(count):
        off=struct.unpack_from("<I",b,p+header+4*i)[0]
        pos=p+start+off
        if pos<p or pos>=p+size: raise ValueError("string out of bounds")
        if utf8:
            _,pos=l8(pos)
            n,pos=l8(pos)
            if pos+n>p+size: raise ValueError("string exceeds pool")
            out.append(b[pos:pos+n].decode("utf-8","replace"))
        else:
            n,pos=l16(pos)
            if pos+2*n>p+size: raise ValueError("string exceeds pool")
            out.append(b[pos:pos+2*n].decode("utf-16-le","replace"))
    return out

def manifest(z):
    try: b=z.read("AndroidManifest.xml")
    except KeyError: return {"error":"Missing manifest"}
    if len(b)>10*1024*1024: return {"error":"Oversized manifest"}
    if b.lstrip().startswith(b"<"):
        import xml.etree.ElementTree as ET
        e=ET.fromstring(b)
        ns="{http://schemas.android.com/apk/res/android}"
        return {k:e.attrib.get(ns+k if k.startswith("version") else k) for k in ("package","split","versionName","versionCode")}
    try:
        if len(b)<8 or struct.unpack_from("<H",b)[0]!=3: raise ValueError("not AXML")
        p=struct.unpack_from("<H",b,2)[0]
        strings=[]
        while p+8<=len(b):
            typ,header,size=struct.unpack_from("<HHI",b,p)
            if header<8 or size<header or p+size>len(b): raise ValueError("invalid chunk")
            if typ==1: strings=parse_stringpool(b,p)
            elif typ==0x102 and header>=36 and strings:
                idx=struct.unpack_from("<I",b,p+20)[0]
                if idx<len(strings) and strings[idx]=="manifest":
                    attr_off,attr_size,count=struct.unpack_from("<HHH",b,p+24)
                    d={}
                    for i in range(count):
                        pos=p+16+attr_off+i*attr_size
                        if attr_size<20 or pos+20>p+size: raise ValueError("invalid attribute")
                        _,nm,raw,_,_,kind,value=struct.unpack_from("<IIIHBBI",b,pos)
                        if nm>=len(strings): continue
                        key=strings[nm]
                        if key not in ("package","split","versionName","versionCode"): continue
                        d[key]=(strings[raw] if raw<len(strings) else
                                strings[value] if kind==3 and value<len(strings) else value)
                    return d
            p+=size
        raise ValueError("manifest tag absent")
    except (ValueError,IndexError,struct.error) as exc:
        return {"error":"AXML parse: "+str(exc)}

def inspect(path):
    d={"name":path.name,"bytes":path.stat().st_size,"sha256":sha256_file(path)}
    with zipfile.ZipFile(path) as z:
        info=z.infolist()
        d["entry_count"]=len(info)
        d["uncompressed_bytes"]=sum(i.file_size for i in info)
        d["manifest"]=manifest(z)
        d["largest_entries"]=[{"path":i.filename,"size":i.file_size} for i in sorted(info,key=lambda x:x.file_size,reverse=True)[:6]]
        d["interesting"]=[]
        d["so_count"]=sum(i.filename.endswith(".so") for i in info)
        for i in info:
            name=i.filename.split("/")[-1]
            if name not in TARGETS: continue
            o={"path":i.filename,"bytes":i.file_size,"compressed_bytes":i.compress_size}
            if i.file_size<=400*1024*1024:
                h=hashlib.sha256()
                first=b""
                counts={s.decode():0 for s in MARKERS}
                last=b""
                with z.open(i) as f:
                    while True:
                        chunk=f.read(2*1024*1024)
                        if not chunk: break
                        if not first: first=chunk[:32]
                        h.update(chunk)
                        if name in ("libil2cpp.so","libloader.so","libunity.so"):
                            joined=last+chunk
                            for s in MARKERS: counts[s.decode()]+=joined.count(s)
                            last=joined[-128:]
                o["sha256"]=h.hexdigest()
                if first.startswith(b"\x7fELF") and len(first)>=20:
                    o["elf_machine"]=struct.unpack_from("<H",first,18)[0]
                if name=="global-metadata.dat":
                    o["header_hex"]=first[:16].hex()
                    if len(first)>=8:o["version_field"]=struct.unpack_from("<I",first,4)[0]
                o["string_markers"]={k:v for k,v in counts.items() if v}
            d["interesting"].append(o)
    return d

def audit(folder,out,expected_sha=""):
    files=[];errors=[]
    for name in NAMES:
        p=folder/name
        if not p.is_file():
            errors.append("missing "+name);continue
        try: files.append(inspect(p))
        except (OSError,zipfile.BadZipFile,ValueError,RuntimeError) as e:
            errors.append(name+": "+str(e))
    pack=next((f for f in files if f["name"]==NAMES[-1]),None)
    if expected_sha and pack and pack["sha256"].lower()!=expected_sha.lower():
        errors.append("asset pack SHA-256 mismatch with Release metadata")
    for field in ("package","versionCode"):
        values={str(f["manifest"][field]) for f in files if f["manifest"].get(field) is not None}
        if len(values)>1:errors.append(field+" mismatch: "+str(sorted(values)))
    r={"status":"PASS" if not errors else "FAIL","files":files,"errors":errors,
       "reference_old_version":"1.13.11946",
       "limitations":["Static inspection only; does not run the game.",
                      "Does not prove that a modified game starts without crashing.",
                      "No live player memory addresses or ESP functionality recovered."]}
    out.mkdir(parents=True,exist_ok=True)
    (out/"audit.json").write_text(json.dumps(r,ensure_ascii=False,indent=2)+"\n")
    lines=["# Oxide APK audit","","Status: **"+r["status"]+"**","","| APK | Version | Entries | SHA256 |","|---|---|---:|---|"]
    for f in files:
        m=f["manifest"]
        lines.append("| "+f["name"]+" | "+str(m.get("versionName","?"))+" / "+str(m.get("versionCode","?"))+" | "+str(f["entry_count"])+" | "+f["sha256"]+" |")
    lines+=["","## Native binaries, metadata and resource catalogs",""]
    for f in files:
        for i in f["interesting"]:
            lines.append("- "+f["name"]+": "+i["path"]+", size="+str(i["bytes"])+", SHA256="+i.get("sha256","not scanned"))
            if "elf_machine" in i:lines.append("  - ELF machine: "+str(i["elf_machine"]))
            if "version_field" in i:lines.append("  - Metadata version field: "+str(i["version_field"])+", header="+i["header_hex"])
            if i.get("string_markers"):lines.append("  - String markers: "+json.dumps(i["string_markers"]))
    lines+=["","## Checks","","- "+e for e in errors] if errors else ["","## Checks","","All four ZIP central directories readable, all APK SHA256 hashes computed, cross-split package/version agreement checked when present."]
    lines+=["","## Limitations","",*["- "+s for s in r["limitations"]],""]
    (out/"REPORT.md").write_text("\n".join(lines))
    return r

def main():
    p=argparse.ArgumentParser()
    p.add_argument("--apk-dir",type=Path,default=Path("."))
    p.add_argument("--out",type=Path,default=Path("audit-output"))
    p.add_argument("--assetpack-sha256",default="")
    a=p.parse_args()
    r=audit(a.apk_dir,a.out,a.assetpack_sha256)
    print(json.dumps({"status":r["status"],"versions":[f["manifest"] for f in r["files"]],"errors":r["errors"]},indent=2))
    return 0 if r["status"]=="PASS" else 1
if __name__=="__main__":raise SystemExit(main())
