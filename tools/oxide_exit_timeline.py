#!/usr/bin/env python3
"""Read-only lifecycle audit for unchanged Oxide Android ARM64 logs.

Does not infer the security trigger, application caller, Play Integrity verdict,
or gameplay from Android framework evidence.
"""
import argparse
from datetime import datetime
import json
import re
from pathlib import Path

PACKAGE = "com.catsbit.oxidesurvivalisland"
ACTIVITY = PACKAGE + "/.OxideActivity"
STAMP = re.compile(r"^(?P<md>\d\d-\d\d) (?P<hms>\d\d:\d\d:\d\d\.\d+)\s+(?P<pid>\d+)\s+(?P<tid>\d+)\s+\w\s+[^:]+: (?P<msg>.*)$")
START_PID = re.compile(r"Start proc (?P<pid>\d+):" + re.escape(PACKAGE) + r"/")
SELF_SIGNAL = re.compile(r"Sending signal\. PID: (?P<pid>\d+) SIG: 9(?:\D|$)")
DEATH = re.compile(r"Process " + re.escape(PACKAGE) + r" \(pid (?P<pid>\d+)\) has died:")
CLOSE_AT = re.compile(r"createdAt=(?P<md>\d\d-\d\d) (?P<hms>\d\d:\d\d:\d\d\.\d+)")
TASK_ID = re.compile(r"\btaskId=(\d+)")

def _datetime(md, hms, year):
    return datetime.strptime(f"{year}-{md} {hms}", "%Y-%m-%d %H:%M:%S.%f")

def _diff_ms(a, b):
    delta = (b-a).total_seconds() * 1000
    if -86_400_000 < delta < 0:
        delta += 86_400_000
    return round(delta, 3)

def assess(logcat, year=2026):
    if not 2000 <= year <= 2100:
        raise ValueError("invalid year")
    started = pid = fully_drawn = signaled = dead = None
    closes = []
    play_store_warning_before_exit = False
    for line in logcat.splitlines():
        match = STAMP.match(line)
        if not match:
            continue
        msg = match["msg"]
        timestamp = _datetime(match["md"], match["hms"], year)
        if started is None and "START u0" in msg and f"cmp={ACTIVITY}" in msg:
            started = timestamp
        proc = START_PID.search(msg)
        if started and pid is None and proc:
            pid = int(proc["pid"])
        if pid is None:
            continue
        if fully_drawn is None and f"Fully drawn {ACTIVITY}" in msg:
            fully_drawn = timestamp
        signal = SELF_SIGNAL.search(msg)
        if signaled is None and signal and int(signal["pid"]) == pid and int(match["pid"]) == pid:
            signaled = timestamp
        death = DEATH.search(msg)
        if dead is None and death and int(death["pid"]) == pid:
            dead = timestamp
        if ("requires the Google Play Store" in msg and PACKAGE in msg
                and (signaled is None or timestamp <= signaled)):
            play_store_warning_before_exit = True
        if "Sent Transition" in msg and "type = CLOSE" in msg and ACTIVITY in msg:
            close_at = CLOSE_AT.search(msg)
            task = TASK_ID.search(msg)
            if close_at and task:
                closes.append((_datetime(close_at["md"], close_at["hms"], year), int(task[1])))
    evidence = {
        "original_activity_started": started is not None,
        "original_pid": pid,
        "original_activity_fully_drawn": fully_drawn is not None,
        "original_self_sigkill_logcat": signaled is not None,
        "original_pid_died": dead is not None,
        "play_store_missing_warning_observed": play_store_warning_before_exit,
    }
    if started and fully_drawn:
        evidence["start_to_fully_drawn_ms"] = _diff_ms(started, fully_drawn)
    if started and signaled:
        evidence["start_to_self_sigkill_ms"] = _diff_ms(started, signaled)
    if signaled and dead:
        evidence["self_sigkill_to_death_ms"] = _diff_ms(signaled, dead)
    valid_closes = []
    if signaled is not None and started is not None:
        for when, task in closes:
            delay = _diff_ms(when, signaled)
            if 0 <= delay <= 200 and _diff_ms(started, when) >= 0:
                valid_closes.append((delay, task))
    if valid_closes:
        lag, task_id = sorted(valid_closes)[0]
        evidence["task_close_before_self_sigkill_ms"] = lag
        evidence["closing_task_id"] = task_id
    if started and pid and signaled and dead and valid_closes:
        status = "TASK_CLOSE_PRECEDES_ORIGINAL_SELF_SIGKILL"
    elif started and signaled and dead:
        status = "ORIGINAL_SELF_SIGKILL_NO_MATCHED_CLOSE"
    else:
        status = "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE"
    return {
        "schema_version": 1,
        "status": status,
        "evidence": evidence,
        "internal_caller": "UNKNOWN",
        "security_trigger": "UNKNOWN",
        "play_integrity_verdict": "NOT_OBSERVED",
        "gameplay_verified": False,
        "limitations": [
            "Task CLOSE preceding SIGKILL does not identify the caller or reason.",
            "Same-PID Android logcat is not a kernel stack trace.",
            "Missing Google Play Store is not proof of the security trigger.",
            "A fully drawn Activity is not proof of a playable game."
        ]
    }

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("logcat", type=Path)
    parser.add_argument("--output", type=Path)
    parser.add_argument("--year", type=int, default=2026)
    args = parser.parse_args()
    report = json.dumps(assess(args.logcat.read_text(encoding="utf8", errors="replace"),
                               args.year), indent=2, sort_keys=True) + "\n"
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(report, encoding="utf8")
    print(report, end="")

if __name__ == "__main__":
    main()
