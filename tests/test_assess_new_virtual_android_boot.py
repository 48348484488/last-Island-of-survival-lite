import importlib.util
import unittest
from pathlib import Path

path = Path(__file__).resolve().parents[1] / "tools" / "assess_new_virtual_android_boot.py"
spec = importlib.util.spec_from_file_location("boot_assessor", path)
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

def valid(version="15"):
    return {
        "host_arch": "aarch64", "container_arch": "arm64",
        "guest_abi": "arm64-v8a", "android_release": version,
        "boot_completed": "1", "rendering_mode": "guest",
        "adb_shell": "PASS", "activity_manager": "PASS",
        "google_play_services": "NOT_FOUND",
        "surfaceflinger_gles": "UNKNOWN",
    }

class NativeBootValidation(unittest.TestCase):
    def test_verified_android15(self):
        result = mod.assess("15", valid())
        self.assertEqual(result["boot_verdict"], "NATIVE_ARM64_ANDROID_BOOT_VERIFIED")
        self.assertFalse(result["original_game_launched"])
        self.assertEqual(result["play_integrity_verdict"], "NOT_ATTESTED")
    def test_verified_android16(self):
        self.assertEqual(mod.assess("16", valid("16"))["boot_verdict"], "NATIVE_ARM64_ANDROID_BOOT_VERIFIED")
    def test_wrong_version_rejected(self):
        self.assertEqual(mod.assess("16", valid("15"))["boot_verdict"], "BOOT_UNVERIFIED")
    def test_native_architecture_not_emulated_x86(self):
        sample = valid()
        sample["host_arch"] = "x86_64"
        self.assertEqual(mod.assess("15", sample)["boot_verdict"], "BOOT_UNVERIFIED")
    def test_no_adb_means_not_booted(self):
        sample = valid()
        sample["adb_shell"] = "FAIL"
        self.assertEqual(mod.assess("15", sample)["boot_verdict"], "BOOT_UNVERIFIED")
    def test_boot_property_required(self):
        sample = valid()
        sample["boot_completed"] = "0"
        self.assertEqual(mod.assess("15", sample)["boot_verdict"], "BOOT_UNVERIFIED")
    def test_activity_manager_required(self):
        sample = valid()
        sample["activity_manager"] = "FAIL"
        self.assertEqual(mod.assess("15", sample)["boot_verdict"], "BOOT_UNVERIFIED")
    def test_image_must_be_arm64(self):
        sample = valid()
        sample["container_arch"] = "amd64"
        self.assertEqual(mod.assess("15", sample)["boot_verdict"], "BOOT_UNVERIFIED")
    def test_invalid_version_refused(self):
        with self.assertRaises(ValueError):
            mod.assess("14", valid("14"))
    def test_google_absence_not_game_cause(self):
        a = mod.assess("15", valid())
        self.assertTrue(a["verified_checks"]["android_boot_completed"])
        self.assertFalse(a["original_gameplay_verified"])
        self.assertNotIn("ACCEPTED", a["play_integrity_verdict"])
    def test_gles_missing_not_assumed_graphic_failure(self):
        a = mod.assess("15", valid())
        self.assertEqual(a["boot_verdict"], "NATIVE_ARM64_ANDROID_BOOT_VERIFIED")
        self.assertEqual(a["gles_observed"], "UNKNOWN")

if __name__ == "__main__":
    unittest.main()
