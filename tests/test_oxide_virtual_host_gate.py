import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest
from tools.oxide_virtual_host_gate import classify, probe_kvm


class HostGateTests(unittest.TestCase):
    def test_missing_kvm_is_blocked(self):
        report = classify("Linux", "aarch64", "ABSENT", None, "NO_KVM_DEVICE")
        self.assertEqual(report["status"], "BLOCKED_NO_KVM")
        self.assertFalse(report["kvm_prerequisite_verified"])
        self.assertFalse(report["gameplay_verified"])

    def test_access_denied_is_not_treated_as_kvm_available(self):
        report = classify("Linux", "aarch64", "CHARACTER_DEVICE", None, "OPEN_PermissionError")
        self.assertEqual(report["status"], "BLOCKED_KVM_NOT_USABLE")

    def test_invalid_api_version_is_blocked(self):
        report = classify("Linux", "aarch64", "CHARACTER_DEVICE", 0)
        self.assertEqual(report["status"], "BLOCKED_KVM_NOT_USABLE")

    def test_valid_arm64_kvm_api_still_never_proves_android_boot(self):
        report = classify("Linux", "aarch64", "CHARACTER_DEVICE", 12)
        self.assertEqual(report["status"], "HOST_KVM_API_AVAILABLE_BOOT_UNTESTED")
        self.assertTrue(report["kvm_prerequisite_verified"])
        for field in ("official_arm64_play_image_verified", "android_booted",
                      "google_play_services_verified", "apk_original_install_verified",
                      "gameplay_verified"):
            self.assertFalse(report[field])

    def test_x86_with_kvm_does_not_prove_native_arm64(self):
        report = classify("Linux", "x86_64", "CHARACTER_DEVICE", 12)
        self.assertEqual(report["status"], "BLOCKED_NOT_NATIVE_LINUX_ARM64")

    def test_macos_requires_separate_hypervisor_probe(self):
        report = classify("Darwin", "arm64", "ABSENT", None)
        self.assertEqual(report["status"], "BLOCKED_NOT_NATIVE_LINUX_ARM64")

    def test_missing_device_probe_does_not_require_privilege(self):
        with tempfile.TemporaryDirectory() as d:
            self.assertEqual(probe_kvm(str(Path(d) / "missing"))[0], "ABSENT")

    def test_non_character_device_is_rejected(self):
        with tempfile.TemporaryDirectory() as d:
            file = Path(d) / "regular"
            file.write_text("nothing")
            self.assertEqual(probe_kvm(str(file))[0], "NOT_CHARACTER_DEVICE")

    def test_host_only_cli_produces_explicit_false_gameplay(self):
        script = Path(__file__).resolve().parents[1] / "tools" / "oxide_virtual_host_gate.py"
        with tempfile.TemporaryDirectory() as d:
            output = Path(d) / "report.json"
            run = subprocess.run([sys.executable, str(script), "--output", str(output)],
                                 cwd=d, capture_output=True, text=True, timeout=10)
            self.assertEqual(run.returncode, 0, run.stderr)
            report = json.loads(output.read_text())
            self.assertFalse(report["gameplay_verified"])
            self.assertFalse(report["android_booted"])
            self.assertEqual(report["host_arch"], os.uname().machine)


if __name__ == "__main__":
    unittest.main()
