import importlib.util
import unittest
from pathlib import Path
spec=importlib.util.spec_from_file_location(
    "crash", Path(__file__).resolve().parents[1]/"tools/classify_original_native_crash.py")
crash=importlib.util.module_from_spec(spec)
spec.loader.exec_module(crash)

TOMBSTONE="""10-13 22:51:45.825 4078 4078 F libc : Fatal signal 4 (SIGILL), code -6 (SI_TKILL)
10-13 22:51:47.848 4104 4104 F DEBUG : Guest architecture: 'arm64'
10-13 22:51:47.854 4104 4104 F DEBUG : #00 pc 0000000000830bec /data/app/lib/arm64/libloader.so
10-13 22:51:47.854 4104 4104 F DEBUG : #01 pc 0000000000830d98 /data/app/lib/arm64/libloader.so
10-13 22:51:47.854 4104 4104 F DEBUG : #02 pc 000000000008b144 /system/bin/arm64/linker64 (call_constructors)
"""
EXIT="ApplicationExitInfo #0: reason=5 (APP CRASH(NATIVE)) status=4"
FORENSIC={"initial_activity_pid":4078, "first_self_sigkill":None}

class NativeCrashTests(unittest.TestCase):
    def test_native_bridge_signal_and_loader_frames_without_mislabeling(self):
        r=crash.classify(TOMBSTONE,EXIT,"",FORENSIC)
        self.assertEqual(r["status"],"NATIVE_SIGILL_SI_TKILL_DURING_LIBLOADER_LINKER_CONSTRUCTORS")
        self.assertEqual(r["guest_architecture"],"arm64")
        self.assertEqual(r["library_frames"][0]["pc"],"0x830bec")
        self.assertEqual(r["signal_code"],"SI_TKILL")
        self.assertFalse(r["unity_il2cpp_start_logged_for_game_pid"])
        self.assertEqual(r["specific_security_response_cause"],"NOT_DETERMINED")
        self.assertFalse(r["esp_verified"])
    def test_non_game_unity_log_does_not_count(self):
        log="10-13 22:51:44.000 999 999 I Unity : Context Type: ActivityOrService"
        r=crash.classify(TOMBSTONE,EXIT,log,FORENSIC)
        self.assertFalse(r["unity_il2cpp_start_logged_for_game_pid"])
    def test_no_tombstone_never_called_native_crash(self):
        r=crash.classify("", "", "",{"initial_activity_pid":1234,
              "first_self_sigkill":"10-13 22:00:00.000"})
        self.assertEqual(r["status"],"SELF_SIGKILL_OBSERVED_NO_NATIVE_TOMBSTONE")
    def test_other_signal_not_claimed_loader_signal(self):
        r=crash.classify(TOMBSTONE.replace("SI_TKILL","ILL_ILLOPC"),EXIT,"",FORENSIC)
        self.assertNotEqual(r["status"],"NATIVE_SIGILL_SI_TKILL_DURING_LIBLOADER_LINKER_CONSTRUCTORS")
if __name__=="__main__":
    unittest.main()
