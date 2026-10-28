#!/usr/bin/env python3
"""Reproducible sanity tests against original Unity Sprite PathIDs and UGUI data."""
import argparse, importlib.util, json
from pathlib import Path
from PIL import ImageChops

p=argparse.ArgumentParser()
p.add_argument("--assets",type=Path,default=Path("last_island_recreated_game/assets/hud_original_xapk"))
args=p.parse_args()
source=Path("tools/render_source_unity_hud.py")
spec=importlib.util.spec_from_file_location("hud_reconstructed_source",source)
m=importlib.util.module_from_spec(spec)
spec.loader.exec_module(m)

melee=m.SourceHUD(args.assets)
centers=melee.render(samples=False,profile="on-foot-melee")
assert len(centers)==7 and centers==sorted(centers,key=lambda x:x[0])
assert 94.9<centers[1][0]-centers[0][0]<95.1
assert len(melee.calls)>=55
assert len(melee._source_hits)>=30
assert not any(".sample" in c["go"] for c in melee.calls)
assert not any(c["go"] in ("m_PatiziImage","m_openGliderIcon","m_addOilIcon") for c in melee.calls)
assert not any(c["go"] in ("m_changeBulletIcon","btn_kaijing") for c in melee.calls)
assert any(c["go"]=="m_JumpImage" for c in melee.calls)
assert any(c["go"]=="m_fireImage" for c in melee.calls)
assert any(c["go"]=="ditu(Map_map original)" for c in melee.calls)
mapbox=next(c for c in melee.calls if c["go"]=="ditu(Map_map original)")
assert melee.layer.getpixel((mapbox["x"]+2,mapbox["y"]+2))[3]<200
assert melee.layer.getbbox()

rifle=m.SourceHUD(args.assets)
rifle.render(profile="on-foot-rifle")
assert len(rifle.calls)>len(melee.calls)
assert any(c["go"]=="m_changeBulletIcon" for c in rifle.calls)

try:
    m.SourceHUD(args.assets).render(items=["fabricated_item"]+[None]*6)
except KeyError:pass
else:raise AssertionError("Fabricated item Sprite should be rejected")

another=m.SourceHUD(args.assets,map_uv=(.2,.8))
another.render()
assert ImageChops.difference(melee.layer.convert("RGB"),another.layer.convert("RGB")).getbbox()

manifest=json.loads((args.assets/"manifest_original.json").read_text(encoding="utf8"))
original_files={v["file"] for v in manifest["sprites"]}
assert all(c["sprite"] in original_files or c["sprite"]=="source Map_map" for c in melee.calls)
print("HUD_V2_TESTS_PASS: seven source slots, correct melee/rifle states, source sprites only")
print("HUD_V2_TESTS_PASS: source-derived circular minimap, data-driven position, no mixed ladder/glider icons")
print("DRAW_CALLS: melee",len(melee.calls),"rifle",len(rifle.calls))
