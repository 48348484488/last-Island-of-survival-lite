#!/usr/bin/env python3
"""Non-synthetic HUD V3 geometry, real source 9-slicing and dynamic Slider tests."""
import importlib.util,json
from pathlib import Path
from PIL import Image,ImageChops
root=Path("last_island_recreated_game/assets/hud_original_xapk")
spec=importlib.util.spec_from_file_location("source_hud",Path("tools/render_source_unity_hud.py"))
mod=importlib.util.module_from_spec(spec);spec.loader.exec_module(mod)
a=mod.SourceHUD(root)
slots=a.render()
assert len(slots)==7
assert all(abs(slots[i+1][0]-slots[i][0]-95.0076)<.01 for i in range(6))
# Old anchor math mistakenly placed hotbar high and automatic running outside viewport.
assert 90<slots[0][1]<120,slots
auto=a.go("btn_autoRun")
tid=a.bygo[auto["path_id"]]["path_id"]
x,y,w,h=a.rect(tid)
assert 0<x<1920 and 0<y<1080 and 0<y-h/2<1080,(x,y,w,h)
assert not any("source_item" in v["go"] for v in a.calls)
expected=a.layer.copy()
b=mod.SourceHUD(root)
b.render(vitals={"health":.84,"food":.56,"water":.72},profile="on-foot-melee")
labels=[x["go"] for x in b.calls]
assert all(n+"_runtime" in labels for n in
 ("m_mybloodtmpFillImage","m_myhungerFillImage","m_myWaterFillImage"))
assert ImageChops.difference(expected,b.layer).getbbox()
c=mod.SourceHUD(root)
c.render(vitals={"health":1.0,"food":.1,"water":.9})
assert ImageChops.difference(c.layer,b.layer).getbbox()
# 9-slice must retain source pixel corner. Use a test fixture, not a HUD illustration.
small=Image.new("RGBA",(12,12),(30,40,50,255))
small.putpixel((0,0),(255,1,1,255))
small.putpixel((11,0),(1,255,1,255))
small.putpixel((0,11),(1,1,255,255))
large=a._sliced_sprite(small,(96,44),{"x":3,"y":3,"z":3,"w":3})
assert large.size==(96,44)
for src,dest in [((0,0),(0,0)),((11,0),(95,0)),((0,11),(0,43))]:
 assert small.getpixel(src)==large.getpixel(dest),(src,dest)
assert len(a.sprite_borders)>100,"Original Unity Sprite border metadata missing"
assert all(v["sprite"] in ("source Map_map","compass_original_source.png") or (root/v["sprite"]).is_file() for v in b.calls)
assert any(v["sprite"]=="compass_original_source.png" for v in b.calls)
assert (root/"compass_original_source.png").is_file()
turn=mod.SourceHUD(root,heading_deg=90)
turn.render()
assert ImageChops.difference(a.layer,turn.layer).getbbox(), "Compass did not respond to camera yaw"
print("ORIGINAL_HUD_V3_REGRESSIONS_PASS",json.dumps({
 "seven_slot_anchor_y":slots[0][1],
 "automatic_run_center":[x,y],
 "sliced_original_sprites":len(a.sprite_borders),
 "runtime_vitals":len([v for v in b.calls if v["go"].endswith("_runtime")]),
 "draw_calls":len(b.calls),
 "source_only":True
 },ensure_ascii=False))
