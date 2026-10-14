#!/usr/bin/env bash
# Real native ARM64 Android WindowManager continuous-state regression.
# A single already-built independent JNI app; ORIGINAL OXIDE GAME ABSENT.
set -euo pipefail
if [[ $# -ne 3 ]]; then
  echo "Usage: $0 ADB_SERIAL STANDALONE_APK OUTPUT_DIRECTORY" >&2; exit 64
fi
SERIAL="$1"; APK="$2"; OUT="$3"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PKG="com.oxide.bridgeprobe"
GAME="com.catsbit.oxidesurvivalisland"
mkdir -p "$OUT"
test -s "$APK"
adb -s "$SERIAL" get-state | grep -qx device
adb -s "$SERIAL" shell getprop ro.product.cpu.abilist > "$OUT/sequence_abi.txt"
grep -q arm64-v8a "$OUT/sequence_abi.txt"
adb -s "$SERIAL" shell pm list packages "$GAME" > "$OUT/sequence_game_packages.txt"
if grep -q "^package:$GAME$" "$OUT/sequence_game_packages.txt"; then
  echo "Original game installed: refusing isolated overlay test" >&2
  exit 65
fi
adb -s "$SERIAL" install -r "$APK" > "$OUT/sequence_install.txt"
grep -q '^Success' "$OUT/sequence_install.txt"
adb -s "$SERIAL" shell appops set "$PKG" SYSTEM_ALERT_WINDOW allow \
  > "$OUT/sequence_permission.txt" 2>&1
adb -s "$SERIAL" shell am force-stop "$PKG" >/dev/null 2>&1 || true
adb -s "$SERIAL" shell input keyevent KEYCODE_HOME >/dev/null 2>&1 || true
adb -s "$SERIAL" logcat -b all -c >/dev/null 2>&1 || true
adb -s "$SERIAL" shell am start -W -n "$PKG/.MainActivity" \
  --ez overlay_probe true --ez overlay_sequence true \
  > "$OUT/sequence_launch.txt" 2>&1
grep -q 'Status: ok' "$OUT/sequence_launch.txt"

# Synchronize screenshots against actual Service log markers, not arbitrary
# sleeps. Each stage lasts 3 seconds for reproducible image capture.
for phase in INITIAL MOVED STALE RESTORED STOPPED; do
  marker="RESULT=PASS PHASE=$phase "
  phase_lower="$(echo "$phase" | tr '[:upper:]' '[:lower:]')"
  found=0
  for attempt in $(seq 1 55); do
    if adb -s "$SERIAL" logcat -b main -d -v threadtime -s OXIDE_BRIDGE_OVERLAY:I '*:S' \
          2>/dev/null | grep -Fq "$marker"; then
      found=1; break
    fi
    sleep 0.10
  done
  if [[ "$found" != 1 ]]; then
    echo "Missing actual JNI/WindowManager phase $phase" >&2
    adb -s "$SERIAL" logcat -b all -d -v threadtime \
      > "$OUT/sequence_logcat.txt" 2>&1 || true
    exit 1
  fi
  sleep 0.35
  adb -s "$SERIAL" shell pidof "$PKG" \
    > "$OUT/sequence_pid_$phase_lower.txt" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p \
    > "$OUT/sequence_$phase_lower.png"
  test -s "$OUT/sequence_$phase_lower.png"
done
adb -s "$SERIAL" logcat -b all -d -v threadtime \
  > "$OUT/sequence_logcat.txt" 2>&1
python3 "$ROOT/esp_core/android_probe/check_virtual_overlay_sequence.py" \
  "$OUT" > "$OUT/sequence_runtime_verdict.txt"
cat "$OUT/sequence_runtime_verdict.txt"
