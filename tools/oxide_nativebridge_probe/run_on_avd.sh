#!/usr/bin/env bash
# Run only our isolated harmless ARM64 JNI diagnostic app in official x86_64 Android.
# The original Oxide APK is not downloaded, modified, installed, or launched.
set -euo pipefail
APK="${1:-reports/diagnostic.apk}"
REPORTS="${2:-reports}"
mkdir -p "$REPORTS"

# Preserve a machine-readable verdict and logcat even if 'adb install' or
# 'am start' fails under the native bridge; never fake a positive result.
finish_probe() {
  exit_code="$?"
  trap - EXIT
  adb logcat -d -v threadtime > "$REPORTS/diagnostic-logcat.txt" 2>&1 || true
  python3 tools/oxide_nativebridge_probe/assess_logcat.py "$REPORTS" \
    --output "$REPORTS/nativebridge-jni-verdict.json" > "$REPORTS/assess-output.txt" 2>&1 || true
  cat "$REPORTS/assess-output.txt" || true
  exit "$exit_code"
}
trap finish_probe EXIT
test -f "$APK"
adb shell getprop sys.boot_completed > "$REPORTS/boot.txt"
adb shell getprop ro.product.cpu.abi > "$REPORTS/abi.txt"
adb shell getprop ro.product.cpu.abilist64 > "$REPORTS/abilist64.txt"
adb shell getprop ro.dalvik.vm.native.bridge > "$REPORTS/native-bridge.txt"
adb shell pm path com.android.vending > "$REPORTS/play-store-pm-path.txt" 2>&1 || true
adb shell pm path com.google.android.gms > "$REPORTS/play-services-pm-path.txt" 2>&1 || true

# Do not install test package if the initial bridge prerequisites are missing.
python3 - "$REPORTS" <<'PY'
from pathlib import Path
import sys
r = Path(sys.argv[1])
read = lambda name: (r / name).read_text(errors="replace").strip()
checks = {
    "boot": read("boot.txt") == "1",
    "x86_64": read("abi.txt") == "x86_64",
    "arm64_advertised": "arm64-v8a" in read("abilist64.txt").split(","),
    "bridge_configured": read("native-bridge.txt") == "libndk_translation.so",
}
print("Bridge preflight:", checks)
(r / "pre-install-bridge-gate.txt").write_text(str(checks) + "\n")
if not all(checks.values()):
    raise SystemExit("BLOCKED_NATIVE_BRIDGE_PREREQUISITES")
PY

adb logcat -c || true
adb install -r "$APK" > "$REPORTS/diagnostic-install.txt" 2>&1
grep -Fxq "Success" "$REPORTS/diagnostic-install.txt"
adb shell am force-stop org.oxideprobe || true
adb shell am start -W -n org.oxideprobe/.ProbeActivity \
  > "$REPORTS/diagnostic-launch.txt" 2>&1
sleep 3
adb shell pidof org.oxideprobe > "$REPORTS/diagnostic-pid.txt" 2>&1 || true
# Final verifier's exit code is meaningful. The EXIT trap re-captures
# logcat and preserves the verdict even on failure.
adb logcat -d -v threadtime > "$REPORTS/diagnostic-logcat.txt" 2>&1 || true
python3 tools/oxide_nativebridge_probe/assess_logcat.py "$REPORTS"
