#!/usr/bin/env python3
"""Require real ADB-controlled snapshots on one virtual ARM64 Android PID."""
import json
import re
from pathlib import Path
from check_virtual_overlay_sequence import evaluate,measure

ACCEPTED=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_SEQ=(\d+)"
                    r" AGE_MS=(\d+) RECT_COUNT=(\d+) SYNTHETIC_ONLY")
REPLAY=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=2")
STOP=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_STOP_REQUEST SEQ=4")
READY=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS FEED_READY SYNTHETIC_SHELL_ONLY")

def check(folder):
    root=Path(folder)
    base=evaluate(root)
    log=(root/"sequence_logcat.txt").read_text(errors="replace")
    accepted=[tuple(map(int,m.groups())) for m in ACCEPTED.finditer(log)]
    replay_pixels=measure(root/"sequence_replay.png")
    stale=base["screenshot_geometry"]["STALE"]
    checks={
        "live_sequence_visual_and_pid_proved":
            base["status"]=="NATIVE_VIRTUAL_ARM64_CONTINUOUS_JNI_OVERLAY_PASS",
        "shell_feed_receiver_ready":len(READY.findall(log))==1,
        "exact_external_sequences_accepted":accepted==[(1,5,1),(2,600,0),(3,5,1)],
        "out_of_order_input_explicitly_rejected":len(REPLAY.findall(log))==1,
        "replayed_input_did_not_restore_expired_box":
            replay_pixels["green_pixels"]<=max(35,stale["green_pixels"]+25),
        "service_stopped_via_next_accepted_sequence":len(STOP.findall(log))==1,
        "no_unexpected_original_game":base["original_game_installed"] is False,
        "same_android_service_pid":base["stable_android_service_pids"],
        "no_real_game_data_claim":base["live_multiplayer_or_real_player_esp_verified"] is False,
    }
    return {
        "status":("NATIVE_ARM64_ADB_FEED_REPLAY_SAFE_PASS" if all(checks.values())
                  else "NATIVE_ARM64_ADB_FEED_NOT_CONFIRMED"),
        "checks":checks,
        "accepted_synthetic_sequences":accepted,
        "replay_screenshot":replay_pixels,
        "projected_screenshots":base["screenshot_geometry"],
        "android_virtual_only":True,
        "game_executed":False,
        "real_player_data_used":False,
        "game_protection_modified":False,
        "multiplayer_esp_verified":False,
    }

def main():
    import argparse
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("folder",type=Path)
    a=p.parse_args()
    d=check(a.folder)
    (a.folder/"feed_verdict.json").write_text(
        json.dumps(d,indent=2)+"\n",encoding="utf-8")
    print(json.dumps(d,indent=2))
    if d["status"]!="NATIVE_ARM64_ADB_FEED_REPLAY_SAFE_PASS":
        raise SystemExit(1)

if __name__=="__main__":
    main()
