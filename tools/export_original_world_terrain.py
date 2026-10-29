#!/usr/bin/env python3
"""Export actual Unity world terrain Mesh and Texture2D from owned LITE 7.3 XAPK.

225-ish source sector bundles (A/B/C/D) have baked X/Y ground coordinates
in OBJ meshes and -90-degree X Transform; convert metadata to Raylib Y-up.
Keep data in dedicated original_world folder, no randomized map geometry.
This DOES NOT restore sceneinfo object placement or unported structure prefabs.
"""
import json,re,zipfile,tempfile,shutil,math
from pathlib import Path
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"last_island_recreated_game/assets/original_world"
R=re.compile(r"^assets/scene/(?:[a-d]-\d+|junshijidi_terrain_\d+|z-\d+)\.ab$",re.I)

def bounds_of_obj(raw):
    # UnityPy's exported mesh uses Z-up. Original Unity scene has Rx(-90)
    # rotation, converting vertex (x, y, z) to native world (x, z, -y).
    points=[]
    for line in raw.splitlines():
        if line.startswith("v "):
            try:
                x,y,z=map(float,line.split()[1:4])
                if not all(map(math.isfinite,(x,y,z))):raise ValueError("Non-finite")
                points.append((x,z,-y))
            except ValueError:continue
    if len(points)<3:return None
    return [min(v[0] for v in points),max(v[0] for v in points),
            min(v[2] for v in points),max(v[2] for v in points),
            min(v[1] for v in points),max(v[1] for v in points)]

def convert(scene,data):
    env=UnityPy.load(data)
    names=[];issues=[]
    textures=[]
    for obj in env.objects:
        if obj.type.name!="Texture2D":continue
        try:
            tex=obj.read()
            image=tex.image
            fname=f"{scene}_tex{len(textures)}.png"
            image.save(OUT/fname)
            textures.append(fname)
        except Exception as exc:issues.append("tex:"+str(exc)[:140])
    count=0;manifest=[]
    for obj in env.objects:
        if obj.type.name!="Mesh":continue
        try:
            mesh=obj.read()
            data=mesh.export()
            bbox=bounds_of_obj(data)
            if bbox is None:continue
            index=count
            fname=f"{scene}_mesh{index}.obj"
            (OUT/fname).write_text(data,encoding="utf8",newline="")
            selected_tex=textures[min(index,len(textures)-1)] if textures else "-"
            manifest.append((fname,selected_tex,*bbox))
            count+=1
        except Exception as exc:issues.append("mesh:"+str(exc)[:200])
    return manifest,issues

def main():
    OUT.mkdir(parents=True,exist_ok=True)
    parts=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
    assert len(parts)==17
    with tempfile.TemporaryDirectory() as tmp:
        joined=Path(tmp)/"original.xapk"
        with joined.open("wb") as dest:
            for part in parts:
                with part.open("rb") as source:
                    shutil.copyfileobj(source,dest,length=8*1024*1024)
        with zipfile.ZipFile(joined) as z:
            inner=Path(tmp)/"pack.apk"
            with z.open("install_time_asset_pack.apk") as source,inner.open("wb") as dest:
                shutil.copyfileobj(source,dest,length=8*1024*1024)
        summary=[];lines=[]
        with zipfile.ZipFile(inner) as pack:
            targets=sorted(n for n in pack.namelist() if R.match(n))
            print("LIOS_WORLD_ORIGINAL_SELECTED_SECTORS",len(targets),flush=True)
            for i,path in enumerate(targets):
                name=Path(path).stem
                try:
                    models,issues=convert(name,pack.read(path))
                    for m in models:
                        lines.append(f"{m[0]} {m[1]} "+ " ".join(f"{v:.5f}" for v in m[2:]))
                    summary.append({"scene":path,"models":len(models),"issues":issues})
                except Exception as exc:summary.append({"scene":path,"error":str(exc)[:250]})
                if i%25==0:
                    print("LIOS_WORLD_ORIGINAL_EXTRACTION_PROGRESS",i,len(targets),
                          "models",len(lines),flush=True)
        (OUT/"map_tiles.txt").write_text("# OBJ TEXTURE MIN_X MAX_X MIN_Z MAX_Z MIN_Y MAX_Y\n"+
                                       "\n".join(lines)+"\n",encoding="utf8")
        (OUT/"export_report.json").write_text(json.dumps(summary,indent=2),encoding="utf8")
        successes=sum(item.get("models",0)>0 for item in summary)
        missing=[item for item in summary if item.get("models",0)==0]
        print("LIOS_WORLD_ORIGINAL_EXTRACTED",json.dumps({
             "source_sectors":len(targets),"successful_sectors":successes,
             "exported_meshes":len(lines),
             "unresolved":missing[:20],
             "directory_bytes":sum(p.stat().st_size for p in OUT.iterdir() if p.is_file())
        }),flush=True)
        assert len(lines)>150,("Too few authentic world terrain meshes",len(lines))
if __name__=="__main__":main()
