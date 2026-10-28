#!/usr/bin/env python3
"""Regenerate Android overlays and C++ RectTransforms from original Unity battlepanel.ab.

Never substitute hand-placed HUD button, hotbar, icon or status rectangles.
The two output RGBA canvases are rendered by the SAME source-driven V7
pipeline as the desktop game; changing the weapon switches the entire
Unity-selected profile and selected original hotbar sprite.

Only the selected on-foot states are reconstructed (not every runtime UI).
Font glyphs, runtime GameObject activation, CanvasScaler/device cutout
and shader masks may still differ from the running original APK.
"""
import argparse
import importlib.util
import json
import math
from pathlib import Path
from PIL import Image

ROOT=Path(__file__).resolve().parents[1]
GAME=ROOT/"last_island_recreated_game"
ASSETS=GAME/"assets/hud_original_xapk"
import sys
sys.path.insert(0,str(GAME))
from original_hud_bindings import original_touch_rects, _layout
from original_hud_touch import original_joystick

spec=importlib.util.spec_from_file_location("ilios_v7",ROOT/"tools/render_original_hud_v7.py")
v7=importlib.util.module_from_spec(spec);spec.loader.exec_module(v7)
HudV7=v7.HudV7

ACTIONS={
    "attack":"Attack","jump":"Jump","auto_run":"Sprint",
    "crouch":"Crouch","prone":"Prone","inventory":"Inventory",
    "craft":"Craft","aim":"Aim","reload":"Reload",
    "settings":"Settings","quick_chat":"QuickChat",
}
TEXTS={"Health":"txt_mybloodNum","Food":"txt_hunger","Water":"txt_bodyWater"}
ITEMS=[{"sprite":"cj_sf","count":1},{"sprite":"m4","count":1}]+[None]*5
RESOURCES={"Wood":"tb_cl_mt","Metal":"tb_cl_tiek","Stone":"tb_cl_st","Sulfur":"ks_lhk"}

def source_rect(hud,go_id):
    go_id=str(go_id)
    b=hud.bygo[go_id]
    x,y,w,h=hud.rect(b["path_id"])
    return {"left":x-w/2,"top":hud.canvas[1]-y-h/2,"right":x+w/2,
            "bottom":hud.canvas[1]-y+h/2}

def fmt(x): return "%.6ff"%float(x)

def literal_rect(b):
    return "{"+",".join(fmt(b[k]) for k in ("left","top","right","bottom"))+"}"

def text_info(hud,name):
    go=hud.go(name)
    original=hud.game_objects[go["path_id"]]
    text=[hud.source_comps[p["path_id"]] for p in original["components"]
          if p["path_id"] in hud.source_comps
          and hud.source_comps[p["path_id"]]["type"]=="Text"]
    if len(text)!=1:raise ValueError("Missing original text component "+name)
    c=text[0];fd=c["font_data"];col=c.get("color") or {}
    return {"bounds":source_rect(hud,go["path_id"]),
        "font_size":int(fd["m_FontSize"]),"align":int(fd["m_Alignment"]),
        "font_style":int(fd.get("m_FontStyle") or 0),
        "color":[max(0,min(255,round(float(col.get(k,1))*255)))
                 for k in ("r","g","b","a")],
        "go_path_id":go["path_id"],
        "text_component_path_id":c["source_path_id"]}

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("--destination",type=Path,required=True)
    args=ap.parse_args()
    dest=args.destination;dest.mkdir(parents=True,exist_ok=True)
    geometry={}
    hud=HudV7(ASSETS,heading_deg=0,map_uv=[.5,.5])
    icons=[]
    for i in range(7):
        go=hud.go("m_Item"+str(i+1))
        child=hud._descendant(go["path_id"],"m_icon")
        num=hud._descendant(go["path_id"],"txt_num")
        icons.append({"index":i,"icon":source_rect(hud,child),
                      "count":text_info(hud,hud.game_objects[num]["name"]) if False else
                      _text_for_id(hud,num),
                      "unity_item_path_id":go["path_id"]})
    geometry["hotbar"]=icons
    geometry["texts"]={k:text_info(hud,n) for k,n in TEXTS.items()}
    geometry["buttons"]={}
    for profile in ("on-foot-melee","on-foot-rifle"):
        actions=original_touch_rects(profile=profile)
        for action,rect in actions.items():
            prev=geometry["buttons"].get(action)
            rect_info={k:float(rect[k]) for k in ("left","top","right","bottom")}
            rect_info["go_path_id"]=rect["gameobject_path_id"]
            if prev is not None and prev!=rect_info:
                raise ValueError("Source UGUI action bounds differ across profiles: "+action)
            geometry["buttons"][action]=rect_info
        # Each original canvas includes the exact source-selected 7-slot bar.
        # No text here: numeric values are redrawn live from the C++ state.
        layer=HudV7(ASSETS,heading_deg=0,map_uv=[.5,.5])
        layer.selected_slot=0 if profile=="on-foot-melee" else 1
        audit=layer.render_ordered(profile=profile,items=ITEMS,vitals=None,samples=False)
        outfile=dest/("hud_original_melee.png" if profile=="on-foot-melee" else "hud_original_rifle.png")
        layer.layer.save(outfile,optimize=True)
        if layer.layer.size!=(1920,1080):raise AssertionError("Unexpected Unity Canvas")
        print("LIOS_MOBILE_ORIGINAL_HUD_OVERLAY",profile,outfile,
              "operations",audit["drawn_operations"],"size",outfile.stat().st_size,flush=True)
        geometry.setdefault("profiles",{})[profile]={
            "drawn_components":audit["source_image_components_drawn_unique"],
            "draw_calls":audit["drawn_operations"],
            "source_hierarchy_order":audit["source_hierarchy_order"],
        }
    geometry["joystick"]=original_joystick()
    geometry["resource_icons"]={}
    for name,sprite in RESOURCES.items():
        entries=hud.sprite_names.get(sprite)
        if not entries:raise ValueError("Source resource Sprite not found: "+sprite)
        entry=sorted(entries,key=lambda a:(a.get("bundle",""),a["file"]))[0]
        path=ASSETS/entry["file"]
        if not path.is_file():raise FileNotFoundError(path)
        with Image.open(path) as img:
            im=img.convert("RGBA")
            im.save(dest/("icon_resource_"+name.lower()+".png"),optimize=True)
        geometry["resource_icons"][name]={
            "source_sprite_name":sprite,"path_id":entry["path_id"],
            "source_file":entry["file"]}
    (dest/"hud_mobile_source_geometry.json").write_text(
        json.dumps(geometry,indent=2,ensure_ascii=False),encoding="utf8")
    lines=["#pragma once",
           "// GENERATED automatically from original Unity battlepanel.ab source.",
           "// No hand-written positions. Regenerate with export_mobile_original_hud.py.",
           "namespace LIOS_HUD {",
           "struct Rect {float left,top,right,bottom;};",
           "struct Text {Rect bounds;int fontSize;int align;unsigned char r,g,b,a;};"]
    for action,rc in geometry["buttons"].items():
        lines.append("static constexpr Rect "+ACTIONS[action]+"="+literal_rect(rc)+";")
    for i,item in enumerate(icons):
        lines.append("static constexpr Rect Slot"+str(i)+"="+literal_rect(item["icon"])+";")
        info=item["count"];col=info["color"]
        lines.append("static constexpr Text SlotCount"+str(i)+"={"+literal_rect(info["bounds"])+
                     ",%d,%d,%d,%d,%d,%d};"%(info["font_size"],info["align"],*col))
    for key,info in geometry["texts"].items():
        col=info["color"]
        lines.append("static constexpr Text "+key+"={"+literal_rect(info["bounds"])+
                     ",%d,%d,%d,%d,%d,%d};"%(info["font_size"],info["align"],*col))
    j=geometry["joystick"];cx,cy=j["center"]
    lines.append("static constexpr Rect JoystickTrigger="+literal_rect(j["trigger"])+";")
    lines.append("static constexpr float JoystickCenterX="+fmt(cx)+";")
    lines.append("static constexpr float JoystickCenterY="+fmt(cy)+";")
    lines.append("static constexpr float JoystickRadius="+fmt(j["radius"])+";")
    lines.append("} // namespace LIOS_HUD")
    h=dest/"mobile_original_hud_layout.hpp";h.write_text("\n".join(lines)+"\n",encoding="utf8")
    # Generated provenance guards:
    assert len(icons)==7 and len(geometry["buttons"])==11
    assert all(p["draw_calls"]>40 for p in geometry["profiles"].values())
    print("LIOS_MOBILE_ORIGINAL_HUD_LAYOUT_PASS",
          "actions",len(geometry["buttons"]),"slots",len(icons),
          "text_rects",len(geometry["texts"]),
          "source_joystick_radius",j["radius"],
          "resource_sprites",len(RESOURCES),
          "output",h,flush=True)

def _text_for_id(hud,go_id):
    """Some seven source txt_num GameObjects share a name; resolve via PathID."""
    obj=hud.game_objects[str(go_id)]
    cs=[hud.source_comps[p["path_id"]] for p in obj["components"]
        if p["path_id"] in hud.source_comps
        and hud.source_comps[p["path_id"]]["type"]=="Text"]
    if len(cs)!=1:raise ValueError("HUD count text missing at "+str(go_id))
    c=cs[0];fd=c["font_data"];color=c.get("color") or {}
    return {"bounds":source_rect(hud,go_id),
            "font_size":int(fd["m_FontSize"]),
            "align":int(fd["m_Alignment"]),
            "color":[max(0,min(255,round(float(color.get(k,1))*255)))
                     for k in ("r","g","b","a")],
            "go_path_id":str(go_id)}

if __name__=="__main__":main()
