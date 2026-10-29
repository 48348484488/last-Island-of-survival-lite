#!/usr/bin/env python3
"""Inspect original LITE terrain AssetBundles and construction prefab directories.
Do not misidentify 16 military-base terrain bundles as the entire 6000m map.
Output metadata only, never fake map coordinates or a made-up elevation model.
"""
from collections import Counter
from pathlib import Path
import tempfile,zipfile,shutil,json
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
PARTS=ROOT/"xapk_original"
def scan_bundle(data,name):
    env=UnityPy.load(data)
    types=Counter(o.type.name for o in env.objects)
    meshes=[];textures=[];objects=[]
    for o in env.objects:
        if o.type.name=="Mesh":
            try:
                m=o.read()
                n=int(len(m.m_Vertices)) if hasattr(m,"m_Vertices") else 0
                meshes.append({"name":str(getattr(m,"m_Name","")),"path_id":str(o.path_id),
                               "vertices_read":n})
            except Exception as e:meshes.append({"error":str(e)[:180]})
        elif o.type.name=="Texture2D":
            try:
                t=o.read()
                textures.append({"name":str(t.m_Name),"width":int(t.m_Width),
                                 "height":int(t.m_Height)})
            except Exception as e:textures.append({"error":str(e)[:150]})
        elif o.type.name=="GameObject":
            try:objects.append(str(o.read().m_Name))
            except Exception:pass
    return {"bundle":name,"counts":dict(types),"mesh_samples":meshes[:25],
            "texture_samples":textures[:25],"game_objects":objects[:35]}
def main():
    parts=sorted(PARTS.glob("Last_Island_LITE_7.3.xapk.part*"))
    assert len(parts)==17
    with tempfile.TemporaryDirectory() as tmp:
        joined=Path(tmp)/"source.xapk"
        with joined.open("wb") as f:
            for part in parts:
                with part.open("rb") as p:shutil.copyfileobj(p,f,length=8*1024*1024)
        with zipfile.ZipFile(joined) as outer:
            nested=next(x for x in outer.namelist() if x.endswith("install_time_asset_pack.apk"))
            ap=Path(tmp)/"asset-pack.apk"
            with outer.open(nested) as f,ap.open("wb") as o:
                shutil.copyfileobj(f,o,length=8*1024*1024)
        with zipfile.ZipFile(ap) as pack:
            infos=pack.infolist()
            folders=Counter("/".join(i.filename.split("/")[:2]) for i in infos)
            # There can be additional scene bundles (including environment or
            # patch variants); audit every one rather than assuming exactly 16.
            candidates=[i.filename for i in infos if i.filename.startswith("assets/scene/") and i.filename.endswith(".ab")]
            assert len(candidates)>=16,("Original scene bundles missing",len(candidates))
            report={"source_asset_pack":nested,
                    "directories":folders.most_common(),
                    "scenes":[]}
            for name in candidates:
                try:
                    scan=scan_bundle(pack.read(name),name)
                    report["scenes"].append(scan)
                    print("LIOS_SOURCE_TERRAIN_BUNDLE",json.dumps(scan,ensure_ascii=False)[:2200],flush=True)
                except Exception as e:
                    report["scenes"].append({"bundle":name,"error":str(e)})
            Path("original_terrain_mesh_audit.json").write_text(json.dumps(report,indent=2),encoding="utf8")
            print("LIOS_ORIGINAL_ASSET_GROUPS",json.dumps(folders.most_common(60)),flush=True)
            print("LIOS_TERRAIN_BUNDLE_COUNT",len(report["scenes"]),flush=True)
if __name__=="__main__":main()
