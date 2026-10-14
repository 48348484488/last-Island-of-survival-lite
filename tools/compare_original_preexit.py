#!/usr/bin/env python3
"""Compare native Android diagnostics emitted by the ORIGINAL Oxide PID.

Uses archived, unchanged Redroid ARM64 Android13 launch evidence.
The previous A/B artifact contains stale event-buffer entries in the
second run. Always recompute the current Activity PID via the corrected
forensic parser, never trust its outdated comparison.json.

A warning that precedes a self-directed SIGKILL is not proof it caused
the game's security rejection. No game APK or Android properties altered.
"""
import argparse
import json
import re
from pathlib import Path
from diagnose_original_exit import diagnose, log_lines, event_ms

EVENTS = {
    "native_build_fingerprint_api_overlength":
        ("libc", re.compile(r'The property "ro[.]build[.]fingerprint" has a value with length (\d+) that is too large')),
    "google_play_store_missing":
        ("GooglePlayServicesUtil", re.compile(r"requires the Google Play Store, but it is missing")),
    "google_play_services_missing":
        ("GooglePlayServicesUtil", re.compile(r"Google Play services is missing")),
    "gmscore_module_invalid":
        ("DynamiteModule", re.compile(r"Invalid GmsCore APK")),
    "firebase_dependency_unavailable":
        ("FirebaseInstanceId", re.compile(r"Google Play services missing")),
    "graphics_buffer_allocation_failed":
        ("GraphicBufferAllocator", re.compile(r"Failed to allocate")),
    "sensor_enumeration_failed":
        ("Sensors", re.compile(r"Failed to get Sensor list")),
    "unity_engine_initialized":
        ("Unity", re.compile(r"MemoryManager: Using|ApplicationInfo |Product Name:")),
    "il2cpp_native_loaded":
        ("IL2CPP", re.compile(r"JNI_OnLoad")),
    "unity_managed_locale":
        ("IL2CPP", re.compile(r"Locale ")),
    "android_security_threat_message_in_game_log":
        (None, re.compile(r"Security Threat Detected by App")),
    "unity_application_quit_logged":
        ("Unity", re.compile(r"Application[.]Quit|OnApplicationQuit|Application quitting",re.I)),
}
SIGNAL = re.compile(r"Sending signal[.] PID: (\d+) SIG: (\d+)")

def analyze(directory):
    directory=Path(directory)
    f=diagnose(directory)
    pid=f["initial_activity_pid"]
    start=f["initial_activity_start"]
    kill=f["first_self_sigkill"]
    if not isinstance(pid,int) or not start:
        raise ValueError("No exact original Activity PID: refuse classification")
    result={
        key:{"count":0,"first_after_launch_ms":None,"last_after_launch_ms":None}
        for key in EVENTS
    }
    warnings={}
    for row in log_lines(directory/"all_buffers.txt"):
        if row["pid"] != pid or row["timestamp"] < start:
            continue
        if kill and row["timestamp"] > kill:
            continue
        for key,(tag,pattern) in EVENTS.items():
            if tag is not None and tag != row["tag"]:
                continue
            m=pattern.search(row["raw"])
            if not m:
                continue
            moment=event_ms(start,row["timestamp"])
            if moment < 0:
                continue
            slot=result[key]
            slot["count"] += 1
            if slot["first_after_launch_ms"] is None:
                slot["first_after_launch_ms"]=moment
            slot["last_after_launch_ms"]=moment
            if key=="native_build_fingerprint_api_overlength":
                warnings[m.group(1)]=warnings.get(m.group(1),0)+1
    return {
        "original_activity_pid":pid,
        "activity_start":start,
        "self_sigkill_timestamp":kill,
        "self_sigkill_ms_after_activity_start":event_ms(start,kill) if kill else None,
        "termination":f["classification"],
        "events":result,
        "fingerprint_reported_lengths":warnings,
        "recorded_toast_from_screenshot":"OBSERVED_IN_SEPARATE_PRIOR_VISUAL_EVIDENCE",
        "security_rejection_root_cause":"NOT_IDENTIFIED",
        "original_game_modified":False,
        "device_identity_changed":False,
        "multiplayer_or_real_esp_verified":False,
    }

def compare(before,after):
    categories=[]
    for key in EVENTS:
        a=before["events"][key];b=after["events"][key]
        cats=(
            "PERSISTENT_BOTH" if a["count"] and b["count"] else
            "ONLY_BEFORE" if a["count"] else
            "ONLY_AFTER" if b["count"] else
            "NOT_OBSERVED_IN_PID_SCOPED_LOGS"
        )
        categories.append({
            "event":key, "classification":cats,
            "before_count":a["count"],"after_count":b["count"],
            "before_first_ms":a["first_after_launch_ms"],
            "after_first_ms":b["first_after_launch_ms"],
        })
    return {
        "before_activity_pid":before["original_activity_pid"],
        "after_activity_pid":after["original_activity_pid"],
        "before_self_sigkill":before["termination"]=="ORIGINAL_ACTIVITY_SELF_SIGKILL",
        "after_self_sigkill":after["termination"]=="ORIGINAL_ACTIVITY_SELF_SIGKILL",
        "process_scoped_event_comparison":categories,
        "can_identify_specific_security_check":False,
        "runtime_permissions_sole_cause_supported":False,
        "causality_limitations":[
            "Persistent warnings are correlations only, not direct causes.",
            "Android libc overlength property warning does not identify which game function checked the property.",
            "GMS missing on both runs cannot distinguish it from other virtual Android constraints.",
            "Android event-buffer history is mixed: select the latest exact Activity PID per attempt.",
            "No source proves which game routine displayed the security toast.",
            "No spoofing, injection, API hooking, memory tampering or bypass was performed.",
        ],
    }

def markdown(d):
    c=d["comparison"]
    lines=["# Original Oxide ARM64: PID-scoped pre-exit environment comparison",
           "",
           "Before permissions PID: "+str(c["before_activity_pid"]),
           "After permissions PID: "+str(c["after_activity_pid"]),
           "Both original processes self-directed SIGKILL: "+
                str(c["before_self_sigkill"] and c["after_self_sigkill"]),
           "",
           "| Original PID event | Before | After | Classification | First ms before / after |",
           "|---|---:|---:|---|---|"]
    for x in c["process_scoped_event_comparison"]:
        lines.append("| "+x["event"]+" | "+str(x["before_count"])+" | "+
                     str(x["after_count"])+" | "+x["classification"]+" | "+
                     str(x["before_first_ms"])+" / "+str(x["after_first_ms"])+" |")
    lines += [
        "", "ro.build.fingerprint native-property reported lengths:",
        "before="+str(d["before"]["fingerprint_reported_lengths"])+
        " after="+str(d["after"]["fingerprint_reported_lengths"]),
        "", "## What the results do NOT establish", "",
    ]
    lines += ["- "+x for x in c["causality_limitations"]]
    lines += ["","Specific security rejection trigger: NOT IDENTIFIED.",
              "No game code/security check or Android system identity modified.",
              "Multiplayer and real-player ESP: NOT VERIFIED."]
    return "\n".join(lines)+"\n"

def main():
    ap=argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--before",type=Path,required=True)
    ap.add_argument("--after",type=Path,required=True)
    ap.add_argument("--out",type=Path,required=True)
    a=ap.parse_args()
    before=analyze(a.before); after=analyze(a.after)
    d={"before":before,"after":after,"comparison":compare(before,after)}
    a.out.mkdir(parents=True,exist_ok=True)
    (a.out/"pid_scoped_preexit.json").write_text(
        json.dumps(d,indent=2)+"\n",encoding="utf-8")
    (a.out/"pid_scoped_preexit.md").write_text(markdown(d),encoding="utf-8")
    print(markdown(d))
    if not (d["comparison"]["before_self_sigkill"] and d["comparison"]["after_self_sigkill"]):
        raise SystemExit("Missing original-game death evidence")
if __name__=="__main__":
    main()
