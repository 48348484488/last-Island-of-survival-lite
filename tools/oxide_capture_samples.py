#!/usr/bin/env python3
"""Read-only early/late Android ARM64 evidence: no APK edits or security bypass."""
import argparse
import json
import subprocess
import time
from pathlib import Path

CHECKPOINTS = (0, 1, 2, 3, 4, 10, 35)
PNG_SIGNATURE = b"\x89PNG\r\n\x1a\n"

def _run(command, timeout):
    try:
        return subprocess.run(command, capture_output=True, timeout=timeout, check=False)
    except subprocess.TimeoutExpired:
        return None

def capture(reports, package, serial="127.0.0.1:5555", checkpoints=CHECKPOINTS,
            runner=_run, clock=time.monotonic, sleeper=time.sleep):
    """Sample relative to start; record actual elapsed time after ADB delays."""
    reports = Path(reports)
    reports.mkdir(parents=True, exist_ok=True)
    start = clock()
    records = []
    adb = ["adb", "-s", serial]
    for target in checkpoints:
        if target < 0:
            raise ValueError("checkpoint must be nonnegative")
        sleeper(max(0, start + target - clock()))
        elapsed = round(clock() - start, 3)
        record = {"target_seconds": target, "elapsed_seconds": elapsed,
                  "pid": "", "png_bytes": 0, "valid_png": False,
                  "activity_dump_captured": False, "errors": []}
        pid = runner(adb + ["shell", "pidof", package], 4)
        if pid is None:
            record["errors"].append("pid_timeout")
        else:
            raw_pid = pid.stdout.decode("utf-8", errors="replace").strip()
            (reports / f"pid-{target}.txt").write_text(raw_pid + "\n", encoding="utf8")
            record["pid"] = raw_pid
            if pid.returncode:
                record["errors"].append("pid_unavailable")
        shot = runner(adb + ["exec-out", "screencap", "-p"], 6)
        if shot is None:
            record["errors"].append("screencap_timeout")
        elif shot.returncode or not shot.stdout.startswith(PNG_SIGNATURE):
            record["errors"].append("invalid_screenshot")
        else:
            (reports / f"screen-{target}.png").write_bytes(shot.stdout)
            record["png_bytes"] = len(shot.stdout)
            record["valid_png"] = True
        if target in (0, 10, 35):
            dump = runner(adb + ["shell", "dumpsys", "activity", "activities"], 6)
            if dump is None:
                record["errors"].append("activity_timeout")
            else:
                (reports / f"activity-{target}.txt").write_bytes(dump.stdout)
                record["activity_dump_captured"] = dump.returncode == 0
                if dump.returncode:
                    record["errors"].append("activity_dump_failed")
        record["finished_seconds"] = round(clock() - start, 3)
        records.append(record)
    (reports / "capture-timing.json").write_text(
        json.dumps({"clock": "monotonic", "checkpoints": records}, indent=2) + "\n", encoding="utf8")
    return records

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--reports", type=Path, default=Path("reports"))
    parser.add_argument("--package", required=True)
    parser.add_argument("--serial", default="127.0.0.1:5555")
    args = parser.parse_args()
    samples = capture(args.reports, args.package, serial=args.serial)
    print(json.dumps({"samples": len(samples), "valid_png": sum(x["valid_png"] for x in samples),
                      "last_elapsed": samples[-1]["elapsed_seconds"]}))

if __name__ == "__main__":
    main()
