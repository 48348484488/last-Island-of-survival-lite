#!/usr/bin/env python3
"""Read-only native Unity Camera census in every Oxide 1.14 original asset bundle.

A Camera in an asset does not prove it is the live multiplayer camera.
"""
import argparse
from collections import Counter
import gc
import hashlib
import json
from pathlib import Path
import zipfile

def ptr(x):
    if not isinstance(x,dict):return None
    f=x.get("m_FileID");p=x.get("m_PathID")
    return int(p) if type(f) is int and f==0 and type(p) is int and p!=0 else None

def scan(raw,filename):
    import UnityPy
    env=UnityPy.load(raw)
    objects=list(env.objects)
    class_count=Counter(o.type.name for o in objects)
    cameras=[o for o in objects if o.type.name=="Camera"]
    candidate_names = any(t in filename.lower() for t in
                          ("camera","core-", "home-menu", "player", "game-scene", "gameplay"))
    if not cameras and not candidate_names:
        return {"bundle":filename,"objects":len(objects),"camera_count":0,
                "cameras":[],"named_camera_gameobjects":[],"errors":[]}
    go_names={}
    errors=[]
    # Every GameObject is decoded only in camera-containing and key scene/prefab bundles.
    for o in objects:
        if o.type.name!="GameObject":continue
        try:
            t=o.read_typetree()
            if isinstance(t,dict):
                go_names[o.path_id]=str(t.get("m_Name",""))[:120]
        except Exception as e:
            errors.append("GameObject "+str(o.path_id)+": "+type(e).__name__)
    native=[]
    for o in cameras:
        rec={"pathID":o.path_id}
        try:
            t=o.read_typetree()
            if isinstance(t,dict):
                gp=ptr(t.get("m_GameObject"))
                rec["gameObject_pathID"]=gp
                rec["gameObject"]=go_names.get(gp) if gp else None
                rec["enabled"]=t.get("m_Enabled")
                rec["fieldOfView"]=t.get("field of view",t.get("m_FieldOfView"))
                rec["nearClipPlane"]=t.get("near clip plane",t.get("m_NearClipPlane"))
                rec["farClipPlane"]=t.get("far clip plane",t.get("m_FarClipPlane"))
                rec["orthographic"]=t.get("orthographic",t.get("m_Orthographic"))
            else:
                errors.append("Camera non-dict typetree "+str(o.path_id))
        except Exception as e:
            errors.append("Camera "+str(o.path_id)+": "+type(e).__name__+": "+str(e)[:120])
        native.append(rec)
    names=[{"pathID":pid,"name":name} for pid,name in go_names.items()
           if any(s in name.lower() for s in ("camera","cinemachine","look root","fp root"))]
    return {"bundle":filename,"objects":len(objects),"camera_count":len(cameras),
            "cameras":native,"named_camera_gameobjects":names[:250],
            "errors":errors[:150]}

def main():
    p=argparse.ArgumentParser()
    p.add_argument("--assetpack",required=True,type=Path)
    p.add_argument("--out",default=Path("camera-census/cameras.json"),type=Path)
    args=p.parse_args()
    found=[];errors=[];processed=0
    h=hashlib.sha256()
    with args.assetpack.open("rb") as f:
        for buf in iter(lambda:f.read(8*1024*1024),b""):h.update(buf)
    with zipfile.ZipFile(args.assetpack) as archive:
        files=[x for x in archive.infolist() if x.filename.endswith(".bundle")]
        for i,item in enumerate(files):
            try:
                if item.file_size>300*1024*1024:
                    raise ValueError("bundle exceeds 300 MB memory limit")
                with archive.open(item) as f:raw=f.read()
                entry=scan(raw,item.filename)
                processed+=1
                if entry["camera_count"] or entry["named_camera_gameobjects"] or entry["errors"]:
                    found.append(entry)
                del entry,raw
            except Exception as e:
                errors.append({"path":item.filename,"error":type(e).__name__+": "+str(e)[:300]})
            gc.collect()
            if (i+1)%25==0:
                print(f"Progress {i+1}/{len(files)}; errors {len(errors)}",flush=True)
    result={"assetpack":args.assetpack.name,"sha256":h.hexdigest(),"bundles_total":len(files),
            "bundles_processed":processed,
            "cameras_native_total":sum(x["camera_count"] for x in found),
            "camera_bundles":[x for x in found if x["camera_count"]],
            "candidate_bundles":[x for x in found if not x["camera_count"]],
            "errors":errors,
            "limitations":["Serialized Camera objects are not necessarily used in multiplayer.",
                           "Absent Camera components in one prefab do not prove runtime absence.",
                           "No game memory, runtime camera matrix or multiplayer enemy position data collected."]}
    args.out.parent.mkdir(parents=True,exist_ok=True)
    args.out.write_text(json.dumps(result,indent=2,ensure_ascii=False)+"\n")
    print(json.dumps({"total":len(files),"scanned":processed,"native_cameras":result["cameras_native_total"],
                      "camera_bundles":len(result["camera_bundles"]),"exceptions":len(errors)}),flush=True)
    if errors or processed!=len(files):raise SystemExit(1)

if __name__=="__main__":main()
