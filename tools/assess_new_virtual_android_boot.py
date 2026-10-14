#!/usr/bin/env python3
"""Fail-closed assessment of genuine native ARM64 Redroid 15/16 boot evidence.

This probe does not install any game and does not infer certification,
integrity outcomes or gameplay from an Android home-screen boot.
"""
import argparse
import json
from pathlib import Path

SUPPORTED = {"15", "16"}

def cleaned(value):
    return str(value or "").strip().replace("\r", "")

def assess(version, evidence):
    version = str(version)
    if version not in SUPPORTED:
        raise ValueError("Only newer Android 15 and 16 are supported")
    status = {key: cleaned(val) for key, val in evidence.items()}
    arch = status.get("host_arch", "")
    abi = status.get("guest_abi", "")
    release = status.get("android_release", "")
    backend = status.get("rendering_mode", "")
    boot = status.get("boot_completed", "")
    images = status.get("container_arch", "")
    checks = {
        "native_arm64_host": arch in {"aarch64", "arm64"},
        "native_arm64_container": images in {"arm64", "aarch64"},
        "guest_arm64_abi": "arm64-v8a" in abi.split(","),
        "expected_android_major": release.split(".", 1)[0] == version,
        "android_boot_completed": boot == "1",
        "guest_rendering_mode": backend == "guest",
        "adb_shell_connected": status.get("adb_shell", "") == "PASS",
        "activity_manager_responded": status.get("activity_manager", "") == "PASS",
    }
    required = all(checks.values())
    return {
        "probe": "ORIGINAL_GAME_INDEPENDENT_NATIVE_ARM64_VIRTUAL_ANDROID_BOOT",
        "android_target": version,
        "boot_verdict": "NATIVE_ARM64_ANDROID_BOOT_VERIFIED" if required else "BOOT_UNVERIFIED",
        "verified_checks": checks,
        "observations": status,
        "google_play_package_observed": status.get("google_play_services", "NOT_MEASURED"),
        "gles_observed": status.get("surfaceflinger_gles", "NOT_MEASURED"),
        "original_game_installed": False,
        "original_game_launched": False,
        "original_gameplay_verified": False,
        "play_integrity_verdict": "NOT_ATTESTED",
        "limitations": [
            "Android home-screen boot does not establish that original Oxide can run.",
            "Google package presence is not a Play Integrity verdict or device certification.",
            "No original game APK or third-party application was installed.",
            "This experiment uses a containerized ARM64 Android guest; no phone is involved.",
        ],
    }

def markdown(result):
    lines = ["# Virtual ARM64 Android " + result["android_target"] + " boot proof",
             "", "**" + result["boot_verdict"] + "**", "",
             "| Check | Observed |", "|---|---|"]
    for label, value in result["verified_checks"].items():
        lines.append("| " + label + " | " + str(value) + " |")
    lines += ["", "Observed Google Play services: " + result["google_play_package_observed"],
              "GLES: " + result["gles_observed"],
              "", "Original game: **NOT INSTALLED OR STARTED**.",
              "Play Integrity: **NOT ATTESTED**.",
              "Original gameplay/ESP: **NOT VERIFIED**."]
    return "\n".join(lines) + "\n"

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--version", required=True, choices=sorted(SUPPORTED))
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--out", required=True, type=Path)
    args = parser.parse_args()
    raw = json.loads(args.input.read_text(encoding="utf-8"))
    result = assess(args.version, raw)
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "virtual_android_boot_verdict.json").write_text(
        json.dumps(result, indent=2) + "\n", encoding="utf-8")
    (args.out / "virtual_android_boot_verdict.md").write_text(markdown(result), encoding="utf-8")
    print(markdown(result))
    if result["boot_verdict"] != "NATIVE_ARM64_ANDROID_BOOT_VERIFIED":
        raise SystemExit("Android guest boot is not independently verified")

if __name__ == "__main__":
    main()
