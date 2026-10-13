#!/usr/bin/env python3
"""Inspect serialized Unity Camera ownership, hierarchy and camera-related references.

Static only: no APK modification, process injection or live player coordinates.
Requires UnityPy >=1.20.
"""
from __future__ import annotations
import argparse, hashlib, json, zipfile
from pathlib import Path
from collections import Counter

PREFIXES=("scenes-tutorial_scenes_", "shared-world-local_assets_", "features-lobby_assets_")
CAMERA_WORDS=("camera","cinemachine","follow","aim","look","observer","spectat")

def local_ptr(x):
    if not isinstance(x, dict):return None
    pid=x.get("m_PathID")
    fid=x.get("m_FileID")
    return pid if type(fid)==int and fid==0 and type(pid)==int and pid!=0 else None

def refs(value,depth=0):
    if depth>5:return []
    p=local_ptr(value)
    if p is not None:return [p]
    if isinstance(value,dict):
        o=[]
        for v in list(value.values())[:96]:o+=refs(v,depth+1)
        return o
    if isinstance(value,list):
        o=[]
        for v in value[:96]:o+=refs(v,depth+1)
        return o
    return []

def simple(value):
    if isinstance(value,(bool,int,float,str)):
        return value if not isinstance(value,str) else value[:160]
    p=local_ptr(value)
    if p is not None:return {"localPathID":p}
    if isinstance(value,dict):
        return {"keys":list(value)[:16],"localPathIDs":refs(value)[:16]}
    if isinstance(value,list):
        return {"listLength":len(value),"localPathIDs":refs(value)[:16]}
    return str(type(value).__name__)

def inspect(data,filename):
    import UnityPy
    env=UnityPy.load(data)
    objects=list(env.objects)
    types={o.path_id:o.type.name for o in objects}
    gameobjects={}; transforms={}; behaviours={}; cameras={}
    errors=Counter()
    for obj in objects:
        kind=types[obj.path_id]
        if kind not in ("GameObject","Transform","RectTransform","MonoBehaviour","Camera"):
            continue
        try:
            t=obj.read_typetree()
            if not isinstance(t,dict):continue
        except Exception as exc:
            errors[kind+"_"+type(exc).__name__]+=1
            continue
        if kind=="GameObject":
            gameobjects[obj.path_id]={"name":str(t.get("m_Name",""))[:140],
                "components":refs(t.get("m_Component",[])), "active":t.get("m_IsActive")}
        elif kind in ("Transform","RectTransform"):transforms[obj.path_id]=t
        elif kind=="MonoBehaviour":behaviours[obj.path_id]=t
        elif kind=="Camera":cameras[obj.path_id]=t

    def component_links(go_id):
        go=gameobjects.get(go_id) or {}
        return [{"pathID":p,"type":types.get(p,"unknown")} for p in go.get("components",[])[:90]]

    def transform_of(go_id):
        for item in (gameobjects.get(go_id) or {}).get("components",[]):
            if item in transforms:return item
        return None

    def hierarchy(go_id):
        t=transform_of(go_id)
        seen=set();chain=[]
        while t is not None and t in transforms and len(chain)<16:
            if t in seen:
                chain.append({"cycle_at":t});break
            seen.add(t)
            entry=transforms[t]
            parent_go=local_ptr(entry.get("m_GameObject"))
            chain.append({"transformID":t,"GameObject":(gameobjects.get(parent_go) or {}).get("name"),"gameObjectID":parent_go})
            father=local_ptr(entry.get("m_Father"))
            t=father if father in transforms else None
        return chain

    candidates=[]
    for goid,go in gameobjects.items():
        name=go["name"]
        if any(w in name.lower() for w in CAMERA_WORDS):
            candidates.append({"gameObjectID":goid,"name":name,
                "parentHierarchy":hierarchy(goid),
                "components":component_links(goid)})

    details=[]
    tracked_component_ids=set()
    for go in candidates:
        for comp in go["components"]:
            tracked_component_ids.add(comp["pathID"])
    for goid,go in gameobjects.items():
        comps=component_links(goid)
        if any(x["type"]=="Camera" for x in comps):
            for comp in comps:tracked_component_ids.add(comp["pathID"])

    for pathid in sorted(tracked_component_ids):
        t=behaviours.get(pathid)
        if t is None:continue
        gameobj=local_ptr(t.get("m_GameObject"))
        fields={key:simple(val) for key,val in t.items() if
                (not key.startswith("m_") or any(w in key.lower() for w in CAMERA_WORDS))}
        details.append({"MonoBehaviourID":pathid,
            "GameObject":(gameobjects.get(gameobj) or {}).get("name"),
            "GameObjectID":gameobj,
            "scriptID":local_ptr(t.get("m_Script")),
            "fieldCount":len(t),
            "fields":dict(list(fields.items())[:125])})

    native=[]
    target_ids=set()
    for pathid,t in cameras.items():
        goid=local_ptr(t.get("m_GameObject"))
        tranid=transform_of(goid)
        target_ids.update(x for x in (pathid,goid,tranid) if x is not None)
        native.append({"cameraPathID":pathid,"gameObjectID":goid,
            "gameObject":(gameobjects.get(goid) or {}).get("name"),
            "hierarchy":hierarchy(goid),
            "components":component_links(goid),
            "projection":{
                "fov":simple(t.get("field of view",t.get("m_FieldOfView"))),
                "near":simple(t.get("near clip plane",t.get("m_NearClipPlane"))),
                "far":simple(t.get("far clip plane",t.get("m_FarClipPlane"))),
                "orthographic":simple(t.get("orthographic",t.get("m_Orthographic"))),
                "orthographic_size":simple(t.get("orthographic size",t.get("m_OrthographicSize")))
            }})

    incoming=[]
    for pathid,t in behaviours.items():
        for key,v in t.items():
            if key in ("m_GameObject","m_Script"):continue
            local_matches=sorted(target_ids.intersection(refs(v)))
            if local_matches:
                goid=local_ptr(t.get("m_GameObject"))
                incoming.append({"sourceMonoBehaviourID":pathid,
                    "sourceGameObject":(gameobjects.get(goid) or {}).get("name"),
                    "sourceObjectID":goid,"field":key,"targetPathIDs":local_matches})
    return {"file":filename,"object_count":len(objects),"class_counts":dict(Counter(types.values())),
        "GameObjects":len(gameobjects),"MonoBehaviours":len(behaviours),
        "native_cameras":native,"camera_named_gameobjects":candidates[:160],
        "camera_related_behaviours":details[:350],
        "incoming_native_camera_refs":incoming[:150],
        "errors":dict(errors),
        "limitations":["Only serialized Unity references; no runtime active Camera, multiplayer matrix or enemy positions.",
                       "A component name is not a C# method address and PPtr path IDs are not memory offsets."]}

def run(apk,out):
    with zipfile.ZipFile(apk) as z:
        matches=[i for i in z.infolist() if
                 any(i.filename.split("/")[-1].startswith(pre) for pre in PREFIXES)]
        if len(matches)!=len(PREFIXES):
            raise ValueError("Expected exactly three Unity bundles; got "+str(len(matches)))
        results=[];errors=[]
        for member in matches:
            try:
                if member.file_size>300*1024*1024:raise ValueError("oversized bundle")
                with z.open(member) as f: buf=f.read()
                result=inspect(buf,member.filename)
                result["sha256"]=hashlib.sha256(buf).hexdigest()
                results.append(result)
                print(json.dumps({"bundle":member.filename,"objects":result["object_count"],
                                  "Camera":len(result["native_cameras"]),"named":len(result["camera_named_gameobjects"]),
                                  "readErrors":sum(result["errors"].values())}),flush=True)
            except Exception as exc:
                errors.append({"bundle":member.filename,"error":type(exc).__name__+": "+str(exc)})
    report={"bundles":results,"errors":errors}
    out.parent.mkdir(parents=True,exist_ok=True)
    out.write_text(json.dumps(report,indent=2,ensure_ascii=False)+"\n")
    if errors or any(r["errors"] for r in results):
        raise SystemExit("Incomplete camera analysis, see JSON")

if __name__=="__main__":
    p=argparse.ArgumentParser()
    p.add_argument("--apk",type=Path,required=True)
    p.add_argument("--out",type=Path,default=Path("camera-links/report.json"))
    x=p.parse_args()
    run(x.apk,x.out)
