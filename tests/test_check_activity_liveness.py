import importlib.util
import json
import tempfile
import unittest
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    "activity_liveness",
    Path(__file__).resolve().parents[1] / "tools" / "check_activity_liveness.py",
)
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)


class ActivityProofTests(unittest.TestCase):
    def run_case(self, initial=100, at10="100", at35="100",
                 resumed10=True, resumed35=True, signal=None, death=None):
        with tempfile.TemporaryDirectory() as tmp:
            p = Path(tmp)
            (p / "forensic_summary.json").write_text(json.dumps({
                "initial_activity_pid": initial, "first_self_sigkill": signal,
                "death_event": death,
            }))
            (p / "pid_10s.txt").write_text(at10)
            (p / "pid_35s.txt").write_text(at35)
            good = "topResumedActivity=ActivityRecord{123 " + mod.PACKAGE + "/com.example.OxideActivity}"
            bad = "topResumedActivity=ActivityRecord{456 com.android.launcher/Launcher}"
            (p / "activities_10s.txt").write_text(good if resumed10 else bad)
            (p / "activities_35s.txt").write_text(good if resumed35 else bad)
            return mod.evaluate(p)

    def test_identical_original_pid_and_resumed_activity(self):
        r = self.run_case()
        self.assertEqual(r["status"], "ACTIVITY_SMOKE_PASS_NOT_GAMEPLAY")
        self.assertFalse(r["gameplay_verified"])
        self.assertFalse(r["esp_verified"])

    def test_analytics_pid_not_game_survival(self):
        r = self.run_case(at10="100 2523", at35="2523")
        self.assertEqual(r["status"], "INITIAL_ACTIVITY_PID_NOT_ALIVE_AT_CHECKPOINTS")

    def test_stale_activity_or_launcher_cannot_pass(self):
        r = self.run_case(resumed35=False)
        self.assertEqual(r["status"], "ORIGINAL_ACTIVITY_NOT_RESUMED_AT_CHECKPOINTS")

    def test_self_signal_overrides_pid_samples(self):
        r = self.run_case(signal="10-13 21:35:15.170")
        self.assertEqual(r["status"], "INITIAL_ACTIVITY_TERMINATED")

    def test_no_initial_pid_fails_closed(self):
        r = self.run_case(initial=None)
        self.assertEqual(r["status"], "INSUFFICIENT_INITIAL_ACTIVITY_PID")

    def test_system_death_event_overrides_samples(self):
        r = self.run_case(death="10-13 21:35:15.420")
        self.assertEqual(r["status"], "INITIAL_ACTIVITY_TERMINATED")


if __name__ == "__main__":
    unittest.main()
