import importlib.util
import unittest
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    "permission_doctor", Path(__file__).resolve().parents[1] / "tools" / "diagnose_prerequisites.py")
doctor = importlib.util.module_from_spec(spec)
spec.loader.exec_module(doctor)

class PrerequisiteTests(unittest.TestCase):
    def test_declarations_not_grant_proof(self):
        s = "uses-permission: name='android.permission.INTERNET'\nuses-permission: name='android.permission.CAMERA'"
        r = doctor.examine_permissions(s)
        self.assertEqual(r["requested_permissions_count"], 2)
        self.assertTrue(all(x["reported_granted"] is None for x in r["requested_permissions"]))
        self.assertEqual(r["security_rejection_cause"], "NOT_IDENTIFIED")
    def test_dumpsys_denied_is_observed(self):
        r = doctor.examine_permissions("uses-permission: name='android.permission.RECORD_AUDIO'",
                                      "android.permission.RECORD_AUDIO: granted=false, flags=[ USER_SENSITIVE ]")
        self.assertEqual(r["requested_permissions"][0]["meaning"], "NOT_GRANTED_BY_ANDROID")
    def test_google_warning_not_claimed_security_cause(self):
        r = doctor.examine_permissions("uses-permission: name='android.permission.INTERNET'",
                                       "", "Google Play services is missing.")
        self.assertTrue(r["gms_log_markers"]["play_services_missing"])
        self.assertEqual(r["security_rejection_cause"], "NOT_IDENTIFIED")
    def test_duplicate_permissions_dedup(self):
        t = "uses-permission: name='android.permission.INTERNET'\n" * 2
        self.assertEqual(doctor.examine_permissions(t)["requested_permissions_count"], 1)

if __name__ == "__main__":
    unittest.main()
