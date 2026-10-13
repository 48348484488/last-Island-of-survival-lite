#!/usr/bin/env python3
"""Audit real serialized camera controller fields and Unity external fileID mapping.

Only reads original Unity assets. Never executes code in the game.
A static MonoScript name is not a runtime C# method or memory address.
"""
import argparse
from collections import defaultdict,Counter
import hashlib,json,zipfile
from pathlib import Path

SCRIPT_NAMES={
    ("Oxide","FPCameraController"),("Oxide","MouseLook"),
    ("Oxide","CameraManager"),("Oxide","RootHeightHandler"),
    ("Oxide","PlayerManager")
}
PREFIXES=("core-common-local_assets_","core-tutorial_assets_","scenes-tutorial_scenes_")

def get_field(o,key,default=None):
    if isinstance(o,dict):return o.get(key,default)
    return getattr(o,key,default)

def safe_value(v,depth=0):
    if depth>3:return "(max depth)"
    if v is None or type(v) in (str,int,float,bool):
        return v[:150] if type(v)==str else v
    if isinstance(v,dict):
        p=v.get("m_PathID")
        f=v.get("m_FileID")
        if type(p)==int and type(f)==int:
            return {"fileID":f,"pathID":p}
        return {str(k):safe_value(x,depth+1) for k,x in list(v.items())[:24]}
    if isinstance(v,(list,tuple)):
        return [safe_value(x,depth+1) for x in v[:24]]
    return repr(v)[:150]

def asset_of(obj):
    # UnityPy ObjectReader's owning SerializedFile, NOT the outer BundleFile.
    return getattr(obj,"assets_file",None)

def ext_summary(externals):
    """Enumerate Unity external refs with original 1-based fileID."""
    if externals is None:return []
    if isinstance(externals,dict):
        pairs=list(externals.items())
    elif isinstance(externals,(list,tuple)):
        pairs=list(enumerate(externals,1))
    else:
        try:pairs=list(enumerate(externals,1))
        except TypeError:return []
    results=[]
    for idx,e in pairs[:64]:
        details={}
        for k in ("name","file_name","path","path_name","guid","type"):
            v=get_field(e,k)
            if v is not None:details[k]=str(v)[:200]
        results.append({"fileID":idx,"info":details or repr(e)[:220]})
    return results

def file_description(file):
    return {"name":str(get_field(file,"name",""))[:220],
            "file_type":type(file).__name__,
            "externalReferences":ext_summary(get_field(file,"externals")),
            "externalType":type(get_field(file,"externals")).__name__}

def registry(data):
    import UnityPy
    env=UnityPy.load(data)
    scripts=defaultdict(list)
    errors=[]
    serialized_names=set()
    for obj in env.objects:
        owner=asset_of(obj)
        name=get_field(owner,"name")
        if name:serialized_names.add(str(name))
        if obj.type.name!="MonoScript":continue
        try:
            t=obj.read_typetree()
            if not isinstance(t,dict):continue
            scripts[obj.path_id].append({
                "namespace":t.get("m_Namespace",""),
                "class":t.get("m_ClassName",""),
                "assembly":t.get("m_AssemblyName","")
            })
        except Exception as e:
            errors.append({"pathID":obj.path_id,"error":type(e).__name__})
    return scripts,errors,sorted(serialized_names)

def pointer(x):
    if not isinstance(x,dict):return None
    p=x.get("m_PathID");f=x.get("m_FileID")
    if type(p)==int and type(f)==int:return {"fileID":f,"pathID":p}
    return None

def summarize_bundle(buf,name,scripts):
    import UnityPy
    env=UnityPy.load(buf)
    objects=list(env.objects)
    file_infos={}
    # Keep ownership in each object and avoid pathID collision between
    # multiple SerializedFiles inside a UnityFS BundleFile.
    for o in objects:
        af=asset_of(o)
        if af is not None:
            key=id(af)
            if key not in file_infos:file_infos[key]=file_description(af)
    selected=[]
    all_class_refs=Counter()
    failed=[]
    for obj in objects:
        if obj.type.name!="MonoBehaviour":continue
        try:
            t=obj.read_typetree()
            if not isinstance(t,dict):continue
        except Exception as e:
            failed.append({"pathID":obj.path_id,"error":type(e).__name__})
            continue
        sp=pointer(t.get("m_Script"))
        if not sp:continue
        classes=scripts.get(sp["pathID"],[])
        matching=[c for c in classes if (c["namespace"],c["class"]) in SCRIPT_NAMES]
        if not matching:continue
        all_class_refs[matching[0]["class"]]+=1
        af=asset_of(obj)
        extinfo=file_infos.get(id(af),{})
        external_refs=extinfo.get("externalReferences",[])
        matched_file=next((x for x in external_refs
                           if x["fileID"]==sp["fileID"]),None)
        fields={k:safe_value(v) for k,v in t.items() if k not in ("m_GameObject","m_Script")}
        selected.append({
            "pathID":obj.path_id,
            "gameObjectPointer":pointer(t.get("m_GameObject")),
            "classMatches":matching,
            "scriptPointer":sp,
            "externalFileForScript":matched_file,
            "externalFileTableLength":len(external_refs),
            "ownerSerializedFile":extinfo.get("name"),
            "fields":fields
        })
    return {"bundle":name,"serializedObjectCount":len(objects),
            "registeredObjectFiles":len(file_infos),"serializedFiles":list(file_infos.values()),
            "targetComponents":selected,"class_counts":dict(all_class_refs),
            "readErrors":failed[:30]}

def run(apk,out):
    with zipfile.ZipFile(apk) as z:
        infos=z.infolist()
        monos=[i for i in infos if i.filename.endswith(".bundle") and "monoscripts" in i.filename.lower()]
        if not monos:raise ValueError("Original asset pack missing MonoScripts bundle")
        registry_dict=defaultdict(list)
        mono_serialized_names=set()
        for info in monos:
            with z.open(info) as fh:buf=fh.read()
            scripts,errors,serialized_names=registry(buf)
            mono_serialized_names.update(serialized_names)
            if errors:raise ValueError(f"MonoScript read errors: {errors[:3]}")
            for pathid,items in scripts.items():registry_dict[pathid].extend(items)
        targets=[i for i in infos if any(i.filename.rsplit("/",1)[-1].startswith(p)
                                         for p in PREFIXES)]
        if len(targets)!=len(PREFIXES):
            raise ValueError(f"Expected {len(PREFIXES)} asset bundles; got {len(targets)}")
        bundles=[]
        for member in targets:
            with z.open(member) as fh:buf=fh.read()
            result=summarize_bundle(buf,member.filename,registry_dict)
            result["sha256"]=hashlib.sha256(buf).hexdigest()
            bundles.append(result)
            print(json.dumps({"bundle":member.filename,
                 "components":len(result["targetComponents"]),
                 "class_counts":result["class_counts"],
                 "assetFiles":len(result["serializedFiles"])}),flush=True)
    items=[x for b in bundles for x in b["targetComponents"]]
    for item in items:
        ext=item["externalFileForScript"]
        name=(ext.get("info") or {}).get("name") if ext else None
        item["MonoScriptCABVerified"]=bool(name and name in mono_serialized_names)
    report={"MonoScriptCount":len(registry_dict),
            "MonoScriptSerializedFiles":sorted(mono_serialized_names),
            "validatedCABMappings":sum(x["MonoScriptCABVerified"] for x in items),
            "targetedBundles":bundles,
            "targetComponentCount":len(items),
            "withScriptFileEntry":sum(c["externalFileForScript"] is not None for c in items),
            "missingExternalFileEntry":sum(c["externalFileForScript"] is None and c["scriptPointer"]["fileID"]>0 for c in items),
            "cautions":["Unity external fileID indexing is per owning SerializedFile, not a global mono script table.",
                        "External entry name must match MonoScripts bundle before treating pathID as resolved foreign key.",
                        "Serialized controller fields do not include a live projection matrix or enemy coordinates."]}
    out.parent.mkdir(parents=True,exist_ok=True)
    out.write_text(json.dumps(report,indent=2,ensure_ascii=False)+"\n")
    print(json.dumps({"actualMonoScriptCABs":sorted(mono_serialized_names),
                      "verifiedCABs":report["validatedCABMappings"]}),flush=True)
    print(json.dumps({"targetComponentCount":report["targetComponentCount"],
                      "withExternalFileEntry":report["withScriptFileEntry"],
                      "missingExternalFileEntry":report["missingExternalFileEntry"]}),flush=True)
    if not items:raise SystemExit("No target camera scripts found")
    if report["validatedCABMappings"]!=len(items):
        raise SystemExit("Unverified external CAB ownership for one or more script pointers")

if __name__=="__main__":
    p=argparse.ArgumentParser()
    p.add_argument("--apk",type=Path,required=True)
    p.add_argument("--out",type=Path,default=Path("camera-contract/report.json"))
    a=p.parse_args()
    run(a.apk,a.out)
