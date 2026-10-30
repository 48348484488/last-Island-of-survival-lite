#!/usr/bin/env bash
# Real Android emulator smoke test. Never claim success from APK packaging alone.
set -Eeuo pipefail
mkdir -p evidence
capture_evidence() {
  local status=$?
  adb logcat -d -v threadtime > evidence/logcat.txt 2>&1 || true
  adb shell dumpsys activity activities > evidence/activities.txt 2>&1 || true
  if [[ ! -s evidence/runtime.png ]]; then
    adb exec-out screencap -p > evidence/runtime.png 2>/dev/null || true
  fi
  printf '%s\n' "$status" > evidence/script-exit-code.txt
}
trap capture_evidence EXIT

# sys.boot_completed alone is insufficient: the previous TCG-only emulator
# advertised boot completion while Android's package/settings services were
# absent. Never classify that infrastructure failure as a game crash.
ready=0
for attempt in $(seq 1 48); do
  boot="$(timeout 8s adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r' || true)"
  package="$(timeout 8s adb shell service check package 2>/dev/null || true)"
  if [[ "$boot" == "1" && "$package" == *"found"* && "$package" != *"not found"* ]]; then
    ready=1
    break
  fi
  sleep 5
done
printf 'boot_completed=%s\npackage_service=%s\n' "$boot" "$package" > evidence/android-service-readiness.txt
if [[ "$ready" -ne 1 ]]; then
  echo "ANDROID_FRAMEWORK_NOT_READY: package manager unavailable; APK was not installed" | tee evidence/framework-failure.txt
  exit 46
fi

adb shell getprop ro.product.cpu.abilist | tee evidence/device-abilist.txt
adb shell getprop ro.dalvik.vm.native.bridge | tee evidence/native-bridge.txt
if ! grep -q 'arm64-v8a' evidence/device-abilist.txt; then
  echo 'ARM64_ABI_UNAVAILABLE: no arm64-v8a support reported' | tee evidence/unsupported-abi.txt
  exit 42
fi
if grep -q 'x86_64' evidence/device-abilist.txt && [[ ! -s evidence/native-bridge.txt ]]; then
  echo 'ARM64_BRIDGE_UNCONFIRMED: x86_64 guest without native translation bridge' | tee evidence/unsupported-abi.txt
  exit 43
fi

APK="$(find dist -type f -name '*.apk' -print -quit)"
if [[ -z "$APK" || ! -s "$APK" ]]; then
  echo 'APK_MISSING' | tee evidence/install.txt
  exit 44
fi
installed=0
for attempt in 1 2 3 4 5; do
  if adb install -r "$APK" > evidence/install.txt 2>&1; then
    installed=1
    break
  fi
  sleep 10
done
cat evidence/install.txt
if [[ "$installed" -ne 1 ]]; then
  echo 'APK_INSTALL_FAILED'
  exit 45
fi
adb shell wm size 1920x1080
adb shell monkey -p org.lios.recreatedlite -c android.intent.category.LAUNCHER 1 | tee evidence/launch.txt
sleep 18
# A previous successful job captured a *solid green* 1080x640 PNG while
# the PID was alive. Do not report that as gameplay: wait for EndDrawing and
# reject low-color/incomplete screenshots, retaining the failed frame.
ready_frame=0
for attempt in $(seq 1 12); do
  if ! adb shell pidof org.lios.recreatedlite > evidence/pid.txt || [[ ! -s evidence/pid.txt ]]; then
    echo 'LIOS_GAME_PROCESS_EXITED_BEFORE_FIRST_FRAME' | tee evidence/frame-failure.txt
    exit 48
  fi
  if ! adb logcat -d -s raylib:I 2>/dev/null | grep -F 'LIOS_FIRST_FRAME_ENDDRAWING' >/dev/null; then
    echo "WAITING_FOR_FIRST_COMPLETE_FRAME attempt=$attempt" | tee -a evidence/frame-attempts.txt
    sleep 5
    continue
  fi
  if adb exec-out screencap -p > evidence/runtime-candidate.png &&
     python3 tools/android_frame_quality.py evidence/runtime-candidate.png \
       --min-colors 16 --report evidence/frame-quality.json >> evidence/frame-attempts.txt; then
    cp evidence/runtime-candidate.png evidence/runtime.png
    ready_frame=1
    echo "LIOS_NONFLAT_FRAME_CONFIRMED attempt=$attempt" | tee -a evidence/frame-attempts.txt
    break
  fi
  cp evidence/runtime-candidate.png evidence/last-incomplete-frame.png || true
  echo "WAITING_FOR_NONFLAT_FRAME attempt=$attempt" | tee -a evidence/frame-attempts.txt
  sleep 5
done
if [[ "$ready_frame" -ne 1 ]]; then
  # Keep diagnostic categories distinct: the game may still be swapping
  # frames while Android's emulator compositor captures only a flat buffer.
  adb logcat -d -s raylib:I > evidence/frame-heartbeats.txt 2>&1 || true
  heartbeat_count="$(grep -c 'LIOS_RENDER_HEARTBEAT frame=' evidence/frame-heartbeats.txt || true)"
  if [[ "$heartbeat_count" -gt 1 ]]; then
    echo "LIOS_FLAT_FRAME_WITH_ACTIVE_RENDER_LOOP heartbeats=$heartbeat_count" | tee evidence/render-loop-diagnosis.txt
  else
    echo "LIOS_FLAT_FRAME_RENDER_LOOP_NOT_CONFIRMED heartbeats=$heartbeat_count" | tee evidence/render-loop-diagnosis.txt
  fi
  echo 'LIOS_RENDERED_FRAME_NOT_READY: no nonflat frame after 12 attempts' | tee evidence/frame-failure.txt
  exit 47
fi
# This build must actually load and collide with the source Unity spawn tile.
# A nonflat screenshot alone could still be the old placeholder plane.
adb logcat -d -s raylib:I > evidence/terrain-startup-logcat.txt 2>&1 || true
python3 - <<'PY' > evidence/terrain-runtime-check.txt
import re
from pathlib import Path
log=Path("evidence/terrain-startup-logcat.txt").read_text(encoding="utf8",errors="replace")
if "LIOS_TERRAIN_SOURCE_TILE_READY" not in log:
    raise SystemExit("LIOS_TERRAIN_SOURCE_TILE_NOT_LOADED")
sectors=re.search(r"LIOS_TERRAIN_SECTORS_READY count=(\d+) total=5",log)
if sectors is None or int(sectors.group(1)) != 5:
    raise SystemExit("LIOS_TERRAIN_NEIGHBOR_SECTORS_NOT_READY")
# These four source heights were measured independently from the Unity-
# extracted OBJ triangles (EdgeAI source seam audit). A mere log marker is
# insufficient: require covered, finite collision values within 1cm of source
# geometry and less than a provisional 0.55m step across each seam.
import math
source_heights = {
    "north": (33.8332333178658, 34.005118328),
    "south": (23.622817541997698, 23.625942600571427),
    "east": (26.394343992758614, 26.355275537427623),
    "west": (8.310323519064955, 8.299384349133796),
}
seam_pattern = re.compile(
    r"LIOS_TERRAIN_SEAM_PROBE (north|south|east|west) "
    r"inner=(\S+) outer=(\S+) height_in=(\S+) height_out=(\S+)"
)
observed = {}
for match in seam_pattern.finditer(log):
    direction, inner, outer, hin_text, hout_text = match.groups()
    if direction in observed:
        raise SystemExit(f"LIOS_TERRAIN_SEAM_DUPLICATE {direction}")
    try:
        hin, hout = float(hin_text), float(hout_text)
    except ValueError:
        raise SystemExit(f"LIOS_TERRAIN_SEAM_INVALID_HEIGHT {direction}")
    if inner != "1" or outer != "1" or not math.isfinite(hin) or not math.isfinite(hout):
        raise SystemExit(f"LIOS_TERRAIN_SEAM_COLLISION_MISSING {direction}")
    observed[direction] = (hin, hout)
if set(observed) != set(source_heights):
    raise SystemExit(f"LIOS_TERRAIN_SEAM_DIRECTIONS_MISMATCH {sorted(observed)}")
for direction, (expected_in, expected_out) in source_heights.items():
    hin, hout = observed[direction]
    if abs(hin-expected_in) > 0.01 or abs(hout-expected_out) > 0.01:
        raise SystemExit(f"LIOS_TERRAIN_SEAM_SOURCE_MISMATCH {direction}")
    if abs(hout-hin) > 0.55:
        raise SystemExit(f"LIOS_TERRAIN_SEAM_STEP_TOO_HIGH {direction}")
    print(f"LIOS_TERRAIN_SEAM_SOURCE_MATCH {direction} delta={hout-hin:.5f}")
print("LIOS_TERRAIN_FIVE_SECTORS_AND_SEAM_PROBES_RECORDED")

# Verify all 20 independent cross-edge runtime collision probes, not just
# the four midpoint values. This is sampled terrain coverage, not a
# simulated movement test and not proof of Unity player traversal.
grid_pattern = re.compile(
    r"LIOS_TERRAIN_SEAM_GRID_PROBE (north|south|east|west) "
    r"offset=(\S+) inner=(\S+) outer=(\S+) "
    r"height_in=(\S+) height_out=(\S+)"
)
offsets = {-150, -75, 0, 75, 150}
expected_grid = {(direction, offset) for direction in source_heights for offset in offsets}
grid = {}
for line in log.splitlines():
    if "LIOS_TERRAIN_SEAM_GRID_PROBE" not in line:
        continue
    match = grid_pattern.search(line)
    if match is None:
        raise SystemExit("LIOS_TERRAIN_GRID_MALFORMED_EVENT")
    direction, offset_text, inner, outer, hin_text, hout_text = match.groups()
    try:
        offset = float(offset_text)
        hin, hout = float(hin_text), float(hout_text)
    except ValueError:
        raise SystemExit("LIOS_TERRAIN_GRID_INVALID_NUMBER")
    if not all(math.isfinite(v) for v in (offset, hin, hout)):
        raise SystemExit("LIOS_TERRAIN_GRID_NONFINITE_NUMBER")
    key = (direction, offset)
    if key in grid:
        raise SystemExit(f"LIOS_TERRAIN_GRID_DUPLICATE {key}")
    if inner != "1" or outer != "1":
        raise SystemExit(f"LIOS_TERRAIN_GRID_UNCOVERED {key}")
    if abs(hout-hin) > 0.55:
        raise SystemExit(f"LIOS_TERRAIN_GRID_STEP_TOO_HIGH {key} delta={hout-hin:.4f}")
    grid[key] = (hin, hout)
if set(grid) != expected_grid:
    raise SystemExit(f"LIOS_TERRAIN_GRID_MISSING_OR_EXTRA expected={sorted(expected_grid)} actual={sorted(grid)}")
print(f"LIOS_TERRAIN_GRID_20_PROBES_PASS count={len(grid)} max_step={max(abs(o-i) for i,o in grid.values()):.5f}")

if "LIOS_TERRAIN_MASK_MATERIAL_PREVIEW" not in log:
    raise SystemExit("LIOS_TERRAIN_MATERIAL_MASK_PREVIEW_NOT_LOADED")
m=re.search(r"LIOS_TERRAIN_SPAWN_GROUND y=([-+]?\d+(?:\.\d+)?) source=(\d+)",log)
if m is None or m.group(2)!="1":
    raise SystemExit("LIOS_TERRAIN_SPAWN_COLLISION_NOT_CONFIRMED")
height=float(m.group(1))
if abs(height-22.3035011)>0.1:
    raise SystemExit(f"LIOS_TERRAIN_SPAWN_HEIGHT_MISMATCH measured={height}")
print(f"LIOS_TERRAIN_SOURCE_SPAWN_CONFIRMED y={height:.5f}")
# Real source-tile smoke: the previous APK had a valid 3D character and
# collision but rendered no ground (78.34% exact sky RGB).
from PIL import Image
im=Image.open("evidence/runtime.png").convert("RGB")
sky=sum(1 for rgb in im.getdata() if rgb==(130,200,230))
fraction=sky/(im.width*im.height)
print(f"LIOS_TERRAIN_EXACT_SKY_FRACTION {fraction:.6f}")
if fraction>=0.70:
    raise SystemExit("LIOS_TERRAIN_GROUND_INVISIBLE_IN_SCREENSHOT")
PY
cat evidence/terrain-runtime-check.txt
# Two independent, source-grounded craft touch probes:
# (1) visible icon center from real 1080x640 gameplay screenshot;
# (2) serialized Unity btn_zhizao RectTransform center (61,1025) in 1920x1080.
# If only (2) works, the recreated HUD hitbox is displaced from its artwork.
read -r visible_x visible_y source_x source_y < <(python3 - <<'PY'
import json
d=json.load(open("evidence/frame-quality.json",encoding="utf-8"))
w,h=d["width"],d["height"]
scale=min(w/1920,h/1080)
left=(w-1920*scale)/2
top=(h-1080*scale)/2
print(round(w*34/1080),round(h*480/640),
      round(left+61*scale),round(top+1025*scale))
PY
)
printf 'visible=%s,%s source=%s,%s\n' "$visible_x" "$visible_y" \
  "$source_x" "$source_y" > evidence/craft-tap-coordinate.txt
# A stale startup/previous-touch marker is not proof of this touch. Clear the
# ring buffer immediately before the probe and preserve each probe's logs.
if ! adb logcat -c; then
  echo 'LIOS_TOUCH_LOG_RESET_FAILED' | tee evidence/craft-touch-result.txt
  exit 50
fi
adb shell input swipe "$visible_x" "$visible_y" "$visible_x" "$visible_y" 900
sleep 5
adb exec-out screencap -p > evidence/craft-after-visible-tap.png
adb logcat -d -s raylib:I > evidence/logcat-after-visible.txt 2>&1 || true
if grep -F 'LIOS_CRAFT_PANEL_OPEN' evidence/logcat-after-visible.txt >/dev/null; then
  cp evidence/craft-after-visible-tap.png evidence/craft-after-tap.png
  # Require both a fresh action event AND a substantial real screenshot
  # transition. Neither a stale log nor a changed frame alone is sufficient.
  if python3 tools/android_frame_quality.py evidence/craft-after-visible-tap.png \
      --compare-with evidence/runtime.png --min-changed-fraction 0.01 \
      --report evidence/craft-transition.json > evidence/craft-transition-check.txt; then
    echo 'LIOS_CRAFT_VISIBLE_ICON_TOUCH_CONFIRMED' | tee evidence/craft-touch-result.txt
  else
    echo 'LIOS_CRAFT_LOGGED_BUT_VISUAL_TRANSITION_UNCONFIRMED' | tee evidence/craft-touch-result.txt
  fi
else
  echo 'LIOS_CRAFT_VISIBLE_ICON_TOUCH_NOT_CONFIRMED' | tee evidence/craft-touch-result.txt
  if ! adb logcat -c; then
    echo 'LIOS_SOURCE_TOUCH_LOG_RESET_FAILED' | tee -a evidence/craft-touch-result.txt
    exit 50
  fi
  adb shell input swipe "$source_x" "$source_y" "$source_x" "$source_y" 900
  sleep 5
  adb exec-out screencap -p > evidence/craft-after-source-tap.png
  cp evidence/craft-after-source-tap.png evidence/craft-after-tap.png
  adb logcat -d -s raylib:I > evidence/logcat-after-source.txt 2>&1 || true
  if grep -F 'LIOS_CRAFT_PANEL_OPEN' evidence/logcat-after-source.txt >/dev/null; then
    echo 'LIOS_CRAFT_SOURCE_RECT_TOUCH_CONFIRMED_BUT_VISIBLE_TOUCH_FAILED' | tee -a evidence/craft-touch-result.txt
  else
    echo 'LIOS_CRAFT_SOURCE_RECT_TOUCH_NOT_CONFIRMED' | tee -a evidence/craft-touch-result.txt
  fi
fi
python3 tools/android_frame_quality.py evidence/craft-after-tap.png \
  --min-colors 16 --report evidence/craft-frame-quality.json > evidence/craft-frame-check.txt || true
# A functional smoke pass now requires the button VISIBLE to a user to open
# the menu. A successful tap at an invisible source-coordinate is a failure.
if ! grep -F 'LIOS_CRAFT_VISIBLE_ICON_TOUCH_CONFIRMED' evidence/craft-touch-result.txt >/dev/null; then
  echo 'LIOS_VISIBLE_CRAFT_BUTTON_FUNCTIONAL_FAILURE' | tee evidence/craft-visible-failure.txt
  exit 49
fi
# Follow-up diagnostic: the original-HUD backpack must open from its visible
# icon, and the same icon must close its own modal. This is intentionally
# evidence-only until a clean Android run verifies both touch coordinates.
# Never classify a missing event or unchanged frame as functional success.
if adb logcat -c &&
   adb shell input swipe 1010 35 1010 35 700; then
  sleep 2
  adb logcat -d -s raylib:I > evidence/craft-close-logcat.txt 2>&1 || true
  if grep -F 'LIOS_CRAFT_PANEL_CLOSE' evidence/craft-close-logcat.txt >/dev/null; then
    adb exec-out screencap -p > evidence/after-craft-close.png || true
    adb logcat -c || true
    if adb shell input swipe 289 454 289 454 700; then
      sleep 3
      adb exec-out screencap -p > evidence/inventory-open.png || true
      adb logcat -d -s raylib:I > evidence/inventory-open-logcat.txt 2>&1 || true
      if grep -F 'LIOS_INVENTORY_PANEL_OPEN' evidence/inventory-open-logcat.txt >/dev/null &&
         python3 tools/android_frame_quality.py evidence/inventory-open.png \
           --compare-with evidence/after-craft-close.png \
           --min-changed-fraction 0.01 --report evidence/inventory-transition.json \
           > evidence/inventory-transition-check.txt 2>&1; then
        echo 'LIOS_INVENTORY_VISIBLE_TOUCH_AND_TRANSITION_CONFIRMED' | tee evidence/inventory-probe.txt
        adb logcat -c || true
        if adb shell input swipe 289 454 289 454 700; then
          sleep 2
          adb logcat -d -s raylib:I > evidence/inventory-close-logcat.txt 2>&1 || true
          if grep -F 'LIOS_INVENTORY_PANEL_CLOSE' evidence/inventory-close-logcat.txt >/dev/null; then
            adb exec-out screencap -p > evidence/inventory-after-close.png || true
            if python3 tools/android_frame_quality.py evidence/inventory-after-close.png \
                --compare-with evidence/inventory-open.png \
                --min-changed-fraction 0.01 --report evidence/inventory-close-transition.json \
                > evidence/inventory-close-transition-check.txt 2>&1; then
              echo 'LIOS_INVENTORY_SAME_BUTTON_CLOSE_CONFIRMED' | tee -a evidence/inventory-probe.txt
            else
              echo 'LIOS_INVENTORY_CLOSE_VISUAL_TRANSITION_UNCONFIRMED' | tee -a evidence/inventory-probe.txt
            fi
          else
            echo 'LIOS_INVENTORY_SAME_BUTTON_CLOSE_UNCONFIRMED' | tee -a evidence/inventory-probe.txt
          fi
        fi
      else
        echo 'LIOS_INVENTORY_VISIBLE_TOUCH_UNCONFIRMED' | tee evidence/inventory-probe.txt
      fi
    fi
  else
    echo 'LIOS_CRAFT_CLOSE_UNCONFIRMED_IN_FOLLOWUP_PROBE' | tee evidence/inventory-probe.txt
  fi
fi
# Inventory is now a strict runtime gate: run #41054357399 independently
# confirmed fresh open/close markers and a 15.9356% screenshot change.
# Do not allow optional probe failures to masquerade as a smoke success.
if ! grep -Fxq 'LIOS_INVENTORY_VISIBLE_TOUCH_AND_TRANSITION_CONFIRMED' evidence/inventory-probe.txt; then
  echo 'LIOS_INVENTORY_OPEN_FUNCTIONAL_FAILURE' | tee evidence/inventory-strict-failure.txt
  exit 51
fi
if ! grep -Fxq 'LIOS_INVENTORY_SAME_BUTTON_CLOSE_CONFIRMED' evidence/inventory-probe.txt; then
  echo 'LIOS_INVENTORY_CLOSE_FUNCTIONAL_FAILURE' | tee evidence/inventory-strict-failure.txt
  exit 52
fi
# Settings open/close was independently confirmed in Android run #41061440637:
# fresh LIOS_SETTINGS_PANEL_OPEN/CLOSE events and 15.9346% changed pixels on
# opening. Require a new independent close screenshot transition as well.
if adb logcat -c && adb shell input swipe 1044 104 1044 104 700; then
  sleep 3
  adb exec-out screencap -p > evidence/settings-open.png || true
  adb logcat -d -s raylib:I > evidence/settings-open-logcat.txt 2>&1 || true
  if grep -F 'LIOS_SETTINGS_PANEL_OPEN' evidence/settings-open-logcat.txt >/dev/null &&
     python3 tools/android_frame_quality.py evidence/settings-open.png \
       --compare-with evidence/inventory-after-close.png \
       --min-changed-fraction 0.01 --report evidence/settings-transition.json \
       > evidence/settings-transition-check.txt 2>&1; then
    echo 'LIOS_SETTINGS_VISIBLE_TOUCH_AND_TRANSITION_CONFIRMED' | tee evidence/settings-probe.txt
    adb logcat -c || true
    if adb shell input swipe 1044 104 1044 104 700; then
      sleep 2
      adb logcat -d -s raylib:I > evidence/settings-close-logcat.txt 2>&1 || true
      if grep -F 'LIOS_SETTINGS_PANEL_CLOSE' evidence/settings-close-logcat.txt >/dev/null; then
        # A close marker alone is insufficient: capture the actual frame and
        # require the panel to visibly disappear before reporting success.
        adb exec-out screencap -p > evidence/settings-after-close.png || true
        if python3 tools/android_frame_quality.py evidence/settings-after-close.png \
            --compare-with evidence/settings-open.png \
            --min-changed-fraction 0.01 --report evidence/settings-close-transition.json \
            > evidence/settings-close-transition-check.txt 2>&1; then
          echo 'LIOS_SETTINGS_SAME_BUTTON_CLOSE_CONFIRMED' | tee -a evidence/settings-probe.txt
        else
          echo 'LIOS_SETTINGS_CLOSE_VISUAL_TRANSITION_UNCONFIRMED' | tee -a evidence/settings-probe.txt
        fi
      else
        echo 'LIOS_SETTINGS_SAME_BUTTON_CLOSE_UNCONFIRMED' | tee -a evidence/settings-probe.txt
      fi
    fi
  else
    echo 'LIOS_SETTINGS_VISIBLE_TOUCH_UNCONFIRMED' | tee evidence/settings-probe.txt
  fi
fi
# Settings is a strict runtime gate only after independent verification of
# the visible gear and its same-button close in a real emulator run.
if ! grep -Fxq 'LIOS_SETTINGS_VISIBLE_TOUCH_AND_TRANSITION_CONFIRMED' evidence/settings-probe.txt; then
  echo 'LIOS_SETTINGS_OPEN_FUNCTIONAL_FAILURE' | tee evidence/settings-strict-failure.txt
  exit 53
fi
if ! grep -Fxq 'LIOS_SETTINGS_SAME_BUTTON_CLOSE_CONFIRMED' evidence/settings-probe.txt; then
  echo 'LIOS_SETTINGS_CLOSE_FUNCTIONAL_FAILURE' | tee evidence/settings-strict-failure.txt
  exit 54
fi
# Probe real Unity source-derived attack/jump hitboxes, not invented pixels.
# A hitbox DOWN alone is insufficient: demand the gameplay transition too.
read -r jump_x jump_y attack_x attack_y < <(python3 - <<'PY'
import json,sys
sys.path.insert(0,"last_island_recreated_game")
from original_hud_bindings import original_touch_rects
d=json.load(open("evidence/frame-quality.json",encoding="utf8"))
w,h=d["width"],d["height"]
scale=min(w/1920.0,h/1080.0)
left=(w-1920*scale)/2
top=(h-1080*scale)/2
rects=original_touch_rects(profile="on-foot-melee")
coords=[]
for action in ("jump","attack"):
    r=rects[action]
    x=(r["left"]+r["right"])/2
    y=(r["top"]+r["bottom"])/2-200.0
    coords.extend((round(left+x*scale),round(top+y*scale)))
print(*coords)
PY
)
printf 'jump=%s,%s attack=%s,%s\n' "$jump_x" "$jump_y" "$attack_x" "$attack_y" \
  > evidence/gameplay-touch-coordinates.txt
for action in jump attack; do
  if [[ "$action" == jump ]]; then
    x="$jump_x"; y="$jump_y"; action_id=4; marker=LIOS_JUMP_TRIGGERED
  else
    x="$attack_x"; y="$attack_y"; action_id=3; marker=LIOS_ATTACK_TRIGGERED
  fi
  adb logcat -c
  adb shell input swipe "$x" "$y" "$x" "$y" 850
  sleep 2
  adb logcat -d -s raylib:I > "evidence/${action}-gameplay-logcat.txt" 2>&1 || true
  adb exec-out screencap -p > "evidence/${action}-after-touch.png" || true
  if grep -E "LIOS_HUD_TOUCH_DOWN finger=[0-9]+ action=${action_id} " \
       "evidence/${action}-gameplay-logcat.txt" >/dev/null &&
     grep -F "$marker" "evidence/${action}-gameplay-logcat.txt" >/dev/null; then
    echo "LIOS_GAMEPLAY_TOUCH_CONFIRMED action=$action" | tee -a evidence/gameplay-touch-result.txt
  else
    echo "LIOS_GAMEPLAY_TOUCH_FAILED action=$action x=$x y=$y" | tee -a evidence/gameplay-touch-result.txt
    exit 55
  fi
done
adb shell pidof org.lios.recreatedlite | tee evidence/pid.txt
test -s evidence/runtime.png
test -s evidence/pid.txt
# Keep real touch-down and frame pacing evidence. These measurements do not
# substitute for an original-game comparison or a controlled FPS benchmark.
adb logcat -d -s raylib:I > evidence/input-frame-logcat.txt 2>&1 || true
grep -E 'LIOS_(FRAME_TIMING|PHASE_TIMING|HUD_TOUCH_DOWN|TERRAIN_DRAW)' \
  evidence/input-frame-logcat.txt > evidence/input-frame-telemetry.txt || true
python3 - <<'PY'
import json
import re
from pathlib import Path
log=Path("evidence/input-frame-telemetry.txt").read_text(encoding="utf8")
rows=[]
for m in re.finditer(
    r"LIOS_FRAME_TIMING frames=(\d+) mean_ms=([\d.]+) p95_ms=([\d.]+) "
    r"max_ms=([\d.]+) slow_33ms=(\d+) mean_fps=([\d.]+)", log):
    rows.append({"frames":int(m[1]),"mean_ms":float(m[2]),
                 "p95_ms":float(m[3]),"max_ms":float(m[4]),
                 "slow_33ms":int(m[5]),"mean_fps":float(m[6])})
result={"observed_windows":len(rows),"observed_frames":sum(r["frames"] for r in rows),
        "windows":rows,"source":"real Android logcat",
        "baseline_comparison_performed":False,
        "original_game_parity_proven":False}
Path("evidence/frame-pacing-summary.json").write_text(
    json.dumps(result,indent=2)+"\n",encoding="utf8")
print("LIOS_FRAME_TIMING_EVIDENCE windows=%d frames=%d" %
      (result["observed_windows"],result["observed_frames"]))
PY
echo 'LIOS_ANDROID_VIRTUAL_ARM64_RUNTIME_SMOKE_PASS'
