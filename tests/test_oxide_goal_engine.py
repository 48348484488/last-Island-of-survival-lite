import json
import tempfile
import unittest
from pathlib import Path
from tools.oxide_goal_engine import evidence_summary, resumed

PKG = "com.catsbit.oxidesurvivalisland/.OxideActivity"


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
            ("activity-0.txt", "topResumedActivity=ActivityRecord{a u0 "+PKG+" t8}")
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
            self.write(f'activity-{t}.txt', 'topResumedActivity=ActivityRecord{a u0 '+PKG+' t8}')
            (self.d/f'screen-{t}.png').write_bytes(b'\x89PNG\r\n\x1a\n'+b'a'*200)
        report=evidence_summary(self.d)
        self.assertEqual(report['goals'][2]['status'], 'blocked')
        self.assertFalse(report['checks']['capture_timing']['verified'])
        self.assertFalse(report['gameplay_confirmed'])

    def test_task_history_not_counted_as_resumed(self):
        self.assertFalse(resumed("* Hist #0: ActivityRecord{a u0 "+PKG+" t8}"))

    def test_stable_pid_and_activity_only_awaits_visual_review(self):
        self.boot_and_launch()
        for t in (10,35):
            self.write(f'pid-{t}.txt','2018')
            self.write(f'activity-{t}.txt','topResumedActivity=ActivityRecord{a u0 '+PKG+' t8}')
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
