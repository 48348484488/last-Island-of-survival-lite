#!/usr/bin/env python3
"""Identify original Unity HUD font ASSET by exact external CAB + Font PPtr.

Only original font NAME, PPtr metadata and verification hashes are retained.
No font bytes are exported, committed, printed or shared.
"""
import json,hashlib,re,shutil,tempfile,zipfile
from pathlib import Path
import UnityPy
ROOT=Path(__file__).resolve().parents[1]
A=ROOT/"last_island_recreated_game/assets/hud_original_xapk"
META=A/"complete_source_mapping/original_unity_text_font_data.json"
UI=A/"ui_prefabs_original/assets/ui/battlepanel.ab"
record=json.loads(META.read_text(encoding="utf8"))
refs={(v["font"]["file_id"],v["font"]["path_id"])
      for v in record["entries"].values() if v["font"]["path_id"]!="0"}
assert len(refs)<=2,refs
env=UnityPy.load(str(UI))
externals=next(iter(env.objects)).assets_file.externals
targets={}
for fid,pid in refs:
    if fid==0:continue
    cab=re.search(r"CAB-([0-9a-f]{32})",str(externals[fid-1]),re.I)
    assert cab,(fid,externals[fid-1])
    targets[(fid,pid)]=cab.group(1).lower()
assert targets
parts=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17
tmp=Path(tempfile.gettempdir())
xapk=tmp/"last_island_font_pointer_scan.xapk"
with xapk.open("wb") as dst:
    for p in parts:
        with p.open("rb") as src:shutil.copyfileobj(src,dst,length=8*1024*1024)
pack=tmp/"last_island_font_install_pack.apk"
with zipfile.ZipFile(xapk) as z:
    with z.open("install_time_asset_pack.apk") as src,pack.open("wb") as dst:
        shutil.copyfileobj(src,dst,length=8*1024*1024)
xapk.unlink(missing_ok=True)
results=[]
with zipfile.ZipFile(pack) as z:
    for entry in z.infolist():
        if not entry.filename.endswith(".ab"):continue
        with z.open(entry) as inp:header=inp.read(2048)
        found=set(x.decode().lower() for x in re.findall(rb"CAB-([0-9a-f]{32})",header,re.I))
        for (fid,pid),cab in targets.items():
            if cab not in found:continue
            dest=tmp/"original_font_for_inspection.ab"
            with z.open(entry) as inp,dest.open("wb") as dst:
                shutil.copyfileobj(inp,dst,length=4*1024*1024)
            source=UnityPy.load(str(dest))
            matching=[o for o in source.objects if str(o.path_id)==pid]
            assert len(matching)==1,(entry.filename,fid,pid)
            obj=matching[0]
            item=obj.read_typetree()
            name=item.get("m_Name","")
            raw_font_data=item.get("m_FontData",b"")
            length=len(raw_font_data) if isinstance(raw_font_data,(bytes,bytearray,list)) else 0
            results.append({"file_id":fid,"path_id":pid,
                "asset_pack_bundle":entry.filename,
                "cab":cab,"unity_class":obj.type.name,
                "original_font_name":name,
                "serialized_embedded_font_byte_length":length,
                "bundle_sha256":hashlib.sha256(dest.read_bytes()).hexdigest()})
            dest.unlink(missing_ok=True)
pack.unlink(missing_ok=True)
assert len(results)==len(targets),("Original source Font references unresolved",results,targets)
out={"source":"Exact Unity original Font PPtr from battlepanel.ab Text.m_FontData",
     "resolved_original_font_references":results,
     "font_binary_exported":False,
     "limitation":"Identification is exact; glyph rasterization through Unity's font engine is not reproduced by the Pillow preview."}
target=A/"complete_source_mapping/original_font_identity_NO_FONT_FILE.json"
target.write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding="utf8")
print("ORIGINAL_UNITY_FONT_IDENTITY",json.dumps(out,ensure_ascii=False),flush=True)
