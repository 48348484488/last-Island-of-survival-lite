#!/usr/bin/env python3
"""Reverse-engineering audit of BUILDABLE pieces in original LITE 7.3 asset pack.
Only use source Unity asset IDs/names. Distinguish world scenery prefabs from
player-placeable parts; both may contain 'building' in their path.
"""
from pathlib import Path
from collections import Counter
import shutil,zipfile,tempfile,re,json,os
import UnityPy
ROOT=Path(__file__).resolve().parents[1]
parts=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17
with tempfile.TemporaryDirectory() as td:
    p=Path(td)/"original.xapk"
    with p.open("wb") as out:
        for part in parts:
            with part.open("rb") as f:shutil.copyfileobj(f,out,8*1024*1024)
    with zipfile.ZipFile(p) as root:
        pp=Path(td)/"pack.apk"
        with root.open("install_time_asset_pack.apk") as f,pp.open("wb") as dst:shutil.copyfileobj(f,dst,8*1024*1024)
    with zipfile.ZipFile(pp) as z:
        files=z.namelist()
        ab=[x for x in files if x.endswith(".ab")]
        folders=Counter(x.split("/")[1] for x in ab if "/" in x)
        print("LIOS_ORIGINAL_ASSET_DIRECTORIES",json.dumps(folders.most_common(100)),flush=True)
        interesting=re.compile(r'part|component|foundation|wall|floor|stair|build|wood|stone|metal|tiet|muwu|qiang|jianz|chuang|diban|menkuang|tiejia|shitou|mushi|tizi|muqiang|ziyuan',re.I)
        candidates=[x for x in ab if interesting.search(Path(x).name) and not x.startswith("assets/scene/")]
        bydir=Counter(x.split("/")[1] for x in candidates)
        print("LIOS_BUILD_CANDIDATE_FOLDERS",json.dumps(bydir.most_common(60)),flush=True)
        for x in candidates:
            if x.startswith(("assets/sceneprefab/","assets/scenemodel/")):continue
            print("LIOS_BUILD_ASSET",x,"bytes",z.getinfo(x).file_size,flush=True)
        strong=[x for x in ab if re.search(
          r"assets/(?:part|parts|building|build|construction|prefab|models|model|res|itemmodel|role|weapon)/",x,re.I)
                and (interesting.search(x) or "/build" in x)]
        scan=[]
        for path in sorted(set(strong))[:250]:
            if z.getinfo(path).file_size>10_000_000:continue
            try:
                env=UnityPy.load(z.read(path));types=Counter(o.type.name for o in env.objects)
                meshes=[];textures=[];materials=[]
                for obj in env.objects:
                    typ=obj.type.name
                    if typ not in ("Mesh","Texture2D","Material","GameObject"):continue
                    try:
                        name=str(obj.read().m_Name)
                    except Exception:continue
                    if typ=="Mesh":meshes.append(dict(name=name,id=str(obj.path_id)))
                    if typ=="Texture2D":textures.append(dict(name=name,id=str(obj.path_id)))
                    if typ=="Material":materials.append(dict(name=name,id=str(obj.path_id)))
                row=dict(path=path,types=dict(types),mesh=meshes[:20],textures=textures[:20],materials=materials[:20])
                scan.append(row)
                if meshes or textures:print("LIOS_BUILD_MESH_SOURCE",json.dumps(row,ensure_ascii=False)[:4500],flush=True)
            except Exception as e:
                print("LIOS_BUILD_SCAN_ERROR",path,str(e)[:180],flush=True)
        Path("original_construction_mesh_audit.json").write_text(
           json.dumps(dict(folders=dict(folders),candidate_count=len(candidates),
                           candidates=candidates,scanned=scan),indent=2,ensure_ascii=False))
        print("LIOS_BUILD_SOURCE_AUDIT_DONE",len(scan),flush=True)
