#!/usr/bin/env python3
"""Reconstruct four real Unity terrain Mesh+Texture2D source assets for validation.

The sceneinf/*.oc sector positions are NOT included in these original XAPK
archives; this script reports source local transform and mesh bounds, no
fabricated world placement. Save original geometry as Wavefront OBJ and PNG.
"""
import json,zipfile,tempfile,shutil,math
from pathlib import Path
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
TARGET=["a-01","a-02","a-03","a-04"]
OUT=Path("original_terrain_preview")
def preview(name,raw):
    env=UnityPy.load(raw)
    result={"name":name,"objects":[],"meshes":[],"textures":[],"transforms":[]}
    for o in env.objects:
        if o.type.name=="Mesh":
            m=o.read()
            try:
                obj=m.export()
                if not obj or not obj.startswith("#") and not any(v.startswith("v ") for v in obj.splitlines()[:30]):
                    raise ValueError("Empty/invalid OBJ mesh export")
                path=OUT/(name+".obj")
                path.write_text(obj,encoding="utf8",newline="")
                vs=[ln for ln in obj.splitlines() if ln.startswith("v ")]
                fs=sum(ln.startswith("f ") for ln in obj.splitlines())
                sample=[list(map(float,v.split()[1:4])) for v in vs]
                bounds=[[min(v[i] for v in sample),max(v[i] for v in sample)] for i in range(3)]
                result["meshes"].append({"mesh":str(m.m_Name),"vertices":len(vs),
                                          "faces":fs,"bounds_xyz":bounds,"obj_bytes":path.stat().st_size})
            except Exception as e:result["meshes"].append({"error":str(e)[:300]})
        if o.type.name=="Texture2D":
            t=o.read()
            try:
                image=t.image
                path=OUT/(name+".png")
                image.save(path)
                result["textures"].append({"name":str(t.m_Name),"size":image.size,
                                           "png_bytes":path.stat().st_size})
            except Exception as e:result["textures"].append({"error":str(e)[:300]})
        if o.type.name=="Transform":
            t=o.read_typetree()
            result["transforms"].append({"name":str(o.path_id),
               "local_position":t.get("m_LocalPosition"),
               "local_scale":t.get("m_LocalScale"),
               "local_rotation":t.get("m_LocalRotation")})
    print("LIOS_ORIGINAL_SOURCE_SCENE_PREVIEW",json.dumps(result,ensure_ascii=False)[:6500],flush=True)
    return result
def main():
    OUT.mkdir(exist_ok=True)
    parts=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
    assert len(parts)==17
    with tempfile.TemporaryDirectory() as tmp:
        joined=Path(tmp)/"source.xapk"
        with joined.open("wb") as f:
            for part in parts:
                with part.open("rb") as p:shutil.copyfileobj(p,f,length=8*1024*1024)
        with zipfile.ZipFile(joined) as outer:
            inner=Path(tmp)/"pack.apk"
            with outer.open("install_time_asset_pack.apk") as f,inner.open("wb") as g:
                shutil.copyfileobj(f,g,length=8*1024*1024)
        with zipfile.ZipFile(inner) as z:
            report=[preview(name,z.read("assets/scene/"+name+".ab")) for name in TARGET]
    (OUT/"terrain_preview_report.json").write_text(json.dumps(report,indent=2),encoding="utf8")
    assert all(x["meshes"] for x in report),report
if __name__=="__main__":main()
