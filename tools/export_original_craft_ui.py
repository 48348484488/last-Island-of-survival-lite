#!/usr/bin/env python3
"""Recover ORIGINAL Unity craft/inventory UI Sprite images, indexed by PPtr PathID.

Uses original 7.3 installation XAPK: no manually redrawn textures.
Outputs only sourced icon pixels and source JSON mapping. The source prefab
alone does not guarantee runtime canvas scaling or scroll/list layout.
"""
import json,zipfile,shutil,tempfile,re,io,hashlib
from pathlib import Path
from collections import Counter
import UnityPy
ROOT=Path(__file__).resolve().parents[1]
OUT=Path("exported_original_craft_ui")
OUT.mkdir(exist_ok=True)
parts=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17
with tempfile.TemporaryDirectory() as temp:
 t=Path(temp)
 joined=t/"original.xapk"
 with joined.open("wb") as dest:
  for part in parts:
   with part.open("rb") as f:shutil.copyfileobj(f,dest,8*1024*1024)
 with zipfile.ZipFile(joined) as xapk:
  with xapk.open("install_time_asset_pack.apk") as f,(t/"pack.apk").open("wb") as dest:
   shutil.copyfileobj(f,dest,8*1024*1024)
 with zipfile.ZipFile(t/"pack.apk") as z:
  ui=UnityPy.load(z.read("assets/ui/bagandbuildpanel.ab"))
  used={}
  for obj in ui.objects:
   if obj.type.name!="MonoBehaviour":continue
   try:
    data=obj.read_typetree()
    v=data.get("m_Sprite")
    if v and int(v.get("m_PathID",0)):
     used[str(v["m_PathID"])]=int(v.get("m_FileID",0))
   except Exception:continue
  # Atlas of same name and common atlas are the exact serialized dependencies.
  atlas_names=["assets/uiatlas/bagandbuildpanel.ab","assets/uiatlas/common.ab"]
  assets=[]
  missing={}
  for path in atlas_names+["assets/ui/bagandbuildpanel.ab"]:
   env=UnityPy.load(z.read(path))
   count=0
   for obj in env.objects:
    if obj.type.name!="Sprite":continue
    ident=str(obj.path_id)
    if ident not in used:continue
    try:
     sprite=obj.read()
     image=sprite.image.convert("RGBA")
     clean=re.sub(r"[^a-zA-Z0-9_-]","_",str(sprite.m_Name))[:48]
     output=f"craft_{ident}_{clean}.png"
     dest=OUT/output
     image.save(dest,optimize=True)
     assets.append(dict(pathid=ident,name=str(sprite.m_Name),file=output,source=path,
                        size=list(image.size),sha256=hashlib.sha256(image.tobytes()).hexdigest(),
                        file_id=used[ident]))
     count+=1
    except Exception as exc:
     missing[ident]=str(exc)[:170]
   print("LIOS_SOURCE_CRAFT_ATLAS",path,"matched",count,flush=True)
  for pathid in used:
   if not any(a["pathid"]==pathid for a in assets):
    missing.setdefault(pathid,"not in selected source atlas dependencies")
  result={"source":"original LITE 7.3 XAPK assets/ui/bagandbuildpanel.ab",
          "matching_atlases":atlas_names,
          "referenced_source_sprites":len(used),
          "recovered":len(assets),"images":assets,
          "missing":missing}
  (OUT/"craft_source_manifest.json").write_text(json.dumps(result,ensure_ascii=False,indent=2))
  print("LIOS_ORIGINAL_CRAFT_SPRITES_EXPORTED",len(assets),"missing",len(missing),flush=True)
  for name,ident in [("create","-6604354997382583515"),("back","-8671269052031048657"),
                     ("item_slot","2730909933824249881")]:
   matches=[a for a in assets if a["pathid"]==ident]
   print("LIOS_CRAFT_SOURCE_BUTTON",name,json.dumps(matches,ensure_ascii=False),flush=True)
  if len(assets)<10:raise RuntimeError("Fewer than 10 source UI sprites resolved; atlas mapping incomplete")
