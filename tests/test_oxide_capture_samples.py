import json
import tempfile
import unittest
from pathlib import Path
from types import SimpleNamespace
from tools.oxide_capture_samples import capture, CHECKPOINTS, PNG_SIGNATURE

class TestCapture(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.reports = Path(self.temp.name)
        self.elapsed = 0.0
        self.calls = []

    def clock(self):
        return self.elapsed

    def sleep(self, seconds):
        self.elapsed += seconds

    def runner(self, command, timeout):
        self.calls.append(command)
        self.elapsed += 0.08
        if 'pidof' in command:
            return SimpleNamespace(stdout=b'1234\n', returncode=0)
        if 'screencap' in command:
            return SimpleNamespace(stdout=PNG_SIGNATURE + b'payload', returncode=0)
        if 'dumpsys' in command:
            return SimpleNamespace(stdout=b'topResumedActivity=OxideActivity', returncode=0)
        raise AssertionError(command)

    def test_all_samples_and_nominal_timing(self):
        result = capture(self.reports, 'com.example.game', runner=self.runner,
                         clock=self.clock, sleeper=self.sleep)
        self.assertEqual([r['target_seconds'] for r in result], list(CHECKPOINTS))
        self.assertEqual([r['elapsed_seconds'] for r in result], list(CHECKPOINTS))
        self.assertTrue(all(r['valid_png'] for r in result))
        self.assertEqual(sum('dumpsys' in x for x in self.calls), len(CHECKPOINTS))
        self.assertEqual((self.reports / 'pid-35.txt').read_text(), '1234\n')
        self.assertEqual(len(json.loads((self.reports / 'capture-timing.json').read_text())['checkpoints']), 7)

    def test_slow_adb_records_actual_time_not_fake_checkpoint(self):
        def slow_runner(command, timeout):
            self.elapsed += 2.5
            return self.runner(command, timeout)
        result = capture(self.reports, 'com.example.game', checkpoints=(0,1),
                         runner=slow_runner, clock=self.clock, sleeper=self.sleep)
        self.assertGreater(result[1]['elapsed_seconds'], 1)
        self.assertEqual(result[1]['target_seconds'], 1)

    def test_async_launcher_runs_before_first_capture_without_waiting(self):
        marks = []
        def launcher():
            marks.append(("launch", round(self.clock(), 3)))
            # Simulate only the cost of dispatch; do not wait for the Activity.
            self.elapsed += 0.05

        result = capture(self.reports, "com.example.game",
                         checkpoints=(0, 1, 2, 3, 4, 10, 35),
                         runner=self.runner, clock=self.clock, sleeper=self.sleep,
                         launcher=launcher)
        self.assertEqual(marks, [("launch", 0.0)])
        self.assertGreaterEqual(result[0]["elapsed_seconds"], 0.05)
        self.assertLess(result[0]["elapsed_seconds"], 1.0)
        meta = json.loads((self.reports / "capture-timing.json").read_text())
        self.assertEqual(meta["reference"], "adb_launch_dispatch")
        self.assertEqual(len([x for x in self.calls if "dumpsys" in x]), 7)

    def test_capture_allows_initial_pid_missing_until_real_activity_starts(self):
        calls = {"pid": 0}
        def runner(command, timeout):
            if "pidof" in command:
                calls["pid"] += 1
                self.elapsed += 0.08
                if calls["pid"] == 1:
                    return SimpleNamespace(stdout=b"", returncode=1)
            return self.runner(command, timeout)

        result = capture(self.reports, "com.example.game",
                         checkpoints=(0, 1, 2, 3, 4, 10, 35), runner=runner,
                         clock=self.clock, sleeper=self.sleep, launcher=lambda: None)
        self.assertEqual(result[0]["pid"], "")
        self.assertEqual(result[1]["pid"], "1234")
        self.assertEqual((self.reports / "pid-0.txt").read_text(), "\n")
        self.assertEqual((self.reports / "pid-35.txt").read_text(), "1234\n")

    def test_timeout_does_not_abort_following_samples(self):
        def timeout_screenshot(command, timeout):
            if 'screencap' in command:
                return None
            return self.runner(command, timeout)
        result = capture(self.reports, 'com.example.game', checkpoints=(0,10,35),
                         runner=timeout_screenshot, clock=self.clock, sleeper=self.sleep)
        self.assertEqual(len(result), 3)
        self.assertEqual(result[2]['pid'], '1234')
        self.assertFalse(result[0]['valid_png'])
        self.assertIn('screencap_timeout', result[0]['errors'])
        self.assertFalse((self.reports / 'screen-0.png').exists())
if __name__ == '__main__':
    unittest.main()
