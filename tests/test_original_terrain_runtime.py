"""Source-level checks for the bounded, Unity-extracted native terrain tile.

The APK/emulator must still validate rendered orientation and ray collision.
"""
import math
import re
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
        # The original 520m radius drew every neighboring 400m tile at
        # spawn. Allow performance tuning while preserving a positive border
        # prefetch margin; do not pin a particular optimization constant.
        match = re.search(r"constexpr float drawRadius=([0-9.]+)f;", header)
        self.assertIsNotNone(match)
        radius = float(match.group(1))
        self.assertGreaterEqual(radius, 50.)
        self.assertLess(radius, 200.)
        # With these world bounds, the spawn at (0,0) sees the center
        # sector, not all four distant neighbors. A neighbor is included
        # before the player actually crosses its 200m boundary.
        distance_to_east_at_spawn = 200.
        distance_to_east_near_seam = 50.
        self.assertGreater(distance_to_east_at_spawn, radius)
        self.assertLessEqual(distance_to_east_near_seam, radius)
        self.assertIn("std::isfinite(x)", header)
        self.assertIn("for(const auto& sector:sectors)", header)


    def test_five_offsets_per_boundary_are_runtime_checked(self):
        import ast
        game = (GAME / "main.cpp").read_text(encoding="utf8")
        smoke = (ROOT / ".github/scripts/android_virtual_smoke.sh").read_text(encoding="utf8")
        self.assertIn("const float seamGridOffsets[5]={-150.f,-75.f,0.f,75.f,150.f}", game)
        self.assertIn("for(float offset:seamGridOffsets)", game)
        self.assertIn("LIOS_TERRAIN_SEAM_GRID_PROBE", game)
        self.assertIn("LIOS_TERRAIN_GRID_20_PROBES_PASS", smoke)
        self.assertIn("if set(grid) != expected_grid:", smoke)
        self.assertIn("if abs(hout-hin) > 0.55:", smoke)
        start = "python3 - <<'PY' > evidence/terrain-runtime-check.txt\n"
        end = "\nPY\ncat evidence/terrain-runtime-check.txt"
        # Parse the exact embedded Python that the Android virtual job runs.
        self.assertEqual(smoke.count(start), 1)
        embedded = smoke.split(start, 1)[1].split(end, 1)[0]
        ast.parse(embedded)


if __name__ == "__main__":
    unittest.main()
