"""Prevent raylib's editor/debug grid from leaking into Android gameplay."""
import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MAIN = ROOT / "last_island_recreated_game" / "main.cpp"


class WorldRenderingRegression(unittest.TestCase):
    def test_render_loop_heartbeat_is_not_a_screenshot_pass(self):
        source = MAIN.read_text(encoding="utf-8")
        self.assertIn("LIOS_RENDER_HEARTBEAT frame=%u", source)
        self.assertIn("completedFrames % 60 == 0", source)
        self.assertIn("LIOS_FIRST_FRAME_ENDDRAWING", source)

    def test_release_scene_has_no_debug_grid(self):
        source = MAIN.read_text(encoding="utf-8")
        self.assertIn("BeginMode3D(camera)", source)
        self.assertRegex(source, r"DrawPlane\s*\(")
        self.assertIsNone(re.search(r"\bDrawGrid\s*\(", source),
                          "Debug grid must not be drawn over the gameplay terrain")


if __name__ == "__main__":
    unittest.main()
