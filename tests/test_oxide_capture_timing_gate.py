import copy
import json
import tempfile
import unittest
from pathlib import Path

from tools.oxide_capture_timing_gate import verify_capture_timing


def timing():
    return {"clock": "monotonic", "checkpoints": [
        {"target_seconds": t, "elapsed_seconds": float(t),
         "finished_seconds": float(t) + 0.2, "pid": "1234",
         "activity_dump_captured": True}
        for t in (0, 1, 2, 3, 4, 10, 35)
    ]}


class TimingGateTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.path = Path(self.temp.name)
        self.good = timing()
        for t in (0, 10, 35):
            (self.path / f"pid-{t}.txt").write_text("1234\n")

    def check(self, report, expected):
        (self.path / "capture-timing.json").write_text(json.dumps(report))
        result = verify_capture_timing(self.path)
        self.assertEqual(result["reason"], expected)
        self.assertFalse(result["verified"])

    def test_realistic_monotonic_35s_is_accepted(self):
        (self.path / "capture-timing.json").write_text(json.dumps(self.good))
        result = verify_capture_timing(self.path)
        self.assertTrue(result["verified"])
        self.assertEqual(result["elapsed_35_seconds"], 35.0)

    def test_no_timing_file_fails_closed(self):
        self.assertEqual(verify_capture_timing(self.path)["reason"],
                         "MISSING_OR_INVALID_CAPTURE_TIMING")

    def test_json_truncated_fails(self):
        (self.path / "capture-timing.json").write_text('{"checkpoints":')
        self.assertFalse(verify_capture_timing(self.path)["verified"])

    def test_wrong_clock_fails(self):
        data = copy.deepcopy(self.good)
        data["clock"] = "wall"
        self.check(data, "NON_MONOTONIC_CAPTURE_CLOCK")

    def test_missing_35s_checkpoint_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"].pop()
        self.check(data, "MISSING_REQUIRED_CAPTURE_CHECKPOINT")

    def test_fake_35s_label_without_elapsed_does_not_pass(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"][-1]["elapsed_seconds"] = 6.0
        data["checkpoints"][-1]["finished_seconds"] = 6.2
        self.check(data, "INVALID_MONOTONIC_SAMPLE_TIMES")

    def test_duplicate_record_target_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"].append(copy.deepcopy(data["checkpoints"][-1]))
        self.check(data, "DUPLICATED_OR_UNORDERED_TARGET")

    def test_out_of_order_record_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"][-1], data["checkpoints"][-2] = data["checkpoints"][-2], data["checkpoints"][-1]
        self.check(data, "DUPLICATED_OR_UNORDERED_TARGET")

    def test_nonfinite_elapsed_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"][-1]["elapsed_seconds"] = float("nan")
        self.check(data, "INVALID_MONOTONIC_SAMPLE_TIMES")

    def test_boolean_time_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"][-1]["elapsed_seconds"] = True
        self.check(data, "INVALID_MONOTONIC_SAMPLE_TIMES")

    def test_pid_mismatch_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"][-1]["pid"] = "2296"
        self.check(data, "CAPTURE_PID_MISMATCH")

    def test_pid_file_missing_fails(self):
        (self.path / "pid-35.txt").unlink()
        self.check(self.good, "MISSING_CHECKPOINT_PID")

    def test_missing_activity_capture_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"][-1]["activity_dump_captured"] = False
        self.check(data, "ACTIVITY_DUMP_CAPTURE_NOT_CONFIRMED")

    def test_inverted_finish_fails(self):
        data = copy.deepcopy(self.good)
        data["checkpoints"][-1]["finished_seconds"] = 34.0
        self.check(data, "INVALID_MONOTONIC_SAMPLE_TIMES")

    def test_elapsed_35_cannot_claim_gameplay_by_itself(self):
        (self.path / "capture-timing.json").write_text(json.dumps(self.good))
        result = verify_capture_timing(self.path)
        self.assertNotIn("gameplay_confirmed", result)


if __name__ == "__main__":
    unittest.main()
