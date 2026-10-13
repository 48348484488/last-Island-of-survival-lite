#!/usr/bin/env python3
"""Diagnose an OFFICIAL Google Play Android emulator for Oxide ARM64 compatibility.

Does not obtain or fabricate Play Integrity tokens, patch games or bypass
licensing. Package presence and ABI support are prerequisite evidence only.
"""
import argparse
import json
from pathlib import Path

GOOGLE_PACKAGES = {
    "play_services": "com.google.android.gms",
    "play_store": "com.android.vending",
    "google_services_framework": "com.google.android.gsf",
}

def summarize(abis="", native_bridge="", packages="", sdk="", boot_complete=""):
    package_names = {line.strip().removeprefix("package:") for line in packages.splitlines()
                     if line.strip().startswith("package:")}
    abi_list = [a.strip() for a in abis.strip().split(",") if a.strip()]
    presence = {label: package in package_names
                for label, package in GOOGLE_PACKAGES.items()}
    arm_supported = "arm64-v8a" in abi_list
    all_google = all(presence.values())
    status = ("GOOGLE_PACKAGES_AND_ARM64_ABI_OBSERVED" if arm_supported and all_google else
              "GOOGLE_PACKAGES_PRESENT_ARM64_ABI_MISSING" if all_google else
              "ARM64_AVAILABLE_GOOGLE_PACKAGES_INCOMPLETE" if arm_supported else
              "UNSUITABLE_OR_INCOMPLETE_ENVIRONMENT")
    return {
        "status": status,
        "abis_reported": abi_list,
        "arm64_game_binary_abi_advertised": arm_supported,
        "native_bridge_property": native_bridge.strip() or "NOT_REPORTED",
        "google_packages": presence,
        "android_api_level": sdk.strip() or "UNKNOWN",
        "boot_completed": boot_complete.strip() == "1",
        "play_integrity_verdict": "NOT_MEASURED",
        "play_account_signed_in": "NOT_MEASURED",
        "game_installation_tested": False,
        "game_launch_tested": False,
        "gameplay_tested": False,
        "limitations": [
            "System-reported ABI compatibility does not guarantee successful native code execution.",
            "Installed Google Play packages do not prove Play licensing or integrity verdict.",
            "Android emulator system image is not interchangeable with genuine certified hardware.",
            "No Oxide APK download, patch, injection, or security manipulation was performed.",
        ],
    }

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("evidence", type=Path)
    args = p.parse_args()
    root = args.evidence
    def read(name):
        path = root / name
        return path.read_text(encoding="utf-8", errors="replace") if path.is_file() else ""
    result = summarize(read("abis.txt"), read("native_bridge.txt"),
                       read("packages.txt"), read("sdk.txt"), read("boot.txt"))
    (root / "prerequisites.json").write_text(json.dumps(result, indent=2) + "\n",
                                             encoding="utf-8")
    lines = [
        "# Official Google Play Android emulator — native Oxide prerequisites",
        "", "Status: " + result["status"], "",
        "Android SDK: " + result["android_api_level"],
        "Boot completed: " + str(result["boot_completed"]),
        "Advertises arm64-v8a: " + str(result["arm64_game_binary_abi_advertised"]),
        "ABI list: " + ", ".join(result["abis_reported"]),
        "Native bridge: " + result["native_bridge_property"],
        "", "| Google package | Installed |", "|---|---|",
    ]
    lines.extend("| " + key + " | " + str(val) + " |"
                 for key, val in result["google_packages"].items())
    lines += ["", "Play Integrity verdict: NOT MEASURED",
              "Oxide original installation: NOT TESTED",
              "Oxide original launch: NOT TESTED",
              "", "No game protection was modified or bypassed."]
    (root / "prerequisites.md").write_text("\n".join(lines) + "\n", encoding="utf-8")
    print("\n".join(lines))
    if not result["boot_completed"]:
        raise SystemExit("Emulator not completely booted")

if __name__ == "__main__":
    main()
