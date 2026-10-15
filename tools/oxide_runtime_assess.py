#!/usr/bin/env python3
"""Conservative, read-only evaluation of original Oxide runtime evidence."""
import argparse
import json
import re
from pathlib import Path

# Handle module imports and the direct script entry point used in Android CI.
if __package__:
    from .oxide_goal_engine import ORIGINAL, resumed, first_resumed_original
    from .oxide_capture_timing_gate import verify_capture_timing
else:
    from oxide_goal_engine import ORIGINAL, resumed, first_resumed_original
    from oxide_capture_timing_gate import verify_capture_timing


def _value(directory, name):
    p = directory / name
    return p.read_text(errors="replace").strip() if p.is_file() else ""


def _original_pid(value):
    """Reject multiple or non-numeric PID values from pidof."""
    return bool(re.fullmatch(r"[1-9][0-9]*", value))


def _valid_png(path):
    if not path.is_file():
        return False
    with path.open("rb") as image:
        return image.read(8) == b"\x89PNG\r\n\x1a\n"


def evaluate(directory):
    d = Path(directory)
    preflight_file = d / "preflight.json"
    preflight = json.loads(preflight_file.read_text()) if preflight_file.is_file() else {}
    if preflight.get("next_action") == "HOST_NO_BINDER":
        return {"status": "BLOCKED_HOST_BINDER", "gameplay_confirmed": False}
    if _value(d, "boot.txt") != "1":
        return {"status": "BLOCKED_ANDROID_BOOT", "gameplay_confirmed": False}
    if _value(d, "abi.txt") != "arm64-v8a":
        return {"status": "BLOCKED_WRONG_ABI", "gameplay_confirmed": False}
    if "Success" not in _value(d, "install.txt"):
        return {"status": "BLOCKED_ORIGINAL_APK_INSTALL", "gameplay_confirmed": False}

    pids = [_value(d, f"pid-{t}.txt") for t in (0, 10, 35)]
    first = first_resumed_original(d)
    baseline_pid = first["pid"] if first else pids[0]
    stable = _original_pid(baseline_pid) and pids[1] == pids[2] == baseline_pid
    if not stable:
        return {"status": "FAILED_PID_STABILITY_35S", "pids": pids,
                "first_original_resumed_checkpoint": first["checkpoint"] if first else None,
                "gameplay_confirmed": False}
    launch = _value(d, "launch.txt")
    if "Status: ok" not in launch or ORIGINAL not in launch:
        return {"status": "PID_STABLE_MISSING_ORIGINAL_LAUNCH", "pids": pids, "gameplay_confirmed": False}
    activities = [_value(d, f"activity-{t}.txt") for t in (10, 35)]
    screenshots = [d / f"screen-{t}.png" for t in (10, 35)]
    pngs = all(_valid_png(p) for p in screenshots)
    if not all(activities) or not pngs:
        return {"status": "PID_STABLE_MISSING_VISUAL_EVIDENCE", "pids": pids, "gameplay_confirmed": False}
    if first is None or not all(resumed(activity) for activity in activities):
        return {"status": "PID_STABLE_WITHOUT_ORIGINAL_RESUMED_ACTIVITY", "pids": pids, "gameplay_confirmed": False}
    timing = verify_capture_timing(d)
    if not timing["verified"]:
        return {"status": "PID_STABLE_UNVERIFIED_35S_TIMING", "pids": pids,
                "timing": timing, "gameplay_confirmed": False}
    return {
        "status": "PID_STABLE_35S_MENU_NOT_YET_REVIEWED",
        "timing": timing,
        "pids": pids,
        "screenshots_available": True,
        "original_activity_resumed_at_early_10_35": True,
        "first_original_resumed_checkpoint": first["checkpoint"],
        "first_original_resumed_pid": first["pid"],
        "original_activity_resumed_at_0_10_35": first["checkpoint"] == 0,
        "gameplay_confirmed": False,
        "note": "Matching original resumed Activity and PID do not prove rendered menu/gameplay.",
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("reports", type=Path)
    args = parser.parse_args()
    result = evaluate(args.reports)
    (args.reports / "runtime-verdict.json").write_text(json.dumps(result, indent=2) + "\n")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["status"] == "PID_STABLE_35S_MENU_NOT_YET_REVIEWED" else 1


if __name__ == "__main__":
    raise SystemExit(main())
