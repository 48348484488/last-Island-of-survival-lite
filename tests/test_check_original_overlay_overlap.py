import sys
import json
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from check_original_overlay_overlap import check, PACKAGE


class SameCaptureOriginalOverlayProof(unittest.TestCase):
    def fixture(self, box_index=1, resumed_at=1, pid_at=1, original_pid=2099,
                service=True, cleared=True, graphics_pass=True):
        tmp = tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        root = Path(tmp.name)
        (root / "forensic_summary.json").write_text(json.dumps({
            "initial_activity_pid": original_pid,
        }))
        (root / "loading_overlay_verdict.json").write_text(json.dumps({
            "status": "ORIGINAL_LOADING_OVERLAY_PASS" if graphics_pass else "ORIGINAL_LOADING_OVERLAY_NOT_CONFIRMED",
            "service_start": "DIAGNOSTIC_OVERLAY_SERVICE_START=PASS" if service else "DIAGNOSTIC_OVERLAY_SERVICE_START=FAIL",
            "overlay_disappeared_after_stop": cleared,
            "game_screens_with_test_marker": [f"overlay_on_game_{box_index}.png"],
            "captures": {f"overlay_on_game_{box_index}.png": {"pixels": 2000}},
        }))
        for i in (1, 2, 3):
            activity = (f"topResumedActivity=ActivityRecord{{x {PACKAGE}/X.OxideActivity}}"
                        if i == resumed_at else
                        "topResumedActivity=ActivityRecord{x com.android.launcher/Launcher}")
            (root / f"activities_capture_{i}.txt").write_text(activity)
            (root / f"game_pid_capture_{i}.txt").write_text(
                str(original_pid) if i == pid_at else "2500"
            )
        return root

    def test_matching_original_pid_activity_and_marker(self):
        x = check(self.fixture())
        self.assertEqual(x["status"], "ORIGINAL_LOADING_ACTIVITY_AND_STANDALONE_OVERLAY_CORROBORATED")
        self.assertTrue(x["capture_samples"][0]["sample_correlates"])
        self.assertFalse(x["real_multiplayer_gameplay_verified"])
        self.assertFalse(x["real_player_esp_verified"])

    def test_overlay_only_on_launcher_is_not_game_overlap(self):
        x = check(self.fixture(box_index=3, resumed_at=1, pid_at=1))
        self.assertEqual(x["status"], "INSUFFICIENT_SAME_CAPTURE_ORIGINAL_ACTIVITY_EVIDENCE")

    def test_game_pid_and_activity_different_capture_rejected(self):
        x = check(self.fixture(box_index=1, resumed_at=1, pid_at=2))
        self.assertEqual(x["status"], "INSUFFICIENT_SAME_CAPTURE_ORIGINAL_ACTIVITY_EVIDENCE")

    def test_analytics_pid_not_original(self):
        x = check(self.fixture(original_pid=2099, pid_at=3, resumed_at=1))
        self.assertEqual(x["status"], "INSUFFICIENT_SAME_CAPTURE_ORIGINAL_ACTIVITY_EVIDENCE")

    def test_service_start_required(self):
        x = check(self.fixture(service=False))
        self.assertEqual(x["status"], "INSUFFICIENT_SAME_CAPTURE_ORIGINAL_ACTIVITY_EVIDENCE")

    def test_overlay_must_disappear_on_stop(self):
        x = check(self.fixture(cleared=False))
        self.assertEqual(x["status"], "INSUFFICIENT_SAME_CAPTURE_ORIGINAL_ACTIVITY_EVIDENCE")

    def test_visual_verdict_required(self):
        x = check(self.fixture(graphics_pass=False))
        self.assertEqual(x["status"], "INSUFFICIENT_SAME_CAPTURE_ORIGINAL_ACTIVITY_EVIDENCE")

    def test_boolean_initial_pid_rejected(self):
        with self.assertRaisesRegex(ValueError, "PID"):
            check(self.fixture(original_pid=True))

    def test_missing_original_pid_rejected(self):
        with self.assertRaisesRegex(ValueError, "PID"):
            check(self.fixture(original_pid=None))

    def test_third_game_capture_supported(self):
        x = check(self.fixture(box_index=3, resumed_at=3, pid_at=3))
        self.assertEqual(x["status"], "ORIGINAL_LOADING_ACTIVITY_AND_STANDALONE_OVERLAY_CORROBORATED")


if __name__ == "__main__":
    unittest.main()
