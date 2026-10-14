#!/usr/bin/env python3
"""Verify original Oxide kill(SIGKILL) observed in SAME run, on host and guest.

Evidence sources are independent: live /proc NSpid, kernel syscall trace,
Android logcat Activity self-signal, and ApplicationExitInfo exact PID.
Read-only, no injected hooks or game modifications.
"""
import argparse
import json
from pathlib import Path

from correlate_kernel_sigkill import extract_events
from audit_original_exit_order import app_exit_entry

GAME = "com.catsbit.oxidesurvivalisland"


def prove(mapping, trace, forensic, exit_info):
    guest = forensic.get("initial_activity_pid")
    original_exit = (
        type(guest) is int and guest > 0
        and forensic.get("classification") == "ORIGINAL_ACTIVITY_SELF_SIGKILL"
        and forensic.get("first_self_sigkill") is not None
        and forensic.get("activity_finish") is not None
        and forensic["activity_finish"] <= forensic["first_self_sigkill"]
        and app_exit_entry(exit_info, guest)["signal_9_recorded"]
    ) if type(guest) is int and guest > 0 else False
    chain = mapping.get("nspid_chain", [])
    host = mapping.get("host_pid")
    namespace_ok = (
        mapping.get("status") == "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN"
        and type(host) is int and host > 1
        and mapping.get("guest_pid") == guest
        and isinstance(chain, list) and len(chain) >= 2
        and chain[0] == host and chain[-1] == guest
        and mapping.get("original_process_cmdline") == GAME
        and type(mapping.get("host_start_ticks")) is int
        and mapping["host_start_ticks"] > 0
    )
    events = extract_events(trace)
    matches = [
        item for item in events
        if namespace_ok and original_exit
        and item["host_sender"] == host and item["signal"] == 9
        and item["syscall"] == "kill" and item["target_pid"] == guest
        # Validate independent kernel process/thread name, not a random
        # SIGKILL from ActivityManager or another host process.
        and "esurvivalisland" in item["comm"].lower()
    ]
    status = ("SAME_RUN_HOST_KERNEL_GUEST_ORIGINAL_SELF_SIGKILL_PROVEN"
              if len(matches) >= 1 else "SAME_RUN_KERNEL_SENDER_NOT_PROVEN")
    return {
        "status": status,
        "android_activity_guest_pid": guest,
        "linux_host_pid": host if namespace_ok else None,
        "nspid_chain": chain if namespace_ok else None,
        "live_namespace_mapping_verified": bool(namespace_ok),
        "android_original_activity_finish_before_self_signal": bool(original_exit),
        "kernel_syscall_event_count": len(events),
        "matching_real_original_kernel_kill_syscalls": len(matches),
        "matched_syscalls": matches,
        "self_sigkill_exit_info_status9": bool(original_exit),
        "activity_finish_requester_identified": False,
        "original_security_reason": "UNKNOWN",
        "playable_game_verified": False,
        "real_player_esp_verified": False,
        "limitations": [
            "The Linux kernel confirmed kill(SIGKILL) emitter and target in the same run.",
            "This does not recover who requested Activity.finish() or the C++ kill caller.",
            "Host NSpid evidence was collected before process death; concurrent API reads are not atomic.",
            "The Android image, game APKs and integrity systems were not patched, bypassed or spoofed.",
        ],
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("original_evidence_dir", type=Path)
    args = parser.parse_args()
    root = args.original_evidence_dir
    mapping = json.loads((root / "host_pid_namespace.json").read_text())
    forensic = json.loads((root / "forensic_summary.json").read_text())
    trace = (root / "kernel_sigkill_trace.txt").read_text(errors="replace")
    exit_info = (root / "exit_info.txt").read_text(errors="replace")
    result = prove(mapping, trace, forensic, exit_info)
    (root / "same_run_kernel_namespace_original_verdict.json").write_text(
        json.dumps(result, indent=2) + "\n")
    print(json.dumps(result, indent=2))
    return 0 if result["status"] == "SAME_RUN_HOST_KERNEL_GUEST_ORIGINAL_SELF_SIGKILL_PROVEN" else 1


if __name__ == "__main__":
    raise SystemExit(main())
