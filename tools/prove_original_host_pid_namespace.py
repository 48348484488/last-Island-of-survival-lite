#!/usr/bin/env python3
"""Correlate ORIGINAL Oxide Android guest PID with real Linux host PID.

Read-only Linux /proc observation in a disposable native ARM64 CI host.
No ptrace, hooks, app modification, security evasion or synthetic PID mapping.
"""
import argparse
import json
import os
import re
import time
from pathlib import Path

GAME = "com.catsbit.oxidesurvivalisland"
NSPID = re.compile(r"^NSpid:\s*(\d+(?:\s+\d+)*)\s*$", re.M)


def details(procroot, pid):
    folder = procroot / str(pid)
    try:
        status = (folder / "status").read_text(errors="replace")
        match = NSPID.search(status)
        nspids = [int(v) for v in match.group(1).split()] if match else []
        ns = os.readlink(folder / "ns" / "pid")
        command = (folder / "cmdline").read_bytes().split(b"\x00", 1)[0].decode(
            "utf-8", errors="replace"
        )
        # /proc/PID/stat comm may contain spaces and parentheses. Field 22
        # starts at index 19 after stripping "PID (comm) " (field 3 is index 0).
        stat = (folder / "stat").read_text(errors="replace")
        start_ticks = int(stat.rsplit(") ", 1)[1].split()[19])
        return {"host_pid": pid, "nspid": nspids, "pid_namespace": ns,
                "cmdline": command, "start_ticks": start_ticks}
    except (OSError, ValueError, IndexError, TypeError):
        return None


def inspect(procroot, container_init_host_pid, guest_pid, package=GAME):
    procroot = Path(procroot)
    parent = details(procroot, container_init_host_pid)
    if not parent or parent["host_pid"] != (parent["nspid"][0] if parent["nspid"] else None):
        return {"status": "CONTAINER_PID1_NOT_VERIFIED", "matches": 0}
    # The actual guest /proc/1 must be Android container init. Do not
    # accidentally compare process IDs from another running Android container.
    if len(parent["nspid"]) < 2 or parent["nspid"][-1] != 1:
        return {"status": "CONTAINER_GUEST_INIT_NOT_VERIFIED", "matches": 0}
    hits = []
    for entry in procroot.iterdir():
        if not entry.name.isdecimal():
            continue
        host_pid = int(entry.name)
        if host_pid == container_init_host_pid:
            continue
        p = details(procroot, host_pid)
        if (p and p["nspid"] and p["nspid"][0] == host_pid
                and len(p["nspid"]) >= 2
                and p["nspid"][-1] == guest_pid
                and p["pid_namespace"] == parent["pid_namespace"]
                and p["cmdline"] == package):
            hits.append(p)
    if len(hits) != 1:
        return {"status": "ORIGINAL_PROCESS_HOST_PID_NOT_UNIQUELY_CORRELATED",
                "matches": len(hits), "guest_pid": guest_pid,
                "container_init_host_pid": container_init_host_pid,
                "pid_namespace": parent["pid_namespace"]}
    p = hits[0]
    return {
        "status": "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN",
        "guest_pid": guest_pid,
        "host_pid": p["host_pid"],
        "nspid_chain": p["nspid"],
        "host_start_ticks": p["start_ticks"],
        "pid_namespace": p["pid_namespace"],
        "container_init_host_pid": container_init_host_pid,
        "container_init_nspid_chain": parent["nspid"],
        "original_process_cmdline": p["cmdline"],
        "method": "HOST_PROC_NSPID_AND_NAMESPACE_AND_EXACT_PROCESS_CMDLINE",
        "not_gameplay_proof": True,
        "security_reason": "NOT_IDENTIFIED",
    }


def crosscheck(mapping, forensic):
    pid = forensic.get("initial_activity_pid")
    same = (
        type(pid) is int and pid > 0
        and forensic.get("classification") == "ORIGINAL_ACTIVITY_SELF_SIGKILL"
        and forensic.get("first_self_sigkill") is not None
        and mapping.get("status") == "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN"
        and mapping.get("guest_pid") == pid
        and type(mapping.get("host_pid")) is int
        and mapping["host_pid"] > 0
    )
    return {
        "status": "HOST_AND_GUEST_PID_LINKED_TO_ORIGINAL_SELF_SIGKILL"
        if same else "HOST_AND_GUEST_PID_EXIT_EVIDENCE_INCOMPLETE",
        "original_guest_activity_pid": pid,
        "linux_host_pid_for_same_original_process": mapping.get("host_pid") if same else None,
        "host_nspid_chain": mapping.get("nspid_chain") if same else None,
        "host_process_start_ticks": mapping.get("host_start_ticks") if same else None,
        "original_process_self_sigkill_in_android_logcat": (
            forensic.get("classification") == "ORIGINAL_ACTIVITY_SELF_SIGKILL"
        ),
        "host_pid_live_when_observed": mapping.get("status") == "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN",
        "host_kernel_signal_emitter_at_same_instant_observed": False,
        "finish_activity_requester_identified": False,
        "root_cause_established": False,
        "original_apks_modified": False,
        "limitation": (
            "A pre-exit /proc NSpid mapping and post-exit Android self-SIGKILL are "
            "separate observations; this identifies the host PID of the original "
            "process during its lifetime, not the caller of Activity.finish()."
        ),
    }


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--container-init-host-pid", type=int, required=True)
    p.add_argument("--guest-pid", type=int, required=True)
    p.add_argument("--out", type=Path, required=True)
    p.add_argument("--procroot", type=Path, default=Path("/proc"))
    p.add_argument("--attempts", type=int, default=6)
    p.add_argument("--interval-ms", type=int, default=80)
    p.add_argument("--forensic", type=Path)
    args = p.parse_args()
    if args.container_init_host_pid <= 1 or args.guest_pid <= 1:
        p.error("Live host container and guest PID must be > 1")
    if not (1 <= args.attempts <= 100 and 0 <= args.interval_ms <= 2000):
        p.error("Invalid read-only probe limits")
    result = {}
    for attempt in range(1, args.attempts + 1):
        result = inspect(args.procroot, args.container_init_host_pid, args.guest_pid)
        result["attempts_used"] = attempt
        result["observed_host_unix_ns"] = time.time_ns()
        if result["status"] == "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN":
            break
        if attempt < args.attempts:
            time.sleep(args.interval_ms / 1000)
    if args.forensic:
        result = {"mapping": result, "crosscheck": crosscheck(
            result, json.loads(args.forensic.read_text()))}
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + "\n")
    print(json.dumps(result, indent=2))
    status = result.get("crosscheck", result.get("mapping", result)).get("status")
    return 0 if status in (
        "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN",
        "HOST_AND_GUEST_PID_LINKED_TO_ORIGINAL_SELF_SIGKILL",
    ) else 1


if __name__ == "__main__":
    raise SystemExit(main())
