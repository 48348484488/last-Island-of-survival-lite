#!/usr/bin/env python3
"""One-shot, noninvasive ARM64 GitHub runner virtualization preflight.

Only host architecture and Linux KVM API are examined. Does NOT install/start
Android, download APKs, run an emulator, fake integrity, or change a device.
A KVM-ready host is a prerequisite, NOT proof of playable Oxide or Play services.
"""
import argparse
import errno
import fcntl
import json
import os
import platform
import stat
from pathlib import Path

KVM_GET_API_VERSION = 0xAE00
EXPECTED_KVM_API_VERSION = 12
ELIGIBLE_ARCHS = frozenset(("aarch64", "arm64"))


def inspect_kvm(path="/dev/kvm", stat_fn=os.stat, open_fn=os.open,
                ioctl_fn=fcntl.ioctl, close_fn=os.close):
    """Check only the KVM API version; never create a virtual machine."""
    try:
        info = stat_fn(path)
    except FileNotFoundError:
        return {"status": "DEVICE_ABSENT", "api_version": None}
    except PermissionError:
        return {"status": "DEVICE_STAT_PERMISSION_DENIED", "api_version": None}
    except OSError:
        return {"status": "DEVICE_STAT_ERROR", "api_version": None}
    if not stat.S_ISCHR(info.st_mode):
        return {"status": "NOT_CHARACTER_DEVICE", "api_version": None}
    try:
        fd = open_fn(path, os.O_RDWR | getattr(os, "O_CLOEXEC", 0))
    except PermissionError:
        return {"status": "DEVICE_OPEN_PERMISSION_DENIED", "api_version": None}
    except OSError as exc:
        code = "DEVICE_OPEN_UNAVAILABLE" if exc.errno in (
            errno.ENODEV, errno.ENOENT, errno.ENXIO) else "DEVICE_OPEN_ERROR"
        return {"status": code, "api_version": None}
    try:
        try:
            version = ioctl_fn(fd, KVM_GET_API_VERSION, 0)
        except OSError:
            return {"status": "KVM_IOCTL_UNAVAILABLE", "api_version": None}
    finally:
        close_fn(fd)
    if type(version) is not int or version != EXPECTED_KVM_API_VERSION:
        return {"status": "KVM_API_NOT_12", "api_version": version
                if type(version) is int else None}
    return {"status": "KVM_API_12_ACCESSIBLE", "api_version": version}


def evaluate(platform_name, machine, kvm_probe, runner_image):
    """Keep host suitability and authorization to run Android separate."""
    if platform_name != "Linux":
        status = "HOST_NOT_LINUX"
    elif machine.lower() not in ELIGIBLE_ARCHS:
        status = "HOST_NOT_ARM64"
    elif kvm_probe.get("status") != "KVM_API_12_ACCESSIBLE":
        status = "ARM64_HOST_KVM_NOT_AVAILABLE_FOR_ACCELERATED_AVD"
    else:
        status = "ARM64_HOST_KVM_PREREQUISITE_MET"
    ready = status == "ARM64_HOST_KVM_PREREQUISITE_MET"
    return {
        "status": status,
        "runner_image": runner_image or "UNKNOWN",
        "host_os": platform_name,
        "host_machine": machine,
        "kvm": kvm_probe,
        "native_arm64_kvm_prerequisite_verified": ready,
        "arm64_android_system_image_verified": False,
        "official_google_play_arm64_image_verified": False,
        "android_or_oxide_executed": False,
        "original_apk_modified": False,
        "gameplay_verified": False,
        "original_activity_35s_verified": False,
        "heavy_android_retry_authorized": False,
        "security_trigger": "UNKNOWN",
        "next_action": (
            "VERIFY_OFFICIAL_ARM64_SYSTEM_IMAGE_AND_SUPPORT_WITHOUT_DOWNLOADING"
            if ready else "STOP_NO_USABLE_ARM64_HOST_KVM_IN_THIS_RUNNER"
        ),
        "limitations": [
            "KVM API 12 only checks a host acceleration prerequisite.",
            "Missing KVM does not prove all unaccelerated emulation impossible.",
            "KVM access does not establish official Google Play ARM64 availability.",
            "KVM access does not establish emulator acceptance or game compatibility.",
            "No APK or emulator download/boot is permitted by this preflight.",
        ],
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()
    host = platform.system()
    arch = platform.machine()
    probe = inspect_kvm() if host == "Linux" and arch.lower() in ELIGIBLE_ARCHS else {
        "status": "NOT_PROBED_INCOMPATIBLE_HOST", "api_version": None}
    report = evaluate(host, arch, probe, os.getenv("ImageOS", "UNKNOWN"))
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n",
                           encoding="utf8")
    print(json.dumps(report, sort_keys=True))
    # Absent KVM is a valid diagnostic outcome, not a workflow crash.
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
