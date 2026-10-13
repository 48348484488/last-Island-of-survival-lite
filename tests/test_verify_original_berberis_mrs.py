import sys
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from verify_original_berberis_mrs import verify

def row(pid,tag,msg):
    return f"10-13 23:22:09.274 {pid} {pid} I {tag}: {msg}"

class GameBerberis(unittest.TestCase):
    def fixtures(self):
        original="\n".join([
            row(4078,"berberis","Undefined instruction 0xd5380429 at 0xAAA"),
            row(4078,"libc","Fatal signal 4 (SIGILL), code -6 (SI_TKILL)"),
            row(4104,"DEBUG","#00 pc 0000000000830bec /data/app/original/lib/arm64/libloader.so")])
        translated="\n".join([
            row(2727,"OXIDE_CPU_ID_PROBE","STAGE=BEFORE_MRS_ID_AA64PFR1_EL1"),
            row(2727,"berberis","Undefined instruction 0xd5380420 at 0xBB"),
            row(2727,"libc","Fatal signal 4 (SIGILL), code -6 (SI_TKILL)")])
        native="\n".join([
            row(1720,"OXIDE_CPU_ID_PROBE","STAGE=BEFORE_MRS_ID_AA64PFR1_EL1"),
            row(1720,"OXIDE_CPU_ID_PROBE","STAGE=MRS_RETURNED VALUE=0x0")])
        return original,{"initial_activity_pid":4078},translated,native
    def test_three_experiments_correlate_direct_game_crash(self):
        r=verify(*self.fixtures())
        self.assertTrue(r["status"].startswith("ORIGINAL_OXIDE_BERBERIS_MRS"))
        self.assertTrue(r["original_game_guest_top_frame_pc_confirmed"])
        self.assertFalse(r["original_game_playable"])
    def test_unrelated_game_process_cannot_claim_direct_crash(self):
        a,b,c,d=self.fixtures()
        a=a.replace("4078 4078","9988 9988")
        self.assertTrue(verify(a,b,c,d)["status"].startswith("INCOMPLETE"))
    def test_wrong_game_opcode_cannot_claim_direct_crash(self):
        a,b,c,d=self.fixtures()
        a=a.replace("0xd5380429","0xd503201f")
        self.assertTrue(verify(a,b,c,d)["status"].startswith("INCOMPLETE"))
    def test_missing_guest_frame_fails_closed(self):
        a,b,c,d=self.fixtures()
        a=a.replace("0x830bec","0x830bad").replace("0000000000830bec","0000000000830bad")
        self.assertTrue(verify(a,b,c,d)["status"].startswith("INCOMPLETE"))
    def test_independent_bridge_not_failing_is_not_confirmed(self):
        a,b,c,d=self.fixtures()
        c=c.replace("Undefined instruction 0xd5380420","Unsupported call")
        self.assertTrue(verify(a,b,c,d)["status"].startswith("INCOMPLETE"))
if __name__=="__main__":
    unittest.main()
