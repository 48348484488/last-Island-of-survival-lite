import importlib.util
import unittest
from pathlib import Path
spec=importlib.util.spec_from_file_location(
    "checker",Path(__file__).resolve().parents[1]/"tools/audit_google_play_emulator.py")
checker=importlib.util.module_from_spec(spec)
spec.loader.exec_module(checker)

class GooglePlayEmulatorTests(unittest.TestCase):
    def test_arm64_and_real_package_presence_only_precondition(self):
        r=checker.summarize("x86_64,x86,arm64-v8a","libndk_translation.so",
          "package:com.android.vending\npackage:com.google.android.gms\npackage:com.google.android.gsf", "35","1")
        self.assertEqual(r["status"],"GOOGLE_PACKAGES_AND_ARM64_ABI_OBSERVED")
        self.assertEqual(r["play_integrity_verdict"],"NOT_MEASURED")
        self.assertFalse(r["game_installation_tested"])
    def test_google_image_without_arm64_does_not_claim_game_ready(self):
        r=checker.summarize("x86_64,x86","",
          "package:com.android.vending\npackage:com.google.android.gms\npackage:com.google.android.gsf","35","1")
        self.assertEqual(r["status"],"GOOGLE_PACKAGES_PRESENT_ARM64_ABI_MISSING")
    def test_prefix_packages_not_matching_real_google_packages(self):
        r=checker.summarize("arm64-v8a","",
          "package:com.google.android.gms.fake\npackage:com.android.vending","35","1")
        self.assertFalse(r["google_packages"]["play_services"])
        self.assertFalse(r["google_packages"]["google_services_framework"])
    def test_installed_google_does_not_imply_certification(self):
        r=checker.summarize("arm64-v8a","","package:com.google.android.gms\npackage:com.android.vending\npackage:com.google.android.gsf","35","1")
        self.assertEqual(r["play_integrity_verdict"],"NOT_MEASURED")
        self.assertEqual(r["play_account_signed_in"],"NOT_MEASURED")
        self.assertFalse(r["gameplay_tested"])
if __name__=="__main__":
    unittest.main()
