#!/usr/bin/env python3
"""Real ARM64 virtual Android isolated overlay stress verdict.

Requires continuous SAME PID, 64 distinct shell-delivered synthetic JNI
snapshots, strict type rejection, stale/duplicate suppression, new-data
recovery, idle auto-clear, and teardown screenshots. No game data is used.
"""
import argparse
import json
import re
from pathlib import Path
from check_virtual_overlay_sequence import measure

COUNT=64
ACCEPT=re.compile(
    r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_SEQ=(\d+)"
    r" AGE_MS=(\d+) RECT_COUNT=(\d+) SYNTHETIC_ONLY"
)
DROP_TYPE=re.compile(r"OXIDE_BRIDGE_FEED:\s+RESULT=DROP MALFORMED_FIELDS")
DROP_MISSING=re.compile(r"OXIDE_BRIDGE_FEED:\s+RESULT=DROP MISSING_REQUIRED_FIELDS SEQ=68")
DROP_REPLAY=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=66")
STOP=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_STOP_REQUEST SEQ=68")
STOPPED=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE")
IDLE=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_IDLE_TIMEOUT RECT_COUNT=0")
FAIL=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=FAIL|AndroidRuntime:.*Process: com\.oxide\.bridgeprobe|Fatal signal.*com\.oxide\.bridgeprobe")

STAGES=("FIRST","LAST","STALE","REPLAY","RESTORED","IDLE","STOPPED")
LIVE=("FIRST","LAST","STALE","REPLAY","RESTORED","IDLE")

def evaluate(folder):
    root=Path(folder)
    log=(root/"stress_logcat.txt").read_text(errors="replace")
    images={stage:measure(root/("stress_"+stage.lower()+".png"))
            for stage in STAGES}
    pids={stage:(root/("stress_pid_"+stage.lower()+".txt")).read_text(errors="replace").strip()
          for stage in STAGES}
    accepted=[tuple(map(int,m.groups())) for m in ACCEPT.finditer(log)]
    expected=[(i,5,1) for i in range(1,COUNT+1)]
    expected.extend([(65,5,1),(66,600,0),(67,5,1)])
    first=images["FIRST"];last=images["LAST"]
    cleared_threshold=max(35,int(first["green_pixels"]*.04))
    cleared=lambda stage: images[stage]["green_pixels"]<=cleared_threshold
    same_pid=bool(all(re.fullmatch(r"\d+",pids[k]) for k in LIVE) and
                  len({pids[k] for k in LIVE})==1)
    checks={
        "all_64_consecutive_native_snapshots_accepted_exactly_once":accepted==expected,
        "wrong_type_rejected_without_consuming_sequence":len(DROP_TYPE.findall(log))==1,
        "missing_coordinate_rejected_without_consuming_sequence":len(DROP_MISSING.findall(log))==1,
        "repeated_stale_packet_rejected":len(DROP_REPLAY.findall(log))==1,
        "explicit_stop_delivered":len(STOP.findall(log))==1,
        "window_removed_by_android_service":len(STOPPED.findall(log))==1,
        "timeout_cleared_once":len(IDLE.findall(log))==1,
        "no_android_or_jni_crash":FAIL.search(log) is None,
        "all_live_screens_same_pid":same_pid,
        "first_box_visible":first["green_pixels"]>=500,
        "last_box_visible":last["green_pixels"]>=500,
        "first_to_last_box_moved":first["center_x"] is not None and
                                  last["center_x"] is not None and
                                  last["center_x"]-first["center_x"]>=40,
        "stale_pixels_cleared":cleared("STALE"),
        "replay_did_not_restore_box":cleared("REPLAY"),
        "fresh_after_stale_restores_box":images["RESTORED"]["green_pixels"]>=500,
        "idle_timeout_cleared_without_new_data":cleared("IDLE"),
        "stopped_overlay_pixels_cleared":cleared("STOPPED"),
    }
    pkg=(root/"stress_game_packages.txt").read_text(errors="replace")
    checks["original_oxide_not_installed"]="package:com.catsbit.oxidesurvivalisland" not in pkg
    return {
        "status":"NATIVE_ARM64_VIRTUAL_64_SNAPSHOT_FEED_STRESS_PASS"
                  if all(checks.values()) else "NATIVE_ARM64_FEED_STRESS_NOT_CONFIRMED",
        "checks":checks,
        "accepted_snapshot_count":len(accepted),
        "accepted_snapshot_sequence_first_last":[accepted[0][0],accepted[-1][0]]
                                                if accepted else [],
        "synthetic_snapshot_count_requested":COUNT,
        "pixel_evidence":images,
        "same_android_service_pid":same_pid,
        "original_oxide_executed":False,
        "real_player_coordinates_proven":False,
        "original_game_security_altered":False,
        "limitations":"Only independent diagnostic JNI and TYPE_APPLICATION_OVERLAY; "
                      "no original game, no multiplayer, no ESP with real players.",
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("folder",type=Path)
    opt=p.parse_args()
    result=evaluate(opt.folder)
    (opt.folder/"stress_verdict.json").write_text(
        json.dumps(result,indent=2)+"\n",encoding="utf-8")
    print(json.dumps(result,indent=2))
    if result["status"]!="NATIVE_ARM64_VIRTUAL_64_SNAPSHOT_FEED_STRESS_PASS":
        raise SystemExit(1)

if __name__=="__main__":
    main()
