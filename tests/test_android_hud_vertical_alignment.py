"""Compare original Unity 7.3 HUD control centers with real Android pixels.

Evidence: source battlepanel.ab RectTransforms and 1080x640 screenshots from
virtual Android workflow #41041246689. These are alignment checks, not proof
of original Unity CanvasScaler equivalence.
"""
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
INPUT = ROOT / "last_island_recreated_game" / "mobile_hud_input.hpp"
MAIN = ROOT / "last_island_recreated_game" / "main.cpp"

# (source center X, source center Y, observed on-screen center X/Y).
# Source PathIDs: craft 210616117287627897; inventory 3694177321274107552;
# settings 5039290222044803911; jump 6868289150097382791.
CONTROLS = {
    "craft": (61.0, 1025.0, 34.0, 480.0),
    "inventory": (513.02501, 976.625, 289.0, 454.0),
    "settings": (1854.70078, 354.98001, 1043.0, 103.0),
    "jump": (1834.49316, 789.9768, 1032.0, 348.0),
    "joystick": (326.3, 794.7, 183.0, 351.0),
}


def projected(x, y, width=1080, height=640, source_offset_y=-200.0):
    scale = min(width / 1920.0, height / 1080.0)
    left = (width - 1920.0 * scale) / 2
    top = (height - 1080.0 * scale) / 2
    return left + x * scale, top + (y + source_offset_y) * scale


class AndroidHudAlignmentTests(unittest.TestCase):
    def test_five_original_control_centers_match_real_pixels(self):
        for name, (sx, sy, px, py) in CONTROLS.items():
            with self.subTest(name=name):
                x, y = projected(sx, sy)
                self.assertAlmostEqual(x, px, delta=3.0)
                self.assertAlmostEqual(y, py, delta=3.0)

    def test_touch_and_render_use_one_shared_offset(self):
        source = INPUT.read_text(encoding="utf-8")
        main = MAIN.read_text(encoding="utf-8")
        self.assertIn("HudVisualYOffset = -200.0f", source)
        self.assertIn("y=(screen.y-top)/scale-HudVisualYOffset", source)
        self.assertIn("centerY+dy*LIOS_HUD::JoystickRadius+HudVisualYOffset", source)
        self.assertIn("centerY+HudVisualYOffset", source)
        self.assertIn("bounds.top+dy", main)
        self.assertIn("LIOS_INPUT::HudVisualYOffset", main)


    def test_modal_buttons_can_close_only_their_own_panels(self):
        main = MAIN.read_text(encoding="utf-8")
        self.assertIn(
            '(!overlayWasOpen||activePanel==1)&&hudInput.justPressed(HA::Inventory)',
            main)
        self.assertIn(
            '(!overlayWasOpen||activePanel==3)&&hudInput.justPressed(HA::Settings)',
            main)
        self.assertIn(
            '(!overlayWasOpen||activePanel==4)&&hudInput.justPressed(HA::QuickChat)',
            main)
        self.assertIn('LIOS_INVENTORY_PANEL_OPEN', main)
        self.assertIn('LIOS_INVENTORY_PANEL_CLOSE', main)
        self.assertIn('LIOS_SETTINGS_PANEL_OPEN', main)
        self.assertIn('LIOS_SETTINGS_PANEL_CLOSE', main)
        # Unrelated gameplay controls remain modal-gated.
        self.assertIn('!overlayWasOpen&&!buildMode&&hudInput.justPressed(HA::Aim)', main)
        self.assertIn('!overlayWasOpen&&hudInput.justPressed(HA::Craft)', main)


if __name__ == "__main__":
    unittest.main()
