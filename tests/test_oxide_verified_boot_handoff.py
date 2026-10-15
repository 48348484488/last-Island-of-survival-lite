"""Ensure zero-account official ARM64 boot automatically triggers exactly one gated original test."""
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BOOT = ROOT / ".github/workflows/oxide-m3-official-arm64-tcg-boot.yml"
FOLLOW = ROOT / ".github/workflows/oxide-m4-official-arm64-original-on-boot.yml"
SUPERVISOR = ROOT / ".github/workflows/oxide-event-supervisor.yml"


class VerifiedBootHandoffTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.boot = BOOT.read_text(encoding="utf8")
        cls.follow = FOLLOW.read_text(encoding="utf8")
        cls.supervisor = SUPERVISOR.read_text(encoding="utf8")

    def test_no_cron_or_repeated_automatic_boot(self):
        self.assertNotIn("schedule:", self.boot)
        self.assertNotIn("schedule:", self.follow)
        self.assertIn("workflow_run:", self.follow)
        self.assertIn("types: [completed]", self.follow)

    def test_follow_only_on_success_from_our_repository(self):
        self.assertIn("github.event.workflow_run.conclusion == 'success'", self.follow)
        self.assertIn("github.event.workflow_run.head_repository.full_name == github.repository", self.follow)
        self.assertIn("oxide-official-arm64-android15-tcg-boot-evidence", self.follow)

    def test_boot_artifacts_required_before_expensive_download(self):
        self.assertLess(self.follow.index("gh run download"),
                        self.follow.index("reactivecircus/android-emulator-runner@v2"))
        self.assertIn('assert result["proceed_with_original_apk_download"] is True', self.follow)
        self.assertIn('assert result["gameplay_confirmed"] is False', self.follow)

    def test_software_tcg_is_explicit(self):
        for wf in (self.boot, self.follow):
            self.assertIn('ANDROID_I_WANT_MY_TCG: "yes"', wf)
            self.assertIn("arch: arm64-v8a", wf)
            self.assertIn("target: google_apis_playstore", wf)
            self.assertIn("disable-linux-hw-accel: true", wf)

    def test_supervisor_tracks_official_boot_and_game_completion(self):
        self.assertIn("      - Oxide M3 official ARM64 Play Android software boot (no account)", self.supervisor)
        self.assertIn("      - Oxide M4 original APK on verified official ARM64 TCG (one attempt)", self.supervisor)
        self.assertIn("managed-arm64-gate.json", self.supervisor)
        self.assertIn("oxide-exit-timeline.json", self.supervisor)
        self.assertIn("status.txt", self.supervisor)

    def test_supervisor_does_not_duplicate_handoff_or_retry_official_boot(self):
        self.assertIn("no duplicate dispatch here", self.supervisor)
        self.assertIn("Do not download Oxide or schedule endless expensive retries", self.supervisor)
        self.assertIn("Do not count game or ESP as verified", self.supervisor)
        self.assertIn("Do not rerun automatically", self.supervisor)

    def test_only_original_hash_protected_game_trial(self):
        self.assertIn("oxide_official_arm64_game_trial.sh", self.follow)
        self.assertIn("actions: read", self.follow)
        self.assertNotIn("gmsaas", self.follow.lower())
        self.assertNotIn("GENYMOTION_API_TOKEN", self.follow)
        self.assertNotIn("smoke-bypass", self.follow)


if __name__ == "__main__":
    unittest.main()
