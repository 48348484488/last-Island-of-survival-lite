import json
import tempfile
import unittest
from pathlib import Path
from tools.oxide_goal_engine import evidence_summary, resumed

PKG = "com.catsbit.oxidesurvivalisland/.OxideActivity"


def original_activity(pid="2018", token="a1"):
    return (f"topResumedActivity=ActivityRecord{{{token} u0 {PKG} t8}}\n"
            f"* Hist  #0: ActivityRecord{{{token} u0 {PKG} t8}}\n"
            f"  packageName=com.catsbit.oxidesurvivalisland processName=com.catsbit.oxidesurvivalisland\n"
            f"  app=ProcessRecord{{b1 {pid}:com.catsbit.oxidesurvivalisland/u0a87}}\n")


class TestGoals(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)
        self.d = Path(self.tmp.name)

    def write(self, name, value):
        (self.d/name).write_text(value)

    def boot_and_launch(self):
        for name, value in [
            ("boot.txt","1"), ("abi.txt","arm64-v8a"), ("install.txt","Success"),
            ("launch.txt", "Starting: Intent { cmp="+PKG+" }\nStatus: ok\n"),
            ("pid-0.txt","2018"),
            ("activity-0.txt", original_activity(2018))
        ]:
            self.write(name,value)

    def test_no_unverified_boot(self):
        r=evidence_summary(self.d)
        self.assertEqual(r['goals'][0]['status'],'blocked')
        self.assertEqual(r['next_action'],'HOST_PREPARE')

    def test_launch_passed_without_35s(self):
        self.boot_and_launch()
        self.write('pid-10.txt','2268')
        self.write('pid-35.txt','2268')
        self.write('logcat.txt',
            '10-15 18:23:42.179 287 735 I ActivityTaskManager: START u0 { cmp='+PKG+' }\n'
            '10-15 18:23:46.763 2018 2018 I Process : Sending signal. PID: 2018 SIG: 9\n')
        r=evidence_summary(self.d)
        self.assertEqual([g['status'] for g in r['goals']], ['complete','complete','blocked','pending','pending'])
        self.assertEqual(r['next_action'],'FORENSIC_APP_EXIT')
        self.assertTrue(r['checks']['initial_activity_self_sigkill_observed'])
        self.assertEqual(r['checks']['start_to_sigkill_seconds'],4.584)

    def test_analytics_pid_not_false_positive(self):
        self.boot_and_launch()
        for t in (10,35):
            self.write(f'pid-{t}.txt','2018')
            self.write(f'activity-{t}.txt',"Task{... "+PKG+" ...} OtherApp Resumed")
        self.assertFalse(evidence_summary(self.d)['checks']['original_activity_resumed_at_10_and_35'])

    def test_name_35_without_clock_cannot_complete_goal(self):
        self.boot_and_launch()
        for t in (10, 35):
            self.write(f'pid-{t}.txt', '2018')
            self.write(f'activity-{t}.txt', original_activity(2018))
            (self.d/f'screen-{t}.png').write_bytes(b'\x89PNG\r\n\x1a\n'+b'a'*200)
        report=evidence_summary(self.d)
        self.assertEqual(report['goals'][2]['status'], 'blocked')
        self.assertFalse(report['checks']['capture_timing']['verified'])
        self.assertFalse(report['gameplay_confirmed'])

    def test_async_launch_initially_has_no_pid_but_original_resumes_at_second_two(self):
        self.boot_and_launch()
        self.write("pid-0.txt", "")
        self.write("activity-0.txt", "topResumedActivity=ActivityRecord{other u0 com.android.launcher/.Home t1}")
        self.write("pid-2.txt", "2018")
        self.write("activity-2.txt", original_activity(2018, token="ab"))
        for t in (10, 35):
            self.write(f"pid-{t}.txt", "2018")
            self.write(f"activity-{t}.txt", original_activity(2018, token="ab"))
            (self.d / f"screen-{t}.png").write_bytes(b"\x89PNG\r\n\x1a\n" + b"a"*200)
        self.write("capture-timing.json", json.dumps({
            "clock": "monotonic",
            "reference": "adb_launch_dispatch",
            "checkpoints": [
                {"target_seconds": t, "elapsed_seconds": float(t),
                 "finished_seconds": float(t)+0.2,
                 "pid": "" if t == 0 else "2018",
                 "activity_dump_captured": True}
                for t in (0, 1, 2, 3, 4, 10, 35)
            ],
        }))
        report = evidence_summary(self.d)
        self.assertEqual(report["checks"]["first_original_resumed_checkpoint"], 2)
        self.assertEqual(report["checks"]["first_original_resumed_pid"], "2018")
        self.assertEqual(report["goals"][2]["status"], "complete")
        self.assertFalse(report["gameplay_confirmed"])

    def test_analytics_service_pid_does_not_replace_original_early_resumed_pid(self):
        self.boot_and_launch()
        self.write("pid-0.txt", "")
        self.write("pid-2.txt", "2018")
        self.write("activity-0.txt", "")
        self.write("activity-2.txt", original_activity(2018, token="ab"))
        for t in (10, 35):
            self.write(f"pid-{t}.txt", "2296")
            self.write(f"activity-{t}.txt", original_activity(2018, token="ab"))
        report = evidence_summary(self.d)
        self.assertEqual(report["checks"]["first_original_resumed_pid"], "2018")
        self.assertFalse(report["checks"]["original_activity_resumed_at_10_and_35"])
        self.assertEqual(report["goals"][2]["status"], "blocked")

    def test_sigkill_correlates_first_original_pid_when_t0_empty(self):
        self.boot_and_launch()
        self.write("pid-0.txt", "")
        self.write("activity-0.txt", "topResumedActivity=ActivityRecord{other u0 com.android.launcher/.Home t1}")
        self.write("pid-2.txt", "2023")
        self.write("activity-2.txt", original_activity(2023, token="ab"))
        self.write("pid-10.txt", "2296")
        self.write("pid-35.txt", "2296")
        self.write("logcat.txt",
            "10-15 18:55:51.207  287 735 I ActivityTaskManager: START u0 { cmp="+PKG+" }\n"
            "10-15 18:55:55.687 2023 2023 I Process: Sending signal. PID: 2023 SIG: 9\n")
        report = evidence_summary(self.d)
        self.assertEqual(report["checks"]["first_original_resumed_pid"], "2023")
        self.assertTrue(report["checks"]["initial_activity_self_sigkill_observed"])
        self.assertEqual(report["checks"]["start_to_sigkill_seconds"], 4.48)
        self.assertEqual(report["next_action"], "FORENSIC_APP_EXIT")

    def test_activity_pid_mismatch_blocks_goal_even_if_pidof_is_stable(self):
        self.boot_and_launch()
        for t in (10, 35):
            self.write(f"pid-{t}.txt", "2018")
            self.write(f"activity-{t}.txt", original_activity("2296"))
        verdict = evidence_summary(self.d)
        self.assertEqual(verdict["checks"]["first_original_resumed_pid"], "2018")
        self.assertEqual(verdict["checks"]["activity_owners"]["10"], "2296")
        self.assertFalse(verdict["checks"]["original_activity_resumed_at_10_and_35"])
        self.assertEqual(verdict["goals"][2]["status"], "blocked")

    def test_task_history_not_counted_as_resumed(self):
        self.assertFalse(resumed("* Hist #0: ActivityRecord{a u0 "+PKG+" t8}"))

    def _contradictory_35s_checkpoints(self):
        self.boot_and_launch()
        for t in (10, 35):
            self.write(f"pid-{t}.txt", "2018")
            self.write(f"activity-{t}.txt", original_activity("2018"))
            (self.d / f"screen-{t}.png").write_bytes(
                b"\\x89PNG\\r\\n\\x1a\\n" + b"a" * 200)
        self.write("capture-timing.json", json.dumps({
            "clock": "monotonic",
            "checkpoints": [
                {"target_seconds": t, "elapsed_seconds": float(t),
                 "finished_seconds": float(t) + 0.2, "pid": "2018",
                 "activity_dump_captured": True}
                for t in (0, 1, 2, 3, 4, 10, 35)
            ],
        }))

    def test_35s_goal_rejects_same_pid_self_sigkill_at_four_seconds(self):
        self._contradictory_35s_checkpoints()
        self.write("logcat.txt",
            "10-15 18:23:42.179 287 735 I ActivityTaskManager: START u0 { cmp=" + PKG + " }\\n"
            "10-15 18:23:42.200 287 735 I ActivityManager: Start proc 2018:com.catsbit.oxidesurvivalisland/u0a87 for next-top-activity\\n"
            "10-15 18:23:46.763 2018 2018 I Process: Sending signal. PID: 2018 SIG: 9\\n")
        r = evidence_summary(self.d)
        self.assertTrue(r["checks"]["capture_timing"]["verified"])
        self.assertTrue(r["checks"]["fatal_original_exit_conflicts_with_35s"])
        self.assertEqual(r["goals"][2]["status"], "blocked")
        self.assertEqual(r["next_action"], "FORENSIC_APP_EXIT")
        self.assertFalse(r["gameplay_confirmed"])

    def test_35s_goal_rejects_same_pid_sigill_before_thirty_five_seconds(self):
        self._contradictory_35s_checkpoints()
        self.write("logcat.txt",
            "10-15 18:23:42.179 287 735 I ActivityTaskManager: START u0 { cmp=" + PKG + " }\\n"
            "10-15 18:23:42.200 287 735 I ActivityManager: Start proc 2018:com.catsbit.oxidesurvivalisland/u0a87 for next-top-activity\\n"
            "10-15 18:23:46.763 2018 2018 F libc: Fatal signal 4 (SIGILL), code -6 (SI_TKILL) in tid 2018 (esurvivalisland), pid 2018 (esurvivalisland)\\n")
        r = evidence_summary(self.d)
        self.assertTrue(r["checks"]["fatal_original_exit_conflicts_with_35s"])
        self.assertEqual(r["goals"][2]["status"], "blocked")

    def test_fatal_after_thirty_five_seconds_does_not_retroactively_erase_survival(self):
        self._contradictory_35s_checkpoints()
        self.write("logcat.txt",
            "10-15 18:23:42.179 287 735 I ActivityTaskManager: START u0 { cmp=" + PKG + " }\\n"
            "10-15 18:23:42.200 287 735 I ActivityManager: Start proc 2018:com.catsbit.oxidesurvivalisland/u0a87 for next-top-activity\\n"
            "10-15 18:24:22.179 2018 2018 I Process: Sending signal. PID: 2018 SIG: 9\\n")
        r = evidence_summary(self.d)
        self.assertFalse(r["checks"]["fatal_original_exit_conflicts_with_35s"])
        self.assertEqual(r["goals"][2]["status"], "complete")
        self.assertEqual(r["goals"][3]["status"], "pending_visual_review")
        self.assertFalse(r["gameplay_confirmed"])

    def test_unrelated_process_fatal_does_not_invalidate_original_activity(self):
        self._contradictory_35s_checkpoints()
        self.write("logcat.txt",
            "10-15 18:23:42.179 287 735 I ActivityTaskManager: START u0 { cmp=" + PKG + " }\\n"
            "10-15 18:23:42.200 287 735 I ActivityManager: Start proc 9999:com.catsbit.oxidesurvivalisland/u0a87 for next-top-activity\\n"
            "10-15 18:23:46.763 9999 9999 I Process: Sending signal. PID: 9999 SIG: 9\\n")
        r = evidence_summary(self.d)
        self.assertFalse(r["checks"]["fatal_original_exit_conflicts_with_35s"])
        self.assertEqual(r["goals"][2]["status"], "complete")
        self.assertFalse(r["gameplay_confirmed"])

    def test_stable_pid_and_activity_only_awaits_visual_review(self):
        self.boot_and_launch()
        for t in (10,35):
            self.write(f'pid-{t}.txt','2018')
            self.write(f'activity-{t}.txt',original_activity(2018))
            (self.d/f'screen-{t}.png').write_bytes(b'\x89PNG\r\n\x1a\n'+b'a'*200)
        self.write("capture-timing.json", json.dumps({
            "clock": "monotonic",
            "checkpoints": [
                {"target_seconds": t, "elapsed_seconds": float(t), "finished_seconds": float(t)+0.2,
                 "pid": "2018", "activity_dump_captured": True}
                for t in (0, 1, 2, 3, 4, 10, 35)
            ],
        }))
        r=evidence_summary(self.d)
        self.assertEqual([g['status'] for g in r['goals']], ['complete','complete','complete','pending_visual_review','pending'])
        self.assertFalse(r['gameplay_confirmed'])
        self.assertFalse(r['synthetic_overlay_confirmed'])


if __name__ == '__main__':
    unittest.main()
