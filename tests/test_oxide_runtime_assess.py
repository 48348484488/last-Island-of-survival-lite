import json
import subprocess
import sys
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
        self.put("capture-timing.json", json.dumps({
            "clock": "monotonic",
            "checkpoints": [
                {"target_seconds": t, "elapsed_seconds": float(t),
                 "finished_seconds": float(t) + 0.2,
                 "pid": "100", "activity_dump_captured": True}
                for t in (0, 1, 2, 3, 4, 10, 35)
            ],
        }))

    def run_cli(self):
        # Reproduce GitHub Actions invoking the script directly from another cwd.
        script = Path(__file__).resolve().parents[1] / "tools" / "oxide_runtime_assess.py"
        return subprocess.run([sys.executable, str(script), str(self.d)],
                              cwd=self.d, capture_output=True, text=True, timeout=15)

    def test_cli_direct_script_preserves_gameplay_gate(self):
        self.valid_stable()
        proc = self.run_cli()
        self.assertEqual(proc.returncode, 0, proc.stderr)
        result = json.loads(proc.stdout)
        self.assertEqual(result["status"], "PID_STABLE_35S_MENU_NOT_YET_REVIEWED")
        self.assertFalse(result["gameplay_confirmed"])
        self.assertEqual(json.loads((self.d / "runtime-verdict.json").read_text()), result)

    def test_cli_direct_script_fails_closed_on_realistic_pid_exit(self):
        self.put("boot.txt", "1")
        self.put("abi.txt", "arm64-v8a")
        self.put("install.txt", "Success")
        for name, pid in (("pid-0.txt", "2023"), ("pid-10.txt", "2296"),
                          ("pid-35.txt", "2296")):
            self.put(name, pid)
        proc = self.run_cli()
        self.assertEqual(proc.returncode, 1, proc.stderr)
        result = json.loads(proc.stdout)
        self.assertEqual(result["status"], "FAILED_PID_STABILITY_35S")
        self.assertFalse(result["gameplay_confirmed"])
        self.assertEqual(json.loads((self.d / "runtime-verdict.json").read_text()), result)

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

    def test_stable_pid_without_elapsed_time_must_not_pass(self):
        self.valid_stable()
        (self.d / "capture-timing.json").unlink()
        result = evaluate(self.d)
        self.assertEqual(result["status"], "PID_STABLE_UNVERIFIED_35S_TIMING")
        self.assertFalse(result["gameplay_confirmed"])

    def test_fake_35_second_filename_cannot_approve_stability(self):
        self.valid_stable()
        captured = json.loads((self.d / "capture-timing.json").read_text())
        captured["checkpoints"][-1]["elapsed_seconds"] = 5.0
        captured["checkpoints"][-1]["finished_seconds"] = 5.2
        self.put("capture-timing.json", json.dumps(captured))
        result = evaluate(self.d)
        self.assertEqual(result["status"], "PID_STABLE_UNVERIFIED_35S_TIMING")
        self.assertFalse(result["timing"]["verified"])

    def test_cli_rejects_missing_monotonic_capture(self):
        self.valid_stable()
        (self.d / "capture-timing.json").unlink()
        proc = self.run_cli()
        self.assertEqual(proc.returncode, 1, proc.stderr)
        self.assertEqual(json.loads(proc.stdout)["status"],
                         "PID_STABLE_UNVERIFIED_35S_TIMING")

    def test_launch_and_capture_overlap_with_empty_pid_at_t0(self):
        self.valid_stable()
        self.put("pid-0.txt", "")
        self.put("activity-0.txt", "topResumedActivity=ActivityRecord{x u0 com.android.launcher/.Home t1}")
        self.put("pid-2.txt", "100")
        self.put("activity-2.txt", f"topResumedActivity=ActivityRecord{{x u0 {ORIGINAL} t8}}")
        timing = json.loads((self.d / "capture-timing.json").read_text())
        timing["reference"] = "adb_launch_dispatch"
        timing["checkpoints"][0]["pid"] = ""
        self.put("capture-timing.json", json.dumps(timing))
        result = evaluate(self.d)
        self.assertEqual(result["status"], "PID_STABLE_35S_MENU_NOT_YET_REVIEWED")
        self.assertEqual(result["first_original_resumed_checkpoint"], 2)
        self.assertEqual(result["first_original_resumed_pid"], "100")
        self.assertFalse(result["original_activity_resumed_at_0_10_35"])
        self.assertTrue(result["original_activity_resumed_at_early_10_35"])
        self.assertFalse(result["gameplay_confirmed"])

    def test_later_analytics_pid_must_not_replace_early_original(self):
        self.valid_stable()
        self.put("pid-0.txt", "")
        self.put("pid-2.txt", "100")
        self.put("activity-0.txt", "topResumedActivity=ActivityRecord{x u0 com.android.launcher/.Home t1}")
        self.put("activity-2.txt", f"topResumedActivity=ActivityRecord{{x u0 {ORIGINAL} t8}}")
        self.put("pid-10.txt", "2296")
        self.put("pid-35.txt", "2296")
        result = evaluate(self.d)
        self.assertEqual(result["status"], "FAILED_PID_STABILITY_35S")
        self.assertEqual(result["first_original_resumed_checkpoint"], 2)
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
