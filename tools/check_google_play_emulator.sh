#!/usr/bin/env bash
# Probe official Google Play AVD prerequisites only; no Oxide installation.
set -euo pipefail
DIR=google-play-emulator-evidence
mkdir -p "$DIR"
adb wait-for-device
adb shell getprop ro.product.cpu.abilist | tr -d '\r' > "$DIR/abis.txt"
adb shell getprop ro.dalvik.vm.native.bridge | tr -d '\r' > "$DIR/native_bridge.txt"
adb shell getprop ro.build.version.sdk | tr -d '\r' > "$DIR/sdk.txt"
adb shell getprop sys.boot_completed | tr -d '\r' > "$DIR/boot.txt"
adb shell pm list packages | tr -d '\r' > "$DIR/packages.txt"
adb shell getprop ro.product.model | tr -d '\r' > "$DIR/model.txt"
adb shell getprop ro.build.fingerprint | tr -d '\r' > "$DIR/build_fingerprint.txt"
adb shell dumpsys SurfaceFlinger | grep -i -m 2 'GLES:' > "$DIR/gles.txt" || true
python3 tools/audit_google_play_emulator.py "$DIR"
