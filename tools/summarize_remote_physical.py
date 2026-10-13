#!/usr/bin/env python3
"""Produce only safe summary booleans from authorized physical ADB evidence.

Never export raw logcat, screenshots, serials, build fingerprints or accounts.
"""
import argparse
import json
from pathlib import Path

def summarize(folder, rc):
    folder = Path(folder)
    def load(name):
        try:
            return json.loads((folder / name).read_text(encoding="utf-8"))
        except (OSError, ValueError):
            return {}
    a = load("activity_verdict.json")
    f = load("forensic_summary.json")
    env = load("environment_inventory.json")
    return {
        "test": "UNMODIFIED_ORIGINAL_OXIDE_AUTHORIZED_REMOTE_ARM64",
        "android_guest_reached": True,
        "physical_hardware_attestation": "NOT_MEASURED",
        "google_package_presence": env.get("google_package_presence", {}),
        "play_integrity_verdict": "NOT_MEASURED",
        "original_activity_state": a.get("status", "NOT_CLASSIFIED"),
        "original_process_classification": f.get("classification", "NOT_CLASSIFIED"),
        "initial_game_pid_at_10s": bool(a.get("pid_present_at_10s", False)),
        "initial_game_pid_at_35s": bool(a.get("pid_present_at_35s", False)),
        "game_resumed_10s": bool(a.get("oxide_activity_resumed_at_10s", False)),
        "game_resumed_35s": bool(a.get("oxide_activity_resumed_at_35s", False)),
        "game_smoke_exit_code": rc,
        "gameplay_verified": False,
        "real_players_esp_verified": False,
        "private_screenshots_or_full_logcat_uploaded": False,
        "apk_modified": False,
        "limitations": [
            "An external provider must supply an authorized physical Android and ADB tunnel.",
            "Google package presence or ADB sideload does not establish Play licensing or integrity.",
            "Activity survival is not evidence of multiplayer or real player coordinates.",
            "Full original screenshots, logs and device identifiers are not published.",
        ],
    }

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--evidence", type=Path, required=True)
    p.add_argument("--out", type=Path, required=True)
    p.add_argument("--smoke-exit-code", type=int, required=True)
    args = p.parse_args()
    d = summarize(args.evidence, args.smoke_exit_code)
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "remote_physical_verdict.json").write_text(
        json.dumps(d, indent=2) + "\n", encoding="utf-8")
    print("ORIGINAL_ACTIVITY=" + d["original_activity_state"])
    print("GAMEPLAY_VERIFIED=false")
if __name__ == "__main__":
    main()
