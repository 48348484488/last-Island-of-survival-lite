#!/usr/bin/env python3
"""Compare two original Oxide ARM64 starts with only normal app permission grants."""
import argparse
import json
from pathlib import Path

PERMS = (
    "android.permission.CAMERA",
    "android.permission.RECORD_AUDIO",
    "android.permission.BLUETOOTH_CONNECT",
    "android.permission.POST_NOTIFICATIONS",
)

def compare(before, after, requests):
    b = {p["name"]: p["grant"] for p in before.get("requested_permission_status", [])}
    a = {p["name"]: p["grant"] for p in after.get("requested_permission_status", [])}
    differences = [{
        "permission": p,
        "before": b.get(p, "NOT_MEASURED"),
        "after": a.get(p, "NOT_MEASURED"),
        "changed_to_granted": b.get(p) != "GRANTED" and a.get(p) == "GRANTED",
        "grant_attempt": requests.get(p, "NOT_ATTEMPTED"),
    } for p in PERMS]
    changed = sum(x["changed_to_granted"] for x in differences)
    term_before = before.get("game_termination", "NOT_CLASSIFIED")
    term_after = after.get("game_termination", "NOT_CLASSIFIED")
    if changed == 0:
        status = "NO_NEW_EFFECTIVE_PERMISSIONS"
    elif term_before == "ORIGINAL_ACTIVITY_SELF_SIGKILL" and term_after == "ORIGINAL_ACTIVITY_SELF_SIGKILL":
        status = "PERMISSIONS_GRANTED_BUT_SELF_SIGKILL_PERSISTS"
    elif term_after == "ORIGINAL_ACTIVITY_SELF_SIGKILL":
        status = "PERMISSIONS_GRANTED_SELF_SIGKILL_OBSERVED"
    else:
        status = "TERMINATION_CHANGED_REQUIRES_GAMEPLAY_VALIDATION"
    return {
        "status": status,
        "grant_changes": changed,
        "transitions": differences,
        "before_game_termination": term_before,
        "after_game_termination": term_after,
        "before_game_pid": before.get("game_initial_pid"),
        "after_game_pid": after.get("game_initial_pid"),
        "play_services_after": after.get("play_services_package_presence"),
        "device_integrity_verdict": "NOT_ATTESTED",
        "gameplay_proven": False,
        "player_positions_proven": False,
        "security_rejection_cause": "NOT_IDENTIFIED",
        "experiment_limitations": [
            "Both starts ran in one Android guest, but package runtime state and timing can differ.",
            "Normal Android permission grants do not substitute for Google Play services or certification.",
            "A changed process termination result would not alone prove multiplayer or ESP.",
            "No original APK, identity or security controls were altered.",
        ],
    }

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--before", type=Path, required=True)
    p.add_argument("--after", type=Path, required=True)
    p.add_argument("--attempts", type=Path, required=True)
    p.add_argument("--out", type=Path, required=True)
    args = p.parse_args()
    before = json.loads(args.before.read_text(encoding="utf-8"))
    after = json.loads(args.after.read_text(encoding="utf-8"))
    attempts = json.loads(args.attempts.read_text(encoding="utf-8"))
    result = compare(before, after, attempts)
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "comparison.json").write_text(json.dumps(result, indent=2) + "\n")
    lines = ["# Original Oxide Android permission A/B", "",
             "Classification: **" + result["status"] + "**", "",
             "| Permission | Before | After | Command |", "|---|---|---|---|"]
    for t in result["transitions"]:
        lines.append("| " + t["permission"] + " | " + t["before"] + " | " +
                     t["after"] + " | " + t["grant_attempt"] + " |")
    lines += ["", "Before termination: " + result["before_game_termination"],
              "After termination: " + result["after_game_termination"],
              "New effective permissions: " + str(result["grant_changes"]),
              "Play Services after grants: " + str(result["play_services_after"]),
              "Device certification/integrity: NOT ATTESTED",
              "Gameplay and real player ESP: NOT VERIFIED", "",
              "## Limitations", ""]
    lines.extend("- " + t for t in result["experiment_limitations"])
    (args.out / "comparison.md").write_text("\n".join(lines) + "\n")
    print("\n".join(lines))

if __name__ == "__main__":
    main()
