#!/usr/bin/env python3
"""Require original Oxide loading artwork AND synthetic JNI rectangle in ONE PNG.

The Android immersive "Viewing full screen" onboarding dialog may entirely
obscure the middle of the loading scene. Never count a diagnostic rectangle
on a black pre-render frame as proof of visible game artwork.
Only assesses screenshots of an authorized, unchanged Activity on a virtual
Android ARM64 host. It does not inspect game internals or player data.
"""
import argparse
import json
from pathlib import Path

from PIL import Image

from check_original_overlay_pixels import evaluate, measure, plausible_centered_box


def screen_appearance(path):
    with Image.open(path) as screenshot:
        im = screenshot.convert("RGB")
        w, h = im.size
        # Sample genuine artwork in the left edge, away from centered overlays
        # and fullscreen tutorial. A near-black unrendered Activity is not art.
        left = [
            im.getpixel((x, y))
            for y in range(max(6, h // 10), min(h - 5, h * 9 // 10), max(6, h // 42))
            for x in range(5, max(6, w // 5), max(6, w // 55))
        ]
        colors = len(set(tuple(p // 16 for p in c) for c in left))
        lit_fraction = sum(max(c) > 35 for c in left) / len(left) if left else 0
        # The Redroid native 1280x720 fullscreen tutorial has a large white
        # panel across the center where the rectangle should be.
        bright = [
            im.getpixel((x, y))
            for y in range(max(1, h // 6), min(h - 1, h * 55 // 100), max(4, h // 55))
            for x in range(w * 35 // 100, w * 65 // 100, max(4, w // 65))
        ]
        pale_fraction = sum(min(c) >= 215 and max(c) - min(c) < 35 for c in bright) / len(bright) if bright else 1
        return {
            "size": [w, h],
            "left_artwork_quantized_colors": colors,
            "left_artwork_lit_fraction": round(lit_fraction, 4),
            "center_pale_dialog_fraction": round(pale_fraction, 4),
            "image_contains_nonuniform_artwork": colors >= 24 and lit_fraction >= 0.40,
            "fullscreen_tutorial_no_longer_covers_center": pale_fraction < 0.60,
        }


def bright_stroke(path):
    """Require the vivid synthetic JNI rectangle, not green-ish artwork.

    This strict proof runs after dismissal of Android's fullscreen prompt;
    green pixels dimmed by an active system dialog are not sufficient.
    """
    with Image.open(path) as screenshot:
        im = screenshot.convert("RGB")
        width, height = im.size
        hits = []
        for y in range(height * 35 // 100, height * 72 // 100):
            for x in range(width * 32 // 100, width * 68 // 100):
                r, g, b = im.getpixel((x, y))
                if 15 <= r <= 110 and g >= 200 and b <= 160 and \
                   g > r * 1.8 and g > b * 1.3:
                    hits.append((x, y))
        if not hits:
            return {"pixels": 0, "bounds": None, "plausible": False}
        left, top = min(x for x, _ in hits), min(y for _, y in hits)
        right, bottom = max(x for x, _ in hits), max(y for _, y in hits)
        center_x, center_y = (left + right) / 2, (top + bottom) / 2
        plausible = (
            len(hits) >= 500 and right - left >= 30 and bottom - top >= 30
            and width * .38 <= center_x <= width * .62
            and height * .30 <= center_y <= height * .76
        )
        return {"pixels": len(hits), "bounds": [left, top, right, bottom],
                "plausible": plausible}


def assess(folder):
    folder = Path(folder)
    base = evaluate(folder)
    screenshot = folder / "overlay_on_game_3.png"
    image = measure(screenshot)
    appearance = screen_appearance(screenshot)
    bright = bright_stroke(screenshot)
    game_marker = bright["plausible"]
    foreground = (
        (folder / "original_foreground.txt").read_text().strip()
        == "ORIGINAL_ACTIVITY_RESUMED_DURING_CAPTURE=OBSERVED"
    )
    service = (
        (folder / "overlay_service.txt").read_text().strip()
        == "DIAGNOSTIC_OVERLAY_SERVICE_START=PASS"
    )
    pid_proof_path = folder / "original_overlay_overlap_verdict.json"
    pid_proof = json.loads(pid_proof_path.read_text()) if pid_proof_path.is_file() else {}
    samples = pid_proof.get("capture_samples", [])
    same_capture = any(
        x.get("capture") == screenshot.name and x.get("sample_correlates") is True
        for x in samples
    )
    checks = {
        "original_loading_activity_resumed": foreground,
        "separate_jni_service_running": service,
        "original_activity_pid_and_overlay_matched_capture3": same_capture,
        "green_jni_rectangle_in_actual_game_artwork_png": game_marker,
        "nonblack_game_loading_background_visible": appearance["image_contains_nonuniform_artwork"],
        "android_fullscreen_tutorial_not_obscuring_rectangle": appearance["fullscreen_tutorial_no_longer_covers_center"],
        "marker_removed_when_standalone_service_stopped": base["overlay_disappeared_after_stop"] is True,
    }
    return {
        "status": "ORIGINAL_LOADING_ARTWORK_WITH_SYNTHETIC_OVERLAY_VISIBLE"
        if all(checks.values()) else "LOADING_ARTWORK_AND_OVERLAY_SIMULTANEITY_NOT_PROVEN",
        "checks": checks,
        "green_marker": image,
        "synthetic_bright_stroke": bright,
        "screen_appearance": appearance,
        "gameplay_proven": False,
        "real_player_coordinates_proven": False,
        "game_security_modified": False,
        "limitations": [
            "ActivityManager snapshots precede screenshots and are not atomic.",
            "Original game loading artwork is not the playable world.",
            "The green rectangle is generated by standalone JNI, not by game data.",
            "A fullscreen tutorial or a near-black first render does not satisfy this visual proof.",
        ],
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("folder", type=Path)
    args = parser.parse_args()
    result = assess(args.folder)
    (args.folder / "original_artwork_overlay_verdict.json").write_text(
        json.dumps(result, indent=2) + "\n")
    print(json.dumps(result, indent=2))
    return 0 if result["status"] == "ORIGINAL_LOADING_ARTWORK_WITH_SYNTHETIC_OVERLAY_VISIBLE" else 1


if __name__ == "__main__":
    raise SystemExit(main())
