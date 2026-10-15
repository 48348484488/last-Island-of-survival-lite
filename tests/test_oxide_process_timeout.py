"""Portable process timeout regression: macOS and Linux shell trial compatibility."""
import subprocess
import sys
from pathlib import Path
import unittest

CMD = Path(__file__).resolve().parents[1] / "tools" / "oxide_process_timeout.py"


class TestPortableTimeout(unittest.TestCase):
    def test_success_exit_code(self):
        p = subprocess.run([sys.executable, str(CMD), "--seconds", "5", "--",
                            sys.executable, "-c", "print('ok')"],
                           capture_output=True, text=True, timeout=8)
        self.assertEqual(p.returncode, 0, p.stderr)
        self.assertEqual(p.stdout.strip(), "ok")

    def test_child_failure_retained(self):
        p = subprocess.run([sys.executable, str(CMD), "--seconds", "5", "--",
                            sys.executable, "-c", "import sys;sys.exit(37)"],
                           capture_output=True, text=True, timeout=8)
        self.assertEqual(p.returncode, 37)

    def test_timeout_fails_closed(self):
        p = subprocess.run([sys.executable, str(CMD), "--seconds", "0.08", "--",
                            sys.executable, "-c", "import time;time.sleep(10)"],
                           capture_output=True, text=True, timeout=8)
        self.assertEqual(p.returncode, 124)
        self.assertIn("Process exceeded", p.stderr)

    def test_invalid_timeout(self):
        p = subprocess.run([sys.executable, str(CMD), "--seconds", "0", "--",
                            sys.executable, "-V"],
                           capture_output=True, text=True, timeout=8)
        self.assertNotEqual(p.returncode, 0)


if __name__ == "__main__":
    unittest.main()
