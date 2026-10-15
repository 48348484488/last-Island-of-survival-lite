#!/usr/bin/env python3
"""Fail-closed proof that a *separate diagnostic app* invoked ARM64 JNI on x86.

ABI names or libndk_translation.so presence are necessary but not sufficient.
No original Oxide package, APK, anti-cheat, attestation, or game assets are used.
"""
import json
import re
from pathlib import Path

TAG_OK = re.compile(
    r"(?m)^\S+\s+\S+\s+(?P<pid>\d+)\s+\d+\s+I\s+"
    r"OXIDE_ARM64_BRIDGE\s*:\s*ARM64_JNI_EXECUTION_VERIFIED value=42\b")
TAG_FAIL = re.compile(r"ARM64_JNI_(?:EXECUTION_FAILED|WRONG_RESULT)")
PACKAGE = "org.oxideprobe"


def assess(folder):
    root = Path(folder)

    def read(name):
        try:
            return (root / name).read_text(errors="replace").strip()
        except OSError:
            return ""

    boot = read("boot.txt") == "1"
    abi = read("abi.txt") == "x86_64"
    abis = read("abilist64.txt").split(",")
    arm64_advertised = "arm64-v8a" in abis
    nativebridge = read("native-bridge.txt")
    bridge_configured = nativebridge == "libndk_translation.so"
    installed = "Success" in read("diagnostic-install.txt").splitlines()
    launch = read("diagnostic-launch.txt")
    started = "Status: ok" in launch and PACKAGE in launch
    pid = read("diagnostic-pid.txt")
    pid_valid = bool(re.fullmatch(r"[1-9]\d*", pid))
    log = read("diagnostic-logcat.txt")
    ok_pids = [m["pid"] for m in TAG_OK.finditer(log)]
    error_tag = bool(TAG_FAIL.search(log))
    native_jni = (boot and abi and arm64_advertised and bridge_configured
                  and installed and started and pid_valid
                  and pid in ok_pids and not error_tag)
    if not boot:
        status = "BLOCKED_AVD_NOT_BOOTED"
    elif not bridge_configured or not arm64_advertised or not abi:
        status = "BLOCKED_NO_VALID_ARM64_NATIVE_BRIDGE"
    elif not installed:
        status = "BLOCKED_DIAGNOSTIC_APP_NOT_INSTALLED"
    elif not started:
        status = "BLOCKED_DIAGNOSTIC_ACTIVITY_NOT_STARTED"
    elif native_jni:
        status = "ARM64_JNI_EXECUTED_VIA_NATIVE_BRIDGE_ORIGINAL_GAME_UNTESTED"
    else:
        status = "ARM64_JNI_EXECUTION_NOT_VERIFIED"

    return {
        "schema_version": 1,
        "status": status,
        "official_android_booted": boot,
        "host_abi_x86_64": abi,
        "arm64_abi_advertised": arm64_advertised,
        "native_bridge_configured": bridge_configured,
        "diagnostic_package_installed": installed,
        "diagnostic_activity_started": started,
        "diagnostic_pid": pid if pid_valid else None,
        "diagnostic_success_log_pids": ok_pids,
        "arm64_jni_executed": native_jni,
        "original_oxide_apk_installed": False,
        "original_oxide_gameplay_verified": False,
        "play_integrity_verdict": "NOT_OBSERVED",
        "note": "A tiny JNI success would show translation works for the isolated test, "
                "not that the original protected Oxide game accepts the emulator.",
    }


def main():
    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument("reports", type=Path)
    parser.add_argument("--output", type=Path, default=None)
    args = parser.parse_args()
    verdict = assess(args.reports)
    output = args.output or args.reports / "nativebridge-jni-verdict.json"
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(verdict, indent=2, sort_keys=True) + "\n")
    print(json.dumps(verdict, sort_keys=True))
    return 0 if verdict["arm64_jni_executed"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
