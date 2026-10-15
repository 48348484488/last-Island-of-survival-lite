"""One-off KVM recovery can never repeat an Oxide APK attempt after installation."""
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
WORKFLOW = ROOT / ".github/workflows/oxide-m4-x86-nativebridge-kvm-recovery-once.yml"


class GuardedOriginalRecoveryTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.wf = WORKFLOW.read_text(encoding="utf-8")

    def test_only_exact_predecessor_failed_or_cancelled(self):
        self.assertIn("github.event.workflow_run.id == 38817690114", self.wf)
        self.assertIn("github.event.workflow_run.conclusion == 'failure'", self.wf)
        self.assertIn("github.event.workflow_run.conclusion == 'cancelled'", self.wf)
        self.assertIn("github.event.workflow_run.head_repository.full_name == github.repository", self.wf)
        self.assertNotIn("  schedule:", self.wf)
        self.assertNotIn("  push:", self.wf)
        self.assertIn("types: [completed]", self.wf)

    def test_requires_proof_original_never_downloaded_before_retry(self):
        self.assertIn("gh run download 38817690114", self.wf)
        self.assertIn('assert stage == "JNI_PROOF_VERIFIED_ORIGINAL_APK_NOT_DOWNLOADED"', self.wf)
        for item in ("original-apk-inventory.json", "original-sha256-verdict.json",
                     "apk-badging.txt", "install.txt", "capture-timing.json"):
            self.assertIn(item, self.wf)
        self.assertIn('assert proof["arm64_jni_executed"] is True', self.wf)
        self.assertIn('assert proof["original_oxide_gameplay_verified"] is False', self.wf)

    def test_kvm_real_api_required_before_android_download(self):
        self.assertIn("sudo chmod a+rw /dev/kvm", self.wf)
        self.assertIn("fcntl.ioctl(fd,0xAE00,0)", self.wf)
        self.assertIn('assert api==12', self.wf)
        self.assertLess(self.wf.index("fcntl.ioctl(fd,0xAE00,0)"),
                        self.wf.index("reactivecircus/android-emulator-runner@v2"))

    def test_only_single_game_trial_and_fail_closed(self):
        self.assertEqual(self.wf.count("reactivecircus/android-emulator-runner@v2"), 1)
        self.assertIn("script: bash tools/oxide_nativebridge_probe/original_on_verified_bridge.sh reports", self.wf)
        self.assertIn('retry_count": 1', self.wf)
        self.assertIn("if: always()", self.wf)
        self.assertIn("gameplay_verified", self.wf)
        self.assertNotIn("setprop", self.wf)
        self.assertNotIn("pm grant", self.wf)


if __name__ == "__main__":
    unittest.main()
