#!/usr/bin/env python3
"""Independent stock Android Settings BACK control for Binder endpoint attribution.

The original game is never modified. Matching a Binder target node and numeric
transaction 15 to a controlled Android Settings activity finish strongly
corroborates an activity finish request, but cannot identify which part of
the original app decided to initiate the request.
"""
import argparse
import json
from pathlib import Path

from audit_original_binder_finish_window import LINE
from corroborate_original_kernel_and_namespace import prove
from diagnose_original_exit import log_lines

def assess(root):
    root=Path(root)
    read=lambda name:json.loads((root/name).read_text())
    game=read("host_pid_namespace.json")
    server=read("host_system_server_namespace.json")
    settings=read("host_settings_namespace.json")
    control=read("settings_back_window.json")
    forensic=read("forensic_summary.json")
    trace=(root/"kernel_sigkill_trace.txt").read_text(errors="replace")
    exitinfo=(root/"exit_info.txt").read_text(errors="replace")
    kernel=prove(game,trace,forensic,exitinfo)
    game_pid=game.get("host_pid")
    server_pid=server.get("host_pid")
    settings_pid=settings.get("host_pid")
    server_ok=(
        server.get("status")=="ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN"
        and server.get("original_process_cmdline")=="system_server"
        and server.get("pid_namespace")==game.get("pid_namespace")
    )
    settings_ok=(
        settings.get("status")=="ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN"
        and settings.get("original_process_cmdline")=="com.android.settings"
        and settings.get("pid_namespace")==game.get("pid_namespace")
        and type(settings_pid) is int and settings_pid>0
    )
    clocks_ok=(
        type(control.get("before_host_boot_ns")) is int
        and type(control.get("after_host_boot_ns")) is int
        and control.get("input_keyevent_exit_code")==0
        and 0<control["before_host_boot_ns"]<=control["after_host_boot_ns"]
    )
    finishes = [
        x for x in log_lines(root/"all_buffers.txt")
        if x["tag"]=="wm_finish_activity" and "com.android.settings/" in x["msg"]
    ]
    log_finish=bool(finishes)
    kill_ns=min((x["kernel_boot_ns"] for x in kernel.get("matched_syscalls",[])),default=None)
    game_ipc=[]
    control_ipc=[]
    if (server_ok and settings_ok and clocks_ok
        and kernel.get("status")=="SAME_RUN_TIMESTAMPED_HOST_KERNEL_GUEST_SELF_SIGKILL_PROVEN"
        and kill_ns):
        for line in trace.splitlines():
            m=LINE.search(line)
            if not m:
                continue
            (sender,thread,dest,dest_thread,txn,node,code,reply,flags,comm,when)=m.groups()
            when=int(when)
            if int(dest)!=server_pid or int(reply)!=0 or node is None or int(node)<=0 or int(code)!=15:
                continue
            entry={
                "host_sender":int(sender),"host_thread":int(thread),
                "target_node":int(node),"transaction":int(txn),
                "numeric_code":15,"kernel_boot_ns":when,"sender_comm":comm
            }
            if int(sender)==settings_pid and control["before_host_boot_ns"]-2_000_000_000<=when<=control["after_host_boot_ns"]+3_000_000_000:
                control_ipc.append(entry)
            elif int(sender)==game_pid and 0<=kill_ns-when<=20_000_000:
                entry["milliseconds_before_game_sigkill"]=round((kill_ns-when)/1e6,3)
                game_ipc.append(entry)
    control_nodes={e["target_node"] for e in control_ipc}
    game_nodes={e["target_node"] for e in game_ipc}
    endpoint_match=(log_finish and len(control_nodes)==1 and len(game_nodes)==1
                    and control_nodes==game_nodes)
    return {
        "status":("CONTROLLED_SETTINGS_FINISH_BINDER_NODE_MATCHES_ORIGINAL_FINAL_CODE15"
                  if endpoint_match else "CONTROLLED_FINISH_INTERFACE_ENDPOINT_NOT_PROVEN"),
        "original_host_pid":game_pid,
        "original_guest_pid":forensic.get("initial_activity_pid"),
        "android_settings_host_pid":settings_pid if settings_ok else None,
        "system_server_host_pid":server_pid if server_ok else None,
        "original_kernel_sigkill_verified":kernel.get("status")=="SAME_RUN_TIMESTAMPED_HOST_KERNEL_GUEST_SELF_SIGKILL_PROVEN",
        "settings_activity_finish_logged":log_finish,
        "settings_controlled_binder_code15_nodes":sorted(control_nodes),
        "original_last20ms_binder_code15_nodes":sorted(game_nodes),
        "controlled_settings_code15_events":control_ipc[-10:],
        "original_last20ms_code15_events":game_ipc[-10:],
        "shared_binder_endpoint_node_correlated":endpoint_match,
        "interface_descriptor_read_from_transaction":False,
        "original_application_finish_caller_identified":False,
        "reason_for_activity_finish":"UNKNOWN",
        "gameplay_verified":False,
        "original_apks_modified":False,
        "limitations":[
            "A stock Settings BACK action is a separate positive control, not activity injection into the original game.",
            "A shared binder endpoint and numeric code strongly corroborate the same service operation, not the application call stack.",
            "The kernel Binder tracepoint does not expose its interface descriptor; do not claim direct AIDL type verification.",
            "No anti-cheat or Play Integrity modification, no spoofing and no physical phone."
        ]
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("artifact_dir",type=Path)
    a=p.parse_args()
    r=assess(a.artifact_dir)
    (a.artifact_dir/"original_finish_binder_endpoint_control.json").write_text(
        json.dumps(r,indent=2)+"\n")
    print(json.dumps(r,indent=2))
    return 0 if r["shared_binder_endpoint_node_correlated"] else 1
if __name__=="__main__":
    raise SystemExit(main())
