#!/usr/bin/env bash
# ADB batch protocol: eight laboratory-only entities with a caller-supplied camera.
# Original Oxide package must NOT be installed. No physical Android is used.
set -euo pipefail
[[ $# -eq 3 ]] || { echo "usage: $0 ADB_SERIAL APK OUT" >&2; exit 64; }
S="$1"; APK="$2"; OUT="$3"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PKG=com.oxide.bridgeprobe
RX="$PKG/.SyntheticFeedReceiver"
ACTION="$PKG.SYNTHETIC_FEED"
IDS="1,2,3,4,5,6,7,8"
XYZ="-3.5,0.35,-5,-3.5,-0.35,-5,-2.5,0.35,-5,-2.5,-0.35,-5,-1.5,0.35,-5,-1.5,-0.35,-5,-0.5,0.35,-5,-0.5,-0.35,-5,0.5,0.35,-5,0.5,-0.35,-5,1.5,0.35,-5,1.5,-0.35,-5,2.5,0.35,-5,2.5,-0.35,-5,3.5,0.35,-5,3.5,-0.35,-5"
AGES="5,5,5,5,5,5,5,5"
HALF_STALE="5,600,5,600,5,600,5,600"
CAMERA="1,0,0,0,0,1,0,0,0,0,-1.002002,-0.2002002,0,0,-1,0"
CAMERA_ZOOM="0.8,0,0,0,0,1,0,0,0,0,-1.002002,-0.2002002,0,0,-1,0"
mkdir -p "$OUT"
test -s "$APK"
adb -s "$S" get-state | grep -qx device
adb -s "$S" shell getprop ro.product.cpu.abilist | grep -q arm64-v8a
adb -s "$S" shell pm list packages com.catsbit.oxidesurvivalisland > "$OUT/batch_game_packages.txt"
! grep -q '^package:com.catsbit.oxidesurvivalisland$' "$OUT/batch_game_packages.txt" ||
  { echo "Original game installed; refusing isolated batch experiment" >&2; exit 65; }
adb -s "$S" install -r "$APK" > "$OUT/batch_install.txt"
grep -q '^Success' "$OUT/batch_install.txt"
adb -s "$S" shell appops set "$PKG" SYSTEM_ALERT_WINDOW allow >/dev/null
adb -s "$S" shell am force-stop "$PKG" >/dev/null 2>&1 || true
adb -s "$S" shell input keyevent KEYCODE_HOME >/dev/null 2>&1 || true
adb -s "$S" logcat -b all -c >/dev/null 2>&1 || true
adb -s "$S" shell am start -W -n "$PKG/.MainActivity" \
  --ez overlay_probe true --ez overlay_feed true > "$OUT/batch_launch.txt"
grep -q 'Status: ok' "$OUT/batch_launch.txt"
wait_marker() {
  for attempt in $(seq 1 80); do
    if adb -s "$S" logcat -b main -v threadtime -d \
       -s OXIDE_BRIDGE_OVERLAY:I OXIDE_BRIDGE_FEED:I '*:S' 2>/dev/null |
       grep -Fq "$1"; then return 0; fi
    sleep .10
  done
  echo "Missing ARM64 Android runtime marker: $1" >&2
  return 1
}
capture() {
  sleep .3
  adb -s "$S" shell pidof "$PKG" > "$OUT/batch_pid_$1.txt" 2>&1 || true
  adb -s "$S" exec-out screencap -p > "$OUT/batch_$1.png"
  test -s "$OUT/batch_$1.png"
}
send_batch() {
  local seq="$1" ids="$2" xyz="$3" ages="$4" camera="$5"
  adb -s "$S" shell am broadcast -a "$ACTION" -n "$RX" \
    --el seq "$seq" \
    --ela batch_ids "$ids" --efa batch_xyz "$xyz" \
    --ela batch_ages "$ages" --efa batch_camera "$camera" \
    --el batch_camera_age 5 --ei batch_width 1080 \
    --ei batch_height 1920 --ei batch_flags 0 >/dev/null
}
wait_marker 'RESULT=PASS FEED_READY SYNTHETIC_SHELL_ONLY'
send_batch 100 "$IDS" "$XYZ" "$AGES" "$CAMERA"
wait_marker 'RESULT=PASS FEED_BATCH_SEQ=100 INPUT_COUNT=8 RECT_COUNT=8'
capture all
send_batch 101 "$IDS" "$XYZ" "$HALF_STALE" "$CAMERA"
wait_marker 'RESULT=PASS FEED_BATCH_SEQ=101 INPUT_COUNT=8 RECT_COUNT=4'
capture half
send_batch 101 "$IDS" "$XYZ" "$AGES" "$CAMERA"
wait_marker 'RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=101'
capture replay
# Length mismatch must be rejected before JNI and must NOT consume sequence 102.
send_batch 102 "$IDS" "1,2,3,4,5,6" "$AGES" "$CAMERA"
wait_marker 'RESULT=DROP INVALID_BATCH_SHAPE SEQ=102'
# Duplicate native entity IDs must be rejected without drawing or consuming seq.
send_batch 102 "1,1,3,4,5,6,7,8" "$XYZ" "$AGES" "$CAMERA"
wait_marker 'RESULT=DROP INVALID_BATCH_PROJECTION SEQ=102'
# The valid sample with the SAME seq then succeeds using a different camera.
send_batch 102 "$IDS" "$XYZ" "$AGES" "$CAMERA_ZOOM"
wait_marker 'RESULT=PASS FEED_BATCH_SEQ=102 INPUT_COUNT=8 RECT_COUNT=8'
capture changed_camera
# A silent producer disconnect must clear all eight boxes, with no new packet.
wait_marker 'RESULT=PASS FEED_IDLE_TIMEOUT RECT_COUNT=0'
capture idle
adb -s "$S" shell am broadcast -a "$ACTION" -n "$RX" \
  --el seq 103 --ez stop true >/dev/null
wait_marker 'RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE'
capture stopped
adb -s "$S" logcat -b all -v threadtime -d > "$OUT/batch_logcat.txt"
python3 "$ROOT/esp_core/android_probe/check_virtual_feed_batch.py" "$OUT" \
  > "$OUT/batch_runtime_verdict.txt"
cat "$OUT/batch_runtime_verdict.txt"
