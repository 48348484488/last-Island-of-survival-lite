#!/usr/bin/env python3
"""Extract original *raw* Unity AnimationClip serialized fields from animationbundle.ab.
Unlike the game's already-existing NPZ, this directly reads the source AssetBundle.
Also inspect weapon prefab original Transform data, without editing either source.
"""
import json
from pathlib import Path
from collections import Counter
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"original_axe_source_extraction"
OUT.mkdir(parents=True,exist_ok=True)
TARGET=[
 "fu_zhan_attack_01",
 "tongyong_bishou_zhan_stand01",
 "shuangshouchui_zhan_stand01",
 "dao_zhan_attack01",
 "dao_zhan_nawuqi01",
 "dao_zhan_shouwuqi01",
 "dao_zhan_rush_front01",
 "zhan_stand01"
]
def enc(v):
 if isinstance(v,bytes):return {"bytes_hex":v[:256].hex(),"bytes_length":len(v)}
 if hasattr(v,"tolist"):return v.tolist()
 return str(v)

def describe(obj):
 try:
  d=obj.read_typetree()
  return d,None
 except Exception as e:
  return {},str(e)

def rec(value, max_depth=6, depth=0):
 if depth>=max_depth:return "<nested>"
 if isinstance(value,dict):
  return {k:rec(v,max_depth,depth+1) for k,v in value.items()}
 if isinstance(value,list):
  if len(value)>50:return {"count":len(value),"sample":[rec(x,max_depth,depth+1) for x in value[:3]]}
  return [rec(x,max_depth,depth+1) for x in value]
 return enc(value) if not isinstance(value,(str,int,float,bool,type(None))) else value

def main():
 env=UnityPy.load(str(ROOT/"raw_assets/assets/role/animationbundle.ab"))
 counts=Counter(o.type.name for o in env.objects)
 findings={"asset_bundle":"raw_assets/assets/role/animationbundle.ab",
           "object_counts":dict(counts),"found":[],"missing":[],"errors":[]}
 seen=set()
 clip_index={}
 for o in env.objects:
  if o.type.name!="AnimationClip":continue
  a=o.read()
  name=str(a.m_Name)
  clip_index[str(o.path_id)]=name
  if name not in TARGET:continue
  d,err=describe(o)
  # Save original unmodified clip fields, including native streams/samples, for independent inspection.
  f=OUT/("animation_"+name+"_"+str(o.path_id)+".json")
  f.write_text(json.dumps(d,default=enc,ensure_ascii=False),encoding="utf-8")
  keys=list(d.keys())
  mc=d.get("m_MuscleClip",{})
  piece={"clip":name,"path_id":str(o.path_id),"serialized_json":f.name,"json_bytes":f.stat().st_size,
         "top_keys":keys,"muscle_keys":list(mc) if isinstance(mc,dict) else [],
         "sample_rate":d.get("m_SampleRate"),"stop_time":mc.get("m_StopTime") if isinstance(mc,dict) else None,
         "clip_structure":rec(mc.get("m_Clip",{}),4) if isinstance(mc,dict) else None,
         "type_error":err}
  findings["found"].append(piece)
  seen.add(name)
 (OUT/"clip_index.json").write_text(json.dumps(clip_index,ensure_ascii=False,indent=2),encoding="utf-8")
 for name in TARGET:
  if name not in seen:findings["missing"].append(name)
 try:
  weapon=UnityPy.load(str(ROOT/"raw_assets/assets/model/wq_shifu.ab"))
  findings["axe_counts"]=dict(Counter(o.type.name for o in weapon.objects))
  for o in weapon.objects:
   if o.type.name not in ("GameObject","Transform"):continue
   d,err=describe(o)
   # Preserve relevant transform hierarchies (original weapon rig, zero local transform in Player code).
   if o.type.name=="Transform":
    piece={"id":str(o.path_id),"position":d.get("m_LocalPosition"),"rotation":d.get("m_LocalRotation"),"scale":d.get("m_LocalScale"),"father":d.get("m_Father"),"children":d.get("m_Children")}
    findings.setdefault("axe_transforms",[]).append(piece)
   else:
    findings.setdefault("axe_objects",[]).append({"id":str(o.path_id),"name":d.get("m_Name")})
 except Exception as e:
  findings["errors"].append("axe AssetBundle: "+str(e))
 (OUT/"source_report.json").write_text(json.dumps(findings,ensure_ascii=False,indent=2,default=enc),encoding="utf-8")
 print("SOURCE_AB_OBJECTS",json.dumps(findings["object_counts"],ensure_ascii=False))
 for x in findings["found"]:
  print("EXTRACTED_RAW",x["clip"],"size",x["json_bytes"],"fps",x["sample_rate"],
        "keys",x["muscle_keys"],"clip_structure",json.dumps(x["clip_structure"],default=enc)[:1300])
 print("MISSING",findings["missing"])
 print("AXE_COUNTS",findings.get("axe_counts"))
 print("AXE_TRANSFORMS",json.dumps(findings.get("axe_transforms",[])[:15],default=enc)[:3500])
 print("ERRORS",findings["errors"])
if __name__=="__main__":main()
