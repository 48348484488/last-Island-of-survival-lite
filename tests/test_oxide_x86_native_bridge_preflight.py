"""Keep one-shot official x86_64 ARM64-bridge host diagnosis noninvasive.

A native-bridge property or ABI string alone never establishes translation.
"""
import unittest
from pathlib import Path

WORKFLOW = (Path(__file__).resolve().parents[1] /
            ".github/workflows/oxide-m3-x86-arm64-bridge-preflight.yml")


class X86NativeBridgePreflightTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.source = WORKFLOW.read_text(encoding="utf8")

    def test_one_shot_no_schedule_or_self_repeating_work(self):
        self.assertIn("  workflow_dispatch:", self.source)
        self.assertNotIn("  schedule:", self.source)
        self.assertIn("timeout-minutes: 18", self.source)

    def test_kvm_permission_and_real_api_checked_before_large_image(self):
        self.assertLess(self.source.index("sudo chmod a+rw /dev/kvm"),
                        self.source.index("fcntl.ioctl(fd, 0xAE00, 0)"))
        self.assertLess(self.source.index("fcntl.ioctl(fd, 0xAE00, 0)"),
                        self.source.index("reactivecircus/android-emulator-runner"))

    def test_unchanged_official_google_play_image(self):
        self.assertIn("api-level: 35", self.source)
        self.assertIn("target: google_apis_playstore", self.source)
        self.assertIn("arch: x86_64", self.source)
        self.assertNotIn("arch: arm64-v8a", self.source)

    def test_emulator_runner_script_is_posix_sh_compatible(self):
        # This GitHub Action runs its inline script via /bin/sh -c, not bash.
        self.assertIn("          script: |\n            set -eu\n", self.source)
        self.assertNotIn("          script: |\n            set -euo pipefail", self.source)

    def test_does_not_advertise_translation_proof(self):
        self.assertIn('"arm64_translator_execution_verified":False', self.source)
        self.assertIn('"gameplay_verified":False', self.source)
        self.assertIn('"original_apk_installed":False', self.source)
        self.assertIn('"arm64_advertised":', self.source)

    def test_no_game_download_install_or_security_changes(self):
        for forbidden in ("gh release download", "install-multiple",
                          "GENYMOTION_API_TOKEN", "pm grant", "setprop"):
            self.assertNotIn(forbidden, self.source)


if __name__ == "__main__":
    unittest.main()
