#!/usr/bin/env python3
"""Fail-closed screenshot verdict: two independent synthetic Android boxes."""
import json
import re
import sys
from pathlib import Path
from PIL import Image

STAGES = ("both", "one", "replay", "empty", "restored", "moved", "stopped")
EVENT = re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_MULTI_SEQ=(\d+) RECT_COUNT=(\d+) SYNTHETIC_ONLY")
CRASH = re.compile(r"OXIDE_BRIDGE_OVERLAY:.*RESULT=FAIL|AndroidRuntime:.*Process: com\.oxide\.bridgeprobe|Fatal signal.*com\.oxide\.bridgeprobe")

def sides(file):
    with Image.open(file) as im0:
        im = im0.convert("RGB")
        w, h = im.size
        counts = [0, 0]
        for y in range(int(h*.30), int(h*.80)):
            for x in range(int(w*.20), int(w*.80)):
                r,g,b = im.getpixel((x,y))
                if 15 <= r <= 110 and g >= 190 and b <= 160 and g > r*2 and g > b*1.3:
                    counts[int(x >= w//2)] += 1
        return counts

def centers(file):
    with Image.open(file) as im0:
        im = im0.convert("RGB")
        w, h = im.size
        counts = [0, 0]
        sum_x = [0, 0]
        for y in range(int(h*.30), int(h*.80)):
            for x in range(int(w*.20), int(w*.80)):
                r,g,b = im.getpixel((x,y))
                if 15 <= r <= 110 and g >= 190 and b <= 160 and g > r*2 and g > b*1.3:
                    i = int(x >= w//2)
                    counts[i] += 1
                    sum_x[i] += x
        return [round(sum_x[i]/counts[i], 2) if counts[i] else None for i in (0, 1)]

def evaluate(folder):
    root = Path(folder)
    log = (root/"multi_logcat.txt").read_text(errors="replace")
    pixels = {stage: sides(root/("multi_"+stage+".png")) for stage in STAGES}
    pids = [(root/("multi_pid_"+stage+".txt")).read_text().strip() for stage in STAGES[:-1]]
    # Both rectangles overlap when they approach the viewport center; the\n    # per-half green-pixel centroid shifts less than either true box center.\n    # Compare directly measured half-screen centroids, not nominal coordinates.\n    start_x = centers(root/"multi_both.png")
    moved_x = centers(root/"multi_moved.png")
    checks = {
        "two_boxes": min(pixels["both"]) >= 400,
        "second_expired_independently": pixels["one"][0] >= 400 and pixels["one"][1] <= 35,
        "replay_keeps_second_hidden": pixels["replay"][0] >= 400 and pixels["replay"][1] <= 35,
        "both_expired": max(pixels["empty"]) <= 35,
        "both_restored": min(pixels["restored"]) >= 400,
        "both_visible_after_independent_motion": min(pixels["moved"]) >= 400,
        "left_target_moved_right": all(x is not None for x in (start_x[0], moved_x[0])) and
                                   moved_x[0] - start_x[0] >= 20,
        "right_target_moved_left": all(x is not None for x in (start_x[1], moved_x[1])) and
                                   start_x[1] - moved_x[1] >= 20,
        "window_cleared_on_stop": max(pixels["stopped"]) <= 35,
        "native_jni_count_sequence": [(int(a),int(b)) for a,b in EVENT.findall(log)] == [(1,2),(2,1),(3,0),(4,2),(5,2)],
        "replay_denied": log.count("RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=2") == 1,
        "malformed_second_denied": log.count("RESULT=DROP MALFORMED_SECOND_TARGET") == 1,
        "service_window_removed": log.count("RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE") == 1,
        "unchanged_android_pid": all(re.fullmatch(r"\d+", x) for x in pids) and len(set(pids)) == 1,
        "no_app_crash": CRASH.search(log) is None,
        "original_game_absent": "package:com.catsbit.oxidesurvivalisland" not in (root/"multi_game_packages.txt").read_text(errors="replace"),
    }
    return {"status": "NATIVE_ARM64_TWO_BOXES_PASS" if all(checks.values()) else "TWO_BOXES_NOT_CONFIRMED",
            "checks": checks, "screenshot_green_pixels_left_right": pixels,
            "initial_box_centers": start_x, "moved_box_centers": moved_x,
            "original_game_executed": False, "real_player_data_verified": False}

if __name__ == "__main__":
    r=evaluate(sys.argv[1])
    (Path(sys.argv[1])/"multi_verdict.json").write_text(json.dumps(r,indent=2)+"\n")
    print(json.dumps(r,indent=2))
    if r["status"] != "NATIVE_ARM64_TWO_BOXES_PASS":
        raise SystemExit(1)
