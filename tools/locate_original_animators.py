#!/usr/bin/env python3
"""Find any genuine AnimatorController assets among available original bundles."""
from pathlib import Path
import json, UnityPy
root=Path(__file__).resolve().parents[1]
files=sorted((root/"raw_assets/assets").rglob("*.ab"))
result={"scanned":0,"controllers":[],"animators":[],"errors":[]}
for path in files:
 try:
  env=UnityPy.load(str(path))
  result["scanned"]+=1
  for obj in env.objects:
   typ=obj.type.name
   if typ not in ("AnimatorController","AnimatorOverrideController","Animator"):
    continue
   try:
    data=obj.read()
    info={"bundle":str(path.relative_to(root)),"type":typ,"path_id":str(obj.path_id),
          "name":getattr(data,"m_Name","")}
    if typ!="Animator":
     tree=obj.read_typetree()
     name=str(tree.get("m_Name",info["name"]))
     info["name"]=name
     info["keys"]=list(tree)[:50]
     (root/"original_animator_controller").mkdir(exist_ok=True)
     target=(root/"original_animator_controller"/(path.stem+"_"+name[:50]+"_"+str(obj.path_id)+".json"))
     target.write_text(json.dumps(tree,default=str),encoding="utf-8")
     info["json"]=target.name
     result["controllers"].append(info)
    else:
     info["controller_pointer"]=str(getattr(data,"m_Controller",None))
     result["animators"].append(info)
   except Exception as e:result["errors"].append(str(e)[:140])
 except Exception as e:result["errors"].append(str(path)+": "+str(e)[:140])
(root/"original_animator_controller").mkdir(exist_ok=True)
(root/"original_animator_controller"/"index.json").write_text(json.dumps(result,indent=2,ensure_ascii=False),encoding="utf-8")
print("ASSET_BUNDLES_SCANNED",result["scanned"])
print("CONTROLLER_COUNT",len(result["controllers"]))
for item in result["controllers"]:print("CONTROLLER",item)
print("ANIMATOR_SAMPLES",result["animators"][:10])
print("ERROR_COUNT",len(result["errors"]))
