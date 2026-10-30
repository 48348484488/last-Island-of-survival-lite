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

if __name__ == "__main__":
    unittest.main()
