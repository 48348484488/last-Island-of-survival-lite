#!/usr/bin/env python3
"""Extract EXACT original LITE 7.3 level-1 build surface textures.

Source pointers stay in manifest. Never substitute non-original procedural
textures if source assets are missing. Log whether Mesh lives in each bundle.
"""
import tempfile,shutil,zipfile,json,re,hashlib
from pathlib import Path
from collections import Counter
import UnityPy
ROOT=Path(__file__).resolve().parents[1]
OUT=Path("original_build_surfaces")
OUT.mkdir(exist_ok=True)
PARTS=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(PARTS)==17
SOURCE={
  "foundation":"component_diji_lv1",
  "wall":"component_qiangbi_lv1",
  "roof":"component_wuding_lv1",
  "window":"component_chuanghu_lv1",
  "doorway":"component_menkuang_lv1_d",
  "stair":"component_louti_d",
}
report=[]
with tempfile.TemporaryDirectory() as temp:
    join=Path(temp)/"original.xapk"
    with join.open("wb") as dst:
        for p in PARTS:
            with p.open("rb") as f:shutil.copyfileobj(f,dst,8*1024*1024)
    with zipfile.ZipFile(join) as z:
        pack=Path(temp)/"pack.apk"
        with z.open("install_time_asset_pack.apk") as f,pack.open("wb") as dst:
            shutil.copyfileobj(f,dst,8*1024*1024)
    with zipfile.ZipFile(pack) as z:
        for role,bundle in SOURCE.items():
            path="assets/partinfo_material/"+bundle+".ab"
            env=UnityPy.load(z.read(path))
            typecounts=Counter(x.type.name for x in env.objects)
            rows=[]
            for ob in env.objects:
                if ob.type.name!="Texture2D":continue
                try:
                    d=ob.read();name=str(d.m_Name)
                    im=d.image.convert("RGBA")
                    if im.width<32 or im.height<32:continue
                    rows.append((name,ob.path_id,im))
                except Exception as ex:
                    print("LIOS_SOURCE_BUILD_TEXTURE_ERROR",role,str(ex)[:180],flush=True)
            # Original *_ba / *_d is the albedo, not normal/metal channel.
            def rank(x):
                n=x[0].lower()
                return (0 if n.endswith(("_ba","_d","_diffuse","_basecolor")) else 1,
                        1 if n.endswith(("_ns","_me","_n","_metal")) else 0,
                        -(x[2].width*x[2].height))
            rows.sort(key=rank)
            info=dict(role=role,source_bundle=path,objects=dict(typecounts),candidates=[])
            for name,pid,im in rows:
                info["candidates"].append(dict(name=name,id=str(pid),size=[im.width,im.height]))
            if rows:
                name,pid,im=rows[0]
                fn="build_original_"+role+".png"
                im.save(OUT/fn,optimize=True)
                info.update(file=fn,source_texture_name=name,source_texture_id=str(pid),
                            sha256=hashlib.sha256(im.tobytes()).hexdigest())
            report.append(info)
            print("LIOS_ORIGINAL_BUILD_SURFACE",json.dumps(info,ensure_ascii=False),flush=True)
(OUT/"build_surface_manifest.json").write_text(json.dumps(report,indent=2,ensure_ascii=False))
print("LIOS_ORIGINAL_BUILD_SURFACES_EXPORTED",len([r for r in report if r.get("file")]),flush=True)
