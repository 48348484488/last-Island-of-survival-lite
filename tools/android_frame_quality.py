#!/usr/bin/env python3
"""Generic screenshot readiness gate; a live PID alone does not prove a rendered frame.

A screenshot with fewer than the required distinct RGB colors is not accepted
as evidence of gameplay. This is a smoke-test heuristic, NOT visual parity.
"""
import argparse
import json
from pathlib import Path
from PIL import Image


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
        return {
            "width": width, "height": height,
            "min_distinct_rgb_colors": min_colors,
            "distinct_rgb_colors_at_least": count_floor,
            "color_count_is_lower_bound": ready,
            "rgb_channel_extrema": [list(pair) for pair in extrema],
            "nonflat_frame": ready,
            "gameplay_or_visual_parity_proven": False,
        }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("screenshot", type=Path)
    parser.add_argument("--min-colors", type=int, default=16)
    parser.add_argument("--report", type=Path)
    args = parser.parse_args()
    try:
        result = inspect_frame(args.screenshot, args.min_colors)
    except Exception as exc:
        result = {"nonflat_frame": False, "error": str(exc)}
    payload = json.dumps(result, indent=2) + "\n"
    if args.report:
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(payload, encoding="utf-8")
    print(payload, end="")
    return 0 if result["nonflat_frame"] else 1


if __name__ == "__main__":
    raise SystemExit(main())
