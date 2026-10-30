#!/usr/bin/env python3
"""Source-backed static regression: drawing and hit testing share rectangles."""
from pathlib import Path
import re
import unittest

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "last_island_recreated_game/mobile_original_craft_layout.hpp"
CLIENT = ROOT / "last_island_recreated_game/mobile_real_craft_catalog.hpp"

class CraftGeometryContract(unittest.TestCase):
    def test_shared_back_hitbox(self):
        code = CLIENT.read_text(encoding="utf-8")
        self.assertIn("spr(art.back,sourceRect(LIOS_CRAFT_SOURCE::Back,sc,ox,oy))", code)
        self.assertIn("CheckCollisionPointRec(p,sourceRect(LIOS_CRAFT_SOURCE::Back,sc,ox,oy))", code)
        source = SOURCE.read_text(encoding="utf-8")
        self.assertRegex(source, r"Back\s*=\s*\{1654\.0f,0\.0f,316\.0f,86\.0f\}")

    def test_subtabs_share_source_spacing(self):
        code = CLIENT.read_text(encoding="utf-8")
        self.assertIn("Rectangle tabBox=subTabRect(i,sc,ox,oy)", code)
        self.assertIn("spr(art.subTab,tabBox", code)
        self.assertIn("DrawRectangleRec(tabBox", code)
        self.assertIn("CheckCollisionPointRec(p,subTabRect(i,sc,ox,oy))", code)
        self.assertIn("LIOS_CRAFT_SOURCE::SubTabStepX*index", code)
        self.assertRegex(SOURCE.read_text(encoding="utf-8"),
                         r"SubTabStepX\s*=\s*145\.0f")

    def test_quantity_controls_render_and_touch_use_same_unity_bounds(self):
        code = CLIENT.read_text(encoding="utf-8")
        source = SOURCE.read_text(encoding="utf-8")
        for control in ("QuantityMinus", "QuantityPlus", "QuantityMax"):
            ref = "LIOS_CRAFT_SOURCE::" + control
            self.assertIn("DrawRectangleRec(sourceRect(" + ref + ",sc,ox,oy)", code)
            self.assertIn("CheckCollisionPointRec(p,sourceRect(" + ref + ",sc,ox,oy))", code)
            self.assertRegex(source, rf"{control}\\s*=\\s*\\{{[0-9.]+f,954\\.3f,")
        self.assertNotIn("hit(p,1225,741", code)
        self.assertNotIn("hit(p,1380,741", code)
        self.assertNotIn("hit(p,1480,741", code)

    def test_native_create_fallback_does_not_overlap_source_quantity_max(self):
        source = SOURCE.read_text(encoding="utf-8")
        code = CLIENT.read_text(encoding="utf-8")
        match = re.search(r"QuantityMax\\s*=\\s*\\{([0-9.]+)f,([0-9.]+)f,([0-9.]+)f,([0-9.]+)f\\}", source)
        self.assertIsNotNone(match)
        x, y, w, h = map(float, match.groups())
        create = (1510., 912., 237., 72.)
        self.assertIn("DrawRectangleRec(B(1510,912,237,72)", code)
        self.assertIn("hit(p,1510,912,237,72,sc,ox,oy)", code)
        overlap_w = max(0., min(x + w, create[0] + create[2]) - max(x, create[0]))
        overlap_h = max(0., min(y + h, create[1] + create[3]) - max(y, create[1]))
        self.assertEqual(overlap_w * overlap_h, 0.)
        self.assertIn("state.quantity+1", code)
        self.assertIn("state.maxQuantity(*recipe,resources)", code)

if __name__ == "__main__":
    unittest.main()
