"""Genuine native ARM64 vs x86 Android with independently proven ARM64 JNI."""
import json
import tempfile
import unittest
from pathlib import Path

from tools.oxide_runtime_environment import evaluate_runtime_environment

PROVEN = "ARM64_JNI_EXECUTED_VIA_NATIVE_BRIDGE_ORIGINAL_GAME_UNTESTED"


class RuntimeEnvironmentTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)
        self.root = Path(self.tmp.name)

    def write(self, file, value):
        (self.root / file).write_text(value)

    def translated(self):
        self.write("boot.txt", "1")
        self.write("abi.txt", "x86_64")
        self.write("abilist64.txt", "x86_64,arm64-v8a")
        self.write("native-bridge.txt", "libndk_translation.so")
        proof = {
            "status": PROVEN,
            "official_android_booted": True,
            "host_abi_x86_64": True,
            "arm64_abi_advertised": True,
            "native_bridge_configured": True,
            "diagnostic_activity_started": True,
            "diagnostic_package_installed": True,
            "arm64_jni_executed": True,
            "original_oxide_apk_installed": False,
            "original_oxide_gameplay_verified": False,
            "diagnostic_pid": "2754",
            "diagnostic_success_log_pids": ["2754"],
        }
        self.write("nativebridge-prerequisite.json", json.dumps(proof))
        return proof

    def test_native_arm64_requires_android_boot(self):
        self.write("abi.txt", "arm64-v8a")
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])
        self.write("boot.txt", "1")
        x = evaluate_runtime_environment(self.root)
        self.assertEqual(x["status"], "ARM64_NATIVE_ANDROID")
        self.assertTrue(x["android_arm64_native"])
        self.assertFalse(x["arm64_jni_translation_proven_in_predecessor"])

    def test_real_x86_translated_jni_is_supported_but_not_gameplay(self):
        self.translated()
        x = evaluate_runtime_environment(self.root)
        self.assertTrue(x["original_arm64_runtime_eligible"])
        self.assertEqual(x["status"], "X86_64_ANDROID_ARM64_NATIVE_BRIDGE_JNI_PROVEN")
        self.assertFalse(x["android_arm64_native"])
        self.assertTrue(x["arm64_jni_translation_proven_in_predecessor"])
        self.assertFalse(x["device_integrity_certified"])
        self.assertFalse(x["gameplay_verified"])

    def test_bridge_library_presence_without_jni_proof_not_enough(self):
        self.translated()
        (self.root / "nativebridge-prerequisite.json").unlink()
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])

    def test_invalid_or_malformed_proof_rejected(self):
        self.translated()
        self.write("nativebridge-prerequisite.json", "bad{")
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])
        self.write("nativebridge-prerequisite.json", "[42]")
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])

    def test_different_native_bridge_rejected(self):
        self.translated()
        self.write("native-bridge.txt", "")
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])

    def test_missing_arm64_guest_abi_rejected(self):
        self.translated()
        self.write("abilist64.txt", "x86_64")
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])

    def test_wrong_pid_in_predecessor_proof_rejected(self):
        proof = self.translated()
        proof["diagnostic_success_log_pids"] = ["2296"]
        self.write("nativebridge-prerequisite.json", json.dumps(proof))
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])

    def test_unconfirmed_jni_test_rejected(self):
        proof = self.translated()
        proof["arm64_jni_executed"] = False
        self.write("nativebridge-prerequisite.json", json.dumps(proof))
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])

    def test_prior_original_game_not_claimed(self):
        proof = self.translated()
        proof["original_oxide_gameplay_verified"] = True
        self.write("nativebridge-prerequisite.json", json.dumps(proof))
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])

    def test_boot_still_required_after_jni_proof(self):
        self.translated()
        self.write("boot.txt", "0")
        self.assertFalse(evaluate_runtime_environment(self.root)["original_arm64_runtime_eligible"])


if __name__ == "__main__":
    unittest.main()
