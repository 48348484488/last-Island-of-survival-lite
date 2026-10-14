#!/usr/bin/env python3
"""Correlate real Oxide Android PID logcat with Linux host SIGKILL tracepoints.

Never equate host PID and Android namespace PID without /proc NSpid evidence.
No APK patch, injection, or security-trigger inference.
"""
import argparse
import json
import re
from pathlib import Path

EVENT = re.compile(r"KERNEL_SIGKILL\s+syscall=(\w+)\s+host_sender=(\d+)\s+host_tid=(\d+)\s+target_(pid|tgid)=(-?\d+)(?:\s+target_tid=(-?\d+))?\s+signal=(\d+)\s+comm=([^\\\r\n]+)")

def extract_events(trace):
    out = []
    for line in trace.splitlines():
        m = EVENT.search(line)
        if not m:
            continue
        op, host, tid, kind, target, target_tid, sig, name = m.groups()
        e = dict(syscall=op, host_sender=int(host), host_tid=int(tid),
                 target_kind=kind, target_pid=int(target), signal=int(sig),
                 comm=name.strip())
        if target_tid is not None:
            e["target_tid"] = int(target_tid)
        out.append(e)
    return out

def get_mappings(data):
    mapping, ambiguous = {}, set()
    for row in data.get("samples", []):
        try:
            host, guest = int(row["host_pid"]), int(row["guest_pid"])
            chain = [int(x) for x in row["nspid"]]
            name = str(row.get("name", "")).strip()
        except (KeyError, ValueError, TypeError):
            continue
        if len(chain) < 2 or chain[0] != host or chain[-1] != guest:
            continue
        value = dict(guest_pid=guest, name=name, nspid=chain)
        if host in mapping and mapping[host] != value:
            ambiguous.add(host)
        else:
            mapping[host] = value
    for h in ambiguous:
        mapping.pop(h, None)
    return mapping, sorted(ambiguous)

def audit(trace, forensic, samples=None):
    game_pid = forensic.get("initial_activity_pid")
    logcat_self = (forensic.get("classification") == "ORIGINAL_ACTIVITY_SELF_SIGKILL"
                   and forensic.get("first_self_sigkill") is not None)
    mapping, ambiguous = get_mappings(samples or {})
    events, confirmed = extract_events(trace), []
    for e in events:
        sender = mapping.get(e["host_sender"])
        e["mapped_sender_guest_pid"] = sender["guest_pid"] if sender else None
        e["sender_name"] = sender["name"] if sender else None
        targets_game = (game_pid is not None and e["syscall"] == "kill" and
                        e["target_pid"] == int(game_pid) and e["signal"] == 9)
        if targets_game and sender and sender["guest_pid"] == int(game_pid) and "esurvivalisland" in sender["name"].lower():
            e["classification"] = "MAPPED_GAME_SELF_SIGNAL"
            confirmed.append(e)
        elif sender and (game_pid is None or sender["guest_pid"] != int(game_pid)):
            e["classification"] = "OTHER_PROCESS"
        elif targets_game:
            e["classification"] = "GAME_PID_TARGET_SENDER_UNVERIFIED"
        else:
            e["classification"] = "UNATTRIBUTED_EVENT"
    status = ("GAME_SELF_SIGNAL_HOST_PID_CORROBORATED" if logcat_self and confirmed
              else "GAME_SELF_SIGKILL_LOGCAT_ONLY" if logcat_self
              else "GAME_SELF_SIGKILL_NOT_CONFIRMED")
    return dict(status=status, game_activity_guest_pid=game_pid,
                game_logcat_self_sigkill=bool(logcat_self),
                host_guest_pid_mapping_confirmed=bool(confirmed),
                captured_kernel_sigkill_event_count=len(events),
                game_self_kill_kernel_event_count=len(confirmed),
                host_pid_mapping_ambiguous=ambiguous, events=events,
                specific_security_cause_proven=False, gameplay_verified=False,
                limitations=[
                  "A host PID is not an Android PID unless /proc NSpid was captured while live.",
                  "A kernel SIGKILL and Android log event do not reveal the originating C++ caller.",
                  "The particular integrity, licensing, or security decision was not identified.",
                  "Original gameplay and any multiplayer overlay were not verified."
                ])

def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--trace", required=True)
    ap.add_argument("--forensic", required=True)
    ap.add_argument("--nspid")
    ap.add_argument("--out", required=True)
    a = ap.parse_args()
    ns = json.loads(Path(a.nspid).read_text()) if a.nspid and Path(a.nspid).is_file() else None
    result = audit(Path(a.trace).read_text(errors="replace"),
                   json.loads(Path(a.forensic).read_text()), ns)
    dest = Path(a.out)
    dest.mkdir(parents=True, exist_ok=True)
    (dest / "kernel_sigkill_correlation.json").write_text(json.dumps(result, indent=2) + "\n")
    (dest / "kernel_sigkill_correlation.md").write_text(
        "# Original Oxide: ARM64 kernel signal evidence\n\n"
        + "- Status: **" + result["status"] + "**\n"
        + "- Android Activity PID: " + str(result["game_activity_guest_pid"]) + "\n"
        + "- Kernel SIGKILL events: " + str(result["captured_kernel_sigkill_event_count"]) + "\n"
        + "- Verified original sender event(s): " + str(result["game_self_kill_kernel_event_count"]) + "\n"
        + "- Security root cause: **unknown**\n"
        + "- Gameplay: **not verified**\n")
    print(result["status"])

if __name__ == "__main__":
    main()
