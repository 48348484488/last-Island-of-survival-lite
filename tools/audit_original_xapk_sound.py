#!/usr/bin/env python3
"""Investigate ONLY original LITE 7.3 XAPK audio resources (no synthetic claims).

Reassembles the unchanged original shards from git, lists asset packs,
parses cfg.SoundCfg.oc using Share.Octets.cs documented LE binary format,
and selectively decodes original AudioClip WAVs by cfg sound IDs 330/331
and source-proven footprint/walk/jump/surface path names if available.

Runs only under manual / separate CI job to avoid downloading 1.5 GB with
every Android APK build. Do not upload the XAPK or unrelated copyrighted data.
"""
import json
import os
import re
import shutil
import struct
import tempfile
import zipfile
from pathlib import Path

HERE=Path(__file__).resolve().parents[1]
SRC=HERE/"xapk_original"
DEST=Path(os.environ.get("LIOS_ORIGINAL_SOUND_AUDIT", "/tmp/lios-original-sound-audit"))
MAX_SAMPLE_BYTES=10_000_000
REGEX=re.compile(r"sound/|audio/|\.bank$|\.fsb$|foot|step|jump|land|walk|run|wq_m4_1",re.I)

def sound_cfg(data):
    """C# cfg.SoundCfg: int32 id, int32 UTF8 length, bytes, int32 volume/radius."""
    p=0
    entries=[]
    try:
        while p<len(data):
            if p+8>len(data):raise ValueError("truncated cfg")
            id,length=struct.unpack_from("<ii",data,p)
            p+=8
            if length<0 or length>500 or p+length+8>len(data):
                raise ValueError("invalid cfg sound row")
            name=data[p:p+length].decode("utf8")
            p+=length
            volume,radius=struct.unpack_from("<ii",data,p)
            p+=8
            if not(0<id<2_000_000 and -10<=volume<=200 and -100<=radius<=10000):
                raise ValueError("invalid cfg values")
            entries.append({"id":id,"path":name,"volume":volume,"radius":radius})
        if len(entries)<30:raise ValueError("too few source cfg sound entries")
        return entries
    except (UnicodeDecodeError,ValueError,struct.error) as exc:
        return {"parse_error":str(exc),"bytes":len(data),"prefix_hex":data[:32].hex()}

def archive_index(z,origin):
    names=z.namelist()
    clips=[x for x in names if REGEX.search(x)]
    return {"archive":origin,"entries":len(names),
            "all_audio_named":clips[:1000],
            "sound_count":sum("/sound/" in x.lower() or x.lower().startswith("sound/") for x in names),
            "nested_apks":[x for x in names if x.lower().endswith(".apk")],
            "config_paths":[x for x in names if x.endswith("cfg.SoundCfg.oc")]}

def try_extract(z,names,cfg,report):
    output=DEST/"decoded_original_audio"
    output.mkdir(exist_ok=True)
    try:
        import UnityPy
    except ImportError:
        report.setdefault("notes",[]).append("UnityPy not installed; only indexed")
        return
    exact={r["path"].lower() for r in cfg if isinstance(r,dict) and r.get("id") in (330,331)}
    terms=re.compile(r"foot|step|jump|land|walk|run|wq_m4_1",re.I)
    candidates=[]
    for name in names:
        low=name.lower()
        if not low.endswith(".ab"):continue
        stem=Path(low).stem
        if stem in exact or any(low.endswith("/"+q+".ab") for q in exact) or terms.search(stem):
            candidates.append(name)
    report.setdefault("candidate_bundles",[]).extend(candidates[:100])
    for n in candidates[:40]:
        try:
            obj_bytes=z.read(n)
            if len(obj_bytes)>32_000_000:continue
            env=UnityPy.load(obj_bytes)
            for obj in env.objects:
                if obj.type.name!="AudioClip":continue
                clip=obj.read()
                samples=clip.samples
                for name,data in (samples or {}).items():
                    if not isinstance(data,(bytes,bytearray)) or len(data)>MAX_SAMPLE_BYTES:continue
                    safe=re.sub(r"[^a-zA-Z0-9_.-]","_",Path(n).stem+"_"+name)
                    file=output/safe
                    file.write_bytes(data)
                    report.setdefault("extracted_original_samples",[]).append(
                        {"bundle":n,"sample_name":name,"file":safe,"bytes":len(data)})
        except Exception as ex:
            report.setdefault("unresolved_bundles",[]).append(
                {"file":n,"reason":str(ex)[:400]})

def main():
    DEST.mkdir(parents=True,exist_ok=True)
    parts=sorted(SRC.glob("Last_Island_LITE_7.3.xapk.part*"))
    assert len(parts)==17,("Original XAPK shard count changed; never replace source",len(parts))
    output=DEST/"reassembled.xapk"
    with output.open("wb") as out:
        for part in parts:
            with part.open("rb") as inp:
                shutil.copyfileobj(inp,out,length=8*1024*1024)
    report={"original_shards":len(parts),"original_xapk_bytes":output.stat().st_size,
            "archives":[],"sound_config":[],"notes":[]}
    assert 1_500_000_000<output.stat().st_size<1_600_000_000
    with zipfile.ZipFile(output) as outer:
        report["archives"].append(archive_index(outer,"xapk"))
        for name in outer.namelist():
            if name.endswith("cfg.SoundCfg.oc"):
                report["sound_config"]=sound_cfg(outer.read(name))
        for nested in [x for x in outer.namelist() if x.lower().endswith(".apk")]:
            # Decompress individual APK in a temporary file for seeking
            with tempfile.TemporaryDirectory() as td:
                target=Path(td)/"nested.apk"
                with outer.open(nested) as data,target.open("wb") as dest:
                    shutil.copyfileobj(data,dest,length=8*1024*1024)
                try:
                    with zipfile.ZipFile(target) as z:
                        report["archives"].append(archive_index(z,nested))
                        allnames=z.namelist()
                        for entry in allnames:
                            if entry.endswith("cfg.SoundCfg.oc"):
                                report["sound_config"]=sound_cfg(z.read(entry))
                        cfg=report["sound_config"] if isinstance(report["sound_config"],list) else []
                        try_extract(z,allnames,cfg,report)
                except zipfile.BadZipFile:
                    report["notes"].append("Nested APK is not ZIP: "+nested)
    output.unlink()  # IMPORTANT: never upload or leave original giant XAPK
    config=report["sound_config"]
    report["target_sound_cfg"]=[x for x in config if x["id"] in (330,331)] if isinstance(config,list) else []
    (DEST/"source_audio_index.json").write_text(json.dumps(report,indent=2,ensure_ascii=False))
    print("LIOS_SOURCE_AUDIO_INDEX",json.dumps({
        "outer_bytes":report["original_xapk_bytes"],
        "archives":[{"archive":x["archive"],"entries":x["entries"],
                     "sound_count":x["sound_count"],
                     "sound_examples":x["all_audio_named"][:12]} for x in report["archives"]],
        "cfg_count":len(config) if isinstance(config,list) else 0,
        "jump_sound_ids":report["target_sound_cfg"],
        "extracted_count":len(report.get("extracted_original_samples",[])),
        "errors":report.get("unresolved_bundles",[])[:6],
    },ensure_ascii=False),flush=True)

if __name__=="__main__":main()
