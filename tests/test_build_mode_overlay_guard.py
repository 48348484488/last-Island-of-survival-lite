"""Protect modal input ownership from the keyboard build-mode shortcut.

This is a source-level regression guard; full interaction still requires the
Android emulator and an appropriate game-state setup.
"""
import unittest
from pathlib import Path


class BuildModeModalGuardTests(unittest.TestCase):
    def test_keyboard_and_hud_build_shortcuts_share_overlay_gate(self):
        source = (Path(__file__).resolve().parents[1] /
                  "last_island_recreated_game" / "main.cpp").read_text(
                      encoding="utf-8")
        self.assertIn(
            "if(!overlayWasOpen&&building.blueprint&&\n"
            "            (IsKeyPressed(KEY_B)||hudInput.justPressed(HA::Slot6)))",
            source)
        self.assertNotIn(
            "if((IsKeyPressed(KEY_B)||\n"
            "            (!overlayWasOpen&&building.blueprint&&hudInput.justPressed(HA::Slot6)))",
            source)

    def test_build_transition_consumes_its_opening_touch(self):
        source = (Path(__file__).resolve().parents[1] /
                  "last_island_recreated_game" / "main.cpp").read_text(
                      encoding="utf-8")
        self.assertIn("const bool buildModeAtFrameStart=buildMode;", source)
        self.assertIn(
            "if(activePanel!=panelAtFrameStart || buildMode!=buildModeAtFrameStart){",
            source)
        segment = source.split(
            "if(activePanel!=panelAtFrameStart || buildMode!=buildModeAtFrameStart){",
            1)[1].split("for(const Vector2 &click:uiClicks)", 1)[0]
        self.assertIn("uiClicks.clear();", segment)
        self.assertIn("LIOS_INPUT_TRANSITION frame=%u", source)
        self.assertIn("LIOS_INPUT_DISPATCH frame=%u scope=build", source)


if __name__ == "__main__":
    unittest.main()
