import json
import sys
import tempfile
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from diagnose_virtual_activity_pid import analyze

APP="com.catsbit.oxidesurvivalisland"
ACT="cTOOT3.abKmV1.tP0Gr1.OxideActivity"

class LifecycleRecovery(unittest.TestCase):
    def check(self,trace="",logs="",forensic=None,launch="Status: ok"):
        with tempfile.TemporaryDirectory() as tmp:
            d=Path(tmp)
            (d/"pid_trace.txt").write_text(trace)
            (d/"all_buffers.txt").write_text(logs)
            (d/"activity_start.txt").write_text(launch)
            (d/"forensic_summary.json").write_text(json.dumps(forensic or {}))
            return analyze(d)
    def test_android12_missing_event_but_main_pid_was_sampled(self):
        trace="\n".join(
            f"sample={i} utc=2026-10-13T23:58:0{i}Z pid=1444"
            for i in range(1,5))
        log="10-13 23:58:12.354  224  593 I wm_finish_activity: [0,8,9,"+APP+"/"+ACT+",finish-activity]"
        r=self.check(trace,log)
        self.assertEqual(r["selected_pid"],1444)
        self.assertEqual(r["pid_source"],"MAIN_PACKAGE_PID_TRACE_ONLY")
        self.assertFalse(r["pid_confirms_activity_process"])
        self.assertFalse(r["self_sigkill_attributed_to_original_activity"])
    def test_android12_activity_manager_fallback(self):
        log=("10-13 23:58:10.123 224 224 I ActivityManager: "
             "Start proc 1444:"+APP+"/u0a77 for next-top-activity "
             "{"+APP+"/"+ACT+"}\n"
             "10-13 23:58:13.500 1444 1444 I Process : Sending signal. PID: 1444 SIG: 9")
        r=self.check("sample=1 utc=2026-10-13T23:58:11Z pid=1444",log)
        self.assertEqual(r["pid_source"],"ACTIVITYMANAGER_NEXT_TOP_ACTIVITY_LOG")
        self.assertTrue(r["self_sigkill_attributed_to_original_activity"])
    def test_am_proc_start_remains_authoritative(self):
        forensic={"initial_activity_pid":2578,
                  "classification":"ORIGINAL_ACTIVITY_SELF_SIGKILL"}
        r=self.check("sample=1 utc=2026-10-13T23:58:11Z pid=8888","",forensic)
        self.assertEqual(r["selected_pid"],2578)
        self.assertTrue(r["pid_confirms_activity_process"])
    def test_background_service_not_in_pid_trace(self):
        log=("10-13 23:58:10.123 224 224 I ActivityManager: "
             "Start proc 5555:"+APP+"/u0a77 for service "
             "{"+APP+"/com.google.android.gms.measurement.AppMeasurementJobService}")
        r=self.check("",log)
        self.assertEqual(r["status"],"NO_PROCESS_PID_OBSERVED")
    def test_other_pid_sigkill_not_correlated(self):
        log="10-13 23:58:12.123 3333 3333 I Process : Sending signal. PID: 3333 SIG: 9"
        r=self.check("sample=1 utc=2026-10-13T23:58:11Z pid=1444",log)
        self.assertFalse(r["android_process_self_sigkill_logged_for_selected_pid"])
    def test_no_evidence_never_claims_game_started(self):
        r=self.check()
        self.assertIsNone(r["selected_pid"])
        self.assertFalse(r["gameplay_verified"])
    def test_pid_trace_multiple_entries_keep_other_pid_separate(self):
        r=self.check("sample=1 utc=2026-10-13T23:58:11Z pid=1444,4000\n"
                     "sample=2 utc=2026-10-13T23:58:12Z pid=1444")
        self.assertEqual(r["selected_pid"],1444)
        self.assertEqual(r["other_observed_pids"],[4000])
if __name__=="__main__":
    unittest.main()
