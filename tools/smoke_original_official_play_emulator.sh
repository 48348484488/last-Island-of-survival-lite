#!/usr/bin/env bash
# Diagnose unchanged Oxide 1.14.11949 on Google's OFFICIAL Play Store AVD.
# Installs only exact-hash original signed splits; never changes game protection,
# APK contents, certificates, Android device identity, or Play integrity results.
set -euo pipefail
if [[ $# -ne 2 ]]; then
  echo "Usage: $0 ORIGINAL_APK_DIRECTORY OUTPUT_DIRECTORY" >&2
  exit 64
fi
APK="$1"
OUT="$2"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PKG=com.catsbit.oxidesurvivalisland
ACTIVITY=cTOOT3.abKmV1.tP0Gr1.OxideActivity
mkdir -p "$OUT"
python3 "$ROOT/tools/verify_original_splits.py" "$APK" \
  --output "$OUT/original_sha256.json"
adb wait-for-device
adb shell getprop ro.product.cpu.abilist > "$OUT/abis.txt"
adb shell getprop ro.dalvik.vm.native.bridge > "$OUT/native_bridge.txt"
adb shell getprop ro.build.version.sdk > "$OUT/android_api.txt"
adb shell pm list packages | grep -E 'com.google.android.gms|com.android.vending|com.google.android.gsf' \
  > "$OUT/google_packages.txt" || true
adb shell getprop sys.boot_completed > "$OUT/boot_completed.txt"
# Only one install attempt; capture exact original package-manager error.
set +e
timeout 600 adb install-multiple -r \
  "$APK/base.apk" \
  "$APK/split_config.arm64_v8a.apk" \
  "$APK/split_config.xxhdpi.apk" \
  "$APK/split_UnityDataAssetPack.apk" \
  > "$OUT/install.txt" 2>&1
installed_code=$?
set -e
echo "$installed_code" > "$OUT/install_exit_code.txt"
cat "$OUT/install.txt"
if [[ "$installed_code" -ne 0 ]] || ! grep -q '^Success' "$OUT/install.txt"; then
  echo "INSTALL_ORIGINAL=FAILED_OR_REJECTED" | tee "$OUT/status.txt"
  echo "No Activity was started; review install.txt for actual reason."
  exit 0
fi
echo "INSTALL_ORIGINAL=SUCCESS" | tee "$OUT/status.txt"
adb shell pm path "$PKG" > "$OUT/installed_splits.txt" 2>&1 || true
adb shell dumpsys package "$PKG" > "$OUT/package_permissions.txt" 2>&1 || true
adb shell cmd appops get "$PKG" > "$OUT/appops.txt" 2>&1 || true

adb logcat -b all -c 2>/dev/null || true
adb shell am force-stop "$PKG" > "$OUT/force_stop.txt" 2>&1 || true
adb logcat -b all -v threadtime > "$OUT/all_buffers.txt" 2>&1 &
LOGPID=$!
close_log() {
  kill "$LOGPID" 2>/dev/null || true
  wait "$LOGPID" 2>/dev/null || true
}
trap close_log EXIT
adb shell am start -W -n "$PKG/$ACTIVITY" > "$OUT/launch.txt" 2>&1 || true
cat "$OUT/launch.txt"
adb shell pidof "$PKG" > "$OUT/pid_early.txt" 2>&1 || true
adb exec-out screencap -p > "$OUT/screen_early.png" 2>/dev/null || true
sleep 4
adb shell pidof "$PKG" > "$OUT/pid_5s.txt" 2>&1 || true
adb shell dumpsys activity activities > "$OUT/activities_5s.txt" 2>&1 || true
adb exec-out screencap -p > "$OUT/screen_5s.png" 2>/dev/null || true
sleep 5
adb shell pidof "$PKG" > "$OUT/pid_10s.txt" 2>&1 || true
adb shell dumpsys activity activities > "$OUT/activities_10s.txt" 2>&1 || true
adb exec-out screencap -p > "$OUT/screen_10s.png" 2>/dev/null || true
sleep 25
adb shell pidof "$PKG" > "$OUT/pid_35s.txt" 2>&1 || true
adb shell dumpsys activity activities > "$OUT/activities_35s.txt" 2>&1 || true
adb exec-out screencap -p > "$OUT/screen_35s.png" 2>/dev/null || true
adb shell dumpsys activity exit-info "$PKG" > "$OUT/exit_info.txt" 2>&1 || true
adb logcat -b crash -d -v threadtime > "$OUT/crash_buffer.txt" 2>&1 || true
close_log
trap - EXIT
python3 "$ROOT/tools/diagnose_original_exit.py" "$OUT" \
  > "$OUT/forensic_stdout.txt" 2>&1 || true
python3 "$ROOT/tools/check_activity_liveness.py" "$OUT" \
  > "$OUT/activity_verdict_stdout.txt" 2>&1 || true
python3 - "$OUT" <<'PY'
import json
import sys
from pathlib import Path
out=Path(sys.argv[1])
def load(name):
    p=out/name
    return json.loads(p.read_text()) if p.is_file() else {}
f=load("forensic_summary.json")
v=load("activity_verdict.json")
result={
    "scope":"UNMODIFIED_ORIGINAL_OXIDE_GOOGLE_PLAY_EMULATOR",
    "original_apk_install":"SUCCESS",
    "original_activity_pid":f.get("initial_activity_pid"),
    "observed_exit":f.get("classification","NOT_CLASSIFIED"),
    "initial_activity_liveness":v.get("status","NOT_MEASURED"),
    "initial_activity_pid_alive_10s":v.get("pid_present_at_10s",False),
    "initial_activity_pid_alive_35s":v.get("pid_present_at_35s",False),
    "activity_resumed_10s":v.get("oxide_activity_resumed_at_10s",False),
    "activity_resumed_35s":v.get("oxide_activity_resumed_at_35s",False),
    "play_integrity_verdict":"NOT_MEASURED",
    "account_license":"NOT_MEASURED",
    "multiplayer_gameplay_verified":False,
    "real_player_esp_verified":False,
    "limits":"Application startup test only, no app modifications or security bypass."
}
(out/"game_start_verdict.json").write_text(json.dumps(result,indent=2)+"\n")
print(json.dumps(result,indent=2))
PY
echo "ORIGINAL_GAME_OBSERVATION_COMPLETED" | tee -a "$OUT/status.txt"
