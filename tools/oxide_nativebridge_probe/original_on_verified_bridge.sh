#!/usr/bin/env bash
# One unchanged original Oxide trial only AFTER a separately verified ARM64 JNI
# native-bridge execution on this official x86_64 Android configuration.
# Does not alter APKs, Google packages, security signals or Android identity.
set -euo pipefail
REPORTS="${1:-reports}"
SERIAL="emulator-5554"
APP_PACKAGE="com.catsbit.oxidesurvivalisland"
APP_ACTIVITY="com.catsbit.oxidesurvivalisland.OxideActivity"
mkdir -p "$REPORTS"

collect_logcat() {
  adb -s "$SERIAL" logcat -d -v threadtime > "$REPORTS/logcat.txt" 2>&1 || true
}
trap collect_logcat EXIT

# Trust only the signed-off predecessor evidence, never mere ABI advertising.
test -f "$REPORTS/nativebridge-prerequisite.json"
python3 - "$REPORTS/nativebridge-prerequisite.json" <<'PY'
import json, sys
from pathlib import Path
d=json.loads(Path(sys.argv[1]).read_text())
assert d["status"] == "ARM64_JNI_EXECUTED_VIA_NATIVE_BRIDGE_ORIGINAL_GAME_UNTESTED"
assert d["arm64_jni_executed"] is True
assert d["original_oxide_gameplay_verified"] is False
print("Verified independent ARM64 JNI execution; original protected game untested.")
PY
adb -s "$SERIAL" shell getprop sys.boot_completed > "$REPORTS/boot.txt"
adb -s "$SERIAL" shell getprop ro.product.cpu.abi > "$REPORTS/abi.txt"
adb -s "$SERIAL" shell getprop ro.product.cpu.abilist64 > "$REPORTS/abilist64.txt"
adb -s "$SERIAL" shell getprop ro.dalvik.vm.native.bridge > "$REPORTS/native-bridge.txt"
adb -s "$SERIAL" shell pm path com.android.vending > "$REPORTS/play-store-pm-path.txt" 2>&1 || true
adb -s "$SERIAL" shell pm path com.google.android.gms > "$REPORTS/play-services-pm-path.txt" 2>&1 || true
python3 - "$REPORTS" <<'PY'
from pathlib import Path
import sys
r=Path(sys.argv[1])
read=lambda n:(r/n).read_text(errors="replace").strip()
assert read("boot.txt")=="1", "BLOCKED_X86_ANDROID_NOT_BOOTED"
assert read("abi.txt")=="x86_64", "BLOCKED_NOT_VALIDATED_X86_ARCH"
assert "arm64-v8a" in read("abilist64.txt").split(","), "BLOCKED_ARM64_ABI_NOT_ADVERTISED"
assert read("native-bridge.txt")=="libndk_translation.so", "BLOCKED_BRIDGE_LIBRARY_NOT_CONFIGURED"
assert read("play-store-pm-path.txt").startswith("package:"), "BLOCKED_PLAY_STORE_PACKAGE_MISSING"
assert read("play-services-pm-path.txt").startswith("package:"), "BLOCKED_PLAY_SERVICES_PACKAGE_MISSING"
print("Original APK prerequisites verified; Play Integrity still unobserved.")
PY

# If a different x86_64 guest can't pass, do not download ~900MB Oxide assets.
echo "VERIFIED_BRIDGE_GUEST_DOWNLOADING_UNMODIFIED_SPLITS" > "$REPORTS/trial-stage.txt"
gh release download v1.15.11966 --repo 48348484488/oxide \
  --pattern split_UnityDataAssetPack.apk --dir .
python3 tools/audit_original_split_hashes.py --output "$REPORTS/original-apk-inventory.json"
python3 tools/oxide_original_hash_gate.py \
  --inventory "$REPORTS/original-apk-inventory.json" \
  --output "$REPORTS/original-sha256-verdict.json"

aapt_bin="$(command -v aapt || true)"
if [ -z "$aapt_bin" ] && [ -n "${ANDROID_HOME:-${ANDROID_SDK_ROOT:-}}" ]; then
  aapt_bin="$(find "${ANDROID_HOME:-${ANDROID_SDK_ROOT:-}}/build-tools" -type f -name aapt -print -quit 2>/dev/null || true)"
fi
test -n "$aapt_bin"
"$aapt_bin" dump badging base.apk > "$REPORTS/apk-badging.txt"
grep -Fq "package: name='$APP_PACKAGE'" "$REPORTS/apk-badging.txt"
grep -Fq "launchable-activity: name='$APP_ACTIVITY'" "$REPORTS/apk-badging.txt"

python3 tools/oxide_process_timeout.py --seconds 300 -- \
  adb -s "$SERIAL" install-multiple -r \
  base.apk split_config.arm64_v8a.apk split_config.xxhdpi.apk \
  split_UnityDataAssetPack.apk > "$REPORTS/install.txt" 2>&1
grep -Fxq 'Success' "$REPORTS/install.txt"
echo 'FOUR_UNCHANGED_ORIGINAL_APKS_INSTALLED' > "$REPORTS/trial-stage.txt"
adb -s "$SERIAL" shell am force-stop "$APP_PACKAGE" || true
adb -s "$SERIAL" logcat -c || true

# Launch alongside true 0..4/10/35s Activity-owned PID/PNG sampling.
python3 tools/oxide_capture_samples.py \
  --reports "$REPORTS" --package "$APP_PACKAGE" --serial "$SERIAL" \
  --launch-activity "$APP_PACKAGE/$APP_ACTIVITY"
collect_logcat
python3 tools/oxide_exit_timeline.py "$REPORTS/logcat.txt" \
  --output "$REPORTS/oxide-exit-timeline.json"
python3 tools/oxide_runtime_assess.py "$REPORTS"
