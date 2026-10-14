#!/usr/bin/env python3
"""Correlate isolated synthetic overlay pixels with the ORIGINAL Activity PID.

Read-only proof from a native Android screenshot experiment. A snapshot of
ActivityManager and a PID query immediately preceding a screenshot are not
atomic; never claim exact simultaneous nanosecond-level synchronization.
"""
import argparse
import json
import re
from pathlib import Path

PACKAGE = "com.catsbit.oxidesurvivalisland"
ACTIVITY = "OxideActivity"
PROBE = "com.oxide.bridgeprobe"


def check(directory):
    folder = Path(directory)
    forensic = json.loads((folder / "forensic_summary.json").read_text(encoding="utf-8"))
    visuals = json.loads((folder / "loading_overlay_verdict.json").read_text(encoding="utf-8"))
    pid = forensic.get("initial_activity_pid")
    if type(pid) is not int or pid <= 0:
        raise ValueError("Cannot attribute images to original Activity without verified PID")
    samples = []
    for i in (1, 2, 3):
        name = f"overlay_on_game_{i}.png"
        activity_path = folder / f"activities_capture_{i}.txt"
        pid_path = folder / f"game_pid_capture_{i}.txt"
        activity = activity_path.read_text(errors="replace") if activity_path.is_file() else ""
        observed_pids = set(map(int, re.findall(r"\b\d+\b", pid_path.read_text(errors="replace")))) \
            if pid_path.is_file() else set()
        # Android topResumedActivity / mResumedActivity may include
        # ActivityRecord{... user/package/activity task-id}. Never accept
        # an Activity merely listed in stopped/finishing history.
        resumed = any(
            ("topResumedActivity" in line or "mResumedActivity" in line)
            and PACKAGE + "/" in line and ACTIVITY in line
            for line in activity.splitlines()
        )
        green = (
            name in visuals.get("game_screens_with_test_marker", [])
            and visuals.get("captures", {}).get(name, {}).get("pixels", 0) >= 500
        )
        samples.append({
            "capture": name,
            "original_activity_pid_still_listed_before_capture": pid in observed_pids,
            "original_activity_resumed_before_capture": resumed,
            "synthetic_overlay_marker_visible_in_capture": green,
            "sample_correlates": pid in observed_pids and resumed and green,
        })
    service = visuals.get("service_start") == "DIAGNOSTIC_OVERLAY_SERVICE_START=PASS"
    cleared = visuals.get("overlay_disappeared_after_stop") is True
    visual_pass = visuals.get("status") == "ORIGINAL_LOADING_OVERLAY_PASS"
    accepted = service and cleared and visual_pass and any(x["sample_correlates"] for x in samples)
    return {
        "status": (
            "ORIGINAL_LOADING_ACTIVITY_AND_STANDALONE_OVERLAY_CORROBORATED"
            if accepted else "INSUFFICIENT_SAME_CAPTURE_ORIGINAL_ACTIVITY_EVIDENCE"
        ),
        "original_activity_pid": pid,
        "capture_samples": samples,
        "diagnostic_overlay_service_started": service,
        "diagnostic_overlay_removed_after_stop": cleared,
        "synthetic_coordinates_only": True,
        "original_activity_35_seconds_verified": False,
        "real_multiplayer_gameplay_verified": False,
        "real_player_esp_verified": False,
        "synchronization_caveat":
            "ActivityManager and pidof snapshots precede PNG capture; no atomic lock across apps. "
            "This proves near-concurrent loading Activity/overlay snapshots, not gameplay.",
        "game_security_root_cause": "NOT_IDENTIFIED",
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("evidence_dir", type=Path)
    args = parser.parse_args()
    result = check(args.evidence_dir)
    out = args.evidence_dir / "original_overlay_overlap_verdict.json"
    out.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(result, indent=2))
    return 0 if result["status"] == "ORIGINAL_LOADING_ACTIVITY_AND_STANDALONE_OVERLAY_CORROBORATED" else 1


if __name__ == "__main__":
    raise SystemExit(main())
