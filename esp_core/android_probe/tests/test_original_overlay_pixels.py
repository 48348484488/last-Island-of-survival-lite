import importlib.util
import json
import tempfile
import unittest
from pathlib import Path
from PIL import Image, ImageDraw

spec = importlib.util.spec_from_file_location(
    "overlay_pixels",
    Path(__file__).resolve().parents[1] / "check_original_overlay_pixels.py",
)
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)


class OriginalOverlayPixelTests(unittest.TestCase):
    def populate(self, folder, green=(7, 47, 20), visible=True,
                 foreground=True, service=True):
        root = Path(folder)
        # The native Redroid screenshot changes from portrait to landscape.
        launcher = Image.new("RGB", (720, 1280), (0, 0, 0))
        ImageDraw.Draw(launcher).rectangle([300, 472, 419, 711],
                                           outline=(33, 195, 88), width=4)
        launcher.save(root / "overlay_on_launcher.png")
        game = Image.new("RGB", (1280, 720), (0, 0, 0))
        if visible:
            ImageDraw.Draw(game).rectangle([535, 287, 744, 432],
                                            outline=green, width=4)
        game.save(root / "overlay_on_game_1.png")
        # Android's system fullscreen tutorial can completely cover the marker.
        Image.new("RGB", (1280, 720), (90, 106, 151)).save(
            root / "overlay_on_game_2.png")
        Image.new("RGB", (1280, 720), (0, 0, 0)).save(
            root / "overlay_removed.png")
        (root / "original_foreground.txt").write_text(
            "ORIGINAL_ACTIVITY_RESUMED_DURING_CAPTURE=" +
            ("OBSERVED" if foreground else "NOT_CONFIRMED"))
        (root / "overlay_service.txt").write_text(
            "DIAGNOSTIC_OVERLAY_SERVICE_START=" +
            ("PASS" if service else "FAIL"))

    def test_native_dimmed_rectangle_is_detected(self):
        with tempfile.TemporaryDirectory() as tmp:
            self.populate(tmp)
            r = mod.evaluate(tmp)
            self.assertEqual(r["status"], "ORIGINAL_LOADING_OVERLAY_PASS")
            self.assertTrue(r["captures"]["overlay_on_game_1.png"]["pixels"] > 500)
            self.assertEqual(r["captures"]["overlay_on_game_2.png"]["pixels"], 0)
            self.assertTrue(r["overlay_disappeared_after_stop"])
            self.assertFalse(r["real_players_verified"])

    def test_black_game_capture_is_not_accepted(self):
        with tempfile.TemporaryDirectory() as tmp:
            self.populate(tmp, visible=False)
            self.assertEqual(mod.evaluate(tmp)["status"],
                             "ORIGINAL_LOADING_OVERLAY_NOT_CONFIRMED")

    def test_legacy_bright_green_also_passes(self):
        with tempfile.TemporaryDirectory() as tmp:
            self.populate(tmp, green=(33, 195, 88))
            self.assertEqual(mod.evaluate(tmp)["status"],
                             "ORIGINAL_LOADING_OVERLAY_PASS")

    def test_foreground_or_service_must_be_proven(self):
        for kw in ({"foreground": False}, {"service": False}):
            with self.subTest(kw=kw), tempfile.TemporaryDirectory() as tmp:
                self.populate(tmp, **kw)
                self.assertEqual(mod.evaluate(tmp)["status"],
                                 "ORIGINAL_LOADING_OVERLAY_NOT_CONFIRMED")

    def test_optional_third_capture(self):
        with tempfile.TemporaryDirectory() as tmp:
            self.populate(tmp, visible=False)
            game = Image.new("RGB", (1280, 720), (12, 16, 23))
            ImageDraw.Draw(game).rectangle([535, 287, 744, 432],
                                            outline=(33, 195, 88), width=4)
            game.save(Path(tmp) / "overlay_on_game_3.png")
            result = mod.evaluate(tmp)
            self.assertEqual(result["status"], "ORIGINAL_LOADING_OVERLAY_PASS")
            self.assertIn("overlay_on_game_3.png",
                          result["game_screens_with_test_marker"])


if __name__ == "__main__":
    unittest.main()
