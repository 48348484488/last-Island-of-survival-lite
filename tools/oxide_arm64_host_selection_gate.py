#!/usr/bin/env python3
"""Fail-closed host selector over two previously collected tiny CI JSON reports.

A runner reporting KVM or WHPX support is only a candidate for further metadata
inspection. It NEVER authorizes Android image download, original APK execution,
device certification changes, or any action on anti-cheat / Play Integrity.
"""
import argparse
import hashlib
import json
from pathlib import Path


def _safe_json(blob):
    try:
        result = json.loads(blob.decode("utf8"))
    except (UnicodeDecodeError, ValueError, TypeError):
        return None
    return result if isinstance(result, dict) else None


def assess_hosts(linux_raw, windows_raw):
    linux = _safe_json(linux_raw)
    windows = _safe_json(windows_raw)

    def blocked(reason):
        return {
            "status": "BLOCKED_INVALID_HOST_EVIDENCE",
            "reason": reason,
            "evidence_validated": False,
            "native_arm64_vm_acceleration_prerequisite_met": False,
            "gameplay_verified": False,
            "heavy_android_or_apk_trial_authorized": False,
        }

    if linux is None or windows is None:
        return blocked("MISSING_OR_INVALID_JSON")
    if not (
        linux.get("host_os") == "Linux"
        and linux.get("host_machine") == "aarch64"
        and linux.get("runner_image") == "ubuntu26-arm64"
        and windows.get("host_architecture") == "Arm64"
        and type(windows.get("windows_build")) is int
        and windows["windows_build"] >= 26100
    ):
        return blocked("WRONG_OS_ARCH_OR_RUNNER")

    linux_kvm = linux.get("kvm")
    if not isinstance(linux_kvm, dict):
        return blocked("LINUX_KVM_REPORT_MISSING")

    linux_ready = linux.get("status") == "ARM64_HOST_KVM_PREREQUISITE_MET"
    linux_blocked = linux.get("status") == "ARM64_HOST_KVM_NOT_AVAILABLE_FOR_ACCELERATED_AVD"
    windows_ready = windows.get("status") == "WHPX_ARM64_PLATFORM_PREREQUISITE_MET"
    windows_blocked = windows.get("status") in {
        "WHPX_HYPERVISOR_NOT_PRESENT",
        "WHPX_HYPERVISOR_QUERY_FAILED",
        "WHPX_FEATURES_QUERY_FAILED",
        "WHPX_ARM64_FEATURE_NOT_EXPOSED",
    }
    if not (linux_ready or linux_blocked) or not (windows_ready or windows_blocked):
        return blocked("UNKNOWN_HOST_RESULT")

    if (linux.get("native_arm64_kvm_prerequisite_verified") is not linux_ready
            or windows.get("whpx_arm64_prerequisite_verified") is not windows_ready):
        return blocked("HOST_CAPABILITY_STATUS_CONTRADICTION")
    if linux_ready and (
            linux_kvm.get("status") != "KVM_API_12_ACCESSIBLE"
            or linux_kvm.get("api_version") != 12):
        return blocked("KVM_API_EVIDENCE_CONTRADICTION")
    if linux_blocked and linux_kvm.get("status") == "KVM_API_12_ACCESSIBLE":
        return blocked("FALSE_LINUX_BLOCKER")
    if windows_ready and (
            windows.get("windows_hypervisor_present") is not True
            or windows.get("whpx_arm64_support_bit") is not True):
        return blocked("WHPX_CAPABILITY_EVIDENCE_CONTRADICTION")
    if windows_blocked and windows.get("whpx_arm64_prerequisite_verified") is not False:
        return blocked("FALSE_WINDOWS_BLOCKER")

    false_fields = (
        (linux, (
            "android_or_oxide_executed", "original_apk_modified",
            "heavy_android_retry_authorized", "gameplay_verified",
            "arm64_android_system_image_verified",
            "official_google_play_arm64_image_verified",
        )),
        (windows, (
            "original_android_vm_started", "original_oxide_apks_downloaded",
            "original_oxide_apks_modified", "heavy_android_retry_authorized",
            "original_gameplay_verified", "official_android_emulator_available",
            "official_arm64_gms_system_image_available",
        )),
    )
    if any(report.get(field) is not False for report, fields in false_fields
           for field in fields):
        return blocked("UNSUPPORTED_ANDROID_OR_GAMEPLAY_CLAIM")

    any_prerequisite = linux_ready or windows_ready
    return {
        "status": ("HOST_ACCELERATION_PREREQUISITE_ONLY_NO_LAUNCH"
                   if any_prerequisite
                   else "BOTH_NEW_ARM64_RUNNERS_LACK_HYPERVISOR_ACCELERATION"),
        "evidence_validated": True,
        "linux_ubuntu26": {
            "status": linux["status"],
            "kvm": linux_kvm["status"],
            "source_sha256": hashlib.sha256(linux_raw).hexdigest(),
        },
        "windows_arm64_vs2026": {
            "status": windows["status"],
            "whpx_hypervisor_present": windows.get("windows_hypervisor_present"),
            "source_sha256": hashlib.sha256(windows_raw).hexdigest(),
        },
        "native_arm64_vm_acceleration_prerequisite_met": any_prerequisite,
        "android_system_image_verified": False,
        "legitimate_google_services_environment_verified": False,
        "original_activity_35s_verified": False,
        "gameplay_verified": False,
        "heavy_android_or_apk_trial_authorized": False,
        "next_action": ("CHECK_OFFICIAL_ANDROID_IMAGE_SUPPORT_ONLY"
                        if any_prerequisite else "STOP_NO_NEW_ACCELERATED_ARM64_HOST"),
        "limits": [
            "Neither host test tried Android or the protected original game.",
            "No KVM/WHPX does not preclude all unaccelerated guest emulation.",
            "A future host capability alone does not authorize Android/APK downloads.",
            "No conclusion about the original app's security decision is supported.",
        ],
    }


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--linux-json", type=Path, required=True)
    p.add_argument("--windows-json", type=Path, required=True)
    p.add_argument("--output", type=Path, required=True)
    args = p.parse_args()
    verdict = assess_hosts(args.linux_json.read_bytes(), args.windows_json.read_bytes())
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(verdict, indent=2, sort_keys=True) + "\n")
    print(json.dumps(verdict, sort_keys=True))
    return 0 if verdict["evidence_validated"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
