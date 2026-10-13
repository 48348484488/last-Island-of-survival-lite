#!/usr/bin/env python3
"""Check survival of the *initial* Oxide Activity, not a package analytics service.

This is a conservative Android smoke criterion, NOT gameplay or ESP validation.
Uses observational, read-only artifacts created by the authorized ADB runner.
"""
import argparse
import json
import re
from pathlib import Path

PACKAGE = "com.catsbit.oxidesurvivalisland"


def pid_sample(path):
    if not path.is_file():
        return set()
    return {int(x) for x in re.findall(r"\b\d+\b", path.read_text(errors="replace"))}


def activity_resumed(path):
    if not path.is_file():
        return False
    for line in path.read_text(errors="replace").splitlines():
        if re.search(r"\b(?:topResumedActivity|mResumedActivity)\s*[:=]", line) \
           and PACKAGE + "/" in line and "OxideActivity" in line:
            return True
    return False


def evaluate(directory):
    folder = Path(directory)
    try:
        evidence = json.loads((folder / "forensic_summary.json").read_text(encoding="utf-8"))
    except (OSError, ValueError):
        evidence = {}
    pid = evidence.get("initial_activity_pid")
    if not isinstance(pid, int) or isinstance(pid, bool) or pid <= 0:
        pid = None
    alive10 = pid is not None and pid in pid_sample(folder / "pid_10s.txt")
    alive35 = pid is not None and pid in pid_sample(folder / "pid_35s.txt")
    resumed10 = activity_resumed(folder / "activities_10s.txt")
    resumed35 = activity_resumed(folder / "activities_35s.txt")
    ended = bool(evidence.get("first_self_sigkill") or evidence.get("death_event"))
    if pid is None:
        status = "INSUFFICIENT_INITIAL_ACTIVITY_PID"
    elif ended:
        status = "INITIAL_ACTIVITY_TERMINATED"
    elif not alive10 or not alive35:
        status = "INITIAL_ACTIVITY_PID_NOT_ALIVE_AT_CHECKPOINTS"
    elif not resumed10 or not resumed35:
        status = "ORIGINAL_ACTIVITY_NOT_RESUMED_AT_CHECKPOINTS"
    else:
        status = "ACTIVITY_SMOKE_PASS_NOT_GAMEPLAY"
    return {
        "status": status,
        "initial_activity_pid": pid,
        "pid_present_at_10s": bool(alive10),
        "pid_present_at_35s": bool(alive35),
        "oxide_activity_resumed_at_10s": bool(resumed10),
        "oxide_activity_resumed_at_35s": bool(resumed35),
        "initial_activity_exit_observed": ended,
        "gameplay_verified": False,
        "esp_verified": False,
        "scope": "Activity survival only; not a multiplayer/gameplay test",
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("artifact_dir", type=Path)
    args = parser.parse_args()
    result = evaluate(args.artifact_dir)
    (args.artifact_dir / "activity_verdict.json").write_text(
        json.dumps(result, indent=2) + "\n", encoding="utf-8"
    )
    print(json.dumps(result, indent=2))
    return 0 if result["status"] == "ACTIVITY_SMOKE_PASS_NOT_GAMEPLAY" else 1


if __name__ == "__main__":
    raise SystemExit(main())
