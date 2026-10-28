"""Source-derived Last Island LITE 7.3 HUD wired into the recreated game.

Renders the real battlepanel.ab RectTransform hierarchy and XAPK Sprite PNGs
through tools/render_original_hud_v7.py. This is NOT a captured Unity frame.
Original fonts, device safe-area, shader masks and player map coordinates
are not completely recoverable from the serialized prefab.
"""
from __future__ import annotations

import importlib.util
import math
import sys
import time
from pathlib import Path

REFERENCE_SIZE = (1920, 1080)
RENDER_SIZE = (960, 540)
ASSETS = Path(__file__).resolve().parent / "assets" / "hud_original_xapk"
TOOLS = Path(__file__).resolve().parents[1] / "tools"


def game_hud_state(*, health, food, water, yaw, equipped_slot, wood=0,
                   metal=0, stone=0, sulfur=0):
    """Translate live game variables to the V7 original Unity HUD state.

    Slots 0/1 are equipped weapons, but their exact inventory icon Sprite
    references are not proven by the original battlepanel prefab. Do not
    silently use an unrelated icon. Resource icons in slots 2-5 ARE source
    Sprite names and use the actual collected game counts.
    """
    if equipped_slot not in (0, 1):
        raise ValueError("Source HUD currently supports the two equipped game slots")
    for name, value in (("health", health), ("food", food), ("water", water)):
        if not math.isfinite(float(value)):
            raise ValueError("Non-finite player vital: " + name)
    inventory = [None] * 7
    for index, (sprite, count) in enumerate(
        (("tb_cl_mt", wood), ("tb_cl_tiek", metal),
         ("tb_cl_st", stone), ("ks_lhk", sulfur)), start=2
    ):
        number = max(0, min(999999, int(count)))
        if number:
            inventory[index] = {"sprite": sprite, "count": number}
    return {
        "profile": "on-foot-rifle" if equipped_slot == 1 else "on-foot-melee",
        "selected_slot": equipped_slot,
        "heading_deg": round(math.degrees(float(yaw)) % 360, 1),
        "vitals": {key: max(0, min(1, float(val) / 100.0))
                   for key, val in (("health", health), ("food", food),
                                    ("water", water))},
        "inventory": inventory,
    }


def viewport_bounds(width, height):
    """Fit the 1920x1080 source Canvas without distorting Unity circles."""
    width, height = int(width), int(height)
    if width <= 0 or height <= 0:
        raise ValueError("Invalid HUD viewport")
    scale = min(width / REFERENCE_SIZE[0], height / REFERENCE_SIZE[1])
    w, h = REFERENCE_SIZE[0] * scale, REFERENCE_SIZE[1] * scale
    return (0.5 * (width - w), 0.5 * (height - h), w, h)


def viewport_to_original(x, y, width, height):
    """Convert a top-left-origin screen touch to Unity source HUD coordinates."""
    left, top, w, h = viewport_bounds(width, height)
    if not (left <= x <= left + w and top <= y <= top + h):
        return None
    return ((x - left) / w * REFERENCE_SIZE[0],
            (y - top) / h * REFERENCE_SIZE[1])


def _load_v7():
    # V7 in turn loads the V6/SourceHUD modules using original project paths.
    # The game is started from its own directory, not from tools/, so this
    # import path must be present before importing the source renderer.
    if str(TOOLS) not in sys.path:
        sys.path.insert(0, str(TOOLS))
    path = TOOLS / "render_original_hud_v7.py"
    spec = importlib.util.spec_from_file_location("_lios_source_hud_v7", path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module.HudV7


def render_source_layer(state, *, assets=ASSETS):
    """Pure Pillow frame; deliberately does not require an OpenGL context."""
    HudV7 = _load_v7()
    hud = HudV7(assets, map_uv=[0.5, 0.5], heading_deg=state["heading_deg"])
    audit = hud.render_complete(state=state, demo=False)
    if audit["preview_inventory_is_demonstrative"]:
        raise AssertionError("Source HUD must use actual inventory, not demo items")
    return hud.layer, audit


class OriginalUnityHud:
    """One reusable OpenGL texture; source assets are not uploaded per sprite.

    Raster updates are bounded to two per second, and the same GPU storage is
    reused. This preserves the existing 3D render loop and avoids recreating
    dozens of GL textures per frame on integrated graphics.
    """
    def __init__(self, *, assets=ASSETS, update_interval=0.5):
        if not (assets / "manifest_original.json").is_file():
            raise FileNotFoundError("Original XAPK HUD sprites not installed")
        self.assets = Path(assets)
        self.interval = max(0.25, float(update_interval))
        self.texture = None
        self._last_time = float("-inf")
        self._last_signature = None
        self._last_report = None
        self._tex_dims = (1024, 1024)

    def _upload(self, source):
        from PIL import Image
        from OpenGL.GL import (
            GL_CLAMP, GL_LINEAR, GL_RGBA, GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER,
            GL_TEXTURE_MIN_FILTER, GL_TEXTURE_WRAP_S, GL_TEXTURE_WRAP_T,
            GL_UNPACK_ALIGNMENT, GL_UNSIGNED_BYTE, glBindTexture, glGenTextures,
            glPixelStorei, glTexImage2D, glTexParameteri, glTexSubImage2D,
        )
        reduced = source.resize(RENDER_SIZE, Image.Resampling.LANCZOS)
        rgba = Image.new("RGBA", self._tex_dims)
        rgba.paste(reduced, (0, 0))
        pixels = rgba.tobytes("raw", "RGBA", 0, -1)
        if self.texture is None:
            self.texture = glGenTextures(1)
            glBindTexture(GL_TEXTURE_2D, self.texture)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP)
            glPixelStorei(GL_UNPACK_ALIGNMENT, 1)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, *self._tex_dims,
                         0, GL_RGBA, GL_UNSIGNED_BYTE, pixels)
        else:
            glBindTexture(GL_TEXTURE_2D, self.texture)
            glPixelStorei(GL_UNPACK_ALIGNMENT, 1)
            glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, *self._tex_dims,
                            GL_RGBA, GL_UNSIGNED_BYTE, pixels)
        glBindTexture(GL_TEXTURE_2D, 0)

    def draw(self, width, height, state):
        from OpenGL.GL import (
            GL_BLEND, GL_COLOR_BUFFER_BIT, GL_DEPTH_TEST, GL_ENABLE_BIT,
            GL_LIGHTING, GL_MODELVIEW, GL_ONE_MINUS_SRC_ALPHA, GL_PROJECTION,
            GL_QUADS, GL_SRC_ALPHA, GL_TEXTURE_2D, GL_TEXTURE_BIT,
            glBegin, glBindTexture, glBlendFunc, glColor4f, glDisable, glEnable,
            glEnd, glLoadIdentity, glMatrixMode, glOrtho, glPopAttrib,
            glPopMatrix, glPushAttrib, glPushMatrix, glTexCoord2f, glVertex2f,
        )
        # Dynamic compass needs rebuilding when its source heading changes.
        # Snap headings to 5 degrees so a tiny mouse motion does not rebuild
        # the full original Canvas each frame.
        signature = (state["profile"], state["selected_slot"],
                     round(state["heading_deg"] / 5) * 5,
                     tuple(state["vitals"].values()),
                     tuple((x["sprite"], x["count"]) if x else None
                           for x in state["inventory"]))
        now = time.monotonic()
        if self.texture is None or (
            signature != self._last_signature and now - self._last_time >= self.interval
        ):
            layer, self._last_report = render_source_layer(state, assets=self.assets)
            self._upload(layer)
            self._last_signature = signature
            self._last_time = now

        left, bottom, w, h = viewport_bounds(width, height)
        umax = RENDER_SIZE[0] / self._tex_dims[0]
        vmax = RENDER_SIZE[1] / self._tex_dims[1]
        glPushAttrib(GL_ENABLE_BIT | GL_COLOR_BUFFER_BIT | GL_TEXTURE_BIT)
        glMatrixMode(GL_PROJECTION)
        glPushMatrix()
        glLoadIdentity()
        glOrtho(0, width, 0, height, -1, 1)
        glMatrixMode(GL_MODELVIEW)
        glPushMatrix()
        glLoadIdentity()
        glDisable(GL_DEPTH_TEST)
        glDisable(GL_LIGHTING)
        glEnable(GL_TEXTURE_2D)
        glEnable(GL_BLEND)
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA)
        glColor4f(1, 1, 1, 1)
        glBindTexture(GL_TEXTURE_2D, self.texture)
        glBegin(GL_QUADS)
        glTexCoord2f(0, 0);       glVertex2f(left, bottom)
        glTexCoord2f(umax, 0);    glVertex2f(left + w, bottom)
        glTexCoord2f(umax, vmax); glVertex2f(left + w, bottom + h)
        glTexCoord2f(0, vmax);    glVertex2f(left, bottom + h)
        glEnd()
        glBindTexture(GL_TEXTURE_2D, 0)
        glPopMatrix()
        glMatrixMode(GL_PROJECTION)
        glPopMatrix()
        glMatrixMode(GL_MODELVIEW)
        glPopAttrib()

    def close(self):
        if self.texture is not None:
            from OpenGL.GL import glDeleteTextures
            glDeleteTextures(int(self.texture))
            self.texture = None
