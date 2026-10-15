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
        self.assertEqual(sum('dumpsys' in x for x in self.calls), 3)
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
