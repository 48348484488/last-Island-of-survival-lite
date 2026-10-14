"""Realistic original Android log fixtures; no APK or device required."""
import sys,tempfile,unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from audit_original_launch_stages import analyze,compare
P="com.catsbit.oxidesurvivalisland"
A=P+"/cTOOT3.abKmV1.tP0Gr1.OxideActivity"
def row(clock,pid,tag,message):
    return f"10-14 13:49:{clock} {pid:5d} {pid:5d} I {tag}: {message}"
def start(clock,pid):
    return row(clock,228,"am_proc_start",f"[0,{pid},10077,{P},next-top-activity,{{{A}}}]")
class Tests(unittest.TestCase):
    def run_rows(self,rows):
        tmp=tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        Path(tmp.name,"all_buffers.txt").write_text("\n".join(rows)+"\n")
        return analyze(tmp.name)
    def test_realistic_milestones_and_distinct_system_graphics(self):
        x=self.run_rows([start("32.985",2168),
           row("33.021",2168,"linker","libloader.so"),
           row("33.297",2168,"IL2CPP","JNI_OnLoad"),
           row("33.750",2168,"Unity","Built from 6000.3"),
           row("33.135",540,"GraphicBufferAllocator","Failed to allocate"),
           row("33.170",2168,"GooglePlayServicesUtil","requires the Google Play Store"),
           row("37.389",228,"ActivityTaskManager",f"Fully drawn {A}"),
           row("37.520",228,"wm_finish_activity",f"[0,3,8,{A},finish-activity]"),
           row("37.633",2168,"Process","Sending signal. PID: 2168 SIG: 9"),
           row("37.869",228,"am_proc_died",f"[0,2168,{P},200,2]")])
        self.assertEqual(x["activity_pid"],2168)
        self.assertEqual(x["milestones_ms_after_activity_start"]["il2cpp_jni"],312)
        self.assertEqual(x["milestones_ms_after_activity_start"]["fully_drawn"],4404)
        self.assertEqual(x["milestones_ms_after_activity_start"]["self_sigkill"],4648)
        self.assertEqual(x["warnings"]["graphics_allocator_errors_other_pid"],1)
        self.assertEqual(x["warnings"]["play_store_missing_game_pid"],1)
        self.assertFalse(x["playable_game_verified"])
        self.assertFalse(x["security_cause_established"])
    def test_previous_or_other_pid_unity_not_counted(self):
        x=self.run_rows([start("20.000",1000),
            row("21.000",1000,"Unity","Built from 6000"),
            start("32.985",2168),
            row("33.100",1000,"IL2CPP","JNI_OnLoad"),
            row("33.100",540,"Unity","Built from 6000"),
            row("37.633",2168,"Process","Sending signal. PID: 2168 SIG: 9")])
        self.assertFalse(x["unity_and_il2cpp_observed"])
        self.assertEqual(x["activity_pid"],2168)
        self.assertTrue(x["self_sigkill_observed"])
    def test_after_signal_is_not_game_start(self):
        x=self.run_rows([start("32.985",2168),
            row("37.633",2168,"Process","Sending signal. PID: 2168 SIG: 9"),
            row("38.100",2168,"Unity","Built from 6000"),
            row("38.200",228,"ActivityTaskManager",f"Fully drawn {A}")])
        self.assertIsNone(x["milestones_ms_after_activity_start"]["unity_version"])
        self.assertIsNone(x["milestones_ms_after_activity_start"]["fully_drawn"])
    def test_without_original_start_never_claim_success(self):
        x=self.run_rows([row("37.633",2168,"Process","Sending signal. PID: 2168 SIG: 9")])
        self.assertIsNone(x["activity_pid"])
        with self.assertRaisesRegex(ValueError,"absent"):compare(x,x)
    def test_android_process_warning_not_game(self):
        x=self.run_rows([start("32.985",2168),
            row("33.135",540,"GraphicBufferAllocator","Failed to allocate"),
            row("33.170",540,"GooglePlayServicesUtil","requires the Google Play Store")])
        self.assertEqual(x["warnings"]["play_store_missing_game_pid"],0)
        self.assertEqual(x["warnings"]["graphics_allocator_errors_other_pid"],1)
if __name__=="__main__":unittest.main()
