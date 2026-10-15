#!/usr/bin/env python3
"""Conservative, read-only evaluation of original Oxide runtime evidence."""
import argparse
import json
from pathlib import Path


def _value(directory, name):
    p = directory / name
    return p.read_text(errors="replace").strip() if p.is_file() else ""


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
    stable = bool(pids[0]) and all(pid == pids[0] for pid in pids)
    if not stable:
        return {"status": "FAILED_PID_STABILITY_35S", "pids": pids, "gameplay_confirmed": False}
    activities = [_value(d, f"activity-{t}.txt") for t in (10, 35)]
    screenshots = [d / f"screen-{t}.png" for t in (10, 35)]
    pngs = all(p.is_file() and p.open("rb").read(8) == b"\x89PNG\r\n\x1a\n" for p in screenshots)
    if not activities[0] or not activities[1] or not pngs:
        return {"status": "PID_STABLE_MISSING_VISUAL_EVIDENCE", "pids": pids, "gameplay_confirmed": False}
    return {
        "status": "PID_STABLE_35S_MENU_NOT_YET_REVIEWED",
        "pids": pids,
        "screenshots_available": True,
        "gameplay_confirmed": False,
        "note": "PID stability is not proof of actual rendered menu or playable game.",
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
