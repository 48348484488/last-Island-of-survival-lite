import json
import tempfile
import unittest
from pathlib import Path
from tools.oxide_runtime_assess import evaluate
from tools.oxide_goal_engine import ORIGINAL


class TestRuntimeAssessment(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)
        self.d = Path(self.tmp.name)

    def put(self, name, data):
        (self.d / name).write_text(data)

    def valid_stable(self):
        for name, value in [("boot.txt", "1"), ("abi.txt", "arm64-v8a"), ("install.txt", "Success")]:
            self.put(name, value)
        self.put("launch.txt", f"Starting: Intent {{ cmp={ORIGINAL} }}\nStatus: ok\nActivity: {ORIGINAL}\n")
        for t in (0, 10, 35):
            self.put(f"pid-{t}.txt", "100")
            self.put(f"activity-{t}.txt", f"  topResumedActivity=ActivityRecord{{value u0 {ORIGINAL} t8}}\n")
        for t in (10, 35):
            (self.d / f"screen-{t}.png").write_bytes(b"\x89PNG\r\n\x1a\nabc")

    def test_binder_blocker(self):
        self.put("preflight.json", json.dumps({"next_action": "HOST_NO_BINDER"}))
        self.assertEqual(evaluate(self.d)["status"], "BLOCKED_HOST_BINDER")

    def test_android_boot_is_not_assumed(self):
        self.assertEqual(evaluate(self.d)["status"], "BLOCKED_ANDROID_BOOT")

    def test_install_must_be_confirmed(self):
        self.put("boot.txt", "1")
        self.put("abi.txt", "arm64-v8a")
        self.assertEqual(evaluate(self.d)["status"], "BLOCKED_ORIGINAL_APK_INSTALL")

    def test_pid_exit_detected(self):
        self.put("boot.txt", "1")
        self.put("abi.txt", "arm64-v8a")
        self.put("install.txt", "Success")
        self.put("pid-0.txt", "100")
        self.put("pid-10.txt", "100")
        self.assertEqual(evaluate(self.d)["status"], "FAILED_PID_STABILITY_35S")

    def test_stable_pid_requires_launch_proof(self):
        self.valid_stable()
        self.put("launch.txt", "Status: ok\nActivity: com.example.other/.Activity")
        self.assertEqual(evaluate(self.d)["status"], "PID_STABLE_MISSING_ORIGINAL_LAUNCH")

    def test_stable_pid_alone_not_gameplay(self):
        self.valid_stable()
        (self.d / "screen-35.png").unlink()
        self.assertEqual(evaluate(self.d)["status"], "PID_STABLE_MISSING_VISUAL_EVIDENCE")
        (self.d / "screen-35.png").write_bytes(b"\x89PNG\r\n\x1a\nabc")
        result = evaluate(self.d)
        self.assertEqual(result["status"], "PID_STABLE_35S_MENU_NOT_YET_REVIEWED")
        self.assertTrue(result["original_activity_resumed_at_0_10_35"])
        self.assertFalse(result["gameplay_confirmed"])

    def test_background_task_history_is_not_resumed_activity(self):
        self.valid_stable()
        self.put("activity-35.txt", f"Hist #0: ActivityRecord{{test u0 {ORIGINAL} t8}}\n")
        self.assertEqual(evaluate(self.d)["status"], "PID_STABLE_WITHOUT_ORIGINAL_RESUMED_ACTIVITY")

    def test_other_resumed_activity_does_not_pass(self):
        self.valid_stable()
        self.put("activity-10.txt", "topResumedActivity=ActivityRecord{other u0 com.android.launcher/.Home t1}")
        self.assertEqual(evaluate(self.d)["status"], "PID_STABLE_WITHOUT_ORIGINAL_RESUMED_ACTIVITY")

    def test_multiple_pidof_pids_rejected(self):
        self.valid_stable()
        for t in (0, 10, 35):
            self.put(f"pid-{t}.txt", "100 101")
        self.assertEqual(evaluate(self.d)["status"], "FAILED_PID_STABILITY_35S")


if __name__ == "__main__":
    unittest.main()
