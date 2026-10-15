#!/usr/bin/env python3
"""Read-only runtime architecture gate for original Oxide 1.15.11966.

Supports real native ARM64 Android and separately proved *translated* ARM64
JNI execution on an official x86_64 AVD. Declared ABI alone is NEVER proof.
Neither mode certifies a device or guarantees game/runtime compatibility.
"""
import json
import re
from pathlib import Path

EXPECTED_BRIDGE_STATUS = "ARM64_JNI_EXECUTED_VIA_NATIVE_BRIDGE_ORIGINAL_GAME_UNTESTED"


def evaluate_runtime_environment(directory):
    directory = Path(directory)

    def read(name):
        path = directory / name
        return path.read_text(encoding="utf8", errors="replace").strip() if path.is_file() else ""

    boot = read("boot.txt") == "1"
    abi = read("abi.txt")
    native = boot and abi == "arm64-v8a"
    proof_path = directory / "nativebridge-prerequisite.json"
    try:
        proof = json.loads(proof_path.read_text(encoding="utf8")) if proof_path.is_file() else {}
    except (OSError, ValueError, UnicodeError):
        proof = {}
    if not isinstance(proof, dict):
        proof = {}

    pids = proof.get("diagnostic_success_log_pids")
    original_pid = proof.get("diagnostic_pid")
    known_jni = (
        proof.get("status") == EXPECTED_BRIDGE_STATUS
        and proof.get("official_android_booted") is True
        and proof.get("host_abi_x86_64") is True
        and proof.get("arm64_abi_advertised") is True
        and proof.get("native_bridge_configured") is True
        and proof.get("diagnostic_activity_started") is True
        and proof.get("diagnostic_package_installed") is True
        and proof.get("arm64_jni_executed") is True
        and proof.get("original_oxide_apk_installed") is False
        and proof.get("original_oxide_gameplay_verified") is False
        and isinstance(original_pid, str)
        and bool(re.fullmatch(r"[1-9]\d*", original_pid))
        and isinstance(pids, list)
        and original_pid in pids
    )
    translated = (boot and abi == "x86_64"
                  and "arm64-v8a" in read("abilist64.txt").split(",")
                  and read("native-bridge.txt") == "libndk_translation.so"
                  and known_jni)
    mode = ("ARM64_NATIVE_ANDROID" if native
            else "X86_64_ANDROID_ARM64_NATIVE_BRIDGE_JNI_PROVEN" if translated
            else "BLOCKED_NOT_VERIFIED_ARM64_RUNTIME")
    return {
        "status": mode,
        "original_arm64_runtime_eligible": bool(native or translated),
        "android_booted": boot,
        "guest_abi": abi or None,
        "android_arm64_native": bool(native),
        "arm64_jni_translation_proven_in_predecessor": bool(translated),
        "device_integrity_certified": False,
        "gameplay_verified": False,
    }
