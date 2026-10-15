import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

from tools.oxide_managed_arm64_gate import classify, installed

APP = "package:/data/app/~~example/com.android.vending-123/base.apk\n"
GMS = "package:/data/app/~~example/com.google.android.gms-456/base.apk\n"


class ManagedHostGateTests(unittest.TestCase):
    def test_all_necessary_packages_present_is_still_not_play_integrity(self):
        r = classify("1\n", "arm64-v8a\n", APP, GMS)
        self.assertEqual(r["status"], "ARM64_GOOGLE_PACKAGES_PRESENT_TRUST_UNVERIFIED")
        self.assertTrue(r["proceed_with_original_apk_download"])
        self.assertEqual(r["android_certified"], "UNKNOWN")
        self.assertEqual(r["play_integrity_verdict"], "NOT_OBSERVED")
        self.assertFalse(r["gameplay_confirmed"])

    def test_android_not_booted_is_blocked(self):
        self.assertEqual(classify("0", "arm64-v8a", APP, GMS)["status"],
                         "BLOCKED_REMOTE_ANDROID_NOT_BOOTED")

    def test_wrong_abi_blocks_before_downloading_one_gigabyte(self):
        self.assertEqual(classify("1", "x86_64", APP, GMS)["status"],
                         "BLOCKED_REMOTE_NOT_NATIVE_ARM64")

    def test_missing_play_store_blocks_expensive_apk_download(self):
        self.assertEqual(classify("1", "arm64-v8a", "", GMS)["status"],
                         "BLOCKED_NO_GOOGLE_PLAY_STORE_PACKAGE")

    def test_missing_play_services_blocks_download(self):
        self.assertEqual(classify("1", "arm64-v8a", APP, "")["status"],
                         "BLOCKED_NO_GOOGLE_PLAY_SERVICES_PACKAGE")

    def test_error_output_not_counted_as_installed(self):
        self.assertFalse(installed("Error: Package com.android.vending unknown\n"))

    def test_incomplete_pm_path_not_counted(self):
        self.assertFalse(installed("package:unknown\n"))

    def test_command_line_gate_fails_closed_without_android(self):
        script = Path(__file__).resolve().parents[1] / "tools" / "oxide_managed_arm64_gate.py"
        with tempfile.TemporaryDirectory() as directory:
            p = Path(directory)
            proc = subprocess.run([sys.executable, str(script), "--reports", str(p)],
                                  capture_output=True, text=True, timeout=10, cwd=p)
            self.assertEqual(proc.returncode, 2, proc.stderr)
            result = json.loads((p / "managed-arm64-gate.json").read_text())
            self.assertFalse(result["proceed_with_original_apk_download"])
            self.assertFalse(result["gameplay_confirmed"])

    def test_command_line_gate_accepts_observed_packages_only_as_eligibility(self):
        script = Path(__file__).resolve().parents[1] / "tools" / "oxide_managed_arm64_gate.py"
        with tempfile.TemporaryDirectory() as directory:
            p = Path(directory)
            for name, value in [("boot.txt", "1"), ("abi.txt", "arm64-v8a"),
                                ("play-store-pm-path.txt", APP),
                                ("play-services-pm-path.txt", GMS)]:
                (p / name).write_text(value)
            proc = subprocess.run([sys.executable, str(script), "--reports", str(p)],
                                  capture_output=True, text=True, timeout=10, cwd=p)
            self.assertEqual(proc.returncode, 0, proc.stderr)
            result = json.loads(proc.stdout)
            self.assertTrue(result["proceed_with_original_apk_download"])
            self.assertFalse(result["gameplay_confirmed"])


if __name__ == "__main__":
    unittest.main()
