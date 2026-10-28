#!/usr/bin/env python3
"""Locate pixel-identical original weapon icons in XAPK Unity Sprite atlases.

Only the ORIGINAL install_time_asset_pack.apk is examined. A match is accepted
only if decoded RGBA pixels equal the existing cached item icon byte-for-byte.
This avoids guessing that a similarly named gun graphic is the same UI item.
Results include original AssetBundle, Sprite PathID, pixel SHA256, and PNG.
"""
from __future__ import annotations
import hashlib
import json
import re
import shutil
import tempfile
import zipfile
from collections import Counter
from pathlib import Path

import UnityPy
from PIL import Image

ROOT=Path(__file__).resolve().parents[1]
PARTS=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
ASSETS=ROOT/"last_island_recreated_game/assets"
OUT=ASSETS/"hud_original_xapk/complete_source_mapping"
PNG=OUT/"weapon_verified"
SOURCE_NAMES=("wq_shifu","m4_icon")
# The full pack contains many decorative weapon UI images. Limit initial scan
# to actual item-icon atlases instead of unrelated UI panel references.
ATLAS_NAMES=("gun.ab","equip.ab","common.ab","parts.ab","rareitem.ab",
             "gunpf.ab","equippart.ab")
assert len(PARTS)==17,len(PARTS)

def pixelhash(im):
    return hashlib.sha256(im.convert("RGBA").tobytes()).hexdigest()

def main():
    refs={}
    for name in SOURCE_NAMES:
        f=ASSETS/"ui_hud"/(name+".png")
        with Image.open(f) as png:
            refs[name]={"width":png.width,"height":png.height,
                        "pixel_sha256":pixelhash(png)}
    report={"source":"Last Island LITE 7.3 original XAPK install-time asset pack",
            "comparison":"decoded Sprite RGBA pixels, not PNG file bytes",
            "reference_cached_icons":refs,"scanned_bundles":[],
            "exact_pixel_matches":[],"name_only_candidates":[],
            "unsupported_replacements":[]}
    OUT.mkdir(parents=True,exist_ok=True)
    with tempfile.TemporaryDirectory(prefix="lios_original_icon_") as tmp:
        tmp=Path(tmp)
        merged=tmp/"original.xapk"
        with merged.open("wb") as out:
            for p in PARTS:
                with p.open("rb") as f:
                    shutil.copyfileobj(f,out,8*1024*1024)
        with zipfile.ZipFile(merged) as xapk:
            with xapk.open("install_time_asset_pack.apk") as src, (tmp/"assets.apk").open("wb") as out:
                shutil.copyfileobj(src,out,8*1024*1024)
        merged.unlink()
        with zipfile.ZipFile(tmp/"assets.apk") as archive:
            matches=[info for info in archive.infolist()
                     if info.filename.startswith("assets/icon/")
                     and Path(info.filename).name in ATLAS_NAMES]
            print("ICON_SOURCE_ATLASES",[(x.filename,x.file_size) for x in matches],flush=True)
            for info in matches:
                short=Path(info.filename).name
                target=tmp/short
                with archive.open(info) as src,target.open("wb") as out:
                    shutil.copyfileobj(src,out,4*1024*1024)
                row={"bundle":info.filename,"size":info.file_size,"sprites":0,"decoded":0,"errors":[]}
                report["scanned_bundles"].append(row)
                try:
                    bundle=UnityPy.load(str(target))
                    for ob in bundle.objects:
                        if ob.type.name!="Sprite":continue
                        row["sprites"]+=1
                        try:
                            s=ob.read()
                            name=str(s.m_Name)
                            # Decode all Sprite pixel data to allow exact comparison,
                            # including names unrelated to the original cache names.
                            img=s.image.convert("RGBA")
                            row["decoded"]+=1
                            digest=pixelhash(img)
                            matched=[key for key,v in refs.items() if
                                     v["width"]==img.width and v["height"]==img.height
                                     and v["pixel_sha256"]==digest]
                            if matched:
                                file=PNG/(short[:-3]+"_"+str(ob.path_id)+".png")
                                file.parent.mkdir(parents=True,exist_ok=True)
                                img.save(file,optimize=True)
                                report["exact_pixel_matches"].append({
                                    "matched_reference_icons":matched,
                                    "sprite_name":name,"original_sprite_path_id":str(ob.path_id),
                                    "original_bundle":info.filename,
                                    "width":img.width,"height":img.height,
                                    "pixel_sha256":digest,
                                    "source_png":str(file.relative_to(ASSETS/"hud_original_xapk")),
                                })
                            elif re.search(r"shifu|m4_1|m4icon|m4_icon|stone.?axe|futou|\bm4\b",name,re.I):
                                report["name_only_candidates"].append({
                                    "name":name,"bundle":info.filename,
                                    "path_id":str(ob.path_id),"dimensions":[img.width,img.height],
                                    "pixel_sha256":digest})
                        except Exception as e:
                            if len(row["errors"])<12:
                                row["errors"].append({"path_id":str(ob.path_id),"error":str(e)[:150]})
                except Exception as e:
                    row["errors"].append({"bundle_error":str(e)[:200]})
                finally:
                    target.unlink(missing_ok=True)
                print("ICON_ATLAS_SCAN",json.dumps({
                    "bundle":short,"sprites":row["sprites"],"decoded":row["decoded"],
                    "errors":len(row["errors"])}),flush=True)
    proven={name for rec in report["exact_pixel_matches"]
            for name in rec["matched_reference_icons"]}
    report["verified_original_ui_items"]=sorted(proven)
    report["not_pixel_verified"]=sorted(set(refs)-proven)
    dest=OUT/"original_weapon_icon_atlas_provenance.json"
    dest.write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding="utf8")
    print("ORIGINAL_WEAPON_ICON_PIXEL_AUDIT",json.dumps({
        "verified":report["verified_original_ui_items"],
        "missing":report["not_pixel_verified"],
        "original_matches":report["exact_pixel_matches"],
        "named_candidates":len(report["name_only_candidates"])}),flush=True)
if __name__=="__main__":main()
