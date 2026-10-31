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
# The visible crafting icon is already source-calibrated by the main smoke.
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
read -r foundation_x foundation_y wall_x wall_y child_x child_y rotate_x rotate_y place_x place_y exit_x exit_y < <(python3 - <<'PY'
import re
from pathlib import Path
s=Path("evidence/build-equip-logcat.txt").read_text()
coords=[]
for name in ("foundation","wall","child0","rotate","place","exit"):
    m=re.search(r"LIOS_BUILD_HITBOX name="+name+
      r" left=([-\d.]+) top=([-\d.]+) right=([-\d.]+) bottom=([-\d.]+)",s)
    if not m:raise SystemExit("LIOS_BUILD_HITBOX_MISSING "+name)
    l,t,r,b=map(float,m.groups())
    if not (r>l and b>t):raise SystemExit("LIOS_BUILD_INVALID_HITBOX "+name)
    coords.extend((round((l+r)/2),round((t+b)/2)))
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
tap exit "$exit_x" "$exit_y" 'LIOS_BUILD_EXIT'
grep -F 'LIOS_BUILD_MODE_TRANSITION enabled=0 blueprint=1' \
 evidence/build-exit-logcat.txt >/dev/null ||
 { echo LIOS_BUILD_EXIT_STATE_INVALID; exit 64; }
adb exec-out screencap -p > evidence/build-after-exit.png || true
echo LIOS_ANDROID_BUILDER_CRAFT_EQUIP_PLACE_EXIT_PASS | tee evidence/build-probe-result.txt
