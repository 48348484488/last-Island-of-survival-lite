"""Replay saved original Activity/PID evidence without running an APK."""
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from tools.oxide_activity_binding_audit import audit
from tools.oxide_activity_binding import PACKAGE, ORIGINAL


def owned_activity(pid):
    return (
        f"  topResumedActivity=ActivityRecord{{a1 u0 {ORIGINAL} t8}}\n"
        f"  * Hist  #0: ActivityRecord{{a1 u0 {ORIGINAL} t8}}\n"
        f"    app=ProcessRecord{{b1 {pid}:{PACKAGE}/u0a87}}\n"
    )


class TestHistoricalActivityAudit(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)
        self.root = Path(self.tmp.name)
        for t in (0, 10, 35):
            (self.root / f"pid-{t}.txt").write_text("2023")
            (self.root / f"activity-{t}.txt").write_text(owned_activity(2023))
        self.record_valid_timing()

    def record_valid_timing(self):
        self.root.joinpath("capture-timing.json").write_text(json.dumps({
            "clock": "monotonic",
            "checkpoints": [
                {"target_seconds": t, "elapsed_seconds": float(t),
                 "finished_seconds": t + 0.1,
                 "pid": "2023", "activity_dump_captured": True}
                for t in (0, 1, 2, 3, 4, 10, 35)
            ]
        }))

    def test_stable_original_activity_is_not_gameplay(self):
        report = audit(self.root)
        self.assertEqual(report["status"], "ORIGINAL_ACTIVITY_STABLE_35S_GAMEPLAY_UNREVIEWED")
        self.assertTrue(report["same_original_activity_alive_35s"])
        self.assertFalse(report["gameplay_verified"])

    def test_android15_analytics_survivor_is_not_game(self):
        for t in (10, 35):
            (self.root / f"pid-{t}.txt").write_text("2296")
            (self.root / f"activity-{t}.txt").write_text(
                "topResumedActivity=ActivityRecord{a2 u0 com.android.launcher3/.Home t7}\n"
                f"* Hist  #1: ActivityRecord{{a1 u0 {ORIGINAL} t8}}\n")
        report = audit(self.root)
        self.assertEqual(report["first_original_activity_pid"], "2023")
        self.assertFalse(report["same_original_activity_alive_35s"])
        self.assertIsNone(report["checkpoints"][-1]["resumed_original_activity_owner_pid"])

    def test_mismatched_process_owner_is_not_game(self):
        (self.root / "activity-35.txt").write_text(owned_activity(2296))
        report = audit(self.root)
        self.assertFalse(report["same_original_activity_alive_35s"])
        self.assertEqual(report["checkpoints"][-1]["resumed_original_activity_owner_pid"], "2296")

    def test_missing_timing_cannot_pass(self):
        (self.root / "capture-timing.json").unlink()
        report = audit(self.root)
        self.assertFalse(report["same_original_activity_alive_35s"])
        self.assertFalse(report["timing"]["verified"])

    def test_direct_cli_writes_machine_readable_json(self):
        script = Path(__file__).resolve().parents[1] / "tools" / "oxide_activity_binding_audit.py"
        output = self.root / "result.json"
        r = subprocess.run([sys.executable, str(script), str(self.root),
                            "--output", str(output)],
                           cwd=self.root, text=True, capture_output=True, timeout=10)
        self.assertEqual(r.returncode, 0, r.stderr)
        self.assertEqual(json.loads(r.stdout), json.loads(output.read_text()))


if __name__ == "__main__":
    unittest.main()
