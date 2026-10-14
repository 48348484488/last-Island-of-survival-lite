#!/usr/bin/env python3
"""Read-only Binder attribution window for the unmodified original Android Activity.

Binder tracepoints expose sender process, destination process, numeric code and
kernel time. A numeric transaction code does NOT identify finishActivity unless
the original Android interface descriptor/transaction map is independently
proved. A correlated Binder call is only an IPC candidate, not a root cause.
"""
import argparse
import json
import re
from pathlib import Path
from corroborate_original_kernel_and_namespace import prove
from diagnose_original_exit import PACKAGE

LINE = re.compile(
    r"\bANDROID_BINDER\s+host_sender=(\d+)\s+host_tid=(\d+)\s+"
    r"dest_proc=(\d+)\s+dest_thread=(\d+)\s+"
    r"transaction=(\d+)\s+code=(\d+)\s+reply=(\d+)\s+flags=(\d+)\s+"
    r"comm=(\S+)\s+boot_ns=(\d+)\b"
)

def inspect(root):
    root = Path(root)
    read_json = lambda name: json.loads((root / name).read_text(encoding="utf-8"))
    game = read_json("host_pid_namespace.json")
    server = read_json("host_system_server_namespace.json")
    forensic = read_json("forensic_summary.json")
    trace = (root / "kernel_sigkill_trace.txt").read_text(errors="replace")
    exit_info = (root / "exit_info.txt").read_text(errors="replace")
    kernel = prove(game, trace, forensic, exit_info)

    server_ok = (
        server.get("status") == "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN"
        and server.get("original_process_cmdline") == "system_server"
        and type(server.get("host_pid")) is int and server["host_pid"] > 1
        and isinstance(server.get("nspid_chain"), list)
        and len(server["nspid_chain"]) >= 2
        and server["nspid_chain"][0] == server["host_pid"]
        and server["nspid_chain"][-1] == server.get("guest_pid")
        and server.get("pid_namespace") == game.get("pid_namespace")
    )
    original_host_pid = game.get("host_pid")
    game_ok = kernel["status"] == "SAME_RUN_TIMESTAMPED_HOST_KERNEL_GUEST_SELF_SIGKILL_PROVEN"
    kill_events = kernel["matched_syscalls"]
    if not (game_ok and server_ok and kill_events):
        return {
            "status": "HOST_OR_GUEST_PID_ORIGINAL_KERNEL_PROOF_MISSING",
            "game_kernel_proven": game_ok, "system_server_mapping_proven": server_ok,
            "finish_requester_identified": False, "root_cause_identified": False,
        }
    kill_ns = min(e["kernel_boot_ns"] for e in kill_events)
    sender_events = []
    total_binder = 0
    for line in trace.splitlines():
        m = LINE.search(line)
        if not m:
            continue
        total_binder += 1
        (sender, thread, dest, dest_thread, transaction, code, reply,
         flags, comm, when_ns) = m.groups()
        when_ns = int(when_ns)
        if int(sender) != original_host_pid or int(dest) != server["host_pid"]:
            continue
        if int(reply) != 0 or not (0 <= kill_ns - when_ns <= 15_000_000_000):
            continue
        sender_events.append({
            "sender_host_pid": int(sender),
            "sender_host_thread_id": int(thread),
            "destination_system_server_host_pid": int(dest),
            "transaction_id": int(transaction),
            "numeric_transaction_code": int(code),
            "flags": int(flags),
            "kernel_boot_ns": when_ns,
            "milliseconds_before_game_self_sigkill": round((kill_ns - when_ns)/1e6,3),
            "sender_thread_comm": comm,
            "transaction_interface": "NOT_IDENTIFIED",
            "is_finish_activity_call_proven": False,
        })
    sender_events.sort(key=lambda x:x["kernel_boot_ns"])
    result = {
        "status": ("ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_OBSERVED"
                   if sender_events else "NO_ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_CAPTURED"),
        "original_host_pid": original_host_pid,
        "original_guest_pid": game["guest_pid"],
        "system_server_host_pid": server["host_pid"],
        "kernel_original_self_sigkill_boot_ns": kill_ns,
        "all_captured_to_system_server_binder_events": total_binder,
        "original_process_to_system_server_events_before_sigkill": len(sender_events),
        "events": sender_events[-60:],
        "activity_finish_requester_identified": False,
        "interface_descriptor_identified": False,
        "root_cause_identified": False,
        "playable_gameplay_verified": False,
        "real_player_esp_verified": False,
        "limitations": [
            "Binder kernel events identify sender/destination and numeric code only, not AIDL descriptor.",
            "A call before SIGKILL does not prove it was finishActivity() or caused the exit.",
            "A missing trace event may reflect system configuration, alternate code path or missed capture.",
            "No APK modifications, integrity bypass, game-process injection or physical device.",
        ],
    }
    return result

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("original_evidence_dir", type=Path)
    args = p.parse_args()
    result = inspect(args.original_evidence_dir)
    (args.original_evidence_dir / "original_binder_finish_window.json").write_text(
        json.dumps(result, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(result, indent=2))
    return 0 if result["status"] == "ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_OBSERVED" else 1

if __name__ == "__main__":
    raise SystemExit(main())
