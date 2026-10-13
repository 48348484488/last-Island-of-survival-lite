#!/usr/bin/env python3
"""Interpret read-only Android runtime prerequisites for unmodified Oxide.

Never grants permissions, edits system identity or modifies game binaries.
Separates package-manager state, external services, and app termination.
"""
import argparse
import json
import re
from pathlib import Path

PKG = "com.catsbit.oxidesurvivalisland"
DECLARED = re.compile(r"^uses-permission(?:-sdk-\d+)?:\s+name=['\"]([^'\"]+)['\"]", re.M)
GRANTED = re.compile(r"^\s*([\w.]+):\s+granted=(true|false)\b", re.M | re.I)
SIGNALS = {
    "play_store_missing": r"requires the Google Play Store, but it is missing",
    "play_services_missing": r"Google Play services is missing",
    "firebase_services_missing_or_denied": r"Google Play services missing or without correct permission",
    "gmscore_invalid": r"Invalid GmsCore APK, remote loading disabled",
    "sensor_enumeration_error": r"Sensors\s*:\s*Failed to get Sensor list",
    "graphic_buffer_allocation_error": r"GraphicBufferAllocator\s*:\s*Failed to allocate",
    "self_directed_sigkill_log": r"Process\s*:\s*Sending signal\. PID: (\d+) SIG: 9",
}
INTEGRITY_STATE = "NOT_ATTESTED"

def summarize(aapt="", dumpsys="", appops="", play_packages="", logs="", forensic=None):
    names = sorted(set(DECLARED.findall(aapt)))
    grants = {name: value.lower() == "true" for name, value in GRANTED.findall(dumpsys)}
    requested = [{"name": name,
                  "grant": ("GRANTED" if grants.get(name) is True else
                            "DENIED" if grants.get(name) is False else "NOT_MEASURED")}
                 for name in names]
    declared_denied = [r["name"] for r in requested if r["grant"] == "DENIED"]
    observations = {key: len(re.findall(pattern, logs, re.I))
                    for key, pattern in SIGNALS.items()}
    pkg_lines = {x.strip() for x in play_packages.splitlines() if x.strip()}
    gms_state = ("PRESENT" if "package:com.google.android.gms" in pkg_lines else
                 "NOT_FOUND" if "__PACKAGE_QUERY_COMPLETED__" in pkg_lines else "NOT_OBSERVED")
    return {
        "app": PKG,
        "declared_permission_count": len(names),
        "measured_permission_count": sum(x["grant"] != "NOT_MEASURED" for x in requested),
        "requested_permission_status": requested,
        "declared_denied_permissions": declared_denied,
        "appops_observation": ("CAPTURED" if appops.strip() else "NOT_MEASURED"),
        "play_services_package_presence": gms_state,
        "runtime_log_signal_counts": observations,
        "game_termination": (forensic or {}).get("classification", "NOT_CLASSIFIED"),
        "game_initial_pid": (forensic or {}).get("initial_activity_pid"),
        "device_integrity": INTEGRITY_STATE,
        "proven_security_rejection_cause": "UNKNOWN",
        "meaning": [
            "Package grant flags, app-op policy and installed Play components are separate.",
            "Missing Google services does not prove the cause of a security response.",
            "Sensor and graphics messages can originate from other Android processes.",
            "Even all permissions granted would not establish Play certification or working gameplay.",
            "This audit never changes runtime permissions, game code, signing or device identity.",
        ],
    }

def markdown(result):
    lines = [
        "# Android runtime prerequisites - original Oxide",
        "",
        "Declared permissions: " + str(result["declared_permission_count"]),
        "Measured permission grants: " + str(result["measured_permission_count"]),
        "",
        "| Original APK permission | Observed grant |", "|---|---|",
    ]
    for item in result["requested_permission_status"]:
        lines.append("| " + item["name"] + " | " + item["grant"] + " |")
    lines += [
        "", "Google Play Services installed: " + result["play_services_package_presence"],
        "AppOps information: " + result["appops_observation"],
        "", "## Android log signals (counts; not causation)", "",
    ]
    for name, count in result["runtime_log_signal_counts"].items():
        lines.append("- " + name + ": " + str(count))
    lines += [
        "", "Original game process termination: " + result["game_termination"],
        "Game PID: " + str(result["game_initial_pid"]),
        "Device integrity attestation: NOT_ATTESTED",
        "Specific game security rejection cause: UNKNOWN",
        "", "## Interpretation limits", "",
    ] + ["- " + limitation for limitation in result["meaning"]]
    return "\n".join(lines) + "\n"

def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--aapt", type=Path, required=True)
    ap.add_argument("--dumpsys", type=Path)
    ap.add_argument("--appops", type=Path)
    ap.add_argument("--play-packages", type=Path)
    ap.add_argument("--logcat", type=Path)
    ap.add_argument("--forensic-json", type=Path)
    ap.add_argument("--out", type=Path, required=True)
    args = ap.parse_args()
    def read(path):
        return path.read_text(encoding="utf-8", errors="replace") if path and path.is_file() else ""
    forensic = {}
    if args.forensic_json and args.forensic_json.is_file():
        forensic = json.loads(args.forensic_json.read_text(encoding="utf-8"))
    result = summarize(read(args.aapt), read(args.dumpsys), read(args.appops),
                       read(args.play_packages), read(args.logcat), forensic)
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "runtime_prerequisites.json").write_text(
        json.dumps(result, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    (args.out / "runtime_prerequisites.md").write_text(markdown(result), encoding="utf-8")
    print(markdown(result))
    if not result["declared_permission_count"]:
        raise SystemExit("No original APK permission declarations: input or AAPT mismatch")

if __name__ == "__main__":
    main()
