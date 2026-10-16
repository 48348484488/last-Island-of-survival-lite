#!/usr/bin/env python3
"""Deterministic goals for *unchanged* Oxide runtime evidence.

This program cannot infer rendered gameplay, service/Activity equivalence, or
why the original application terminates. Never auto-advance on CI success alone.
"""
import argparse
import json
import re
from datetime import datetime
from pathlib import Path

if __package__:
    from .oxide_capture_timing_gate import verify_capture_timing
    from .oxide_activity_binding import original_resumed_activity_pid
    from .oxide_runtime_environment import evaluate_runtime_environment
    from .oxide_exit_timeline import assess as assess_exit_timeline
else:
    from oxide_capture_timing_gate import verify_capture_timing
    from oxide_activity_binding import original_resumed_activity_pid
    from oxide_runtime_environment import evaluate_runtime_environment
    from oxide_exit_timeline import assess as assess_exit_timeline

GOAL_IDS = ("android_arm64_boot", "original_launch", "activity_35s", "menu_gameplay", "synthetic_overlay")
ORIGINAL = "com.catsbit.oxidesurvivalisland/.OxideActivity"


def read(root, name):
    p = Path(root) / name
    return p.read_text(encoding="utf-8", errors="replace").strip() if p.is_file() else ""


def resumed(activity):
    """A task mention in dumpsys is not a resumed Activity."""
    return any(
        ORIGINAL in line and ("topResumedActivity=" in line or "ResumedActivity:" in line
                              or "mResumedActivity=" in line)
        for line in activity.splitlines()
    )


def pid_value(value):
    return value.strip() if re.fullmatch(r"[1-9][0-9]*", value.strip()) else ""


def first_resumed_original(root):
    """Find first original resumed Activity and its PID in the early exit window.

    At t=0 the launch request may not yet have created the process. A late
    background analytics PID, Activity history or mere package reference is
    not acceptable. This is only an early lifecycle baseline.
    """
    for t in (0, 1, 2, 3, 4):
        pid = pid_value(read(root, f"pid-{t}.txt"))
        owner_pid = original_resumed_activity_pid(read(root, f"activity-{t}.txt"))
        if pid and owner_pid == pid:
            return {"checkpoint": t, "pid": pid}
    return None


def evidence_summary(root):
    root = Path(root)
    runtime_environment = evaluate_runtime_environment(root)
    boot = runtime_environment["original_arm64_runtime_eligible"]
    installed = boot and read(root, "install.txt").startswith("Success")
    launch_log = read(root, "launch.txt")
    pids = {str(t): pid_value(read(root, f"pid-{t}.txt")) for t in (0, 10, 35)}
    activity = {str(t): read(root, f"activity-{t}.txt") for t in (0, 10, 35)}
    initial = first_resumed_original(root)
    launched = (installed and "Status: ok" in launch_log and ORIGINAL in launch_log
                and initial is not None)
    stable_pid = launched and initial["pid"] == pids["10"] == pids["35"]
    timing = verify_capture_timing(root)
    logcat = read(root, "logcat.txt")
    native_exit_diagnostic = assess_exit_timeline(logcat)
    native_evidence = native_exit_diagnostic["evidence"]
    # Cross-check the original Activity PID against independent logcat evidence.
    # An explicit fatal signal before 35 seconds overrides contradictory PID
    # snapshots, even when those snapshots and their timestamps appear valid.
    exit_at_ms = [
        native_evidence[key] for key in ("start_to_self_sigkill_ms", "start_to_sigill_ms")
        if isinstance(native_evidence.get(key), (int, float))
    ]
    original_pid_in_logcat = (
        initial is not None
        and native_evidence["original_pid"] is not None
        and str(native_evidence["original_pid"]) == initial["pid"]
    )
    observed_fatal = any(native_evidence.get(key) is True for key in (
        "original_self_sigkill_logcat", "original_fatal_sigill_observed",
        "original_pid_died", "original_zygote_signal4_exit_observed"
    ))
    fatal_original_exit_conflicts_with_35s = bool(
        original_pid_in_logcat and observed_fatal
        and (not exit_at_ms or min(exit_at_ms) <= 35_000)
    )
    resumed_35s = (stable_pid and initial is not None
                   and original_resumed_activity_pid(activity["10"]) == initial["pid"]
                   and original_resumed_activity_pid(activity["35"]) == initial["pid"]
                   and timing["verified"]
                   and not fatal_original_exit_conflicts_with_35s)
    screenshots = {}
    for t in (0, 10, 35):
        path = root / f"screen-{t}.png"
        if path.is_file() and path.stat().st_size > 100:
            with path.open("rb") as image:
                screenshots[str(t)] = image.read(8) == b"\x89PNG\r\n\x1a\n"
        else:
            screenshots[str(t)] = False
    original_process_started = (installed and "Status: ok" in launch_log
                                and ORIGINAL in launch_log
                                and native_exit_diagnostic["evidence"]["original_pid"] is not None)
    android_version = read(root, "android-version.txt")
    if not android_version:
        observed = re.search(r"Android OS (1[3-6])", logcat)
        android_version = observed.group(1) if observed else "UNKNOWN"
    sigkill = False
    time_start = None
    time_kill = None
    # Async launches can legitimately have no PID at t=0. Correlate the
    # self-signal to the first *resumed original Activity* PID, never to
    # a later background analytics service PID.
    original_pid = initial["pid"] if initial else pids["0"]
    if original_pid:
        signal_regex = re.compile(r"Sending signal\. PID:\s*" + re.escape(original_pid) + r"\s+SIG:\s*9")
        for line in logcat.splitlines():
            if time_start is None and "ActivityTaskManager: START u0" in line and ORIGINAL in line:
                time_start = line.split(maxsplit=2)[1] if len(line.split()) > 1 else None
            if signal_regex.search(line):
                sigkill = True
                time_kill = line.split(maxsplit=2)[1] if len(line.split()) > 1 else None
                break
    runtime_seconds = None
    if time_start and time_kill:
        try:
            x = datetime.strptime(time_start, "%H:%M:%S.%f")
            y = datetime.strptime(time_kill, "%H:%M:%S.%f")
            runtime_seconds = round((y-x).total_seconds() % 86400, 3)
        except ValueError:
            pass
    goal_status = [
        "complete" if boot else "blocked",
        "complete" if launched else "blocked" if boot else "pending",
        "complete" if resumed_35s else "blocked" if launched else "pending",
        "pending_visual_review" if resumed_35s and screenshots["10"] and screenshots["35"] else "pending",
        "pending",
    ]
    stages = [dict(id=k, status=v) for k,v in zip(GOAL_IDS,goal_status)]
    if not boot:
        next_action = "HOST_PREPARE"
    elif not installed:
        next_action = "CHECK_APK_INSTALL"
    elif not launched and native_exit_diagnostic["evidence"]["original_pid_died"]:
        next_action = "FORENSIC_APP_EXIT"
    elif not launched:
        next_action = "CHECK_ORIGINAL_ACTIVITY_LAUNCH"
    elif not resumed_35s:
        next_action = "FORENSIC_APP_EXIT"
    else:
        next_action = "REVIEW_REAL_MENU"
    return {
        "schema_version": 1,
        "android_version": android_version,
        "goals": stages,
        "next_action": next_action,
        "checks": {
            "virtual_arm64_booted": boot,
            "android_arm64_native": runtime_environment["android_arm64_native"],
            "arm64_jni_translation_proven": runtime_environment["arm64_jni_translation_proven_in_predecessor"],
            "runtime_environment": runtime_environment,
            "original_process_started_from_logcat": bool(original_process_started),
            "native_exit_diagnostic": native_exit_diagnostic,
            "original_split_install_confirmed": installed,
            "original_activity_started": launched,
            "first_original_resumed_checkpoint": initial["checkpoint"] if initial else None,
            "first_original_resumed_pid": initial["pid"] if initial else None,
            "original_activity_resumed_at_10_and_35": bool(resumed_35s),
            "fatal_original_exit_conflicts_with_35s": fatal_original_exit_conflicts_with_35s,
            "fully_drawn_is_not_gameplay_evidence": native_evidence[
                "original_activity_fully_drawn"],
            "activity_owners": {
                str(t): original_resumed_activity_pid(read(root, f"activity-{t}.txt"))
                for t in (0, 1, 2, 3, 4, 10, 35)
            },
            "capture_timing": timing,
            "pids": pids,
            "screenshots_png_available": screenshots,
            "initial_activity_self_sigkill_observed": sigkill,
            "start_to_sigkill_seconds": runtime_seconds,
        },
        "gameplay_confirmed": False,
        "synthetic_overlay_confirmed": False,
        "security_cause": "UNKNOWN",
        "explanation": "Independent fatal logcat evidence overrides contradictory 10s/35s PID snapshots. A later same-package PID may be an analytics service. Fully-drawn Activity and screenshots alone do not prove gameplay.",
    }


def to_markdown(result, catalog):
    assert [g["id"] for g in catalog["goals"]] == list(GOAL_IDS)
    lines = ["# Oxide AutoDev — metas verificaveis", "", "Objetivo: " + catalog["objective"], "",
             "| Meta | Estado | Criterio |", "|---|---|---|"]
    for item, status in zip(catalog["goals"],result["goals"]):
        lines.append("| " + item["title"] + " | " + status["status"] + " | " + item["criteria"] + " |")
    lines += ["", "Proxima acao: **" + result["next_action"] + "**",
              "", "PID inicial e checkpoints: " + json.dumps(result["checks"]["pids"]),
              "", "SIGKILL da Activity inicial: " + str(result["checks"]["initial_activity_self_sigkill_observed"]),
              "", "**Gameplay e ESP sintetico ainda nao confirmados.**",
              "", "Sem alterar o APK e sem contornar controles de integridade.\n"]
    return "\n".join(lines)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("reports", type=Path)
    ap.add_argument("--catalog", type=Path, default=Path("goals/oxide-goals.json"))
    ap.add_argument("--output", type=Path, default=Path("reports/goal-evidence"))
    args = ap.parse_args()
    result = evidence_summary(args.reports)
    catalog = json.loads(args.catalog.read_text(encoding="utf8"))
    args.output.mkdir(parents=True, exist_ok=True)
    (args.output / "goals.json").write_text(json.dumps(result, indent=2, ensure_ascii=False) + "\n", encoding="utf8")
    (args.output / "goals.md").write_text(to_markdown(result, catalog), encoding="utf8")
    print(json.dumps({"next_action":result["next_action"],"goals":result["goals"],"sigkill":result["checks"]["initial_activity_self_sigkill_observed"]}))


if __name__ == "__main__":
    main()
