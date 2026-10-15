#!/usr/bin/env bash
# Run only unchanged original Oxide splits on already-booted official ARM64 Android.
# Never alter the APK, spoof attestation, modify Android identity or bypass security.
set -euo pipefail
SERIAL="${1:-emulator-5554}"
REPORTS="${2:-reports}"
mkdir -p "$REPORTS"
ADB=(adb -s "$SERIAL")
APP_PACKAGE="com.catsbit.oxidesurvivalisland"
APP_ACTIVITY="com.catsbit.oxidesurvivalisland.OxideActivity"

collect_logcat() {
  "${ADB[@]}" logcat -d -v threadtime > "$REPORTS/logcat.txt" 2>&1 || true
}
trap collect_logcat EXIT

"${ADB[@]}" get-state > "$REPORTS/adb-state.txt"
"${ADB[@]}" shell getprop sys.boot_completed > "$REPORTS/boot.txt"
"${ADB[@]}" shell getprop ro.product.cpu.abi > "$REPORTS/abi.txt"
"${ADB[@]}" shell getprop ro.build.version.release > "$REPORTS/android-version.txt"
"${ADB[@]}" shell pm path com.android.vending > "$REPORTS/play-store-pm-path.txt" 2>&1 || true
"${ADB[@]}" shell pm path com.google.android.gms > "$REPORTS/play-services-pm-path.txt" 2>&1 || true
python3 tools/oxide_managed_arm64_gate.py --reports "$REPORTS"
echo "ARM64_AND_GOOGLE_PACKAGES_PRESENT_NOT_CERTIFICATION" > "$REPORTS/trial-stage.txt"

# Fail before a large asset download unless the official device passed readiness.
gh release download v1.15.11966 --repo 48348484488/oxide \
  --pattern split_UnityDataAssetPack.apk --dir .
python3 tools/audit_original_split_hashes.py --output "$REPORTS/original-apk-inventory.json"
python3 tools/oxide_original_hash_gate.py \
  --inventory "$REPORTS/original-apk-inventory.json" \
  --output "$REPORTS/original-sha256-verdict.json"
echo "FOUR_UNCHANGED_APK_HASHES_VERIFIED" > "$REPORTS/trial-stage.txt"

AAPT="$(command -v aapt || true)"
if [ -z "$AAPT" ] && [ -n "${ANDROID_HOME:-}" ]; then
  AAPT="$(find "${ANDROID_HOME:-}/build-tools" -type f -name aapt -print -quit 2>/dev/null || true)"
fi
test -n "$AAPT"
"$AAPT" dump badging base.apk > "$REPORTS/apk-badging.txt"
PACKAGE="$(sed -n "s/^package: name='\([^']*\)'.*/\1/p" "$REPORTS/apk-badging.txt" | head -n 1)"
ACTIVITY="$(awk -F"'" '/^launchable-activity: name=/ {print $2; exit}' "$REPORTS/apk-badging.txt")"
test "$PACKAGE" = "$APP_PACKAGE"
test "$ACTIVITY" = "$APP_ACTIVITY"

python3 tools/oxide_process_timeout.py --seconds 300 -- "${ADB[@]}" install-multiple -r \
  base.apk split_config.arm64_v8a.apk split_config.xxhdpi.apk \
  split_UnityDataAssetPack.apk > "$REPORTS/install.txt" 2>&1
grep -q '^Success' "$REPORTS/install.txt"
echo "ORIGINAL_APK_INSTALLED" > "$REPORTS/trial-stage.txt"
"${ADB[@]}" shell am force-stop "$APP_PACKAGE" || true
"${ADB[@]}" logcat -c || true
# The capture clock starts at the same instant that am start -W is sent
# asynchronously. Blocking on -W previously discarded the crucial first seconds.
python3 tools/oxide_capture_samples.py --reports "$REPORTS" \
  --package "$APP_PACKAGE" --serial "$SERIAL" \
  --launch-activity "$APP_PACKAGE/$APP_ACTIVITY"
collect_logcat
python3 tools/oxide_exit_timeline.py "$REPORTS/logcat.txt" \
  --output "$REPORTS/oxide-exit-timeline.json"
python3 tools/oxide_runtime_assess.py "$REPORTS"
