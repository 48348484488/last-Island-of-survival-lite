#!/usr/bin/env bash
# Authorized, non-invasive Android 13 native A/B experiment on the same guest.
# The ONLY state changed is Android's ordinary runtime permission grants for
# the installed, signed original package. No spoofing, injection or game edits.
set -euo pipefail
if [[ $# -ne 3 ]]; then
  echo "Usage: $0 ADB_SERIAL AAPT_PERMISSION_FILE OUTPUT_DIRECTORY" >&2
  exit 64
fi
SERIAL="$1"
AAPT="$2"
OUT="$3"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PKG="com.catsbit.oxidesurvivalisland"
ACTIVITY="cTOOT3.abKmV1.tP0Gr1.OxideActivity"
mkdir -p "$OUT/before" "$OUT/after"
test -s "$AAPT"
adb -s "$SERIAL" get-state | grep -qx device
for name in before after; do
  mkdir -p "$OUT/$name"
done
adb -s "$SERIAL" shell pm list packages com.google.android.gms \
  > "$OUT/play_packages.txt" 2>&1 || true
# A failed query must NOT be interpreted as 'not installed'.
if adb -s "$SERIAL" shell pm list packages com.google.android.gms \
  > "$OUT/play_packages.txt" 2>&1; then
  echo __PACKAGE_QUERY_COMPLETED__ >> "$OUT/play_packages.txt"
fi

capture() {
  local scenario="$1"
  local target="$OUT/$scenario"
  adb -s "$SERIAL" shell dumpsys package "$PKG" \
    > "$target/dumpsys_package.txt" 2>&1 || true
  adb -s "$SERIAL" shell cmd appops get "$PKG" \
    > "$target/appops.txt" 2>&1 || true
  # Clear *all* Android log buffers; -c alone can retain previous events.
  adb -s "$SERIAL" logcat -b all -c 2>/dev/null || true
  adb -s "$SERIAL" shell am force-stop "$PKG" > "$target/force_stop.txt" 2>&1 || true
  adb -s "$SERIAL" logcat -b all -v threadtime \
    > "$target/live_logcat.txt" 2>&1 &
  local capture_pid=$!
  adb -s "$SERIAL" shell am start -W -n "$PKG/$ACTIVITY" \
    > "$target/launch.txt" 2>&1 || true
  adb -s "$SERIAL" shell pidof "$PKG" \
    > "$target/pid_early.txt" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p \
    > "$target/startup.png" 2>/dev/null || true
  sleep 9
  adb -s "$SERIAL" shell pidof "$PKG" \
    > "$target/pid_at_9_seconds.txt" 2>&1 || true
  adb -s "$SERIAL" shell dumpsys activity activities \
    > "$target/activity_at_9_seconds.txt" 2>&1 || true
  adb -s "$SERIAL" shell dumpsys activity exit-info "$PKG" \
    > "$target/exit_info.txt" 2>&1 || true
  kill "$capture_pid" 2>/dev/null || true
  wait "$capture_pid" 2>/dev/null || true
  cp "$target/live_logcat.txt" "$target/all_buffers.txt"
  python3 "$ROOT/tools/diagnose_original_exit.py" "$target" \
    > "$target/forensic_stdout.txt"
  python3 "$ROOT/tools/diagnose_android_runtime.py" \
    --aapt "$AAPT" --dumpsys "$target/dumpsys_package.txt" \
    --appops "$target/appops.txt" \
    --play-packages "$OUT/play_packages.txt" \
    --logcat "$target/all_buffers.txt" \
    --forensic-json "$target/forensic_summary.json" \
    --out "$target" > "$target/prerequisite_stdout.txt"
  echo "$scenario: $(python3 -c 'import json,sys;print(json.load(open(sys.argv[1]))["classification"])' "$target/forensic_summary.json")"
}

# Run original game with Android's default grants first.
capture before

# Android's normal runtime permission API; only declared permissions,
# and each actual grant is independently remeasured after this operation.
: > "$OUT/grant_attempts.tsv"
for permission in \
    android.permission.CAMERA \
    android.permission.RECORD_AUDIO \
    android.permission.BLUETOOTH_CONNECT \
    android.permission.POST_NOTIFICATIONS; do
  status="FAILED"
  if grep -Fq "name='$permission'" "$AAPT"; then
    logfile="$OUT/grant_$(echo "$permission" | tr '.' '_').txt"
    if adb -s "$SERIAL" shell pm grant "$PKG" "$permission" > "$logfile" 2>&1; then
      status="REQUEST_ACCEPTED"
    else
      status="REQUEST_REJECTED"
    fi
  else
    status="NOT_DECLARED"
  fi
  printf '%s\t%s\n' "$permission" "$status" >> "$OUT/grant_attempts.tsv"
done

# Keep all four requests in a machine-readable log; no identifiers.
python3 - "$OUT/grant_attempts.tsv" "$OUT/grant_attempts.json" <<'PY'
import json
import sys
from pathlib import Path
result = {}
for line in Path(sys.argv[1]).read_text().splitlines():
    name, status = line.split("\t", 1)
    result[name] = status
Path(sys.argv[2]).write_text(json.dumps(result, indent=2) + "\n")
PY

# Launch again on the SAME guest to isolate the effect of allowed grants.
capture after
python3 "$ROOT/tools/compare_android_grants.py" \
  --before "$OUT/before/runtime_prerequisites.json" \
  --after "$OUT/after/runtime_prerequisites.json" \
  --attempts "$OUT/grant_attempts.json" \
  --out "$OUT"
cat "$OUT/comparison.md"
echo "A/B DIAGNOSTIC COMPLETE: game stability and multiplayer not assumed"
