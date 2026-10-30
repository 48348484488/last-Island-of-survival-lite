"""Regression tests for Android screenshot readiness; no game screenshots are fabricated."""
import importlib.util
import tempfile
import unittest
from pathlib import Path
from PIL import Image

MODULE = Path(__file__).resolve().parents[1] / "tools" / "android_frame_quality.py"
spec = importlib.util.spec_from_file_location("android_frame_quality", MODULE)
gate = importlib.util.module_from_spec(spec)
spec.loader.exec_module(gate)


class FrameQualityTests(unittest.TestCase):
    def test_flat_green_frame_is_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "flat.png"
            Image.new("RGB", (1080, 640), (68, 115, 52)).save(path)
            result = gate.inspect_frame(path)
            self.assertFalse(result["nonflat_frame"])
            self.assertEqual(result["distinct_rgb_colors_at_least"], 1)

    def test_multicolor_game_frame_passes_heuristic(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "frame.png"
            image = Image.new("RGB", (640, 360))
            for x in range(32):
                for y in range(32):
                    image.putpixel((x, y), (x * 7, y * 5, (x + y) * 3))
            image.save(path)
            result = gate.inspect_frame(path)
            self.assertTrue(result["nonflat_frame"])
            self.assertTrue(result["color_count_is_lower_bound"])
            self.assertFalse(result["gameplay_or_visual_parity_proven"])

    def test_small_or_corrupt_capture_is_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "frame.png"
            Image.new("RGB", (20, 20)).save(path)
            with self.assertRaisesRegex(ValueError, "resolution"):
                gate.inspect_frame(path)
            path.write_bytes(b"not a screenshot")
            with self.assertRaises(Exception):
                gate.inspect_frame(path)


    def test_identical_screenshots_do_not_prove_transition(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360), (68, 115, 52)).save(before)
            Image.new("RGBA", (640, 360), (68, 115, 52, 255)).save(after)
            result = gate.inspect_transition(before, after)
            self.assertFalse(result["transition_detected"])
            self.assertEqual(result["changed_pixels"], 0)
            self.assertIsNone(result["difference_bounds"])

    def test_changed_panel_region_detected(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360), (68, 115, 52)).save(before)
            frame = Image.new("RGB", (640, 360), (68, 115, 52))
            frame.paste((200, 210, 220), (100, 60, 300, 260))
            frame.save(after)
            result = gate.inspect_transition(before, after, min_changed_fraction=0.1)
            self.assertTrue(result["transition_detected"])
            self.assertEqual(result["changed_pixels"], 40000)
            self.assertEqual(result["difference_bounds"], [100, 60, 300, 260])
            self.assertFalse(result["action_or_original_game_parity_proven"])

    def test_small_change_rejected_at_threshold(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360), (0, 0, 0)).save(before)
            frame = Image.new("RGB", (640, 360), (0, 0, 0))
            frame.paste((255, 0, 0), (0, 0, 10, 10))
            frame.save(after)
            self.assertFalse(gate.inspect_transition(before, after)["transition_detected"])
            self.assertEqual(gate.inspect_transition(before, after, min_changed_fraction=0.0001)["changed_pixels"], 100)

    def test_resolution_and_invalid_threshold_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360)).save(before)
            Image.new("RGB", (320, 240)).save(after)
            with self.assertRaisesRegex(ValueError, "resolutions differ"):
                gate.inspect_transition(before, after)
            with self.assertRaisesRegex(ValueError, "min_changed_fraction"):
                gate.inspect_transition(before, after, float("nan"))
            with self.assertRaisesRegex(ValueError, "min_channel_delta"):
                gate.inspect_transition(before, after, 0.01, 0)


    def test_flat_frame_failure_collects_render_loop_diagnostics(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(encoding="utf-8")
        self.assertIn("LIOS_FLAT_FRAME_WITH_ACTIVE_RENDER_LOOP", smoke)
        self.assertIn("LIOS_FLAT_FRAME_RENDER_LOOP_NOT_CONFIRMED", smoke)
        self.assertIn("evidence/frame-heartbeats.txt", smoke)
        self.assertIn("exit 47", smoke)

    def test_android_smoke_requires_fresh_touch_event_and_visual_change(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(encoding="utf-8")
        self.assertIn("adb logcat -c", smoke)
        self.assertIn("--compare-with evidence/runtime.png", smoke)
        self.assertIn("LIOS_CRAFT_LOGGED_BUT_VISUAL_TRANSITION_UNCONFIRMED", smoke)
        self.assertIn("LIOS_VISIBLE_CRAFT_BUTTON_FUNCTIONAL_FAILURE", smoke)


if __name__ == "__main__":
    unittest.main()
