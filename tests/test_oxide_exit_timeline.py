"""Deterministic replays: synthetic excerpts reflecting real Android 15/16 timelines."""
import unittest
from tools.oxide_exit_timeline import assess, ACTIVITY

def fixture(start="18:55:51.207", draw="18:55:54.292", close="18:55:55.684",
            kill="18:55:55.687", death="18:55:55.728", proc=2023):
    return "\n".join([
        f"10-15 {start}   281  1748 I ActivityTaskManager: START u0 {{cmp={ACTIVITY}}} from uid 2000",
        f"10-15 18:55:51.215   281   314 I ActivityManager: Start proc {proc}:com.catsbit.oxidesurvivalisland/u0a87 for next-top-activity",
        "10-15 18:55:51.295  2023  2071 W GooglePlayServicesUtil: com.catsbit.oxidesurvivalisland requires the Google Play Store, but it is missing.",
        f"10-15 {draw}   281   302 I ActivityTaskManager: Fully drawn {ACTIVITY}: +3s91ms",
        f"10-15 {kill}  {proc}  {proc} I Process : Sending signal. PID: {proc} SIG: 9",
        f"10-15 {death}   281  1748 I ActivityManager: Process com.catsbit.oxidesurvivalisland (pid {proc}) has died: fg  TOP",
        f"10-15 18:55:55.871   281   302 V WindowManager: Sent Transition (#6) createdAt=10-15 {close} via request=TransitionRequestInfo {{ type = CLOSE, triggerTask = TaskInfo{{taskId=8 baseIntent=Intent {{ cmp={ACTIVITY} }} }} }}",
    ])

class ExitTimelineTests(unittest.TestCase):
    def test_android15(self):
        r = assess(fixture())
        self.assertEqual(r["status"], "TASK_CLOSE_PRECEDES_ORIGINAL_SELF_SIGKILL")
        self.assertEqual(r["evidence"]["start_to_self_sigkill_ms"], 4480)
        self.assertEqual(r["evidence"]["task_close_before_self_sigkill_ms"], 3)
        self.assertFalse(r["gameplay_verified"])
        self.assertEqual(r["security_trigger"], "UNKNOWN")

    def test_android16(self):
        r = assess(fixture(start="18:38:25.470", draw="18:38:28.642",
                           close="18:38:30.137", kill="18:38:30.140",
                           death="18:38:30.182", proc=2108))
        self.assertEqual(r["evidence"]["start_to_self_sigkill_ms"], 4670)
        self.assertEqual(r["evidence"]["task_close_before_self_sigkill_ms"], 3)

    def test_unrelated_sender_rejected(self):
        log = fixture().replace("2023  2023 I Process : Sending signal. PID: 2023",
                                "2222  2222 I Process : Sending signal. PID: 2023")
        self.assertEqual(assess(log)["status"], "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE")

    def test_other_app_close_rejected(self):
        log = fixture().replace("cmp=" + ACTIVITY + " } } }",
                                "cmp=com.example.other/.Activity } } }")
        self.assertEqual(assess(log)["status"], "ORIGINAL_SELF_SIGKILL_NO_MATCHED_CLOSE")

    def test_stale_close_rejected(self):
        self.assertEqual(assess(fixture(close="18:55:51.684"))["status"],
                         "ORIGINAL_SELF_SIGKILL_NO_MATCHED_CLOSE")

    def test_close_after_signal_rejected(self):
        self.assertEqual(assess(fixture(close="18:55:55.690"))["status"],
                         "ORIGINAL_SELF_SIGKILL_NO_MATCHED_CLOSE")

    def test_other_process_death_rejected(self):
        log = fixture().replace("(pid 2023) has died", "(pid 9999) has died")
        self.assertEqual(assess(log)["status"], "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE")

    def test_missing_records_fail_closed(self):
        r = assess("")
        self.assertEqual(r["status"], "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE")
        self.assertIsNone(r["evidence"]["original_pid"])

    def test_invalid_year(self):
        with self.assertRaises(ValueError):
            assess(fixture(), 1999)

if __name__ == "__main__":
    unittest.main()
