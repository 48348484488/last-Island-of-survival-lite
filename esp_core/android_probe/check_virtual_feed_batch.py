#!/usr/bin/env python3
"""Strict real-screen verification for eight synthetic ADB batch targets."""
import json
import re
import sys
from pathlib import Path
from PIL import Image

X = (-3.5, -2.5, -1.5, -0.5, 0.5, 1.5, 2.5, 3.5)
STAGES = ("all", "half", "replay", "changed_camera", "idle", "stopped")
LOG_EVENT = re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_BATCH_SEQ=(\d+) INPUT_COUNT=(\d+) RECT_COUNT=(\d+)")
CRASH = re.compile(r"OXIDE_BRIDGE_OVERLAY:.*RESULT=FAIL|AndroidRuntime:.*Process: com\.oxide\.bridgeprobe|Fatal signal.*com\.oxide\.bridgeprobe")

def image_bands(path, matrix_scale=1.0):
    with Image.open(path) as source:
        im = source.convert("RGB")
        width, height = im.size
        pixels = im.load()
        centers = [(0.5 + x * matrix_scale / 10.0) * width for x in X]
        hits = [0] * len(X)
        # At z=-5, projected ±0.35 head/foot draws a 90px-high rectangle
        # on a 720x1280 image. Isolate each box's unique horizontal band.
        for i, cx in enumerate(centers):
            for y in range(int(height * .40), int(height * .60)):
                for px in range(max(0, int(cx)-25), min(width, int(cx)+26)):
                    r,g,b = pixels[px,y]
                    if 15 <= r <= 110 and g >= 190 and b <= 160 and g > r*2 and g > b*1.3:
                        hits[i] += 1
        return {"hits": hits, "centers_px": [round(c,1) for c in centers],
                "resolution": [width,height]}

def evaluate(folder):
    root = Path(folder)
    log = (root/"batch_logcat.txt").read_text(errors="replace")
    screens = {name: image_bands(root/f"batch_{name}.png",
                  .8 if name=="changed_camera" else 1.) for name in STAGES}
    valid = lambda stage: screens[stage]["hits"]
    pids = [(root/f"batch_pid_{s}.txt").read_text().strip() for s in STAGES[:-1]]
    # Camera changed from x-scale 1.0 to 0.8. The extreme original
    # marker bands should now be empty; the new expected bands nonempty.
    alternate_old = image_bands(root/"batch_changed_camera.png",1.0)["hits"]
    checks = {
        "eight_fresh_visible": all(v >= 150 for v in valid("all")),
        "four_independently_stale": all((v >= 150 if i%2==0 else v <= 30)
                                         for i,v in enumerate(valid("half"))),
        "replay_keeps_four_stale": all((v >= 150 if i%2==0 else v <= 30)
                                       for i,v in enumerate(valid("replay"))),
        "custom_camera_eight_visible": all(v >= 150 for v in valid("changed_camera")),
        "custom_camera_moved_outer_targets": alternate_old[0] <= 30 and alternate_old[-1] <= 30,
        "silent_idle_clears_all": max(valid("idle")) <= 30,
        "service_stop_clears_all": max(valid("stopped")) <= 30,
        "exact_three_native_batch_updates": [(int(a),int(b),int(c)) for a,b,c in LOG_EVENT.findall(log)] ==
                                              [(100,8,8),(101,8,4),(102,8,8)],
        "replay_blocked": log.count("RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=101") == 1,
        "bad_array_lengths_blocked": log.count("RESULT=DROP INVALID_BATCH_SHAPE SEQ=102") == 1,
        "duplicate_ids_blocked": log.count("RESULT=DROP INVALID_BATCH_PROJECTION SEQ=102") == 1,
        "watchdog_fired": log.count("RESULT=PASS FEED_IDLE_TIMEOUT RECT_COUNT=0") == 1,
        "window_removed": log.count("RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE") == 1,
        "stable_service_pid": bool(all(re.fullmatch(r"\d+", p) for p in pids) and len(set(pids)) == 1),
        "no_android_jni_crash": CRASH.search(log) is None,
        "original_oxide_absent": "package:com.catsbit.oxidesurvivalisland" not in
                                 (root/"batch_game_packages.txt").read_text(errors="replace"),
    }
    return {"status": "ARM64_VIRTUAL_8_ENTITY_BATCH_PASS" if all(checks.values())
                      else "ARM64_VIRTUAL_BATCH_NOT_CONFIRMED",
            "checks": checks, "screenshots": screens, "old_camera_bands_after_change": alternate_old,
            "original_game_executed": False, "real_player_data_verified": False}

if __name__=="__main__":
    result=evaluate(sys.argv[1])
    (Path(sys.argv[1])/"batch_verdict.json").write_text(json.dumps(result,indent=2)+"\n")
    print(json.dumps(result,indent=2))
    if result["status"]!="ARM64_VIRTUAL_8_ENTITY_BATCH_PASS": raise SystemExit(1)
