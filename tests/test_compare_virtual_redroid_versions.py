import sys
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from compare_virtual_redroid_versions import summarize
def sample(pid,death,alive=False,resumed=False):
    return {"boot":"PASS","install":"SUCCESS","host_architecture":"aarch64",
            "abi":"arm64-v8a","play_services":"NOT_FOUND",
            "forensic":{"initial_activity_pid":pid,"classification":death},
            "initial_pid_alive_10s":alive,"activity_resumed_10s":resumed}
class VirtualComparison(unittest.TestCase):
    def test_two_real_android_runs_both_self_kill(self):
        r=summarize({"12":sample(100,"ORIGINAL_ACTIVITY_SELF_SIGKILL"),
                     "14":sample(400,"ORIGINAL_ACTIVITY_SELF_SIGKILL")})
        self.assertTrue(r["both_android_images_booted"])
        self.assertEqual(r["versions"][1]["outcome"],"ORIGINAL_SELF_SIGKILL")
        self.assertFalse(r["original_gameplay_verified"])
    def test_alive_means_only_activity(self):
        r=summarize({"12":sample(10,"INITIAL_ACTIVITY_NO_SELF_SIGKILL_OBSERVED",True,True),
                     "14":sample(11,"INITIAL_ACTIVITY_NO_SELF_SIGKILL_OBSERVED",True,True)})
        self.assertEqual(r["versions"][0]["outcome"],"ACTIVITY_ALIVE_AT_10S_GAMEPLAY_UNVERIFIED")
        self.assertFalse(r["versions"][0]["gameplay_verified"])
    def test_android_boot_failure_does_not_claim_game_success(self):
        r=summarize({"12":{},"14":sample(44,"ORIGINAL_ACTIVITY_SELF_SIGKILL")})
        self.assertFalse(r["both_android_images_booted"])
        self.assertEqual(r["versions"][0]["outcome"],"GAME_INSTALL_NOT_CONFIRMED")
    def test_other_package_pid_does_not_count_as_original(self):
        r=summarize({"12":sample(None,"ORIGINAL_ACTIVITY_START_NOT_FOUND",True,True),
                     "14":sample(33,"ORIGINAL_ACTIVITY_SELF_SIGKILL")})
        self.assertEqual(r["versions"][0]["outcome"],"ORIGINAL_ACTIVITY_START_NOT_CONFIRMED")
if __name__=="__main__":
    unittest.main()
