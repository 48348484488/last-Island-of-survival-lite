#!/usr/bin/env python3
"""Recover *observed* original Oxide main-process PID independently of am_proc_start.

Android 12 can render the original Unity loading UI while the event ring
omits its process-start record. Sources are kept distinct: sampled pidof
shows a package process, not necessarily the Activity's thread; explicit
ActivityManager next-top-activity records can establish Activity launch.
Never convert screenshots into PID or invent an app security root cause.
"""
import argparse
import json
import re
from collections import Counter
from pathlib import Path
from diagnose_original_exit import PACKAGE, log_lines, SIG_RE

AM_ACTIVITY = re.compile(
    r"\bStart proc\s+(\d+):" + re.escape(PACKAGE) + r"/\S+.*\bnext-top-activity\b",
    re.I,
)
SAMPLE = re.compile(r"^sample=(\d+)\s+utc=(\S+)\s+pid=([0-9, ]+|none)\s*$")
PROC_DIED = re.compile(r"^\[\d+,(\d+)," + re.escape(PACKAGE) + r"[,:\]]")

def parse_trace(text):
    rows = []
    for raw in text.splitlines():
        match = SAMPLE.match(raw.strip())
        if not match:
            continue
        pids = sorted({int(x) for x in re.findall(r"\d+", match.group(3))}
                      - {0})
        rows.append({"sample":int(match.group(1)),
                     "host_time_utc":match.group(2),
                     "main_package_pids":pids})
    return rows

def analyze(folder):
    folder=Path(folder)
    def read(name):
        p=folder/name
        return p.read_text(encoding="utf-8",errors="replace") if p.is_file() else ""
    try:
        forensic=json.loads(read("forensic_summary.json"))
    except (ValueError,TypeError):
        forensic={}
    sample_rows=parse_trace(read("pid_trace.txt"))
    pid_count=Counter(pid for row in sample_rows for pid in row["main_package_pids"])
    lines=list(log_lines(folder/"all_buffers.txt")) if (folder/"all_buffers.txt").is_file() else []
    am_candidates=[]
    for row in lines:
        if row["tag"]!="ActivityManager":
            continue
        hit=AM_ACTIVITY.search(row["raw"])
        if hit:
            am_candidates.append({"pid":int(hit.group(1)),"time":row["timestamp"]})
    event_pid=forensic.get("initial_activity_pid")
    event_pid=event_pid if isinstance(event_pid,int) and event_pid>0 else None
    if event_pid:
        pid=event_pid
        source="AM_PROC_START_EVENT"
        activity_attributed=True
    elif am_candidates:
        pid=am_candidates[-1]["pid"]
        source="ACTIVITYMANAGER_NEXT_TOP_ACTIVITY_LOG"
        activity_attributed=True
    elif pid_count:
        pid, _=pid_count.most_common(1)[0]
        source="MAIN_PACKAGE_PID_TRACE_ONLY"
        activity_attributed=False
    else:
        pid=None
        source="PID_NOT_OBSERVED"
        activity_attributed=False
    sigs=[]
    deaths=[]
    if pid:
        for row in lines:
            if row["tag"]=="Process":
                m=SIG_RE.search(row["raw"])
                if m and int(m.group(1))==pid and int(m.group(2))==9:
                    sigs.append({"emitter_pid":row["pid"],
                                 "same_process":row["pid"]==pid,
                                 "timestamp":row["timestamp"]})
            if row["tag"]=="am_proc_died":
                m=PROC_DIED.search(row["raw"].split(": ",1)[-1])
                if m and int(m.group(1))==pid:
                    deaths.append(row["timestamp"])
    self_killed=any(x["same_process"] for x in sigs)
    stage=("ACTIVITY_LAUNCH_PID_CONFIRMED" if activity_attributed else
           "MAIN_PACKAGE_PROCESS_OBSERVED_ACTIVITY_PID_UNVERIFIED" if pid else
           "NO_PROCESS_PID_OBSERVED")
    return {
        "status":stage,
        "selected_pid":pid,
        "pid_source":source,
        "pid_confirms_activity_process":activity_attributed,
        "observed_pid_trace_samples":len(sample_rows),
        "selected_pid_sample_count":pid_count.get(pid,0),
        "other_observed_pids":[x for x in pid_count if x != pid],
        "explicit_start_status_ok":"Status: ok" in read("activity_start.txt"),
        "android_process_self_sigkill_logged_for_selected_pid":self_killed,
        "self_sigkill_attributed_to_original_activity":self_killed and activity_attributed,
        "process_death_events_for_selected_pid":len(deaths),
        "original_forensic_classification":forensic.get("classification","NOT_MEASURED"),
        "game_loading_ui_from_screenshot":"NOT_ANALYZED_BY_THIS_TOOL",
        "security_rejection_root_cause":"NOT_DETERMINED",
        "play_integrity_verdict":"NOT_MEASURED",
        "gameplay_verified":False,
        "real_player_esp_verified":False,
        "limitations":[
            "PID samples identify a main package process, not necessarily the resumed Activity.",
            "An ActivityManager next-top-activity line can establish lifecycle attribution even when am_proc_start is missing.",
            "A same-PID SIGKILL establishes the emitter, not the reason the security warning appeared.",
            "Screenshots need separate visual interpretation and do not establish process PIDs.",
            "Never attribute background analytics services to the original game Activity.",
            "No game binaries or Android identity/integrity data were changed.",
        ],
    }

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument("folder",type=Path)
    args=parser.parse_args()
    d=analyze(args.folder)
    (args.folder/"lifecycle_recovery.json").write_text(json.dumps(d,indent=2)+"\n")
    summary=["# Original Oxide Android lifecycle recovery (read-only)",
             "", "Classification: "+d["status"],
             "Selected PID: "+str(d["selected_pid"]),
             "Evidence source: "+d["pid_source"],
             "Attributed to Activity: "+str(d["pid_confirms_activity_process"]),
             "Concurrent package PID samples: "+str(d["selected_pid_sample_count"]),
             "Same-process SIGKILL observed: "+str(d["android_process_self_sigkill_logged_for_selected_pid"]),
             "Self SIGKILL proven for Activity: "+str(d["self_sigkill_attributed_to_original_activity"]),
             "Exact security reason: NOT DETERMINED", "",
             "No physical device, no game patches, no multiplayer or ESP verified."]
    (args.folder/"lifecycle_recovery.md").write_text("\n".join(summary)+"\n")
    print("\n".join(summary))

if __name__=="__main__":
    main()
