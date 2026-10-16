"""Fail-closed regressions for original Redroid graphics initialization evidence."""
import hashlib
from unittest.mock import patch
import tempfile
import unittest
from pathlib import Path

from tools import oxide_original_graphics_evidence as observed

PID = 2023
BADGING = b"\n".join((
    b"package: name='com.catsbit.oxidesurvivalisland' versionName='1.15.11966'",
    b"  uses-gl-es: '0x30002'",
    b"  uses-feature-not-required: name='android.hardware.vulkan.version'",
))

SAMPLE = "\n".join((
    "10-15 18:55:51.207 281 301 I ActivityTaskManager: START u0 {cmp=com.catsbit.oxidesurvivalisland/.OxideActivity}",
    "10-15 18:55:51.215 281 314 I ActivityManager: Start proc 2023:com.catsbit.oxidesurvivalisland/u0a87",
    "10-15 18:55:51.458 2023 2122 W libEGL: ANGLE Warn:vulkan_icd.cpp:315 (ChoosePhysicalDevice): Preferred device ICD not found. Using default physicalDevice instead.",
    "10-15 18:55:51.459 2023 2122 W SwiftShader: WARNING: UNSUPPORTED: curExtension->sType: 1000338000",
    "10-15 18:55:51.462 2023 2122 I Gralloc4: mapper 4.x is not supported",
    "10-15 18:55:51.463 2023 2122 E GraphicBufferAllocator: Failed to allocate (4 x 4) layerCount 1 format 43 usage b00: 3",
    "10-15 18:55:51.463 2023 2122 E GraphicBufferAllocator: Failed to allocate (4 x 4) layerCount 1 format 56 usage b00: 3",
    "10-15 18:55:51.463 2023 2122 E GraphicBufferAllocator: Failed to allocate (4 x 4) layerCount 1 format 59 usage b00: 3",
    "10-15 18:55:51.464 2023 2122 I ANGLE: Version (2.1 hash), Renderer (Vulkan 1.3.0 (SwiftShader Device (LLVM 16.0.0) (0x0000C0DE)))",
    "10-15 18:55:51.756 2023 2129 I Unity: Built from '6000.3/staging', Scripting Backend 'il2cpp'",
    "10-15 18:55:52.055 2023 2129 I Unity: Product Name: OXIDE",
    "10-15 18:55:55.687 2023 2023 I Process: Sending signal. PID: 2023 SIG: 9",
)).encode()


class GraphicsTimelineTests(unittest.TestCase):
    def read(self, data=SAMPLE, badging=BADGING, pid=PID):
        return observed.analyze_original(
            data, badging, pid, hashlib.sha256(data).hexdigest())

    def test_original_like_angle_renderer_followed_by_unity(self):
        r = self.read()
        self.assertTrue(r["validated"])
        self.assertEqual(r["graphics_buffer_probe_formats_failed"], [43, 56, 59])
        self.assertEqual(r["original_manifest_gles_requirement"], "OpenGL ES 3.2")
        self.assertEqual(r["original_manifest_vulkan_feature"], "NOT_REQUIRED")
        self.assertTrue(r["unity_il2cpp_reported_after_graphics_warnings"])
        self.assertFalse(r["graphics_warnings_immediately_blocked_unity_identification"])
        self.assertFalse(r["original_gameplay_verified"])
        self.assertEqual(r["security_trigger"], "UNKNOWN")

    def test_wrong_pid_is_rejected(self):
        self.assertFalse(self.read(pid=9999)["validated"])

    def test_wrong_thread_for_angle_renderer_is_rejected(self):
        data = SAMPLE.replace(b"2023 2122 I ANGLE:", b"2023 9999 I ANGLE:")
        self.assertFalse(self.read(data)["validated"])

    def test_unity_before_graphics_warning_is_rejected(self):
        data = SAMPLE.replace(b"18:55:51.756 2023 2129 I Unity:",
                              b"18:55:51.457 2023 2129 I Unity:")
        self.assertFalse(self.read(data)["validated"])

    def test_no_angle_renderer_report_is_not_full_success(self):
        data = SAMPLE.replace(b"Renderer (Vulkan 1.3.0 (SwiftShader Device", b"Renderer (unknown")
        self.assertFalse(self.read(data)["validated"])

    def test_missing_buffer_format_not_success(self):
        data = SAMPLE.replace(b"format 59 usage b00", b"format 77 usage b00")
        self.assertFalse(self.read(data)["validated"])

    def test_mismatched_original_log_hash_fails_closed(self):
        r = observed.analyze_original(SAMPLE, BADGING, PID, "f" * 64)
        self.assertEqual(r["status"], "BLOCKED_ORIGINAL_LOGCAT_SHA_MISMATCH")

    def test_required_manifest_feature_changes_gate(self):
        badging = BADGING.replace(
            b"uses-feature-not-required: name='android.hardware.vulkan.version'",
            b"uses-feature: name='android.hardware.vulkan.version'")
        self.assertEqual(self.read(badging=badging)["status"],
                         "BLOCKED_ORIGINAL_GRAPHICS_MANIFEST_MISMATCH")

    def test_known_original_image_annotation_requires_exact_sha(self):
        mock_png = b"\x89PNG\r\n\x1a\n" + b"placeholder test data"
        digest = hashlib.sha256(mock_png).hexdigest()
        with patch.dict(observed.REVIEWED_IMMERSIVE_DIALOG_PNG_SHA256,
                        {"screen-2.png": digest}):
            good = observed.review_exact_screen(mock_png, "screen-2.png")
            self.assertEqual(good["status"],
                             "EXACT_ORIGINAL_SCREEN_REVIEWED_ANDROID_IMMERSIVE_DIALOG")
            self.assertFalse(good["gameplay_evidence"])
            bad = observed.review_exact_screen(mock_png + b"x", "screen-2.png")
            self.assertEqual(bad["status"], "NOT_VISUALLY_REVIEWED")
            wrong_name = observed.review_exact_screen(mock_png, "screen-10.png")
            self.assertEqual(wrong_name["status"], "NOT_VISUALLY_REVIEWED")

    def test_combined_report_requires_reviewed_screens(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            for name in ("android15", "android16"):
                d = root / name
                d.mkdir()
                (d / "logcat.txt").write_bytes(SAMPLE)
                (d / "apk-badging.txt").write_bytes(BADGING)
            with patch.dict(observed.SHA256_LOGCAT,
                            {"android15": hashlib.sha256(SAMPLE).hexdigest(),
                             "android16": hashlib.sha256(SAMPLE).hexdigest()}), \
                 patch.dict(observed.EXPECTED_PIDS,
                            {"android15": 2023, "android16": 2023}):
                result = observed.analyze_two_existing_reports(root / "android15", root / "android16")
                self.assertEqual(result["status"],
                                 "BLOCKED_MISSING_MANUALLY_REVIEWED_SCREENSHOT")
                self.assertFalse(result["validated"])


if __name__ == "__main__":
    unittest.main()
