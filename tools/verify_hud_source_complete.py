#!/usr/bin/env python3
"""Prove all source Sprite pointers in original Last Island 7.3 HUD resolve.

Confirms original FileID + PathID, known Sprite PNG files, 1,716
RectTransform coordinates, and only original source assets. Never asserts
that runtime Canvas animation, CanvasScaler, native shaders, or states match.
"""
import json,hashlib
from pathlib import Path
R=Path(__file__).resolve().parents[1]
A=R/"last_island_recreated_game/assets/hud_original_xapk"
G=json.loads((A/"complete_source_mapping/original_unity_all_hud_components.json").read_text(encoding="utf8"))
M=json.loads((A/"manifest_original.json").read_text(encoding="utf8"))
X=json.loads((A/"complete_source_mapping/external_hud_sprites_index.json").read_text(encoding="utf8"))
P=json.loads((A/"complete_source_mapping/hud_original_all_positions.json").read_text(encoding="utf8"))
counts={"image_components_total":0,"nonzero_original_source_sprite_refs":0,
        "original_source_sprite_links_resolved":0,"no_sprite_expected":0,
        "source_missing_files":[],"other_source_components":len(G["components"])}
for comp in G["components"].values():
    if comp["type"]!="Image":continue
    counts["image_components_total"]+=1
    ptr=comp["sprite"]
    if ptr["path_id"]=="0":
        counts["no_sprite_expected"]+=1
        continue
    counts["nonzero_original_source_sprite_refs"]+=1
    relative=comp.get("resolved_sprite_file")
    if relative and (A/relative).is_file():
        counts["original_source_sprite_links_resolved"]+=1
    else:
        counts["source_missing_files"].append({"file_id":ptr["file_id"],
              "path_id":ptr["path_id"],"source":relative,
              "go_path_id":comp["go"]["path_id"]})
assert counts["image_components_total"]>900,counts
assert counts["nonzero_original_source_sprite_refs"]>=840,counts
assert not counts["source_missing_files"],counts["source_missing_files"][:10]
assert counts["nonzero_original_source_sprite_refs"]==counts["original_source_sprite_links_resolved"]
assert len(X["sprites"])==52, len(X["sprites"])
assert len(G["sprite_assets"])==7
assert all((A/x["file"]).is_file() for x in X["sprites"])
assert all((A/v["file"]).is_file() for v in G["sprite_assets"].values())
assert P["stats"]["mapped_2d_rects"]==1716
assert P["stats"]["exact_named_button_hitboxes"]>=100

from importlib.util import spec_from_file_location,module_from_spec
spec=spec_from_file_location("renderer",R/"tools/render_source_unity_hud.py")
m=module_from_spec(spec);spec.loader.exec_module(m)
hud=m.SourceHUD(A)
assert hud.external_original_sprites==52
assert hud.embedded_original_sprites==7
slots=hud.render(profile="on-foot-melee")
assert len(slots)==7
assert len(hud.calls)>=60
assert all((A/v["sprite"]).is_file() or v["sprite"] in ("source Map_map","compass_original_source.png") for v in hud.calls)
result={"source_XAPK":"Last Island of Survival LITE 7.3 original",
       "status":"All nonempty Unity Image.m_Sprite pointers are matched to original PNGs",
       "components":counts,"mapped_recttransforms":P["stats"]["mapped_2d_rects"],
       "touch_hitboxes":P["stats"]["exact_named_button_hitboxes"],
       "original_external_assetbundles":X["summary"]["matching_original_asset_bundles"],
       "external_source_sprite_files":len(X["sprites"]),
       "prefab_local_sprites":len(G["sprite_assets"]),
       "on_foot_preview_draw_calls":len(hud.calls),
       "limitations":["Serialized prefab has no runtime player values and all GameObjects have active_self=True.",
           "Dynamic visibility, Unity animations, native shaders, masking, full CanvasScaler and device safe area are not validated 1:1.",
           "Current game still uses its own on-screen HUD renderer; this source-based renderer is an independently generated overlay."]}
(A/"complete_source_mapping/validated_source_coverage.json").write_text(json.dumps(result,indent=2,ensure_ascii=False),encoding="utf8")
print("ORIGINAL_HUD_SOURCE_COVERAGE_COMPLETE",json.dumps(result,ensure_ascii=False)[:2500],flush=True)
