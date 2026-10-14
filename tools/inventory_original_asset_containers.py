#!/usr/bin/env python3
"""Inventory original APK ZIP member format metadata, not security implementation.

Identify UnityFS/UnityWeb/UnityRaw headers at the beginning of stored assets.
These formats may contain *internally compressed* Unity resource blocks that a
literal scan over ZIP-decompressed bytes cannot search transparently.
No decompression of nested proprietary content and no bypass/instrumentation.
"""
import argparse
import json
from collections import Counter
from pathlib import Path
import zipfile
import struct
from verify_original_splits import EXPECTED_SHA256, sha256_file

CONTAINER_MAGIC = (
    (b"UnityFS\x00", "UNITY_FS_ASSET_BUNDLE"),
    (b"UnityWeb\x00", "UNITY_WEB_ASSET_BUNDLE"),
    (b"UnityRaw\x00", "UNITY_RAW_ASSET_BUNDLE"),
    (b"SQLite format 3\x00", "SQLITE_DATABASE"),
    (b"\x1f\x8b\x08", "GZIP_CONTENT"),
    (b"\xfd7zXZ\x00", "XZ_CONTENT"),
    (b"PK\x03\x04", "NESTED_ZIP_CONTENT"),
)
ZIP_METHODS={0:"STORED",8:"DEFLATED",12:"BZIP2",14:"LZMA_ZIP"}

def identify(header):
    for magic,description in CONTAINER_MAGIC:
        if header.startswith(magic):
            return description
    return "NOT_IDENTIFIED_BY_SIGNATURE"

def parse_unityfs_header(header):
    """Read only UnityFS version/block-info compression flags; do not decode data."""
    if not header.startswith(bytes((85,110,105,116,121,70,83,0))):
        return None
    if len(header) < 20:
        return {"status":"INCOMPLETE_HEADER"}
    version=struct.unpack_from(">I",header,8)[0]
    pos=12
    names=[]
    for _ in range(2):
        end=header.find(bytes((0,)),pos)
        if end==-1 or end-pos>90:
            return {"status":"INCOMPLETE_HEADER","bundle_format_version":version}
        names.append(header[pos:end].decode("utf-8",errors="replace"))
        pos=end+1
    if len(header)<pos+20:
        return {"status":"INCOMPLETE_HEADER","bundle_format_version":version}
    total_size,packed,unpacked,flags=struct.unpack_from(">QIII",header,pos)
    compression_code=flags&0x3f
    compression={0:"NONE",1:"LZMA",2:"LZ4",3:"LZ4HC"}.get(
        compression_code,"UNKNOWN_"+str(compression_code))
    return {
        "status":"HEADER_READ",
        "bundle_format_version":version,
        "unity_version":names[0],
        "unity_revision":names[1],
        "bundle_declared_size":total_size,
        "blocks_info_compressed_bytes":packed,
        "blocks_info_uncompressed_bytes":unpacked,
        "blocks_info_compression":compression,
        "blocks_info_at_file_end":bool(flags&0x80),
    }

def inventory(paths,expected=None):
    expected=expected or EXPECTED_SHA256
    actual={}
    member_totals=Counter()
    format_totals=Counter()
    unityfs_compression_totals=Counter()
    unityfs_format_versions=Counter()
    method_totals=Counter()
    by_split={}
    signature_examples=[]
    largest=[]
    for path in paths:
        if path.name not in expected:
            raise ValueError("Not an expected signed original split: "+path.name)
        digest=sha256_file(path)
        if digest!=expected[path.name]:
            raise ValueError("Original APK checksum mismatch: "+path.name)
        actual[path.name]=digest
        records=[]
        with zipfile.ZipFile(path) as archive:
            for member in archive.infolist():
                if member.is_dir():
                    continue
                with archive.open(member) as handle:
                    # Reading the initial 64B is bounded, even for giant Unity assets.
                    header=handle.read(512)
                kind=identify(header)
                unity_header=parse_unityfs_header(header) if kind=="UNITY_FS_ASSET_BUNDLE" else None
                if unity_header:
                    unityfs_compression_totals[unity_header.get("blocks_info_compression",
                                                 "INCOMPLETE_HEADER")]+=1
                    unityfs_format_versions[str(unity_header.get("bundle_format_version",
                                                "UNKNOWN"))]+=1
                method=ZIP_METHODS.get(member.compress_type,
                        "UNKNOWN_METHOD_"+str(member.compress_type))
                method_totals[method]+=1
                format_totals[kind]+=1
                member_totals[path.name]+=1
                record={"apk":path.name,"entry":member.filename,
                        "size_uncompressed":member.file_size,
                        "size_in_zip":member.compress_size,
                        "zip_method":method,
                        "header_kind":kind}
                largest.append(record)
                if kind!="NOT_IDENTIFIED_BY_SIGNATURE":
                    if unity_header is not None:
                        record["unityfs_header"]=unity_header
                    signature_examples.append(record)
                records.append(record)
        by_split[path.name]={
            "members":len(records),
            "bytes_uncompressed":sum(x["size_uncompressed"] for x in records),
            "bytes_in_zip":sum(x["size_in_zip"] for x in records),
            "recognized_header_counts":dict(Counter(x["header_kind"] for x in records
                                                   if x["header_kind"]!="NOT_IDENTIFIED_BY_SIGNATURE")),
        }
    largest.sort(key=lambda x:x["size_uncompressed"],reverse=True)
    return {
        "status":"ORIGINAL_APK_ZIP_MEMBER_FORMAT_INVENTORY_COMPLETE",
        "apks_sha256_verified":actual,
        "all_original_splits_included":set(actual)==set(EXPECTED_SHA256),
        "members_total":sum(member_totals.values()),
        "by_split":by_split,
        "zip_compression_methods":dict(method_totals),
        "identified_header_kinds":dict(format_totals),
        "unityfs_block_info_compression":dict(unityfs_compression_totals),
        "unityfs_format_versions":dict(unityfs_format_versions),
        "recognized_nested_container_examples":signature_examples[:75],
        "largest_members_by_uncompressed_size":largest[:20],
        "limitations":[
            "Only ZIP member header bytes, sizes, names and compression metadata were inspected.",
            "An asset bundle may use internal LZ4/LZMA compression; no nested decompression was attempted.",
            "A missing UnityFS header is not proof that resource data is uncompressed or does not exist.",
            "This file format inventory cannot identify the code, source or trigger of any security toast.",
            "No APK, Android device identity, security protection or game player data was modified.",
        ],
        "original_game_modified":False,
        "app_security_root_cause_proven":False,
        "gameplay_or_real_esp_verified":False,
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("apks",nargs="+",type=Path)
    p.add_argument("--out",type=Path,required=True)
    a=p.parse_args()
    result=inventory(a.apks)
    a.out.mkdir(parents=True,exist_ok=True)
    (a.out/"original_asset_containers.json").write_text(
        json.dumps(result,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
    lines=["# Original Oxide APK resource container inventory","",
        "APK SHA-256 verified: "+str(len(result["apks_sha256_verified"])),
        "All four original splits: "+str(result["all_original_splits_included"]),
        "ZIP members inventoried: "+str(result["members_total"]),
        "", "## ZIP member compression methods",""]
    for key,val in result["zip_compression_methods"].items():
        lines.append("- "+key+": "+str(val))
    lines+=["","## Identified nested content magic headers",""]
    for key,val in result["identified_header_kinds"].items():
        lines.append("- "+key+": "+str(val))
    lines+=["","## UnityFS block-directory compression (header flags only)",""]
    for key,val in result["unityfs_block_info_compression"].items():
        lines.append("- "+key+": "+str(val))
    lines+=["","## By split","",
            "| Split | Members | Expanded bytes | In-ZIP bytes |",
            "|---|---:|---:|---:|"]
    for name,r in result["by_split"].items():
        lines.append("| "+name+" | "+str(r["members"])+" | "+
          str(r["bytes_uncompressed"])+" | "+str(r["bytes_in_zip"])+" |")
    lines+=["","A ZIP member header is not enough to decode Unity's internal data blocks.",
        "This inventory does not determine why the original game displayed its warning.",
        "No original game execution, protection bypass, player telemetry or ESP verified."]
    (a.out/"original_asset_containers.md").write_text("\n".join(lines)+"\n",encoding="utf-8")
    print("\n".join(lines))

if __name__=="__main__":
    main()
