import sys
import tempfile
import unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
import audit_original_boot_timeline as audit

PACKAGE = "com.catsbit.oxidesurvivalisland"
ACTIVITY = PACKAGE + "/cTOOT3.abKmV1.tP0Gr1.OxideActivity"

def stamp(t, pid, tag, msg, tid=None):
    return "10-13 " + t + " " + str(pid) + " " + str(tid or pid) + " I " + tag + ": " + msg

def start(t, pid):
    return stamp(t, 224, "am_proc_start", "[0," + str(pid) +
                 ",10077," + PACKAGE + ",next-top-activity,{" + ACTIVITY + "}]")

class TimelineTests(unittest.TestCase):
    def run_lines(self, entries):
        with tempfile.TemporaryDirectory() as folder:
            Path(folder,"all_buffers.txt").write_text("\n".join(entries)+"\n")
            return audit.analyze(folder)

    def test_latest_pid_only_and_real_self_sigkill(self):
        entries = [
            start("22:35:04.086", 2197),
            stamp("22:35:04.270", 2197, "GooglePlayServicesUtil", "Google Play services is missing"),
            stamp("22:35:09.025", 2197, "Process", "Sending signal. PID: 2197 SIG: 9"),
            start("22:35:15.776", 2578),
            stamp("22:35:15.976", 2578, "FirebaseInstanceId",
                  "Google Play services missing or without correct permission"),
            stamp("22:35:16.076", 2578, "IL2CPP", "JNI_OnLoad"),
            stamp("22:35:16.388", 2578, "Sensors", "Failed to get Sensor list"),
            stamp("22:35:20.380", 224, "wm_finish_activity",
                  "[0,2,9," + ACTIVITY + ",finish-activity]"),
            stamp("22:35:20.425", 2578, "Process", "Sending signal. PID: 2578 SIG: 9"),
            stamp("22:35:20.500", 2578, "Sensors", "Failed to get Sensor list"),
        ]
        r = self.run_lines(entries)
        self.assertEqual(r["activity_pid"], 2578)
        self.assertEqual(r["process_exit_classification"], "ORIGINAL_ACTIVITY_SELF_SIGKILL")
        self.assertEqual(r["time_to_self_sigkill_ms"], 4649)
        self.assertEqual(r["events"]["firebase_gms_unavailable"]["first_after_launch_ms"], 200)
        self.assertEqual(r["events"]["il2cpp_jni_loaded"]["count"], 1)
        self.assertEqual(r["events"]["sensor_list_errors"]["count"], 1)
        self.assertEqual(r["events"]["play_services_missing"]["count"], 0)

    def test_unrelated_graphics_process_not_counted(self):
        r = self.run_lines([start("22:35:15.776",2578),
                            stamp("22:35:16.000", 889, "GraphicBufferAllocator",
                                  "Failed to allocate"), 
                            stamp("22:35:16.010", 2578, "GraphicBufferAllocator",
                                  "Failed to allocate")])
        self.assertEqual(r["events"]["graphic_buffer_errors"]["count"], 1)
        self.assertIsNone(r["time_to_self_sigkill_ms"])

    def test_no_pid_is_unknown_not_success(self):
        r = self.run_lines([stamp("22:35:20.425",2578,"Process",
                                  "Sending signal. PID: 2578 SIG: 9")])
        self.assertIsNone(r["activity_pid"])
        self.assertEqual(r["process_exit_classification"], "ORIGINAL_ACTIVITY_START_NOT_FOUND")

    def test_both_killed_is_not_playable(self):
        r = self.run_lines([start("22:35:15.776",2578),
                            stamp("22:35:20.425",2578,"Process",
                                  "Sending signal. PID: 2578 SIG: 9")])
        c = audit.compare(r,r)
        self.assertTrue(c["both_self_terminated"])
        self.assertFalse(c["original_gameplay_verified"])
        self.assertFalse(c["root_cause_identified"])

if __name__ == "__main__":
    unittest.main()
