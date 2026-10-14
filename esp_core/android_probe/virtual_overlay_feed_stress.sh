#!/usr/bin/env bash
# 64 real external synthetic snapshots through a single standalone Android
# JNI+WindowManager overlay on a native virtual ARM64 guest.
# NEVER installs or interacts with the original Oxide game.
set -euo pipefail
if [[ $# -ne 3 ]]; then
  echo "Usage: $0 ADB_SERIAL INDEPENDENT_APK OUTPUT_DIR" >&2; exit 64
fi
SERIAL="$1"; APK="$2"; OUT="$3"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
APP=com.oxide.bridgeprobe
GAME=com.catsbit.oxidesurvivalisland
ACTION="$APP.SYNTHETIC_FEED"
COMPONENT="$APP/.SyntheticFeedReceiver"
mkdir -p "$OUT"
test -s "$APK"
adb -s "$SERIAL" get-state | grep -qx device
adb -s "$SERIAL" shell getprop ro.product.cpu.abilist > "$OUT/stress_abi.txt"
grep -q arm64-v8a "$OUT/stress_abi.txt"
adb -s "$SERIAL" shell pm list packages "$GAME" > "$OUT/stress_game_packages.txt"
if grep -q "^package:$GAME$" "$OUT/stress_game_packages.txt"; then
  echo "Refuse: original Oxide installed in isolated test" >&2; exit 65
fi
adb -s "$SERIAL" install -r "$APK" > "$OUT/stress_install.txt"
grep -q '^Success' "$OUT/stress_install.txt"
adb -s "$SERIAL" shell appops set "$APP" SYSTEM_ALERT_WINDOW allow \
  > "$OUT/stress_permission.txt" 2>&1
adb -s "$SERIAL" shell am force-stop "$APP" >/dev/null 2>&1 || true
adb -s "$SERIAL" shell input keyevent KEYCODE_HOME >/dev/null 2>&1 || true
adb -s "$SERIAL" logcat -b all -c >/dev/null 2>&1 || true
adb -s "$SERIAL" shell am start -W -n "$APP/.MainActivity" \
  --ez overlay_probe true --ez overlay_feed true \
  > "$OUT/stress_launch.txt" 2>&1
grep -q 'Status: ok' "$OUT/stress_launch.txt"

wait_marker() {
  local marker="$1"
  for attempt in $(seq 1 65); do
    if adb -s "$SERIAL" logcat -b main -v threadtime -d \
          -s OXIDE_BRIDGE_OVERLAY:I OXIDE_BRIDGE_FEED:I '*:S' \
          2>/dev/null | grep -Fq "$marker"; then return 0; fi
    sleep .10
  done
  echo "No expected Android runtime evidence: $marker" >&2
  return 1
}
snapshot() {
  local stage="$1"
  sleep .35
  adb -s "$SERIAL" shell pidof "$APP" \
    > "$OUT/stress_pid_$stage.txt" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p > "$OUT/stress_$stage.png"
  test -s "$OUT/stress_$stage.png"
}
broadcast_sample() {
  local seq="$1" x="$2" age="$3"
  adb -s "$SERIAL" shell am broadcast -a "$ACTION" -n "$COMPONENT" \
    --el seq "$seq" --ef x "$x" --el age_ms "$age" \
    > /dev/null
}
wait_marker 'RESULT=PASS FEED_READY SYNTHETIC_SHELL_ONLY'

# No app restart. 64 strictly increasing caller-controlled JNI snapshots.
for seq in $(seq 1 64); do
  if ((seq%2)); then x="-0.75"; else x="0.75"; fi
  broadcast_sample "$seq" "$x" 5
  if ((seq==1)); then
    wait_marker 'RESULT=PASS FEED_SEQ=1 '
    snapshot first
  fi
done
wait_marker 'RESULT=PASS FEED_SEQ=64 '
snapshot last

# Malformed and incomplete inputs must not crash or consume sequence 65/68.
adb -s "$SERIAL" shell am broadcast -a "$ACTION" -n "$COMPONENT" \
  --el seq 65 --es x not-a-float --el age_ms 5 \
  > "$OUT/stress_bad_type_broadcast.txt"
wait_marker 'RESULT=DROP MALFORMED_FIELDS'
broadcast_sample 65 0.75 5
wait_marker 'RESULT=PASS FEED_SEQ=65 '

broadcast_sample 66 0.75 600
wait_marker 'RESULT=PASS FEED_SEQ=66 '
snapshot stale
broadcast_sample 66 -0.75 5
wait_marker 'RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=66'
snapshot replay
broadcast_sample 67 -0.75 5
wait_marker 'RESULT=PASS FEED_SEQ=67 '
snapshot restored

# No input for 4s. The watchdog must clear the window without killing Service.
wait_marker 'RESULT=PASS FEED_IDLE_TIMEOUT RECT_COUNT=0'
snapshot idle
adb -s "$SERIAL" shell am broadcast -a "$ACTION" -n "$COMPONENT" \
  --el seq 68 --el age_ms 5 > "$OUT/stress_missing_coordinate.txt"
wait_marker 'RESULT=DROP MISSING_REQUIRED_FIELDS SEQ=68'
adb -s "$SERIAL" shell am broadcast -a "$ACTION" -n "$COMPONENT" \
  --el seq 68 --ez stop true > "$OUT/stress_stop.txt"
wait_marker 'RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE'
snapshot stopped
adb -s "$SERIAL" logcat -b all -d -v threadtime \
  > "$OUT/stress_logcat.txt" 2>&1
python3 "$ROOT/esp_core/android_probe/check_virtual_feed_stress.py" \
  "$OUT" > "$OUT/stress_runtime_verdict.txt"
cat "$OUT/stress_runtime_verdict.txt"
