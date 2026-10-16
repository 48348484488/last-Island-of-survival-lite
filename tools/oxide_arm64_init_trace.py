#!/usr/bin/env python3
"""Read-only original Redroid ARM64 lifecycle milestones from existing logcat.

Observed library-lookup proximity is NOT evidence of a crash cause.
No APK modification, execution, or protection analysis.
"""
import argparse
from datetime import datetime
import hashlib
import json
import re
from pathlib import Path

PACKAGE = "com.catsbit.oxidesurvivalisland"
ACTIVITY = PACKAGE + "/.OxideActivity"
STAMP = re.compile(
    r"^(?P<date>\d\d-\d\d) (?P<time>\d\d:\d\d:\d\d\.\d{3,6})\s+"
    r"(?P<pid>\d+)\s+(?P<tid>\d+)\s+\w\s+(?P<tag>[^:]+):\s?(?P<msg>.*)$")
PROC = re.compile(r"\bStart proc (?P<pid>\d+):" + re.escape(PACKAGE) + r"/")
SELF_KILL = re.compile(r"\bSending signal\. PID: (?P<pid>\d+) SIG: 9(?:\D|$)")
CLOSE = re.compile(r"\bcreatedAt=(?P<date>\d\d-\d\d) (?P<time>\d\d:\d\d:\d\d\.\d+)")
OPTIONAL_BURST = (
    ("burst_lookup_liblib", "liblib_burst_0_0.so"),
    ("burst_lookup_lib", "lib_burst_0_0.so"),
)


def stamp(date, time):
    return datetime.strptime(f"2026-{date} {time}", "%Y-%m-%d %H:%M:%S.%f")


def ms(before, after):
    if before is None or after is None:
        return None
    return round((after - before).total_seconds() * 1000, 3)


def inspect_native_logcat(data):
    text = data.decode("utf-8", errors="replace")
    events = {}
    launch = original_pid = None
    for line in text.splitlines():
        m = STAMP.match(line)
        if m is None:
            continue
        msg, at = m["msg"], stamp(m["date"], m["time"])
        source_pid = int(m["pid"])
        if launch is None and "START u0" in msg and f"cmp={ACTIVITY}" in msg:
            launch = at
            events["activity_start"] = at
        if launch is None:
            continue
        proc = PROC.search(msg)
        if original_pid is None and proc is not None and at >= launch:
            original_pid = int(proc["pid"])
            events["original_pid_started"] = at
        if original_pid is None:
            continue
        if source_pid == original_pid:
            if m["tag"].strip() == "Unity":
                if "Built from" in msg and "Scripting Backend" in msg:
                    events.setdefault("unity_il2cpp_build_record", at)
                if "Product Name: OXIDE" in msg:
                    events.setdefault("unity_product_name", at)
            if ("nativeloader" in m["tag"] and "lib_burst_generated.so" in msg
                    and msg.endswith(": ok")):
                events.setdefault("burst_generated_loaded", at)
            if "nativeloader" in m["tag"] and "dlopen failed:" in msg:
                for key, name in OPTIONAL_BURST:
                    if f"Load {name} using" in msg and f'library "{name}" not found' in msg:
                        events.setdefault(key, at)
            kill = SELF_KILL.search(msg)
            if kill and int(kill["pid"]) == original_pid and int(m["tid"]) == original_pid:
                events.setdefault("self_sigkill", at)
        # CLOSE 'createdAt' can predate its appearance in the logcat dump.
        if "Sent Transition" in msg and "type = CLOSE" in msg and ACTIVITY in msg:
            recorded = CLOSE.search(msg)
            if recorded:
                events.setdefault("task_close_created",
                                  stamp(recorded["date"], recorded["time"]))

    expected = ("activity_start", "original_pid_started",
                "unity_il2cpp_build_record", "unity_product_name",
                "burst_generated_loaded", "burst_lookup_liblib",
                "burst_lookup_lib", "task_close_created", "self_sigkill")
    complete = all(key in events for key in expected)
    ordered = complete and (
        events["activity_start"] <= events["original_pid_started"]
        <= events["unity_il2cpp_build_record"]
        <= events["unity_product_name"] <= events["burst_generated_loaded"]
        <= events["burst_lookup_liblib"] <= events["burst_lookup_lib"]
        <= events["task_close_created"] <= events["self_sigkill"]
    )
    return {
        "status": ("NATIVE_UNITY_BURST_LOOKUPS_BEFORE_EXIT_OBSERVED"
                   if ordered else "INCOMPLETE_NATIVE_INIT_MILESTONE_EVIDENCE"),
        "all_required_milestones_in_order": bool(ordered),
        "original_pid": original_pid,
        "raw_logcat_sha256": hashlib.sha256(data).hexdigest(),
        "observations": {
            key: {
                "timestamp": value.strftime("%m-%d %H:%M:%S.%f")[:-3],
                "elapsed_from_activity_start_ms": ms(launch, value),
            }
            for key, value in sorted(events.items(), key=lambda item: item[1])
        },
        "burst_first_lookup_to_self_sigkill_ms": ms(
            events.get("burst_lookup_liblib"), events.get("self_sigkill")),
        "burst_last_lookup_to_self_sigkill_ms": ms(
            events.get("burst_lookup_lib"), events.get("self_sigkill")),
        "task_close_to_self_sigkill_ms": ms(
            events.get("task_close_created"), events.get("self_sigkill")),
        "optional_burst_lookup_missing_library_is_crash_cause": "NOT_ESTABLISHED",
        "native_security_trigger": "UNKNOWN",
        "original_apks_modified": False,
        "original_gameplay_verified": False,
    }


def compare_native_logs(android15, android16):
    runs = {
        "android15": inspect_native_logcat(android15),
        "android16": inspect_native_logcat(android16),
    }
    expected_pids = {"android15": 2023, "android16": 2108}
    verified = all(
        runs[key]["all_required_milestones_in_order"]
        and runs[key]["original_pid"] == expected_pids[key]
        for key in runs
    )
    return {
        "status": ("SHARED_OPTIONAL_BURST_LOOKUP_BEFORE_EXIT_OBSERVED"
                   if verified else "NATIVE_INIT_COMPARISON_INCOMPLETE"),
        "replay_verified": bool(verified),
        "executions": runs,
        "causal_link_from_burst_lookup_to_exit": "NOT_ESTABLISHED",
        "gameplay_verified": False,
        "native_security_trigger": "UNKNOWN",
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--android15-logcat", type=Path, required=True)
    parser.add_argument("--android16-logcat", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = compare_native_logs(
        args.android15_logcat.read_bytes(), args.android16_logcat.read_bytes())
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["replay_verified"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
