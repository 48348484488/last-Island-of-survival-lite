#!/usr/bin/env bash
# Run our independent signed JNI CPU-ID test on an official Google Play AVD.
# No original Oxide game code, player telemetry, security checks or APKs.
set -euo pipefail
if [[ $# -ne 2 ]]; then echo "Usage: $0 PROBE_APK OUTPUT_DIR" >&2; exit 64; fi
APK="$1"
OUT="$2"
test -s "$APK"
mkdir -p "$OUT"
timeout 120 adb wait-for-device
ready=0
for i in $(seq 1 30); do
  if [[ "$(timeout 5 adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r' || true)" == 1 ]] &&
     timeout 5 adb shell echo ADB_RESPONSIVE 2>/dev/null | grep -q ADB_RESPONSIVE; then
    ready=1
    break
  fi
  sleep 2
done
if [[ "$ready" != 1 ]]; then
  echo ANDROID_GUEST_ADB_NOT_STABLE > "$OUT/environment_status.txt"
  exit 1
fi
echo ANDROID_GUEST_ADB_RESPONSIVE > "$OUT/environment_status.txt"
adb shell getprop ro.build.version.sdk | tr -d '\r' > "$OUT/api.txt"
adb shell getprop ro.product.cpu.abilist | tr -d '\r' > "$OUT/abi.txt"
adb shell getprop ro.dalvik.vm.native.bridge | tr -d '\r' > "$OUT/native_bridge.txt"
adb shell pm list packages com.google.android.gms > "$OUT/gms_presence.txt" 2>&1 || true
# A single intact argument: no multi-shell trailing backslash ambiguity.
adb install -r "$APK" > "$OUT/install.txt" 2>&1
grep -q '^Success' "$OUT/install.txt"
adb logcat -b all -c 2>/dev/null || true
adb logcat -b all -v threadtime > "$OUT/logcat.txt" 2>&1 &
LOGGER=$!
stop_logger() {
  kill "$LOGGER" 2>/dev/null || true
  wait "$LOGGER" 2>/dev/null || true
}
trap stop_logger EXIT
adb shell am start -W -n com.oxide.bridgeprobe/.MainActivity \
  --ez cpu_id_probe true > "$OUT/activity_start.txt" 2>&1 || true
sleep 5
stop_logger
trap - EXIT
python3 tools/classify_cpu_id_probe.py "$OUT/logcat.txt" --out "$OUT"
python3 - "$OUT/cpu_id_probe_verdict.json" <<'PY'
import json,sys
result=json.load(open(sys.argv[1],encoding="utf-8"))
assert result["status"] in (
    "ORIGINAL_CRASH_SITE_MRS_RETURNED_IN_INDEPENDENT_APK",
    "INDEPENDENT_CPU_ID_MRS_SIGILL_ON_NATIVE_BRIDGE"
), result
print("INDEPENDENT_CPU_ID_VALID_RUNTIME_RESULT="+result["status"])
PY
