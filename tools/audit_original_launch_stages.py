#!/usr/bin/env python3
"""Read-only original Oxide Android ARM64 startup milestone comparison.

All native stages are attributed to the current original Activity PID.
System graphic allocator events are counted SEPARATELY; no warnings are
treated as proof of security cause or playable gameplay.
"""
import argparse
import json
from pathlib import Path
from diagnose_original_exit import diagnose,event_ms,log_lines,PACKAGE,ACTIVITY

STAGES=(("loader","linker","libloader.so"),("il2cpp_jni","IL2CPP","JNI_OnLoad"),
        ("unity_context","Unity","Context Type:"),
        ("unity_memory","Unity","MemoryManager:"),
        ("unity_version","Unity","Built from"),
        ("unity_product","Unity","Product Name: OXIDE"),
        ("il2cpp_locale","IL2CPP","Locale "))
def analyze(folder):
    folder=Path(folder)
    result=diagnose(folder)
    pid=result["initial_activity_pid"]
    begin=result["initial_activity_start"]
    end=result["first_self_sigkill"] or result["death_event"]
    stamps={k:None for k,_,_ in STAGES}
    stamps.update({"fully_drawn":None,"activity_finish":None,
                   "self_sigkill":None,"original_pid_death":None})
    warnings={"play_store_missing_game_pid":0,"sensor_list_game_pid":0,
              "graphics_allocator_errors_other_pid":0}
    if pid is not None and begin:
        for line in log_lines(folder/"all_buffers.txt"):
            stamp=line["timestamp"]
            if stamp<begin or end and stamp>end:continue
            tag,text=line["tag"],line["raw"]
            if line["pid"]==pid:
                for k,t,marker in STAGES:
                    if stamps[k] is None and t==tag and marker in text:
                        stamps[k]=event_ms(begin,stamp)
                if tag=="GooglePlayServicesUtil" and "requires the Google Play Store" in text:
                    warnings["play_store_missing_game_pid"]+=1
                if tag=="Sensors" and "Failed to get Sensor list" in text:
                    warnings["sensor_list_game_pid"]+=1
            elif tag=="GraphicBufferAllocator" and "Failed to allocate" in text:
                warnings["graphics_allocator_errors_other_pid"]+=1
            if (stamps["fully_drawn"] is None and line["pid"]!=pid and
                tag=="ActivityTaskManager" and "Fully drawn" in text and
                PACKAGE+"/" in text and ACTIVITY in text):
                stamps["fully_drawn"]=event_ms(begin,stamp)
        for name,key in (("activity_finish","activity_finish"),
                         ("self_sigkill","first_self_sigkill"),
                         ("original_pid_death","death_event")):
            if result.get(key) and result[key]>=begin:
                stamps[name]=event_ms(begin,result[key])
    return {
        "status":"PID_SCOPED_ORIGINAL_GAME_STARTUP",
        "activity_pid":pid,"activity_start":begin,
        "original_process_exit":result["classification"],
        "milestones_ms_after_activity_start":stamps,
        "warnings":warnings,
        "unity_and_il2cpp_observed":stamps["unity_version"] is not None and stamps["il2cpp_jni"] is not None,
        "self_sigkill_observed":stamps["self_sigkill"] is not None,
        "security_cause_established":False,
        "play_integrity_verdict":"NOT_MEASURED",
        "playable_game_verified":False,
        "real_player_esp_verified":False,
        "limitations":[
            "Fully drawn means an Android Activity drew a window, not playable gameplay.",
            "Other-PID graphic allocator warnings are not attributed to game code.",
            "Warnings or timing correlations do not identify the specific security rejection cause.",
            "Absent markers are not evidence a game stage never executed.",
            "Reads archived logs only: no APK execution, patches, spoofing, or hooks."
        ]
    }
def compare(before,after):
    if before["activity_pid"] is None or after["activity_pid"] is None:
        raise ValueError("cannot compare absent original Activity PID")
    return {
        "status":"ORIGINAL_ARM64_ARCHIVED_PID_MILESTONE_COMPARISON",
        "before_pid":before["activity_pid"],"after_pid":after["activity_pid"],
        "both_unity_il2cpp_observed":before["unity_and_il2cpp_observed"] and after["unity_and_il2cpp_observed"],
        "both_original_processes_self_sigkill":before["self_sigkill_observed"] and after["self_sigkill_observed"],
        "before_ms_to_self_sigkill":before["milestones_ms_after_activity_start"]["self_sigkill"],
        "after_ms_to_self_sigkill":after["milestones_ms_after_activity_start"]["self_sigkill"],
        "root_cause_proven":False,"playable_game_proven":False
    }
def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("--before",type=Path,required=True)
    p.add_argument("--latest",type=Path,required=True)
    p.add_argument("--out",type=Path,required=True)
    args=p.parse_args()
    a=analyze(args.before);b=analyze(args.latest)
    data={"before":a,"latest":b,"comparison":compare(a,b)}
    args.out.mkdir(parents=True,exist_ok=True)
    (args.out/"original_stage_milestones.json").write_text(json.dumps(data,indent=2)+"\n")
    print(json.dumps(data["comparison"],indent=2))
if __name__=="__main__":main()
