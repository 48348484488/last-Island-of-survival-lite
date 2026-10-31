#!/usr/bin/env bash
# Actual Android touch probe for the offline builder. Requires the explicitly
# opt-in debug.lios.build_fixture=resources_v1 property set before game launch.
# This seeds materials only; the blueprint must be crafted through the UI.
set -Eeuo pipefail
mkdir -p evidence
tap() {
  local label="$1" x="$2" y="$3" expected="$4"
  adb logcat -c
  adb shell input swipe "$x" "$y" "$x" "$y" 700
  sleep 2
  adb logcat -d -s raylib:I > "evidence/build-$label-logcat.txt" 2>&1 || true
  if ! grep -F "$expected" "evidence/build-$label-logcat.txt" >/dev/null; then
    echo "LIOS_BUILD_TOUCH_FAILED step=$label x=$x y=$y marker=$expected" |
      tee -a evidence/build-probe-result.txt
    exit 59
  fi
  echo "LIOS_BUILD_TOUCH_PASS step=$label x=$x y=$y" |
    tee -a evidence/build-probe-result.txt
}
grep -F 'LIOS_TEST_FIXTURE_RESOURCES wood=250 stone=250 metal=250 blueprint=0' \
 evidence/terrain-startup-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_FIXTURE_NOT_APPLIED; exit 58; }
# Same visible crafting icon already validated by the main smoke.
read -r visible_x visible_y < <(python3 - <<'PY'
import json
from pathlib import Path
d=json.loads(Path("evidence/frame-quality.json").read_text())
print(round(d["width"]*34/1080),round(d["height"]*480/640))
PY
)
tap craft_open "$visible_x" "$visible_y" 'LIOS_CRAFT_PANEL_OPEN'
read -r prototype_x prototype_y create_x create_y close_x close_y slot6_x slot6_y < <(python3 - <<'PY'
import json
from pathlib import Path
d=json.loads(Path("evidence/frame-quality.json").read_text())
w,h=d["width"],d["height"]
scale=min(w/1920.,h/1080.)
left=(w-1920*scale)/2
top=(h-1080*scale)/2
def pt(x,y):
    return round(left+x*scale),round(top+y*scale)
preview=json.loads(Path("last_island_recreated_game/assets/hud_original_xapk/complete_source_mapping/hud_v8_onfoot_source_preview.json").read_text())
hits=[v for v in preview["draw_calls"] if v.get("go")=="m_Item7.runtime_m_icon"]
if len(hits)!=1: raise SystemExit("LIOS_BUILD_SLOT6_SOURCE_ICON_MISSING")
v=hits[0]
coords=pt(1685+225/2,1011+60/2)+pt(1510+200/2,951+75/2)+pt(1654+316/2,86/2)+pt(v["x"]+v["w"]/2,v["y"]+v["h"]/2-200)
if any(not (0<=coords[i]<w and 0<=coords[i+1]<h) for i in range(0,len(coords),2)):
    raise SystemExit("LIOS_BUILD_SOURCE_COORDINATE_OUTSIDE_VIEWPORT")
print(*coords)
PY
)
printf 'prototype=%s,%s create=%s,%s close=%s,%s slot6=%s,%s\n' \
 "$prototype_x" "$prototype_y" "$create_x" "$create_y" \
 "$close_x" "$close_y" "$slot6_x" "$slot6_y" > evidence/build-craft-coordinates.txt
tap prototype "$prototype_x" "$prototype_y" 'LIOS_OFFLINE_CRAFT_PROTOTYPE enabled=1'
tap queue "$create_x" "$create_y" 'LIOS_OFFLINE_CRAFT_QUEUED recipe=0 count=1'
sleep 4
adb logcat -d -s raylib:I > evidence/build-blueprint-ready-logcat.txt 2>&1 || true
grep -F 'LIOS_OFFLINE_BLUEPRINT_READY crafted=1' evidence/build-blueprint-ready-logcat.txt ||
 { echo LIOS_BUILD_BLUEPRINT_NOT_READY; exit 60; }
tap craft_close "$close_x" "$close_y" 'LIOS_CRAFT_PANEL_CLOSE'
tap equip "$slot6_x" "$slot6_y" 'LIOS_BUILD_MODE_TRANSITION enabled=1 blueprint=1'
adb exec-out screencap -p > evidence/build-mode.png || true
# Require real transition and prevent dispatch on the opening frame.
python3 - <<'PY'
import re
from pathlib import Path
s=Path("evidence/build-equip-logcat.txt").read_text()
m=re.search(r"LIOS_INPUT_TRANSITION frame=(\d+) from=game to=build",s)
if not m:raise SystemExit("LIOS_BUILD_ENTER_TRANSITION_MISSING")
if re.search(r"LIOS_INPUT_DISPATCH frame="+m[1]+r" scope=build\b",s):
    raise SystemExit("LIOS_BUILD_OPENING_TOUCH_LEAKED")
for name in ("foundation","wall","child0","rotate","place","exit"):
    if not re.search(r"LIOS_BUILD_HITBOX name="+name+r" left=",s):
        raise SystemExit("LIOS_BUILD_HITBOX_MISSING "+name)
print("LIOS_BUILD_ENTER_AND_TOUCH_ISOLATION_PASS frame="+m[1])
PY
# Coordinates are emitted from the live C++ RectTransforms, not guesses.
read -r foundation_x foundation_y wall_x wall_y child_x child_y rotate_x rotate_y place_x place_y exit_x exit_y < <(python3 - "$slot6_x" "$slot6_y" <<'PY'
import json,re,sys
from pathlib import Path
s=Path("evidence/build-equip-logcat.txt").read_text()
startup=Path("evidence/terrain-startup-logcat.txt").read_text()
quality=json.loads(Path("evidence/frame-quality.json").read_text())
heartbeat=re.search(r"LIOS_RENDER_HEARTBEAT frame=1 screen=(\d+)x(\d+)",startup)
touch=re.search(r"LIOS_HUD_TOUCH_DOWN finger=\d+ action=20 source_x=([-\d.]+) source_y=([-\d.]+)",s)
if not heartbeat or not touch:raise SystemExit("LIOS_BUILD_VIEWPORT_CALIBRATION_MISSING")
game_w,game_h=map(int,heartbeat.groups())
source_x,source_y=map(float,touch.groups())
canvas_scale=min(game_w/1920.,game_h/1080.)
canvas_left=(game_w-1920*canvas_scale)/2
canvas_top=(game_h-1080*canvas_scale)/2
# Raylib uses logical 1280x720 while ADB screenshots are 1080x640.
# Android letterboxes the game viewport; sourceRect() returns LOGICAL pixels.
# Calibrate the physical touch origin using the observed slot-6 down event.
physical_scale=quality["width"]/game_w
logical_x=canvas_left+source_x*canvas_scale
logical_y=canvas_top+(source_y-200.)*canvas_scale
origin_x=int(sys.argv[1])-logical_x*physical_scale
origin_y=int(sys.argv[2])-logical_y*physical_scale
if abs(origin_x)>quality["width"]*.15 or abs(origin_y)>quality["height"]*.15:
    raise SystemExit("LIOS_BUILD_VIEWPORT_CALIBRATION_OUTLIER")
coords=[]
for name in ("foundation","wall","child0","rotate","place","exit"):
    m=re.search(r"LIOS_BUILD_HITBOX name="+name+
      r" left=([-\d.]+) top=([-\d.]+) right=([-\d.]+) bottom=([-\d.]+)",s)
    if not m:raise SystemExit("LIOS_BUILD_HITBOX_MISSING "+name)
    l,t,r,b=map(float,m.groups())
    if not (r>l and b>t):raise SystemExit("LIOS_BUILD_INVALID_HITBOX "+name)
    x=round((l+r)/2*physical_scale+origin_x)
    y=round((t+b)/2*physical_scale+origin_y)
    if not (0<=x<quality["width"] and 0<=y<quality["height"]):
        raise SystemExit("LIOS_BUILD_HITBOX_OFFSCREEN "+name)
    coords.extend((x,y))
Path("evidence/build-touch-calibration.json").write_text(json.dumps({
 "logical_size":[game_w,game_h],
 "physical_size":[quality["width"],quality["height"]],
 "physical_scale":physical_scale,
 "physical_origin":[origin_x,origin_y],
 "calibration_touch":"slot6 live HUD source coordinates",
 "hitbox_centers_physical":dict(zip(("foundation","wall","child0","rotate","place","exit"),
                                    [coords[i:i+2] for i in range(0,len(coords),2)]))
},indent=2)+"\n")
print(*coords)
PY
)
printf 'foundation=%s,%s wall=%s,%s child0=%s,%s rotate=%s,%s place=%s,%s exit=%s,%s\n' \
 "$foundation_x" "$foundation_y" "$wall_x" "$wall_y" "$child_x" "$child_y" \
 "$rotate_x" "$rotate_y" "$place_x" "$place_y" "$exit_x" "$exit_y" \
 > evidence/build-runtime-hitboxes.txt
tap foundation "$foundation_x" "$foundation_y" 'LIOS_BUILD_CATEGORY'
grep -E 'LIOS_BUILD_CATEGORY frame=[0-9]+ category=2 submenu=1' \
 evidence/build-foundation-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_FOUNDATION_CATEGORY_NOT_SELECTED; exit 61; }
tap child0 "$child_x" "$child_y" 'LIOS_BUILD_CHILD'
grep -E 'LIOS_BUILD_CHILD frame=[0-9]+ category=2 child=0 .*native_piece=0' \
 evidence/build-child0-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_FOUNDATION_CHILD_NOT_SELECTED; exit 62; }
tap place "$place_x" "$place_y" 'LIOS_BUILD_PLACE'
grep -E 'LIOS_BUILD_PLACE frame=[0-9]+ piece=0 placed=1' \
 evidence/build-place-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_FOUNDATION_NOT_PLACED; exit 63; }
adb exec-out screencap -p > evidence/build-after-place.png || true
# Continue with a second source-derived category. Place the first wall on
# the foundation, rotate the preview and place another wall on a new edge.
# This tests actual Android taps and the native placement state, not parity
# with the original Unity server.
tap wall "$wall_x" "$wall_y" 'LIOS_BUILD_CATEGORY'
grep -E 'LIOS_BUILD_CATEGORY frame=[0-9]+ category=3 submenu=1' \
 evidence/build-wall-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_WALL_CATEGORY_NOT_SELECTED; exit 65; }
tap wall_child0 "$child_x" "$child_y" 'LIOS_BUILD_CHILD'
grep -E 'LIOS_BUILD_CHILD frame=[0-9]+ category=3 child=0 source_id=24012 native_piece=1 source_angle=180' \
 evidence/build-wall_child0-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_WALL_CHILD_NOT_SELECTED; exit 66; }
tap wall_place "$place_x" "$place_y" 'LIOS_BUILD_PLACE'
grep -E 'LIOS_BUILD_PLACE frame=[0-9]+ piece=1 placed=2' \
 evidence/build-wall_place-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_WALL_NOT_PLACED; exit 67; }
grep -F 'LIOS_WALL_SWEEP_RUNTIME rotation=0 cross_blocked=1 short_escape_allowed=1 inward_blocked=1 outside_allowed=1' \
 evidence/build-wall_place-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_WALL_SWEEP_RUNTIME_FAILED; exit 69; }
echo LIOS_BUILD_WALL_SWEEP_RUNTIME_PASS rotation=0 | tee -a evidence/build-probe-result.txt
adb exec-out screencap -p > evidence/build-wall-before-rotate.png || true
tap rotate "$rotate_x" "$rotate_y" 'LIOS_BUILD_ROTATE'
python3 - <<'PY'
import re
from pathlib import Path
child=Path("evidence/build-wall_child0-logcat.txt").read_text()
rotation=Path("evidence/build-rotate-logcat.txt").read_text()
source=re.search(r"LIOS_BUILD_CHILD frame=\d+ category=3 child=0 .*source_angle=(\d+)",child)
change=re.search(r"LIOS_BUILD_ROTATE frame=\d+ before=(\d+) orientation=(\d+) source_angle=(\d+)",rotation)
if not source or not change:
    raise SystemExit("LIOS_BUILD_WALL_ROTATION_EVIDENCE_MISSING")
angle=int(source[1]);before,after,logged_angle=map(int,change.groups())
if angle!=180 or angle!=logged_angle:
    raise SystemExit("LIOS_BUILD_WALL_SOURCE_ANGLE_NOT_SUPPORTED angle=%d" % angle)
if after!=(before+angle//90)%4 or after==before:
    raise SystemExit("LIOS_BUILD_WALL_ROTATION_INCORRECT before=%d after=%d angle=%d" %
                     (before,after,angle))
print("LIOS_BUILD_WALL_ROTATION_PASS before=%d after=%d angle=%d" %
      (before,after,angle))
PY
adb exec-out screencap -p > evidence/build-wall-after-rotate.png || true
tap wall_rotated_place "$place_x" "$place_y" 'LIOS_BUILD_PLACE'
grep -E 'LIOS_BUILD_PLACE frame=[0-9]+ piece=1 placed=3' \
 evidence/build-wall_rotated_place-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_ROTATED_WALL_NOT_PLACED; exit 68; }
grep -F 'LIOS_WALL_SWEEP_RUNTIME rotation=2 cross_blocked=1 short_escape_allowed=1 inward_blocked=1 outside_allowed=1' \
 evidence/build-wall_rotated_place-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_ROTATED_WALL_SWEEP_RUNTIME_FAILED; exit 70; }
echo LIOS_BUILD_WALL_SWEEP_RUNTIME_PASS rotation=2 | tee -a evidence/build-probe-result.txt
adb exec-out screencap -p > evidence/build-wall-after-place.png || true
# Report measured screenshot deltas without claiming Unity visual parity.
python3 - <<'PY'
import json
from pathlib import Path
from PIL import Image, ImageChops
pairs=[
    ("rotation_preview","build-wall-before-rotate.png","build-wall-after-rotate.png"),
    ("rotated_placement","build-wall-after-rotate.png","build-wall-after-place.png")
]
result={}
for name,before,after in pairs:
    a=Image.open(Path("evidence")/before).convert("RGB")
    b=Image.open(Path("evidence")/after).convert("RGB")
    if a.size!=b.size:raise SystemExit("LIOS_BUILD_SCREENSHOT_SIZE_MISMATCH")
    changed=sum(1 for pixel in ImageChops.difference(a,b).getdata()
                if max(pixel)>30)
    result[name]={"resolution":list(a.size),"changed_pixels_rgb30":changed,
                  "changed_fraction_rgb30":changed/(a.width*a.height)}
Path("evidence/build-wall-visual-diff.json").write_text(
    json.dumps({"comparisons":result,"original_parity_proven":False},indent=2))
print("LIOS_BUILD_WALL_VISUAL_DIFF",json.dumps(result))
PY
tap exit "$exit_x" "$exit_y" 'LIOS_BUILD_EXIT'
grep -F 'LIOS_BUILD_MODE_TRANSITION enabled=0 blueprint=1' \
 evidence/build-exit-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_EXIT_STATE_INVALID; exit 64; }
adb exec-out screencap -p > evidence/build-after-exit.png || true
# Preserve ordered, real logcat fragments for the generic EdgeAI auditor.
cat evidence/build-craft_open-logcat.txt evidence/build-prototype-logcat.txt \
 evidence/build-queue-logcat.txt evidence/build-blueprint-ready-logcat.txt \
 evidence/build-craft_close-logcat.txt evidence/build-equip-logcat.txt \
 evidence/build-foundation-logcat.txt evidence/build-child0-logcat.txt \
 evidence/build-place-logcat.txt evidence/build-wall-logcat.txt \
 evidence/build-wall_child0-logcat.txt evidence/build-wall_place-logcat.txt \
 evidence/build-rotate-logcat.txt evidence/build-wall_rotated_place-logcat.txt \
 evidence/build-exit-logcat.txt \
 > evidence/build-interaction-trace.txt
# After leaving BuildState, move the real avatar with the Android touch
# joystick. The runtime records actual player positions and the collision
# decision; this is NOT a synthetic call to blocksMotion().
read -r joy_x joy_y joy_end_y < <(python3 - <<'PY'
import json,re
from pathlib import Path
s=Path("evidence/terrain-startup-logcat.txt").read_text()
m=re.search(r"LIOS_JOYSTICK_CENTER logical_x=([-\d.]+) logical_y=([-\d.]+) radius=([-\d.]+)",s)
if not m:raise SystemExit("LIOS_JOYSTICK_CENTER_MISSING")
x,y,r=map(float,m.groups())
d=json.loads(Path("evidence/build-touch-calibration.json").read_text())
scale=d["physical_scale"];ox,oy=d["physical_origin"];w,h=d["physical_size"]
sx=round(ox+x*scale);sy=round(oy+y*scale)
ey=round(sy-r*scale*.9)
if not (0<=sx<w and 0<=sy<h and 0<=ey<h and ey<sy):
    raise SystemExit("LIOS_JOYSTICK_TOUCH_OUTSIDE_VIEWPORT")
print(sx,sy,ey)
PY
)
printf 'joystick_start=%s,%s joystick_end=%s,%s\n' \
 "$joy_x" "$joy_y" "$joy_x" "$joy_end_y" > evidence/build-joystick-coordinates.txt
adb logcat -c
adb shell input swipe "$joy_x" "$joy_y" "$joy_x" "$joy_end_y" 5000
sleep 1
adb logcat -d -s raylib:I > evidence/build-joystick-motion-logcat.txt 2>&1 || true
python3 - <<'PY'
import json,math,re
from pathlib import Path
s=Path("evidence/build-joystick-motion-logcat.txt").read_text()
pattern=re.compile(
 r"LIOS_JOYSTICK_MOTION frame=(\d+) start_x=([-\d.]+) start_z=([-\d.]+)"
 r" target_x=([-\d.]+) target_z=([-\d.]+)"
 r" wall_blocked=(\d+) terrain_blocked=(\d+)"
 r" stick_x=([-\d.]+) stick_y=([-\d.]+) placed=(\d+)")
rows=[]
for m in pattern.finditer(s):
    f,x,z,tx,tz,blocked,terrain,sx,sy,placed=m.groups()
    rows.append(dict(frame=int(f),x=float(x),z=float(z),target_x=float(tx),
        target_z=float(tz),blocked=int(blocked),terrain=int(terrain),
        stick_x=float(sx),stick_y=float(sy),placed=int(placed)))
colliders=[]
for label in ("wall_place","wall_rotated_place"):
    log=Path("evidence/build-"+label+"-logcat.txt").read_text()
    m=re.search(r"LIOS_WALL_COLLIDER_RUNTIME rotation=(\d+) center_x=([-\d.]+) center_z=([-\d.]+) half_x=([-\d.]+) half_z=([-\d.]+)",log)
    if not m:raise SystemExit("LIOS_JOYSTICK_COLLIDER_GEOMETRY_MISSING "+label)
    rotation,cx,cz,hx,hz=m.groups()
    colliders.append({"rotation":int(rotation),"center":[float(cx),float(cz)],
                      "half_extents":[float(hx),float(hz)]})
Path("evidence/build-joystick-motion.json").write_text(
    json.dumps({"records":rows,"colliders":colliders,
                "source":"actual Android touch joystick and player motion",
                "original_parity_proven":False},indent=2)+"\n")
if len(rows)<5:raise SystemExit("LIOS_JOYSTICK_TELEMETRY_INSUFFICIENT")
if any(b["frame"]<=a["frame"] for a,b in zip(rows,rows[1:])):
    raise SystemExit("LIOS_JOYSTICK_FRAME_ORDER_INVALID")
if any(r["placed"]<3 for r in rows):
    raise SystemExit("LIOS_JOYSTICK_WALLS_MISSING")
free=[r for r in rows if not r["blocked"] and not r["terrain"]]
blocked=[r for r in rows if r["blocked"] and not r["terrain"]]
travel=math.hypot(rows[-1]["x"]-rows[0]["x"],rows[-1]["z"]-rows[0]["z"])
if len(free)<2 or travel<.15:
    raise SystemExit("LIOS_JOYSTICK_NO_CONFIRMED_MOVEMENT free=%d travel=%.3f"%(len(free),travel))
if len(blocked)<2:
    raise SystemExit("LIOS_JOYSTICK_WALL_CONTACT_NOT_OBSERVED blocked=%d"%len(blocked))
contact_drift=max(math.hypot(r["x"]-blocked[0]["x"],r["z"]-blocked[0]["z"]) for r in blocked)
if contact_drift>.12:
    raise SystemExit("LIOS_JOYSTICK_CONTACT_POSITION_UNSTABLE drift=%.3f"%contact_drift)
if not all(math.hypot(r["target_x"]-r["x"],r["target_z"]-r["z"])>0.0001 for r in blocked):
    raise SystemExit("LIOS_JOYSTICK_BLOCKED_WITHOUT_MOVEMENT_INPUT")
print("LIOS_JOYSTICK_WALL_CONTACT_PASS samples=%d free=%d blocked=%d travel=%.3f drift=%.3f"%
      (len(rows),len(free),len(blocked),travel,contact_drift))
PY
adb exec-out screencap -p > evidence/build-joystick-wall-contact.png || true
echo LIOS_JOYSTICK_WALL_CONTACT_PASS | tee -a evidence/build-probe-result.txt

echo LIOS_ANDROID_BUILDER_FOUNDATION_WALL_ROTATE_PLACE_EXIT_PASS | tee -a evidence/build-probe-result.txt
