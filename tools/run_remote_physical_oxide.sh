#!/usr/bin/env bash
# Authorized remote physical ARM64 Android only. No original-game modifications.
set -euo pipefail
if [ "$#" -ne 1 ]; then echo "Usage: $0 PRIVATE_OUTPUT_DIR" >&2; exit 64; fi
OUT="$1"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
mkdir -p "$OUT"
python3 "$ROOT/tools/remote_physical_preflight.py" --output "$OUT/preflight.json"
python3 - "$OUT/preflight.json" <<'PY'
import json,sys
d=json.load(open(sys.argv[1]))
if d["status"]!="REMOTE_PHYSICAL_CONNECTION_CONFIGURED":
    raise SystemExit("REMOTE_PHYSICAL_NOT_CONFIGURED; no network or game file requests")
PY
for cmd in ssh adb curl python3 timeout; do
  command -v "$cmd" >/dev/null || { echo "Missing tool: $cmd" >&2; exit 69; }
done
TEMP="$(mktemp -d)"
SSH_PID=""
cleanup() {
  if [ -n "$SSH_PID" ]; then kill "$SSH_PID" 2>/dev/null || true; wait "$SSH_PID" 2>/dev/null || true; fi
  rm -rf "$TEMP"
}
trap cleanup EXIT
umask 077
printf '%s\n' "$OXIDE_REMOTE_SSH_KEY" > "$TEMP/id_ssh"
printf '%s\n' "$OXIDE_REMOTE_KNOWN_HOSTS" > "$TEMP/known_hosts"
chmod 600 "$TEMP/id_ssh" "$TEMP/known_hosts"
ssh -i "$TEMP/id_ssh" -o BatchMode=yes -o StrictHostKeyChecking=yes \
  -o UserKnownHostsFile="$TEMP/known_hosts" -o ConnectTimeout=20 \
  -o ExitOnForwardFailure=yes -o ServerAliveInterval=15 \
  -N -L 127.0.0.1:5038:127.0.0.1:5037 \
  "$OXIDE_REMOTE_USER@$OXIDE_REMOTE_HOST" > "$TEMP/ssh.log" 2>&1 &
SSH_PID=$!
export ADB_SERVER_SOCKET="tcp:127.0.0.1:5038"
ready=0
for attempt in $(seq 1 12); do
  if ! kill -0 "$SSH_PID" 2>/dev/null; then break; fi
  if timeout 8 adb -s "$OXIDE_REMOTE_ADB_SERIAL" get-state 2>/dev/null | grep -qx device; then
    ready=1
    break
  fi
  sleep 2
done
if [ "$ready" -ne 1 ]; then
  echo "REMOTE_ADB_NOT_CONNECTED; no original assets downloaded"
  echo REMOTE_ADB_NOT_CONNECTED > "$OUT/connection_status.txt"
  exit 69
fi
ABI="$(adb -s "$OXIDE_REMOTE_ADB_SERIAL" shell getprop ro.product.cpu.abilist | tr -d '\r')"
QEMU="$(adb -s "$OXIDE_REMOTE_ADB_SERIAL" shell getprop ro.kernel.qemu | tr -d '\r')"
BOOT_QEMU="$(adb -s "$OXIDE_REMOTE_ADB_SERIAL" shell getprop ro.boot.qemu | tr -d '\r')"
if [[ "$ABI" != *arm64-v8a* ]]; then
  echo REMOTE_NOT_ARM64 > "$OUT/connection_status.txt"
  exit 65
fi
if [[ "$QEMU" == 1 || "$BOOT_QEMU" == 1 ]]; then
  echo REMOTE_REPORTS_EMULATOR > "$OUT/connection_status.txt"
  exit 65
fi
# Property values are sanity checks, NEVER physical/certification attestation.
echo AUTHORIZED_ARM64_ADB_CONNECTED_NOT_ATTESTED > "$OUT/connection_status.txt"
echo "REMOTE_ADB_ARM64_CHECKED, downloading original game resource split"
APK="$TEMP/original-apks"
mkdir -p "$APK"
for name in base.apk split_config.arm64_v8a.apk split_config.xxhdpi.apk; do
  cp "$ROOT/$name" "$APK/$name"
done
curl --fail --location --retry 4 --retry-all-errors \
  --connect-timeout 30 --max-time 1200 --silent --show-error \
  --output "$APK/split_UnityDataAssetPack.apk" \
  "https://github.com/48348484488/oxide/releases/download/v1.0.0/split_UnityDataAssetPack.apk"
python3 "$ROOT/tools/verify_original_splits.py" "$APK" \
  --output "$OUT/apk_sha256.json" >/dev/null
echo ORIGINAL_SPLITS_SHA256_VERIFIED > "$OUT/original_integrity_status.txt"
PRIVATE="$TEMP/private-evidence"
mkdir -p "$PRIVATE"
set +e
bash "$ROOT/tools/smoke_original_on_connected_device.sh" \
  "$OXIDE_REMOTE_ADB_SERIAL" "$APK" "$PRIVATE" > "$TEMP/smoke_stdout.log" 2>&1
game_rc=$?
set -e
python3 "$ROOT/tools/summarize_remote_physical.py" \
  --evidence "$PRIVATE" --out "$OUT" --smoke-exit-code "$game_rc"
echo "ORIGINAL_GAME_SMOKE_EXIT_CODE=$game_rc (NOT gameplay proof)"
