#!/usr/bin/env bash
# Unmodified Oxide splits on authorized physical Android via local/remote ADB.
set -euo pipefail
usage() { echo "Usage: $0 ADB_SERIAL APK_DIRECTORY PRIVATE_OUTPUT_DIRECTORY"; }
if [[ $# -eq 1 ]]; then
  if [[ "$1" == "--help" || "$1" == "-h" ]]; then usage; exit 0; fi
fi
if [[ $# -ne 3 ]]; then usage >&2; exit 64; fi
SERIAL="$1"
APK_DIR="$2"
OUTPUT="$3"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PKG="com.catsbit.oxidesurvivalisland"
ACTIVITY="cTOOT3.abKmV1.tP0Gr1.OxideActivity"
# Verify all four full SHA-256 digests against the repository's archived originals
# BEFORE calling ADB or installing anything. Missing or altered files fail closed.
command -v python3 >/dev/null || { echo "Missing python3" >&2; exit 69; }
mkdir -p "$OUTPUT"
python3 "$ROOT/tools/verify_original_splits.py" "$APK_DIR" \
  --output "$OUTPUT/apk_hashes.json"
# Only after all four file hashes pass may the runner contact ADB.
command -v adb >/dev/null || { echo "Missing adb" >&2; exit 69; }
adb -s "$SERIAL" get-state | grep -qx device || { echo "ADB target unavailable" >&2; exit 69; }
ABI="$(adb -s "$SERIAL" shell getprop ro.product.cpu.abilist | tr -d '\r')"
if [[ "$ABI" != *arm64-v8a* ]]; then echo "Not an ARM64 target: $ABI" >&2; exit 65; fi
printf 'ABI: %s\n' "$ABI" > "$OUTPUT/device.txt"
python3 "$ROOT/tools/audit_android_environment.py" --serial "$SERIAL" --out "$OUTPUT"
adb -s "$SERIAL" install-multiple -r \
  "$APK_DIR/base.apk" "$APK_DIR/split_config.arm64_v8a.apk" \
  "$APK_DIR/split_config.xxhdpi.apk" "$APK_DIR/split_UnityDataAssetPack.apk" \
  > "$OUTPUT/install.log" 2>&1
cat "$OUTPUT/install.log"
adb -s "$SERIAL" shell pm path "$PKG" > "$OUTPUT/installed_splits.txt"
# logcat can contain sensitive information; files must remain private until redacted.
adb -s "$SERIAL" logcat -b all -v threadtime > "$OUTPUT/live_logcat.txt" 2>&1 &
LOGCAT_PID=$!
cleanup() { kill "$LOGCAT_PID" 2>/dev/null || true; wait "$LOGCAT_PID" 2>/dev/null || true; }
trap cleanup EXIT
adb -s "$SERIAL" shell am force-stop "$PKG" || true
adb -s "$SERIAL" shell am start -W -n "$PKG/$ACTIVITY" > "$OUTPUT/launch.txt" 2>&1 || true
previous=0
for second in 3 10 35; do
  sleep "$((second-previous))"
  previous="$second"
  pid_file="$(printf 'pid_%ss.txt' "$second")"
  activity_file="$(printf 'activities_%ss.txt' "$second")"
  screen_file="$(printf 'screen_%ss.png' "$second")"
  adb -s "$SERIAL" shell pidof "$PKG" > "$OUTPUT/$pid_file" 2>&1 || true
  adb -s "$SERIAL" shell dumpsys activity activities > "$OUTPUT/$activity_file" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p > "$OUTPUT/$screen_file" 2>/dev/null || true
done
adb -s "$SERIAL" shell dumpsys activity exit-info "$PKG" > "$OUTPUT/exit_info.txt" 2>&1 || true
cleanup
trap - EXIT
cp "$OUTPUT/live_logcat.txt" "$OUTPUT/all_buffers.txt"
python3 "$ROOT/tools/diagnose_original_exit.py" "$OUTPUT" > "$OUTPUT/forensic_run_output.txt"
cat "$OUTPUT/forensic_summary.md"
# The existence of a package PID or a resumed activity on its own is not enough.
# Require continuity of the original PID at both checkpoints and no exit event.
if python3 "$ROOT/tools/check_activity_liveness.py" "$OUTPUT" \
     > "$OUTPUT/activity_verdict_stdout.txt"; then
  echo "ACTIVITY_SMOKE_PASS_NOT_GAMEPLAY" | tee "$OUTPUT/status.txt"
else
  cat "$OUTPUT/activity_verdict_stdout.txt"
  echo "ORIGINAL_GAME_ACTIVITY_SMOKE_FAILED" | tee "$OUTPUT/status.txt"
  exit 1
fi
