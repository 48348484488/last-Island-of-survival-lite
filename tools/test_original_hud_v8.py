#!/usr/bin/env python3
"""Original HUD V8 regression: Unity Image geometry + source build mode."""
import importlib.util,json
from pathlib import Path
from PIL import Image,ImageChops

from unity_ugui_raster import apply_fill,preserve_aspect

# Original Unity filled UI Sprite semantics, no custom invented textures.
src=Image.new("RGBA",(10,10),(50,120,200,255))
left=apply_fill(src,.4,method=0,origin=0)
right=apply_fill(src,.4,method=0,origin=1)
bottom=apply_fill(src,.4,method=1,origin=0)
top=apply_fill(src,.4,method=1,origin=1)
assert [left.getpixel((x,5))[3] for x in range(10)]==[255]*4+[0]*6
assert [right.getpixel((x,5))[3] for x in range(10)]==[0]*6+[255]*4
assert [bottom.getpixel((5,y))[3] for y in range(10)]==[0]*6+[255]*4
assert [top.getpixel((5,y))[3] for y in range(10)]==[255]*4+[0]*6
for method in (2,3,4):
    radial=apply_fill(src,.5,method=method,origin=0,clockwise=True)
    assert 0 < sum(v==255 for v in radial.getchannel("A").getdata()) < 100
assert apply_fill(src,1.0).getbbox()==(0,0,10,10)
assert apply_fill(src,0.0).getbbox() is None
aspect=preserve_aspect(Image.new("RGBA",(8,4),(255,255,255,255)),(20,20))
assert aspect.size==(20,20)
assert aspect.getpixel((10,0))[3]==0
assert aspect.getpixel((10,10))[3]==255
assert aspect.getpixel((10,19))[3]==0

root=Path("last_island_recreated_game/assets/hud_original_xapk")
graph=json.loads((root/"complete_source_mapping/original_unity_all_hud_components.json").read_text(encoding="utf8"))
images=[v for v in graph["components"].values() if v["type"]=="Image"]
assert sum(c.get("image_type")==3 for c in images)==19
assert sum(bool(c.get("preserve_aspect")) for c in images)==23
assert any(c.get("image_type")==3 and c.get("fill_method")==1 and c.get("fill_origin")==1 for c in images)
assert any(c.get("image_type")==3 and c.get("fill_method")==4 for c in images)

spec=importlib.util.spec_from_file_location("v7",Path("tools/render_original_hud_v7.py"))
v7=importlib.util.module_from_spec(spec)
spec.loader.exec_module(v7)
onfoot=v7.HudV7()
st=onfoot.render_complete(state={"profile":"on-foot-melee"})
assert "btn_fire" in st["source_hierarchy_order"]
assert "btn_build" not in st["source_hierarchy_order"]
building=v7.HudV7()
bst=building.render_complete(state={"profile":"on-foot-build",
 "vitals":{"health":.80,"food":.58,"water":.92},
 "inventory":[{"sprite":"tb_cl_mt","count":42}]+[None]*6})
assert "btn_build" in bst["source_hierarchy_order"]
assert "btn_fire" not in bst["source_hierarchy_order"]
assert "m_buildPartTypes" in bst["source_hierarchy_order"]
assert "btn_freeBuildModel" in bst["source_hierarchy_order"]
assert any(c["go"]=="btn_build" for c in building.calls)
assert not any(c["go"]=="btn_fire" for c in building.calls)
assert all(c.get("original_component_path_id") in graph["components"]
    for c in building.calls if c.get("original_component_path_id"))
assert bst["build_mode_is_source_derived_but_runtime_activation_unverified"]
assert not st["build_mode_is_source_derived_but_runtime_activation_unverified"]
assert ImageChops.difference(building.layer,onfoot.layer).getbbox()
assert bst["original_text_components_drawn_with_fallback"]>=4
assert len(bst["original_item_icon_rectangles"])==1
assert bst["source_image_components_drawn_unique"]>35
print("HUD_V8_SOURCE_IMAGE_AND_BUILD_TESTS_PASS",json.dumps({
 "serialized_filled_sprite_components":19,
 "serialized_preserve_aspect_components":23,
 "filling_directions":["horizontal left/right","vertical bottom/top","radial approximation"],
 "build_mode_source_sibling_order":bst["source_hierarchy_order"],
 "build_draws":len(building.calls),
 "melee_draws":len(onfoot.calls),
 "source_gameobjects_exact":True
},ensure_ascii=False))
