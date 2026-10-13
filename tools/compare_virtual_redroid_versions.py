#!/usr/bin/env python3
"""Compare actual, unchanged original Oxide launch on native Redroid ARM64 images.

An environment compatibility report, not a security bypass or a playable-game
claim. Data must come from each real Android process and its Android logs.
"""
import argparse
import json
from pathlib import Path

VERSIONS = ("12", "14")

def summarize(states):
    rows = []
    for version in VERSIONS:
        state = states.get(version, {})
        install = state.get("install", "NOT_MEASURED")
        forensic = state.get("forensic", {})
        pid = forensic.get("initial_activity_pid")
        termination = forensic.get("classification", "NOT_CLASSIFIED")
        if install != "SUCCESS":
            outcome = "GAME_INSTALL_NOT_CONFIRMED"
        elif termination == "ORIGINAL_ACTIVITY_SELF_SIGKILL":
            outcome = "ORIGINAL_SELF_SIGKILL"
        elif pid is None:
            outcome = "ORIGINAL_ACTIVITY_START_NOT_CONFIRMED"
        elif state.get("initial_pid_alive_10s") is not True:
            outcome = "GAME_ACTIVITY_NOT_ALIVE_AT_10S"
        elif state.get("activity_resumed_10s") is not True:
            outcome = "ACTIVITY_NOT_RESUMED_AT_10S"
        else:
            outcome = "ACTIVITY_ALIVE_AT_10S_GAMEPLAY_UNVERIFIED"
        rows.append({
            "android_major":version,
            "host_architecture":state.get("host_architecture","UNKNOWN"),
            "guest_cpu_abi":state.get("abi","NOT_MEASURED"),
            "gpu_mode":"guest_software",
            "boot":state.get("boot","NOT_CONFIRMED"),
            "original_apk_install":install,
            "original_game_initial_pid_observed":pid is not None,
            "original_exit":termination,
            "game_pid_alive_10s":state.get("initial_pid_alive_10s",False),
            "activity_resumed_10s":state.get("activity_resumed_10s",False),
            "outcome":outcome,
            "play_services":state.get("play_services","NOT_MEASURED"),
            "play_integrity":"NOT_ATTESTED",
            "gameplay_verified":False,
            "real_player_esp_verified":False,
        })
    return {
        "experiment":"UNMODIFIED_OXIDE_NATIVE_ARM64_ANDROID_VERSION_COMPARISON",
        "versions":rows,
        "both_android_images_booted":all(x["boot"]=="PASS" for x in rows),
        "original_gameplay_verified":False,
        "device_integrity_verified":False,
        "limits":[
            "An Android process surviving ten seconds would not establish multiplayer or ESP.",
            "Different Android versions can have different dependencies and compatibility behavior.",
            "An absent Play service is not proof it triggered the game's own security response.",
            "No security mechanism, package name, build identity, Play Integrity token or APK was modified.",
        ],
    }

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("input",type=Path)
    p.add_argument("--out",type=Path,required=True)
    args=p.parse_args()
    states=json.loads(args.input.read_text(encoding="utf-8"))
    result=summarize(states)
    args.out.mkdir(parents=True,exist_ok=True)
    (args.out/"virtual_android_version_comparison.json").write_text(
        json.dumps(result,indent=2)+"\n",encoding="utf-8")
    lines=["# Oxide original — native ARM64 virtual Android versions",
           "","Neither physical devices nor native code translation were used.",
           "","| Android | Boot | Install | Exit | PID 10s | Activity 10s | Outcome |",
           "|---|---|---|---|---|---|---|"]
    for r in result["versions"]:
        lines.append("| "+r["android_major"]+" | "+r["boot"]+
                     " | "+r["original_apk_install"]+" | "+r["original_exit"]+
                     " | "+str(r["game_pid_alive_10s"])+" | "+
                     str(r["activity_resumed_10s"])+" | "+r["outcome"]+" |")
    lines+=["","No Play Integrity attestation, gameplay, remote player telemetry, or ESP is proven."]
    (args.out/"virtual_android_version_comparison.md").write_text("\n".join(lines)+"\n")
    print("\n".join(lines))
    if not result["both_android_images_booted"]:
        raise SystemExit("At least one native Android container failed to boot: test inconclusive")

if __name__=="__main__":
    main()
