#!/usr/bin/env python3
"""Collect non-invasive Android environment facts; this is NOT attestation."""
import argparse
import json
import re
import subprocess
from datetime import datetime, timezone
from pathlib import Path

PROPERTIES = (
    "ro.product.cpu.abilist", "ro.build.version.release", "ro.build.version.sdk",
    "ro.hardware", "ro.product.manufacturer", "ro.product.model",
    "ro.build.type", "ro.build.tags", "ro.kernel.qemu", "ro.boot.qemu",
    "ro.opengles.version",
)
PACKAGES = {
    "play_services": "com.google.android.gms",
    "play_store": "com.android.vending",
    "google_services_framework": "com.google.android.gsf",
}
FEATURES = (
    "android.hardware.sensor.accelerometer",
    "android.hardware.sensor.gyroscope",
    "android.hardware.touchscreen",
    "android.hardware.opengles.aep",
)

def adb_runner(serial):
    def invoke(args):
        try:
            p = subprocess.run(["adb", "-s", serial, "shell", *args],
                               capture_output=True, text=True, timeout=12, check=False)
            return p.returncode, p.stdout[:6000], p.stderr[:500]
        except (subprocess.TimeoutExpired, OSError) as exc:
            return 1, "", type(exc).__name__
    return invoke

def collect(run):
    props = {}
    for key in PROPERTIES:
        rc, out, _ = run(["getprop", key])
        props[key] = out.strip()[:180] if rc == 0 and out.strip() else "UNKNOWN"
    packages = {}
    for label, name in PACKAGES.items():
        rc, out, _ = run(["pm", "path", name])
        packages[label] = ("UNKNOWN" if rc != 0 else
                           "PRESENT" if any(x.strip().startswith("package:")
                                            for x in out.splitlines()) else "NOT_FOUND")
    features = {}
    for name in FEATURES:
        rc, out, _ = run(["pm", "has-feature", name])
        val = out.strip().lower()
        features[name] = val if rc == 0 and val in ("true", "false") else "unknown"
    rc, out, _ = run(["dumpsys", "SurfaceFlinger"])
    gpu = "UNKNOWN"
    if rc == 0:
        for line in out.splitlines():
            m = re.search(r"\bGLES\s*:\s*(.+)", line, re.I)
            if m:
                gpu = m.group(1).strip()[:180]
                break
    indicators = []
    if props["ro.hardware"].lower() in ("redroid", "ranchu", "goldfish", "vbox86"):
        indicators.append("hardware_matches_known_virtual_platform")
    if props["ro.boot.qemu"] == "1" or props["ro.kernel.qemu"] == "1":
        indicators.append("qemu_property_enabled")
    return {
        "schema_version": 1,
        "collected_at_utc": datetime.now(timezone.utc).isoformat(timespec="seconds"),
        "build_properties": props,
        "google_package_presence": packages,
        "reported_hardware_features": features,
        "gpu_gles_description": gpu,
        "virtual_environment_hints": indicators,
        "device_integrity_verdict": "NOT_MEASURED",
        "game_security_reason": "NOT_IDENTIFIED",
        "game_start_success": "NOT_TESTED_BY_THIS_SCRIPT",
        "limitations": [
            "Installed Google packages do not establish device certification or Play Integrity verdicts.",
            "Build properties are diagnostic, not proof of a certified or physical device.",
            "A virtual environment hint does not identify a particular game's security check.",
            "No APK or system protections were modified and no player data was collected.",
        ],
    }

def markdown(data):
    lines = ["# Android environment inventory", "",
             "This is an observation only, not a device-attestation test.", "",
             "## Build properties", "| Property | Value |", "|---|---|"]
    for key, val in data["build_properties"].items():
        lines.append("| " + key + " | " + val.replace("|", " ") + " |")
    lines += ["", "## Google packages", "| Package | Presence |", "|---|---|"]
    for key, val in data["google_package_presence"].items():
        lines.append("| " + key + " | " + val + " |")
    lines += ["", "## Android features", "| Feature | Reported |", "|---|---|"]
    for key, val in data["reported_hardware_features"].items():
        lines.append("| " + key + " | " + val + " |")
    lines += ["", "GLES description: " + data["gpu_gles_description"].replace("|", " "),
              "", "Virtual environment hints: " + (", ".join(data["virtual_environment_hints"]) or
                    "none found (not proof of physical device)"),
              "", "Device integrity verdict: NOT MEASURED",
              "", "Game security rejection reason: NOT IDENTIFIED",
              "", "## Limitations", ""]
    lines.extend("- " + s for s in data["limitations"])
    return "\n".join(lines) + "\n"

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--serial", required=True, help="ADB connection; never recorded in result")
    parser.add_argument("--out", type=Path, required=True)
    args = parser.parse_args()
    args.out.mkdir(parents=True, exist_ok=True)
    result = collect(adb_runner(args.serial))
    (args.out / "environment_inventory.json").write_text(
        json.dumps(result, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    (args.out / "environment_inventory.md").write_text(markdown(result), encoding="utf-8")
    print("Android environment audit generated (non-invasive).")
    print("Google package presence:", result["google_package_presence"])
    print("Virtual hints:", result["virtual_environment_hints"])

if __name__ == "__main__":
    main()
