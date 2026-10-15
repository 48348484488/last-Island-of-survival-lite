"""Regression for free GitHub macOS ARM64 official Android, one original APK trial.

Static workflow checks do not prove boot, Play Integrity, or gameplay.
"""
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
HOST = ROOT / ".github/workflows/oxide-m3-macos-arm64-host.yml"
RUN = ROOT / ".github/workflows/oxide-m4-macos-arm64-original.yml"
SCRIPT = ROOT / "tools/oxide_official_arm64_game_trial.sh"


class MacOriginalWorkflowTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.host = HOST.read_text()
        cls.trial = RUN.read_text()
        cls.script = SCRIPT.read_text()

    def test_native_standard_macos_arm_runner(self):
        for body in (self.host, self.trial):
            self.assertIn("runs-on: macos-15", body)
        self.assertNotIn("macos-15-xlarge", self.trial)

    def test_real_emulator_hvf_not_nonexistent_sysctl(self):
        self.assertIn("Hypervisor.Framework", self.host)
        self.assertIn("MAC_ARM64_EMULATOR_HVF_ACCEL_AVAILABLE_BOOT_UNTESTED", self.host)
        self.assertNotIn('elif hv != "1"', self.host)
        self.assertIn("grep -Fq 'Hypervisor.Framework'", self.trial)
        self.assertIn('"$ANDROID_HOME/emulator/emulator" -accel-check', self.trial)

    def test_matching_official_arm64_play_image(self):
        self.assertIn("api-level: 35", self.trial)
        self.assertIn("target: google_apis_playstore", self.trial)
        self.assertIn("arch: arm64-v8a", self.trial)
        self.assertNotIn("ANDROID_I_WANT_MY_TCG", self.trial)
        self.assertNotIn("-accel off", self.trial)

    def test_one_attempt_no_schedule_or_external_account(self):
        self.assertIn("timeout-minutes: 28", self.trial)
        self.assertNotIn("  schedule:", self.trial)
        self.assertNotIn("  workflow_run:", self.trial)
        self.assertNotIn("GENYMOTION_API_TOKEN", self.trial)
        self.assertNotIn("gmsaas", self.trial)

    def test_preflight_first_and_originals_never_modified(self):
        self.assertLess(self.trial.index("Verify actual Apple Silicon"),
                        self.trial.index("Boot official Android"))
        self.assertIn("oxide_official_arm64_game_trial.sh", self.trial)
        self.assertLess(self.script.index("tools/oxide_managed_arm64_gate.py"),
                        self.script.index("gh release download"))
        self.assertLess(self.script.index("tools/oxide_original_hash_gate.py"),
                        self.script.index("install-multiple -r"))
        self.assertNotIn("pm grant", self.script)
        self.assertNotIn("--resign", self.script)

    def test_evidence_is_saved_on_failure(self):
        self.assertIn("if: always()", self.trial)
        self.assertIn("oxide-macos-arm64-original-game-evidence", self.trial)
        self.assertIn("tools/oxide_capture_samples.py", self.script)
        self.assertIn("tools/oxide_runtime_assess.py", self.script)


if __name__ == "__main__":
    unittest.main()
