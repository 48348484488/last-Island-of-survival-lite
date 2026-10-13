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

# First scene: only explicitly labeled controlled test geometry is drawn.
adb shell am start -W -n com.oxide.bridgeprobe/.MainActivity 2>&1 | tee android-probe-out/start.log
sleep 3
adb exec-out screencap -p > android-probe-out/screen.png
adb shell pidof com.oxide.bridgeprobe > android-probe-out/pid.txt || true

# Second scene: no samples, no box. A process that keeps stale rectangles fails.
adb shell am force-stop com.oxide.bridgeprobe
adb shell am start -W -n com.oxide.bridgeprobe/.MainActivity --ez no_boxes true 2>&1 | tee android-probe-out/start_no_samples.log
sleep 3
adb exec-out screencap -p > android-probe-out/screen_empty.png
adb shell pidof com.oxide.bridgeprobe > android-probe-out/pid_empty.txt || true
# Third scene: stale sample must not keep the prior player's marker.
adb shell am force-stop com.oxide.bridgeprobe
adb shell am start -W -n com.oxide.bridgeprobe/.MainActivity --ez stale_boxes true 2>&1 | tee android-probe-out/start_stale.log
sleep 3
adb exec-out screencap -p > android-probe-out/screen_stale.png
adb shell pidof com.oxide.bridgeprobe > android-probe-out/pid_stale.txt || true
# Fourth scene: draw a transparent overlay over the Android launcher.
# The app is a standalone diagnostic and does not inspect any other application.
adb shell am force-stop com.oxide.bridgeprobe
adb shell appops set com.oxide.bridgeprobe SYSTEM_ALERT_WINDOW allow
adb shell am start -W -n com.oxide.bridgeprobe/.MainActivity --ez overlay_probe true 2>&1 | tee android-probe-out/start_overlay.log
sleep 3
adb shell input keyevent KEYCODE_HOME
sleep 2
adb exec-out screencap -p > android-probe-out/screen_overlay.png
adb shell am force-stop com.oxide.bridgeprobe
sleep 2
adb exec-out screencap -p > android-probe-out/screen_overlay_removed.png
adb logcat -d -v threadtime > android-probe-out/logcat.log || true
grep 'OXIDE_BRIDGE_TEST' android-probe-out/logcat.log | tail -25 || true

if ! grep -q 'OXIDE_BRIDGE_TEST.*RESULT=PASS CODE=0 MODE=TEST_RECT_PRESENT' android-probe-out/logcat.log; then
  echo "FAIL: missing successful native rectangle test marker" | tee android-probe-out/result.txt
  exit 1
fi
if ! grep -q 'OXIDE_BRIDGE_TEST.*RESULT=PASS CODE=0 MODE=NO_SAMPLES' android-probe-out/logcat.log; then
  echo "FAIL: missing successful no-data test marker" | tee android-probe-out/result.txt
  exit 1
fi
if ! grep -q 'OXIDE_BRIDGE_TEST.*RESULT=PASS CODE=0 MODE=STALE_SAMPLES' android-probe-out/logcat.log; then
  echo "FAIL: missing stale-player rejection marker" | tee android-probe-out/result.txt
  exit 1
fi
if ! grep -q 'OXIDE_BRIDGE_OVERLAY.*RESULT=PASS MODE=OVERLAY_DIAGNOSTIC' android-probe-out/logcat.log; then
  echo "FAIL: diagnostic overlay Service did not start successfully" | tee android-probe-out/result.txt
  exit 1
fi
# Ignore unrelated system crashes, but never accept an app-specific exception.
if grep -Eq 'AndroidRuntime: Process: com\\.oxide\\.bridgeprobe|libc[[:space:]]*: Fatal signal.*com\\.oxide\\.bridgeprobe' android-probe-out/logcat.log; then
  echo "FAIL: diagnostic app crash detected in logcat" | tee android-probe-out/result.txt
  exit 1
fi

if [ ! -s android-probe-out/pid.txt ] || [ ! -s android-probe-out/pid_empty.txt ] || [ ! -s android-probe-out/pid_stale.txt ]; then
  echo "FAIL: diagnostic app did not remain alive after both rendering states" | tee android-probe-out/result.txt
  exit 1
fi

python3 esp_core/android_probe/check_render.py \
  android-probe-out/screen.png android-probe-out/screen_empty.png android-probe-out/screen_stale.png \
  | tee android-probe-out/render_pixels.txt
python3 esp_core/android_probe/check_overlay.py \
  android-probe-out/screen_overlay.png android-probe-out/screen_overlay_removed.png \
  | tee android-probe-out/overlay_pixels.txt
echo "PASS: Java JNI accepted timestamped snapshots, rejected stale/empty data, rendered and cleared Android transparent overlay. Test data only." | tee android-probe-out/result.txt
