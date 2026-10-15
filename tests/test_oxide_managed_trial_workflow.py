"""Static cost/security guards for the manual-only existing SaaS virtual instance trial."""
import re
import unittest
from pathlib import Path

WORKFLOW = (Path(__file__).resolve().parents[1] /
            ".github/workflows/oxide-managed-cloud-arm64-trial.yml")


class CloudTrialWorkflowGuardTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.yaml = WORKFLOW.read_text(encoding="utf8")

    def test_no_automatic_dispatch_or_schedule(self):
        self.assertIn("on:\n  workflow_dispatch:", self.yaml)
        self.assertFalse(re.search(r"^  (?:push|schedule|workflow_run):", self.yaml, re.MULTILINE))

    def test_no_automatic_device_start_or_trial_subscription(self):
        self.assertNotIn("gmsaas instances start", self.yaml)
        self.assertNotIn("gmsaas instances create", self.yaml)
        self.assertNotIn("subscribe", self.yaml.lower())

    def test_secret_is_never_input_or_literal(self):
        self.assertIn("GENYMOTION_API_TOKEN: ${{ secrets.GENYMOTION_API_TOKEN }}", self.yaml)
        self.assertNotIn("gmsaas auth token ", self.yaml)

    def test_preflight_runs_before_large_asset_download(self):
        self.assertLess(self.yaml.index("tools/oxide_managed_arm64_gate.py"),
                        self.yaml.index("gh release download"))

    def test_all_four_hashes_are_verified_before_install(self):
        self.assertLess(self.yaml.index("tools/oxide_original_hash_gate.py"),
                        self.yaml.index('adb -s "$CLOUD_ADB_SERIAL" install-multiple'))

    def test_only_original_apks_are_deployed(self):
        for expected in ("base.apk", "split_config.arm64_v8a.apk",
                         "split_config.xxhdpi.apk", "split_UnityDataAssetPack.apk"):
            self.assertIn(expected, self.yaml)
        self.assertNotIn("--resign", self.yaml)
        self.assertNotIn("pm grant", self.yaml)

    def test_evidence_always_retained_and_optin_stop(self):
        self.assertIn("name: Save diagnostic outputs even if game exits or preflight fails", self.yaml)
        self.assertIn("if: always()", self.yaml)
        self.assertIn("if: ${{ always() && inputs.stop_instance_after_test }}", self.yaml)
        self.assertIn('gmsaas instances stop "$INSTANCE_UUID"', self.yaml)

    def test_no_gameplay_proof_assumed_on_passing_host(self):
        self.assertIn("tools/oxide_runtime_assess.py reports", self.yaml)
        self.assertIn("tools/oxide_exit_timeline.py", self.yaml)


if __name__ == "__main__":
    unittest.main()
