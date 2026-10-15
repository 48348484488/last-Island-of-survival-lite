#!/usr/bin/env python3
"""Read-only eligibility of an already running hosted ARM64 Android virtual device.

The presence of Google packages is not evidence of trusted Play Integrity,
device certification, entitlement, or that the original Oxide will run.
No attestation, bypass, identity changes or APK modification is performed.
"""
import argparse
import json
import re
from pathlib import Path

PM_PATH = re.compile(r"^package:/\S+\.apk$")


def installed(path_output):
    return any(PM_PATH.fullmatch(line.strip()) is not None
               for line in path_output.splitlines())


def classify(boot, abi, vending_path, gms_path):
    arm64 = abi.strip() == "arm64-v8a"
    booted = boot.strip() == "1"
    vending = installed(vending_path)
    gms = installed(gms_path)
    if not booted:
        status = "BLOCKED_REMOTE_ANDROID_NOT_BOOTED"
    elif not arm64:
        status = "BLOCKED_REMOTE_NOT_NATIVE_ARM64"
    elif not vending:
        status = "BLOCKED_NO_GOOGLE_PLAY_STORE_PACKAGE"
    elif not gms:
        status = "BLOCKED_NO_GOOGLE_PLAY_SERVICES_PACKAGE"
    else:
        status = "ARM64_GOOGLE_PACKAGES_PRESENT_TRUST_UNVERIFIED"
    return {
        "schema_version": 1,
        "status": status,
        "android_booted": booted,
        "abi_arm64": arm64,
        "play_store_package_present": vending,
        "play_services_package_present": gms,
        "proceed_with_original_apk_download": (
            status == "ARM64_GOOGLE_PACKAGES_PRESENT_TRUST_UNVERIFIED"),
        "android_certified": "UNKNOWN",
        "play_integrity_verdict": "NOT_OBSERVED",
        "original_apk_unchanged": "NOT_TESTED",
        "original_activity_35s": False,
        "gameplay_confirmed": False,
        "limitations": [
            "Presence of packages does not demonstrate legitimate Google Play certification.",
            "An app may still deny service in virtual Android; do not bypass security.",
            "No original game APK has been downloaded, installed or run by this gate."
        ],
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--reports", type=Path, default=Path("reports"))
    args = parser.parse_args()
    d = args.reports

    def read(filename):
        path = d / filename
        return path.read_text(errors="replace") if path.is_file() else ""

    result = classify(read("boot.txt"), read("abi.txt"),
                      read("play-store-pm-path.txt"), read("play-services-pm-path.txt"))
    d.mkdir(parents=True, exist_ok=True)
    (d / "managed-arm64-gate.json").write_text(
        json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf8")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["proceed_with_original_apk_download"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
