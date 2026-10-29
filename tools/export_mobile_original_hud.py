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
from PIL import Image, ImageChops

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

class NativeMobileHud(HudV7):
    """Keep original thumb and compass texture separate for native runtime."""
    hide_build_categories=False
    hide_hotbar=False
    def render_compass(self):
        # Original compass moves with yaw. Draw it in C++ using the generated
        # exact-source 72-frame atlas, never bake a compass pointing north.
        return None
    def node(self,go,depth=3,exclude=()):
        name=go if isinstance(go,str) else go["name"]
        if name=="m_littleYaogan":
            return
        if self.hide_build_categories and name=="m_buildPartTypes":
            return
        if self.hide_hotbar and name=="m_bagItems":
            return
        return super().node(go,depth=depth,exclude=exclude)

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
        # The source contains multiple nested GameObjects named Background.
        # Unity hotbar() explicitly draws the DIRECT child named Background;
        # resolve by parent RectTransform rather than a recursive name search.
        direct=[]
        for tid in hud.bygo[go["path_id"]]["children"]:
            rect=hud.t.get(tid)
            if rect and hud.g.get(rect["go"],{}).get("name")=="Background":
                direct.append(rect["go"])
        if len(direct)!=1:
            raise ValueError(f"Unity m_Item{i+1} direct Background not unique: {direct}")
        background=direct[0]
        num=hud._descendant(go["path_id"],"txt_num")
        # m_ItemN is a zero-size Unity anchor! Its child Background is the
        # actual visible 95x95 hotbar hit area. Using the parent prevented
        # selection on Android even though the item icon was rendered.
        icons.append({"index":i,"slot":source_rect(hud,background),
                      "icon":source_rect(hud,child),
                      "count":text_info(hud,hud.game_objects[num]["name"]) if False else
                      _text_for_id(hud,num),
                      "unity_item_path_id":go["path_id"]})
    geometry["hotbar"]=icons
    # Each of the seven Unity m_ItemN objects has its own m_heightLignt
    # Image. Extract the REAL selected-state graphic per slot. The atlas
    # lets the Android renderer move the highlight without baking a fake
    # or leaving two slots highlighted when the user switches equipment.
    selection_tiles=[]
    for i in range(7):
        item_go=hud.go("m_Item"+str(i+1))
        selected_go=hud._descendant(item_go["path_id"],"m_heightLignt")
        bounds=source_rect(hud,selected_go)
        crop=tuple(round(bounds[k]) for k in ("left","top","right","bottom"))
        selection_layer=HudV7(ASSETS,heading_deg=0,map_uv=[.5,.5])
        selection_layer.node(selection_layer.g[selected_go],depth=0)
        tile=selection_layer.layer.crop(crop)
        if tile.width<=0 or tile.height<=0 or tile.getbbox() is None:
            raise ValueError("Original Unity selection highlight absent: "+str(i))
        selection_tiles.append(tile)
        icons[i]["selected_bounds"]=bounds
        icons[i]["selected_source_path_id"]=selected_go
    selection_width=max(tile.width for tile in selection_tiles)
    selection_height=max(tile.height for tile in selection_tiles)
    selection_atlas=Image.new("RGBA",(7*selection_width,selection_height))
    for i,tile in enumerate(selection_tiles):
        selection_atlas.alpha_composite(tile,(i*selection_width,0))
    selection_atlas.save(dest/"hud_original_hotbar_selection.png",optimize=True)
    geometry["selection_atlas"]={"slots":7,"tile_width":selection_width,
                                 "tile_height":selection_height}
    print("LIOS_UNITY_ORIGINAL_HOTBAR_SELECTION",selection_atlas.size,
          "slots",7,flush=True)
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
        # Draw the original base WITHOUT the selected sprite; the seven
        # source-correct m_heightLignt graphics are composited at runtime.
        # No text here: numeric values are redrawn live from the C++ state.
        layer=NativeMobileHud(ASSETS,heading_deg=0,map_uv=[.5,.5])
        layer.selected_slot=-1
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
    # Original source Unity Build HUD. The prefab includes runtime-managed
    # children of m_buildPartTypes initially stacked at one X coordinate.
    # Export these as independent source sprites, then arrange on device
    # across the ORIGINAL HorizontalLayoutGroup 797.8 source-width anchor.
    # Rendering all prefab children in one static PNG made 8 tabs overlap.
    build=NativeMobileHud(ASSETS,heading_deg=0,map_uv=[.5,.5])
    build.hide_build_categories=True
    build.hide_hotbar=True   # Base Layer build UI replaces live hotbar at bottom.
    build.selected_slot=-1
    build_audit=build.render_ordered(profile="on-foot-build",
                                    items=ITEMS,vitals=None,samples=False)
    outfile=dest/"hud_original_build.png"
    build.layer.save(outfile,optimize=True)
    if outfile.stat().st_size<1000:
        raise ValueError("Original Build HUD was not rendered")
    geometry["profiles"]["on-foot-build"]={
        "drawn_components":build_audit["source_image_components_drawn_unique"],
        "draw_calls":build_audit["drawn_operations"],
        "source_hierarchy_order":build_audit["source_hierarchy_order"]}
    # RectTransforms of exact original controls and derived position strip.
    geometry["build_controls"]={}
    for name in ("btn_build","btn_freeBuildModel","btn_opRotate","m_buildPartTypes"):
        go=hud.go(name)
        geometry["build_controls"][name]={
            "bounds":source_rect(hud,go["path_id"]),
            "go_path_id":go["path_id"],
            "source_parent":hud.t[hud.bygo[go["path_id"]]["father"]]["go"]
                if hud.bygo[go["path_id"]]["father"] in hud.t else None}
    names=("btn_null","btn_lantu","btn_fundation","btn_wall",
           "btn_floor","btn_stair","btn_window","btn_obstacle")
    bounds=geometry["build_controls"]["m_buildPartTypes"]["bounds"]
    count=len(names)
    selection_sprites=[]
    tiles=[]
    class SingleSourceTab(NativeMobileHud):
        def node(self,go,depth=3,exclude=()):
            name=go if isinstance(go,str) else go["name"]
            # Runtime Toggle groups activate their own Checkmark only.
            if name=="Checkmark":return
            return super().node(go,depth=depth,exclude=exclude)
    for i,name in enumerate(names):
        obj=hud.go(name)
        assert hud.t[hud.bygo[obj["path_id"]]["father"]]["go"]==hud.go("m_buildPartTypes")["path_id"]
        tab=SingleSourceTab(ASSETS,heading_deg=0,map_uv=[.5,.5])
        tab.node(obj,depth=4)
        source=source_rect(hud,obj["path_id"])
        src=(round(source["left"]),round(source["top"]),
             round(source["right"]),round(source["bottom"]))
        tile=tab.layer.crop(src)
        if tile.getbbox() is None:
            raise ValueError("Empty source original Build category: "+name)
        tiles.append(tile)
        # The ROOT source layout rect is 757.9 pixels wide after hierarchy scale.
        # Slots are N equal-width columns; source button 95 pixels each.
        width=(bounds["right"]-bounds["left"])/count
        slot=dict(left=bounds["left"]+width*i,top=bounds["top"],
                  right=bounds["left"]+width*(i+1),bottom=bounds["bottom"])
        geometry["build_controls"][name]={"bounds":slot,"go_path_id":obj["path_id"],
                                          "original_unity_tab":name,
                                          "source_image_bounds":source}
    w=max(x.width for x in tiles);h=max(x.height for x in tiles)
    atlas=Image.new("RGBA",(len(tiles)*w,h))
    for i,tile in enumerate(tiles):
        atlas.alpha_composite(tile,(i*w,0))
    atlas.save(dest/"hud_original_build_categories.png",optimize=True)
    selected_go=hud._descendant(hud.go("btn_fundation")["path_id"],"Checkmark")
    selected_layer=NativeMobileHud(ASSETS,heading_deg=0,map_uv=[.5,.5])
    selected_layer.node(selected_layer.g[selected_go],depth=0)
    rb=source_rect(hud,selected_go)
    crop=tuple(round(rb[key]) for key in ("left","top","right","bottom"))
    check=selected_layer.layer.crop(crop)
    if check.getbbox() is None:
        raise ValueError("Unity Toggle.Checkmark original source is missing")
    check.save(dest/"hud_original_build_selected.png",optimize=True)
    geometry["build_category_selected"]={
        "source_toggle_child":"btn_fundation/Checkmark",
        "source_gameobject_path_id":selected_go,
        "source_pixel_bounds":rb,
        "asset":"hud_original_build_selected.png"}
    geometry["build_category_atlas"]={"count":len(names),"tile_width":w,"tile_height":h,
                                       "source_group":"m_buildPartTypes",
                                       "order":names}
    # Operator rotate is a separate original button, in its source
    # m_buildBaseBtns branch. It is not part of the static on-foot build
    # profile, so render the exact original subtree at its own source bounds.
    rot=NativeMobileHud(ASSETS,heading_deg=0,map_uv=[.5,.5])
    rot.node(hud.go("btn_opRotate"),depth=3)
    br=geometry["build_controls"]["btn_opRotate"]["bounds"]
    original_rotate=rot.layer.crop(tuple(round(br[k]) for k in
                                          ("left","top","right","bottom")))
    if original_rotate.getbbox() is None:
        raise ValueError("Original Unity btn_opRotate icon missing")
    original_rotate.save(dest/"hud_original_build_rotate.png",optimize=True)

    print("LIOS_UNITY_ORIGINAL_BUILD_HUD",outfile.stat().st_size,
          "source_tabs",names,"atlas",atlas.size,flush=True)

    # Compute original RawImage compass displacement using the same exact
    # PathID m_compassImage, screen crop, and mask_leftrightfade logic as the
    # PC version's render_source_unity_hud.py.
    metadata=json.loads((ASSETS/"compass_original_source.json").read_text())
    assert int(metadata["rawimage_component"]["m_Texture"]["m_PathID"])==7430988158235635878
    compass_go=hud.go("m_compassImage")
    assert compass_go["path_id"]==metadata["gameobject_path_id"]
    rect=hud.bygo[compass_go["path_id"]]
    parent=hud.t[rect["father"]]
    compass_bounds=source_rect(hud,parent["go"])
    _,_,cw_raw,ch_raw=hud.rect(parent["path_id"])
    cw,ch=max(1,round(cw_raw)),max(1,round(ch_raw))
    _,_,fw_raw,fh_raw=hud.rect(rect["path_id"])
    fw,fh=max(cw,round(abs(fw_raw))),max(ch,round(abs(fh_raw)))
    with Image.open(ASSETS/"compass_original_source.png") as f:
        source_compass=f.convert("RGBA").resize((fw,fh),Image.Resampling.LANCZOS)
    fade_options=hud.sprite_names.get("mask_leftrightfade",[])
    assert fade_options,"Original compass fade sprite missing"
    fade=next((v for v in fade_options if v["bundle"]=="common.ab"),fade_options[0])
    with Image.open(ASSETS/fade["file"]) as im:
        alpha_mask=im.convert("RGBA").getchannel("A").resize((cw,ch),Image.Resampling.BILINEAR)
    # Keep Unity sibling decorations (Image (2)), composited *after* the
    # moving strip in every frame so the graphics do not disappear.
    decoration_renderer=HudV7(ASSETS,heading_deg=0,map_uv=[.5,.5])
    root=decoration_renderer.go("m_kedu")
    for childtid in decoration_renderer.bygo[root["path_id"]]["children"]:
        rt=decoration_renderer.t.get(childtid)
        child=decoration_renderer.g.get(rt["go"]) if rt else None
        if child and child["name"]=="Image (2)":
            decoration_renderer.node(child,depth=0)
    area=(
        round(compass_bounds["left"]),
        round(compass_bounds["top"]),
        round(compass_bounds["left"])+cw,
        round(compass_bounds["top"])+ch,
    )
    decoration=decoration_renderer.layer.crop(area)
    atlas=Image.new("RGBA",(4*cw,18*ch),(0,0,0,0))
    for frame in range(72):
        heading=frame*5
        shifted=ImageChops.offset(source_compass,-round(heading/360*fw),0)
        crop_left=(fw-cw)//2
        crop_top=max(0,(fh-ch)//2)
        tile=shifted.crop((crop_left,crop_top,crop_left+cw,crop_top+ch))
        tile.putalpha(ImageChops.multiply(tile.getchannel("A"),alpha_mask))
        tile.alpha_composite(decoration)
        atlas.alpha_composite(tile,((frame%4)*cw,(frame//4)*ch))
    assert atlas.getbbox() is not None
    atlas.save(dest/"hud_original_compass_strip.png",optimize=True)
    geometry["compass"]={"bounds":compass_bounds,"tile_width":cw,"tile_height":ch,
                         "frames":72,"degrees_per_frame":5,
                         "source_texture_path_id":7430988158235635878,
                         "source_fade_sprite_path_id":fade["path_id"]}
    print("LIOS_HUD_ORIGINAL_RUNTIME_COMPASS_ATLAS",atlas.size,
          "tile",(cw,ch),"frames",72,flush=True)

    geometry["joystick"]=original_joystick()
    knob=hud.go("m_littleYaogan")
    # This GameObject may contain a nested sprite instead of an Image directly.
    # Rasterize just its ORIGINAL subtree using the same source renderer,
    # preserving border/preserve-aspect/color/sibling order when cropping.
    knob_layer=HudV7(ASSETS,heading_deg=0,map_uv=[.5,.5])
    knob_layer.node("m_littleYaogan",depth=5)
    geometry["joystick"]["thumb_rect"]=source_rect(hud,knob["path_id"])
    rc=geometry["joystick"]["thumb_rect"]
    crop=(round(rc["left"]),round(rc["top"]),round(rc["right"]),round(rc["bottom"]))
    thumb=knob_layer.layer.crop(crop)
    if not thumb.getbbox():
        raise ValueError("Source Unity joystick thumb subtree has no visible graphics; "+
                         repr({"gameobject":knob["path_id"],
                               "components":hud.game_objects[knob["path_id"]]["components"],
                               "children":hud.bygo[knob["path_id"]]["children"]}))
    thumb.save(dest/"hud_original_joystick_knob.png",optimize=True)
    geometry["joystick"]["thumb_sprite_source"]="source Unity m_littleYaogan subtree with children"

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
    for name,entry in geometry["build_controls"].items():
        lines.append("static constexpr Rect Build_"+name+"="+
                     literal_rect(entry["bounds"])+";")
    lines.append("static constexpr int BuildCategoryCount=%d;"%geometry["build_category_atlas"]["count"])
    lines.append("static constexpr int BuildCategoryTileW=%d;"%geometry["build_category_atlas"]["tile_width"])
    lines.append("static constexpr int BuildCategoryTileH=%d;"%geometry["build_category_atlas"]["tile_height"])
    for i,item in enumerate(icons):
        lines.append("static constexpr Rect SlotBox"+str(i)+"="+literal_rect(item["slot"])+";")
        lines.append("static constexpr Rect Slot"+str(i)+"="+literal_rect(item["icon"])+";")
        lines.append("static constexpr Rect SlotHighlight"+str(i)+"="+
                     literal_rect(item["selected_bounds"])+";")
        info=item["count"];col=info["color"]
        lines.append("static constexpr Text SlotCount"+str(i)+"={"+literal_rect(info["bounds"])+
                     ",%d,%d,%d,%d,%d,%d};"%(info["font_size"],info["align"],*col))
    for key,info in geometry["texts"].items():
        col=info["color"]
        lines.append("static constexpr Text "+key+"={"+literal_rect(info["bounds"])+
                     ",%d,%d,%d,%d,%d,%d};"%(info["font_size"],info["align"],*col))
    lines.append("static constexpr int HotbarHighlightTileW=%d;"%
                 selection_width)
    lines.append("static constexpr int HotbarHighlightTileH=%d;"%
                 selection_height)
    compass=geometry["compass"]
    lines.append("static constexpr Rect Compass="+literal_rect(compass["bounds"])+";")
    lines.append("static constexpr int CompassTileW=%d;"%compass["tile_width"])
    lines.append("static constexpr int CompassTileH=%d;"%compass["tile_height"])
    lines.append("static constexpr int CompassFrames=72;")
    j=geometry["joystick"];cx,cy=j["center"]
    lines.append("static constexpr Rect JoystickTrigger="+literal_rect(j["trigger"])+";")
    lines.append("static constexpr Rect JoystickThumb="+literal_rect(j["thumb_rect"])+";")
    lines.append("static constexpr float JoystickCenterX="+fmt(cx)+";")
    lines.append("static constexpr float JoystickCenterY="+fmt(cy)+";")
    lines.append("static constexpr float JoystickRadius="+fmt(j["radius"])+";")
    lines.append("} // namespace LIOS_HUD")
    h=dest/"mobile_original_hud_layout.hpp";h.write_text("\n".join(lines)+"\n",encoding="utf8")
    # Generated provenance guards:
    assert len(icons)==7 and len(geometry["buttons"])==11
    for slot in icons:
        b=slot["slot"]
        assert b["right"]-b["left"]>=50 and b["bottom"]-b["top"]>=50,slot
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
