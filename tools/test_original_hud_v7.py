#!/usr/bin/env python3
"""V7 regression: original source UI text rects and runtime inventory values."""
import importlib.util
import json
from pathlib import Path
from PIL import ImageChops

spec=importlib.util.spec_from_file_location("v7",Path("tools/render_original_hud_v7.py"))
v7=importlib.util.module_from_spec(spec)
spec.loader.exec_module(v7)

blank=v7.HudV7()
blank_stats=blank.render_complete(state={})
assert blank_stats["selected_hotbar_slot"]==0
assert blank_stats["runtime_inventory_values_supplied"] is False
assert not blank_stats["texts"]
assert not blank_stats["original_item_icon_rectangles"]
assert blank_stats["all_source_sprite_links_resolved"]

items=[{"sprite":"tb_cl_mt","count":1},
       {"sprite":"tb_cl_st","count":15},
       None,{"sprite":"tb_cl_tiek","count":3},
       None,{"sprite":"wupin_shuihu_1","count":2},None]
vals={"health":.82,"food":.41,"water":.94}
state={"profile":"on-foot-melee","selected_slot":3,
       "inventory":items,"vitals":vals,"heading_deg":65,
       "map_uv":[.65,.30]}
a=v7.HudV7(heading_deg=65,map_uv=(.65,.30))
result=a.render_complete(state=state)
assert result["selected_hotbar_slot"]==3
assert result["runtime_inventory_values_supplied"]
assert result["preview_inventory_is_demonstrative"] is False
assert len(result["original_item_icon_rectangles"])==4
texts=result["texts"]
assert len(texts)==6,(len(texts),texts)
assert {t["value"] for t in texts}=={"15","3","2","82","41","94"}
assert all(t["source_gameobject_path_id"] in a.game_objects for t in texts)
assert all(t["source_rect_path_id"] in a.t for t in texts)
assert all(t["source_component_path_id"] in a.source_comps for t in texts)
assert all(a.source_comps[t["source_component_path_id"]]["type"]=="Text" for t in texts)
assert all(t["font_origin"]=="fallback, original m_Font PathID=0" for t in texts)
assert all((a.assets/r["original_sprite_file"]).is_file() for r in result["original_item_icon_rectangles"])
assert all(r["source_icon_rect"] in a.t for r in result["original_item_icon_rectangles"])
assert len(result["source_hierarchy_order"])==22
assert ImageChops.difference(blank.layer,a.layer).getbbox()
assert result["source_image_components_drawn_unique"]>=50
assert len(a.calls)>=55

# Runtime values change the image without changing any source rect, texture or font.
b=v7.HudV7(heading_deg=65,map_uv=(.65,.30))
newstate={**state,"vitals":{"health":.25,"food":.92,"water":.63}}
r2=b.render_complete(state=newstate)
assert len(r2["texts"])==len(texts)
assert [(p["source_rect_path_id"]) for p in r2["texts"]]==[p["source_rect_path_id"] for p in texts]
assert ImageChops.difference(a.layer,b.layer).getbbox()

# No invented item sprite is allowed.
try:
    v7.HudV7().render_complete(state={"inventory":[{"sprite":"fake_not_in_xapk","count":3}]+[None]*6})
except KeyError:
    pass
else:
    raise AssertionError("A fake item sprite must be rejected")

try:
    v7.HudV7().render_complete(state={"selected_slot":9})
except AssertionError:
    pass
else:
    raise AssertionError("Selected slot outside original 7-slot inventory should fail")

rifle=v7.HudV7()
rifle_stats=rifle.render_complete(state={"profile":"on-foot-rifle"})
assert len(rifle_stats["source_hierarchy_order"])==24
assert "m_kaijing" not in {t["object"] for t in rifle_stats["texts"]}
print("HUD_V7_SOURCE_TESTS_PASS",json.dumps({
    "source_slots":7,
    "dynamic_text_labels":len(texts),
    "state_item_icons":len(result["original_item_icon_rectangles"]),
    "original_text_rects":True,
    "original_unity_font_substitution_explicit":True,
    "melee_draws":len(a.calls),
    "rifle_draws":len(rifle.calls),
},ensure_ascii=False))
