#!/usr/bin/env bash
# Observe overlay window of a standalone JNI test app above the unmodified
# Oxide loading Activity. This DOES NOT acquire player positions or bypass checks.
set -euo pipefail
if [[ $# -ne 4 ]]; then
  echo "Usage: $0 ADB_SERIAL APK_DIRECTORY DIAGNOSTIC_APK OUTPUT_DIRECTORY" >&2
  exit 64
fi
SERIAL="$1"
ORIGINALS="$2"
PROBE_APK="$3"
OUTPUT="$4"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
GAME="com.catsbit.oxidesurvivalisland"
PROBE="com.oxide.bridgeprobe"
mkdir -p "$OUTPUT"
python3 "$ROOT/tools/verify_original_splits.py" "$ORIGINALS" \
  --output "$OUTPUT/original_hashes.json"
test -s "$PROBE_APK"
adb -s "$SERIAL" get-state | grep -qx device
adb -s "$SERIAL" install-multiple -r "$ORIGINALS/base.apk" \
  "$ORIGINALS/split_config.arm64_v8a.apk" \
  "$ORIGINALS/split_config.xxhdpi.apk" \
  "$ORIGINALS/split_UnityDataAssetPack.apk" > "$OUTPUT/game_install.txt" 2>&1
grep -q '^Success' "$OUTPUT/game_install.txt"
adb -s "$SERIAL" install -r "$PROBE_APK" > "$OUTPUT/probe_install.txt" 2>&1
grep -q '^Success' "$OUTPUT/probe_install.txt"
adb -s "$SERIAL" shell am force-stop "$PROBE" || true
adb -s "$SERIAL" shell am force-stop "$GAME" || true
adb -s "$SERIAL" logcat -c 2>/dev/null || true
adb -s "$SERIAL" logcat -b all -v threadtime > "$OUTPUT/live_logcat.txt" 2>&1 &
LOG_PID=$!
close_log() {
  kill "$LOG_PID" 2>/dev/null || true
  wait "$LOG_PID" 2>/dev/null || true
}
trap close_log EXIT

# Explicit grant on our diagnostic APK only; no changes to original game.
adb -s "$SERIAL" shell appops set "$PROBE" SYSTEM_ALERT_WINDOW allow \
  > "$OUTPUT/overlay_permission.txt" 2>&1
adb -s "$SERIAL" shell am start -W \
  -n "$PROBE/.MainActivity" --ez overlay_probe true \
  > "$OUTPUT/overlay_launch.txt" 2>&1
sleep 2
adb -s "$SERIAL" exec-out screencap -p > "$OUTPUT/overlay_on_launcher.png" 2>/dev/null || true

# Start the signed, unmodified original while overlay service is running.
adb -s "$SERIAL" shell am start -W \
  -n "$GAME/cTOOT3.abKmV1.tP0Gr1.OxideActivity" \
  > "$OUTPUT/game_launch.txt" 2>&1 || true
cat "$OUTPUT/game_launch.txt"
adb -s "$SERIAL" shell pidof "$GAME" > "$OUTPUT/game_pid_capture_1.txt" 2>&1 || true
adb -s "$SERIAL" shell dumpsys activity activities > "$OUTPUT/activities_capture_1.txt" 2>&1 || true
adb -s "$SERIAL" shell dumpsys window > "$OUTPUT/window_capture_1.txt" 2>&1 || true
adb -s "$SERIAL" exec-out screencap -p > "$OUTPUT/overlay_on_game_1.png" 2>/dev/null || true
sleep 1
adb -s "$SERIAL" shell pidof "$GAME" > "$OUTPUT/game_pid_capture_2.txt" 2>&1 || true
adb -s "$SERIAL" shell dumpsys activity activities > "$OUTPUT/activities_capture_2.txt" 2>&1 || true
adb -s "$SERIAL" exec-out screencap -p > "$OUTPUT/overlay_on_game_2.png" 2>/dev/null || true
sleep 7
adb -s "$SERIAL" shell dumpsys activity exit-info "$GAME" > "$OUTPUT/game_exit_info.txt" 2>&1 || true

# Verify that all overlay pixels disappear when the diagnostic app is stopped.
adb -s "$SERIAL" shell am force-stop "$PROBE" > "$OUTPUT/probe_stop.txt" 2>&1 || true
sleep 1
adb -s "$SERIAL" exec-out screencap -p > "$OUTPUT/overlay_removed.png" 2>/dev/null || true
close_log
trap - EXIT
cp "$OUTPUT/live_logcat.txt" "$OUTPUT/all_buffers.txt"
python3 "$ROOT/tools/diagnose_original_exit.py" "$OUTPUT" \
  > "$OUTPUT/forensic_run_output.txt" 2>&1 || true

# The overlay proof requires the genuine game to have been resumed *at capture*.
if grep -Eq 'topResumedActivity=.*com[.]catsbit[.]oxidesurvivalisland/.*OxideActivity|mResumedActivity.*com[.]catsbit[.]oxidesurvivalisland/.*OxideActivity' \
   "$OUTPUT/activities_capture_1.txt" "$OUTPUT/activities_capture_2.txt"; then
  echo "ORIGINAL_ACTIVITY_RESUMED_DURING_CAPTURE=OBSERVED" | tee "$OUTPUT/original_foreground.txt"
else
  echo "ORIGINAL_ACTIVITY_RESUMED_DURING_CAPTURE=NOT_CONFIRMED" | tee "$OUTPUT/original_foreground.txt"
fi
if grep -q 'OXIDE_BRIDGE_OVERLAY.*RESULT=PASS MODE=OVERLAY_DIAGNOSTIC' "$OUTPUT/all_buffers.txt"; then
  echo "DIAGNOSTIC_OVERLAY_SERVICE_START=PASS" | tee "$OUTPUT/overlay_service.txt"
else
  echo "DIAGNOSTIC_OVERLAY_SERVICE_START=FAIL" | tee "$OUTPUT/overlay_service.txt"
fi
# Pixel verification happens in the caller, which decides whether the game
# stayed in front and whether the screenshot itself proved overlay composition.
echo "Evidence stored; do not confuse original-game loading overlay with ESP gameplay."
