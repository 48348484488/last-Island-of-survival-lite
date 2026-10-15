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


def evidence_summary(root):
    root = Path(root)
    boot = read(root, "boot.txt") == "1" and read(root, "abi.txt") == "arm64-v8a"
    installed = boot and read(root, "install.txt").startswith("Success")
    launch_log = read(root, "launch.txt")
    pids = {str(t): pid_value(read(root, f"pid-{t}.txt")) for t in (0, 10, 35)}
    activity = {str(t): read(root, f"activity-{t}.txt") for t in (0, 10, 35)}
    launched = (installed and "Status: ok" in launch_log and ORIGINAL in launch_log
                and bool(pids["0"]) and resumed(activity["0"]))
    stable_pid = launched and pids["0"] == pids["10"] == pids["35"]
    resumed_35s = stable_pid and resumed(activity["10"]) and resumed(activity["35"])
    screenshots = {}
    for t in (0, 10, 35):
        path = root / f"screen-{t}.png"
        if path.is_file() and path.stat().st_size > 100:
            with path.open("rb") as image:
                screenshots[str(t)] = image.read(8) == b"\x89PNG\r\n\x1a\n"
        else:
            screenshots[str(t)] = False
    logcat = read(root, "logcat.txt")
    sigkill = False
    time_start = None
    time_kill = None
    if pids["0"]:
        signal_regex = re.compile(r"Sending signal\. PID:\s*" + re.escape(pids["0"]) + r"\s+SIG:\s*9")
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
    elif not launched:
        next_action = "CHECK_ORIGINAL_ACTIVITY_LAUNCH"
    elif not resumed_35s:
        next_action = "FORENSIC_APP_EXIT"
    else:
        next_action = "REVIEW_REAL_MENU"
    return {
        "schema_version": 1,
        "goals": stages,
        "next_action": next_action,
        "checks": {
            "virtual_arm64_booted": boot,
            "original_split_install_confirmed": installed,
            "original_activity_started": launched,
            "original_activity_resumed_at_10_and_35": bool(resumed_35s),
            "pids": pids,
            "screenshots_png_available": screenshots,
            "initial_activity_self_sigkill_observed": sigkill,
            "start_to_sigkill_seconds": runtime_seconds,
        },
        "gameplay_confirmed": False,
        "synthetic_overlay_confirmed": False,
        "security_cause": "UNKNOWN",
        "explanation": "A later same-package PID may be an analytics service; original resumed Activity is also required. Screenshots alone do not prove gameplay.",
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
