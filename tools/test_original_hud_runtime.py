#!/usr/bin/env python3
"""Regression for V9: original XAPK HUD appears in the actual 3D viewer.

Headless verification: no pygame window, no GPU, no Android emulator.
The preview deliberately uses synthetic test data and is NOT a game capture.
"""
import json
import math
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "last_island_recreated_game"))
from original_hud_runtime import (
    game_hud_state, viewport_bounds, viewport_to_original, render_source_layer,
)

def verify_clipped_composite_pixel_identity():
    # Reference: old full-canvas composite versus the optimized sprite-local
    # composite. Both must be pixel-identical, including partial offscreen UI.
    from PIL import Image
    dst = Image.new("RGBA", (32, 20), (55, 75, 90, 130))
    sprite = Image.new("RGBA", (12, 8))
    for x in range(12):
        for y in range(8):
            sprite.putpixel((x, y), (x * 19, y * 30, 120, (x * y * 7) % 256))
    for left, top in [(-7, -3), (2, 4), (27, 16), (36, 0), (0, -9)]:
        full = Image.new("RGBA", dst.size)
        full.paste(sprite, (left, top))
        reference = Image.alpha_composite(dst, full)
        clipped = dst.copy()
        x0, y0 = max(0, left), max(0, top)
        x1, y1 = min(32, left + 12), min(20, top + 8)
        if x0 < x1 and y0 < y1:
            clipped.alpha_composite(
                sprite.crop((x0-left, y0-top, x1-left, y1-top)),
                dest=(x0, y0))
        assert clipped.tobytes() == reference.tobytes(), (left, top)


def main():
    verify_clipped_composite_pixel_identity()
    state = game_hud_state(
        health=82, food=66, water=25, yaw=math.radians(25),
        equipped_slot=1, wood=41, metal=12, stone=7, sulfur=3,
    )
    assert state["profile"] == "on-foot-rifle"
    assert state["selected_slot"] == 1
    assert state["heading_deg"] == 25.0
    assert state["vitals"] == {"health": .82, "food": .66, "water": .25}
    icon_proof = (ROOT / "last_island_recreated_game/assets/hud_original_xapk/"
                  "complete_source_mapping/verified_equipped_weapon_icons.json")
    weapon_names = ["cj_sf", "m4"] if icon_proof.is_file() else []
    assert [x["count"] if x else None for x in state["inventory"]] == [
        *([1,1] if weapon_names else [None,None]), 41, 12, 7, 3, None]
    assert [x["sprite"] for x in state["inventory"] if x] == [
        *weapon_names, "tb_cl_mt", "tb_cl_tiek", "tb_cl_st", "ks_lhk"]
    assert viewport_bounds(960, 540) == (0, 0, 960, 540)
    assert viewport_bounds(2340, 1080) == (210, 0, 1920, 1080)
    assert viewport_to_original(480, 270, 960, 540) == (960, 540)
    assert viewport_to_original(80, 540, 2340, 1080) is None

    # Every visual in this PNG is rasterized from original Sprite FileID/PathID
    # and source RectTransforms. The text glyphs use fallback fonts.
    layer, audit = render_source_layer(state)
    assert layer.size == (1920, 1080)
    assert layer.getbbox() is not None
    assert not audit["preview_inventory_is_demonstrative"]
    assert audit["runtime_inventory_values_supplied"]
    assert audit["selected_hotbar_slot"] == 1
    assert "btn_kaijing" in audit["source_hierarchy_order"]
    assert len(audit["original_item_icon_rectangles"]) == 4 + len(weapon_names)
    assert audit["original_text_components_drawn_with_fallback"] >= 7
    assert all(x.get("original_sprite_path_id")
               for x in audit["original_item_icon_rectangles"])

    empty = game_hud_state(
        health=0, food=120, water=-3, yaw=0,
        equipped_slot=0, wood=0, metal=0, stone=0, sulfur=0)
    assert empty["profile"] == "on-foot-melee"
    assert empty["vitals"] == {"health": 0, "food": 1, "water": 0}
    assert empty["inventory"] == (
        ([{"sprite":"cj_sf","count":1},{"sprite":"m4","count":1}]+[None]*5)
        if icon_proof.is_file() else [None]*7)

    output = ROOT / "last_island_recreated_game/assets/hud_original_xapk/" \
        "complete_source_mapping/hud_v9_live_bridge_synthetic_test.png"
    layer.save(output, optimize=True)
    summary = {
        "purpose": "Source HUD runtime bridge; simulated input values, not gameplay screenshot",
        "reference_canvas": list(layer.size),
        "status": "headless source render validated",
        "source_slot_count": len(state["inventory"]),
        "real_source_inventory_icons": len(audit["original_item_icon_rectangles"]),
        "verified_equipped_weapon_icons": weapon_names,
        "rendered_texts_fallback_font": audit["original_text_components_drawn_with_fallback"],
        "profile": state["profile"],
        "source_components_drawn": audit["source_image_components_drawn_unique"],
        "original_glyph_shapes_proven": False,
        "Android_APK_tested": False,
        "OpenGL_gameplay_tested": False,
    }
    output.with_suffix(".json").write_text(
        json.dumps(summary, indent=2), encoding="utf8")
    print("HUD_V9_RUNTIME_BRIDGE_HEADLESS_PASS", json.dumps(summary))

if __name__ == "__main__":
    main()
