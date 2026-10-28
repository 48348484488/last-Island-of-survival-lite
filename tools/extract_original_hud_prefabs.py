#!/usr/bin/env python3
"""Extract original HUD UI Prefabs preserving asset-pack paths; avoid atlas/prefab name collision.
Actual source: xapk_original/*part -> install_time_asset_pack.apk -> assets/ui/battlepanel.ab.
"""
from pathlib import Path
import tempfile,zipfile,shutil,hashlib,json,collections
import UnityPy
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"hud_original_prefab_fonte"
OUT.mkdir(exist_ok=True)
PARTS=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
PREFABS=[
 "assets/ui/battlepanel.ab","assets/ui/battlecustompanel.ab",
 "assets/ui/battleselfaimpanel.ab","assets/ui/battlemappanel.ab",
 "assets/ui/battlemapsettingpanel.ab","assets/ui/battlecarpanel.ab",
 "assets/ui/battleloadingpanel.ab","assets/ui/battleinfopaneltwopanel.ab",
 "assets/uiatlas/battlepanel.ab","assets/uiatlas/battlepanel2.ab",
]
def ptr(d):
 if isinstance(d,dict):return str(d.get("m_PathID",0))
 return str(getattr(d,"path_id",0))
def reduce(v):
 if isinstance(v,dict):return {k:reduce(x) for k,x in v.items() if not k.startswith("m_Stream")}
 if isinstance(v,list):return [reduce(x) for x in v[:20]]
 if isinstance(v,(int,float,str,bool)) or v is None:return v
 return str(v)
report={"source":"Last Island LITE 7.3 original XAPK, install_time_asset_pack.apk",
        "original_prefabs":[],"gameobjects":[],"recttransforms":[],"canvas_components":[],"source_sprites":[],"errors":[]}
xapk=Path(tempfile.gettempdir())/"source_xapk_for_ui_prefab.xapk"
with xapk.open("wb") as dst:
 for p in PARTS:
  with p.open("rb") as f:shutil.copyfileobj(f,dst,length=8*1024*1024)
with zipfile.ZipFile(xapk) as zipx:
 i=zipx.getinfo("install_time_asset_pack.apk")
 apk=Path(tempfile.gettempdir())/"original_ui_asset_pack.apk"
 with zipx.open(i) as f,apk.open("wb") as w:
  shutil.copyfileobj(f,w,length=8*1024*1024)
xapk.unlink(missing_ok=True)
with zipfile.ZipFile(apk) as z:
 for name in PREFABS:
  if name not in z.namelist():
   report["errors"].append({"missing":name});continue
  target=OUT/name
  target.parent.mkdir(parents=True,exist_ok=True)
  with z.open(name) as src,target.open("wb") as dst:
   shutil.copyfileobj(src,dst,length=4*1024*1024)
  row={"path":name,"bytes":target.stat().st_size,
       "sha256":hashlib.sha256(target.read_bytes()).hexdigest()}
  env=UnityPy.load(str(target))
  row["types"]=dict(collections.Counter(obj.type.name for obj in env.objects))
  report["original_prefabs"].append(row)
  for obj in env.objects:
   typ=obj.type.name
   try:
    if typ=="GameObject":
     d=obj.read_typetree()
     report["gameobjects"].append({"source_bundle":name,"path_id":str(obj.path_id),
       "name":d.get("m_Name"),"components":[ptr(x.get("component",{})) for x in d.get("m_Component",[])]})
    elif typ=="RectTransform":
     d=obj.read_typetree()
     report["recttransforms"].append({"source_bundle":name,"path_id":str(obj.path_id),
       "go":ptr(d.get("m_GameObject",{})),"father":ptr(d.get("m_Father",{})),
       "children":[ptr(x) for x in d.get("m_Children",[])],
       "anchor_min":d.get("m_AnchorMin"),"anchor_max":d.get("m_AnchorMax"),
       "anchored_position":d.get("m_AnchoredPosition"),
       "size_delta":d.get("m_SizeDelta"),"pivot":d.get("m_Pivot"),
       "local_scale":d.get("m_LocalScale"),"local_rotation":d.get("m_LocalRotation")})
    elif typ=="MonoBehaviour":
     try:
      d=obj.read_typetree()
      report["canvas_components"].append({"source_bundle":name,
        "path_id":str(obj.path_id),"go":ptr(d.get("m_GameObject",{})),
        "sprite_pointer":ptr(d.get("m_Sprite",{})),
        "script":d.get("m_Name"),"serialized_keys":list(d.keys())[:60],
        "raycast":d.get("m_RaycastTarget"),
        "color":d.get("m_Color")})
     except Exception as err:pass
    elif typ=="Sprite":
     report["source_sprites"].append({"source_bundle":name,"path_id":str(obj.path_id),
       "name":str(obj.read().m_Name)})
   except Exception as ex:report["errors"].append({"bundle":name,"obj":str(obj.path_id),"type":typ,"error":str(ex)[:300]})
  print("ORIGINAL_UI_PREFAB",json.dumps(row),flush=True)
apk.unlink(missing_ok=True)
report["counts"]={"prefabs":len(report["original_prefabs"]),"recttransforms":len(report["recttransforms"]),
  "gameobjects":len(report["gameobjects"]),"image_components":len(report["canvas_components"])}
(OUT/"original_hud_ui_hierarchy.json").write_text(json.dumps(report,indent=2,ensure_ascii=False),encoding="utf8")
print("ORIGINAL_HUD_PREFAB_COUNTS",json.dumps(report["counts"]),flush=True)
if report["recttransforms"]:
 game={(x["source_bundle"],x["path_id"]):x["name"] for x in report["gameobjects"]}
 for rect in report["recttransforms"]:
  rect["name"]=game.get((rect["source_bundle"],rect["go"]))
 wanted=["Battle","Fight","Attack","Jump","HP","MiniMap","Map","Joy","Run","Bag","Sprint"]
 rows=[r for r in report["recttransforms"] if r.get("name") and
       any(w.lower() in r["name"].lower() for w in wanted)]
 (OUT/"hud_button_coordinates.json").write_text(json.dumps(rows,indent=2,ensure_ascii=False))
 print("ORIGINAL_HUD_RECTS_NAMED",json.dumps(rows[:30],ensure_ascii=False)[:5000])
