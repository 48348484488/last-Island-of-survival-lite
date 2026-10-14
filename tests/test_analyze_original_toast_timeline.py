import sys
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from analyze_original_toast_timeline import analyze,compare

APP="com.catsbit.oxidesurvivalisland"
ACT="cTOOT3.abKmV1.tP0Gr1.OxideActivity"
def row(at,pid,tag,msg):
    return f"10-13 {at} {pid} {pid} I {tag}: {msg}"
def log(pid,start,toast,finish,signal,death,cleanup):
    return "\n".join([
      row(start,224,"am_proc_start",f"[0,{pid},10077,{APP},next-top-activity,{{{APP}/{ACT}}}]"),
      row(toast,224,"CoreBackPreview","Window{abcd123 u0 Toast}: Setting back callback OnBackInvokedCallbackInfo{mCallback=x}"),
      row(finish,224,"wm_finish_activity",f"[0,1,8,{APP}/{ACT},finish-activity]"),
      row(signal,pid,"Process",f"Sending signal. PID: {pid} SIG: 9"),
      row(death,224,"am_proc_died",f"[0,{pid},{APP},0,2]"),
      row(cleanup,224,"NotificationService",f"Toast already killed. pkg={APP} token=xyz"),
    ])
class TimelineTests(unittest.TestCase):
    def test_exact_real_shape_ordering(self):
        t=log(2197,"22:35:04.086","22:35:04.946","22:35:08.928",
          "22:35:08.981","22:35:09.026","22:35:09.059")
        out=analyze(t)
        self.assertEqual(out["classification"],"ORIGINAL_SELF_SIGKILL_LIFECYCLE_AND_TOAST_WINDOW_OBSERVED")
        self.assertEqual(out["phases_ms_from_original_start"]["generic_toast_window"],860)
        self.assertEqual(out["phases_ms_from_original_start"]["original_process_self_sigkill"],4895)
        self.assertFalse(out["toast_text_captured_in_logs"])
    def test_second_launch_ignores_old_buffer_rows(self):
        a=log(2197,"22:35:04.086","22:35:04.946","22:35:08.928",
          "22:35:08.981","22:35:09.026","22:35:09.059")
        b=log(2578,"22:35:15.776","22:35:16.428","22:35:20.380",
          "22:35:20.425","22:35:20.483","22:35:20.526")
        out=analyze(a+"\n"+b)
        self.assertEqual(out["original_activity_pid"],2578)
        self.assertEqual(out["original_activity_launch_events_in_buffer"],2)
        self.assertEqual(out["phases_ms_from_original_start"]["generic_toast_window"],652)
        self.assertEqual(out["phases_ms_from_original_start"]["original_process_self_sigkill"],4649)
        self.assertEqual(compare(analyze(a),out)["status"],"REPEATED_ACTIVITY_SELF_KILL_WITH_TOAST_WINDOW")
    def test_other_process_signal_does_not_count(self):
        a=log(2197,"22:35:04.086","22:35:04.946","22:35:08.928",
          "22:35:08.981","22:35:09.026","22:35:09.059")
        a=a.replace("22:35:08.981 2197 2197","22:35:08.981 224 224")
        self.assertEqual(analyze(a)["classification"],"PARTIAL_LIFECYCLE_EVIDENCE")
    def test_window_is_not_attested_as_game_owner(self):
        a=log(2197,"22:35:04.086","22:35:04.946","22:35:08.928",
          "22:35:08.981","22:35:09.026","22:35:09.059")
        out=analyze(a)
        self.assertEqual(out["toast_window_specific_package_owner"],"NOT_ATTESTED")
        self.assertEqual(out["specific_protection_trigger"],"UNKNOWN")
        self.assertFalse(out["gameplay_verified"])
    def test_if_no_current_activity_do_not_infer_from_toast(self):
        a=row("22:35:04.946",224,"CoreBackPreview",
              "Window{abcd123 u0 Toast}: Setting back callback OnBackInvokedCallbackInfo")
        self.assertEqual(analyze(a)["classification"],"NO_ORIGINAL_ACTIVITY_START_EVENT")
if __name__=="__main__":
    unittest.main()
