"""Static no-account, no-bypass validation of official ARM64 original APK trial."""
import subprocess
import unittest
from pathlib import Path

SCRIPT = Path(__file__).resolve().parents[1] / "tools" / "oxide_official_arm64_game_trial.sh"


class OfficialTrialTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.script = SCRIPT.read_text(encoding="utf-8")

    def test_shell_script_parses(self):
        p = subprocess.run(["bash", "-n", str(SCRIPT)], capture_output=True, text=True, timeout=5)
        self.assertEqual(p.returncode, 0, p.stderr)

    def test_readiness_before_large_apk_download(self):
        self.assertLess(self.script.index("tools/oxide_managed_arm64_gate.py"),
                        self.script.index("gh release download"))

    def test_sha256_before_install(self):
        self.assertLess(self.script.index("tools/oxide_original_hash_gate.py"),
                        self.script.index("install-multiple -r"))

    def test_unchanged_splits_and_original_activity(self):
        for name in ("base.apk", "split_config.arm64_v8a.apk",
                     "split_config.xxhdpi.apk", "split_UnityDataAssetPack.apk"):
            self.assertIn(name, self.script)
        self.assertIn("com.catsbit.oxidesurvivalisland.OxideActivity", self.script)
        self.assertNotIn("pm grant", self.script)
        self.assertNotIn("setprop", self.script)
        self.assertNotIn("resign", self.script.lower())

    def test_original_runtime_requires_35_seconds(self):
        self.assertIn("tools/oxide_capture_samples.py", self.script)
        self.assertIn("tools/oxide_runtime_assess.py", self.script)

    def test_portable_timeout_used_for_macos(self):
        self.assertIn("tools/oxide_process_timeout.py --seconds 300", self.script)
        self.assertIn("tools/oxide_process_timeout.py --seconds 40", self.script)
        self.assertNotIn("timeout 300 ", self.script)
        self.assertNotIn("timeout 40 ", self.script)

    def test_logcat_retained_on_failure(self):
        self.assertIn("trap collect_logcat EXIT", self.script)


if __name__ == "__main__":
    unittest.main()
