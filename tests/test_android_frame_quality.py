"""Regression tests for Android screenshot readiness; no game screenshots are fabricated."""
import importlib.util
import tempfile
import unittest
from pathlib import Path
from PIL import Image

MODULE = Path(__file__).resolve().parents[1] / "tools" / "android_frame_quality.py"
spec = importlib.util.spec_from_file_location("android_frame_quality", MODULE)
gate = importlib.util.module_from_spec(spec)
spec.loader.exec_module(gate)


class FrameQualityTests(unittest.TestCase):
    def test_flat_green_frame_is_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "flat.png"
            Image.new("RGB", (1080, 640), (68, 115, 52)).save(path)
            result = gate.inspect_frame(path)
            self.assertFalse(result["nonflat_frame"])
            self.assertEqual(result["distinct_rgb_colors_at_least"], 1)

    def test_multicolor_game_frame_passes_heuristic(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "frame.png"
            image = Image.new("RGB", (640, 360))
            for x in range(32):
                for y in range(32):
                    image.putpixel((x, y), (x * 7, y * 5, (x + y) * 3))
            image.save(path)
            result = gate.inspect_frame(path)
            self.assertTrue(result["nonflat_frame"])
            self.assertTrue(result["color_count_is_lower_bound"])
            self.assertFalse(result["gameplay_or_visual_parity_proven"])

    def test_colorful_hud_on_flat_world_is_not_a_ready_scene(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "hud-only.png"
            frame = Image.new("RGB", (640, 360), (86, 115, 72))
            for x in range(30):
                for y in range(30):
                    frame.putpixel((x, y), (x * 7, y * 7, (x + y) * 4))
            frame.save(path)
            report = gate.inspect_frame(path)
            self.assertTrue(report["nonflat_frame"])
            self.assertFalse(report["spatial_scene_coverage"])
            self.assertFalse(report["scene_ready"])
            self.assertFalse(report["center_tile_varied"])

    def test_distributed_scene_has_spatial_coverage(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "scene.png"
            frame = Image.new("RGB", (640, 360))
            for y in range(360):
                for x in range(640):
                    frame.putpixel((x, y), (x * 7 % 256, y * 9 % 256,
                                           (x + y) * 3 % 256))
            frame.save(path)
            report = gate.inspect_frame(path)
            self.assertTrue(report["scene_ready"])
            self.assertEqual(report["varied_tiles"], 9)
            self.assertFalse(report["gameplay_or_visual_parity_proven"])

    def test_first_frame_smoke_requires_spatial_coverage(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text()
        self.assertIn("--require-spatial-coverage", smoke)
        self.assertIn("evidence/frame-quality.json", smoke)

    def test_small_or_corrupt_capture_is_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "frame.png"
            Image.new("RGB", (20, 20)).save(path)
            with self.assertRaisesRegex(ValueError, "resolution"):
                gate.inspect_frame(path)
            path.write_bytes(b"not a screenshot")
            with self.assertRaises(Exception):
                gate.inspect_frame(path)


    def test_identical_screenshots_do_not_prove_transition(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360), (68, 115, 52)).save(before)
            Image.new("RGBA", (640, 360), (68, 115, 52, 255)).save(after)
            result = gate.inspect_transition(before, after)
            self.assertFalse(result["transition_detected"])
            self.assertEqual(result["changed_pixels"], 0)
            self.assertIsNone(result["difference_bounds"])

    def test_changed_panel_region_detected(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360), (68, 115, 52)).save(before)
            frame = Image.new("RGB", (640, 360), (68, 115, 52))
            frame.paste((200, 210, 220), (100, 60, 300, 260))
            frame.save(after)
            result = gate.inspect_transition(before, after, min_changed_fraction=0.1)
            self.assertTrue(result["transition_detected"])
            self.assertEqual(result["changed_pixels"], 40000)
            self.assertEqual(result["difference_bounds"], [100, 60, 300, 260])
            self.assertFalse(result["action_or_original_game_parity_proven"])

    def test_small_change_rejected_at_threshold(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360), (0, 0, 0)).save(before)
            frame = Image.new("RGB", (640, 360), (0, 0, 0))
            frame.paste((255, 0, 0), (0, 0, 10, 10))
            frame.save(after)
            self.assertFalse(gate.inspect_transition(before, after)["transition_detected"])
            self.assertEqual(gate.inspect_transition(before, after, min_changed_fraction=0.0001)["changed_pixels"], 100)

    def test_resolution_and_invalid_threshold_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            before, after = Path(folder) / "before.png", Path(folder) / "after.png"
            Image.new("RGB", (640, 360)).save(before)
            Image.new("RGB", (320, 240)).save(after)
            with self.assertRaisesRegex(ValueError, "resolutions differ"):
                gate.inspect_transition(before, after)
            with self.assertRaisesRegex(ValueError, "min_changed_fraction"):
                gate.inspect_transition(before, after, float("nan"))
            with self.assertRaisesRegex(ValueError, "min_channel_delta"):
                gate.inspect_transition(before, after, 0.01, 0)


    def test_flat_frame_failure_collects_render_loop_diagnostics(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(encoding="utf-8")
        self.assertIn("LIOS_FLAT_FRAME_WITH_ACTIVE_RENDER_LOOP", smoke)
        self.assertIn("LIOS_FLAT_FRAME_RENDER_LOOP_NOT_CONFIRMED", smoke)
        self.assertIn("evidence/frame-heartbeats.txt", smoke)
        self.assertIn("exit 47", smoke)

    def test_android_smoke_requires_fresh_touch_event_and_visual_change(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(encoding="utf-8")
        self.assertIn("adb logcat -c", smoke)
        self.assertIn("--compare-with evidence/runtime.png", smoke)
        self.assertIn("LIOS_CRAFT_LOGGED_BUT_VISUAL_TRANSITION_UNCONFIRMED", smoke)
        self.assertIn("LIOS_VISIBLE_CRAFT_BUTTON_FUNCTIONAL_FAILURE", smoke)


    def test_followup_inventory_probe_requires_action_and_real_transition(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(
                     encoding="utf-8")
        self.assertIn("LIOS_CRAFT_PANEL_CLOSE", smoke)
        self.assertIn("LIOS_INVENTORY_PANEL_OPEN", smoke)
        self.assertIn("LIOS_INVENTORY_PANEL_CLOSE", smoke)
        self.assertIn("--compare-with evidence/after-craft-close.png", smoke)
        self.assertIn("--min-changed-fraction 0.01", smoke)
        self.assertIn("LIOS_INVENTORY_VISIBLE_TOUCH_UNCONFIRMED", smoke)
        self.assertIn("LIOS_INVENTORY_SAME_BUTTON_CLOSE_UNCONFIRMED", smoke)


    def test_inventory_gate_requires_fresh_open_and_visual_close(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(
                     encoding="utf-8")
        self.assertIn("LIOS_INVENTORY_OPEN_FUNCTIONAL_FAILURE", smoke)
        self.assertIn("LIOS_INVENTORY_CLOSE_FUNCTIONAL_FAILURE", smoke)
        self.assertIn("exit 51", smoke)
        self.assertIn("exit 52", smoke)
        self.assertIn("--compare-with evidence/inventory-open.png", smoke)
        self.assertIn("LIOS_INVENTORY_CLOSE_VISUAL_TRANSITION_UNCONFIRMED", smoke)

    def test_settings_probe_collects_independent_action_and_frame(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(
                     encoding="utf-8")
        self.assertIn("LIOS_SETTINGS_PANEL_OPEN", smoke)
        self.assertIn("LIOS_SETTINGS_PANEL_CLOSE", smoke)
        self.assertIn("--compare-with evidence/inventory-after-close.png", smoke)
        self.assertIn("LIOS_SETTINGS_VISIBLE_TOUCH_UNCONFIRMED", smoke)
        self.assertIn("evidence/settings-after-close.png", smoke)
        self.assertIn("--compare-with evidence/settings-open.png", smoke)
        self.assertIn("evidence/settings-close-transition.json", smoke)
        self.assertIn("LIOS_SETTINGS_CLOSE_VISUAL_TRANSITION_UNCONFIRMED", smoke)


    def test_settings_gate_requires_fresh_open_and_visual_close(self):
        smoke = (Path(__file__).resolve().parents[1] /
                 ".github" / "scripts" / "android_virtual_smoke.sh").read_text(
                     encoding="utf-8")
        self.assertIn("LIOS_SETTINGS_OPEN_FUNCTIONAL_FAILURE", smoke)
        self.assertIn("LIOS_SETTINGS_CLOSE_FUNCTIONAL_FAILURE", smoke)
        self.assertIn("exit 53", smoke)
        self.assertIn("exit 54", smoke)
        self.assertIn("--compare-with evidence/settings-open.png", smoke)
        self.assertIn("LIOS_SETTINGS_SAME_BUTTON_CLOSE_CONFIRMED", smoke)


    def test_runtime_captures_measured_frame_pacing_and_touch_ownership(self):
        root = Path(__file__).resolve().parents[1]
        game = (root / "last_island_recreated_game" / "main.cpp").read_text(encoding="utf8")
        input_header = (root / "last_island_recreated_game" /
                        "mobile_hud_input.hpp").read_text(encoding="utf8")
        terrain = (root / "last_island_recreated_game" /
                   "mobile_original_terrain.hpp").read_text(encoding="utf8")
        smoke = (root / ".github" / "scripts" /
                 "android_virtual_smoke.sh").read_text(encoding="utf8")
        self.assertIn("LIOS_FRAME_TIMING frames=60", game)
        self.assertIn("LIOS_HUD_TOUCH_DOWN finger=", input_header)
        self.assertIn("LIOS_TERRAIN_DRAW sectors=", terrain)
        self.assertIn("evidence/frame-pacing-summary.json", smoke)
        self.assertIn("evidence/input-frame-telemetry.txt", smoke)
        self.assertIn('"baseline_comparison_performed":False', smoke)

    def test_android_build_reuses_pinned_source_artifacts_without_skipping_validation(self):
        workflow = (Path(__file__).resolve().parents[1] /
                    ".github" / "workflows" /
                    "build_android_raylib_lite.yml").read_text(encoding="utf-8")
        self.assertIn("actions/cache@v4", workflow)
        for name in ("native_craft_source_manifest.json",
                     "build_surface_manifest.json",
                     "part_icons_manifest.json",
                     "edgeai_real_buildpart_decoded.json"):
            self.assertIn(name, workflow)
            self.assertIn("PINNED_SOURCE_ARTIFACT_UNAVAILABLE " + name, workflow)
        self.assertIn("for attempt in 1 2 3; do", workflow)
        self.assertIn("if [[ \"$attempt\" -eq 3 ]]; then", workflow)


    def test_release_publish_retries_transient_api_errors_and_fails_closed(self):
        workflow = (Path(__file__).resolve().parents[1] /
                    ".github" / "workflows" /
                    "build_android_raylib_lite.yml").read_text(encoding="utf-8")
        publish = workflow.split("  publish-direct-apk:", 1)[1].split(
            "  android-virtual-smoke:", 1)[0]
        self.assertIn("for attempt in 1 2 3 4; do", publish)
        self.assertIn('gh release upload "$TAG" "$APK"', publish)
        self.assertIn('gh release create "$TAG" "$APK"', publish)
        self.assertIn("LIOS_RELEASE_PUBLISH_RETRY", publish)
        self.assertIn("LIOS_RELEASE_PUBLISH_FAILED_AFTER_RETRIES", publish)
        self.assertIn("LIOS_RELEASE_VERIFY_FAILED", publish)
        self.assertIn('if [[ "$published" -ne 1 ]]; then', publish)
        self.assertIn('test -s "$APK"', publish)


if __name__ == "__main__":
    unittest.main()
