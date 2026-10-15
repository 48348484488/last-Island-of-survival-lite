#!/usr/bin/env python3
"""Replay historical Android ARM64 Activity/PID evidence without APK or emulator."""
import argparse
import json
import re
from pathlib import Path

if __package__:
    from .oxide_activity_binding import original_resumed_activity_pid
    from .oxide_capture_timing_gate import verify_capture_timing
else:
    from oxide_activity_binding import original_resumed_activity_pid
    from oxide_capture_timing_gate import verify_capture_timing

CHECKPOINTS = (0, 1, 2, 3, 4, 10, 35)


def audit(reports):
    root = Path(reports)
    observations = []
    first_pid = first_checkpoint = None
    for t in CHECKPOINTS:
        f_pid = root / f"pid-{t}.txt"
        f_activity = root / f"activity-{t}.txt"
        raw_pid = f_pid.read_text(errors="replace").strip() if f_pid.is_file() else ""
        pid = raw_pid if re.fullmatch(r"[1-9]\d*", raw_pid) else None
        owner = (original_resumed_activity_pid(f_activity.read_text(errors="replace"))
                 if f_activity.is_file() else None)
        owned = bool(pid and owner == pid)
        if first_pid is None and owned and t <= 4:
            first_pid, first_checkpoint = pid, t
        observations.append({
            "checkpoint": t,
            "pidof_package": pid,
            "resumed_original_activity_owner_pid": owner,
            "pid_matches_original_activity_owner": owned,
            "activity_dump_available": f_activity.is_file(),
        })
    last = {row["checkpoint"]: row for row in observations}
    timing = verify_capture_timing(root)
    stable = (first_pid is not None and timing["verified"]
              and all(last[t]["pid_matches_original_activity_owner"]
                      and last[t]["resumed_original_activity_owner_pid"] == first_pid
                      for t in (10, 35)))
    return {
        "schema_version": 1,
        "status": ("ORIGINAL_ACTIVITY_STABLE_35S_GAMEPLAY_UNREVIEWED"
                   if stable else "ORIGINAL_ACTIVITY_EXIT_OR_UNVERIFIED_35S"),
        "first_original_activity_pid": first_pid,
        "first_original_resumed_checkpoint": first_checkpoint,
        "same_original_activity_alive_35s": bool(stable),
        "timing": timing,
        "checkpoints": observations,
        "gameplay_verified": False,
        "security_trigger": "UNKNOWN",
        "note": "Saved process ownership evidence does not establish the security trigger or gameplay.",
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("reports", type=Path)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    result = audit(args.reports)
    text = json.dumps(result, sort_keys=True, indent=2) + "\n"
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(text, encoding="utf8")
    print(text, end="")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
