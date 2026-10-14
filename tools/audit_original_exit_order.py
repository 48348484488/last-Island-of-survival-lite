#!/usr/bin/env python3
"""Validate unchanged original Activity finish -> self-SIGKILL -> death.

Reads original logcat and Android ApplicationExitInfo archives only.
Does not identify the caller/reason for Activity.finish() or modify the game.
"""
import argparse
import json
import re
from pathlib import Path

from diagnose_original_exit import PACKAGE, ACTIVITY, diagnose, event_ms, log_lines

EXIT_BLOCK = re.compile(
    r"ApplicationExitInfo\s+#\d+:([\s\S]*?)(?=ApplicationExitInfo\s+#\d+:|\Z)"
)


def app_exit_entry(text, pid):
    matching = []
    for match in EXIT_BLOCK.finditer(text):
        entry = match.group(1)
        if (re.search(r"\bpid=" + str(pid) + r"\b", entry) and
                "process=" + PACKAGE in entry):
            matching.append(entry)
    return {
        "exact_original_pid_found": len(matching) == 1,
        "signal_9_recorded": len(matching) == 1 and bool(re.search(
            r"\breason=2\s*\(SIGNALED\).*?\bstatus=9\b", matching[0], re.S
        )),
        "matching_entries": len(matching),
    }


def analyze(directory, android_version):
    folder = Path(directory)
    original = diagnose(folder)
    pid = original["initial_activity_pid"]
    start = original["initial_activity_start"]
    if type(pid) is not int or pid <= 0 or not start:
        raise ValueError("Original Activity PID/start missing: refuse verification")
    rows = sorted(
        (r for r in log_lines(folder / "all_buffers.txt") if r["timestamp"] >= start),
        key=lambda r: r["timestamp"],
    )
    def first(tag, predicate):
        return next((r for r in rows if r["tag"] == tag and predicate(r)), None)
    def own_activity(r):
        return PACKAGE + "/" in r["raw"] and ACTIVITY in r["raw"]
    finish = first("wm_finish_activity", own_activity)
    pause = first("wm_pause_activity", lambda r: own_activity(r) and "finish" in r["raw"])
    signal = first("Process", lambda r: r["pid"] == pid and bool(re.search(
        r"Sending signal\. PID:\s*" + str(pid) + r"\s+SIG:\s*9\b", r["raw"]
    )))
    death = first("am_proc_died", lambda r: bool(re.search(
        r"\[\d+," + str(pid) + r"," + re.escape(PACKAGE) + r",", r["raw"]
    )))
    exit_path = folder / "exit_info.txt"
    if not exit_path.is_file():
        raise ValueError("ApplicationExitInfo absent: refuse verification")
    exit_info = app_exit_entry(exit_path.read_text(errors="replace"), pid)
    def relative(r):
        return event_ms(start, r["timestamp"]) if r else None
    milestones = {
        "original_activity_started_ms": 0,
        "activity_finish_requested_ms": relative(finish),
        "activity_paused_for_finish_ms": relative(pause),
        "original_self_sigkill_ms": relative(signal),
        "original_process_death_ms": relative(death),
    }
    ordered = (
        all((finish, signal, death)) and
        start <= finish["timestamp"] <= signal["timestamp"] <= death["timestamp"]
    )
    gap = (event_ms(finish["timestamp"], signal["timestamp"])
           if finish and signal else None)
    original_pid_java_fatal = sum(
        1 for r in rows if r["pid"] == pid and r["tag"] == "AndroidRuntime"
        and "FATAL EXCEPTION" in r["raw"]
        and (death is None or r["timestamp"] <= death["timestamp"])
    )
    original_pid_native_fatal = sum(
        1 for r in rows if r["pid"] == pid and r["tag"] == "libc"
        and re.search(r"Fatal signal\s+(?:6|11)\b", r["raw"])
        and (death is None or r["timestamp"] <= death["timestamp"])
    )
    status = (
        "OBSERVED_FINISH_THEN_SELF_SIGKILL_STATUS9"
        if ordered and exit_info["signal_9_recorded"]
        else "INCOMPLETE_OR_NONMATCHING_EXIT_EVIDENCE"
    )
    return {
        "android_version": str(android_version),
        "original_activity_pid": pid,
        "classification": status,
        "original_game_lifecycle": original["classification"],
        "events_ms_after_start": milestones,
        "finish_before_self_signal": bool(ordered),
        "finish_to_self_sigkill_ms": gap,
        "application_exit_info": exit_info,
        "original_pid_java_fatal_markers": original_pid_java_fatal,
        "original_pid_native_fatal_6_or_11_markers": original_pid_native_fatal,
        "activity_finish_requester": "NOT_IDENTIFIED",
        "security_warning_cause": "NOT_IDENTIFIED",
        "gameplay_verified": False,
        "original_apk_modified": False,
        "limitations": [
            "wm_finish_activity is a framework event; it does not prove who requested Activity.finish().",
            "No FATAL marker in captured logs cannot rule out all software faults.",
            "Self-directed SIGKILL and exit status 9 prove mechanism, not its motive.",
            "No game process hook, APK modifications, device spoofing, Play Integrity alteration or player data.",
        ],
    }


def compare(cases):
    passed = all(
        x["classification"] == "OBSERVED_FINISH_THEN_SELF_SIGKILL_STATUS9"
        for x in cases.values()
    )
    return {
        "status": ("THREE_REAL_NATIVE_ANDROID_VERSIONS_CONFIRMED"
                   if passed and set(cases) == {"13", "15", "16"} else "UNVERIFIED"),
        "versions": cases,
        "all_original_activities_finish_before_self_sigkill": bool(passed),
        "all_application_exit_info_records_signal_9": all(
            x["application_exit_info"]["signal_9_recorded"] for x in cases.values()
        ),
        "requester_identified": False,
        "root_cause_proven": False,
        "gameplay_proven": False,
    }


def markdown(report):
    lines = [
        "# Original Oxide native ARM64: independent Activity finish and process exit",
        "", "| Android | Activity PID | Activity finish (ms) | Self-SIGKILL (ms) | Gap (ms) | System exit |",
        "|---|---:|---:|---:|---:|---|",
    ]
    for version, result in report["versions"].items():
        milestones = result["events_ms_after_start"]
        lines.append(
            "| " + version + " | " + str(result["original_activity_pid"]) +
            " | " + str(milestones["activity_finish_requested_ms"]) +
            " | " + str(milestones["original_self_sigkill_ms"]) +
            " | " + str(result["finish_to_self_sigkill_ms"]) +
            " | " + ("SIGNALED 9" if result["application_exit_info"]["signal_9_recorded"]
                      else "NOT VERIFIED") + " |"
        )
    lines += [
        "", "**" + report["status"] + "**", "",
        "The Android system records a finish request before the original process sends SIGKILL to itself.",
        "The requesting component and reason are **not identified** by these observations.",
        "No causal inference about the game's security check, Play Integrity, or graphics failures.",
        "No playable game, multiplayer data, or real ESP was demonstrated.",
    ]
    return "\n".join(lines) + "\n"


def main():
    p = argparse.ArgumentParser(description=__doc__)
    for version in ("13", "15", "16"):
        p.add_argument("--android" + version, type=Path, required=True)
    p.add_argument("--out", type=Path, required=True)
    args = p.parse_args()
    cases = {v: analyze(getattr(args, "android" + v), v)
             for v in ("13", "15", "16")}
    report = compare(cases)
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "original_exit_order.json").write_text(
        json.dumps(report, indent=2) + "\n", encoding="utf-8"
    )
    (args.out / "original_exit_order.md").write_text(
        markdown(report), encoding="utf-8"
    )
    print(markdown(report))
    return 0 if report["status"] == "THREE_REAL_NATIVE_ANDROID_VERSIONS_CONFIRMED" else 1


if __name__ == "__main__":
    raise SystemExit(main())
