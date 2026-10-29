#!/usr/bin/env python3
"""Inspect original LITE 7.3 Player-build part icons in icon/buildpart*.ab"""
from pathlib import Path
import zipfile,tempfile,shutil,re,json
import UnityPy
parts=sorted(Path("xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17
with tempfile.TemporaryDirectory() as td:
 p=Path(td)/"original.xapk"
 with p.open("wb") as out:
  for part in parts:
   with part.open("rb") as f:shutil.copyfileobj(f,out,8*1024*1024)
 with zipfile.ZipFile(p) as outer:
  pack=Path(td)/"pack.apk"
  with outer.open("install_time_asset_pack.apk") as i,pack.open("wb") as o:shutil.copyfileobj(i,o,8*1024*1024)
 with zipfile.ZipFile(pack) as z:
  paths=[n for n in z.namelist() if re.match(r"assets/icon/(?:buildpart|parts)\w*\.ab$",n)]
  infos=[]
  output=Path("original_part_icons");output.mkdir(exist_ok=True)
  for path in paths:
   env=UnityPy.load(z.read(path))
   icons=[]
   for o in env.objects:
    if o.type.name not in ("Sprite","Texture2D"):continue
    try:
     data=o.read();name=str(data.m_Name)
     if o.type.name=="Sprite":
      im=data.image.convert("RGBA")
      if im.width<24 or im.height<24:continue
      outname="particon_"+str(o.path_id)+"_"+re.sub(r"[^a-zA-Z0-9_]+","_",name)[:60]+".png"
      im.save(output/outname,optimize=True)
      icons.append(dict(name=name,id=str(o.path_id),file=outname,size=[im.width,im.height]))
     else:icons.append(dict(name=name,id=str(o.path_id),type="Texture2D"))
    except Exception as exc:print("ICON_ERROR",path,str(exc)[:120],flush=True)
   infos.append(dict(bundle=path,icons=icons))
   print("LIOS_ORIGINAL_BUILD_PART_ICONS",path,len(icons),json.dumps(icons[:120],ensure_ascii=False)[:16000],flush=True)
  (output/"part_icons_manifest.json").write_text(json.dumps(infos,ensure_ascii=False,indent=2))
  print("LIOS_ORIGINAL_BUILD_PART_BUNDLES",len(infos),flush=True)
