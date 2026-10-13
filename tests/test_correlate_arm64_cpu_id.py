import importlib.util
import sys
import unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1]/"tools"))
from correlate_arm64_cpu_id import analyze

def row(pid,tag,msg,ts="23:22:09.274"):
    return f"10-13 {ts} {pid} {pid} I {tag}: {msg}"

class CompareCpuId(unittest.TestCase):
    def make_logs(self,opcode="0xd5380420",error_pid=2727):
        translated="\n".join([
            row(2727,"OXIDE_CPU_ID_PROBE","STAGE=BEFORE_MRS_ID_AA64PFR1_EL1"),
            row(error_pid,"berberis","Undefined instruction "+opcode),
            row(2727,"libc","Fatal signal 4 (SIGILL), code -6 (SI_TKILL)")])
        native="\n".join([
            row(1720,"OXIDE_CPU_ID_PROBE","STAGE=BEFORE_MRS_ID_AA64PFR1_EL1"),
            row(1720,"OXIDE_CPU_ID_PROBE","STAGE=MRS_RETURNED VALUE=0x0")])
        return translated,native
    def test_matched_opcode_differs_only_destination_and_native_returns(self):
        a,b=self.make_logs()
        r=analyze(a,b)
        self.assertEqual(r["status"],"ARM64_TRANSLATOR_MRS_INCOMPATIBILITY_REPRODUCED")
        self.assertFalse(r["original_game_sigill_root_cause_proven"])
        self.assertTrue(r["translated_bridge_errors"][0]["matches_original_instruction_ignoring_destination_register"])
    def test_wrong_instruction_no_proof(self):
        a,b=self.make_logs("0xd503201f")
        r=analyze(a,b)
        self.assertTrue(r["status"].startswith("EVIDENCE_INCOMPLETE"))
    def test_other_pid_cannot_count(self):
        a,b=self.make_logs(error_pid=1222)
        self.assertEqual(analyze(a,b)["translated_bridge_errors"],[])
    def test_native_mrs_not_returned_no_success(self):
        a,b=self.make_logs()
        b=b.splitlines()[0]
        self.assertTrue(analyze(a,b)["status"].startswith("EVIDENCE_INCOMPLETE"))
if __name__=="__main__":
    unittest.main()
