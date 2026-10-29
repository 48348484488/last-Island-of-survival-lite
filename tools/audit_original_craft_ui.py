#!/usr/bin/env python3
"""Index original install-time UI bundles and locate the actual BagAndBuildPanel.
All source files read from the original, unchanged 17-part XAPK.
"""
import json,zipfile,shutil,tempfile,os,re
from pathlib import Path
SRC=Path("xapk_original")
parts=sorted(SRC.glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17,len(parts)
with tempfile.TemporaryDirectory() as td:
 t=Path(td);zpath=t/"full.xapk"
 with zpath.open("wb") as dest:
  for p in parts:
   with p.open("rb") as source:shutil.copyfileobj(source,dest,8*1024*1024)
 with zipfile.ZipFile(zpath) as outer:
  for name in outer.namelist():
   if not name.endswith("install_time_asset_pack.apk"):continue
   sub=t/"pack.apk"
   with outer.open(name) as source,sub.open("wb") as dest:shutil.copyfileobj(source,dest,8*1024*1024)
   with zipfile.ZipFile(sub) as pack:
    names=pack.namelist()
    ui=[(x,pack.getinfo(x).file_size) for x in names if x.startswith("assets/ui/")]
    cfg=[(x,pack.getinfo(x).file_size) for x in names if "DrawingCfg" in x or "ItemCfg" in x]
    print("LIOS_ORIGINAL_UI_BUNDLE_COUNT",len(ui),flush=True)
    print("LIOS_ORIGINAL_UI_BUNDLE_NAMES",json.dumps(ui[:600],ensure_ascii=False),flush=True)
    print("LIOS_ORIGINAL_RECIPE_CONFIGS",json.dumps(cfg[:100],ensure_ascii=False),flush=True)
    if os.getenv("INSPECT_UI"):
     import UnityPy
     for x,n in ui:
      if not x.lower().endswith(".ab") or n>18_000_000:continue
      try:
       env=UnityPy.load(pack.read(x))
       gos=[]
       for ob in env.objects:
        if ob.type.name!="GameObject":continue
        gos.append(str(ob.read().m_Name))
       matches=[y for y in gos if re.search(r"bag|craft|make|prod|draw|build|zhiz|zhizao|item|list|scro|tab|create|blue|shez|type|kuai",y,re.I)]
       if re.search(r"bag|produce|craft|build|item|backpack|drawing",x,re.I) or len(matches)>17:
        print("LIOS_ORIGINAL_UI_PREFAB",x,"size",n,"objects",len(gos),"names",json.dumps(matches[:180],ensure_ascii=False),flush=True)
      except Exception as ex:
       print("LIOS_UI_INSPECT_FAIL",x,str(ex)[:150],flush=True)
    Path("source_ui_craft_index.json").write_text(json.dumps(dict(ui=ui,cfg=cfg),ensure_ascii=False,indent=2))
