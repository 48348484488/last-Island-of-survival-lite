#!/usr/bin/env bash
# Compare two unmodified native ARM64 Redroid virtual Android images.
# No physical device, no x86 translation, no app/security modifications.
set -euo pipefail
if [[ $# -ne 2 ]]; then
  echo "Usage: $0 ORIGINAL_APK_DIRECTORY OUTPUT_DIRECTORY" >&2; exit 64
fi
APK="$1"; OUT="$2"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
GAME=com.catsbit.oxidesurvivalisland
ACTIVITY=cTOOT3.abKmV1.tP0Gr1.OxideActivity
SERIAL=127.0.0.1:5555
mkdir -p "$OUT"
python3 "$ROOT/tools/verify_original_splits.py" "$APK" \
  --output "$OUT/original_hashes.json" >/dev/null
test "$(uname -m)" = aarch64

cleanup() {
  docker rm -f oxide-virtual-version >/dev/null 2>&1 || true
  adb disconnect "$SERIAL" >/dev/null 2>&1 || true
}
trap cleanup EXIT

for VERSION in 12 14; do
  DIR="$OUT/android_$VERSION"; mkdir -p "$DIR"
  IMAGE="redroid/redroid:$VERSION.0.0_64only-latest"
  cleanup
  echo "$IMAGE" > "$DIR/image.txt"
  echo STARTED > "$DIR/experiment_phase.txt"
  if ! timeout 600 docker pull "$IMAGE" > "$DIR/docker_pull.txt" 2>&1; then
    echo IMAGE_PULL_FAILED > "$DIR/experiment_phase.txt"
    continue
  fi
  docker run -d --rm --privileged --name oxide-virtual-version \
    -p 127.0.0.1:5555:5555 -v /dev/binderfs:/dev/binderfs \
    -v "/tmp/oxide-virtual-$VERSION-data:/data" \
    "$IMAGE" \
    androidboot.redroid_gpu_mode=guest \
    androidboot.redroid_width=720 androidboot.redroid_height=1280 \
    androidboot.redroid_dpi=320 \
    > "$DIR/container.txt" 2>&1 || {
      echo CONTAINER_START_FAILED > "$DIR/experiment_phase.txt"
      continue
    }
  READY=0
  for attempt in $(seq 1 60); do
    adb connect "$SERIAL" >/dev/null 2>&1 || true
    if timeout 6 adb -s "$SERIAL" shell getprop sys.boot_completed \
      2>/dev/null | grep -q '^1'; then READY=1; break; fi
    sleep 3
  done
  if [[ "$READY" != 1 ]]; then
    echo ANDROID_BOOT_FAILED > "$DIR/experiment_phase.txt"
    docker logs oxide-virtual-version > "$DIR/container_log.txt" 2>&1 || true
    continue
  fi
  echo BOOT_PASS > "$DIR/experiment_phase.txt"
  adb -s "$SERIAL" shell getprop ro.product.cpu.abilist \
    > "$DIR/abi.txt" 2>&1 || true
  adb -s "$SERIAL" shell getprop ro.build.version.release \
    > "$DIR/release.txt" 2>&1 || true
  adb -s "$SERIAL" shell getprop ro.build.version.sdk \
    > "$DIR/sdk.txt" 2>&1 || true
  if ! grep -q arm64-v8a "$DIR/abi.txt"; then
    echo NON_ARM64_GUEST > "$DIR/experiment_phase.txt"
    continue
  fi
  adb -s "$SERIAL" shell pm list packages com.google.android.gms \
    > "$DIR/google_packages.txt" 2>&1 || true
  if ! adb -s "$SERIAL" install-multiple -r \
    "$APK/base.apk" "$APK/split_config.arm64_v8a.apk" \
    "$APK/split_config.xxhdpi.apk" "$APK/split_UnityDataAssetPack.apk" \
    > "$DIR/install.txt" 2>&1; then
    echo ORIGINAL_INSTALL_FAILED > "$DIR/experiment_phase.txt"
    continue
  fi
  if ! grep -q '^Success' "$DIR/install.txt"; then
    echo ORIGINAL_INSTALL_NOT_CONFIRMED > "$DIR/experiment_phase.txt"
    continue
  fi
  echo ORIGINAL_INSTALL_SUCCESS > "$DIR/experiment_phase.txt"
  adb -s "$SERIAL" logcat -b all -c > "$DIR/logcat_clear.txt" 2>&1 || true
  adb -s "$SERIAL" shell am force-stop "$GAME" > "$DIR/force_stop.txt" 2>&1 || true
  adb -s "$SERIAL" logcat -b all -v threadtime > "$DIR/all_buffers.txt" 2>&1 &
  LOG_PID=$!
  adb -s "$SERIAL" shell am start -W -n "$GAME/$ACTIVITY" \
    > "$DIR/activity_start.txt" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p \
    > "$DIR/screen_early.png" 2>/dev/null || true
  sleep 2
  adb -s "$SERIAL" exec-out screencap -p \
    > "$DIR/screen_2s.png" 2>/dev/null || true
  sleep 8
  adb -s "$SERIAL" shell pidof "$GAME" > "$DIR/pid_10s.txt" 2>&1 || true
  adb -s "$SERIAL" shell dumpsys activity activities \
    > "$DIR/activities_10s.txt" 2>&1 || true
  adb -s "$SERIAL" shell dumpsys activity exit-info "$GAME" \
    > "$DIR/exit_info.txt" 2>&1 || true
  adb -s "$SERIAL" logcat -b crash -d -v threadtime \
    > "$DIR/crash_buffer.txt" 2>&1 || true
  kill "$LOG_PID" 2>/dev/null || true
  wait "$LOG_PID" 2>/dev/null || true
  echo OBSERVATION_COMPLETE > "$DIR/experiment_phase.txt"
  python3 "$ROOT/tools/diagnose_original_exit.py" "$DIR" \
    > "$DIR/forensic_stdout.txt" 2>&1 || true
  docker logs oxide-virtual-version > "$DIR/container_log.txt" 2>&1 || true
done

python3 - "$OUT" <<'PY'
import json,sys
from pathlib import Path
root=Path(sys.argv[1])
result={}
for version in ("12","14"):
    folder=root/("android_"+version)
    def read(name):
        p=folder/name
        return p.read_text(errors="replace").strip() if p.is_file() else ""
    def load(name):
        try: return json.loads(read(name))
        except (ValueError,TypeError): return {}
    foren=load("forensic_summary.json")
    pids={x for x in read("pid_10s.txt").split() if x.isdecimal()}
    pid=foren.get("initial_activity_pid")
    activities=read("activities_10s.txt")
    resumed=any(
        "OxideActivity" in x and "com.catsbit.oxidesurvivalisland/" in x
        and ("topResumedActivity" in x or "mResumedActivity" in x)
        for x in activities.splitlines())
    gms=read("google_packages.txt")
    result[version]={
        "host_architecture":"aarch64",
        "abi":read("abi.txt"),
        "boot":"PASS" if read("experiment_phase.txt") in
            ("BOOT_PASS","ORIGINAL_INSTALL_FAILED","ORIGINAL_INSTALL_NOT_CONFIRMED",
             "ORIGINAL_INSTALL_SUCCESS","OBSERVATION_COMPLETE") else "FAIL",
        "install":"SUCCESS" if read("install.txt").startswith("Success") else "NOT_CONFIRMED",
        "initial_pid_alive_10s":pid is not None and str(pid) in pids,
        "activity_resumed_10s":resumed,
        "forensic":foren,
        "play_services":"PRESENT" if "package:com.google.android.gms" in gms.splitlines() else
                        "NOT_FOUND" if gms or read("experiment_phase.txt")=="OBSERVATION_COMPLETE" else "NOT_MEASURED",
        "experiment_phase":read("experiment_phase.txt"),
    }
(root/"raw_version_states.json").write_text(json.dumps(result,indent=2)+"\n")
PY
python3 "$ROOT/tools/compare_virtual_redroid_versions.py" \
  "$OUT/raw_version_states.json" --out "$OUT"
