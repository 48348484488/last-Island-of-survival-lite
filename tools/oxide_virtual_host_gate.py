#!/usr/bin/env python3
"""Host-only Android ARM64 virtualization gate.

No game APK, Android emulator, Play services, attestation or game protection
is installed, modified or probed. KVM is only a host prerequisite.
"""
import argparse
import fcntl
import json
import os
import platform
import stat
from pathlib import Path

KVM_GET_API_VERSION = 0xAE00
EXPECTED_KVM_API_VERSION = 12


def probe_kvm(path="/dev/kvm"):
    """Return (device_kind, kvm_api_version, error); fail closed on all errors."""
    try:
        st = os.stat(path)
    except FileNotFoundError:
        return ("ABSENT", None, "NO_KVM_DEVICE")
    except OSError as exc:
        return ("INACCESSIBLE", None, "STAT_" + type(exc).__name__)
    if not stat.S_ISCHR(st.st_mode):
        return ("NOT_CHARACTER_DEVICE", None, "NOT_CHARACTER_DEVICE")
    try:
        fd = os.open(path, os.O_RDWR | os.O_CLOEXEC)
    except OSError as exc:
        return ("CHARACTER_DEVICE", None, "OPEN_" + type(exc).__name__)
    try:
        version = fcntl.ioctl(fd, KVM_GET_API_VERSION, 0)
        return ("CHARACTER_DEVICE", int(version), None)
    except OSError as exc:
        return ("CHARACTER_DEVICE", None, "IOCTL_" + type(exc).__name__)
    finally:
        os.close(fd)


def classify(system, arch, device_kind, api_version, error=None):
    native = system == "Linux" and arch in ("aarch64", "arm64")
    if not native:
        status = "BLOCKED_NOT_NATIVE_LINUX_ARM64"
    elif device_kind == "ABSENT":
        status = "BLOCKED_NO_KVM"
    elif device_kind != "CHARACTER_DEVICE":
        status = "BLOCKED_INVALID_KVM_DEVICE"
    elif api_version != EXPECTED_KVM_API_VERSION:
        status = "BLOCKED_KVM_NOT_USABLE"
    else:
        status = "HOST_KVM_API_AVAILABLE_BOOT_UNTESTED"
    return {
        "schema_version": 1,
        "host_os": system,
        "host_arch": arch,
        "kvm_device_kind": device_kind,
        "kvm_api_version": api_version,
        "kvm_error": error,
        "status": status,
        "native_arm64_host": native,
        "kvm_prerequisite_verified": status == "HOST_KVM_API_AVAILABLE_BOOT_UNTESTED",
        "official_arm64_play_image_verified": False,
        "android_booted": False,
        "google_play_services_verified": False,
        "apk_original_install_verified": False,
        "gameplay_verified": False,
        "note": "KVM API availability is a host prerequisite, not proof that an ARM64 Android Emulator boots or a game is compatible."
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", type=Path, default=Path("reports/arm64-host-preflight.json"))
    args = parser.parse_args()
    device_kind, version, error = probe_kvm()
    result = classify(platform.system(), platform.machine(), device_kind, version, error)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf8")
    print(json.dumps(result, sort_keys=True))
    # Host blocked still means the *diagnostic* completed. Never claim gameplay.
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
