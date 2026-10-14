import sys
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from verify_virtual_android12_process import verify,activity_pid

APP="com.catsbit.oxidesurvivalisland"
ACT="cTOOT3.abKmV1.tP0Gr1.OxideActivity"
SNAP=(
"  mResumedActivity: ActivityRecord{aaa u0 "+APP+"/"+ACT+" t11}\n"
"  * Hist #0: ActivityRecord{aaa u0 "+APP+"/"+ACT+" t11}\n"
"        packageName="+APP+" processName="+APP+"\n"
"        app=ProcessRecord{abc 1829:"+APP+"/u0a70}\n"
)
TRACE=("sample=1 utc=2026-10-14T00:08:24Z pid=1829\n"
       "sample=2 utc=2026-10-14T00:08:25Z pid=1829\n")
EXIT=(
"  package: "+APP+"\n        ApplicationExitInfo #0:\n"
"          timestamp=2026-10-14 00:08:29.328\n"
"          pid=1829\n          process="+APP+"\n"
"          reason=2 (SIGNALED)\n          subreason=3 (TOO MANY EMPTY PROCS)\n"
"          status=9\n          description=null\n"
)
OLD={"selected_pid":1829,
     "android_process_self_sigkill_logged_for_selected_pid":True}

class CrossSourceTests(unittest.TestCase):
    def test_recovered_snapshot_activity_pid_and_sigkill(self):
        d=verify(SNAP,TRACE,EXIT,OLD)
        self.assertEqual(d["status"],"ORIGINAL_ACTIVITY_PID_AND_SIGNAL_CONFIRMED")
        self.assertEqual(d["activity_snapshot_pid"],1829)
        self.assertTrue(d["android_exit_info_sigkill_status9"])
        self.assertFalse(d["multiplayer_gameplay_verified"])
    def test_wrong_pid_logcat_is_not_original_game(self):
        self.assertTrue(verify(SNAP,TRACE,EXIT,{"selected_pid":99,
                 "android_process_self_sigkill_logged_for_selected_pid":True})
                  ["status"].startswith("INCOMPLETE"))
    def test_no_resumed_original_activity_not_confirmed(self):
        self.assertTrue(verify(SNAP.replace("mResumedActivity:","other:"),TRACE,EXIT,OLD)
                  ["status"].startswith("INCOMPLETE"))
    def test_app_pid_not_in_foreground_activity_record(self):
        snap=SNAP.replace("app=ProcessRecord{abc 1829:"+APP,
                          "app=ProcessRecord{abc 1829:other.app")
        self.assertIsNone(activity_pid(snap))
    def test_exit_info_other_signal_cannot_count(self):
        self.assertTrue(verify(SNAP,TRACE,EXIT.replace("status=9","status=11"),OLD)
                  ["status"].startswith("INCOMPLETE"))
    def test_multiple_trace_pids_not_sufficient_if_pid_missing(self):
        self.assertTrue(verify(SNAP,"sample=1 utc=now pid=3333\n",EXIT,OLD)
                  ["status"].startswith("INCOMPLETE"))
if __name__=="__main__":
    unittest.main()
