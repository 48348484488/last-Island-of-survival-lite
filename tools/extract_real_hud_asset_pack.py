#!/usr/bin/env python3
"""Rebuild actual XAPK, inspect modern install_time_asset_pack.apk, extract Unity UI bundles,
and verify cached original HUD sprites by pixel hash. Never generate replacement sprites.
"""
import tempfile,zipfile,hashlib,shutil,json,io,os
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"hud_original_comprovacao_xapk"
OUT.mkdir(exist_ok=True)
cached=ROOT/"last_island_recreated_game/assets/ui_hud"
partdir=ROOT/"xapk_original"
parts=sorted(partdir.glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17
report={"source":"Original Last Island LITE 7.3 XAPK split into 17 parts",
 "source_parts":len(parts),"original_nested_apks":[],"chosen_asset_bundles":[],
 "sprite_pixel_matches":[],"sprite_names_not_identified":[],"issues":[]}
def digest(path):
 h=hashlib.sha256()
 with open(path,"rb") as f:
  for piece in iter(lambda:f.read(8*1024*1024),b""):h.update(piece)
 return h.hexdigest()
def render_from_original(refs):
 # This is strictly a sample contact sheet of independent original bytes.
 W=1500;H=150+((len(refs)+4)//5)*194
 H=max(330,H)
 canvas=Image.new("RGB",(W,H),"#111b25"); d=ImageDraw.Draw(canvas)
 d.text((20,20),"ORIGINAL UNITY SPRITES — EXTRAIDOS DO ASSET PACK DO XAPK",fill="#ffffff")
 d.text((20,43),"Contato dos sprites originais. NAO e captura de gameplay nem layout validado.",fill="#fbbf7d")
 d.text((20,64),"Sprite -> Unity AssetBundle -> XAPK (SHA-256 registrado no relatorio).",fill="#b2cadc")
 for idx,item in enumerate(refs):
  if idx>=35:break
  x=20+(idx%5)*298;y=100+(idx//5)*194
  d.rounded_rectangle((x,y,x+286,y+184),radius=8,fill="#22303f")
  im=Image.open(OUT/"source_png"/item["file"]).convert("RGBA")
  im.thumbnail((260,135),Image.Resampling.LANCZOS)
  bg=Image.new("RGBA",im.size,"#354759");bg.alpha_composite(im)
  canvas.paste(bg.convert("RGB"),(x+(286-im.width)//2,y+7+(136-im.height)//2))
  d.text((x+7,y+149),item["name"][:34],fill="#ffffff")
  d.text((x+7,y+164),f'{item["dimensions"][0]}x{item["dimensions"][1]} — {item["bundle"][:17]}',fill="#bdcddd")
 canvas.save(OUT/"hud_texturas_comprovadas_no_xapk.png")
def copy_member(z,info,target):
 target.parent.mkdir(parents=True,exist_ok=True)
 with z.open(info) as s,target.open("wb") as d:
  shutil.copyfileobj(s,d,length=4*1024*1024)
 return target
def main():
 merged=Path(tempfile.gettempdir())/"last_island_rebuild_verify.xapk"
 with merged.open("wb") as dst:
  for p in parts:
   with p.open("rb") as fd:shutil.copyfileobj(fd,dst,length=8*1024*1024)
 report["xapk_sha256"]=digest(merged)
 print("XAPK_REASSEMBLED",merged.stat().st_size,report["xapk_sha256"],flush=True)
 with zipfile.ZipFile(merged) as xapk:
  members=[i for i in xapk.infolist() if i.filename.endswith(".apk")]
  for info in members:
   if not ("asset_pack" in info.filename or info.filename.startswith("com.")):continue
   tmp=Path(tempfile.gettempdir())/("last_island_hud_"+Path(info.filename).name)
   copy_member(xapk,info,tmp)
   nested={"name":info.filename,"bytes":tmp.stat().st_size}
   with zipfile.ZipFile(tmp) as assetpack:
    items=assetpack.infolist()
    nested["members"]=len(items)
    nested["first_40_paths"]=[x.filename for x in items[:40]]
    ab=[i for i in items if i.filename.lower().endswith(".ab")]
    nested["bundle_count"]=len(ab)
    nested["matching_ui_ab"]=[i.filename for i in ab if any(k in i.filename.lower()
       for k in ("battlepanel","common.ab","tubiao","map_map","battlecustom","battleplane","ui/"))][:250]
    possible=[i for i in ab if any(k in i.filename.lower()
       for k in ("battlepanel","battlecustom","battleplane","common.ab","tubiao","map_map"))]
    for file in possible:
     local=OUT/"unity_source_ab"/Path(file.filename).name
     copy_member(assetpack,file,local)
     report["chosen_asset_bundles"].append({"nested_apk":info.filename,
       "path":file.filename,"file":local.name,"size":local.stat().st_size,"sha256":digest(local)})
   report["original_nested_apks"].append(nested)
   print("ASSET_PACK_INSPECTED",json.dumps(nested,ensure_ascii=False)[:3500],flush=True)
   tmp.unlink(missing_ok=True)
 merged.unlink(missing_ok=True)
 # Pixel-verified source Sprite extraction against cached ui_hud PNGs, if Unity bundles found
 targets={"BattlePanel_attack_button","BattlePanel_sprint_button","BattlePanel_aim_button",
 "1BattlePanel_cz_t","1BattlePanel_cz_dun","1BattlePanel_cz_pa",
 "BattlePanel2_car_yaogan_1","BattlePanel2_car_yaogan_2",
 "1BattlePanel_buff_xue","1BattlePanel_buff_jie","1BattlePanel_buff_shui",
 "1BattlePanel_buttle_fang_1","1BattlePanell_bb","1BattlePanel_map_di_2_1",
 "1BattlePanel_tq_bg","BattlePanel_cxfmt_zhunxing","battlepanel_kedu_yw",
 "1BattlePanel_xue_bg_1","1BattlePanel_di_cb","wq_shifu"}
 source_asset_files=list((OUT/"unity_source_ab").glob("*.ab")) if (OUT/"unity_source_ab").exists() else []
 if source_asset_files:
  import UnityPy
  for bundle in source_asset_files:
   print("SOURCE_BUNDLE",bundle.name,bundle.stat().st_size,flush=True)
   try:asset=UnityPy.load(str(bundle))
   except Exception as ex:
    report["issues"].append({"bundle":bundle.name,"load_error":str(ex)});continue
   for obj in asset.objects:
    if obj.type.name!="Sprite":continue
    try:
     s=obj.read();name=str(s.m_Name)
     if name not in targets:continue
     im=s.image.convert("RGBA")
     pngpath=OUT/"source_png"/(name+".png")
     pngpath.parent.mkdir(parents=True,exist_ok=True);im.save(pngpath)
     row={"name":name,"bundle":bundle.name,"path_id":str(obj.path_id),
       "dimensions":list(im.size),"file":pngpath.name,"source_sha256":digest(pngpath)}
     old=cached/(name+".png")
     if old.exists():
      with Image.open(old) as cachedim:
       old_rgba=cachedim.convert("RGBA")
       row["cached_pixel_identical"]=bool(old_rgba.size==im.size and old_rgba.tobytes()==im.tobytes())
       row["existing_sha256"]=digest(old)
     report["sprite_pixel_matches"].append(row)
    except Exception as e:
     report["issues"].append({"bundle":bundle.name,"path":str(obj.path_id),"error":str(e)})
 if report["sprite_pixel_matches"]:
  render_from_original(report["sprite_pixel_matches"])
 report["result"]={"ui_bundles":len(report["chosen_asset_bundles"]),
   "source_sprites":len(report["sprite_pixel_matches"]),
   "pixel_identical_to_repo":sum(x.get("cached_pixel_identical",False) for x in report["sprite_pixel_matches"])}
 (OUT/"comprovacao.json").write_text(json.dumps(report,indent=2,ensure_ascii=False),encoding="utf8")
 print("FORENSIC_RESULT",json.dumps(report["result"]))
 print("OUTPUT_IMAGE_EXISTS",(OUT/"hud_texturas_comprovadas_no_xapk.png").is_file())
 if not report["chosen_asset_bundles"]:print("WARNING: NO AB MATCHES; SEE REPORT ASSET PACK INDEX")
if __name__=="__main__":main()
