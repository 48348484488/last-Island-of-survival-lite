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
    # Item/crafting panel lives in a DIFFERENT original Unity prefab.
    # Audit its actual children, sprite PPtrs and source atlas names.
    uiatlas=[(x,pack.getinfo(x).file_size) for x in names if x.startswith("assets/uiatlas/")]
    atlases=[x for x in uiatlas if re.search(r"bag|produce|build|common|tubiao|item|drawing",x[0],re.I)]
    print("LIOS_ORIGINAL_UI_ATLAS_CANDIDATES",json.dumps(atlases[:150]),flush=True)
    import UnityPy
    for filename in ("assets/ui/bagandbuildpanel.ab","assets/ui/blueprintbuildpanel.ab"):
     env=UnityPy.load(pack.read(filename))
     gos={};rects={};comps={};assets=[]
     for obj in env.objects:
      typ=obj.type.name
      if typ=="GameObject":
       d=obj.read_typetree()
       gos[str(obj.path_id)]={"name":d.get("m_Name"),
            "active":d.get("m_IsActive"),
            "components":[str(c.get("component",{}).get("m_PathID",0)) for c in d.get("m_Component",[])]}
      elif typ=="RectTransform":
       d=obj.read_typetree()
       rects[str(obj.path_id)]={"go":str(d.get("m_GameObject",{}).get("m_PathID",0)),
          "father":str(d.get("m_Father",{}).get("m_PathID",0)),
          "children":[str(c.get("m_PathID",0)) for c in d.get("m_Children",[])],
          "pos":d.get("m_AnchoredPosition"),"size":d.get("m_SizeDelta"),
          "scale":d.get("m_LocalScale"),"anchorMin":d.get("m_AnchorMin"),
          "anchorMax":d.get("m_AnchorMax"),"pivot":d.get("m_Pivot")}
      elif typ=="MonoBehaviour":
       try:
        d=obj.read_typetree()
        if "m_Sprite" in d or "m_Text" in d or "m_Color" in d:
         comps[str(obj.path_id)]={"go":str(d.get("m_GameObject",{}).get("m_PathID",0)),
             "name":d.get("m_Name"),"image":d.get("m_Sprite"),
             "text":d.get("m_Text"),"color":d.get("m_Color"),
             "enabled":d.get("m_Enabled")}
       except Exception:pass
      elif typ in ("Texture2D","Sprite"):
       try: assets.append({"type":typ,"name":str(obj.read().m_Name),"id":str(obj.path_id)})
       except Exception:pass
     a={"file":filename,"objects":gos,"rects":rects,"components":comps,"assets":assets}
     path=Path(filename).stem+"_craft_original_layout.json"
     path.write_text(json.dumps(a,ensure_ascii=False,indent=2))
     highlights=[(i,v) for i,v in gos.items() if re.search(
       r"^((m_)?produce|btn_|scp_|(m_)?bag|item|txt_|m_create|m_drawing|m_material|create|type|m_part)",v["name"] or "",re.I)]
     print("LIOS_CRAFT_PREFAB_DETAIL",filename,
           "go",len(gos),"rects",len(rects),"comp",len(comps),
           "internal_sprites",assets[:18],
           "important_nodes",json.dumps(highlights[:140],ensure_ascii=False),flush=True)
    Path("source_ui_craft_index.json").write_text(json.dumps(dict(ui=ui,cfg=cfg,atlases=uiatlas),ensure_ascii=False,indent=2))
