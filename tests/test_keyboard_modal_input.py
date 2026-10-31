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
        self.assertIn("if(activePanel!=panelAtFrameStart){", self.code)
        self.assertIn("hudInput.captureModal();", self.code)
        self.assertLess(
            self.code.index("if(activePanel!=panelAtFrameStart){"),
            self.code.index("for(const Vector2 &click:uiClicks)"))
        self.assertIn("LIOS_INVENTORY_PANEL_OPEN", self.code)
        self.assertIn("LIOS_CRAFT_PANEL_OPEN", self.code)

    def test_jump_respects_modal_capture(self):
        self.assertIn("bool wantJump = !overlayWasOpen&&activePanel==0", self.code)

    def test_touch_controller_receives_modal_state(self):
        self.assertIn("GetScreenHeight(),overlayWasOpen,buildMode);", self.code)
        header = (SOURCE.parent / "mobile_hud_input.hpp").read_text(encoding="utf-8")
        self.assertIn("if(modalOpen && a!=Action::Inventory", header)
        self.assertIn("void captureModal(){", header)

    def test_modal_touch_cannot_fall_through_to_camera_look(self):
        header = (SOURCE.parent / "mobile_hud_input.hpp").read_text(encoding="utf-8")
        self.assertRegex(
            header,
            r"if\(!modalOpen\s*&&\s*!buildWidget\s*&&\s*a==Action::None\s*&&"
        )
        self.assertLess(header.index("if(modalOpen && a!=Action::Inventory"),
                        header.index("if(!modalOpen && !buildWidget && a==Action::None"))


    def test_construction_controls_capture_before_battle_hud(self):
        header = (SOURCE.parent / "mobile_hud_input.hpp").read_text(encoding="utf-8")
        self.assertIn("static bool buildWidgetHit(float x,float y)", header)
        for control in ("Build_btn_build", "Build_btn_opRotate",
                        "Build_btn_changeButton", "Build_btn_wall",
                        "BuildChildBox0", "BuildChildBox10"):
            with self.subTest(control=control):
                self.assertIn("LIOS_HUD::" + control, header)
        self.assertIn("Action a=buildWidget?Action::None:hit(x,y,rifle);", header)
        self.assertIn("!modalOpen && !buildWidget && a==Action::None", header)
        self.assertIn("LIOS_BUILD_WIDGET_TOUCH_CAPTURE", header)
        self.assertIn("overlayWasOpen,buildMode);", self.code)
        self.assertIn("!overlayWasOpen&&activePanel==0&&!buildMode", self.code)
        self.assertIn("LIOS_BUILD_MODE_TRANSITION enabled=%d", self.code)

    def test_hotbar_and_aim_report_real_state_transitions(self):
        self.assertIn("LIOS_HOTBAR_TOUCH_SELECTED slot=0 equipped=0", self.code)
        self.assertIn("LIOS_HOTBAR_TOUCH_SELECTED slot=1 equipped=1", self.code)
        self.assertIn("LIOS_HOTBAR_TOUCH_SELECTED slot=%d equipped=-1", self.code)
        self.assertIn("LIOS_AIM_TOUCH_TRANSITION aim=%d slot=%d", self.code)
        self.assertIn("!overlayWasOpen&&!buildMode&&hudInput.justPressed(HA::Aim)", self.code)

    def test_builder_shortcut_keeps_modal_guard(self):
        self.assertRegex(
            self.code,
            r"if\(!overlayWasOpen&&building\.blueprint&&\s*"
            r"\(IsKeyPressed\(KEY_B\)\|\|hudInput\.justPressed\(HA::Slot6\)\)\)")


if __name__ == "__main__":
    unittest.main()
