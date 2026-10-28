#!/usr/bin/env python3
"""Export a complete, auditable Unity UGUI HUD component graph from original XAPK prefab.

NOT a screenshot reconstruction: this contains all original serialized objects,
RectTransforms, MonoBehaviour Image/RawImage/Text components and their exact
PPtr fileID/pathID. No guessed textures, names, or coordinates.
"""
from pathlib import Path
from collections import Counter,defaultdict
import hashlib,json
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
ASSETS=ROOT/"last_island_recreated_game/assets/hud_original_xapk"
SRC=ASSETS/"ui_prefabs_original/assets/ui/battlepanel.ab"
OUT=ASSETS/"complete_source_mapping"
OUT.mkdir(parents=True,exist_ok=True)
assert SRC.is_file()
env=UnityPy.load(str(SRC))
# Unity external fileIDs are not arbitrary: file_id=N indexes source AB's
# m_Externals[N-1]. Retain their names/CAB GUIDs to resolve missing sprites.
debug_files=[]
external_records=[]
for k,v in getattr(env,"files",{}).items():
    attrs=[a for a in dir(v) if "extern" in a.lower()]
    debug_files.append({"key":str(k),"external_attributes":attrs,
      "type":type(v).__name__})
    for attr in attrs:
        try:
            value=getattr(v,attr)
            if isinstance(value,list):
                for i,e in enumerate(value):
                    external_records.append({"file_id":i+1,"field":attr,
                      "path_name":str(getattr(e,"path_name",getattr(e,"m_PathName",""))),
                      "asset_name":str(getattr(e,"asset_name",getattr(e,"m_AssetName",""))),
                      "guid":str(getattr(e,"guid",getattr(e,"m_GUID",""))),
                      "repr":str(e)[:220]})
        except Exception as ex:pass
print("ORIGINAL_UI_BUNDLE_EXTERNALS",json.dumps(external_records,ensure_ascii=False)[:10000],flush=True)
print("ORIGINAL_UI_DEBUG_FILES",json.dumps(debug_files,ensure_ascii=False)[:1500],flush=True)
original_manifest=json.loads((ASSETS/"manifest_original.json").read_text(encoding="utf8"))
atlas={str(x["path_id"]):x for x in original_manifest["sprites"]}
objs={str(o.path_id):o for o in env.objects}
def ref(v):
    if not isinstance(v,dict):return {"file_id":0,"path_id":"0"}
    return {"file_id":int(v.get("m_FileID",0)),"path_id":str(v.get("m_PathID",0))}
def clipcolor(v):
    return {k:float(v[k]) for k in ("r","g","b","a")} if isinstance(v,dict) and all(k in v for k in ("r","g","b","a")) else None
report={
 "original_bundle":"install_time_asset_pack.apk/assets/ui/battlepanel.ab",
 "external_dependencies":external_records,
 "source_bundle_sha256":hashlib.sha256(SRC.read_bytes()).hexdigest(),
 "atlases_from_original_XAPK":sorted(set(a["bundle"] for a in original_manifest["sprites"])),
 "objects":{},"rects":{},"components":{},"sprite_assets":{},"canvases":[],
 "unresolved_sprite_references":[],"per_component_type":{},
}
counts=Counter()
for ob in env.objects:
    typ=ob.type.name;pid=str(ob.path_id)
    if typ not in ("GameObject","RectTransform","Canvas","MonoBehaviour","Sprite"):
        continue
    try:d=ob.read_typetree()
    except Exception as ex:
        counts["typetree_failed_"+typ]+=1;continue
    if typ=="GameObject":
        report["objects"][pid]={
          "name":str(d.get("m_Name","")),"active_self":bool(d.get("m_IsActive",True)),
          "layer":d.get("m_Layer",0),
          "components":[ref(x.get("component")) for x in d.get("m_Component",[])],
          "source_path_id":pid}
    elif typ=="RectTransform":
        report["rects"][pid]={
          "go":ref(d.get("m_GameObject")),"parent":ref(d.get("m_Father")),
          "children":[ref(x) for x in d.get("m_Children",[])],
          "anchor_min":d.get("m_AnchorMin"),"anchor_max":d.get("m_AnchorMax"),
          "pivot":d.get("m_Pivot"),"size_delta":d.get("m_SizeDelta"),
          "anchored_position":d.get("m_AnchoredPosition"),
          "local_rotation":d.get("m_LocalRotation"),"local_scale":d.get("m_LocalScale"),
          "local_position":d.get("m_LocalPosition")}
    elif typ=="Canvas":
        report["canvases"].append({
            "path_id":pid,"gameobject":ref(d.get("m_GameObject")),
            "render_mode":d.get("m_RenderMode"),
            "sorting_order":d.get("m_SortingOrder"),
            "override_sorting":d.get("m_OverrideSorting")})
    elif typ=="MonoBehaviour":
        kind=("Image" if "m_Sprite" in d and "m_Type" in d
              else "RawImage" if "m_Texture" in d and "m_UVRect" in d
              else "Text" if "m_Text" in d
              else "Mask" if "m_ShowMaskGraphic" in d
              else "RectMask2D" if "m_Padding" in d and "m_Softness" in d
              else "Slider" if "m_FillRect" in d and "m_HandleRect" in d
              else "CanvasScaler" if "m_ReferenceResolution" in d
              else "Other")
        counts[kind]+=1
        component={"type":kind,
                   "go":ref(d.get("m_GameObject")),
                   "enabled":bool(d.get("m_Enabled",True)),
                   "script":ref(d.get("m_Script")),
                   "color":clipcolor(d.get("m_Color")),
                   "source_path_id":pid}
        if kind=="Image":
            component.update({
                "sprite":ref(d.get("m_Sprite")),
                "image_type":d.get("m_Type"),
                "fill_amount":d.get("m_FillAmount"),
                "fill_method":d.get("m_FillMethod"),
                "fill_origin":d.get("m_FillOrigin"),
                "fill_clockwise":d.get("m_FillClockwise"),
                "preserve_aspect":d.get("m_PreserveAspect"),
                "fill_center":d.get("m_FillCenter"),
                "maskable":d.get("m_Maskable"),
                "material":ref(d.get("m_Material"))
                })
        elif kind=="RawImage":
            component.update({"texture":ref(d.get("m_Texture")),
                              "uv_rect":d.get("m_UVRect"),"material":ref(d.get("m_Material"))})
        elif kind=="Text":
            component.update({"text":d.get("m_Text"),"font":ref(d.get("m_Font")),
                "font_size":d.get("m_FontSize"),"font_style":d.get("m_FontStyle"),
                "alignment":d.get("m_Alignment"),
                "support_rich_text":d.get("m_SupportRichText"),
                "horizontal_overflow":d.get("m_HorizontalOverflow"),
                "vertical_overflow":d.get("m_VerticalOverflow")})
        elif kind=="Mask":component["show_graphic"]=d.get("m_ShowMaskGraphic")
        elif kind=="RectMask2D":component["padding"]=d.get("m_Padding")
        elif kind=="CanvasScaler":
            component.update({"resolution":d.get("m_ReferenceResolution"),
              "scale_mode":d.get("m_UiScaleMode"),"screen_match_mode":d.get("m_ScreenMatchMode"),
              "match":d.get("m_MatchWidthOrHeight")})
        elif kind=="Slider":
            component.update({"fill_rect":ref(d.get("m_FillRect")),
              "handle_rect":ref(d.get("m_HandleRect")),"min_value":d.get("m_MinValue"),
              "max_value":d.get("m_MaxValue"),"value":d.get("m_Value")})
        report["components"][pid]=component
    elif typ=="Sprite":
        # Source prefab embeds extra Sprite objects omitted by earlier atlas-only export.
        sprite=ob.read()
        entry={"name":str(sprite.m_Name),"source_bundle":"assets/ui/battlepanel.ab",
               "embedded":True,"path_id":pid}
        try:
            img=sprite.image.convert("RGBA")
            path=OUT/"embedded_sprites"/("source_sprite_"+pid+".png")
            path.parent.mkdir(exist_ok=True,parents=True)
            img.save(path,optimize=True)
            entry.update({"file":str(path.relative_to(ASSETS)),
               "width":img.width,"height":img.height,
               "sha256":hashlib.sha256(path.read_bytes()).hexdigest()})
        except (FileNotFoundError,ValueError,AttributeError) as e:
            # Unity Sprite references can require a missing external CAB/atlas.
            # Preserve exact original Sprite PPtr rather than fabricate pixels.
            entry["unresolved_texture_dependency"]=str(e)[:350]
            counts["embedded_external_missing"]+=1
        report["sprite_assets"][pid]=entry

sprite_ids=set(atlas)|set(report["sprite_assets"])
for pid,c in report["components"].items():
    if c["type"]!="Image":continue
    pointer=c["sprite"]
    if pointer["path_id"]=="0":continue
    if pointer["file_id"]==0 and pointer["path_id"] in report["sprite_assets"]:
        record=report["sprite_assets"][pointer["path_id"]]
        if record.get("file"):
            c["resolved_sprite_file"]=record["file"]
            counts["resolved_embedded"]+=1
        else:
            c["original_embedded_sprite_dependency_missing"]=True
            counts["embedded_ref_external_dependency"]+=1
    elif pointer["path_id"] in atlas:
        c["resolved_sprite_file"]=atlas[pointer["path_id"]]["file"]
        counts["resolved_atlas"]+=1
    else:
        report["unresolved_sprite_references"].append({
            "component_id":pid,"gameobject_id":c["go"]["path_id"],
            "pointer":pointer})
        counts["unresolved"]+=1

assert len(report["objects"])>=1800 and len(report["rects"])>=1700
assert len(report["components"])>=1700
assert len(report["sprite_assets"])>=5
assert counts["resolved_atlas"]>=700
assert len(report["unresolved_sprite_references"])<150
report["per_component_type"]=dict(counts)
report["summary"]={
 "gameobjects":len(report["objects"]),
 "recttransforms":len(report["rects"]),
 "components":len(report["components"]),
 "ui_canvases":len(report["canvases"]),
 "source_embedded_sprites":len(report["sprite_assets"]),
 "exact_atlas_sprite_links":counts["resolved_atlas"],
 "exact_embedded_sprite_links":counts["resolved_embedded"],
 "unresolved_external_sprite_links":counts["unresolved"],
 "original_XAPK_sprites_available":len(original_manifest["sprites"])}
(OUT/"original_unity_all_hud_components.json").write_text(
    json.dumps(report,ensure_ascii=False,separators=(",",":")),encoding="utf8")
print("SOURCE_HUD_COMPONENT_GRAPH",json.dumps(report["summary"],ensure_ascii=False),flush=True)
print("SOURCE_HUD_UNRESOLVED_FIRST",json.dumps(report["unresolved_sprite_references"][:8]),flush=True)
