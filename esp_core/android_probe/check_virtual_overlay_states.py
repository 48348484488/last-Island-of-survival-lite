#!/usr/bin/env python3
"""Actual Android screenshot and Android log proof for isolated overlay states.

Runs on screenshots taken from native ARM64 Redroid, never on original game
screens; distinguishes fresh native-projected synthetic rectangle, empty,
stale and service-removed states. The analyzer is fail-closed.
"""
import argparse
import json
from pathlib import Path
import re

from check_overlay import count_markers

MARKERS={
    "fresh":"OVERLAY_DIAGNOSTIC",
    "empty":"OVERLAY_NO_SAMPLES",
    "stale":"OVERLAY_STALE_SAMPLES",
}
LOG=re.compile(
    r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS MODE=(\S+)"
    r" RECT_COUNT=(\d+) SOURCE=CONTROLLED_SYNTHETIC_ONLY"
)

def evaluate(folder):
    folder=Path(folder)
    shots={mode:count_markers(folder/("overlay_"+mode+".png"))
           for mode in (*MARKERS,"removed")}
    log=(folder/"logcat.txt").read_text(errors="replace")
    matched=list(LOG.finditer(log))
    occurrences={mode:sum(x.group(1)==marker and
                          int(x.group(2))==(4 if mode=="fresh" else 0)
                          for x in matched) for mode,marker in MARKERS.items()}
    fresh,empty,stale,removed=(shots[k] for k in
                               ("fresh","empty","stale","removed"))
    upper=max(35,int(fresh*0.04),removed+25)
    checks={
        "fresh_marker_visible":fresh>=500,
        "zero_sample_marker_cleared":empty<=upper,
        "stale_sample_marker_cleared":stale<=upper,
        "overlay_disappeared_after_stop":removed<=max(35,int(fresh*0.04)),
        "service_log_marker_each_mode":all(n==1 for n in occurrences.values()),
        "no_service_failure_log":"OXIDE_BRIDGE_OVERLAY" in log and
              not re.search(r"OXIDE_BRIDGE_OVERLAY:.*RESULT=FAIL",log),
        "no_android_runtime_crash":not re.search(
              r"AndroidRuntime:.*Process: com\.oxide\.bridgeprobe"
              r"|Fatal signal.*com\.oxide\.bridgeprobe",log),
    }
    for mode in MARKERS:
        text=(folder/("pid_"+mode+".txt")).read_text().strip()
        checks["service_pid_alive_"+mode]=bool(re.fullmatch(r"\d+(?:\s+\d+)*",text))
        checks["activity_launch_ok_"+mode]="Status: ok" in (
            folder/("launch_"+mode+".txt")).read_text()
    installed=(folder/"game_packages.txt").read_text(errors="replace")
    checks["original_game_absent"]="package:com.catsbit.oxidesurvivalisland" not in installed
    return {
        "result":"VIRTUAL_ARM64_ISOLATED_OVERLAY_3_STATES_AND_STOP_PASS"
                 if all(checks.values()) else "VIRTUAL_ARM64_OVERLAY_NOT_CONFIRMED",
        "checks":checks,
        "green_center_region_pixels":shots,
        "android_service_pass_markers":occurrences,
        "fresh_rectangles_from_native_jni":1 if checks["service_log_marker_each_mode"] else None,
        "original_game_installed":False if checks["original_game_absent"] else "UNEXPECTED",
        "original_game_executed":False,
        "player_positions_from_game":False,
        "real_multiplayer_esp_verified":False,
        "limits":"Synthetic JNI projections on native ARM64 Android launcher; "
                 "not playable original game, not real-player ESP, and no bypass.",
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("directory",type=Path)
    a=p.parse_args()
    result=evaluate(a.directory)
    (a.directory/"overlay_states_verdict.json").write_text(
        json.dumps(result,indent=2)+"\n",encoding="utf-8")
    print(json.dumps(result,indent=2))
    if result["result"]!="VIRTUAL_ARM64_ISOLATED_OVERLAY_3_STATES_AND_STOP_PASS":
        raise SystemExit(1)

if __name__=="__main__":
    main()
