import importlib.util
import unittest
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    "runtime_doctor", Path(__file__).resolve().parents[1] / "tools" / "diagnose_android_runtime.py"
)
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)

class RuntimePrerequisiteTests(unittest.TestCase):
    AAPT = """package: com.catsbit.oxidesurvivalisland
uses-permission: name='android.permission.INTERNET'
uses-permission: name='android.permission.RECORD_AUDIO'
uses-permission: name='android.permission.CAMERA'
uses-permission: name='android.permission.INTERNET'
"""

    def test_permission_flags_are_not_equivalent_to_gms(self):
        dumpsys = """requested permissions:
    android.permission.INTERNET
runtime permissions:
    android.permission.RECORD_AUDIO: granted=false, flags=[USER_SET]
    android.permission.CAMERA: granted=true, flags=[USER_SET]
install permissions:
    android.permission.INTERNET: granted=true
"""
        r = module.summarize(self.AAPT, dumpsys, "RECORD_AUDIO: ignore", "",
                             "E FirebaseInstanceId: Google Play services missing or without correct permission.",
                             {"classification":"ORIGINAL_ACTIVITY_SELF_SIGKILL","initial_activity_pid":1234})
        self.assertEqual(r["declared_permission_count"], 3)
        self.assertEqual(r["measured_permission_count"], 3)
        self.assertEqual(r["declared_denied_permissions"], ["android.permission.RECORD_AUDIO"])
        self.assertEqual(r["play_services_package_presence"], "NOT_OBSERVED")
        self.assertEqual(r["game_termination"], "ORIGINAL_ACTIVITY_SELF_SIGKILL")
        self.assertEqual(r["proven_security_rejection_cause"], "UNKNOWN")

    def test_does_not_make_attestation_claim_from_google_package(self):
        r = module.summarize(self.AAPT, "", "", "package:com.google.android.gms",
                             "W GooglePlayServicesUtil: Google Play services is missing.")
        self.assertEqual(r["play_services_package_presence"], "PRESENT")
        self.assertEqual(r["device_integrity"], "NOT_ATTESTED")
        self.assertEqual(r["measured_permission_count"], 0)
        self.assertEqual(r["runtime_log_signal_counts"]["play_services_missing"], 1)

    def test_does_not_interpret_other_package_or_graphics_as_game_security(self):
        r = module.summarize(self.AAPT, "", "", "package:com.google.android.gms.rogue",
                             "E GraphicBufferAllocator: Failed to allocate (4 x 4)\n"
                             "E Sensors : Failed to get Sensor list\n"
                             "I Process : Sending signal. PID: 99 SIG: 9")
        self.assertEqual(r["play_services_package_presence"], "NOT_FOUND")
        self.assertEqual(r["runtime_log_signal_counts"]["sensor_enumeration_error"], 1)
        self.assertEqual(r["runtime_log_signal_counts"]["graphic_buffer_allocation_error"], 1)
        self.assertEqual(r["proven_security_rejection_cause"], "UNKNOWN")
        self.assertEqual(r["game_termination"], "NOT_CLASSIFIED")

    def test_empty_aapt_fails_measurement_not_claimed_granted(self):
        r = module.summarize("")
        self.assertEqual(r["declared_permission_count"], 0)
        self.assertEqual(r["measured_permission_count"], 0)
        self.assertEqual(r["device_integrity"], "NOT_ATTESTED")

if __name__ == "__main__":
    unittest.main()
