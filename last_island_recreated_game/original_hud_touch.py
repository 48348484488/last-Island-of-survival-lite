"""Device touch input mapped to the original Last Island LITE Unity UGUI.

The only hitboxes used here are serialized RectTransforms from battlepanel.ab.
The source joystick trigger, visual center and base radius are taken from the
original m_yaoganTrigger / m_littleYaogan / m_YaoganBG GameObjects.

Coordinates are top-left-origin viewport pixels. Source CanvasScaler/safe-area
have NOT been recovered, so input fitting currently uses the same letterboxed
1920x1080 projection as the existing V11 HUD renderer.
"""
from __future__ import annotations

import math
from functools import lru_cache

from original_hud_bindings import DEFAULT_LAYOUT, _layout, hit_test
from original_hud_runtime import viewport_to_original

_JOY_TRIGGER = "m_yaoganTrigger"
_JOY_CENTER = "m_littleYaogan"
_JOY_BG = "m_YaoganBG"


@lru_cache(maxsize=3)
def original_joystick(path=str(DEFAULT_LAYOUT)):
    data = _layout(str(path))
    nodes = data["nodes"].values() if isinstance(data["nodes"], dict) else data["nodes"]
    wanted = {}
    for node in nodes:
        if node.get("name") in (_JOY_TRIGGER, _JOY_CENTER, _JOY_BG):
            wanted.setdefault(node["name"], []).append(node)
    for name in (_JOY_TRIGGER, _JOY_CENTER, _JOY_BG):
        if len(wanted.get(name, [])) != 1:
            raise ValueError("Ambiguous original joystick node: " + name)
    trigger = wanted[_JOY_TRIGGER][0]["pixel_bounds_top_left"]
    center = wanted[_JOY_CENTER][0]["pixel_bounds_top_left"]
    circle = wanted[_JOY_BG][0]["pixel_bounds_top_left"]
    for rect in (trigger, center, circle):
        if not (rect["left"] < rect["right"] and rect["top"] < rect["bottom"]):
            raise ValueError("Invalid Unity joystick geometry")
    return {
        "trigger": dict(trigger),
        "center": ((center["left"] + center["right"]) / 2,
                   (center["top"] + center["bottom"]) / 2),
        "radius": min(circle["right"] - circle["left"],
                      circle["bottom"] - circle["top"]) / 2,
        "source": {
            "trigger": wanted[_JOY_TRIGGER][0]["go_path_id"],
            "center": wanted[_JOY_CENTER][0]["go_path_id"],
            "background": wanted[_JOY_BG][0]["go_path_id"],
        },
    }


class OriginalHudTouch:
    """Multi-finger controller. Returns edge events as (action, down/up).

    No UI/gameplay effects occur here: the game decides how to implement
    attack, jump, aiming and menu actions. Finger identities are never
    conflated, and joystick movement does not cancel a second-finger attack.
    """
    def __init__(self, *, profile="on-foot-melee", path=DEFAULT_LAYOUT):
        self.path = str(path)
        self.profile = profile
        self.joystick = original_joystick(self.path)
        self._owners = {}  # finger_id -> (control kind, action)
        self._counts = {}  # button action -> number of held fingers
        self.axis = (0.0, 0.0)  # x positive right; y positive down

    def set_profile(self, profile):
        if profile not in ("on-foot-melee", "on-foot-rifle"):
            raise ValueError("Unknown Unity on-foot profile")
        if profile == self.profile:
            return []
        self.profile = profile
        from original_hud_bindings import original_touch_rects
        allowed = original_touch_rects(profile=profile, path=self.path)
        released = []
        for finger, (kind, action) in list(self._owners.items()):
            if kind == "button" and action not in allowed:
                released.extend(self.end(finger))
        return released

    @property
    def held_actions(self):
        return frozenset(k for k, v in self._counts.items() if v > 0)

    def held(self, action):
        return self._counts.get(action, 0) > 0

    def _source(self, x, y, width, height):
        if not (math.isfinite(x) and math.isfinite(y)):
            return None
        return viewport_to_original(x, y, width, height)

    def start(self, finger, x, y, width, height):
        if finger in self._owners:
            # Duplicate down from SDL mouse synthesis must never double-trigger.
            return []
        xy = self._source(x, y, width, height)
        if xy is None:
            return []
        action = hit_test(*xy, profile=self.profile, path=self.path)
        if action:
            self._owners[finger] = ("button", action)
            old = self._counts.get(action, 0)
            self._counts[action] = old + 1
            return [(action, "down")] if old == 0 else []
        bounds = self.joystick["trigger"]
        if all((bounds["left"] <= xy[0] <= bounds["right"],
                bounds["top"] <= xy[1] <= bounds["bottom"])):
            # One joystick owner; the remaining fingers stay free for buttons.
            if any(kind == "joystick" for kind, _ in self._owners.values()):
                return []
            self._owners[finger] = ("joystick", None)
            self._move_joystick(*xy)
        return []

    def _move_joystick(self, x, y):
        cx, cy = self.joystick["center"]
        radius = self.joystick["radius"]
        dx, dy = (x - cx) / radius, (y - cy) / radius
        dist = math.hypot(dx, dy)
        if dist > 1:
            dx, dy = dx / dist, dy / dist
        self.axis = (dx, dy)

    def move(self, finger, x, y, width, height):
        if self._owners.get(finger, (None,))[0] != "joystick":
            return []
        xy = self._source(x, y, width, height)
        if xy is not None:
            self._move_joystick(*xy)
        return []

    def end(self, finger):
        owned = self._owners.pop(finger, None)
        if owned is None:
            return []
        kind, action = owned
        if kind == "joystick":
            self.axis = (0.0, 0.0)
            return []
        count = self._counts[action] - 1
        if count <= 0:
            self._counts.pop(action, None)
            return [(action, "up")]
        self._counts[action] = count
        return []

    def reset(self):
        released = []
        for finger in list(self._owners):
            released.extend(self.end(finger))
        return released
