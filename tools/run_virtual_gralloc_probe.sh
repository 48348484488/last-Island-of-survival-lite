#!/usr/bin/env bash
# Runs only our standalone graphics app on an already-started native ARM64 Redroid.
set -euo pipefail
if [[ $# -ne 3 ]]; then echo "Usage: $0 ADB_SERIAL DIAGNOSTIC_APK OUT" >&2; exit 64; fi
SERIAL="$1"; APK="$2"; OUT="$3"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
mkdir -p "$OUT"
test -s "$APK"
adb -s "$SERIAL" get-state | grep -qx device
adb -s "$SERIAL" shell getprop ro.product.cpu.abilist \
  > "$OUT/abi.txt" 2>&1
grep -q arm64-v8a "$OUT/abi.txt"
adb -s "$SERIAL" shell getprop ro.build.version.sdk > "$OUT/api.txt" 2>&1
adb -s "$SERIAL" shell pm list packages com.catsbit.oxidesurvivalisland \
  > "$OUT/oxide_packages.txt" 2>&1
if grep -q '^package:com\.catsbit\.oxidesurvivalisland$' "$OUT/oxide_packages.txt"; then
  echo "Refusing probe: original game unexpectedly installed in independent test" >&2
  exit 65
fi
adb -s "$SERIAL" install -r "$APK" > "$OUT/install.txt" 2>&1
grep -q '^Success' "$OUT/install.txt"
adb -s "$SERIAL" logcat -b all -c >/dev/null 2>&1 || true
adb -s "$SERIAL" shell am start -W -n com.oxide.grallocprobe/.MainActivity \
  > "$OUT/start.txt" 2>&1
sleep 3
adb -s "$SERIAL" logcat -b main -b system -v threadtime -d \
  > "$OUT/logcat.txt" 2>&1
python3 "$ROOT/tools/classify_virtual_gralloc_probe.py" \
  "$OUT/logcat.txt" --out "$OUT"
