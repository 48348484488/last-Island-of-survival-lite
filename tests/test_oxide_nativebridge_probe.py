"""Native bridge proof is only for a separate harmless diagnostic APK."""
import json
import tempfile
import unittest
from pathlib import Path

from tools.oxide_nativebridge_probe.assess_logcat import assess


def log_entry(pid=4321, success=True):
    status = ("ARM64_JNI_EXECUTION_VERIFIED value=42"
              if success else "ARM64_JNI_EXECUTION_FAILED")
    return (f"10-15 21:30:02.123 {pid} {pid} I OXIDE_ARM64_BRIDGE: "
            f"{status} supported_abis=[x86_64, arm64-v8a]\n")


class NativeBridgeDiagnosticTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)
        self.d = Path(self.tmp.name)
        self.base()

    def write(self, name, value):
        (self.d / name).write_text(value, encoding="utf8")

    def base(self):
        for name, data in (
            ("boot.txt", "1\n"),
            ("abi.txt", "x86_64\n"),
            ("abilist64.txt", "x86_64,arm64-v8a\n"),
            ("native-bridge.txt", "libndk_translation.so\n"),
            ("diagnostic-install.txt", "Success\n"),
            ("diagnostic-launch.txt", "Status: ok\nActivity: org.oxideprobe/.ProbeActivity\n"),
            ("diagnostic-pid.txt", "4321\n"),
            ("diagnostic-logcat.txt", log_entry()),
        ):
            self.write(name, data)

    def test_exact_native_arm64_jni_execution_verified(self):
        r = assess(self.d)
        self.assertTrue(r["arm64_jni_executed"])
        self.assertEqual(r["status"], "ARM64_JNI_EXECUTED_VIA_NATIVE_BRIDGE_ORIGINAL_GAME_UNTESTED")
        self.assertEqual(r["diagnostic_pid"], "4321")
        self.assertFalse(r["original_oxide_gameplay_verified"])
        self.assertFalse(r["original_oxide_apk_installed"])

    def test_advertised_abi_only_not_proof(self):
        self.write("diagnostic-logcat.txt", "")
        self.assertFalse(assess(self.d)["arm64_jni_executed"])

    def test_bridge_library_name_not_proof(self):
        self.write("native-bridge.txt", "")
        self.assertEqual(assess(self.d)["status"], "BLOCKED_NO_VALID_ARM64_NATIVE_BRIDGE")

    def test_foreign_process_pid_cannot_claim_success(self):
        self.write("diagnostic-logcat.txt", log_entry(pid=5555))
        r = assess(self.d)
        self.assertFalse(r["arm64_jni_executed"])
        self.assertEqual(r["diagnostic_success_log_pids"], ["5555"])

    def test_failed_tag_blocks_positive_verdict(self):
        self.write("diagnostic-logcat.txt", log_entry() + log_entry(success=False))
        self.assertFalse(assess(self.d)["arm64_jni_executed"])

    def test_different_return_value_fails(self):
        self.write("diagnostic-logcat.txt", log_entry().replace("value=42", "value=41"))
        self.assertFalse(assess(self.d)["arm64_jni_executed"])

    def test_missing_install_result_fails(self):
        self.write("diagnostic-install.txt", "INSTALL_FAILED_NO_MATCHING_ABIS\n")
        self.assertEqual(assess(self.d)["status"], "BLOCKED_DIAGNOSTIC_APP_NOT_INSTALLED")

    def test_failed_activity_start_fails(self):
        self.write("diagnostic-launch.txt", "Error: Activity class does not exist\n")
        self.assertEqual(assess(self.d)["status"], "BLOCKED_DIAGNOSTIC_ACTIVITY_NOT_STARTED")

    def test_missing_pid_fails(self):
        self.write("diagnostic-pid.txt", "")
        self.assertFalse(assess(self.d)["arm64_jni_executed"])

    def test_no_android_boot_fails(self):
        self.write("boot.txt", "0\n")
        self.assertEqual(assess(self.d)["status"], "BLOCKED_AVD_NOT_BOOTED")

    def test_missing_reports_cannot_claim_positive(self):
        with tempfile.TemporaryDirectory() as d:
            r = assess(d)
            self.assertFalse(r["arm64_jni_executed"])
            self.assertFalse(r["original_oxide_gameplay_verified"])

    def test_source_never_mutates_original_app(self):
        root = Path(__file__).resolve().parents[1]
        source = (root / "tools/oxide_nativebridge_probe/run_on_avd.sh").read_text()
        build = (root / "tools/oxide_nativebridge_probe/build_probe.sh").read_text()
        java = (root / "tools/oxide_nativebridge_probe/ProbeActivity.java").read_text()
        self.assertIn('org.oxideprobe/.ProbeActivity', source)
        self.assertIn('System.loadLibrary("oxideprobe")', java)
        self.assertIn('nativeCheck()', java)
        self.assertNotIn("com.catsbit.oxidesurvivalisland", source + build + java)
        self.assertNotIn("gh release download", source + build)
        self.assertNotIn("pm grant", source + build)
        self.assertIn("temporary-debug-keystore.jks", build)
        self.assertIn("rm -f", build)


if __name__ == "__main__":
    unittest.main()
