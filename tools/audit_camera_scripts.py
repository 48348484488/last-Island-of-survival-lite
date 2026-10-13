#!/usr/bin/env python3
"""Resolve original Oxide Unity MonoBehaviour scripts for camera-related objects.

Pure static asset inspection. Serialized MonoScript names do not identify live method
addresses, in-game active cameras, or enemy positions.
"""
import argparse
from collections import Counter,defaultdict
import hashlib,json,zipfile
from pathlib import Path

PREFIXES=("core-common-local_assets_","core-tutorial_assets_",
          "scenes-tutorial_scenes_","shared-world-local_assets_","features-lobby_assets_")
CANDIDATE_NAMES={"World Camera Root","World Camera","UI Camera","Camera","CameraController",
                 "CharacterCamera","Look Root","player","tutorialPlayer Variant"}

def pp(v):
    if not isinstance(v,dict):return None
    f=v.get("m_FileID");p=v.get("m_PathID")
    if type(f)==int and type(p)==int:return {"fileID":f,"pathID":p}
    return None

def component_refs(value,depth=0):
    if depth>4:return []
    p=pp(value)
    if p:return [p]
    if isinstance(value,dict):
        out=[]
        for v in list(value.values())[:100]:out+=component_refs(v,depth+1)
        return out
    if isinstance(value,list):
        out=[]
        for v in value[:100]:out+=component_refs(v,depth+1)
        return out
    return []

def registry(data):
    import UnityPy
    env=UnityPy.load(data)
    out=defaultdict(list)
    errors=Counter()
    for o in env.objects:
        if o.type.name!="MonoScript":continue
        try:
            x=o.read_typetree()
            if not isinstance(x,dict):continue
            out[o.path_id].append({
                "namespace":x.get("m_Namespace",""),
                "class":x.get("m_ClassName",""),
                "assembly":x.get("m_AssemblyName","")
            })
        except Exception as e:errors[type(e).__name__]+=1
    return out,dict(errors)

def scan(data,filename,registry):
    import UnityPy
    env=UnityPy.load(data)
    objects=list(env.objects)
    external_tables=[]
    for file_name, asset_file in (getattr(env,"files",{}) or {}).items():
        entries=[]
        for index, ext in enumerate(getattr(asset_file,"externals",[]) or [],start=1):
            def attr(name):
                if isinstance(ext,dict):return ext.get(name)
                return getattr(ext,name,None)
            values={k:str(attr(k))[:250] for k in
                    ("file_name","path_name","name","path") if attr(k) is not None}
            entries.append({"fileID":index,"values":values,"repr":repr(ext)[:220]})
        if entries:
            external_tables.append({"assetFile":str(file_name)[:180],"entries":entries[:40]})
    gameobjects={};mb={};kinds={o.path_id:o.type.name for o in objects}
    for obj in objects:
        if obj.type.name not in ("GameObject","MonoBehaviour"):continue
        try:
            t=obj.read_typetree()
            if not isinstance(t,dict):continue
        except Exception:continue
        if obj.type.name=="GameObject":
            gameobjects[obj.path_id]=str(t.get("m_Name",""))
        else:mb[obj.path_id]=t
    items=[]
    for obj in objects:
        if obj.type.name!="GameObject":continue
        name=gameobjects.get(obj.path_id,"")
        if name not in CANDIDATE_NAMES:continue
        try:t=obj.read_typetree()
        except Exception:continue
        comps=component_refs(t.get("m_Component",[]))
        detail=[]
        for cp in comps:
            if cp["fileID"]!=0:continue
            pid=cp["pathID"]
            cm={"componentPathID":pid,"unityType":kinds.get(pid,"unresolved")}
            tree=mb.get(pid)
            if tree is not None:
                sp=pp(tree.get("m_Script"))
                cm["scriptPointer"]=sp
                if sp:
                    candidates=registry.get(sp["pathID"],[])
                    cm["resolvedMonoScripts"]=candidates[:5]
                    cm["scriptResolution"]="matched" if len(candidates)==1 else ("ambiguous" if candidates else "unmatched")
                cm["cameraRelatedFieldNames"]=[
                    k for k in tree if any(s in k.lower() for s in
                    ("camera","look","zoom","pitch","follow","observe","network","render"))][:30]
                cm["worldCameraRoot"]=pp(tree.get("worldCameraRoot"))
                cm["m_Camera"]=pp(tree.get("m_Camera"))
            detail.append(cm)
        items.append({"gameObject":name,"pathID":obj.path_id,"components":detail})
    return {"bundle":filename,"candidateObjects":items,"monobehavioursRead":len(mb),
            "serializedObjects":len(objects),"externalAssetFileTables":external_tables}

def main():
    p=argparse.ArgumentParser()
    p.add_argument("--apk",type=Path,required=True)
    p.add_argument("--out",type=Path,default=Path("camera-scripts/scripts.json"))
    a=p.parse_args()
    with zipfile.ZipFile(a.apk) as z:
        files=z.infolist()
        mons=[i for i in files if "monoscripts" in i.filename.lower() and i.filename.endswith(".bundle")]
        if not mons:raise ValueError("MonoScript Unity bundle not found")
        lookup=defaultdict(list);error_details=[]
        for m in mons:
            with z.open(m) as f: buf=f.read()
            entries,errors=registry(buf)
            for k,v in entries.items():lookup[k].extend(v)
            if errors:error_details.append({"bundle":m.filename,"errors":errors})
        matches=[i for i in files if any(i.filename.split("/")[-1].startswith(pre) for pre in PREFIXES)]
        if len(matches)!=len(PREFIXES):raise ValueError("Missing target prefab/scene bundles")
        data=[]
        for m in matches:
            with z.open(m) as f:buf=f.read()
            item=scan(buf,m.filename,lookup)
            item["sha256"]=hashlib.sha256(buf).hexdigest()
            data.append(item)
            print(json.dumps({"bundle":m.filename,"candidateObjects":len(item["candidateObjects"]),
                              "mb":item["monobehavioursRead"]}),flush=True)
        stats=Counter()
        for b in data:
            for go in b["candidateObjects"]:
                for c in go["components"]:
                    if "scriptResolution" in c:stats[c["scriptResolution"]]+=1
        result={"registeredMonoScriptPathIDs":len(lookup),
                "monoscriptBundles":[m.filename for m in mons],
                "monoscriptReadErrors":error_details,
                "stats":dict(stats),"bundles":data,
                "limitations":["Script names map serialized MonoScript references only.",
                               "Cross-bundle pathID coincidences must be validated with external file IDs.",
                               "This does not retrieve live camera matrices or in-game player positions."]}
    a.out.parent.mkdir(parents=True,exist_ok=True)
    a.out.write_text(json.dumps(result,ensure_ascii=False,indent=2)+"\n")
    print(json.dumps({"scripts":len(lookup),"stats":dict(stats),"errors":error_details}),flush=True)

if __name__=="__main__":main()
