"""Keyboard gameplay shortcuts must not bypass an active HUD modal.

Source-level regression only. Android runtime still validates the actual
touch/UI transitions; this test does not claim original-game parity.
"""
import re
import unittest
from pathlib import Path

SOURCE = (Path(__file__).resolve().parents[1] /
          "last_island_recreated_game" / "main.cpp")


class KeyboardModalInputTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.code = SOURCE.read_text(encoding="utf-8")

    def test_modal_snapshot_precedes_keyboard_processing(self):
        self.assertEqual(self.code.count("const bool overlayWasOpen=activePanel!=0;"), 1)
        self.assertLess(self.code.index("const bool overlayWasOpen=activePanel!=0;"),
                        self.code.index("IsKeyPressed(KEY_TAB)"))

    def test_camera_shortcuts_respect_overlay(self):
        self.assertRegex(
            self.code,
            r"if \(!overlayWasOpen\s*&&\s*"
            r"\(IsKeyPressed\(KEY_TAB\)\s*\|\|\s*"
            r"\(!buildMode&&IsKeyPressed\(KEY_V\)\)\)\)")

    def test_all_seven_hotbar_keys_respect_overlay(self):
        for key in ("ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN"):
            with self.subTest(key=key):
                self.assertIn(
                    f"if (!overlayWasOpen&&IsKeyPressed(KEY_{key}))", self.code)
                self.assertNotIn(f"if (IsKeyPressed(KEY_{key}))", self.code)

    def test_opening_touch_is_not_replayed_into_new_modal(self):
        self.assertIn("const int panelAtFrameStart=activePanel;", self.code)
        self.assertIn("if(activePanel!=panelAtFrameStart) uiClicks.clear();", self.code)
        self.assertLess(
            self.code.index("if(activePanel!=panelAtFrameStart) uiClicks.clear();"),
            self.code.index("for(const Vector2 &click:uiClicks)"))
        self.assertIn("LIOS_INVENTORY_PANEL_OPEN", self.code)
        self.assertIn("LIOS_CRAFT_PANEL_OPEN", self.code)

    def test_builder_shortcut_keeps_modal_guard(self):
        self.assertRegex(
            self.code,
            r"if\(!overlayWasOpen&&building\.blueprint&&\s*"
            r"\(IsKeyPressed\(KEY_B\)\|\|hudInput\.justPressed\(HA::Slot6\)\)\)")


if __name__ == "__main__":
    unittest.main()
