#!/usr/bin/env python3
"""Read-only same-original-PID Android native loader naming/fallback timeline.

A failed dlopen can precede a successful alternate-name load. Neither a
fallback nor a failed dynamic lookup establishes a later app exit cause.
"""
import argparse
from datetime import datetime
import hashlib
import json
import re
from pathlib import Path

PKG = "com.catsbit.oxidesurvivalisland"
ACTIVITY = PKG + "/.OxideActivity"
STAMP = re.compile(
    r"^(?P<md>\d\d-\d\d) (?P<time>\d\d:\d\d:\d\d\.\d+)\s+"
    r"(?P<pid>\d+)\s+(?P<tid>\d+)\s+\w\s+(?P<tag>[^:]+):\s?(?P<msg>.*)$")
PROCID = re.compile(r"\bStart proc (?P<pid>\d+):" + re.escape(PKG) + r"/")
SIGKILL = re.compile(r"\bSending signal\. PID: (?P<pid>\d+) SIG: 9(?:\D|$)")
NATIVE_LOAD = re.compile(r"^Load (?P<item>\S+) using .*: (?P<status>ok|dlopen failed: .+)$")
TARGETS = (
    "liblibAudioPluginDissonance.so", "libAudioPluginDissonance.so",
    "lib_burst_generated.so", "liblib_burst_0_0.so", "lib_burst_0_0.so",
)


def _ms(a, b):
    return round((b - a).total_seconds() * 1000, 3)


def _stamp(md, clock):
    return datetime.strptime(f"2026-{md} {clock}", "%Y-%m-%d %H:%M:%S.%f")


def analyze_logcat(blob, expected_pid):
    rows = []
    pid_start = None
    activity_start = None
    for line in blob.decode("utf8", errors="replace").splitlines():
        m = STAMP.match(line)
        if not m:
            continue
        msg = m["msg"]
        at = _stamp(m["md"], m["time"])
        started = PROCID.search(msg)
        if "START u0" in msg and f"cmp={ACTIVITY}" in msg:
            activity_start = activity_start or at
        if started and activity_start is not None and pid_start is None:
            pid_start = int(started["pid"])
        if int(m["pid"]) != expected_pid:
            continue
        tid = int(m["tid"])
        if m["tag"].strip() == "nativeloader":
            load = NATIVE_LOAD.search(msg)
            if load:
                name = load["item"].rsplit("/", 1)[-1]
                if name in TARGETS:
                    rows.append({
                        "name": name,
                        "status": "loaded" if load["status"] == "ok" else "failed",
                        "tid": tid,
                        "when": at,
                    })
        sig = SIGKILL.search(msg)
        if sig and int(sig["pid"]) == expected_pid and tid == expected_pid:
            rows.append({"name": "self_sigkill", "status": "signal", "tid": tid, "when": at})

    def matched(name, status):
        return [r for r in rows if r["name"] == name and r["status"] == status]

    audio_fail = matched("liblibAudioPluginDissonance.so", "failed")
    audio_ok = matched("libAudioPluginDissonance.so", "loaded")
    burst_generated = matched("lib_burst_generated.so", "loaded")
    burst_fail_1 = matched("liblib_burst_0_0.so", "failed")
    burst_fail_2 = matched("lib_burst_0_0.so", "failed")
    signal = matched("self_sigkill", "signal")

    def ordered_pair(failures, successes, within_ms):
        return next(((f, s) for f in failures for s in successes
                     if f["tid"] == s["tid"] and 0 <= _ms(f["when"], s["when"]) <= within_ms),
                    None)

    audio_pair = ordered_pair(audio_fail, audio_ok, 20)
    burst_pair = ordered_pair(burst_fail_1, burst_fail_2, 20)
    burst_ok = matched("liblib_burst_0_0.so", "loaded") + matched("lib_burst_0_0.so", "loaded")
    valid_sequence = bool(
        pid_start == expected_pid and activity_start is not None and
        audio_pair and burst_pair and burst_generated and signal and
        audio_pair[1]["when"] < burst_generated[0]["when"] <= burst_pair[0]["when"] <=
        burst_pair[1]["when"] < signal[0]["when"] and not burst_ok
    )
    return {
        "status": ("ORIGINAL_AUDIO_FALLBACK_AND_BURST_LOOKUPS_OBSERVED"
                   if valid_sequence else "INCOMPLETE_ORIGINAL_NATIVE_LOADER_SEQUENCE"),
        "original_pid": pid_start,
        "logcat_sha256": hashlib.sha256(blob).hexdigest(),
        "alternate_audio_name_load_succeeded": audio_pair is not None,
        "alternate_audio_name_gap_ms": _ms(audio_pair[0]["when"], audio_pair[1]["when"])
        if audio_pair else None,
        "burst_generated_loaded": bool(burst_generated),
        "burst_names_failed_in_observed_window": bool(burst_pair and not burst_ok),
        "burst_name_failure_gap_ms": _ms(burst_pair[0]["when"], burst_pair[1]["when"])
        if burst_pair else None,
        "last_burst_failed_load_to_self_sigkill_ms": _ms(
            burst_pair[1]["when"], signal[0]["when"]) if burst_pair and signal else None,
        "same_thread_audio_pair": bool(audio_pair),
        "same_thread_burst_pair": bool(burst_pair),
        "verified_same_pid_sequence": valid_sequence,
        "burst_dependency_required": "NOT_ESTABLISHED",
        "library_failure_caused_exit": "NOT_ESTABLISHED",
        "security_trigger": "UNKNOWN",
        "gameplay_verified": False,
    }


def compare_logs(android15, android16):
    reports = {
        "android15": analyze_logcat(android15, 2023),
        "android16": analyze_logcat(android16, 2108),
    }
    complete = all(v["verified_same_pid_sequence"] for v in reports.values())
    return {
        "status": ("TWO_REAL_NATIVE_AUDIO_FALLBACKS_VERSUS_BURST_FAILED_LOOKUPS"
                   if complete else "NATIVE_LOADER_FALLBACK_COMPARISON_INCOMPLETE"),
        "evidence_validated": complete,
        "original_runs": reports,
        "no_causal_security_attribution": True,
        "original_gameplay_verified": False,
    }


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--android15", type=Path, required=True)
    p.add_argument("--android16", type=Path, required=True)
    p.add_argument("--output", type=Path, required=True)
    a = p.parse_args()
    result = compare_logs(a.android15.read_bytes(), a.android16.read_bytes())
    a.output.parent.mkdir(parents=True, exist_ok=True)
    a.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["evidence_validated"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
