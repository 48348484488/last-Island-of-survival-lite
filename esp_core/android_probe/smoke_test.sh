#!/usr/bin/env bash
set -euo pipefail
mkdir -p android-probe-out
{
  echo "Android emulated ABI report"
  adb shell getprop ro.build.version.release
  adb shell getprop ro.product.cpu.abilist
  adb shell getprop ro.dalvik.vm.native.bridge
} > android-probe-out/android_environment.txt
cat android-probe-out/android_environment.txt
adb logcat -c || true
adb install -r android-probe-out/oxide-bridge-jni-smoke.apk 2>&1 | tee android-probe-out/install.log
adb shell am start -W -n com.oxide.bridgeprobe/.MainActivity 2>&1 | tee android-probe-out/start.log
sleep 12
adb logcat -d -v threadtime > android-probe-out/logcat.log || true
adb shell pidof com.oxide.bridgeprobe > android-probe-out/pid.txt || true
adb exec-out screencap -p > android-probe-out/screen.png || true
grep 'OXIDE_BRIDGE_TEST' android-probe-out/logcat.log | tail -25 || true
if grep -q 'OXIDE_BRIDGE_TEST.*RESULT=PASS CODE=0' android-probe-out/logcat.log && [ -s android-probe-out/pid.txt ]; then
  echo "PASS: JNI loaded ARM64 native library, 20k native frame tests completed, process remained alive." | tee android-probe-out/result.txt
  exit 0
fi
echo "FAIL: JNI result marker or surviving Android process missing. Check logcat and start log." | tee android-probe-out/result.txt
grep -E 'FATAL EXCEPTION|Fatal signal|SIGILL|SIGSEGV|UnsatisfiedLinkError|INSTALL_FAILED' android-probe-out/logcat.log | tail -30 || true
exit 1
