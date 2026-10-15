"""Exercise the actual Android diagnostic shell runner with fake ADB responses.

This is a real shell subprocess integration test, not an Android emulator.
It must never claim JNI or Oxide gameplay on fabricated data.
"""
import json
import os
import subprocess
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SCRIPT = ROOT / "tools" / "oxide_nativebridge_probe" / "run_on_avd.sh"


class ProbeShellFailureEvidenceTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.directory = Path(self.temp.name)
        self.reports = self.directory / "reports"
        self.reports.mkdir()
        (self.reports / "diagnostic.apk").write_bytes(b"test-fixture-NOT-APK")
        bindir = self.directory / "bin"
        bindir.mkdir()
        self.adb = bindir / "adb"
        self.env = dict(os.environ, PATH=str(bindir) + os.pathsep + os.environ.get("PATH", ""))

    def invoke(self, bridge="libndk_translation.so", install_exit=1):
        fake_adb = f"""#!/bin/sh
case "$*" in
  *"getprop sys.boot_completed"*) echo 1; exit 0;;
  *"getprop ro.product.cpu.abi"*) echo x86_64; exit 0;;
  *"getprop ro.product.cpu.abilist64"*) echo x86_64,arm64-v8a; exit 0;;
  *"getprop ro.dalvik.vm.native.bridge"*) echo "{bridge}"; exit 0;;
  *"pm path com.android.vending"*) echo package:/system/priv-app/Vending/Vending.apk; exit 0;;
  *"pm path com.google.android.gms"*) echo package:/system/priv-app/Gms/Gms.apk; exit 0;;
  *"logcat -d"*) echo "diagnostic native-bridge failure log"; exit 0;;
  *"logcat -c"*) exit 0;;
  *"install -r"*) echo INSTALL_FAILED_NO_MATCHING_ABIS; exit {install_exit};;
  *) echo "unexpected mocked ADB request: $*" >&2; exit 98;;
esac
"""
        self.adb.write_text(fake_adb)
        self.adb.chmod(0o755)
        result = subprocess.run(["bash", str(SCRIPT), str(self.reports / "diagnostic.apk"), str(self.reports)],
                                env=self.env, cwd=ROOT, text=True, capture_output=True, timeout=12)
        return result

    def test_install_failure_preserves_logcat_and_json(self):
        p = self.invoke()
        self.assertNotEqual(p.returncode, 0)
        self.assertTrue((self.reports / "diagnostic-logcat.txt").exists())
        self.assertIn("INSTALL_FAILED_NO_MATCHING_ABIS",
                      (self.reports / "diagnostic-install.txt").read_text())
        verdict = json.loads((self.reports / "nativebridge-jni-verdict.json").read_text())
        self.assertEqual(verdict["status"], "BLOCKED_DIAGNOSTIC_APP_NOT_INSTALLED")
        self.assertFalse(verdict["arm64_jni_executed"])
        self.assertFalse(verdict["original_oxide_gameplay_verified"])

    def test_missing_native_bridge_blocks_install_but_preserves_verdict(self):
        p = self.invoke(bridge="")
        self.assertNotEqual(p.returncode, 0)
        verdict = json.loads((self.reports / "nativebridge-jni-verdict.json").read_text())
        self.assertEqual(verdict["status"], "BLOCKED_NO_VALID_ARM64_NATIVE_BRIDGE")
        self.assertFalse((self.reports / "diagnostic-install.txt").exists())
        self.assertIn("diagnostic native-bridge failure log",
                      (self.reports / "diagnostic-logcat.txt").read_text())


if __name__ == "__main__":
    unittest.main()
