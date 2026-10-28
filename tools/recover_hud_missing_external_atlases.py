#!/usr/bin/env python3
"""Recover unresolved HUD sprites from the exact external CAB dependencies in ORIGINAL XAPK.

No guessing atlas names: Image.m_Sprite (m_FileID,m_PathID) in
assets/ui/battlepanel.ab points at its SerializedFile.externals. Match each
CAB GUID to original install_time_asset_pack.apk AssetBundle header, then
decode Sprite images directly from matching original AssetBundle objects.
"""
import hashlib,json,re,shutil,tempfile,zipfile
from collections import Counter,defaultdict
from pathlib import Path
import UnityPy

R=Path(__file__).resolve().parents[1]
A=R/"last_island_recreated_game/assets/hud_original_xapk"
P=A/"ui_prefabs_original/assets/ui/battlepanel.ab"
G=A/"complete_source_mapping/original_unity_all_hud_components.json"
OUT=A/"complete_source_mapping"
PARTS=sorted((R/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(PARTS)==17,len(PARTS)
assert P.is_file() and G.is_file()
graph=json.loads(G.read_text(encoding="utf8"))
env=UnityPy.load(str(P))
source=next(iter(env.objects)).assets_file
externals=source.externals
cab_by_id={}
for i,item in enumerate(externals,1):
    m=re.search(r"CAB-([0-9a-f]{32})",str(item),flags=re.I)
    if m:cab_by_id[i]=m.group(1).lower()
targets=defaultdict(set)
for comp in graph["components"].values():
    if comp.get("type")!="Image":continue
    if comp.get("resolved_sprite_file"):continue
    ptr=comp.get("sprite",{})
    fid=int(ptr.get("file_id",0));pid=str(ptr.get("path_id","0"))
    if fid>0 and pid!="0" and fid in cab_by_id:
        targets[fid].add(pid)
print("SOURCE_HUD_MISSING_CAB_IDS",json.dumps({str(fid):{"cab":cab_by_id[fid],"target_sprite_ids":len(v)}
    for fid,v in targets.items()}),flush=True)
if not targets:raise RuntimeError("No original external missing Sprite dependencies found")

temp=Path(tempfile.gettempdir())
xapk=temp/"original_hud_missing_assets_rebuilt.xapk"
with xapk.open("wb") as dst:
    for part in PARTS:
        with part.open("rb") as src:shutil.copyfileobj(src,dst,length=8*1024*1024)
apk=temp/"original_hud_install_time_pack.apk"
with zipfile.ZipFile(xapk) as z:
    with z.open("install_time_asset_pack.apk") as src,apk.open("wb") as dst:
        shutil.copyfileobj(src,dst,length=8*1024*1024)
xapk.unlink(missing_ok=True)
wanted=set(cab_by_id[fid] for fid in targets)
cab_index=defaultdict(list)
with zipfile.ZipFile(apk) as z:
    entries=[i for i in z.infolist() if i.filename.endswith(".ab")]
    print("ASSET_PACK_CAB_SCAN_BEGIN",len(entries),flush=True)
    for info in entries:
        with z.open(info) as fp:
            start=fp.read(2048)
        found=re.findall(rb"CAB-([0-9a-f]{32})",start,re.I)
        for match in found:
            cab=match.decode().lower()
            if cab in wanted:cab_index[cab].append(info.filename)
    print("ORIGINAL_XAPK_MATCHED_EXTERNAL_CABS",json.dumps(cab_index),flush=True)
    result={"source":"Last Island LITE 7.3 XAPK / install_time_asset_pack.apk",
      "prefab":"assets/ui/battlepanel.ab","external_dependency_guids_by_file_id":cab_by_id,
      "matching_bundles":dict(cab_index),"sprites":[],"unresolved":[],"errors":[]}
    for fid,needed in targets.items():
        cab=cab_by_id[fid]
        matches=cab_index.get(cab,[])
        if not matches:
            for pid in needed:result["unresolved"].append({"file_id":fid,"path_id":pid,"reason":"CAB bundle not found in XAPK"})
            continue
        for path in matches:
            # Extract only the matching exact source AssetBundle; avoid writing all 5104.
            tmp=temp/"hud_exact_cab_target.ab"
            with z.open(path) as inp,tmp.open("wb") as out:
                shutil.copyfileobj(inp,out,length=4*1024*1024)
            bundle_hash=hashlib.sha256(tmp.read_bytes()).hexdigest()
            try:source_bundle=UnityPy.load(str(tmp))
            except Exception as ex:
                result["errors"].append({"bundle":path,"error":str(ex)[:200]})
                continue
            objects={str(obj.path_id):obj for obj in source_bundle.objects
                     if obj.type.name=="Sprite"}
            for pid in sorted(needed):
                obj=objects.get(pid)
                if obj is None:continue
                try:
                    sprite=obj.read()
                    image=sprite.image.convert("RGBA")
                    original_name=str(sprite.m_Name)
                    outfile=OUT/"external_sprites"/("file"+str(fid))/("sprite_"+pid+".png")
                    outfile.parent.mkdir(parents=True,exist_ok=True)
                    image.save(outfile,optimize=True)
                    result["sprites"].append({
                      "file_id":fid,"path_id":pid,"name":original_name,
                      "bundle":path,"cab":cab,
                      "file":str(outfile.relative_to(A)),
                      "width":image.width,"height":image.height,
                      "source_bundle_sha256":bundle_hash,
                      "png_sha256":hashlib.sha256(outfile.read_bytes()).hexdigest()})
                except Exception as ex:
                    result["errors"].append({"file_id":fid,"path_id":pid,
                         "bundle":path,"error":str(ex)[:270]})
            tmp.unlink(missing_ok=True)
result_ids={(int(s["file_id"]),s["path_id"]) for s in result["sprites"]}
for fid,needed in targets.items():
    for pid in needed:
        if (fid,pid) not in result_ids and not any(x["file_id"]==fid and x["path_id"]==pid for x in result["unresolved"]):
            result["unresolved"].append({"file_id":fid,"path_id":pid,
                "reason":"Sprite absent or source Texture2D dependency unresolved in exact matching CAB"})
# Merge exact recovered Sprite pointers into existing source component graph.
recovered={(int(s["file_id"]),s["path_id"]):s for s in result["sprites"]}
for component in graph["components"].values():
    if component.get("type")!="Image" or component.get("resolved_sprite_file"):continue
    p=component.get("sprite",{})
    k=(int(p.get("file_id",0)),str(p.get("path_id","0")))
    if k in recovered:
        component["resolved_sprite_file"]=recovered[k]["file"]
        component["source_external_bundle"]=recovered[k]["bundle"]
graph["unresolved_sprite_references"]=[v for v in graph["unresolved_sprite_references"]
    if (int(v["pointer"].get("file_id",0)),str(v["pointer"].get("path_id","0"))) not in recovered]
graph["summary"]["recovered_original_external_sprite_files"]=len(result["sprites"])
graph["summary"]["remaining_unresolved_external_links"]=len(graph["unresolved_sprite_references"])
G.write_text(json.dumps(graph,ensure_ascii=False,separators=(",",":")),encoding="utf8")
result["summary"]={"original_external_sprite_refs":sum(len(v) for v in targets.values()),
 "decoded_exact_external_sprites":len(result["sprites"]),
 "unresolved_exact_sprites":len(result["unresolved"]),
 "matching_original_asset_bundles":len(set(x["bundle"] for x in result["sprites"]))}
(OUT/"external_hud_sprites_index.json").write_text(json.dumps(result,indent=2,ensure_ascii=False),encoding="utf8")
apk.unlink(missing_ok=True)
print("HUD_ORIGINAL_MISSING_SPRITES_RECOVERY",json.dumps(result["summary"],ensure_ascii=False),flush=True)
assert len(result["sprites"])>0,"No original missing images recovered; CAB mapping needs investigation"
