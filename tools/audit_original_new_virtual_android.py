#!/usr/bin/env python3
"""Classify an authentic, unchanged Oxide start on new native ARM64 Android guests.

This is a lifecycle audit. A 35-second checkpoint cannot prove multiplayer
gameplay, Play Integrity, player coordinates, or any security mechanism.
"""
import argparse
import json
from pathlib import Path

SUPPORTED = ("15", "16")

def valid_pid(p):
    return type(p) is int and p > 0

def evaluate(version, data):
    version = str(version)
    if version not in SUPPORTED:
        raise ValueError("Android version not in this experiment")
    data = data if isinstance(data, dict) else {}
    forensic = data.get("forensic") if isinstance(data.get("forensic"), dict) else {}
    lifecycle = data.get("lifecycle_recovery") if isinstance(data.get("lifecycle_recovery"), dict) else {}
    pid = forensic.get("initial_activity_pid")
    source = "AM_PROC_START_FORENSIC"
    if not valid_pid(pid):
        pid = lifecycle.get("selected_pid")
        source = "LIFECYCLE_ACTIVITY_CORRELATION"
        if lifecycle.get("pid_confirms_activity_process") is not True:
            pid = None
    if not valid_pid(pid):
        pid = None
        source = "ACTIVITY_PID_UNVERIFIED"
    boot = data.get("boot") == "PASS" and "arm64-v8a" in str(data.get("abi", ""))
    installed = data.get("install") == "SUCCESS"
    selfkill = (
        forensic.get("classification") == "ORIGINAL_ACTIVITY_SELF_SIGKILL"
        and valid_pid(forensic.get("initial_activity_pid"))
        and forensic.get("first_self_sigkill") is not None
    ) or (
        lifecycle.get("pid_confirms_activity_process") is True
        and lifecycle.get("selected_pid") == pid
        and lifecycle.get("self_sigkill_attributed_to_original_activity") is True
    )
    checkpoint_10 = data.get("initial_pid_alive_10s") is True and data.get("activity_resumed_10s") is True
    checkpoint_35 = data.get("initial_pid_alive_35s") is True and data.get("activity_resumed_35s") is True
    if not boot:
        verdict = "VIRTUAL_ANDROID_BOOT_NOT_VERIFIED"
    elif not installed:
        verdict = "ORIGINAL_APK_INSTALL_NOT_VERIFIED"
    elif pid is None:
        verdict = "ORIGINAL_ACTIVITY_PID_NOT_VERIFIED"
    elif selfkill:
        verdict = "ORIGINAL_ACTIVITY_SELF_SIGKILL"
    elif checkpoint_10 and checkpoint_35:
        verdict = "ACTIVITY_35S_CHECKPOINTS_PASS_NOT_GAMEPLAY"
    else:
        verdict = "ORIGINAL_ACTIVITY_35S_NOT_CONFIRMED"
    return {
        "android_version": version,
        "native_arm64_virtual_boot_confirmed": boot,
        "four_original_apks_installed": installed,
        "original_activity_pid": pid,
        "pid_attribution": source,
        "original_activity_self_sigkill": bool(selfkill),
        "pid_and_activity_seen_at_10s": checkpoint_10,
        "pid_and_activity_seen_at_35s": checkpoint_35,
        "status": verdict,
        "gameplay_verified": False,
        "original_security_cause": "UNKNOWN",
        "play_integrity_verdict": "NOT_ATTESTED",
        "real_player_esp_verified": False,
        "limitations": [
            "An Android PID with a resumed Activity at checkpoints is not proof of playable gameplay.",
            "A PID could be reused between snapshots; full process identity continuity is not established.",
            "A self-SIGKILL does not identify the library or security decision that caused it.",
            "No device identity, security controls or original APK contents were modified.",
        ],
    }

def markdown(result):
    lines = ["# Original unchanged Oxide: native virtual Android " + result["android_version"],
             "", "Outcome: **" + result["status"] + "**",
             "", "| Check | Observed |", "|---|---|"]
    for name in ("native_arm64_virtual_boot_confirmed", "four_original_apks_installed",
                 "original_activity_pid", "pid_attribution", "original_activity_self_sigkill",
                 "pid_and_activity_seen_at_10s", "pid_and_activity_seen_at_35s"):
        lines.append("| " + name + " | " + str(result[name]) + " |")
    lines += ["", "**Playable gameplay: NOT VERIFIED**",
              "Security cause: UNKNOWN; Play Integrity: NOT ATTESTED.",
              "No physical phone and no APK changes."]
    return "\n".join(lines) + "\n"

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--version", required=True, choices=SUPPORTED)
    p.add_argument("--input", type=Path, required=True)
    p.add_argument("--out", type=Path, required=True)
    opts = p.parse_args()
    states = json.loads(opts.input.read_text(encoding="utf-8"))
    result = evaluate(opts.version, states.get(opts.version))
    opts.out.mkdir(parents=True, exist_ok=True)
    (opts.out / "original_game_lifecycle.json").write_text(json.dumps(result, indent=2) + "\n")
    (opts.out / "original_game_lifecycle.md").write_text(markdown(result))
    print(markdown(result))
    if not (result["native_arm64_virtual_boot_confirmed"] and result["four_original_apks_installed"]):
        raise SystemExit("Native boot or unchanged original game installation not established")

if __name__ == "__main__":
    main()
