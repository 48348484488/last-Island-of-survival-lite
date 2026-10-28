#!/usr/bin/env python3
"""Audit source of actual Last Island LITE HUD images; never invent or draw game sprites.
Uses full original XAPK parts and previously exported ui_hud PNG sprites.
Builds an explicit CONTACT SHEET (not a screenshot claiming original layout).
"""
import io,json,os,hashlib,zipfile,tempfile,shutil,re
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageOps
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"hud_original_auditoria"
OUT.mkdir(parents=True,exist_ok=True)
SOURCE=ROOT/"last_island_recreated_game/assets/ui_hud"
SPRITES=[
 "BattlePanel_attack_button.png","BattlePanel_sprint_button.png",
 "BattlePanel_aim_button.png","1BattlePanel_cz_t.png",
 "1BattlePanel_cz_dun.png","1BattlePanel_cz_pa.png",
 "BattlePanel2_car_yaogan_1.png","BattlePanel2_car_yaogan_2.png",
 "1BattlePanel_buff_xue.png","1BattlePanel_buff_jie.png","1BattlePanel_buff_shui.png",
 "1BattlePanel_buttle_fang_1.png","1BattlePanell_bb.png","1BattlePanel_map_di_2_1.png",
 "1BattlePanel_tq_bg.png","1BattlePanel_zuo_shang_infoBG_1.png",
 "BattlePanel_cxfmt_zhunxing.png","battlepanel_kedu_yw.png",
 "wq_shifu.png","m4_icon.png","BattlePanel2_YINGSHUI2.png",
 "1BattlePanel_di_cb.png","1BattlePanel_xue_bg_1.png","1BattlePanel_map_di_2_2.png",
 "1BattlePanel_zhouye_ty.png","1BattlePanel_zhouye_yl.png",
 "BattlePanel_joystick.png","1BattlePanel_setting_shiqu2.png",
 "1BattlePanel_tb_ck.png","1BattlePanel_tb_db.png",
]
report={"type":"forensic asset audit","note":"The poster is an asset contact sheet, NOT an exact original in-game HUD screenshot.",
        "local_existing_sprite_dir":str(SOURCE.relative_to(ROOT)),"sprites":[],"xapk":{},"original_ui_bundles":[]}
for p in sorted(SOURCE.glob("*.png")):
 try:
  with Image.open(p) as im:
   report["sprites"].append({"name":p.name,"size":list(im.size),
       "sha256":hashlib.sha256(p.read_bytes()).hexdigest(),
       "alpha":("A" in im.getbands())})
 except Exception as ex: report.setdefault("errors",[]).append({p.name:str(ex)})
assert len(report["sprites"])>=1000, "Expected source-recovered sprites missing"
W,H=1600,1395
canvas=Image.new("RGB",(W,H),(12,19,26));dr=ImageDraw.Draw(canvas)
dr.text((42,25),"LAST ISLAND LITE 7.3 - SPRITES DE HUD DO REPOSITORIO",fill=(231,244,255))
dr.text((42,56),"PROVA VISUAL DAS TEXTURAS EXISTENTES | NAO E CAPTURA DO HUD ORIGINAL NO JOGO",fill=(255,196,123))
dr.text((42,81),"Cada item abaixo e um PNG sem redesenho. Validacao contra XAPK: veja auditoria.json",fill=(153,180,202))
font=ImageFont.load_default()
cols=5;cellw=300;cellh=204;starty=125
for idx,name in enumerate(SPRITES):
 if idx>=30:break
 row,col=divmod(idx,cols)
 x,y=42+col*310,starty+row*207
 dr.rounded_rectangle((x,y,x+296,y+195),radius=10,fill=(26,39,52),outline=(62,82,98),width=1)
 p=SOURCE/name
 if not p.exists():
  dr.text((x+12,y+82),"NAO ENCONTRADO",fill=(247,129,132),font=font)
 else:
  with Image.open(p) as image:
   original=image.convert("RGBA")
   original.thumbnail((257,143),Image.Resampling.LANCZOS)
   # checkerboard for transparent sprites, no changes to original colors/pixels
   bg=Image.new("RGBA",original.size,(49,62,78,255))
   for cx in range(0,original.width,12):
    for cy in range(0,original.height,12):
     if ((cx//12)+(cy//12))%2==0:
      ImageDraw.Draw(bg).rectangle((cx,cy,cx+11,cy+11),fill=(64,78,95,255))
   bg.alpha_composite(original)
   canvas.paste(bg.convert("RGB"),(x+(296-original.width)//2,y+5+(145-original.height)//2))
   dr.text((x+9,y+150),name[:38],fill=(235,247,255),font=font)
   dr.text((x+9,y+171),f"{image.width} x {image.height} px | PNG existente",fill=(151,195,211),font=font)
canvas.save(OUT/"hud_sprites_existentes_sem_redesenho.png",optimize=True)
report["poster"]={"file":"hud_sprites_existentes_sem_redesenho.png","files_present":sum((SOURCE/n).is_file() for n in SPRITES),"requested_samples":len(SPRITES)}
# Original XAPK reassembly/examination: examine ZIP entries, then original OBB ZIP, if present.
partdir=ROOT/"xapk_original"
parts=sorted(partdir.glob("Last_Island_LITE_7.3.xapk.part*"))
report["xapk"]["parts"]=len(parts)
report["xapk"]["total_size"]=sum(p.stat().st_size for p in parts)
if parts:
 merged=Path(tempfile.gettempdir())/"last_island_original_hud_audit.xapk"
 with merged.open("wb") as dst:
  for p in parts:
   with p.open("rb") as fd:shutil.copyfileobj(fd,dst,length=8*1024*1024)
 report["xapk"]["sha256"]=hashlib.sha256(merged.read_bytes()).hexdigest()
 with zipfile.ZipFile(merged) as z:
  entries=z.infolist()
  report["xapk"]["entry_count"]=len(entries)
  report["xapk"]["entries"]=[{"name":i.filename,"size":i.file_size} for i in entries[:60]]
  nested=[i for i in entries if i.filename.endswith((".obb",".zip"))]
  report["xapk"]["obb_candidates"]=[i.filename for i in nested]
  for part in nested[:2]:
   tmp=Path(tempfile.gettempdir())/("hud_"+Path(part.filename).name)
   with z.open(part) as src,tmp.open("wb") as fd:
    shutil.copyfileobj(src,fd,length=8*1024*1024)
   try:
    with zipfile.ZipFile(tmp) as inner:
     files=inner.namelist()
     report["xapk"].setdefault("obb_indexes",[]).append({
       "file":part.filename,"count":len(files),
       "sample":files[:20]})
     wanted=[s for s in files if s.lower().endswith(".ab") and
        any(k in s.lower() for k in ("battlepanel","battlecustom","battlepanel2","common.ab","tubiao.ab","map_map.ab","battleplane"))]
     report["original_ui_bundles"] += wanted
     for member in wanted:
      target=OUT/"original_unity_ab"/Path(member).name
      target.parent.mkdir(exist_ok=True,parents=True)
      with inner.open(member) as src,target.open("wb") as fd:
       shutil.copyfileobj(src,fd,length=4*1024*1024)
   except zipfile.BadZipFile:
    report["xapk"].setdefault("notes",[]).append(f"Nested file {part.filename} is not a ZIP.")
   finally:tmp.unlink(missing_ok=True)
 merged.unlink(missing_ok=True)
report["bundles_extracted"]=[{"file":p.name,"size":p.stat().st_size}
 for p in (OUT/"original_unity_ab").glob("*.ab")] if (OUT/"original_unity_ab").exists() else []
(OUT/"auditoria.json").write_text(json.dumps(report,indent=2,ensure_ascii=False),encoding="utf8")
print("HUD_PNG_FILES",len(report["sprites"]))
print("HUD_CONTACT_SHEET",str(OUT/"hud_sprites_existentes_sem_redesenho.png"))
print("XAPK_META",json.dumps(report["xapk"],ensure_ascii=False)[:3700])
print("EXTRACTED_SOURCE_BUNDLES",report["bundles_extracted"])
