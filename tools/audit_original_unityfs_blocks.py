#!/usr/bin/env python3
"""Bounded, read-only UnityFS block-directory decoding and literal asset scan.

Only scans *ordinary decompressed resource bytes* within authenticated,
unchanged Oxide APK ZIP members; does not patch/repack APKs or inspect runtime
anti-cheat paths. A missing plaintext match DOES NOT prove absence of a
generated/encrypted/remote message. Every unsupported or skipped block is
recorded; partial coverage is never called complete.

UnityFS header/block table layouts follow the public UnityFS formats used by
AssetStudio/UnityPy. LZ4 and LZ4HC use the same raw LZ4 decompressor.
"""
import argparse
from collections import Counter
import json
from pathlib import Path
import re
import struct
import zipfile

from inventory_original_asset_containers import parse_unityfs_header
from scan_original_warning_literals import patterns
from verify_original_splits import EXPECTED_SHA256, sha256_file

MAX_DIR_BYTES=32 * 1024 * 1024
MAX_BLOCK_BYTES=16 * 1024 * 1024
MAX_BUNDLE_BYTES=512 * 1024 * 1024
MAX_TOTAL_BYTES=3 * 1024 * 1024 * 1024
MAX_BLOCKS=65536
MAX_NODES=65536
MAX_MATCHES_PER_PATTERN=12
ALIGNMENT=16
NEEDLES=patterns()
TAIL_BYTES=max(len(needle) for _term,_enc,needle in NEEDLES)-1

def align(value,step=ALIGNMENT):
    return (value+step-1)//step*step

def read_exact(stream,size):
    value=stream.read(size)
    if len(value)!=size:
        raise ValueError("TRUNCATED")
    return value

def decode_content(payload,expected,codec):
    if expected<0 or expected>MAX_BLOCK_BYTES:
        raise ValueError("BLOCK_DECOMPRESSION_LIMIT")
    if codec==0:
        data=payload
    elif codec in (2,3):
        import lz4.block
        data=lz4.block.decompress(payload,uncompressed_size=expected)
    else:
        raise ValueError("UNSUPPORTED_CODEC_"+str(codec))
    if len(data)!=expected:
        raise ValueError("UNCOMPRESSED_SIZE_MISMATCH")
    return data

def get_reader_bounds(stream,member_size):
    stream.seek(0)
    header=parse_unityfs_header(read_exact(stream,min(512,member_size)))
    if not header or header.get("status")!="HEADER_READ":
        raise ValueError("UNRECOGNIZED_UNITYFS_HEADER")
    if not 6 <= header["bundle_format_version"] <= 9:
        raise ValueError("UNSUPPORTED_UNITYFS_VERSION")
    total=header["bundle_declared_size"]
    if total>member_size or total<64:
        raise ValueError("INVALID_UNITYFS_DECLARED_SIZE")
    pos=header["header_size"]
    version=header["unity_revision"]
    # UnityPy documents version>=7 or Unity 2019.4.15+ header alignment
    m=re.match(r"(\d+)\.(\d+)\.(\d+)",version)
    unity=tuple(map(int,m.groups())) if m else (0,0,0)
    aligned=header["bundle_format_version"]>=7 or unity>=(2019,4,15)
    if aligned:
        pos=align(pos)
    compressed=header["blocks_info_compressed_bytes"]
    unpacked=header["blocks_info_uncompressed_bytes"]
    if not 0<compressed<=MAX_DIR_BYTES or not 0<unpacked<=MAX_DIR_BYTES:
        raise ValueError("BLOCK_DIRECTORY_SIZE_LIMIT")
    at_end=header["blocks_info_at_file_end"]
    directory_offset=total-compressed if at_end else pos
    data_offset=pos if at_end else pos+compressed
    if header["raw_flags"] & 0x200:
        data_offset=align(data_offset)
    if directory_offset<pos or directory_offset+compressed>total:
        raise ValueError("BLOCK_DIRECTORY_OUT_OF_BOUNDS")
    stream.seek(directory_offset)
    directory_compressed=read_exact(stream,compressed)
    codec=header["raw_flags"] & 0x3f
    directory=decode_content(directory_compressed,unpacked,codec)
    return header,directory,data_offset, (directory_offset if at_end else total)

def parse_directory(directory):
    # 16 hash bytes, big-endian signed block count, fixed 10-byte block rows,
    # signed node count, 20-byte node prefixes and NUL-terminated names.
    if len(directory)<24:raise ValueError("DIRECTORY_TOO_SHORT")
    count=struct.unpack_from(">I",directory,16)[0]
    if count>MAX_BLOCKS or count*10>len(directory)-20:
        raise ValueError("INVALID_BLOCK_COUNT")
    blocks=[];pos=20
    for _ in range(count):
        unpacked,packed,flags=struct.unpack_from(">IIH",directory,pos)
        pos+=10
        blocks.append({"uncompressed":unpacked,"compressed":packed,
                       "flags":flags,"codec":flags & 0x3f})
    if pos+4>len(directory):raise ValueError("DIRECTORY_TRUNCATED_NODES")
    nodes=struct.unpack_from(">I",directory,pos)[0];pos+=4
    if nodes>MAX_NODES:raise ValueError("NODE_COUNT_LIMIT")
    sizes=[]
    for _ in range(nodes):
        if pos+20>len(directory):raise ValueError("DIRECTORY_TRUNCATED_NODE")
        offset,size,flags=struct.unpack_from(">QQI",directory,pos)
        pos+=20
        name_end=directory.find(b"\x00",pos,min(len(directory),pos+4096))
        if name_end<0:raise ValueError("DIRECTORY_NODE_NAME_LIMIT")
        pos=name_end+1
        sizes.append((offset,size,flags))
    total_unpacked=sum(x["uncompressed"] for x in blocks)
    # Directory sizes should fit decoded concatenated block address space.
    if any(offset>total_unpacked or size>total_unpacked-offset
           for offset,size,_flags in sizes):
        raise ValueError("DIRECTORY_NODE_OUT_OF_RANGE")
    return blocks,nodes,total_unpacked

def scan_chunk(block,position,tail,matches):
    combined=tail+block
    base=position-len(tail)
    for term,enc,needle in NEEDLES:
        key=term+" ["+enc+"]"
        entries=matches.setdefault(key,[])
        next_pos=0
        while len(entries)<MAX_MATCHES_PER_PATTERN:
            found=combined.find(needle,next_pos)
            if found<0:break
            offset=base+found
            if offset>=0 and (not entries or entries[-1]!=offset):
                entries.append(offset)
            next_pos=found+1
    return combined[-TAIL_BYTES:]

def scan_bundle(stream,member_size,budget):
    h,directory,start,compressed_data_end=get_reader_bounds(stream,member_size)
    blocks,nodes,total_unpacked=parse_directory(directory)
    if total_unpacked>MAX_BUNDLE_BYTES:
        raise ValueError("BUNDLE_DECOMPRESSION_LIMIT")
    if total_unpacked>budget:
        raise ValueError("GLOBAL_DECOMPRESSION_LIMIT")
    packed_sum=sum(x["compressed"] for x in blocks)
    if start+packed_sum>compressed_data_end:
        raise ValueError("DATA_BLOCKS_OUT_OF_BOUNDS")
    meta={
        "bundle_version":h["bundle_format_version"],
        "unity_revision":h["unity_revision"],
        "index_compression":h["blocks_info_compression"],
        "block_count":len(blocks),"node_count":nodes,
        "data_block_codecs":dict(Counter(
            {0:"NONE",1:"LZMA",2:"LZ4",3:"LZ4HC"}.get(x["codec"],"UNKNOWN") for x in blocks)),
        "uncompressed_block_bytes":total_unpacked,
        "compressed_block_bytes":packed_sum,
        "data_scan_complete":False,
    }
    # Stop with a categorized, not thrown-away, partial summary on blocks
    # incompatible with the intentionally limited reader.
    tail=b"";position=0;matches={}
    stream.seek(start)
    for idx,block in enumerate(blocks):
        if block["uncompressed"]>MAX_BLOCK_BYTES or block["compressed"]>MAX_BLOCK_BYTES:
            meta["stop_reason"]="OVERSIZED_BLOCK"
            meta["stopped_at_block"]=idx
            break
        if block["codec"] not in (0,2,3):
            meta["stop_reason"]="UNSUPPORTED_DATA_BLOCK_CODEC_"+str(block["codec"])
            meta["stopped_at_block"]=idx
            break
        try:
            packed=read_exact(stream,block["compressed"])
            raw=decode_content(packed,block["uncompressed"],block["codec"])
        except (ValueError,RuntimeError) as error:
            meta["stop_reason"]="DATA_BLOCK_ERROR_"+str(error)[:75]
            meta["stopped_at_block"]=idx
            break
        tail=scan_chunk(raw,position,tail,matches)
        position+=len(raw)
    else:
        meta["data_scan_complete"]=True
    meta["data_bytes_scanned"]=position
    meta["pattern_offsets"]={key:positions for key,positions in matches.items() if positions}
    return meta

def audit(apks,expected=None):
    expected=expected or EXPECTED_SHA256
    checked={};bundles=[];totals=Counter();skips=Counter()
    remaining=MAX_TOTAL_BYTES
    for path in apks:
        if path.name not in expected:raise ValueError("UNKNOWN_ORIGINAL_APK")
        digest=sha256_file(path)
        if digest!=expected[path.name]:raise ValueError("ORIGINAL_APK_SHA256_MISMATCH")
        checked[path.name]=digest
        with zipfile.ZipFile(path) as z:
            for member in z.infolist():
                if member.is_dir():continue
                with z.open(member) as s:
                    header=s.read(8)
                    if header!=b"UnityFS\x00":continue
                    try:
                        result=scan_bundle(s,member.file_size,remaining)
                        remaining-=result["data_bytes_scanned"]
                        totals["data_decoded_bytes"]+=result["data_bytes_scanned"]
                        totals["blocks_discovered"]+=result["block_count"]
                        totals["nodes_discovered"]+=result["node_count"]
                        if result["data_scan_complete"]:
                            totals["complete_bundle_scans"]+=1
                        else:
                            skips[result.get("stop_reason","INCOMPLETE")]+=1
                    except (ValueError,EOFError,MemoryError) as error:
                        result={"data_scan_complete":False,"data_bytes_scanned":0,
                                "stop_reason":str(error)[:90]}
                        skips[result["stop_reason"]]+=1
                    result["apk"]=path.name
                    result["entry"]=member.filename
                    bundles.append(result)
    return {
        "result":"UNITYFS_READ_ONLY_BOUNDED_SCAN",
        "verified_original_apks":checked,
        "all_original_apk_splits":set(checked)==set(EXPECTED_SHA256),
        "unityfs_bundles_detected":len(bundles),
        "complete_bundle_scans":totals["complete_bundle_scans"],
        "incomplete_bundle_scans":len(bundles)-totals["complete_bundle_scans"],
        "blocks_discovered":totals["blocks_discovered"],
        "nodes_discovered":totals["nodes_discovered"],
        "data_decoded_bytes":totals["data_decoded_bytes"],
        "limits":{"max_index_bytes":MAX_DIR_BYTES,"max_data_block_bytes":MAX_BLOCK_BYTES,
                  "max_bundle_bytes":MAX_BUNDLE_BYTES,"max_total_bytes":MAX_TOTAL_BYTES},
        "reasons_incomplete":dict(skips),
        "literal_matches":[
            {"apk":b["apk"],"entry":b["entry"],"pattern_offsets":b["pattern_offsets"]}
            for b in bundles if b.get("pattern_offsets")
        ],
        "bundle_metadata":bundles,
        "security_rejection_trigger":"UNKNOWN",
        "game_modified":False,
        "playable_game_or_real_player_esp_verified":False,
        "limitations":[
            "This audits decoded UnityFS raw data blocks, not encrypted payloads or object-level managed code strings.",
            "Unsupported, oversized, malformed or unscanned bundles are explicitly counted; no false full-coverage claims.",
            "A raw byte match does not prove what security decision produced a runtime Toast.",
            "No game APK, integrity check, emulator identity or protection was modified.",
        ],
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("apks",nargs="+",type=Path)
    p.add_argument("--out",required=True,type=Path)
    a=p.parse_args()
    result=audit(a.apks)
    a.out.mkdir(parents=True,exist_ok=True)
    (a.out/"unityfs_internal_scan.json").write_text(
        json.dumps(result,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
    lines=[
        "# Oxide original native UnityFS resource block audit",
        "",
        "Original APK SHA256 verified: "+str(len(result["verified_original_apks"])),
        "UnityFS bundles found: "+str(result["unityfs_bundles_detected"]),
        "UnityFS bundles scanned fully: "+str(result["complete_bundle_scans"]),
        "UnityFS bundles incomplete: "+str(result["incomplete_bundle_scans"]),
        "UnityFS decoded data bytes scanned: "+str(result["data_decoded_bytes"]),
        "Blocks discovered: "+str(result["blocks_discovered"]),
        "Asset nodes discovered: "+str(result["nodes_discovered"]),
        "Exact warning pattern matches: "+str(len(result["literal_matches"])),
        "", "## Incomplete bundles by reason","",
    ]
    for key,count in result["reasons_incomplete"].items():
        lines.append("- "+key+": "+str(count))
    lines+=["","No original APK changes, security rejection cause identification, gameplay or live ESP.",
            "Zero matches cannot exclude dynamic, encrypted, object-encoded or remote strings."]
    (a.out/"unityfs_internal_scan.md").write_text("\n".join(lines)+"\n",encoding="utf-8")
    print("\n".join(lines))

if __name__=="__main__":
    main()
