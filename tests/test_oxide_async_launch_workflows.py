"""Regression for immediate original Activity sampling across all ARM64 pipelines.

Static checks verify orchestration and costs; they are not a run of the APK.
"""
from pathlib import Path
import re
import unittest

ROOT = Path(__file__).resolve().parents[1]
REDROID = ROOT / ".github/workflows/oxide-115-arm64-runtime.yml"
CLOUD = ROOT / ".github/workflows/oxide-managed-cloud-arm64-trial.yml"
OFFICIAL = ROOT / "tools/oxide_official_arm64_game_trial.sh"


class AsyncOriginalLaunchPipelineTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.redroid = REDROID.read_text()
        cls.cloud = CLOUD.read_text()
        cls.official = OFFICIAL.read_text()

    def test_all_three_pipelines_dispatch_launch_inside_capture_clock(self):
        for source in (self.redroid, self.cloud, self.official):
            self.assertIn("--launch-activity", source)
            self.assertIn("tools/oxide_capture_samples.py", source)
            self.assertNotRegex(source, r"(?m)^[ \t]*(?:timeout [0-9]+ |python3 tools/oxide_process_timeout.py.*?)?adb .* shell am start -W")

    def test_redroid_900mb_pipeline_is_not_triggered_by_source_edits(self):
        self.assertIn("  workflow_dispatch:", self.redroid)
        self.assertNotIn("\n  push:\n", self.redroid)
        self.assertIn("tools/oxide_runtime_assess.py reports", self.redroid)

    def test_all_pipelines_still_require_original_manifest_component(self):
        self.assertIn('"$APP_PACKAGE/$APP_ACTIVITY"', self.redroid)
        self.assertIn('"$APP_PACKAGE/$APP_ACTIVITY"', self.cloud)
        self.assertIn('"$APP_PACKAGE/$APP_ACTIVITY"', self.official)

    def test_cloud_and_official_game_downloads_remain_gated(self):
        for source in (self.cloud, self.official):
            self.assertLess(source.index("tools/oxide_managed_arm64_gate.py"),
                            source.index("gh release download"))
            self.assertLess(source.index("tools/oxide_original_hash_gate.py"),
                            source.index("install-multiple"))

    def test_capture_preserves_timing_and_original_pid_checks(self):
        sample = (ROOT / "tools/oxide_capture_samples.py").read_text()
        runtime = (ROOT / "tools/oxide_runtime_assess.py").read_text()
        goal = (ROOT / "tools/oxide_goal_engine.py").read_text()
        self.assertIn('"adb_launch_dispatch"', sample)
        self.assertIn("subprocess.Popen(", sample)
        self.assertIn("first_resumed_original", runtime)
        self.assertIn("verify_capture_timing", runtime)
        self.assertIn("first_resumed_original", goal)
        self.assertIn("original_pid = initial", goal)

    def test_early_screenshot_schedule_does_not_preempt_pid_activity(self):
        sample = (ROOT / "tools/oxide_capture_samples.py").read_text()
        self.assertLess(sample.index('if target in CHECKPOINTS:'), sample.index('if target in (2, 3, 4, 10, 35):'))
        self.assertIn('screenshot_timeout = 3 if target <= 4 else 6', sample)


if __name__ == "__main__":
    unittest.main()
