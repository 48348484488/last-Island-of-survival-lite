import importlib.util
import unittest
from pathlib import Path

source = Path(__file__).resolve().parents[1] / "tools" / "audit_original_new_virtual_android.py"
spec = importlib.util.spec_from_file_location("oxide_new_lifecycle", source)
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

def evidence():
    return {
        "boot": "PASS", "abi": "arm64-v8a", "install": "SUCCESS",
        "forensic": {
            "initial_activity_pid": 3001,
            "classification": "INITIAL_ACTIVITY_NO_SELF_SIGKILL_OBSERVED",
            "first_self_sigkill": None,
        },
        "lifecycle_recovery": {},
        "initial_pid_alive_10s": True, "activity_resumed_10s": True,
        "initial_pid_alive_35s": True, "activity_resumed_35s": True,
    }

class OriginalLifecycleTests(unittest.TestCase):
    def test_full_activity_checkpoint_cannot_prove_gameplay(self):
        d = mod.evaluate("15", evidence())
        self.assertEqual(d["status"], "ACTIVITY_35S_CHECKPOINTS_PASS_NOT_GAMEPLAY")
        self.assertFalse(d["gameplay_verified"])
        self.assertFalse(d["real_player_esp_verified"])
    def test_any_self_signal_overrides_bogus_liveness(self):
        d = evidence()
        d["forensic"]["classification"] = "ORIGINAL_ACTIVITY_SELF_SIGKILL"
        d["forensic"]["first_self_sigkill"] = "10-14 17:00:03.400"
        self.assertEqual(mod.evaluate("15", d)["status"], "ORIGINAL_ACTIVITY_SELF_SIGKILL")
    def test_missing_game_install(self):
        d = evidence()
        d["install"] = "FAIL"
        self.assertEqual(mod.evaluate("15", d)["status"], "ORIGINAL_APK_INSTALL_NOT_VERIFIED")
    def test_missing_boot(self):
        d = evidence()
        d["boot"] = "FAIL"
        self.assertEqual(mod.evaluate("15", d)["status"], "VIRTUAL_ANDROID_BOOT_NOT_VERIFIED")
    def test_bad_guest_arch(self):
        d = evidence()
        d["abi"] = "x86_64"
        self.assertEqual(mod.evaluate("16", d)["status"], "VIRTUAL_ANDROID_BOOT_NOT_VERIFIED")
    def test_other_package_pid_not_accepted(self):
        d = evidence()
        d["forensic"]["initial_activity_pid"] = None
        d["lifecycle_recovery"] = {"selected_pid": 12345, "pid_confirms_activity_process": False}
        self.assertEqual(mod.evaluate("15", d)["status"], "ORIGINAL_ACTIVITY_PID_NOT_VERIFIED")
    def test_activity_pid_recovery_used_when_correlated(self):
        d = evidence()
        d["forensic"]["initial_activity_pid"] = None
        d["lifecycle_recovery"] = {"selected_pid": 3001, "pid_confirms_activity_process": True,
                                   "self_sigkill_attributed_to_original_activity": True}
        self.assertEqual(mod.evaluate("15", d)["status"], "ORIGINAL_ACTIVITY_SELF_SIGKILL")
    def test_pid_without_35s_is_not_survival(self):
        d = evidence()
        d["initial_pid_alive_35s"] = False
        self.assertEqual(mod.evaluate("15", d)["status"], "ORIGINAL_ACTIVITY_35S_NOT_CONFIRMED")
    def test_wrong_activity_at_35s_does_not_pass(self):
        d = evidence()
        d["activity_resumed_35s"] = False
        self.assertEqual(mod.evaluate("16", d)["status"], "ORIGINAL_ACTIVITY_35S_NOT_CONFIRMED")
    def test_boolean_pid_not_accepted(self):
        d = evidence()
        d["forensic"]["initial_activity_pid"] = True
        self.assertEqual(mod.evaluate("15", d)["status"], "ORIGINAL_ACTIVITY_PID_NOT_VERIFIED")
    def test_version_outside_15_16_rejected(self):
        with self.assertRaises(ValueError):
            mod.evaluate("14", evidence())
    def test_no_data_fails_closed(self):
        self.assertEqual(mod.evaluate("15", {})["status"], "VIRTUAL_ANDROID_BOOT_NOT_VERIFIED")

if __name__ == "__main__":
    unittest.main()
