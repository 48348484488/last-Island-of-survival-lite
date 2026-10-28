#!/usr/bin/env python3
"""V6 source hierarchy tests: every drawn Image uses exact Unity source PPtr."""
import importlib.util, json
from pathlib import Path
from PIL import ImageChops

spec=importlib.util.spec_from_file_location("hud_v6",Path("tools/render_original_hud_v6.py"))
v6=importlib.util.module_from_spec(spec);spec.loader.exec_module(v6)
a=v6.HudSourceOrdered()
summary=a.render_ordered(profile="on-foot-melee",vitals={"health":.9,"food":.7,"water":.8})
assert summary["all_source_sprite_links_resolved"]
assert len(summary["source_hierarchy_order"])==22
assert summary["source_hierarchy_order"]==sorted(
        summary["source_hierarchy_order"],key=a.z_key)
assert len(a.calls)>=40
graph=a.graph["components"]
for c in a.calls:
    if "original_component_path_id" not in c:continue
    entry=graph[c["original_component_path_id"]]
    assert entry["type"]=="Image"
    assert entry["resolved_sprite_file"]==c["sprite"],(entry,c)
    assert entry["sprite"]["file_id"]==c["original_file_id"]
    assert entry["sprite"]["path_id"]==c["original_path_id"]
assert any(c.get("original_file_id") for c in a.calls if "original_component_path_id" in c)
assert a.external_original_sprites==52
assert a.embedded_original_sprites==7
assert a.layer.getbbox()
b=v6.HudSourceOrdered()
second=b.render_ordered(profile="on-foot-rifle")
assert len(second["source_hierarchy_order"])==24
assert second["source_image_components_drawn_unique"]>0
assert ImageChops.difference(a.layer,b.layer).getbbox()
for x in ("m_map","m_kedu","m_bagItems"):
    assert x in summary["source_hierarchy_order"]
assert summary["on_foot_runtime_selection_not_proven"] is True
print("HUD_V6_TESTS_PASS",json.dumps({
 "melee_operations":len(a.calls),
 "rifle_operations":len(b.calls),
 "source_image_components_drawn_unique":summary["source_image_components_drawn_unique"],
 "exact_FileID_PathID_verified":True,
 "original_hierarchy_sibling_order_verified":True},ensure_ascii=False))
