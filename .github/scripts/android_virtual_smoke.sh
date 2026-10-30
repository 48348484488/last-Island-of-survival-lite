#!/usr/bin/env bash
# Real Android emulator smoke test. Never claim success from APK packaging alone.
set -Eeuo pipefail
mkdir -p evidence
capture_evidence() {
  local status=$?
  adb logcat -d -v threadtime > evidence/logcat.txt 2>&1 || true
  adb shell dumpsys activity activities > evidence/activities.txt 2>&1 || true
  if [[ ! -s evidence/runtime.png ]]; then
    adb exec-out screencap -p > evidence/runtime.png 2>/dev/null || true
  fi
  printf '%s\n' "$status" > evidence/script-exit-code.txt
}
trap capture_evidence EXIT

adb shell getprop ro.product.cpu.abilist | tee evidence/device-abilist.txt
adb shell getprop ro.dalvik.vm.native.bridge | tee evidence/native-bridge.txt
if ! grep -q 'arm64-v8a' evidence/device-abilist.txt; then
  echo 'ARM64_ABI_UNAVAILABLE: no arm64-v8a support reported' | tee evidence/unsupported-abi.txt
  exit 42
fi
if grep -q 'x86_64' evidence/device-abilist.txt && [[ ! -s evidence/native-bridge.txt ]]; then
  echo 'ARM64_BRIDGE_UNCONFIRMED: x86_64 guest without native translation bridge' | tee evidence/unsupported-abi.txt
  exit 43
fi

APK="$(find dist -type f -name '*.apk' -print -quit)"
if [[ -z "$APK" || ! -s "$APK" ]]; then
  echo 'APK_MISSING' | tee evidence/install.txt
  exit 44
fi
installed=0
for attempt in 1 2 3 4 5; do
  if adb install -r "$APK" > evidence/install.txt 2>&1; then
    installed=1
    break
  fi
  sleep 10
done
cat evidence/install.txt
if [[ "$installed" -ne 1 ]]; then
  echo 'APK_INSTALL_FAILED'
  exit 45
fi
adb shell wm size 1920x1080
adb shell monkey -p org.lios.recreatedlite -c android.intent.category.LAUNCHER 1 | tee evidence/launch.txt
sleep 18
adb exec-out screencap -p > evidence/runtime.png
adb shell pidof org.lios.recreatedlite | tee evidence/pid.txt
test -s evidence/runtime.png
test -s evidence/pid.txt
echo 'LIOS_ANDROID_VIRTUAL_ARM64_RUNTIME_SMOKE_PASS'
