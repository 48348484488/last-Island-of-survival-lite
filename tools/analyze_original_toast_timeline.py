#!/usr/bin/env python3
"""Read-only timeline for *observed* original Oxide Android Activity lifecycle.

Correlates original Activity start PID, generic Android Toast window events,
named package Toast cleanup, Activity finish, and same-PID SIGKILL. Does NOT
infer which Toast was displayed, a security vendor, or why the app exited.

Archive replay uses original Android 13 ARM64 Redroid logcat before/after
normal permissions; no new game downloads, binary modifications or hooking.
"""
import argparse
import json
import re
from datetime import datetime
from pathlib import Path

GAME = "com.catsbit.oxidesurvivalisland"
ACTIVITY = "OxideActivity"
ROW = re.compile(
    r"^(\d\d-\d\d)\s+(\d\d:\d\d:\d\d\.\d{3})\s+"
    r"(\d+)\s+(\d+)\s+([A-Z])\s+([^:]+?):\s*(.*)$"
)
START = re.compile(
    r"^\[\d+,(\d+),\d+," + re.escape(GAME) +
    r",next-top-activity,\{[^}]*" + ACTIVITY + r"\}\]"
)
END = re.compile(re.escape(GAME) + r"/[^,\]]*" + ACTIVITY)
SIGNAL = re.compile(r"Sending signal\. PID:\s*(\d+)\s+SIG:\s*(\d+)")
DEATH = re.compile(r"^\[\d+,(\d+)," + re.escape(GAME) + r"(?:,|\])")
TOAST_CREATED = re.compile(r"Window\{(\w+) u\d+ Toast\}.*Setting back callback OnBackInvokedCallbackInfo")
TOAST_REMOVED = re.compile(r"Window\{(\w+) u\d+ Toast\}.*Setting back callback null")
TOAST_OWNER_CLEANUP = re.compile(r"Toast already killed\. pkg=" + re.escape(GAME) + r"\b")

def milliseconds(date_clock):
    return datetime.strptime("2000-" + date_clock, "%Y-%m-%d %H:%M:%S.%f")

def delta(a, b):
    return round((milliseconds(b) - milliseconds(a)).total_seconds() * 1000)

def parse_logs(text):
    rows = []
    for line in text.splitlines():
        m = ROW.match(line)
        if m is None:
            continue
        month_day, clock, pid, tid, level, tag, msg = m.groups()
        rows.append({
            "time":month_day + " " + clock,
            "pid":int(pid),
            "tag":tag.strip(),
            "msg":msg,
        })
    return rows

def analyze(text):
    rows = parse_logs(text)
    starts=[]
    for row in rows:
        if row["tag"] != "am_proc_start":
            continue
        match=START.search(row["msg"])
        if match:
            starts.append((row["time"],int(match.group(1))))
    if not starts:
        return {"classification":"NO_ORIGINAL_ACTIVITY_START_EVENT",
                "original_activity_pid":None,"security_toast_content":"NOT_IN_LOGS",
                "specific_protection_trigger":"UNKNOWN",
                "gameplay_verified":False,"real_player_esp_verified":False}
    start, game_pid=max(starts)
    # Stale Android event buffers might contain older launches. Only most recent
    # original Activity and later events can be correlated to this launch.
    relevant=[r for r in rows if r["time"] >= start]
    phases={}
    open_toasts={}
    toast_window_ids=[]
    owned_cleanup=[]
    for row in relevant:
        name=None
        tag,msg,pid=row["tag"],row["msg"],row["pid"]
        if tag=="CoreBackPreview":
            m=TOAST_CREATED.search(msg)
            if m:
                open_toasts[m.group(1)]=row["time"]
                toast_window_ids.append(m.group(1))
                if "generic_toast_window" not in phases:
                    phases["generic_toast_window"]=row["time"]
            m=TOAST_REMOVED.search(msg)
            if m and m.group(1) in open_toasts:
                phases.setdefault("generic_toast_window_removed",row["time"])
        elif tag=="wm_finish_activity" and END.search(msg):
            name="original_activity_finish"
        elif tag=="Process":
            m=SIGNAL.search(msg)
            if m and int(m.group(1))==game_pid and int(m.group(2))==9 and pid==game_pid:
                name="original_process_self_sigkill"
        elif tag=="am_proc_died":
            m=DEATH.search(msg)
            if m and int(m.group(1))==game_pid:
                name="original_process_death"
        elif tag=="NotificationService" and TOAST_OWNER_CLEANUP.search(msg):
            owned_cleanup.append(row["time"])
            name="package_toast_cleanup"
        if name:
            phases.setdefault(name,row["time"])
    rel={key:delta(start,time) for key,time in phases.items()}
    selfkill=rel.get("original_process_self_sigkill")
    finish=rel.get("original_activity_finish")
    win=rel.get("generic_toast_window")
    toast_cleanup=rel.get("package_toast_cleanup")
    death=rel.get("original_process_death")
    ordered=(selfkill is not None and finish is not None and
             0 <= finish <= selfkill and
             (death is None or selfkill <= death) and
             (win is None or 0 <= win < finish))
    confirmed=(ordered and death is not None)
    status=("ORIGINAL_SELF_SIGKILL_LIFECYCLE_AND_TOAST_WINDOW_OBSERVED"
            if confirmed and win is not None and toast_cleanup is not None
            else "ORIGINAL_SELF_SIGKILL_LIFECYCLE_OBSERVED" if confirmed
            else "PARTIAL_LIFECYCLE_EVIDENCE")
    return {
        "classification":status,
        "original_activity_pid":game_pid,
        "initial_original_activity_start":start,
        "original_activity_launch_events_in_buffer":len(starts),
        "latest_activity_selection":True,
        "phases_ms_from_original_start":rel,
        "generic_toast_window_observed":win is not None,
        "generic_toast_window_count":len(toast_window_ids),
        "package_named_toast_cleanup_observed":toast_cleanup is not None,
        "activity_finish_before_self_sigkill":ordered,
        "android_original_pid_death_event_observed":death is not None,
        "toast_text_captured_in_logs":False,
        "security_toast_content":"NOT_IN_LOGS_VISUAL_EVIDENCE_SEPARATE",
        "toast_window_specific_package_owner":"NOT_ATTESTED",
        "specific_protection_trigger":"UNKNOWN",
        "gameplay_verified":False,
        "real_player_esp_verified":False,
        "limits":[
            "Generic Toast window logs lack app package attribution; named NotificationService cleanup is independent context.",
            "Captured screenshots, not these timestamps, show the game security message.",
            "Activity lifecycle and SIGKILL ordering cannot identify the game protection rule.",
            "A native Android issue and the protected app's own decision must be investigated separately.",
            "No APK was modified, protected process inspected via injection, or Android identity spoofed.",
        ],
    }

def compare(before,after):
    assert before["original_activity_pid"] != after["original_activity_pid"]
    timeline={}
    names=["generic_toast_window","original_activity_finish",
           "original_process_self_sigkill","original_process_death",
           "package_toast_cleanup"]
    for name in names:
        a=before["phases_ms_from_original_start"].get(name)
        b=after["phases_ms_from_original_start"].get(name)
        timeline[name]={"before_ms":a,"after_ms":b,
                        "difference_ms":b-a if a is not None and b is not None else None}
    return {
        "status": "REPEATED_ACTIVITY_SELF_KILL_WITH_TOAST_WINDOW" if
                  all(x["classification"]=="ORIGINAL_SELF_SIGKILL_LIFECYCLE_AND_TOAST_WINDOW_OBSERVED"
                      for x in (before,after)) else "EVIDENCE_INCOMPLETE",
        "before_original_pid":before["original_activity_pid"],
        "after_original_pid":after["original_activity_pid"],
        "timeline":timeline,
        "cause_of_protection_decision":"UNKNOWN",
        "actual_toast_text_from_logcat":"NOT_OBSERVED",
        "specific_vendor":"UNCONFIRMED",
        "security_bypass_performed":False,
        "gameplay_verified":False,
        "real_player_esp_verified":False,
    }

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--before",type=Path,required=True)
    parser.add_argument("--after",type=Path,required=True)
    parser.add_argument("--out",type=Path,required=True)
    opt=parser.parse_args()
    b=analyze(opt.before.read_text(errors="replace"))
    a=analyze(opt.after.read_text(errors="replace"))
    cmp=compare(b,a)
    opt.out.mkdir(parents=True,exist_ok=True)
    data={"before":b,"after":a,"comparison":cmp}
    (opt.out/"activity_toast_sigkill_timeline.json").write_text(
        json.dumps(data,indent=2)+"\n",encoding="utf-8")
    lines=["# Real original Oxide native virtual Android 13 Activity lifecycle","",
           "Result: "+cmp["status"],
           "Before process PID: "+str(cmp["before_original_pid"]),
           "After process PID: "+str(cmp["after_original_pid"]),
           "",
           "| Android lifecycle phase | Before launch +ms | After launch +ms | Delta |",
           "|---|---:|---:|---:|"]
    for key,item in cmp["timeline"].items():
        lines.append("| "+key+" | "+str(item["before_ms"])+" | "+
                     str(item["after_ms"])+" | "+str(item["difference_ms"])+" |")
    lines+=["",
            "Generic system Toast window activity and the original game's",
            "package-scoped Toast cleanup are separately observed. The exact",
            "Toast text is NOT recorded in logcat; it was seen in screenshots.",
            "Neither the security vendor nor the rule triggering self-termination is known.",
            "",
            "No original game or Android security modification and no playable ESP."]
    (opt.out/"activity_toast_sigkill_timeline.md").write_text("\n".join(lines)+"\n")
    print("\n".join(lines))
    if cmp["status"]!="REPEATED_ACTIVITY_SELF_KILL_WITH_TOAST_WINDOW":
        raise SystemExit("Original-game timeline evidence incomplete")
if __name__=="__main__":
    main()
