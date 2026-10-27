#!/usr/bin/env python3
"""Export original stone axe Unity prefab, skinned mesh and hierarchy for local 3D reconstruction."""
import json, traceback
from pathlib import Path
import UnityPy
import numpy as np
from collections import Counter

ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"stone_axe_prefab_source"
OUT.mkdir(parents=True,exist_ok=True)
asset=UnityPy.load(str(ROOT/"raw_assets/assets/model/wq_shifu.ab"))
report={"objects":[],"meshes":[],"renderers":[],"errors":[]}
def simple_tree(o):
 try:return o.read_typetree()
 except Exception as e:return {"error":str(e)}
def qptr(x):
 if isinstance(x,dict):return str(x.get("m_PathID",0))
 return str(getattr(x,"path_id",getattr(x,"m_PathID",0)))
def reduced(x,level=0):
 if level>4:return "..."
 if isinstance(x,dict):
  return {k:reduced(v,level+1) for k,v in x.items() if k not in ("m_VertexData","m_CompressedMesh","m_Colors","m_IndexBuffer")}
 if isinstance(x,list):
  return [reduced(v,level+1) for v in x[:8]]+(["<more omitted>"] if len(x)>8 else [])
 if isinstance(x,(bytes,bytearray)):return f"<{len(x)} raw bytes>"
 if isinstance(x,(int,float,bool,str)) or x is None:return x
 return str(x)
for o in asset.objects:
 kind=o.type.name
 if kind not in ["GameObject","Transform","SkinnedMeshRenderer","Mesh","Material","Texture2D"]:continue
 rec={"type":kind,"id":str(o.path_id)}
 try:
  obj=o.read()
  if kind=="GameObject":
   d=simple_tree(o)
   rec.update({"name":d.get("m_Name"),"components":reduced(d.get("m_Component"))})
  if kind=="Transform":
   d=simple_tree(o)
   rec.update({"gameobject":qptr(d.get("m_GameObject")),"pos":d.get("m_LocalPosition"),
       "rot":d.get("m_LocalRotation"),"scale":d.get("m_LocalScale"),
       "father":qptr(d.get("m_Father")),
       "children":[qptr(x) for x in d.get("m_Children",[])]})
  if kind=="SkinnedMeshRenderer":
   d=simple_tree(o)
   rec.update({"gameobject":qptr(d.get("m_GameObject")),"root_bone":qptr(d.get("m_RootBone")),
    "bones":[qptr(x) for x in d.get("m_Bones",[])],
    "mesh":qptr(d.get("m_Mesh")),"materials":[qptr(x) for x in d.get("m_Materials",[])],
    "bounds":reduced(d.get("m_AABB"))})
  if kind=="Mesh":
   d=simple_tree(o)
   rec.update({"name":d.get("m_Name"),"keys":list(d.keys()),"submeshes":reduced(d.get("m_SubMeshes")),"bindposes":reduced(d.get("m_BindPose"))})
   # UnityPy native mesh decoder (in some versions vertices attributes exposed only after processing)
   verts=getattr(obj,"m_Vertices",None)
   normals=getattr(obj,"m_Normals",None)
   uvs=getattr(obj,"m_UV0",None)
   tris=getattr(obj,"m_Indices",None)
   weights=getattr(obj,"m_Skin",None)
   for key,val in [('verts',verts),('normals',normals),('uvs',uvs),('tris',tris),('weights',weights)]:
    if val is not None:
     try:rec[key+"_info"]={"length":len(val),"first":str(list(val[:3]))[:900]}
     except Exception as e: rec[key+"_error"]=str(e)
   if verts is not None and len(verts):
    def points(vals):
     return np.asarray([[v.x,v.y,v.z] for v in vals],np.float32)
    arrays={"vertices":points(verts)}
    if normals is not None and len(normals):arrays["normals"]=points(normals)
    if uvs is not None and len(uvs):arrays["uvs"]=np.asarray([[v.x,v.y] for v in uvs],np.float32)
    if tris is not None and len(tris):arrays["indices"]=np.asarray(tris,np.uint32)
    np.savez_compressed(OUT/"original_wq_shifu_mesh.npz",**arrays)
    rec["saved_arrays"]=list(arrays.keys())
   # Save original bind pose in full
   if d.get("m_BindPose"):(OUT/"mesh_bindposes.json").write_text(json.dumps(d.get("m_BindPose"),default=str),encoding="utf8")
  if kind=="Material":rec["name"]=getattr(obj,"m_Name","")
  if kind=="Texture2D":
   rec["name"]=str(getattr(obj,"m_Name",""))
   try:
    obj.image.save(str(OUT/("texture_"+rec["name"]+".png")))
    rec["texture_exported"]=True
   except Exception as e:rec["texture_error"]=str(e)
 except Exception as e:
  rec["error"]=str(e);report["errors"].append({"id":rec["id"],"error":traceback.format_exc()})
 report["objects"].append(rec)
(OUT/"prefab.json").write_text(json.dumps(report,ensure_ascii=False,indent=2,default=str),encoding="utf8")
for o in report["objects"]:print("PREFAB_OBJ",json.dumps(o,ensure_ascii=False,default=str)[:5000])
print("FILES",[(p.name,p.stat().st_size) for p in OUT.glob("*")])
print("ERRORS",len(report["errors"]))
