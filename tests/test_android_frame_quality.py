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


if __name__ == "__main__":
    unittest.main()
