#!/usr/bin/env python3
"""Exhaust source model/partskinmesh bundles for BASE level-one build meshes.
Target all GameObject/Mesh names; base mesh bundle may not share texture name.
"""
from pathlib import Path
import zipfile,shutil,tempfile,json,re,collections
import UnityPy
parts=sorted(Path("xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17
check=re.compile(r"diji|qiangbi|wuding|chuanghu|menkuang|louti|fundation|foundation|cube|basepart|wall|floor|stair|window|doorframe|diban|jianzhu",re.I)
with tempfile.TemporaryDirectory() as tmp:
 joined=Path(tmp)/"source.xapk"
 with joined.open("wb") as w:
  for part in parts:
   with part.open("rb") as f:shutil.copyfileobj(f,w,8*1024*1024)
 with zipfile.ZipFile(joined) as outer:
  nested=Path(tmp)/"assets.apk"
  with outer.open("install_time_asset_pack.apk") as f,nested.open("wb") as w:
   shutil.copyfileobj(f,w,8*1024*1024)
 result={"searched":[],"hits":[],"skipped":[]}
 with zipfile.ZipFile(nested) as z:
  names=[x for x in z.namelist() if x.endswith(".ab") and
         x.startswith(("assets/model/","assets/partskinmesh/","assets/sceneprefab/"))]
  print("LIOS_ALL_POTENTIAL_BUILD_MESH_BUNDLES",len(names),flush=True)
  for i,path in enumerate(names):
   if z.getinfo(path).file_size>15_000_000:
    result["skipped"].append(path);continue
   try:env=UnityPy.load(z.read(path))
   except Exception as e:
    result["skipped"].append(path+":"+str(e)[:75]);continue
   meshes=[];matches=[]
   for ob in env.objects:
    if ob.type.name not in ("Mesh","GameObject"):continue
    try:label=str(ob.read().m_Name)
    except Exception:continue
    if ob.type.name=="Mesh":meshes.append(label)
    if check.search(label):matches.append(dict(type=ob.type.name,name=label,id=str(ob.path_id)))
   if matches:
    record=dict(source_bundle=path,matches=matches[:70],mesh_count=len(meshes),
                meshes=meshes[:70])
    result["hits"].append(record)
    print("LIOS_NATIVE_SOURCE_BASIC_MESH_CANDIDATE",json.dumps(record,ensure_ascii=False)[:3700],flush=True)
   result["searched"].append(path)
   if (i+1)%150==0:
    print("LIOS_NATIVE_MESH_SCAN",i+1,len(names),"found",len(result["hits"]),flush=True)
 Path("original_base_mesh_source_index.json").write_text(json.dumps(result,ensure_ascii=False,indent=2))
 print("LIOS_SOURCE_BASIC_BUILD_MESH_AUDIT_DONE",len(result["searched"]),len(result["hits"]),flush=True)
