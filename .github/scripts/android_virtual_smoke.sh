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
  echo 'LIOS_RENDERED_FRAME_NOT_READY: no nonflat frame after 12 attempts' | tee evidence/frame-failure.txt
  exit 47
fi
# A touch experiment is only a functional check when the game logs the real
# craft-panel transition; screenshot color variation alone is insufficient.
# Coordinates are grounded in the prior 1080x640 emulator HUD capture.
read -r tap_x tap_y < <(python3 - <<'PY'
import json
d=json.load(open("evidence/frame-quality.json",encoding="utf-8"))
print(round(d["width"]*1012/1080),round(d["height"]*103/640))
PY
)
printf '%s %s\n' "$tap_x" "$tap_y" > evidence/craft-tap-coordinate.txt
adb shell input swipe "$tap_x" "$tap_y" "$tap_x" "$tap_y" 900
sleep 5
adb exec-out screencap -p > evidence/craft-after-tap.png
python3 tools/android_frame_quality.py evidence/craft-after-tap.png \
  --min-colors 16 --report evidence/craft-frame-quality.json > evidence/craft-frame-check.txt || true
if adb logcat -d -s raylib:I 2>/dev/null | grep -F 'LIOS_CRAFT_PANEL_OPEN' >/dev/null; then
  echo 'LIOS_CRAFT_TOUCH_TRANSITION_CONFIRMED' | tee evidence/craft-touch-result.txt
else
  echo 'LIOS_CRAFT_TOUCH_NOT_CONFIRMED: screenshot captured for diagnosis' | tee evidence/craft-touch-result.txt
fi
adb shell pidof org.lios.recreatedlite | tee evidence/pid.txt
test -s evidence/runtime.png
test -s evidence/pid.txt
echo 'LIOS_ANDROID_VIRTUAL_ARM64_RUNTIME_SMOKE_PASS'
