#!/usr/bin/env python3
"""Generic screenshot readiness gate; a live PID alone does not prove a rendered frame.

A screenshot with fewer than the required distinct RGB colors is not accepted
as evidence of gameplay. This is a smoke-test heuristic, NOT visual parity.
"""
import argparse
import json
import math
from pathlib import Path
from PIL import Image, ImageChops, ImageStat


def inspect_spatial_coverage(rgb, min_colors=16):
    """Require scene detail across the frame, not only a colorful HUD corner.

    This is a generic 3x3 framebuffer readiness heuristic. A passing result
    is NOT proof of the original game's materials, rendering, or behavior.
    """
    width, height = rgb.size
    if width < 3 or height < 3:
        raise ValueError("frame too small for spatial coverage")
    counts = []
    for tile_y in range(3):
        for tile_x in range(3):
            box = (width * tile_x // 3, height * tile_y // 3,
                   width * (tile_x + 1) // 3, height * (tile_y + 1) // 3)
            colors = rgb.crop(box).getcolors(maxcolors=min_colors - 1)
            counts.append(min_colors if colors is None else len(colors))
    varied = sum(count >= min_colors for count in counts)
    return {
        "spatial_grid": [3, 3],
        "tile_color_lower_bounds": counts,
        "varied_tiles": varied,
        "center_tile_varied": counts[4] >= min_colors,
        "spatial_scene_coverage": varied >= 3 and counts[4] >= min_colors,
    }


def inspect_frame(path, min_colors=16):
    if not 2 <= min_colors <= 256:
        raise ValueError("min_colors must be between 2 and 256")
    with Image.open(path) as source:
        if source.format != "PNG":
            raise ValueError("expected a real PNG screenshot")
        source.load()
        width, height = source.size
        if width < 320 or height < 240:
            raise ValueError(f"invalid Android screenshot resolution: {width}x{height}")
        rgb = source.convert("RGB")
        # Pillow returns None as soon as there are >= min_colors unique colors.
        few_colors = rgb.getcolors(maxcolors=min_colors - 1)
        ready = few_colors is None
        count_floor = min_colors if ready else len(few_colors)
        extrema = rgb.getextrema()
        spatial = inspect_spatial_coverage(rgb, min_colors)
        return {
            "width": width, "height": height,
            "min_distinct_rgb_colors": min_colors,
            "distinct_rgb_colors_at_least": count_floor,
            "color_count_is_lower_bound": ready,
            "rgb_channel_extrema": [list(pair) for pair in extrema],
            "nonflat_frame": ready,
            **spatial,
            "scene_ready": ready and spatial["spatial_scene_coverage"],
            "gameplay_or_visual_parity_proven": False,
        }



def inspect_transition(before_path, after_path, min_changed_fraction=0.01,
                       min_channel_delta=30):
    """Measure real screenshot change; alone this does NOT prove a menu opened.

    Compare decoded RGB pixels, not PNG file sizes or byte encodings. The
    runtime action log must independently prove the expected UI transition.
    """
    if (type(min_changed_fraction) not in (float, int) or
            not math.isfinite(min_changed_fraction) or
            not 0 < min_changed_fraction <= 1):
        raise ValueError("min_changed_fraction must be finite and in (0, 1]")
    if type(min_channel_delta) is not int or not 1 <= min_channel_delta <= 255:
        raise ValueError("min_channel_delta must be an integer in [1, 255]")
    with Image.open(before_path) as a, Image.open(after_path) as b:
        if a.format != "PNG" or b.format != "PNG":
            raise ValueError("both captures must be PNG")
        a.load()
        b.load()
        if a.size != b.size:
            raise ValueError("screenshot resolutions differ")
        width, height = a.size
        if width < 320 or height < 240:
            raise ValueError("screenshot resolution is too small")
        diff = ImageChops.difference(a.convert("RGB"), b.convert("RGB"))
        r, g, blue = diff.split()
        max_delta = ImageChops.lighter(ImageChops.lighter(r, g), blue)
        mask = max_delta.point(lambda value: 255 if value >= min_channel_delta else 0)
        changed = mask.histogram()[255]
        fraction = changed / (width * height)
        bbox = mask.getbbox()
        return {
            "width": width, "height": height,
            "changed_pixels": changed,
            "changed_fraction": round(fraction, 6),
            "min_changed_fraction": min_changed_fraction,
            "min_channel_delta": min_channel_delta,
            "mean_abs_rgb_delta": round(sum(ImageStat.Stat(diff).mean) / 3, 4),
            "difference_bounds": list(bbox) if bbox else None,
            "transition_detected": fraction >= min_changed_fraction,
            "action_or_original_game_parity_proven": False,
        }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("screenshot", type=Path)
    parser.add_argument("--min-colors", type=int, default=16)
    parser.add_argument("--require-spatial-coverage", action="store_true",
                        help="Reject colorful HUD-only frames over a flat scene")
    parser.add_argument("--compare-with", type=Path, help="Baseline screenshot for pixel transition audit")
    parser.add_argument("--min-changed-fraction", type=float, default=0.01)
    parser.add_argument("--min-channel-delta", type=int, default=30)
    parser.add_argument("--report", type=Path)
    args = parser.parse_args()
    try:
        if args.compare_with:
            result = inspect_transition(args.compare_with, args.screenshot,
                                        args.min_changed_fraction, args.min_channel_delta)
            passed = result["transition_detected"]
        else:
            result = inspect_frame(args.screenshot, args.min_colors)
            passed = (result["scene_ready"] if args.require_spatial_coverage
                      else result["nonflat_frame"])
    except Exception as exc:
        result = {"transition_detected" if args.compare_with else "nonflat_frame": False,
                  "error": str(exc)}
        passed = False
    payload = json.dumps(result, indent=2) + "\n"
    if args.report:
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(payload, encoding="utf-8")
    print(payload, end="")
    return 0 if passed else 1


if __name__ == "__main__":
    raise SystemExit(main())
