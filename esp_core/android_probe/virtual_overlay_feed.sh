#!/usr/bin/env bash
# Drive a single standalone JNI WindowManager Service with external, synthetic
# ADB shell broadcasts on native virtual ARM64 Android. Original game absent.
set -euo pipefail
if [[ $# -ne 3 ]]; then
  echo "Usage: $0 ADB_SERIAL STANDALONE_APK OUTPUT_DIR" >&2; exit 64
fi
SERIAL="$1"; APK="$2"; OUT="$3"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PKG=com.oxide.bridgeprobe
GAME=com.catsbit.oxidesurvivalisland
RECEIVER="$PKG/.SyntheticFeedReceiver"
ACTION="$PKG.SYNTHETIC_FEED"
mkdir -p "$OUT"
test -s "$APK"
adb -s "$SERIAL" get-state | grep -qx device
adb -s "$SERIAL" shell getprop ro.product.cpu.abilist > "$OUT/sequence_abi.txt"
grep -q arm64-v8a "$OUT/sequence_abi.txt"
adb -s "$SERIAL" shell pm list packages "$GAME" > "$OUT/sequence_game_packages.txt"
if grep -q "^package:$GAME$" "$OUT/sequence_game_packages.txt"; then
  echo "Original game present; refuse isolated synthetic feed test" >&2; exit 65
fi
adb -s "$SERIAL" install -r "$APK" > "$OUT/sequence_install.txt"
grep -q '^Success' "$OUT/sequence_install.txt"
adb -s "$SERIAL" shell appops set "$PKG" SYSTEM_ALERT_WINDOW allow \
  > "$OUT/sequence_permission.txt" 2>&1
adb -s "$SERIAL" shell am force-stop "$PKG" >/dev/null 2>&1 || true
adb -s "$SERIAL" shell input keyevent KEYCODE_HOME >/dev/null 2>&1 || true
adb -s "$SERIAL" logcat -b all -c >/dev/null 2>&1 || true
adb -s "$SERIAL" shell am start -W -n "$PKG/.MainActivity" \
  --ez overlay_probe true --ez overlay_feed true > "$OUT/sequence_launch.txt"
grep -q 'Status: ok' "$OUT/sequence_launch.txt"

wait_log() {
  local marker="$1"
  for attempt in $(seq 1 45); do
    if adb -s "$SERIAL" logcat -b main -v threadtime -d -s OXIDE_BRIDGE_OVERLAY:I '*:S' \
      2>/dev/null | grep -Fq "$marker"; then return 0; fi
    sleep 0.12
  done
  echo "Required real Android log marker missing: $marker" >&2
  return 1
}
capture() {
  local phase="$1"
  local lower
  lower="$(echo "$phase" | tr '[:upper:]' '[:lower:]')"
  sleep .3
  adb -s "$SERIAL" shell pidof "$PKG" > "$OUT/sequence_pid_$lower.txt" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p > "$OUT/sequence_$lower.png"
  test -s "$OUT/sequence_$lower.png"
}
send_snapshot() {
  local seq="$1" x="$2" age="$3"
  adb -s "$SERIAL" shell am broadcast -a "$ACTION" -n "$RECEIVER" \
    --el seq "$seq" --ef x "$x" --el age_ms "$age" \
    > "$OUT/feed_sent_$seq.txt"
}
wait_log 'RESULT=PASS FEED_READY SYNTHETIC_SHELL_ONLY'
wait_log 'RESULT=PASS PHASE=INITIAL '
capture INITIAL
send_snapshot 1 1.0 5
wait_log 'RESULT=PASS FEED_SEQ=1 '
capture MOVED
send_snapshot 2 1.0 600
wait_log 'RESULT=PASS FEED_SEQ=2 '
capture STALE
send_snapshot 2 -1.0 5
wait_log 'RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=2'
capture REPLAY
send_snapshot 3 -1.0 5
wait_log 'RESULT=PASS FEED_SEQ=3 '
capture RESTORED
adb -s "$SERIAL" shell am broadcast -a "$ACTION" -n "$RECEIVER" \
    --el seq 4 --ez stop true > "$OUT/feed_sent_4.txt"
wait_log 'RESULT=PASS PHASE=STOPPED '
capture STOPPED
adb -s "$SERIAL" logcat -b all -v threadtime -d \
  > "$OUT/sequence_logcat.txt" 2>&1
python3 "$ROOT/esp_core/android_probe/check_virtual_overlay_sequence.py" \
  "$OUT" > "$OUT/sequence_runtime_verdict.txt"
python3 "$ROOT/esp_core/android_probe/check_virtual_feed_control.py" \
  "$OUT" > "$OUT/feed_runtime_verdict.txt"
cat "$OUT/feed_runtime_verdict.txt"
