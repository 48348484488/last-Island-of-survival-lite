"""Source-level checks for the bounded, Unity-extracted native terrain tile.

The APK/emulator must still validate rendered orientation and ray collision.
"""
import math
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
GAME = ROOT / "last_island_recreated_game"


class TerrainIntegrationTests(unittest.TestCase):
    def test_real_source_tile_and_texture_present(self):
        folder = GAME / "assets" / "original_world"
        obj = folder / "a-57_mesh0.obj"
        tex = folder / "a-57_tex0.png"
        self.assertGreater(obj.stat().st_size, 100_000)
        self.assertGreater(tex.stat().st_size, 1000)
        vertices = []
        faces = 0
        for line in obj.read_text(encoding="utf8").splitlines():
            if line.startswith("v "):
                x, y, z = map(float, line.split()[1:4])
                self.assertTrue(all(math.isfinite(v) for v in (x, y, z)))
                vertices.append((x, z, -y))
            elif line.startswith("f "):
                faces += 1
        self.assertGreater(len(vertices), 1000)
        self.assertGreater(faces, 1000)
        self.assertTrue(any(abs(x)<1e-5 and abs(z)<1e-5 and
                            abs(y-22.3035011)<1e-4 for x,y,z in vertices))

    def test_native_height_collision_and_fallback_are_wired(self):
        header = (GAME / "mobile_original_terrain.hpp").read_text()
        code = (GAME / "main.cpp").read_text()
        packaging = (ROOT / "tools" / "package_android_assets.py").read_text()
        self.assertIn("MatrixRotateX(-PI/2.0f)", header)
        self.assertIn("GetRayCollisionMesh", header)
        self.assertIn("PIXELFORMAT_UNCOMPRESSED_R8G8B8A8", header)
        self.assertIn("LIOS_TERRAIN_MASK_MATERIAL_PREVIEW", header)
        self.assertIn("LIOS_TERRAIN::Tile terrain;", code)
        self.assertIn("terrain.ground(nextPosition.x,nextPosition.z)", code)
        self.assertIn("terrain.ground(player.position.x,player.position.z)", code)
        self.assertIn("terrain.draw(player.position);", code)
        self.assertIn("if(terrain.ready)", code)
        self.assertIn("new.parent.mkdir", packaging)
        self.assertIn("original_world/a-57_mesh0.obj", code + header)


    def test_neighbor_tiles_are_real_and_match_world_bounds(self):
        folder = GAME / "assets" / "original_world"
        header = (GAME / "mobile_original_terrain.hpp").read_text()
        records = {}
        for line in (folder / "map_tiles.txt").read_text().splitlines():
            if line.startswith("#") or not line.strip():
                continue
            mesh, texture, *numbers = line.split()
            records[mesh] = (texture, list(map(float, numbers)))
        expected = {
            "a-57_mesh0.obj": (-200., 200., -200., 200.),
            "a-56_mesh1.obj": (-200., 200., 200., 600.),
            "d-01_mesh2.obj": (-200., 200., -600., -200.),
            "c-07_mesh0.obj": (200., 600., -200., 200.),
            "b-50_mesh2.obj": (-600., -200., -200., 200.),
        }
        for mesh, bounds in expected.items():
            with self.subTest(mesh=mesh):
                texture, source = records[mesh]
                self.assertIn('"assets/original_world/' + mesh + '"', header)
                self.assertIn('"assets/original_world/' + texture + '"', header)
                self.assertGreater((folder / mesh).stat().st_size, 1000)
                self.assertGreater((folder / texture).stat().st_size, 1000)
                for measured, target in zip(source[:4], bounds):
                    self.assertLess(abs(measured - target), 1.0)
        self.assertIn("LIOS_TERRAIN_SECTORS_READY", header)
        self.assertIn("drawRadius=520.f", header)
        self.assertIn("std::isfinite(x)", header)
        self.assertIn("for(const auto& sector:sectors)", header)


if __name__ == "__main__":
    unittest.main()
