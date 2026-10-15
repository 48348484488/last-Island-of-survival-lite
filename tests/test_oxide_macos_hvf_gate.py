"""Static regression: macOS Hypervisor API must be actually invoked before APKs.

These tests assert safe infrastructure gating, never game or Android success.
"""
import subprocess
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
C_SOURCE = ROOT / "tools/oxide_mac_hvf_vm_create.c"
PLIST = ROOT / "tools/oxide_mac_hvf_entitlements.plist"
PROBE = ROOT / ".github/workflows/oxide-m3-macos-hvf-create-preflight.yml"
TRIAL = ROOT / ".github/workflows/oxide-m4-macos-arm64-original.yml"


class ActualHypervisorGateTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.c_code = C_SOURCE.read_text(encoding="utf8")
        cls.plist = PLIST.read_text(encoding="utf8")
        cls.probe = PROBE.read_text(encoding="utf8")
        cls.trial = TRIAL.read_text(encoding="utf8")

    def test_native_code_checks_real_vm_not_only_accel_listing(self):
        self.assertIn("hv_vm_create(NULL)", self.c_code)
        self.assertIn("hv_vm_destroy()", self.c_code)
        self.assertIn("HV_UNSUPPORTED", self.c_code)
        self.assertIn("HVF_VM_CREATE_SUCCESS_ANDROID_BOOT_UNTESTED", self.c_code)

    def test_prober_has_official_hypervisor_entitlement(self):
        self.assertIn("com.apple.security.hypervisor", self.plist)
        self.assertIn("-framework Hypervisor", self.probe)
        self.assertIn("--entitlements tools/oxide_mac_hvf_entitlements.plist", self.probe)
        self.assertIn("codesign --verify", self.probe)

    def test_probe_is_cost_bounded_no_apk_or_emulator(self):
        self.assertIn("runs-on: macos-15", self.probe)
        self.assertIn("timeout-minutes: 5", self.probe)
        self.assertIn("android_booted", self.probe)
        self.assertIn("gameplay_verified", self.probe)
        self.assertNotIn("reactivecircus/android-emulator-runner", self.probe)
        self.assertNotIn("gh release download", self.probe)
        self.assertNotIn("install-multiple", self.probe)

    def test_probe_distinguishes_execution_status_from_success(self):
        self.assertIn("BLOCKED_HVF_UNSUPPORTED", self.probe)
        self.assertIn("BLOCKED_HVF_VM_CREATE_UNVERIFIED", self.probe)
        self.assertIn("HOST_HVF_VM_CREATE_PASSED_ANDROID_UNTESTED", self.probe)

    def test_prober_source_exists_and_has_license_free_sdk_code(self):
        self.assertIn("#include <Hypervisor/Hypervisor.h>", self.c_code)
        self.assertNotIn("pm grant", self.c_code)
        self.assertNotIn("Play Integrity", self.c_code)


if __name__ == "__main__":
    unittest.main()
