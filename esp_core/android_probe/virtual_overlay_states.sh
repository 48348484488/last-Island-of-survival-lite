#!/usr/bin/env bash
# REAL native ARM64 Redroid validation, 100% virtual and NO original game.
# Test the actual Android WindowManager Service with controlled JNI samples.
set -euo pipefail
if [[ $# -ne 3 ]]; then
  echo "Usage: $0 ADB_SERIAL STANDALONE_APK OUTPUT_DIR" >&2
  exit 64
fi
SERIAL="$1"; APK="$2"; OUT="$3"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PACKAGE=com.oxide.bridgeprobe
GAME=com.catsbit.oxidesurvivalisland
mkdir -p "$OUT"
test -s "$APK"
adb -s "$SERIAL" get-state | grep -qx device
adb -s "$SERIAL" shell getprop ro.product.cpu.abilist > "$OUT/abis.txt"
grep -q arm64-v8a "$OUT/abis.txt"
adb -s "$SERIAL" shell getprop ro.build.version.sdk > "$OUT/api.txt"
# This experiment does not depend on the original game's security check.
adb -s "$SERIAL" shell pm list packages "$GAME" > "$OUT/game_packages.txt"
if grep -q "^package:$GAME$" "$OUT/game_packages.txt"; then
  echo "REFUSING: original game is installed in the isolated overlay experiment" >&2
  exit 65
fi
adb -s "$SERIAL" install -r "$APK" > "$OUT/install.txt"
grep -q '^Success' "$OUT/install.txt"
adb -s "$SERIAL" shell appops set "$PACKAGE" SYSTEM_ALERT_WINDOW allow \
  > "$OUT/permission.txt" 2>&1
adb -s "$SERIAL" logcat -b all -c > /dev/null 2>&1 || true

# These modes exercise the C ABI through JNI and actual TYPE_APPLICATION_OVERLAY,
# not merely a simulated Canvas screenshot.
for mode in fresh empty stale; do
  adb -s "$SERIAL" shell am force-stop "$PACKAGE" > "$OUT/stop_before_$mode.txt" 2>&1 || true
  adb -s "$SERIAL" shell input keyevent KEYCODE_HOME > /dev/null 2>&1 || true
  extra=""
  if [[ "$mode" == empty ]]; then extra="--ez overlay_no_boxes true"; fi
  if [[ "$mode" == stale ]]; then extra="--ez overlay_stale_boxes true"; fi
  adb -s "$SERIAL" shell am start -W \
    -n "$PACKAGE/.MainActivity" --ez overlay_probe true \
    $extra > "$OUT/launch_$mode.txt" 2>&1
  grep -q "Status: ok" "$OUT/launch_$mode.txt"
  sleep 2
  adb -s "$SERIAL" shell input keyevent KEYCODE_HOME >/dev/null 2>&1 || true
  sleep 1
  adb -s "$SERIAL" shell pidof "$PACKAGE" > "$OUT/pid_$mode.txt" 2>&1
  grep -Eq '[0-9]+' "$OUT/pid_$mode.txt"
  adb -s "$SERIAL" shell dumpsys window windows \
    > "$OUT/window_$mode.txt" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p > "$OUT/overlay_$mode.png"
  test -s "$OUT/overlay_$mode.png"
done
# Removing the service's owning process must remove the actual overlay window.
adb -s "$SERIAL" shell am force-stop "$PACKAGE" > "$OUT/stop_final.txt" 2>&1
sleep 2
adb -s "$SERIAL" shell input keyevent KEYCODE_HOME >/dev/null 2>&1 || true
adb -s "$SERIAL" exec-out screencap -p > "$OUT/overlay_removed.png"
test -s "$OUT/overlay_removed.png"
adb -s "$SERIAL" logcat -b all -d -v threadtime > "$OUT/logcat.txt" 2>&1

python3 "$ROOT/esp_core/android_probe/check_virtual_overlay_states.py" \
  "$OUT" > "$OUT/overlay_runtime_verdict.txt"
cat "$OUT/overlay_runtime_verdict.txt"
