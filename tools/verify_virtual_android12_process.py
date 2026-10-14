#!/usr/bin/env python3
"""Correlate read-only original Oxide Android 12 PID evidence from three sources.

Sources: ActivityManager's ActivityRecord -> app=ProcessRecord (explicit),
fast-sampled package PID, and Android ApplicationExitInfo signal status.
The prior run's PID-scoped logcat self-kill report is separately identified
as pre-existing forensic evidence; raw original logcat is not in this ZIP.
"""
import argparse
import json
import re
from pathlib import Path
from diagnose_virtual_activity_pid import PACKAGE, ACTIVITY, parse_trace

HISTORY = re.compile(
    r"\*\s*Hist\s*#\d+:\s*ActivityRecord\{[^\n]*" +
    re.escape(PACKAGE) + r"/[^\n]*" + ACTIVITY + r"[^\n]*\}",
    re.I,
)
PROCESS = re.compile(
    r"\bapp=ProcessRecord\{[^\n]*?\s(\d+):" +
    re.escape(PACKAGE) + r"/u\d+a\d+\}",
)
RESUMED = re.compile(
    r"\bmResumedActivity:\s*ActivityRecord\{[^\n]*" +
    re.escape(PACKAGE) + r"/[^\n]*" + ACTIVITY,
)
EXIT_BLOCK = re.compile(r"ApplicationExitInfo\s+#\d+:\s*(.*?)(?=ApplicationExitInfo\s+#\d+:|\Z)",re.S)
FIELD = re.compile(r"^\s*([a-z_]+)=(.*)$",re.M)

def activity_pid(snapshot):
    lines=snapshot.splitlines()
    for i,line in enumerate(lines):
        if not HISTORY.search(line):
            continue
        for next_line in lines[i+1:i+25]:
            if re.search(r"\*\s*(?:Hist #\d+|Task\{)",next_line):
                break
            match=PROCESS.search(next_line)
            if match:
                return int(match.group(1))
    return None

def exit_records(exit_info):
    rows=[]
    for block in EXIT_BLOCK.findall(exit_info):
        attrs={k:v.strip() for k,v in FIELD.findall(block)}
        if attrs.get("process") != PACKAGE:
            continue
        try:
            pid=int(attrs.get("pid",""))
            signal=int(attrs.get("status",""))
        except ValueError:
            continue
        rows.append({
            "pid":pid,"signal_status":signal,
            "reason":attrs.get("reason",""),
            "subreason":attrs.get("subreason",""),
        })
    return rows

def verify(snapshot,trace,exit_info,old):
    pid=activity_pid(snapshot)
    samples=parse_trace(trace)
    seen=sum(pid is not None and pid in x["main_package_pids"] for x in samples)
    reports=[x for x in exit_records(exit_info) if x["pid"]==pid] if pid else []
    signaled9=any(x["signal_status"]==9 and x["reason"].startswith("2 (SIGNALED)") for x in reports)
    prior_pid=old.get("selected_pid")
    prior_selfkill=(prior_pid == pid and
                    old.get("android_process_self_sigkill_logged_for_selected_pid") is True)
    confirmed=(pid is not None and seen >= 2 and
               bool(RESUMED.search(snapshot)) and
               signaled9 and prior_selfkill)
    return {
        "status":("ORIGINAL_ACTIVITY_PID_AND_SIGNAL_CONFIRMED" if confirmed
                  else "INCOMPLETE_DO_NOT_CLAIM_ORIGINAL_ACTIVITY_EXIT"),
        "activity_snapshot_pid":pid,
        "snapshot_activity_resumed":bool(RESUMED.search(snapshot)),
        "main_package_pid_sample_count":seen,
        "sample_count_total":len(samples),
        "prior_pid_scoped_logcat_self_sigkill":prior_selfkill,
        "android_exit_info_sigkill_status9":signaled9,
        "exit_info_matching_records":reports[:3],
        "process_attributed_to_original_oxide_activity":pid is not None,
        "original_oxide_loading_screen":"OBSERVED_SEPARATELY_IN_REAL_SCREENSHOT",
        "specific_game_security_trigger":"UNKNOWN",
        "multiplayer_gameplay_verified":False,
        "esp_with_real_players_verified":False,
        "caveats":[
            "Same PID in ActivityRecord, sampling and Android exit-info establishes lifecycle association.",
            "Self-directed SIGKILL comes from the prior archived PID-scoped analyzer, not freshly parsed raw logcat.",
            "Android exit-info reason/signaled status does not reveal the game's security decision.",
            "Screenshot visual evidence is separate from PID and exit evidence.",
            "No original game security/integrity checks or emulator identity have been changed.",
        ],
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("root",type=Path)
    p.add_argument("--out",type=Path,required=True)
    a=p.parse_args()
    def read(name):
        return (a.root/name).read_text(encoding="utf-8",errors="replace")
    result=verify(read("activities_early.txt"), read("pid_trace.txt"),
                  read("exit_info.txt"),json.loads(read("lifecycle_recovery.json")))
    a.out.mkdir(parents=True,exist_ok=True)
    (a.out/"android12_activity_pid_exit_verified.json").write_text(
        json.dumps(result,indent=2)+"\n")
    lines=[
        "# Real Oxide Android 12 ARM64 virtual PID replay",
        "",
        "Verification: "+result["status"],
        "Resumed original Oxide Activity PID: "+str(result["activity_snapshot_pid"]),
        "Observed exact main-process PID samples: "+str(result["main_package_pid_sample_count"]),
        "Android ApplicationExitInfo signal 9 for same PID: "+str(result["android_exit_info_sigkill_status9"]),
        "Earlier real PID-scoped logcat self-SIGKILL: "+str(result["prior_pid_scoped_logcat_self_sigkill"]),
        "Game security rejection trigger: UNKNOWN",
        "",
        "No physical phone, APK modification, playable game or real-player ESP proven.",
    ]
    (a.out/"android12_activity_pid_exit_verified.md").write_text("\n".join(lines)+"\n")
    print("\n".join(lines))
    if not result["status"].startswith("ORIGINAL_ACTIVITY_PID_AND_SIGNAL"):
        raise SystemExit("Incomplete cross-source evidence")

if __name__=="__main__":
    main()
