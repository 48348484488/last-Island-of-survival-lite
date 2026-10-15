#!/usr/bin/env python3
"""Conservative monotonic 35s sampling gate for unchanged original Oxide.

Presence of a file named 'pid-35.txt' alone is not elapsed-time evidence.
The capture recorder must report monotonic timing and matching PID evidence.
This is not gameplay or cryptographic attestation of capture provenance.
"""
import json
import math
from pathlib import Path

REQUIRED = (0, 10, 35)


def verify_capture_timing(reports):
    root = Path(reports)

    def fail(reason, elapsed=None):
        return {"verified": False, "reason": reason, "elapsed_35_seconds": elapsed}

    p = root / "capture-timing.json"
    try:
        obj = json.loads(p.read_text(encoding="utf8"))
    except (OSError, UnicodeError, ValueError, TypeError):
        return fail("MISSING_OR_INVALID_CAPTURE_TIMING")
    if not isinstance(obj, dict) or obj.get("clock") != "monotonic":
        return fail("NON_MONOTONIC_CAPTURE_CLOCK")
    samples = obj.get("checkpoints")
    if not isinstance(samples, list) or not samples:
        return fail("INVALID_CAPTURE_CHECKPOINTS")

    seen = {}
    previous_target = -1
    previous_elapsed = -1.0
    previous_finished = -1.0
    for record in samples:
        if not isinstance(record, dict):
            return fail("INVALID_CAPTURE_CHECKPOINTS")
        target = record.get("target_seconds")
        if type(target) is not int or target < 0 or target <= previous_target:
            return fail("DUPLICATED_OR_UNORDERED_TARGET")
        elapsed = record.get("elapsed_seconds")
        finished = record.get("finished_seconds")
        if (type(elapsed) not in (int, float) or type(finished) not in (int, float)
                or not math.isfinite(elapsed) or not math.isfinite(finished)
                or elapsed < 0 or finished < elapsed
                or elapsed < previous_elapsed or finished < previous_finished
                or elapsed + 0.001 < target):
            return fail("INVALID_MONOTONIC_SAMPLE_TIMES")
        previous_target = target
        previous_elapsed = elapsed
        previous_finished = finished
        seen[target] = record

    if any(t not in seen for t in REQUIRED):
        return fail("MISSING_REQUIRED_CAPTURE_CHECKPOINT")

    for t in REQUIRED:
        record = seen[t]
        stored_pid_file = root / f"pid-{t}.txt"
        try:
            stored_pid = stored_pid_file.read_text(encoding="utf8").strip()
        except OSError:
            return fail("MISSING_CHECKPOINT_PID")
        if record.get("pid") != stored_pid:
            return fail("CAPTURE_PID_MISMATCH")
        if record.get("activity_dump_captured") is not True:
            return fail("ACTIVITY_DUMP_CAPTURE_NOT_CONFIRMED")

    elapsed_35 = seen[35]["elapsed_seconds"]
    if elapsed_35 < 35.0:
        return fail("EARLY_FINAL_CHECKPOINT", elapsed_35)

    return {"verified": True, "reason": "MONOTONIC_35S_CHECKPOINT_VERIFIED",
            "elapsed_35_seconds": elapsed_35}
