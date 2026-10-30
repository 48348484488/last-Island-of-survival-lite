#!/usr/bin/env python3
"""Package only the OBJ/PNG assets referenced by the native Raylib mobile engine.

The complete 1.5 GB source XAPK, decompiled C# scripts, desktop NumPy NPZ,
and >1000 original Unity HUD sprite files are deliberately not packaged in
this incremental native Android port. The original 33-clip skeletal binary and HUD are added separately.
"""
import json
import re
import shutil
import sys
from pathlib import Path

def main():
    if len(sys.argv)!=4:
        raise SystemExit("Usage: package_android_assets.py main.cpp asset_source_dir destination")
    source=Path(sys.argv[1])
    root=Path(sys.argv[2])
    dest=Path(sys.argv[3])
    program=source.read_text(encoding="utf8")
    # Original audio WAVs are referenced from the native audio header,
    # not directly from main.cpp. Scan both actual source units so the
    # runtime never ships with silent missing assets.
    audio_header=source.parent/"mobile_audio.hpp"
    if not audio_header.is_file():
        raise FileNotFoundError(audio_header)
    program+="\n"+audio_header.read_text(encoding="utf8")
    names=sorted(set(re.findall(r'"assets/([^"\\]+\.(?:obj|png|wav|ogg))"',program)))
    # Generated skeletal binary and original Unity GUI sprites are copied from
    # recovered asset locations in the workflow, not the flat OBJ directory.
    generated={
        "hud_original_overlay_rgba.png","icon_cj_sf.png","icon_m4.png",
        "hud_original_melee.png","hud_original_rifle.png",
        "hud_original_build.png","hud_original_build_categories.png",
        "hud_original_build_selected.png","hud_original_build_rotate.png",
        "hud_original_build_children.png",
        "hud_original_build_items_type_2.png","hud_original_build_items_type_3.png",
        "hud_original_build_items_type_4.png","hud_original_build_items_type_6.png",
        "hud_original_build_items_type_7.png","hud_original_build_items_type_8.png",
        "build_icon_foundation.png","build_icon_wall.png",
        "build_icon_doorway.png","build_icon_roof.png",
        "build_icon_stair.png","build_icon_window.png",
        "build_icon_obstacle.png",
        "build_original_foundation.png","build_original_wall.png",
        "build_original_doorway.png","build_original_roof.png",
        "build_original_window.png","build_original_stair.png",
        "hud_original_joystick_knob.png",
        "hud_original_compass_strip.png","hud_original_hotbar_selection.png",
        "icon_resource_wood.png","icon_resource_metal.png",
        "icon_resource_stone.png","icon_resource_sulfur.png",
        "native_man_clothes_chushijuese_01_ba.png",
        "native_man_clothes_yf_0_ba.png",
        "native_man_trousers_chushijuese_01_ba.png",
        "native_man_shoes_chushijuese_01_ba.png",
        "native_man_head_chushijuese_01_ba.png",
        "native_man_hair_chushijuese_01_ba.png",
    }
    names=[n for n in names if n not in generated]
    if len(names)<20:
        raise ValueError("Native game references fewer assets than expected")
    dest.mkdir(parents=True,exist_ok=True)
    packaged=[]
    for name in names:
        rel=Path(name)
        if rel.is_absolute() or '..' in rel.parts or not rel.parts:
            raise ValueError("Unexpected asset path "+name)
        original=root/rel
        if not original.is_file():
            raise FileNotFoundError(original)
        if original.stat().st_size==0:
            raise ValueError("Empty original game asset: "+name)
        new=dest/rel
        new.parent.mkdir(parents=True,exist_ok=True)
        shutil.copyfile(original,new)
        packaged.append({"file":name,"bytes":original.stat().st_size})
    result={"native_game_source":str(source),
            "original_asset_count":len(packaged),
            "original_asset_bytes":sum(x["bytes"] for x in packaged),
            "files":packaged,
            "note":"33 original skeletal clips and Unity HUD are added separately by the native ARM64 workflow"}
    print("LIOS_ANDROID_ASSETS_VALIDATED",json.dumps(result,ensure_ascii=False),flush=True)

if __name__=="__main__":main()
