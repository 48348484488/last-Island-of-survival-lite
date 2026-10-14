#!/usr/bin/env python3
"""Read-only matrix of three archived, unchanged Oxide Android ARM64 launches.

Archive replay is NOT a new game launch, protection diagnosis, or gameplay proof.
"""
import argparse
import json
from pathlib import Path

PROVENANCE = {
    "12": {"original_run": 38488270142, "replay_run": 38488773446,
           "evidence_file": "android12_activity_pid_exit_verified.json"},
    "13": {"original_run": 38599474610, "evidence_file": "forensic_summary.json"},
    "14": {"original_run": 38487286930, "evidence_file": "android_14/forensic_summary.json"},
}

def valid_pid(value):
    return type(value) is int and value > 0

def check_version(version, data):
    if not isinstance(data, dict):
        data = {}
    if version == "12":
        pid = data.get("activity_snapshot_pid")
        checks = {
            "cross_source_activity_and_exit": data.get("status") == "ORIGINAL_ACTIVITY_PID_AND_SIGNAL_CONFIRMED",
            "resumed_activity": data.get("snapshot_activity_resumed") is True,
            "pid_sampled_multiple_times": type(data.get("main_package_pid_sample_count")) is int
                                         and data.get("main_package_pid_sample_count") >= 2,
            "same_pid_self_sigkill": data.get("prior_pid_scoped_logcat_self_sigkill") is True,
            "same_pid_android_exit_status9": data.get("android_exit_info_sigkill_status9") is True,
            "gameplay_unverified": data.get("multiplayer_gameplay_verified") is False,
        }
    else:
        pid = data.get("initial_activity_pid")
        line = data.get("evidence", {}).get("sigkill") if isinstance(data.get("evidence"), dict) else None
        # Logcat format: DATE TIME PID TID LEVEL TAG: ...
        parts = line.split() if isinstance(line, str) else []
        checks = {
            "original_activity_self_sigkill": data.get("classification") == "ORIGINAL_ACTIVITY_SELF_SIGKILL",
            "activity_start_recorded": isinstance(data.get("initial_activity_start"), str)
                                       and bool(data["initial_activity_start"]),
            "same_process_signal_recorded": isinstance(data.get("first_self_sigkill"), str)
                                            and bool(data["first_self_sigkill"]),
            "same_pid_in_logcat": len(parts) > 4 and valid_pid(pid)
                                  and parts[2] == str(pid)
                                  and "Sending signal. PID: " + str(pid) + " SIG: 9" in line,
        }
    checks["valid_activity_pid"] = valid_pid(pid)
    verified = all(checks.values())
    return {
        "android_version": version, "backend": "native_arm64_redroid",
        "original_activity_pid": pid if valid_pid(pid) else None,
        "checks": checks, "evidence": dict(PROVENANCE[version]),
        "classification": "ORIGINAL_GAME_EARLY_SELF_SIGKILL_CONFIRMED" if verified else "INSUFFICIENT_EVIDENCE",
        "activity_alive_35s_proven": False,
        "gameplay_proven": False,
        "security_cause_proven": False,
    }

def summarize(android12, android13, android14):
    rows = [check_version(v, source) for v, source in
            (("12", android12), ("13", android13), ("14", android14))]
    return {
        "scope": "ARCHIVED_NATIVE_ARM64_ANDROID_ORIGINAL_OXIDE_V12_V13_V14",
        "cases": rows,
        "all_three_self_sigkill_confirmed": all(
            x["classification"] == "ORIGINAL_GAME_EARLY_SELF_SIGKILL_CONFIRMED" for x in rows),
        "gameplay_verified": False,
        "security_root_cause": "UNKNOWN",
        "host_android_pid_namespace_mapping": "NOT_MEASURED_BY_THIS_REPORT",
        "conclusion": "THREE_OBSERVED_EARLY_EXITS_NOT_A_GENERAL_VIRTUAL_ANDROID_IMPOSSIBILITY",
        "limitations": [
            "Only the three tested Redroid versions are included; another virtual platform might behave differently.",
            "Archived process evidence does not prove the game's internal security rejection cause.",
            "Android Activity creation and rendered loading screen are not a playable multiplayer session.",
            "No original APK modifications, device identity spoofing or protection bypass.",
        ],
    }

def markdown(data):
    lines = ["# Oxide original: three verified native ARM64 virtual Android startups",
             "", "Only archived, original APK evidence; no game downloaded or executed by this comparison.",
             "", "| Android | Activity PID | Verified exit | Original run |",
             "|---|---:|---|---|"]
    for row in data["cases"]:
        p = row["evidence"]["original_run"]
        lines.append("| " + row["android_version"] + " | " + str(row["original_activity_pid"]) +
                     " | " + row["classification"] + " | [#" + str(p) +
                     "](https://github.com/48348484488/oxide/actions/runs/" + str(p) + ") |")
    lines += ["", "All three self-SIGKILL exits confirmed: " + str(data["all_three_self_sigkill_confirmed"]),
              "Original gameplay at 35 seconds: **NOT PROVEN**.",
              "Specific game-security root cause: **UNKNOWN**.",
              "Android 12 is a prior independent cross-source Activity PID verification.",
              "No physical devices or modified APKs."]
    return "\n".join(lines) + "\n"

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for version in ("12", "13", "14"):
        parser.add_argument("--android" + version, type=Path, required=True)
    parser.add_argument("--out", type=Path, required=True)
    args = parser.parse_args()
    data = summarize(*(json.loads(getattr(args, "android" + v).read_text(encoding="utf-8"))
                       for v in ("12", "13", "14")))
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "arm64_android_version_matrix.json").write_text(json.dumps(data, indent=2) + "\n")
    report = markdown(data)
    (args.out / "arm64_android_version_matrix.md").write_text(report)
    print(report)
    if not data["all_three_self_sigkill_confirmed"]:
        raise SystemExit("Evidence inconsistent: refusing to certify Android version matrix")

if __name__ == "__main__":
    main()
