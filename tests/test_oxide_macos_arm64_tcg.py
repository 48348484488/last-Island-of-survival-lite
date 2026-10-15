"""No-account ARM64 TCG boot policy: one attempt, never install Oxide before boot."""
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PREFLIGHT = ROOT / ".github/workflows/oxide-m3-macos-arm64-tcg-preflight.yml"
BOOT = ROOT / ".github/workflows/oxide-m3-macos-arm64-tcg-boot-once.yml"


class MacArm64TcgTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.preflight = PREFLIGHT.read_text(encoding="utf-8")
        cls.boot = BOOT.read_text(encoding="utf-8")

    def test_native_arm64_host_and_explicit_tcg_optin(self):
        self.assertIn("runs-on: macos-15", self.boot)
        self.assertIn('ANDROID_I_WANT_MY_TCG: "yes"', self.boot)
        self.assertIn("-accel off", self.boot)
        self.assertNotIn("-accel on", self.boot)
        self.assertIn('test "$(uname -m)" = arm64', self.boot)

    def test_boot_is_one_bounded_attempt_not_loop(self):
        self.assertIn("timeout-minutes: 24", self.boot)
        self.assertIn("emulator-boot-timeout: 950", self.boot)
        self.assertNotIn("schedule:", self.boot)
        self.assertNotIn("workflow_run:", self.boot)
        self.assertIn("      - '.github/workflows/oxide-m3-macos-arm64-tcg-boot-once.yml'", self.boot)

    def test_exact_official_arm64_play_image(self):
        for text in ("api-level: 35", "target: google_apis_playstore",
                     "arch: arm64-v8a"):
            self.assertIn(text, self.boot)
        self.assertIn("android-version.txt", self.boot)
        self.assertIn("play-store-pm-path.txt", self.boot)
        self.assertIn("play-services-pm-path.txt", self.boot)

    def test_no_original_apk_or_integrity_changes(self):
        for bad in ("gh release download", "install-multiple",
                    "REQUEST_INSTALL_PACKAGES", "setprop", "pm grant", "magisk",
                    "gmsaas", "GENYMOTION_API_TOKEN"):
            self.assertNotIn(bad, self.boot)
        self.assertIn("tools/oxide_managed_arm64_gate.py", self.boot)
        self.assertIn("original_apk", (ROOT / "tools/oxide_managed_arm64_gate.py").read_text())

    def test_failure_artifact_always_preserved(self):
        self.assertIn("if: always()", self.boot)
        self.assertIn("oxide-macos-arm64-tcg-boot-evidence-no-apk", self.boot)

    def test_supervisor_reports_android_only_without_claiming_gameplay(self):
        supervisor = (ROOT / ".github/workflows/oxide-event-supervisor.yml").read_text()
        self.assertIn("Oxide M3 macOS ARM64 official Android15 TCG boot only - one attempt", supervisor)
        self.assertIn("OFFICIAL_MAC_ARM64_TCG_BOOT_SUCCEEDED_ORIGINAL_APK_NOT_INSTALLED", supervisor)
        self.assertIn("attempt-stage.txt", supervisor)
        self.assertIn("boot.txt: 1", supervisor)
        self.assertIn("original Oxide APK and gameplay remain UNTESTED", supervisor)
        self.assertIn("No original APK downloaded", supervisor)

    def test_original_game_handoff_requires_exact_completed_successful_boot(self):
        follow = (ROOT / ".github/workflows/oxide-m4-macos-arm64-tcg-original-on-boot.yml").read_text()
        self.assertIn('workflow_run:', follow)
        self.assertIn("github.event.workflow_run.conclusion == 'success'", follow)
        self.assertIn("github.event.workflow_run.id == 38813694423", follow)
        self.assertIn("github.event.workflow_run.head_repository.full_name == github.repository", follow)
        self.assertIn("gh run download", follow)
        self.assertIn("oxide-macos-arm64-tcg-boot-evidence-no-apk", follow)
        self.assertIn('assert (p / "boot.txt").read_text().strip() == "1"', follow)
        self.assertIn('assert (p / "abi.txt").read_text().strip() == "arm64-v8a"', follow)
        self.assertIn('assert gate["proceed_with_original_apk_download"] is True', follow)
        self.assertIn('assert gate["gameplay_confirmed"] is False', follow)
        self.assertLess(follow.index("gh run download"), follow.index("reactivecircus/android-emulator-runner@v2"))
        self.assertIn("script: bash tools/oxide_official_arm64_game_trial.sh emulator-5554 reports", follow)
        self.assertIn("if: always()", follow)
        self.assertIn("GH_TOKEN: ${{ github.token }}", follow)
        self.assertNotIn("gmsaas", follow)
        self.assertNotIn("schedule:", follow)

    def test_preflight_does_not_claim_android_boot(self):
        self.assertIn('android_booted": False', self.preflight)
        self.assertIn('gameplay_verified": False', self.preflight)
        self.assertIn("NATIVE_MAC_ARM64_TCG_CLI_POSSIBLE_BOOT_UNTESTED", self.preflight)


if __name__ == "__main__":
    unittest.main()
