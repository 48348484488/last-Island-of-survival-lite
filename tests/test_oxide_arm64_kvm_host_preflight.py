"""Synthetic, no-hardware KVM host tests. Never runs Android or game code."""
import errno
import os
import stat
import unittest
from types import SimpleNamespace

from tools.oxide_arm64_kvm_host_preflight import evaluate, inspect_kvm


def character_device():
    return SimpleNamespace(st_mode=stat.S_IFCHR | 0o660)


class Arm64KvmHostPreflightTests(unittest.TestCase):
    def test_arm64_linux_with_kvm_api_12_is_only_a_prerequisite(self):
        closed = []
        probe = inspect_kvm(
            stat_fn=lambda path: character_device(),
            open_fn=lambda path, flags: 123,
            ioctl_fn=lambda fd, request, arg: 12,
            close_fn=closed.append,
        )
        self.assertEqual(probe, {"status": "KVM_API_12_ACCESSIBLE", "api_version": 12})
        self.assertEqual(closed, [123])
        report = evaluate("Linux", "aarch64", probe, "ubuntu26")
        self.assertEqual(report["status"], "ARM64_HOST_KVM_PREREQUISITE_MET")
        self.assertTrue(report["native_arm64_kvm_prerequisite_verified"])
        self.assertFalse(report["arm64_android_system_image_verified"])
        self.assertFalse(report["official_google_play_arm64_image_verified"])
        self.assertFalse(report["heavy_android_retry_authorized"])
        self.assertFalse(report["android_or_oxide_executed"])
        self.assertFalse(report["original_apk_modified"])
        self.assertFalse(report["gameplay_verified"])

    def test_missing_device_is_blocked_not_accidental_success(self):
        def missing(path):
            raise FileNotFoundError(path)
        kvm = inspect_kvm(stat_fn=missing)
        self.assertEqual(kvm["status"], "DEVICE_ABSENT")
        self.assertEqual(evaluate("Linux", "aarch64", kvm, "ubuntu26")["status"],
                         "ARM64_HOST_KVM_NOT_AVAILABLE_FOR_ACCELERATED_AVD")

    def test_char_device_required(self):
        kvm = inspect_kvm(stat_fn=lambda path: SimpleNamespace(st_mode=stat.S_IFREG))
        self.assertEqual(kvm["status"], "NOT_CHARACTER_DEVICE")

    def test_stat_permission_denied_is_not_ready(self):
        def denied(path):
            raise PermissionError(path)
        self.assertEqual(inspect_kvm(stat_fn=denied)["status"],
                         "DEVICE_STAT_PERMISSION_DENIED")

    def test_open_permission_denied_is_not_ready(self):
        def denied(path, flags):
            raise PermissionError(path)
        kvm = inspect_kvm(stat_fn=lambda path: character_device(), open_fn=denied)
        self.assertEqual(kvm["status"], "DEVICE_OPEN_PERMISSION_DENIED")

    def test_kvm_ioctl_failure_still_closes_fd(self):
        closed = []
        def failed_ioctl(fd, request, argument):
            raise OSError(errno.ENOTTY, "unsupported")
        kvm = inspect_kvm(
            stat_fn=lambda path: character_device(), open_fn=lambda p, f: 456,
            ioctl_fn=failed_ioctl, close_fn=closed.append)
        self.assertEqual(kvm["status"], "KVM_IOCTL_UNAVAILABLE")
        self.assertEqual(closed, [456])

    def test_wrong_kvm_api_version_is_rejected(self):
        kvm = inspect_kvm(
            stat_fn=lambda path: character_device(), open_fn=lambda p, f: 1,
            ioctl_fn=lambda fd, request, arg: 11, close_fn=lambda fd: None)
        self.assertEqual(kvm["status"], "KVM_API_NOT_12")
        self.assertEqual(kvm["api_version"], 11)

    def test_dangling_dev_kvm_is_rejected(self):
        def unavailable(path, flags):
            raise OSError(errno.ENODEV, "gone")
        kvm = inspect_kvm(stat_fn=lambda path: character_device(), open_fn=unavailable)
        self.assertEqual(kvm["status"], "DEVICE_OPEN_UNAVAILABLE")

    def test_x86_host_is_not_native_arm64_even_with_kvm(self):
        r = evaluate("Linux", "x86_64",
                     {"status": "KVM_API_12_ACCESSIBLE", "api_version": 12},
                     "ubuntu26")
        self.assertEqual(r["status"], "HOST_NOT_ARM64")
        self.assertFalse(r["native_arm64_kvm_prerequisite_verified"])

    def test_mac_arm64_host_is_not_linux_kvm_ready(self):
        r = evaluate("Darwin", "arm64",
                     {"status": "KVM_API_12_ACCESSIBLE", "api_version": 12},
                     "macos26")
        self.assertEqual(r["status"], "HOST_NOT_LINUX")
        self.assertFalse(r["native_arm64_kvm_prerequisite_verified"])

    def test_arm_machine_spelling_is_accepted(self):
        r = evaluate("Linux", "arm64",
                     {"status": "KVM_API_12_ACCESSIBLE", "api_version": 12},
                     "ubuntu26")
        self.assertTrue(r["native_arm64_kvm_prerequisite_verified"])


if __name__ == "__main__":
    unittest.main()
