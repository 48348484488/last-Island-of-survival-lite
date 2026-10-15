import unittest
from tools.oxide_host_preflight import classify


class TestHostPreflight(unittest.TestCase):
    def test_no_binder_prevents_expensive_android_attempt(self):
        x = classify({})
        self.assertEqual(x["next_action"], "HOST_NO_BINDER")
        self.assertFalse(x["binder_ready"])
        self.assertEqual(len(x["missing_binder_devices"]), 3)

    def test_partial_binder_is_not_sufficient(self):
        x = classify({"binder": True, "hwbinder": False, "vndbinder": True})
        self.assertEqual(x["missing_binder_devices"], ["hwbinder"])

    def test_binderfs_mount_alone_is_not_ready(self):
        x = classify({}, binderfs_available=True)
        self.assertFalse(x["binder_ready"])

    def test_full_binder_allows_android_boot_attempt_not_gameplay_claim(self):
        x = classify({"binder": True, "hwbinder": True, "vndbinder": True})
        self.assertEqual(x["next_action"], "BOOT_ANDROID")
        self.assertFalse(x["android_booted"])
        self.assertFalse(x["game_tested"])


if __name__ == "__main__":
    unittest.main()
