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
  # Export the REAL Unity prefab hierarchy, RectTransform metrics and
  # sprite PPtrs. The previous export recovered pixels but left every
  # native menu coordinate as a manually guessed number.
  original_go={}
  original_rects={}
  original_components={}
  for obj in ui.objects:
   kind=obj.type.name
   if kind not in ("GameObject","RectTransform","MonoBehaviour"):
    continue
   try:
    data=obj.read_typetree()
   except Exception:
    continue
   key=str(obj.path_id)
   if kind=="GameObject":
    original_go[key]={
     "name":data.get("m_Name",""),
     "active":data.get("m_IsActive",0),
     "components":[str(x.get("component",{}).get("m_PathID",0))
                   for x in data.get("m_Component",[])]}
   elif kind=="RectTransform":
    original_rects[key]={
     "gameObject":str(data.get("m_GameObject",{}).get("m_PathID",0)),
     "parent":str(data.get("m_Father",{}).get("m_PathID",0)),
     "children":[str(x.get("m_PathID",0))
                 for x in data.get("m_Children",[])],
     "anchorMin":data.get("m_AnchorMin"),
     "anchorMax":data.get("m_AnchorMax"),
     "anchoredPosition":data.get("m_AnchoredPosition"),
     "sizeDelta":data.get("m_SizeDelta"),
     "pivot":data.get("m_Pivot"),
     "localScale":data.get("m_LocalScale"),
     "localRotation":data.get("m_LocalRotation")}
   elif kind=="MonoBehaviour":
    fields=("m_Sprite","m_Text","m_Color","m_Enabled","m_RaycastTarget",
            "m_PreserveAspect","m_Type","m_FillAmount","m_OnClick")
    present={k:data[k] for k in fields if k in data}
    if present:
     original_components[key]={
       "gameObject":str(data.get("m_GameObject",{}).get("m_PathID",0)),
       "fields":present}
  by_name={}
  for goid,go in original_go.items():
   if go["name"]:
    by_name.setdefault(go["name"],[]).append(goid)
  required=("m_produce_page","m_produce_list","btn_create",
            "btn_reduce","btn_max","btn_back","scp_items")
  missing_nodes=[n for n in required if n not in by_name]
  if missing_nodes:
   raise RuntimeError("Original craft prefab nodes unresolved: "+repr(missing_nodes))
  evidence={
   "source":"LITE 7.3 assets/ui/bagandbuildpanel.ab",
   "confidence":"serialized_unity_prefab_not_runtime_pixels",
   "counts":{"gameObjects":len(original_go),"rectTransforms":len(original_rects),
             "components":len(original_components)},
   "byName":by_name,"gameObjects":original_go,
   "rectTransforms":original_rects,"uiComponents":original_components,
   "unresolved_runtime":["CanvasScaler viewport adaptation",
                         "active-state transitions","localization font",
                         "DrawingCfg recipe data","scroll layout and dynamic cells"]}
  (OUT/"original_craft_prefab_graph.json").write_text(
    json.dumps(evidence,ensure_ascii=False,indent=2,default=str))
  print("LIOS_ORIGINAL_CRAFT_SOURCE_LAYOUT_PASS",
        "nodes",len(original_go),"rects",len(original_rects),
        "components",len(original_components),"required",len(required),flush=True)
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
  # Unity main category On/Off sprites use fileID=8 in the ORIGINAL
  # bagandbuildpanel serialized file, not the two earlier guessed atlases.
  # Resolve FileID -> exact CAB GUID, then locate its genuine AssetBundle
  # within the same unmodified installation pack. No image-name guessing.
  unresolved={pid for pid in used if not any(a["pathid"]==pid for a in assets)}
  original_file=next(iter(ui.objects)).assets_file
  cab_by_fileid={}
  for fileid,external in enumerate(original_file.externals,1):
   match=re.search(r"CAB-([0-9a-f]{32})",str(external),re.I)
   if match:cab_by_fileid[fileid]=match.group(1).lower()
  missing_by_cab={}
  for pid in unresolved:
   cab=cab_by_fileid.get(used[pid])
   if cab:missing_by_cab.setdefault(cab,set()).add(pid)
  candidates={}
  if missing_by_cab:
   for info in z.infolist():
    if not info.filename.endswith(".ab"):continue
    with z.open(info) as inp:
     prefix=inp.read(2048)
    for match in re.finditer(rb"CAB-([0-9a-f]{32})",prefix,re.I):
     cab=match.group(1).decode().lower()
     if cab in missing_by_cab:candidates.setdefault(cab,[]).append(info.filename)
  print("LIOS_CRAFT_EXTERNAL_ATLAS_CABS",json.dumps({
    cab:{"needed":len(ids),"bundles":candidates.get(cab,[])}
    for cab,ids in missing_by_cab.items()}),flush=True)
  for cab,ids in missing_by_cab.items():
   for path in candidates.get(cab,[]):
    source=t/"craft_external_exact.ab"
    with z.open(path) as src,source.open("wb") as dst:
     shutil.copyfileobj(src,dst,8*1024*1024)
    try:
     env=UnityPy.load(str(source))
     objs={str(obj.path_id):obj for obj in env.objects
           if obj.type.name=="Sprite"}
     for ident in sorted(ids):
      obj=objs.get(ident)
      if not obj or any(x["pathid"]==ident for x in assets):continue
      try:
       sprite=obj.read()
       img=sprite.image.convert("RGBA")
       clean=re.sub(r"[^a-zA-Z0-9_-]","_",str(sprite.m_Name))[:48]
       output=f"craft_{ident}_{clean}.png"
       img.save(OUT/output,optimize=True)
       assets.append({"pathid":ident,"name":str(sprite.m_Name),
          "file":output,"source":path,"source_cab":cab,
          "size":list(img.size),"sha256":hashlib.sha256(img.tobytes()).hexdigest(),
          "file_id":used[ident]})
       missing.pop(ident,None)
      except Exception as exc:
       missing[ident]=str(exc)[:170]
    except Exception as exc:
     print("LIOS_CRAFT_EXTERNAL_ATLAS_READ_FAIL",path,str(exc)[:250],flush=True)
    finally:
     source.unlink(missing_ok=True)
  print("LIOS_CRAFT_EXTERNAL_SPRITES_RECOVERED",
        len([a for a in assets if "source_cab" in a]),flush=True)
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
