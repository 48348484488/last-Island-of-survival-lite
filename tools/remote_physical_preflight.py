#!/usr/bin/env python3
"""Fail-closed readiness gate for a *separately authorized* remote physical device.

Requires an SSH-accessible ADB server with a real Android device already
attached. Never connects to external hosts, provisions/charges a device,
downloads game files, reads attestation tokens or guesses ADB serials.
"""
import argparse
import json
import os
import re
from pathlib import Path

FIELDS = (
    "OXIDE_REMOTE_HOST",
    "OXIDE_REMOTE_USER",
    "OXIDE_REMOTE_SSH_KEY",
    "OXIDE_REMOTE_KNOWN_HOSTS",
    "OXIDE_REMOTE_ADB_SERIAL",
)
HOST_PATTERN = re.compile(r"^[a-zA-Z0-9][a-zA-Z0-9.-]{0,250}$")
USER_PATTERN = re.compile(r"^[a-zA-Z_][a-zA-Z0-9_-]{0,63}$")
SERIAL_PATTERN = re.compile(r"^[a-zA-Z0-9][a-zA-Z0-9._:-]{0,127}$")

def evaluate(env):
    missing = [name for name in FIELDS if not env.get(name, "").strip()]
    invalid = []
    if env.get("OXIDE_REMOTE_HOST") and not HOST_PATTERN.fullmatch(env["OXIDE_REMOTE_HOST"]):
        invalid.append("OXIDE_REMOTE_HOST")
    if env.get("OXIDE_REMOTE_USER") and not USER_PATTERN.fullmatch(env["OXIDE_REMOTE_USER"]):
        invalid.append("OXIDE_REMOTE_USER")
    if env.get("OXIDE_REMOTE_ADB_SERIAL") and not SERIAL_PATTERN.fullmatch(env["OXIDE_REMOTE_ADB_SERIAL"]):
        invalid.append("OXIDE_REMOTE_ADB_SERIAL")
    if env.get("OXIDE_REMOTE_SSH_KEY") and "PRIVATE KEY" not in env["OXIDE_REMOTE_SSH_KEY"]:
        invalid.append("OXIDE_REMOTE_SSH_KEY")
    # Trust pinning is mandatory, not dynamic ssh-keyscan (MITM unsafe).
    if env.get("OXIDE_REMOTE_KNOWN_HOSTS") and not any(
        token in env["OXIDE_REMOTE_KNOWN_HOSTS"] for token in ("ssh-ed25519 ", "ecdsa-sha2-", "ssh-rsa ")
    ):
        invalid.append("OXIDE_REMOTE_KNOWN_HOSTS")
    ready = not missing and not invalid
    return {
        "status": "REMOTE_PHYSICAL_CONNECTION_CONFIGURED" if ready else "REMOTE_PHYSICAL_NOT_CONFIGURED",
        "required_secret_names": list(FIELDS),
        "missing_secret_names": missing,
        "invalid_format_secret_names": sorted(set(invalid)),
        "device_connected": False,
        "device_type": "UNVERIFIED",
        "game_installed": False,
        "gameplay_verified": False,
        "original_game_modified": False,
        "cloud_device_billing_triggered": False,
        "notes": [
            "Presence and syntax of credentials do not prove physical device availability.",
            "A rented device must already expose an authorized ADB server through a pinned SSH host.",
            "Original APK downloads/install may begin only after an actual authenticated ADB check.",
            "ADB sideloading does not guarantee Google Play license or app/device integrity.",
            "No security checks are weakened or spoofed.",
        ],
    }

def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--output", required=True, type=Path)
    options = ap.parse_args()
    result = evaluate(os.environ)
    options.output.parent.mkdir(parents=True, exist_ok=True)
    options.output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    # No secret values in stdout, file or GitHub job summary.
    print(result["status"])
    print("Missing fields: " + (", ".join(result["missing_secret_names"]) or "none"))
    print("Invalid formats: " + (", ".join(result["invalid_format_secret_names"]) or "none"))
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
