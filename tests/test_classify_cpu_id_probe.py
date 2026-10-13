import importlib.util
import unittest
from pathlib import Path
spec=importlib.util.spec_from_file_location(
    "cpu",Path(__file__).resolve().parents[1]/"tools/classify_cpu_id_probe.py")
cpu=importlib.util.module_from_spec(spec)
spec.loader.exec_module(cpu)

def log(pid, tag, txt):
    return "10-13 23:30:01.111 %d %d I %s: %s"%(pid,pid,tag,txt)

class ProbeTests(unittest.TestCase):
    def test_same_pid_return(self):
        s="\n".join([log(123,"OXIDE_CPU_ID_PROBE",cpu.PRE),
            log(123,"OXIDE_CPU_ID_PROBE","STAGE=MRS_RETURNED VALUE=0x0")])
        r=cpu.classify(s)
        self.assertTrue(r["mrs_returned"])
        self.assertFalse(r["original_game_executed"])
    def test_independent_signal_4(self):
        s="\n".join([log(123,"OXIDE_CPU_ID_PROBE",cpu.PRE),
             log(123,"libc","Fatal signal 4 (SIGILL), code -6 (SI_TKILL)")])
        r=cpu.classify(s)
        self.assertEqual(r["status"],"INDEPENDENT_CPU_ID_MRS_SIGILL_ON_NATIVE_BRIDGE")
        self.assertTrue(r["sigill_recorded_for_same_pid"])
        self.assertEqual(r["original_game_sigill_causation"],"NOT_PROVEN")
    def test_unrelated_pid_signal_not_accepted(self):
        s="\n".join([log(123,"OXIDE_CPU_ID_PROBE",cpu.PRE),
             log(111,"libc","Fatal signal 4 (SIGILL), code -6 (SI_TKILL)")])
        self.assertEqual(cpu.classify(s)["status"],"PROBE_STARTED_NO_RETURN_OR_SIGILL_OBSERVED")
    def test_missing_jni_stage_fail_closed(self):
        self.assertEqual(cpu.classify("anything")["status"],"PROBE_DID_NOT_REACH_MRS")
if __name__=="__main__":
    unittest.main()
