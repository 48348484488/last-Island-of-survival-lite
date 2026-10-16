"""Regress the offline original-APK host stop gate with synthetic host JSON."""
import json
import unittest

from tools.oxide_arm64_host_selection_gate import assess_hosts


def linux():
    return {
        "host_os": "Linux",
        "host_machine": "aarch64",
        "runner_image": "ubuntu26-arm64",
        "status": "ARM64_HOST_KVM_NOT_AVAILABLE_FOR_ACCELERATED_AVD",
        "kvm": {"status": "DEVICE_ABSENT", "api_version": None},
        "native_arm64_kvm_prerequisite_verified": False,
        "android_or_oxide_executed": False,
        "original_apk_modified": False,
        "heavy_android_retry_authorized": False,
        "gameplay_verified": False,
        "arm64_android_system_image_verified": False,
        "official_google_play_arm64_image_verified": False,
    }


def windows():
    return {
        "host_architecture": "Arm64",
        "windows_build": 26200,
        "status": "WHPX_HYPERVISOR_NOT_PRESENT",
        "whpx_arm64_prerequisite_verified": False,
        "windows_hypervisor_present": False,
        "whpx_arm64_support_bit": False,
        "original_android_vm_started": False,
        "original_oxide_apks_downloaded": False,
        "original_oxide_apks_modified": False,
        "heavy_android_retry_authorized": False,
        "original_gameplay_verified": False,
        "official_android_emulator_available": False,
        "official_arm64_gms_system_image_available": False,
    }


def verify(l=None, w=None):
    return assess_hosts(json.dumps(l if l is not None else linux()).encode(),
                        json.dumps(w if w is not None else windows()).encode())


class HostSelectionTests(unittest.TestCase):
    def test_two_realistic_missing_hypervisors_block_heavy_test(self):
        r = verify()
        self.assertEqual(r["status"],
                         "BOTH_NEW_ARM64_RUNNERS_LACK_HYPERVISOR_ACCELERATION")
        self.assertTrue(r["evidence_validated"])
        self.assertEqual(r["next_action"], "STOP_NO_NEW_ACCELERATED_ARM64_HOST")
        self.assertFalse(r["heavy_android_or_apk_trial_authorized"])
        self.assertFalse(r["gameplay_verified"])

    def test_linux_kvm_ready_does_not_authorize_game_or_android(self):
        l = linux()
        l["status"] = "ARM64_HOST_KVM_PREREQUISITE_MET"
        l["kvm"] = {"status": "KVM_API_12_ACCESSIBLE", "api_version": 12}
        l["native_arm64_kvm_prerequisite_verified"] = True
        r = verify(l=l)
        self.assertEqual(r["status"], "HOST_ACCELERATION_PREREQUISITE_ONLY_NO_LAUNCH")
        self.assertFalse(r["heavy_android_or_apk_trial_authorized"])
        self.assertFalse(r["legitimate_google_services_environment_verified"])

    def test_windows_whpx_ready_does_not_authorize_game_or_android(self):
        w = windows()
        w["status"] = "WHPX_ARM64_PLATFORM_PREREQUISITE_MET"
        w["windows_hypervisor_present"] = True
        w["whpx_arm64_support_bit"] = True
        w["whpx_arm64_prerequisite_verified"] = True
        r = verify(w=w)
        self.assertEqual(r["status"], "HOST_ACCELERATION_PREREQUISITE_ONLY_NO_LAUNCH")
        self.assertFalse(r["heavy_android_or_apk_trial_authorized"])

    def test_false_linux_positive_requires_actual_api12_evidence(self):
        l = linux()
        l["status"] = "ARM64_HOST_KVM_PREREQUISITE_MET"
        l["native_arm64_kvm_prerequisite_verified"] = True
        self.assertEqual(verify(l=l)["reason"], "KVM_API_EVIDENCE_CONTRADICTION")

    def test_false_windows_positive_requires_arm64_support_bit(self):
        w = windows()
        w["status"] = "WHPX_ARM64_PLATFORM_PREREQUISITE_MET"
        w["whpx_arm64_prerequisite_verified"] = True
        w["windows_hypervisor_present"] = True
        self.assertEqual(verify(w=w)["reason"], "WHPX_CAPABILITY_EVIDENCE_CONTRADICTION")

    def test_altered_host_architecture_is_rejected(self):
        l = linux()
        l["host_machine"] = "x86_64"
        self.assertEqual(verify(l=l)["reason"], "WRONG_OS_ARCH_OR_RUNNER")

    def test_report_claiming_gameplay_is_rejected(self):
        w = windows()
        w["original_gameplay_verified"] = True
        self.assertEqual(verify(w=w)["reason"], "UNSUPPORTED_ANDROID_OR_GAMEPLAY_CLAIM")

    def test_missing_host_report_rejected(self):
        result = assess_hosts(b"", json.dumps(windows()).encode())
        self.assertEqual(result["status"], "BLOCKED_INVALID_HOST_EVIDENCE")
        self.assertFalse(result["heavy_android_or_apk_trial_authorized"])

    def test_future_unknown_status_fails_closed(self):
        l = linux()
        l["status"] = "PROBABLY_OK"
        self.assertEqual(verify(l=l)["reason"], "UNKNOWN_HOST_RESULT")

    def test_source_sha256_is_recorded_for_both_real_reports(self):
        r = verify()
        self.assertRegex(r["linux_ubuntu26"]["source_sha256"], r"^[a-f0-9]{64}$")
        self.assertRegex(r["windows_arm64_vs2026"]["source_sha256"], r"^[a-f0-9]{64}$")


if __name__ == "__main__":
    unittest.main()
