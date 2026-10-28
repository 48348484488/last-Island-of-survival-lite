#!/usr/bin/env python3
"""Decode the 6 remaining local HUD Sprite objects by loading exact external CABs.

The Sprite objects are embedded in assets/ui/battlepanel.ab, but their
SpriteAtlas/Texture2D dependency is stored in separate AssetBundles.
Resolve exact missing CAB names from UnityPy errors; never replace them with
similarly named images or screenshot crops.
"""
from pathlib import Path
import json,shutil,re,tempfile,zipfile,hashlib
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
ASSETS=ROOT/"last_island_recreated_game/assets/hud_original_xapk"
F=ASSETS/"complete_source_mapping/original_unity_all_hud_components.json"
P=ASSETS/"ui_prefabs_original/assets/ui/battlepanel.ab"
data=json.loads(F.read_text(encoding="utf8"))
pending={pid:meta for pid,meta in data["sprite_assets"].items()
    if not meta.get("file") and "unresolved_texture_dependency" in meta}
assert 1<=len(pending)<=10,len(pending)
wanted={}
for pid,meta in pending.items():
    c=re.search(r"cab-([0-9a-f]{32})",meta["unresolved_texture_dependency"],re.I)
    assert c,(pid,meta)
    wanted[pid]=c.group(1).lower()
all_cabs=set(wanted.values())
parts=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17
tmp=Path(tempfile.gettempdir())
xapk=tmp/"last_island_source_ui_local_sprites.xapk"
with xapk.open("wb") as dst:
    for p in parts:
        with p.open("rb") as src:shutil.copyfileobj(src,dst,length=8*1024*1024)
pack=tmp/"last_island_ui_atlas_deps.apk"
with zipfile.ZipFile(xapk) as z:
    with z.open("install_time_asset_pack.apk") as src,pack.open("wb") as dst:
        shutil.copyfileobj(src,dst,length=8*1024*1024)
xapk.unlink(missing_ok=True)
bundles={};paths=[]
with zipfile.ZipFile(pack) as z:
    for info in z.infolist():
        if not info.filename.endswith(".ab"):continue
        with z.open(info) as src:head=src.read(2048)
        matches={v.decode().lower() for v in re.findall(rb"CAB-([0-9a-f]{32})",head,re.I)}
        for id in matches&all_cabs:
            output=tmp/("source_original_hud_dependency_"+id+".ab")
            with z.open(info) as src,output.open("wb") as dst:
                shutil.copyfileobj(src,dst,length=4*1024*1024)
            paths.append(output)
            bundles[id]={"asset_pack_path":info.filename,
                "sha256":hashlib.sha256(output.read_bytes()).hexdigest()}
pack.unlink(missing_ok=True)
print("ORIGINAL_HUD_LOCAL_SPRITE_CABS",json.dumps(bundles),flush=True)
assert all_cabs<=set(bundles),{"missing":sorted(all_cabs-set(bundles))}
# Load source prefab and all exact missing asset bundles into one Unity environment,
# allowing UnityPy's PPtr to dereference the original referenced SpriteAtlas.
env=UnityPy.load(str(P),*[str(x) for x in paths])
sprites={str(o.path_id):o for o in env.objects if o.type.name=="Sprite"}
report={"sources":bundles,"sprites":[],"unresolved":[]}
for pid,meta in pending.items():
    obj=sprites.get(pid)
    if not obj:
        report["unresolved"].append({"path_id":pid,"reason":"Sprite missing from composite environment"})
        continue
    try:
        sprite=obj.read()
        im=sprite.image.convert("RGBA")
        assert sprite.m_Name==meta["name"],(pid,sprite.m_Name,meta["name"])
        output=ASSETS/"complete_source_mapping/embedded_sprites"/("source_sprite_"+pid+".png")
        output.parent.mkdir(parents=True,exist_ok=True)
        im.save(output,optimize=True)
        meta.update({"file":str(output.relative_to(ASSETS)),
          "width":im.width,"height":im.height,
          "sha256":hashlib.sha256(output.read_bytes()).hexdigest(),
          "resolved_dependency_asset_pack_path":bundles[wanted[pid]]["asset_pack_path"]})
        report["sprites"].append({"path_id":pid,"name":meta["name"],
          "file":meta["file"],"dependency_cab":wanted[pid],
          "asset_pack_path":bundles[wanted[pid]]["asset_pack_path"]})
    except Exception as ex:
        report["unresolved"].append({"path_id":pid,"name":meta["name"],
             "dependency_cab":wanted[pid],"error":str(ex)[:350]})
for c in data["components"].values():
    if c.get("type")!="Image" or c.get("resolved_sprite_file"):continue
    ptr=c.get("sprite",{})
    if int(ptr.get("file_id",0))==0 and ptr.get("path_id") in data["sprite_assets"]:
        r=data["sprite_assets"][ptr["path_id"]]
        if r.get("file"):c["resolved_sprite_file"]=r["file"]
data["summary"]["resolved_original_local_sprite_texture_dependencies"]=len(report["sprites"])
data["summary"]["remaining_unresolved_local_sprite_ids"]=len(report["unresolved"])
F.write_text(json.dumps(data,ensure_ascii=False,separators=(",",":")),encoding="utf8")
report["count"]=len(report["sprites"])
(ASSETS/"complete_source_mapping/embedded_source_dependencies.json").write_text(
    json.dumps(report,indent=2,ensure_ascii=False))
print("ORIGINAL_HUD_EMBEDDED_SPRITES_RECOVERED",json.dumps(report),flush=True)
for path in paths:path.unlink(missing_ok=True)
assert len(report["sprites"])>=1,"No embedded Sprite could be decoded using exact source CAB"
