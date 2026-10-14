#!/usr/bin/env bash
# Genuine ARM64 virtual Android, independent diagnostic app only.
set -euo pipefail
if [[ $# -ne 3 ]]; then echo "Usage: $0 SERIAL APK DIR" >&2; exit 64; fi
S="$1"; APK="$2"; OUT="$3"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PKG=com.oxide.bridgeprobe
ACTION="$PKG.SYNTHETIC_FEED"
RX="$PKG/.SyntheticFeedReceiver"
mkdir -p "$OUT"
test -s "$APK"
adb -s "$S" get-state | grep -qx device
adb -s "$S" shell getprop ro.product.cpu.abilist | grep -q arm64-v8a
adb -s "$S" shell pm list packages com.catsbit.oxidesurvivalisland > "$OUT/multi_game_packages.txt"
if grep -q '^package:com.catsbit.oxidesurvivalisland$' "$OUT/multi_game_packages.txt"; then
  echo "Original game present; isolated test refused" >&2; exit 65
fi
adb -s "$S" install -r "$APK" | grep -q '^Success'
adb -s "$S" shell appops set "$PKG" SYSTEM_ALERT_WINDOW allow
adb -s "$S" shell am force-stop "$PKG" >/dev/null 2>&1 || true
adb -s "$S" shell input keyevent KEYCODE_HOME >/dev/null 2>&1 || true
adb -s "$S" logcat -b all -c >/dev/null 2>&1 || true
adb -s "$S" shell am start -W -n "$PKG/.MainActivity" \
  --ez overlay_probe true --ez overlay_feed true > "$OUT/multi_launch.txt"
grep -q 'Status: ok' "$OUT/multi_launch.txt"
wait_log() {
  for i in $(seq 1 60); do
    if adb -s "$S" logcat -b main -v threadtime -d \
      -s OXIDE_BRIDGE_OVERLAY:I OXIDE_BRIDGE_FEED:I '*:S' 2>/dev/null | grep -Fq "$1"; then
      return 0
    fi
    sleep .1
  done
  echo "Missing log: $1" >&2; return 1
}
capture() {
  sleep .2
  adb -s "$S" shell pidof "$PKG" > "$OUT/multi_pid_$1.txt" 2>&1 || true
  adb -s "$S" exec-out screencap -p > "$OUT/multi_$1.png"
  test -s "$OUT/multi_$1.png"
}
feed() {
  adb -s "$S" shell am broadcast -a "$ACTION" -n "$RX" \
    --el seq "$1" --ef x -1.1 --el age_ms "$2" \
    --ef x2 1.1 --el age2_ms "$3" >/dev/null
}
wait_log 'RESULT=PASS FEED_READY SYNTHETIC_SHELL_ONLY'
feed 1 5 5
wait_log 'RESULT=PASS FEED_MULTI_SEQ=1 RECT_COUNT=2'
capture both
feed 2 5 600
wait_log 'RESULT=PASS FEED_MULTI_SEQ=2 RECT_COUNT=1'
capture one
feed 2 5 5
wait_log 'RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=2'
capture replay
adb -s "$S" shell am broadcast -a "$ACTION" -n "$RX" \
  --el seq 3 --ef x -1.1 --el age_ms 5 --ef x2 1.1 \
  --es age2_ms invalid >/dev/null
wait_log 'RESULT=DROP MALFORMED_SECOND_TARGET'
feed 3 600 600
wait_log 'RESULT=PASS FEED_MULTI_SEQ=3 RECT_COUNT=0'
capture empty
feed 4 5 5
wait_log 'RESULT=PASS FEED_MULTI_SEQ=4 RECT_COUNT=2'
capture restored
adb -s "$S" shell am broadcast -a "$ACTION" -n "$RX" \
  --el seq 5 --ez stop true >/dev/null
wait_log 'RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE'
capture stopped
adb -s "$S" logcat -b all -v threadtime -d > "$OUT/multi_logcat.txt"
python3 "$ROOT/esp_core/android_probe/check_virtual_feed_multi.py" "$OUT" \
  > "$OUT/multi_runtime_verdict.txt"
cat "$OUT/multi_runtime_verdict.txt"
