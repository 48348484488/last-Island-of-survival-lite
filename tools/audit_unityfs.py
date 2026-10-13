#!/usr/bin/env python3
"""Read-only UnityFS field-name inventory. Hits are strings, not live method offsets."""
import argparse
from collections import Counter
import json
from pathlib import Path
import struct
import zipfile

TERMS=[b"PlayerManager",b"networkPositionObserver",b"playerNetworkSubscriptionController",
       b"AreaOfInterestController",b"NetworkIdentity",b"NetworkTransformReliable",
       b"NetworkTransformUnreliable",b"VisibilityResolver",b"WorldCamera",
       b"PlayerCamera",b"CameraController",b"isSubscribedSync",b"visRange",b"syncDirection"]
MAX_BUNDLE_BYTES=300*1024*1024
MAX_BLOCK_BYTES=128*1024*1024
MAX_INFO_BYTES=32*1024*1024

def zstr(b,p):
    end=b.find(b"\0",p)
    if end<0 or end-p>256:raise ValueError("unterminated header")
    return b[p:end].decode("utf-8","replace"),end+1

def decompress(data,size,kind):
    if size>MAX_BLOCK_BYTES:raise ValueError("oversized block")
    if kind==0:out=data
    elif kind in (2,3):
        import lz4.block
        out=lz4.block.decompress(data,uncompressed_size=size)
    elif kind==1:
        import lzma
        out=lzma.decompress(data)
    else:raise ValueError("unknown compression "+str(kind))
    if len(out)!=size:raise ValueError("decompressed size mismatch")
    return out

def inspect_bundle(bundle):
    if len(bundle)<64:raise ValueError("truncated UnityFS")
    magic,pos=zstr(bundle,0)
    if magic!="UnityFS":raise ValueError("wrong magic")
    version=struct.unpack_from(">I",bundle,pos)[0];pos+=4
    unity,pos=zstr(bundle,pos)
    revision,pos=zstr(bundle,pos)
    declared,info_c,info_u,flags=struct.unpack_from(">QIII",bundle,pos)
    pos+=20
    if declared!=len(bundle):raise ValueError("bundle size mismatch")
    if info_c>MAX_INFO_BYTES or info_u>MAX_INFO_BYTES:raise ValueError("oversized metadata")
    if flags&0x200:pos=(pos+15)&~15
    directory_at_end=bool(flags&0x80)
    info_pos=len(bundle)-info_c if directory_at_end else pos
    if info_pos<0 or info_pos+info_c>len(bundle):raise ValueError("invalid directory offset")
    info=decompress(bundle[info_pos:info_pos+info_c],info_u,flags&0x3f)
    q=16
    if q+4>len(info):raise ValueError("missing block table")
    count=struct.unpack_from(">I",info,q)[0];q+=4
    if count>100000:raise ValueError("oversized block count")
    blocks=[]
    for i in range(count):
        if q+10>len(info):raise ValueError("truncated block descriptor")
        blocks.append(struct.unpack_from(">IIH",info,q));q+=10
    stream_pos=pos if directory_at_end else info_pos+info_c
    hits=Counter();carry=b"";bytes_decoded=0
    for usize,csize,bflags in blocks:
        if stream_pos+csize>len(bundle) or (directory_at_end and stream_pos+csize>info_pos):
            raise ValueError("invalid compressed payload")
        raw=decompress(bundle[stream_pos:stream_pos+csize],usize,bflags&0x3f)
        stream_pos+=csize;bytes_decoded+=len(raw)
        combined=carry+raw;old_len=len(carry)
        for term in TERMS:
            start=0
            while True:
                at=combined.find(term,start)
                if at<0:break
                if at+len(term)>old_len:hits[term.decode()]+=1
                start=at+len(term)
        carry=combined[-96:]
    return {"unity_version":unity,"revision":revision,"format":version,
            "block_count":count,"decoded_bytes":bytes_decoded,"terms":dict(hits)}

def run(apk,out):
    entries=[];errors=[];skipped=[]
    with zipfile.ZipFile(apk) as archive:
        files=[f for f in archive.infolist() if f.filename.endswith(".bundle")]
        for i,info in enumerate(files):
            if info.file_size>MAX_BUNDLE_BYTES:
                skipped.append({"path":info.filename,"bytes":info.file_size});continue
            try:
                with archive.open(info) as stream:data=stream.read(MAX_BUNDLE_BYTES+1)
                if len(data)>MAX_BUNDLE_BYTES:raise ValueError("oversized stream")
                item=inspect_bundle(data);item["path"]=info.filename
                entries.append(item)
            except Exception as e:errors.append({"path":info.filename,"error":type(e).__name__+": "+str(e)})
            if (i+1)%30==0:print(f"Bundles {i+1}/{len(files)}",flush=True)
    total=Counter()
    for item in entries:total.update(item["terms"])
    report={"source_apk":apk.name,"bundle_count":len(files),"scanned":len(entries),
            "errors":errors,"skipped":skipped,"occurrences":dict(total),
            "matched_bundles":[e for e in entries if e["terms"]]}
    out.parent.mkdir(parents=True,exist_ok=True)
    out.write_text(json.dumps(report,ensure_ascii=False,indent=2)+"\n")
    print(json.dumps({"bundles":len(files),"scanned":len(entries),
                      "failures":len(errors),"skipped":len(skipped),"markers":dict(total)},indent=2))
    if errors or skipped:raise SystemExit(1)

if __name__=="__main__":
    p=argparse.ArgumentParser()
    p.add_argument("--assetpack",required=True,type=Path)
    p.add_argument("--out",default=Path("unityfs-output/unityfs.json"),type=Path)
    x=p.parse_args()
    run(x.assetpack,x.out)
