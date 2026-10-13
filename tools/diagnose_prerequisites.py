#!/usr/bin/env python3
"""Read-only analysis of APK permission declarations versus Android grants."""
import argparse
import json
import re
from pathlib import Path

APK_PERMISSION = re.compile(r"^uses-permission(?:-sdk-\d+)?:\s+name=['\"]([^'\"]+)['\"]", re.M)
GRANTED_PERMISSION = re.compile(r"^\s*([A-Za-z][\w.]*):\s+granted=(true|false)\b", re.M)
GMS_MARKERS = {
    "play_store_missing": "requires the Google Play Store, but it is missing",
    "play_services_missing_or_permission": "Google Play services missing or without correct permission",
    "play_services_missing": "Google Play services is missing.",
    "gmscore_invalid": "Invalid GmsCore APK, remote loading disabled.",
}

def examine_permissions(aapt_text, dumpsys_text="", logcat_text=""):
    requested = sorted(set(APK_PERMISSION.findall(aapt_text)))
    grants = {name: status == "true" for name, status in GRANTED_PERMISSION.findall(dumpsys_text)}
    entries = [
        {
            "name": name, "reported_granted": grants.get(name),
            "meaning": ("GRANTED_BY_ANDROID" if grants.get(name) is True else
                        "NOT_GRANTED_BY_ANDROID" if grants.get(name) is False else
                        "NOT_MEASURED"),
        }
        for name in requested
    ]
    return {
        "package": "com.catsbit.oxidesurvivalisland",
        "requested_permissions_count": len(requested),
        "requested_permissions": entries,
        "reported_permissions_count": len(grants),
        "gms_log_markers": {name: marker in logcat_text for name, marker in GMS_MARKERS.items()},
        "permission_state": "NOT_MEASURED" if not dumpsys_text.strip() else "PARTIALLY_MEASURED",
        "security_rejection_cause": "NOT_IDENTIFIED",
        "limitations": [
            "Permission declarations do not prove actual grant status.",
            "Absent Google packages and denied permissions are distinct issues.",
            "No log entry here identifies the game's precise security rejection rule.",
            "This tool never modifies game code, permissions or Android identity.",
        ],
    }

def render_md(report):
    lines = ["# Original Oxide Android prerequisite audit", "",
             "Read-only analysis; no original APK changes.", "",
             "Declared permissions: " + str(report["requested_permissions_count"]), "",
             "| Declared permission | Observed grant |", "|---|---|"]
    for item in report["requested_permissions"]:
        lines.append("| " + item["name"] + " | " + item["meaning"] + " |")
    lines += ["", "## Google service warnings in collected log", ""]
    for key, value in report["gms_log_markers"].items():
        lines.append("- " + key + ": " + ("OBSERVED" if value else "NOT_OBSERVED_IN_SUPPLIED_LOG"))
    lines += ["", "Game security rejection cause: NOT IDENTIFIED", "",
              "These messages cannot establish why the game terminates.", ""]
    return "\n".join(lines)

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--aapt", type=Path, required=True)
    p.add_argument("--dumpsys", type=Path)
    p.add_argument("--logcat", type=Path)
    p.add_argument("--out", type=Path, required=True)
    args = p.parse_args()
    def read(path):
        return path.read_text(encoding="utf-8", errors="replace") if path and path.is_file() else ""
    result = examine_permissions(read(args.aapt), read(args.dumpsys), read(args.logcat))
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "prerequisite_audit.json").write_text(
        json.dumps(result, indent=2) + "\n", encoding="utf-8")
    (args.out / "prerequisite_audit.md").write_text(render_md(result), encoding="utf-8")
    print(render_md(result))
    if not result["requested_permissions_count"]:
        raise SystemExit("No declared permissions detected; validate aapt output")

if __name__ == "__main__":
    main()
