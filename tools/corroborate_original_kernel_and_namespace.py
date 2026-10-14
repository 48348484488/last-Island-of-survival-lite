#!/usr/bin/env python3
"""Verify original Oxide kill(SIGKILL) observed in SAME run, on host and guest.

Evidence sources are independent: live /proc NSpid, kernel syscall trace,
Android logcat Activity self-signal, and ApplicationExitInfo exact PID.
Read-only, no injected hooks or game modifications.
"""
import argparse
import json
import re
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
    sample_boot_ns = mapping.get("observed_host_boot_ns")
    namespace_ok = (
        mapping.get("status") == "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN"
        and type(host) is int and host > 1
        and mapping.get("guest_pid") == guest
        and isinstance(chain, list) and len(chain) >= 2
        and chain[0] == host and chain[-1] == guest
        and mapping.get("original_process_cmdline") == GAME
        and type(mapping.get("host_start_ticks")) is int
        and mapping["host_start_ticks"] > 0
        and type(sample_boot_ns) is int and sample_boot_ns > 0
    )
    events = extract_events(trace)
    for item in events:
        match = re.search(r"\bboot_ns=(\d+)", item["comm"])
        item["kernel_boot_ns"] = int(match.group(1)) if match else None
        if match:
            item["comm"] = item["comm"].split(" boot_ns=", 1)[0].strip()
        delta = ((item["kernel_boot_ns"] - sample_boot_ns) / 1_000_000
                 if namespace_ok and item["kernel_boot_ns"] is not None else None)
        item["milliseconds_after_pid_sample"] = round(delta, 3) if delta is not None else None
    matches = [
        item for item in events
        if namespace_ok and original_exit
        and item["host_sender"] == host and item["signal"] == 9
        and item["syscall"] == "kill" and item["target_pid"] == guest
        # Validate independent kernel process/thread name, not a random
        # SIGKILL from ActivityManager or another host process.
        and "esurvivalisland" in item["comm"].lower()
        and item["milliseconds_after_pid_sample"] is not None
        and 0 <= item["milliseconds_after_pid_sample"] <= 15000
    ]
    status = ("SAME_RUN_TIMESTAMPED_HOST_KERNEL_GUEST_SELF_SIGKILL_PROVEN"
              if len(matches) >= 1 else "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")
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
        "kernel_clock_temporal_window_verified": bool(matches),
        "first_matched_event_after_host_pid_sample_ms": (
            min(item["milliseconds_after_pid_sample"] for item in matches)
            if matches else None
        ),
        "self_sigkill_exit_info_status9": bool(original_exit),
        "activity_finish_requester_identified": False,
        "original_security_reason": "UNKNOWN",
        "playable_game_verified": False,
        "real_player_esp_verified": False,
        "limitations": [
            "On success, kernel kill(SIGKILL) sender and target match the original same-run NSpid.",
            "Temporal requirement: CLOCK_BOOTTIME event must be 0-15s after the live host mapping.",
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
    return 0 if result["status"] == "SAME_RUN_TIMESTAMPED_HOST_KERNEL_GUEST_SELF_SIGKILL_PROVEN" else 1


if __name__ == "__main__":
    raise SystemExit(main())
