#!/usr/bin/env bash
# Read-only smoke test of the unmodified Oxide APKs in a clean Android emulator.
set -u
mkdir -p smoke-output
PKG="com.catsbit.oxidesurvivalisland"
RESULT="NOT_STARTED"
trap 'printf "%s\n" "$RESULT" > smoke-output/status.txt; adb logcat -d -v threadtime -t 18000 > smoke-output/logcat.txt 2>/dev/null || true; adb shell dumpsys activity activities > smoke-output/activities.txt 2>/dev/null || true' EXIT

echo "Smoke test for the original, unmodified four APK splits" | tee smoke-output/README.txt
date -u +"UTC %Y-%m-%d %H:%M:%S" >> smoke-output/README.txt
adb devices -l | tee smoke-output/devices.txt
adb shell getprop ro.build.version.release | tr -d '\r' | tee smoke-output/android_version.txt
adb shell getprop ro.product.cpu.abilist | tr -d '\r' | tee smoke-output/abis.txt
ABILIST="$(cat smoke-output/abis.txt)"
if [[ "$ABILIST" != *"arm64-v8a"* ]]; then
  RESULT="UNSUPPORTED_ABI"
  echo "No arm64 native ABI bridge in emulator; cannot run native Oxide libraries." | tee -a smoke-output/README.txt
  exit 2
fi

echo "Installing original split APKs..." | tee -a smoke-output/README.txt
if ! adb install-multiple -r \
  base.apk split_config.arm64_v8a.apk split_config.xxhdpi.apk split_UnityDataAssetPack.apk \
  > smoke-output/install.log 2>&1; then
  RESULT="INSTALL_FAILED"
  cat smoke-output/install.log
  exit 3
fi
cat smoke-output/install.log
adb shell dumpsys package "$PKG" > smoke-output/package.txt 2>/dev/null || true
adb logcat -c 2>/dev/null || true
adb shell am force-stop "$PKG" >/dev/null 2>&1 || true
echo "Launching via Android launcher..." | tee -a smoke-output/README.txt
adb shell monkey -p "$PKG" -c android.intent.category.LAUNCHER 1 > smoke-output/launch.log 2>&1 || true
cat smoke-output/launch.log
sleep 15
PID1="$(adb shell pidof "$PKG" 2>/dev/null | tr -d '\r' || true)"
printf "%s\n" "$PID1" > smoke-output/pid_after_15s.txt
adb exec-out screencap -p > smoke-output/screen_15s.png 2>/dev/null || true
sleep 30
PID2="$(adb shell pidof "$PKG" 2>/dev/null | tr -d '\r' || true)"
printf "%s\n" "$PID2" > smoke-output/pid_after_45s.txt
adb exec-out screencap -p > smoke-output/screen_45s.png 2>/dev/null || true
adb logcat -d -v threadtime -t 18000 > smoke-output/logcat.txt 2>/dev/null || true
adb shell dumpsys activity activities > smoke-output/activities.txt 2>/dev/null || true

if [[ -n "$PID1" && -n "$PID2" ]]; then
  RESULT="RUNNING_AT_15S_AND_45S"
  echo "Process was observed at 15s and 45s. This does not guarantee a playable menu." | tee -a smoke-output/README.txt
elif [[ -n "$PID1" ]]; then
  RESULT="EXITED_BETWEEN_15S_AND_45S"
  echo "Process disappeared after first sample." | tee -a smoke-output/README.txt
else
  RESULT="NOT_RUNNING_AT_15S"
  echo "Process was not found after 15s; inspect logs for install, bootstrap and crash cause." | tee -a smoke-output/README.txt
fi
printf '%s\n' "$RESULT" | tee smoke-output/status.txt
grep -n -i -E 'FATAL EXCEPTION|Fatal signal|SIGSEGV|SIGBUS|AndroidRuntime|libil2cpp|com\.catsbit\.oxidesurvivalisland' smoke-output/logcat.txt | tail -80 > smoke-output/relevant_lines.txt || true
[[ "$RESULT" == "RUNNING_AT_15S_AND_45S" ]]
