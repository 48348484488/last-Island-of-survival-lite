#!/usr/bin/env python3
"""Audit original Unity UGUI data, and original map art, without inferred sprites.

The original prefab has actual GameObject active flags, RectTransforms,
MonoBehaviour Image components, CanvasScaler, and external UI atlas sprites.
This script writes only source facts for deterministic renderer input.
"""
from pathlib import Path
import json,collections
import UnityPy

R=Path(__file__).resolve().parents[1]
O=R/"hud_canvas_source_audit"
O.mkdir(exist_ok=True)
ab=R/"last_island_recreated_game/assets/hud_original_xapk/ui_prefabs_original/assets/ui/battlepanel.ab"
assert ab.is_file(),ab
env=UnityPy.load(str(ab))
source={"file":"install_time_asset_pack.apk/assets/ui/battlepanel.ab",
        "objects":{},"canvas_scalers":[],"canvases":[],"images":{},"warnings":[]}
for ob in env.objects:
 if ob.type.name not in ("GameObject","MonoBehaviour","Canvas","RectTransform"):continue
 try:d=ob.read_typetree()
 except Exception:continue
 pid=str(ob.path_id)
 if ob.type.name=="GameObject":
  source["objects"][pid]={"name":d.get("m_Name"),"is_active":d.get("m_IsActive"),
  "layer":d.get("m_Layer"),"components":[str(c.get("component",{}).get("m_PathID",0)) for c in d.get("m_Component",[])]}
 if ob.type.name=="MonoBehaviour":
  comp={"go":str(d.get("m_GameObject",{}).get("m_PathID",0)),
   "enabled":d.get("m_Enabled"),"name":d.get("m_Name"),
   "script":d.get("m_Script"),
   "sprite":str(d.get("m_Sprite",{}).get("m_PathID",0)),
   "color":d.get("m_Color"),"type":d.get("m_Type"),
   "fill_amount":d.get("m_FillAmount"),"preserve_aspect":d.get("m_PreserveAspect"),
   "raycast":d.get("m_RaycastTarget"),"native_size":d.get("m_SetNativeSize"),
   "material":d.get("m_Material"),"keys":list(d.keys())}
  if "m_ReferenceResolution" in d:
   source["canvas_scalers"].append({"id":pid,**comp,
      "reference_resolution":d["m_ReferenceResolution"],
      "screen_match_mode":d.get("m_ScreenMatchMode"),"match":d.get("m_MatchWidthOrHeight"),
      "ui_scale_mode":d.get("m_UiScaleMode")})
  if "m_Sprite" in d or "m_Color" in d or "m_Text" in d:
   if "m_Text" in d:comp["text"]=str(d["m_Text"])[:120]
   source["images"][pid]=comp
 if ob.type.name=="Canvas":
  source["canvases"].append({"id":pid,"render_mode":d.get("m_RenderMode"),
    "sorting_order":d.get("m_SortingOrder"),"override_sorting":d.get("m_OverrideSorting"),
    "target_display":d.get("m_TargetDisplay"),"go":str(d.get("m_GameObject",{}).get("m_PathID",0))})
print("PREFAB_INSPECTION",json.dumps({
 "go":len(source["objects"]),"image_components":len(source["images"]),
 "canvas_scalers":source["canvas_scalers"],
 "canvases":source["canvases"][:5]},ensure_ascii=False)[:8000],flush=True)
out=O/"original_canvas_components.json"
out.write_text(json.dumps(source,ensure_ascii=False,separators=(",",":")),encoding="utf8")

# Original map texture is bundled in the game asset-pack, not a web screenshot.
# The APK-origin map_map.ab was already extracted in a separate verified workflow.
maps=R/"hud_original_comprovacao_xapk/unity_source_ab/map_map.ab"
if maps.is_file():
 env=UnityPy.load(str(maps))
 rec=[]
 for ob in env.objects:
  if ob.type.name!="Texture2D":continue
  try:
   t=ob.read()
   name=str(t.m_Name)
   rec.append({"name":name,"path_id":str(ob.path_id),
               "width":t.m_Width,"height":t.m_Height})
   if t.m_Width>=512 and t.m_Height>=512:
    im=t.image.convert("RGBA")
    dest=O/("source_map_"+str(ob.path_id)+".png")
    im.save(dest,optimize=True)
  except Exception as e:rec.append({"path_id":str(ob.path_id),"error":str(e)[:160]})
 (O/"map_map_source_inventory.json").write_text(json.dumps(rec,ensure_ascii=False,indent=2))
 print("SOURCE_MAP_TEXTURES",json.dumps(rec,ensure_ascii=False)[:6000],flush=True)
else:print("NO_MAP_BUNDLE_AVAILABLE",flush=True)
