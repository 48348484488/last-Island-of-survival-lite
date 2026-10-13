#!/usr/bin/env python3
"""Validate test overlay pixels on ORIGINAL game loading screen, including dimming.

The native screenshot taken during Android's immersive-mode transition can
darken the overlay dramatically. The ordinary bright-green-only heuristic
would reject real pixels. This script treats both bright and dim green while
requiring a centered geometric marker, a cleared final frame, and an
independent foreground Activity observation.

Only proves diagnostic overlay composition. No ESP/gameplay claims.
"""
import argparse
import json
from pathlib import Path
from PIL import Image


def measure(path):
    with Image.open(path) as im:
        rgb = im.convert("RGB")
        w, h = rgb.size
        hits = []
        for y in range(int(h * .35), int(h * .72)):
            for x in range(int(w * .32), int(w * .68)):
                r, g, b = rgb.getpixel((x, y))
                if 5 <= r <= 110 and g >= 35 and b <= 160 and g > r * 2 and g > b * 1.3:
                    hits.append((x, y))
        bounds = None
        if hits:
            bounds = [min(x for x, _ in hits), min(y for _, y in hits),
                      max(x for x, _ in hits), max(y for _, y in hits)]
        return {"pixels": len(hits), "width": w, "height": h, "bounds": bounds}


def plausible_centered_box(hit):
    if hit["pixels"] < 500 or not hit["bounds"]:
        return False
    l, t, r, b = hit["bounds"]
    w, h = hit["width"], hit["height"]
    center_x = (l + r) / 2
    center_y = (t + b) / 2
    return (r - l >= 15 and b - t >= 15 and
            w * .38 <= center_x <= w * .62 and
            h * .30 <= center_y <= h * .76)


def evaluate(directory):
    root = Path(directory)
    names = ("overlay_on_launcher.png", "overlay_on_game_1.png",
             "overlay_on_game_2.png", "overlay_removed.png")
    captures = {n: measure(root / n) for n in names}
    game_boxes = [name for name in ("overlay_on_game_1.png", "overlay_on_game_2.png")
                  if plausible_centered_box(captures[name])]
    game_max = max(captures[name]["pixels"] for name in game_boxes) if game_boxes else 0
    off = captures["overlay_removed.png"]["pixels"]
    disappears = off <= max(10, int(game_max * .02))
    foreground = (root / "original_foreground.txt").read_text(encoding="utf-8").strip()
    service = (root / "overlay_service.txt").read_text(encoding="utf-8").strip()
    confirmed = (foreground == "ORIGINAL_ACTIVITY_RESUMED_DURING_CAPTURE=OBSERVED"
                 and service == "DIAGNOSTIC_OVERLAY_SERVICE_START=PASS")
    status = ("ORIGINAL_LOADING_OVERLAY_PASS" if game_boxes and disappears and confirmed
              else "ORIGINAL_LOADING_OVERLAY_NOT_CONFIRMED")
    return {
        "status": status,
        "captures": captures,
        "game_screens_with_test_marker": game_boxes,
        "overlay_disappeared_after_stop": disappears,
        "original_activity_foreground": foreground,
        "service_start": service,
        "snapshot_source": "synthetic diagnostic coordinates only",
        "original_gameplay_verified": False,
        "real_players_verified": False,
        "limits": "Visual overlay on loading Activity only; not ESP functionality or game security bypass.",
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("evidence_dir", type=Path)
    opts = parser.parse_args()
    result = evaluate(opts.evidence_dir)
    (opts.evidence_dir / "loading_overlay_verdict.json").write_text(
        json.dumps(result, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(result, indent=2))
    return 0 if result["status"] == "ORIGINAL_LOADING_OVERLAY_PASS" else 1


if __name__ == "__main__":
    raise SystemExit(main())
