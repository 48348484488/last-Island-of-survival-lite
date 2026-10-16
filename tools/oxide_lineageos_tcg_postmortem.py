#!/usr/bin/env python3
"""Fail-closed automatic postmortem of a completed full Android ARM64 boot CI artifact.

Never downloads Android, APKs, or changes any installed image. A QEMU screenshot
or Android service launch is NOT proof of sys.boot_completed or Oxide gameplay.
"""
import argparse
import json
import re
from pathlib import Path


def inspect(artifact: Path):
    result_file = artifact / "result.txt"
    raw = result_file.read_text(errors="replace") if result_file.is_file() else ""
    fields = {}
    for line in raw.splitlines():
        if "=" in line:
            k, v = line.split("=", 1)
            fields[k] = v
    serial_file = artifact / "serial-console.log"
    serial = serial_file.read_text(errors="replace") if serial_file.is_file() else ""
    screenshots = {}
    for mark in ("0270", "0360", "0450"):
        meta = artifact / ("full-android-" + mark + ".json")
        png = artifact / ("full-android-" + mark + ".png")
        record = {}
        if meta.is_file():
            try:
                obj = json.loads(meta.read_text())
                record = {
                    "framebuffer_captured": bool(obj.get("framebuffer_captured")),
                    "framebuffer_sha256": obj.get("framebuffer_sha256"),
                    "sampled_unique_rgb": obj.get("sampled_unique_rgb"),
                    "width": obj.get("width"),
                    "height": obj.get("height"),
                }
            except (json.JSONDecodeError, OSError, ValueError):
                record = {"invalid_metadata": True}
        record["png_valid_signature"] = (
            png.is_file() and png.open("rb").read(8) == bytes.fromhex("89504e470d0a1a0a")
        )
        screenshots[mark] = record

    boot = (
        fields.get("BOOT_COMPLETED") == "1"
        and fields.get("RESULT") == "PASS_ANDROID_ARM64_FULL_BOOT_NO_GAME"
    )
    same_late_screen = (
        bool(screenshots["0360"].get("framebuffer_sha256"))
        and screenshots["0360"].get("framebuffer_sha256") ==
        screenshots["0450"].get("framebuffer_sha256")
    )
    # The default guest may not expose ADB. Separate Android boot-stage evidence,
    # ADB transport availability, framebuffer status, and original gameplay.
    service_started = lambda name: bool(re.search(
        r"init: (?:\\.\\.\\. )?started service '" + re.escape(name) + r"' has pid",
        serial,
    ))
    adb_stopped = "init.svc.adbd=stopped" in serial
    adb_requested = bool(re.search(r"init: .*started service 'adbd' has pid", serial))
    shell_bytes = (artifact / "hvc0-shell.log").stat().st_size \\
        if (artifact / "hvc0-shell.log").is_file() else 0
    logcat_bytes = (artifact / "hvc1-logcat.log").stat().st_size \\
        if (artifact / "hvc1-logcat.log").is_file() else 0
    images_captured = [k for k, v in screenshots.items()
                       if v.get("png_valid_signature") and v.get("framebuffer_captured")]
    observations = {
        "adb_service_reported_stopped_in_serial": adb_stopped,
        "adb_service_reported_started_in_serial": adb_requested,
        "system_server_process_seen": "system_server" in serial,
        "android_init_observed": "init: " in serial,
        "actual_framebuffer_snapshots": images_captured,
        "graphics_framebuffer_captured": bool(images_captured),
        "boot_completed_via_adb": boot,
        "same_framebuffer_at_360_and_450_seconds": same_late_screen,
        "initialization_animation_service_started":
            bool(re.search(r"init: .*started service 'bootanim' has pid", serial)),
        "android_zygote_service_started":
            bool(re.search(r"init: .*started service 'zygote' has pid", serial)),
        "android_surfaceflinger_service_started":
            bool(re.search(r"init: .*started service 'surfaceflinger' has pid", serial)),
        "seriallogging_service_failure_in_serial":
            bool(re.search(r"init: Service 'seriallogging'.*exited with status [1-9]", serial)),
        "sound_card_setup_sigabrt_in_serial":
            bool(re.search(r"init: Service 'vendor.setup_sound_card'.*received SIGABRT", serial)),
        "hvc0_shell_bytes": (artifact / "hvc0-shell.log").stat().st_size
            if (artifact / "hvc0-shell.log").is_file() else 0,
        "hvc1_logcat_bytes": (artifact / "hvc1-logcat.log").stat().st_size
            if (artifact / "hvc1-logcat.log").is_file() else 0,
        "adb_available": bool(fields.get("LAST_REPORTED_ABI", "")),
        "android_ui_home_screen_confirmed": False,
        "latest_qemu_wall_seconds": fields.get("WALL_BOOT_PROBE_SECONDS"),
        "runtime_exit_classification": fields.get("RESULT", "UNKNOWN"),
        "screenshots": screenshots,
    }
    return {
        "schema_version": 1,
        "status": "ANDROID_ARM64_BOOT_CONFIRMED_NO_GAME" if boot
                  else "ANDROID_ARM64_BOOT_NOT_CONFIRMED",
        "original_oxide_apk_executed": False,
        "intermediate_android_boot_stage_observed": observations["android_init_observed"]
            and observations["initialization_animation_service_started"],
        "original_oxide_gameplay_confirmed": False,
        "google_play_certification_verified": False,
        "observations": observations,
        "note": "A boot animation and two identical frames cannot by themselves establish a deadlock, finished boot, or game compatibility.",
    }


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--artifact", type=Path, required=True)
    p.add_argument("--output", type=Path, required=True)
    args = p.parse_args()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    report = inspect(args.artifact)
    args.output.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n")
    print(json.dumps(report, sort_keys=True))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
