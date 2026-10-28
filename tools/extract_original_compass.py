#!/usr/bin/env python3
"""Recover the original compass RawImage texture from its exact original Unity AB.

No synthetic compass glyphs are drawn: the source is assets/texture/
battlepanel_kedu_yw.ab from the verified Last Island LITE 7.3 XAPK.
"""
import json,hashlib
from pathlib import Path
import UnityPy
R=Path(__file__).resolve().parents[1]
assets=R/"last_island_recreated_game/assets/hud_original_xapk"
ui=assets/"ui_prefabs_original/assets/ui/battlepanel.ab"
atlas=R/"hud_original_comprovacao_xapk/unity_source_ab/battlepanel_kedu_yw.ab"
assert ui.is_file() and atlas.is_file(),(ui,atlas)
env=UnityPy.load(str(ui));go={}
for obj in env.objects:
 if obj.type.name!="GameObject":continue
 d=obj.read_typetree()
 if d.get("m_Name")=="m_compassImage":
  go[str(obj.path_id)]=d
assert len(go)==1,go.keys()
pid=next(iter(go))
component=[str(x.get("component",{}).get("m_PathID",0)) for x in go[pid].get("m_Component",[])]
rawimage=[]
for obj in env.objects:
 if obj.type.name!="MonoBehaviour" or str(obj.path_id) not in component:continue
 try:d=obj.read_typetree()
 except Exception:continue
 if "m_Texture" in d:rawimage.append({"component_path_id":str(obj.path_id),
  "m_Texture":d["m_Texture"],"m_UVRect":d.get("m_UVRect"),"m_Color":d.get("m_Color")})
assert len(rawimage)==1,rawimage
# Unity serialized local PPtr (m_FileID=0) resolves in this SAME prefab,
# not by similarly named Texture2D in an unrelated AssetBundle!
raw=rawimage[0]["m_Texture"]
assert raw["m_FileID"]==0,raw
expected=str(raw["m_PathID"])
originals=[o for o in env.objects if o.type.name=="Texture2D" and str(o.path_id)==expected]
assert len(originals)==1,("Actual RawImage source Texture2D is not in the prefab",raw)
found=[]
for obj in originals:
 t=obj.read()
 im=t.image.convert("RGBA")
 outfile=assets/"compass_original_source.png"
 im.save(outfile,optimize=True)
 found.append({"name":str(t.m_Name),"path_id":str(obj.path_id),
  "dimensions":list(im.size),"file":outfile.name,
  "original_texture_sha256":hashlib.sha256(ui.read_bytes()).hexdigest()})
assert found[0]["path_id"]==expected,("RawImage pointer mismatch",found,expected)
data={"gameobject":"m_compassImage","gameobject_path_id":pid,
 "source_prefab":"assets/ui/battlepanel.ab",
 "source_texture_bundle":"assets/ui/battlepanel.ab (m_Texture local PPtr)",
 "rawimage_component":rawimage[0],"textures":found,
 "verified_source":"Original XAPK install_time_asset_pack.apk"}
(assets/"compass_original_source.json").write_text(json.dumps(data,ensure_ascii=False,indent=2))
print("ORIGINAL_COMPASS_TEXTURE_SOURCE",json.dumps(data,ensure_ascii=False),flush=True)
