#!/usr/bin/env python3
"""Targeted read-only Unity serialized object graph audit for original Oxide assets.

Requires UnityPy. This reads GameObject / MonoBehaviour / MonoScript objects from
the release APK, never executes Unity assemblies or the game itself.
"""
import argparse
from collections import Counter
import hashlib
import json
from pathlib import Path
import sys
import zipfile

TARGET_BUNDLES=("core-common-local_assets_", "core-tutorial_assets_")
KEYS=("networkPositionObserver","playerNetworkSubscriptionController",
      "observedPlayer","worldCameraRoot","observeCameraDist","originalPosition",
      "observedId","observedNetId","isSubscribedSync","isPlusSubscribedSync",
      "areaOfInterest","playerCamera","worldCamera")
COMPONENT_NAMES=("player", "TeacherPlayerVIew", "tutorialPlayer Variant",
                 "AreaOfInterest", "Main Camera", "WorldCamera")

def pointer(x):
    if not isinstance(x,dict): return None
    pid = next((x[k] for k in ("m_PathID","pathID","path_id") if k in x),None)
    fid = next((x[k] for k in ("m_FileID","fileID","file_id") if k in x),None)
    if not isinstance(pid,int): return None
    return {"fileID":fid if isinstance(fid,int) else None, "pathID":pid}

def flatten_pointers(value,depth=0):
    if depth>4:return []
    p=pointer(value)
    if p:return [p]
    if isinstance(value,list):
        out=[]
        for x in value[:128]:out.extend(flatten_pointers(x,depth+1))
        return out
    if isinstance(value,dict):
        out=[]
        for v in value.values():out.extend(flatten_pointers(v,depth+1))
        return out
    return []

def safe_extract(v):
    p=pointer(v)
    if p:return {"ptr":p}
    if isinstance(v,(bool,int,float,str)):
        return v if not isinstance(v,str) or len(v)<200 else v[:200]
    if isinstance(v,list):
        return [safe_extract(x) for x in v[:16]]
    if isinstance(v,dict):
        return {str(k):safe_extract(w) for k,w in list(v.items())[:16]}
    return repr(v)[:200]

def inspect(raw,bundle_name):
    import UnityPy
    env=UnityPy.load(raw)
    objects=list(env.objects)
    counts=Counter()
    go_byid={}
    behaviours=[]
    scripts=[]
    issues=Counter()

    for obj in objects:
        name=obj.type.name
        counts[name]+=1
        if name not in ("GameObject","MonoBehaviour","MonoScript"):continue
        try:
            tree=obj.read_typetree()
            if not isinstance(tree,dict):continue
        except Exception as e:
            issues["read_"+name+"_"+type(e).__name__]+=1
            continue
        pathid=obj.path_id
        if name=="GameObject":
            go_byid[pathid]={
                "pathID":pathid,
                "name":str(tree.get("m_Name",""))[:120],
                "active":tree.get("m_IsActive"),
                "components":flatten_pointers(tree.get("m_Component",[]))
            }
        elif name=="MonoScript":
            scripts.append({
                "pathID":pathid,
                "name":tree.get("m_ClassName", ""),
                "namespace":tree.get("m_Namespace", ""),
                "assembly":tree.get("m_AssemblyName","")
            })
        else:
            behaviours.append((pathid,tree))
    byid={pathid:tree for pathid,tree in behaviours}
    matches=[]
    for pathid,tree in behaviours:
        targeted={key:safe_extract(v) for key,v in tree.items()
                  if any(w.lower() in key.lower() for w in KEYS)}
        if not targeted:continue
        go=pointer(tree.get("m_GameObject"))
        gobj=go_byid.get(go["pathID"]) if go and go["fileID"]==0 else None
        refs={}
        for key,val in targeted.items():
            ref=pointer(tree.get(key))
            if ref:
                target=byid.get(ref["pathID"]) if ref["fileID"]==0 else None
                refs[key]={
                    "pointer":ref,
                    "resolved_local_behaviour":target is not None,
                    "resolved_gameobject":safe_extract(pointer(target.get("m_GameObject")))
                    if target else None,
                    "target_field_keys":[k for k in target if not k.startswith("m_")][:30] if target else []
                }
        matches.append({"pathID":pathid,"gameObject":gobj["name"] if gobj else None,
                        "go_ptr":go,"script_ptr":pointer(tree.get("m_Script")),
                        "fields":targeted,"resolved_references":refs})
    featured=[g for g in go_byid.values() if g["name"] in COMPONENT_NAMES]
    return {
        "bundle":bundle_name,
        "serialized_objects":len(objects),
        "object_classes":dict(counts),
        "GameObject_readable":len(go_byid),
        "MonoBehaviour_readable":len(behaviours),
        "MonoScript_readable":len(scripts),
        "read_issues":dict(issues),
        "named_objects":featured[:30],
        "matching_components":matches[:50],
        "matching_component_count":len(matches),
        "notices":[
            "Unity PPtr pathIDs are serialized asset references, not memory offsets.",
            "A saved prefab is not a live player, camera matrix or network replica.",
            "Name/field matches cannot prove that the server sends obscured enemy positions."
        ]
    }

def main():
    p=argparse.ArgumentParser()
    p.add_argument("--assetpack",type=Path,required=True)
    p.add_argument("--out",type=Path,default=Path("objects-out/objects.json"))
    args=p.parse_args()
    total={"assetpack":args.assetpack.name,"bundles":[],"errors":[]}
    with zipfile.ZipFile(args.assetpack) as z:
        files=z.infolist()
        targets=[x for x in files if any(x.filename.split("/")[-1].startswith(t) for t in TARGET_BUNDLES)]
        if len(targets)!=len(TARGET_BUNDLES):
            total["errors"].append(f"Expected {len(TARGET_BUNDLES)} targeted bundles, got {len(targets)}")
        for entry in targets:
            try:
                with z.open(entry) as src: buf=src.read()
                b=inspect(buf,entry.filename)
                b["sha256"]=hashlib.sha256(buf).hexdigest()
                total["bundles"].append(b)
                print(json.dumps({k:b[k] for k in ("bundle","serialized_objects","GameObject_readable",
                      "MonoBehaviour_readable","matching_component_count","read_issues")}),flush=True)
            except Exception as e:
                total["errors"].append({"bundle":entry.filename,"error":type(e).__name__+": "+str(e)})
                print(f"ERROR {entry.filename}: {e}",flush=True)
    args.out.parent.mkdir(parents=True,exist_ok=True)
    args.out.write_text(json.dumps(total,indent=2,ensure_ascii=False)+"\n")
    found=sum(x["matching_component_count"] for x in total["bundles"])
    failures=sum(sum(x["read_issues"].values()) for x in total["bundles"])
    print(json.dumps({"targeted_bundles":len(total["bundles"]),"interesting_objects":found,
                      "read_issues":failures,"fatal_errors":len(total["errors"])}),flush=True)
    # The script is a research probe: a nonzero result communicates missing object parsing.
    if total["errors"] or found==0:raise SystemExit(1)

if __name__=="__main__":main()
