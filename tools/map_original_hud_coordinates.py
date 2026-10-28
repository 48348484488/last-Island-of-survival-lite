#!/usr/bin/env python3
"""Complete Unity HUD geometry and click/hitbox map using ORIGINAL serialized data.

Inputs: source XAPK battlepanel.ab RectTransform hierarchy and Image PPtrs
already extracted. Does not manually place a single HUD button and does not
use a reference screenshot for textures.
"""
from pathlib import Path
from collections import Counter
import argparse,importlib.util,json,math

ROOT=Path(__file__).resolve().parents[1]
GAME=ROOT/"last_island_recreated_game"
SRC=GAME/"assets/hud_original_xapk"

def load_hud(assets,size):
    spec=importlib.util.spec_from_file_location("render_source_unity_hud",
        ROOT/"tools/render_source_unity_hud.py")
    mod=importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod.SourceHUD(assets,canvas=size)

def export_map(assets=SRC,size=(1920,1080)):
    assets=Path(assets)
    raw=json.loads((assets/"complete_source_mapping/original_unity_all_hud_components.json").read_text(encoding="utf8"))
    hud=load_hud(assets,size)
    graph={"source_prefab":raw["original_bundle"],
           "source_sha256":raw["source_bundle_sha256"],
           "virtual_resolution":list(size),
           "caution":"Native Unity CanvasScaler/safe-area, animations and dynamic active states not yet proven.",
           "nodes":[],"clickable":[],"unresolved_source_images":[]}
    comp=raw["components"]
    metas=raw["objects"]
    rects=raw["rects"]
    for tid,rt in rects.items():
        gid=rt["go"]["path_id"]
        go=metas.get(gid)
        if not go:continue
        x,y,w,h=hud.rect(tid)
        if not all(math.isfinite(v) for v in (x,y,w,h)):
            raise ValueError("Nonfinite Unity RectTransform "+tid)
        parent_id=rt["parent"]["path_id"]
        parent=rects.get(parent_id,{})
        pid=parent.get("go",{}).get("path_id")
        parent_name=metas.get(pid,{}).get("name")
        components=[]
        for pointer in go["components"]:
            c=comp.get(pointer["path_id"])
            if not c:continue
            item={"path_id":pointer["path_id"],"type":c["type"],"enabled":c["enabled"]}
            if c["type"]=="Image":
                item.update({"sprite_ptr":c["sprite"],
                    "sprite_source":c.get("resolved_sprite_file"),
                    "image_type":c.get("image_type"),"color":c.get("color")})
                if c["sprite"]["path_id"]!="0" and not item["sprite_source"]:
                    graph["unresolved_source_images"].append({
                        "go":go["name"],"go_path_id":gid,
                        "component_path_id":pointer["path_id"],
                        "sprite_ptr":c["sprite"]})
            elif c["type"]=="RawImage":
                item.update({"texture_ptr":c["texture"],"uv_rect":c.get("uv_rect")})
            elif c["type"]=="Text":
                item.update({"serialized_text":c.get("text"),"font":c.get("font")})
            components.append(item)
        width,height=abs(w),abs(h)
        node={"name":go["name"],"go_path_id":gid,"rect_path_id":tid,
              "parent_name":parent_name,"parent_rect_path_id":parent_id,
              "children_rect_ids":[p["path_id"] for p in rt["children"]],
              "active_serialized":go["active_self"],
              "local_rect":{k:rt.get(k) for k in
                  ("anchored_position","anchor_min","anchor_max","pivot",
                   "size_delta","local_scale","local_rotation")},
              "canvas_rect":{ "center_x":round(x,5),"center_y":round(y,5),
                  "width":round(w,5),"height":round(h,5)},
              "pixel_bounds_top_left":{
                  "left":round(x-width/2,5),
                  "top":round(size[1]-y-height/2,5),
                  "right":round(x+width/2,5),
                  "bottom":round(size[1]-y+height/2,5)},
              "components":components}
        graph["nodes"].append(node)
        # Exact hitboxes for every named source Button: these can be connected
        # to mouse and touchscreen input without re-deriving any positions.
        if go["name"].startswith("btn_") and width>0 and height>0:
            graph["clickable"].append({"name":go["name"],"go_path_id":gid,
                 "rect_path_id":tid,"bounds":node["pixel_bounds_top_left"],
                 "active_serialized":go["active_self"]})
    assert len(graph["nodes"])>=1700
    assert len(graph["clickable"])>=30
    assert len(graph["unresolved_source_images"])<150
    slotnames=[n for n in graph["nodes"] if n["name"] in
               {"m_Item1","m_Item2","m_Item3","m_Item4","m_Item5","m_Item6","m_Item7"}]
    assert len(slotnames)==7
    slots=sorted(slotnames,key=lambda v:int(v["name"][-1]))
    assert all(slots[i]["canvas_rect"]["center_x"]<
               slots[i+1]["canvas_rect"]["center_x"] for i in range(6))
    names={n["name"] for n in graph["nodes"]}
    for name in ("m_map","m_compassImage","m_bagItems","btn_fire",
                 "btn_pack","btn_autoRun","btn_up2","btn_crouch2","btn_pa2"):
        assert name in names,name
    graph["stats"]={
        "mapped_2d_rects":len(graph["nodes"]),
        "exact_named_button_hitboxes":len(graph["clickable"]),
        "unresolved_original_sprite_components":len(graph["unresolved_source_images"]),
        "hotbar_slots":7,
        "source_sprite_components":sum(sum(c["type"]=="Image" for c in n["components"]) for n in graph["nodes"]),
        "source_recttransforms_only":True,
        "placed_using_hand_tuned_xy":False}
    return graph

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("--assets",type=Path,default=SRC)
    ap.add_argument("--width",type=int,default=1920)
    ap.add_argument("--height",type=int,default=1080)
    args=ap.parse_args()
    assert 320<=args.width<=4096 and 240<=args.height<=2160
    data=export_map(args.assets,(args.width,args.height))
    dst=args.assets/"complete_source_mapping"/"hud_original_all_positions.json"
    dst.parent.mkdir(parents=True,exist_ok=True)
    dst.write_text(json.dumps(data,indent=2,ensure_ascii=False),encoding="utf8")
    print("ALL_HUD_ORIGINAL_POSITIONS_MAPPED",json.dumps(data["stats"]),flush=True)
    print("SOURCE_PANEL_PATH",dst,flush=True)

if __name__=="__main__":main()
