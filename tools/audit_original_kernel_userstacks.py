#!/usr/bin/env python3
"""Read-only attribution of Android original Oxide kernel user stacks.

A stack is a snapshot at an observed syscall/IPC operation, NOT proof of
what initiated an integrity decision. Correlate raw PC addresses with exact
/proc/<host-pid>/maps captured before exit, without accessing process memory.
"""
import argparse
import json
import re
from pathlib import Path

from audit_controlled_activity_finish_node import assess as binder_control
from corroborate_original_kernel_and_namespace import prove as kernel_proof

BLOCK = re.compile(
    r"OXIDE_USERSTACK_BEGIN kind=(binder|kill) host_sender=(\d+) "
    r"host_tid=(\d+) (?:transaction=(\d+) target_node=(\d+) code=(\d+)|"
    r"target_pid=(\d+) signal=(\d+)) boot_ns=(\d+)\r?\n"
    r"(.*?)\r?\nOXIDE_USERSTACK_END kind=\1", re.S
)
MAP = re.compile(
    r"^([0-9a-f]+)-([0-9a-f]+)\s+([rwxps-]+)\s+"
    r"([0-9a-f]+)\s+\S+\s+\d+\s*(.*)$"
)
PC = re.compile(r"^\s*((?:0x)?[0-9a-fA-F]{6,16})\s*$", re.M)


def mappings(path):
    found = []
    for line in path.read_text(errors="replace").splitlines():
        m = MAP.match(line)
        if not m:
            continue
        first, last, perms, offset, name = m.groups()
        if "x" in perms:
            found.append({
                "start": int(first, 16), "end": int(last, 16),
                "file_offset": int(offset, 16), "name": name.strip(),
            })
    return found


def resolve_frame(ip, memory):
    for m in memory:
        if m["start"] <= ip < m["end"]:
            return {
                "pc": "0x%x" % ip,
                "mapped_image": m["name"],
                "file_offset_hex": "0x%x" % (ip-m["start"]+m["file_offset"]),
                "executable_mapping": True,
            }
    return {"pc": "0x%x" % ip, "mapped_image": None,
            "file_offset_hex": None, "executable_mapping": False}


def analyze(root):
    root = Path(root)
    read = lambda name: json.loads((root/name).read_text())
    game = read("host_pid_namespace.json")
    system_server = read("host_system_server_namespace.json")
    forensic = read("forensic_summary.json")
    trace = (root/"kernel_sigkill_trace.txt").read_text(errors="replace")
    kernel = kernel_proof(game, trace, forensic,
                          (root/"exit_info.txt").read_text(errors="replace"))
    control = binder_control(root)
    maps_file = root/"original_host_exec_maps.txt"
    memory = mappings(maps_file) if maps_file.exists() else []
    pid = game.get("host_pid")
    guest = game.get("guest_pid")
    server = system_server.get("host_pid")
    control_events = control.get("original_last20ms_code15_events") or []
    known_nodes = control.get("original_last20ms_binder_code15_nodes") or []
    known_ids = {(x["transaction"], x["kernel_boot_ns"])
                 for x in control_events}
    kernel_events = kernel.get("matched_syscalls") or []
    known_kill_ns = {x["kernel_boot_ns"] for x in kernel_events}
    # Two independently attached tracepoints can fire microseconds apart on
    # the exact same syscall. Match event identity AND a strict <200us window.
    MAX_INDEPENDENT_PROBE_SKEW_NS = 200_000
    proven = (kernel.get("status") ==
              "SAME_RUN_TIMESTAMPED_HOST_KERNEL_GUEST_SELF_SIGKILL_PROVEN"
              and control.get("status") ==
              "CONTROLLED_SETTINGS_FINISH_BINDER_NODE_MATCHES_ORIGINAL_FINAL_CODE15"
              and control.get("original_host_pid") == pid
              and control.get("original_guest_pid") == guest
              and control.get("system_server_host_pid") == server
              and len(known_nodes) == 1
              and bool(known_kill_ns)
              and game.get("status") == "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN")
    captured = []
    for match in BLOCK.finditer(trace):
        (kind, sender, tid, transaction, target_node, code,
         target_pid, signal, ns, stack_text) = match.groups()
        sender, tid, ns = int(sender), int(tid), int(ns)
        if not proven or sender != pid:
            continue
        if kind == "binder":
            if ((int(code), int(target_node)) != (15, known_nodes[0])
                or not any(int(transaction)==txn and abs(ns-when)<=MAX_INDEPENDENT_PROBE_SKEW_NS
                           for txn,when in known_ids)):
                continue
        else:
            if (int(signal) != 9 or int(target_pid) != guest or
                not any(abs(ns-when)<=MAX_INDEPENDENT_PROBE_SKEW_NS for when in known_kill_ns)):
                continue
        pcs = [int(x,16) for x in PC.findall(stack_text)]
        frames = [resolve_frame(pc,memory) for pc in pcs if pc != 0]
        captured.append({
            "kind": kind,
            "original_host_pid": sender,
            "original_host_tid": tid,
            "kernel_boot_ns": ns,
            "transaction_id": int(transaction) if transaction else None,
            "binder_node": int(target_node) if target_node else None,
            "guest_signal_target": int(target_pid) if target_pid else None,
            "raw_frame_count": len(frames),
            "mapped_frame_count": sum(bool(x["mapped_image"]) for x in frames),
            "frames": frames[:32],
        })
    binder = [x for x in captured if x["kind"]=="binder"]
    kills = [x for x in captured if x["kind"]=="kill"]
    have_both = bool(binder and kills)
    both_mapped = (have_both and
                   any(x["mapped_frame_count"]>0 for x in binder) and
                   any(x["mapped_frame_count"]>0 for x in kills))
    result = {
        "status": ("ORIGINAL_BINDER_AND_SIGKILL_CALLSITE_MODULES_CORRELATED"
                   if proven and both_mapped else
                   "ORIGINAL_USERSPACE_STACK_INCOMPLETE_OR_UNMAPPED"),
        "real_original_pid_and_kernel_binder_control_verified": bool(proven),
        "original_host_pid": pid,
        "original_guest_pid": guest,
        "system_server_host_pid": server,
        "binder_endpoint_node": known_nodes[0] if proven else None,
        "executable_maps_found": len(memory),
        "binder_stack_events_attributed": len(binder),
        "sigkill_stack_events_attributed": len(kills),
        "attributed_stacks": captured,
        "both_stacks_contain_mapped_executable_pc": bool(both_mapped),
        "independent_kernel_probe_time_skew_limit_ns": MAX_INDEPENDENT_PROBE_SKEW_NS,
        "internal_game_function_identified": False,
        "source_of_security_decision_identified": False,
        "android_activity_survived_35_seconds": False,
        "real_esp_verified": False,
        "original_apks_modified": False,
        "limitations": [
            "Kernel user stacks may be truncated if Unity/IL2CPP omit frame pointers.",
            "ELF file offsets from process maps do not themselves identify function names.",
            "Top frames at a syscall may be libc, libbinder or ART and not the initiating game function.",
            "A user stack at finish Binder IPC or kill is not evidence of why the game decided to exit.",
            "Tracer and maps are from host Linux only; no code injection, spoofing or game patching.",
        ],
    }
    return result


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("dir",type=Path)
    args=p.parse_args()
    result=analyze(args.dir)
    (args.dir/"original_kernel_userstack_modules_verdict.json").write_text(
        json.dumps(result,indent=2)+"\n")
    print(json.dumps(result,indent=2))
    return 0 if result["status"]=="ORIGINAL_BINDER_AND_SIGKILL_CALLSITE_MODULES_CORRELATED" else 1


if __name__=="__main__":
    raise SystemExit(main())
