#!/usr/bin/env python3
"""Export genuine HUD Sprite bytes from independently verified original XAPK AssetBundles.
Source ABs are downloaded from prior forensic workflow, NEVER generated from UI drawings.
Keeps source IDs, color/alpha, duplicate-name entries and (when present) RectTransforms.
"""
import json,hashlib,io,re,collections
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
ORIGINAL=ROOT/"hud_original_comprovacao_xapk/unity_source_ab"
OUT=ROOT/"last_island_recreated_game/assets/hud_original_xapk"
OUT.mkdir(parents=True,exist_ok=True)
PNG=OUT/"sprites"
CACHED=ROOT/"last_island_recreated_game/assets/ui_hud"
ORIGINAL_SHA="73f007e97dcd9b0fa270912e3690ddefebb16b20b3826bb9ba1646642c1f50b6"
BUNDLES=[
 "battlepanel.ab","battlepanel2.ab","battlecustompanel.ab","common.ab",
 "tubiao.ab","tubiao1.ab",
 "battlepanel_kedu_yw.ab","map_map.ab"]
def safe(s):return re.sub(r'[^A-Za-z0-9_.-]','_',s)[:110]
def phash(im):return hashlib.sha256(im.convert("RGBA").tobytes()).hexdigest()
report={"source_xapk_sha256":ORIGINAL_SHA,"source":"Original XAPK / install_time_asset_pack.apk",
 "origin":"Unity original Sprite objects, NOT reconstructed JPG/PNG sprites",
 "asset_bundles":[],"sprites":[],"gameobjects":[],"recttransforms":[],"components":[],"warnings":[]}
def ptr(d):
 try:return str(d.get("m_PathID",0))
 except:return "0"
for filename in BUNDLES:
 src=ORIGINAL/filename
 if not src.exists():
  report["warnings"].append(f"Source AB missing: {filename}");continue
 e=UnityPy.load(str(src))
 counts=collections.Counter(o.type.name for o in e.objects)
 report["asset_bundles"].append({"file":filename,"sha256":hashlib.sha256(src.read_bytes()).hexdigest(),
   "objects":dict(counts)})
 spriteout=PNG/filename.replace(".ab","")
 spriteout.mkdir(parents=True,exist_ok=True)
 for o in e.objects:
  kind=o.type.name
  try:
   if kind=="Sprite":
    sprite=o.read();name=str(sprite.m_Name)
    im=sprite.image.convert("RGBA")
    path=spriteout/(safe(name)+"_"+str(o.path_id)+".png")
    im.save(path,optimize=True)
    row={"bundle":filename,"name":name,"path_id":str(o.path_id),
      "file":str(path.relative_to(OUT)),
      "width":im.width,"height":im.height,"pixel_sha256":phash(im)}
    cached=CACHED/(name+".png")
    if cached.exists():
     try:
      with Image.open(cached) as prev:
       row["cached_pixel_identical"]=prev.size==im.size and phash(prev)==row["pixel_sha256"]
     except Exception as ex:row["cached_error"]=str(ex)
    report["sprites"].append(row)
   elif kind=="GameObject":
    d=o.read_typetree()
    report["gameobjects"].append({"bundle":filename,"path_id":str(o.path_id),
      "name":str(d.get("m_Name","")),"component_ids":[ptr(c.get("component",{})) for c in d.get("m_Component",[])]})
   elif kind=="RectTransform":
    d=o.read_typetree()
    report["recttransforms"].append({"bundle":filename,"path_id":str(o.path_id),
     "game_object":ptr(d.get("m_GameObject",{})),"parent":ptr(d.get("m_Father",{})),
     "anchor_min":d.get("m_AnchorMin"),"anchor_max":d.get("m_AnchorMax"),
     "pivot":d.get("m_Pivot"),"size_delta":d.get("m_SizeDelta"),
     "anchored_position":d.get("m_AnchoredPosition")})
   elif kind=="MonoBehaviour":
    # Some components are serialized without type data; keep reference only.
    try:
     d=o.read_typetree()
     report["components"].append({"bundle":filename,"path_id":str(o.path_id),
      "object":ptr(d.get("m_GameObject",{})), "sprite":ptr(d.get("m_Sprite",{})),
      "type":str(d.get("m_Name","")),"keys":list(d.keys())[:20]})
    except Exception:pass
  except Exception as ex:
   report["warnings"].append(f"{filename}/{o.path_id} [{kind}]: {str(ex)[:110]}")
 print("SOURCE_BUNDLE_EXPORTED",filename,"counts",dict(counts),flush=True)

report["summary"]={
 "source_sprites":len(report["sprites"]),
 "same_pixels_as_cached":sum(v.get("cached_pixel_identical",False) for v in report["sprites"]),
 "different_pixels_from_cached":sum(v.get("cached_pixel_identical") is False for v in report["sprites"]),
 "rect_transform_count":len(report["recttransforms"]),
 "prefab_gameobjects":len(report["gameobjects"]),
 "unique_sprite_names":len(set(r["name"] for r in report["sprites"]))}
(OUT/"manifest_original.json").write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding="utf8")
# Create a proof contact sheet with source-extracted sprites. Label as Sprite proof, never gameplay.
source=report["sprites"]
pri=lambda r:(0 if r["bundle"] in ("battlepanel.ab","battlepanel2.ab","battlecustompanel.ab") else 1,
  0 if any(k in r["name"].lower() for k in ["battlepanel","attack","sprint","skill","map","cz_","buff","joystick","yaogan","zhunxing","bag"]) else 1,
  r["name"])
chosen=[]
seen=set()
for v in sorted(source,key=pri):
 if v["name"] in seen:continue
 chosen.append(v);seen.add(v["name"])
 if len(chosen)>=45:break
W=1580;columns=5;rows=(len(chosen)+columns-1)//columns;cellw=305;cellh=195
im=Image.new("RGB",(W,145+rows*cellh),(10,19,27))
draw=ImageDraw.Draw(im);font=ImageFont.load_default()
draw.text((30,20),"HUD LAST ISLAND — TEXTURAS VERIFICADAS DIRETAMENTE NO XAPK 7.3",fill=(242,242,241))
draw.text((30,45),"ORIGINAIS: Unity Sprite -> AssetBundle -> install_time_asset_pack.apk -> XAPK",fill=(167,221,173))
draw.text((30,68),"MONTAGEM DE AMOSTRAS (NAO E UM PRINT DO HUD DO JOGO). Sem redesenho de sprites.",fill=(255,187,110))
draw.text((30,91),f"{len(source)} sprites extraidos; {report['summary']['same_pixels_as_cached']} correspondencias exatas com o repositorio",fill=(163,188,205))
for i,v in enumerate(chosen):
 x=30+(i%columns)*309;y=132+(i//columns)*cellh
 draw.rounded_rectangle((x,y,x+295,y+183),radius=10,fill=(28,42,56),outline=(52,71,82),width=1)
 with Image.open(OUT/v["file"]) as sprite:
  s=sprite.convert("RGBA");s.thumbnail((268,129),Image.Resampling.LANCZOS)
  bg=Image.new("RGBA",s.size,(51,64,80,255))
  bg.alpha_composite(s)
  im.paste(bg.convert("RGB"),(x+(295-s.width)//2,y+8+(133-s.height)//2))
 draw.text((x+8,y+146),v["name"][:37],fill=(242,249,253),font=font)
 draw.text((x+8,y+162),f'{v["width"]}x{v["height"]} | {v["bundle"]}',fill=(164,195,210),font=font)
im.save(OUT/"prova_hud_original_sprites_xapk.png",optimize=True)

(OUT/"LEIA-ME.md").write_text("""# HUD original — fontes extraidas e verificadas

Origem: XAPK **Last Island of Survival LITE 7.3**, dividido em 17 partes,
verificado por SHA-256 no arquivo \`manifest_original.json\`.

Este diretório contém sprites decodificados **diretamente do Unity Sprite**
nos AssetBundles originais, separados por nome de bundle e PathID. Nada
foi desenhado para substituir as imagens.

A imagem \`prova_hud_original_sprites_xapk.png\` é uma folha de **amostras
autênticas**, NÃO um print da interface do jogo e NÃO comprova o layout.

O \`manifest_original.json\` tem lista de todos os sprites, hashes dos
pixels, GameObjects e RectTransforms recuperados, se presentes.

ATENÇÃO: o motor Python do jogo ainda usa a pasta \`assets/ui_hud\`.
A correspondência de nomes pode não ser única; alguns sprites com o
mesmo nome em bundles diferentes têm pixels distintos. Para reproduzir
o layout idêntico ao jogo, falta mapear as hierarquias dos GameObjects,
resolução, ancoragens de RectTransform e estados dinâmicos.
""",encoding="utf8")
print("HUD_ORIGINAL_EXTRACTED",json.dumps(report["summary"]),flush=True)
print("OUTPUT",OUT,flush=True)
