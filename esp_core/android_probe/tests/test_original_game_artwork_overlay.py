import importlib.util
import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

module_path = Path(__file__).resolve().parents[1] / "check_original_game_artwork_overlay.py"
spec = importlib.util.spec_from_file_location("artwork_overlay", module_path)
import sys
sys.path.insert(0, str(module_path.parent))
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)


def artwork(marker=True, blank=False, tutorial=False):
    image = Image.new("RGB", (1280, 720), (0, 0, 0))
    draw = ImageDraw.Draw(image)
    if not blank:
        for y in range(0, 720, 16):
            for x in range(0, 1280, 32):
                draw.rectangle((x, y, x+31, y+15), fill=(
                    20+(x//32*5+y//16*3)%110,
                    30+(x//32*8+y//16*11)%140,
                    45+(x//32*6+y//16*7)%125
                ))
    if marker:
        draw.rectangle((535, 287, 744, 432), outline=(38, 255, 110), width=5)
    if tutorial:
        draw.rectangle((140, 0, 1140, 475), fill=(237, 237, 243))
    return image


class RealLoadingArtworkOverlap(unittest.TestCase):
    def evidence(self, blank=False, tutorial=False, marker=True, sample=True,
                 foreground=True, cleared=True):
        tmp = tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        root = Path(tmp.name)
        artwork(marker=True, blank=True).save(root / "overlay_on_launcher.png")
        artwork(marker=False, blank=True).save(root / "overlay_on_game_1.png")
        artwork(marker=False, blank=True).save(root / "overlay_on_game_2.png")
        artwork(marker=marker, blank=blank, tutorial=tutorial).save(
            root / "overlay_on_game_3.png")
        artwork(marker=not cleared, blank=True).save(root / "overlay_removed.png")
        (root / "original_foreground.txt").write_text(
            "ORIGINAL_ACTIVITY_RESUMED_DURING_CAPTURE=" +
            ("OBSERVED" if foreground else "NOT_CONFIRMED"))
        (root / "overlay_service.txt").write_text(
            "DIAGNOSTIC_OVERLAY_SERVICE_START=PASS")
        (root / "original_overlay_overlap_verdict.json").write_text(
            json.dumps({"capture_samples": [{
                "capture": "overlay_on_game_3.png",
                "sample_correlates": sample,
            }]}))
        return root

    def test_visible_original_artwork_and_standalone_marker(self):
        d = mod.assess(self.evidence())
        self.assertEqual(d["status"], "ORIGINAL_LOADING_ARTWORK_WITH_SYNTHETIC_OVERLAY_VISIBLE")
        self.assertTrue(all(d["checks"].values()))
        self.assertFalse(d["real_player_coordinates_proven"])

    def test_black_pre_render_is_not_original_artwork(self):
        d = mod.assess(self.evidence(blank=True))
        self.assertFalse(d["checks"]["nonblack_game_loading_background_visible"])
        self.assertNotEqual(d["status"], "ORIGINAL_LOADING_ARTWORK_WITH_SYNTHETIC_OVERLAY_VISIBLE")

    def test_system_tutorial_still_obscuring_central_marker(self):
        d = mod.assess(self.evidence(tutorial=True))
        self.assertFalse(d["checks"]["android_fullscreen_tutorial_not_obscuring_rectangle"])

    def test_marker_absent_from_game_artwork_rejected(self):
        d = mod.assess(self.evidence(marker=False))
        self.assertFalse(d["checks"]["green_jni_rectangle_in_actual_game_artwork_png"])

    def test_original_pid_must_correlate_with_final_capture(self):
        d = mod.assess(self.evidence(sample=False))
        self.assertFalse(d["checks"]["original_activity_pid_and_overlay_matched_capture3"])

    def test_service_stop_must_remove_marker(self):
        d = mod.assess(self.evidence(cleared=False))
        self.assertFalse(d["checks"]["marker_removed_when_standalone_service_stopped"])

    def test_original_activity_must_have_foreground_evidence(self):
        d = mod.assess(self.evidence(foreground=False))
        self.assertFalse(d["checks"]["original_loading_activity_resumed"])


if __name__ == "__main__":
    unittest.main()
