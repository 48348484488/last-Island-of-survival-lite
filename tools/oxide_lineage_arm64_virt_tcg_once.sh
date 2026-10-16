#!/usr/bin/env bash
# One bounded, no-APK, release-metadata-checked QEMU virt Android ARM64 trial.
set -Eeuo pipefail
EVIDENCE="$PWD/evidence/oxide-lineage-virt-arm64-tcg"
mkdir -p "$EVIDENCE"
echo 'RESULT=PRECHECK_NOT_FULL_ANDROID_PROOF' > "$EVIDENCE/result.txt"
echo 'ORIGINAL_OXIDE_APK_DOWNLOADED=no' >> "$EVIDENCE/result.txt"
echo 'GOOGLE_PLAY_CERTIFICATION=NOT_VERIFIED' >> "$EVIDENCE/result.txt"
echo 'GAMEPLAY_VERIFIED=no' >> "$EVIDENCE/result.txt"
echo 'TRIAL_COUNT=1' >> "$EVIDENCE/result.txt"
echo 'VM=Community LineageOS 23.2 ARM64 virtio; not an official Google Play image' >> "$EVIDENCE/result.txt"

tmp="$(mktemp -d)"
qpid=''
cleanup() {
  local rc=$?
  if [ -n "$qpid" ] && kill -0 "$qpid" 2>/dev/null; then
    kill "$qpid" || true
    sleep 1
    kill -9 "$qpid" 2>/dev/null || true
    wait "$qpid" 2>/dev/null || true
  fi
  if [ -f "$tmp/guest-serial.log" ]; then tail -c 4000000 "$tmp/guest-serial.log" > "$EVIDENCE/guest-serial.log"; fi
  if [ -f "$tmp/qemu-stderr.log" ]; then tail -c 500000 "$tmp/qemu-stderr.log" > "$EVIDENCE/qemu-stderr.log"; fi
  echo "SCRIPT_EXIT_CODE=$rc" >> "$EVIDENCE/result.txt"
  rm -rf "$tmp"
}
trap cleanup EXIT

printf 'HOST_ARCH=%s\nHOST_CPUS=%s\n' "$(uname -m)" "$(nproc)" | tee -a "$EVIDENCE/result.txt"
# Fail closed before any large archive download.
python3 - "$EVIDENCE" <<'PY'
import json, shutil, sys, urllib.request
from pathlib import Path
p = Path(sys.argv[1]); url = 'https://api.github.com/repos/jqssun/android-lineage-qemu/releases/tags/v2026.09.17'
req = urllib.request.Request(url, headers={'Accept':'application/vnd.github+json','User-Agent':'Oxide-ARM64-QEMU-virt-full-boot-one-shot'})
with urllib.request.urlopen(req,timeout=20) as f: meta = json.load(f)
name = 'UTM-VM-lineage-23.2-20260917-jqssun-virtio_arm64only.zip'
matching = [x for x in meta.get('assets',[]) if x.get('name') == name]
assert len(matching)==1, 'EXPECTED_VM_RELEASE_ASSET_MISSING'
a = matching[0]
assert 1_120_000_000 < a['size'] < 1_130_000_000, 'EXPECTED_VM_SIZE_MISMATCH'
assert a.get('digest')=='sha256:0a50afc821d905848b4560c33a5a706e6aee5ac414f85b31f9e52fc1ffabbd61', 'EXPECTED_VM_DIGEST_MISMATCH'
assert a['browser_download_url'] == f'https://github.com/jqssun/android-lineage-qemu/releases/download/v2026.09.17/{name}', 'UNEXPECTED_VM_DOWNLOAD_HOST'
assert shutil.disk_usage('.').free > 8_000_000_000, 'INSUFFICIENT_DISK_FOR_VM'
(p/'upstream-asset-metadata.json').write_text(json.dumps({'tag':meta['tag_name'],'name':name,'bytes':a['size'],'digest':a['digest'],'url':a['browser_download_url']}, indent=2)+'\n')
print('GITHUB_RELEASE_ASSET_METADATA_CHECK=PASS')
print('HOST_DISK_FREE_BYTES=',shutil.disk_usage('.').free)
PY

sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm qemu-efi-aarch64 unzip >/dev/null
qemu-system-aarch64 --version | head -n 1 | tee -a "$EVIDENCE/result.txt"
qemu-system-aarch64 -machine help | grep -Eq '^virt ' || { echo 'NO_VIRT_BOARD'; exit 1; }
qemu-system-aarch64 -device help | grep -q 'virtio-gpu-pci' || { echo 'NO_VIRTIO_GPU_DEVICE'; exit 1; }
fw_code="$(find /usr/share -name AAVMF_CODE.fd -print -quit 2>/dev/null)"
fw_vars="$(find /usr/share -name AAVMF_VARS.fd -print -quit 2>/dev/null)"
if [ -z "$fw_code" ] || [ -z "$fw_vars" ]; then
  echo 'RESULT=BLOCKED_MISSING_MATCHED_AAVMF_FIRMWARE' | tee -a "$EVIDENCE/result.txt"
  exit 1
fi
# Verified independently in the successful firmware-only preflight #38932199116.
code_bytes="$(stat -Lc '%s' "$fw_code")"
vars_bytes="$(stat -Lc '%s' "$fw_vars")"
echo "AAVMF_CODE_REAL_BYTES=$code_bytes" | tee -a "$EVIDENCE/result.txt"
echo "AAVMF_VARS_REAL_BYTES=$vars_bytes" | tee -a "$EVIDENCE/result.txt"
if [ "$code_bytes" -ne 67108864 ] || [ "$vars_bytes" -ne 67108864 ]; then
  echo 'RESULT=BLOCKED_AAVMF_FIRMWARE_NOT_MATCHING_64M' | tee -a "$EVIDENCE/result.txt"
  exit 1
fi
cp "$fw_vars" "$tmp/aavmf-vars-64m.fd"
echo "UEFI_CODE=$fw_code" | tee -a "$EVIDENCE/result.txt"
echo 'UEFI_VARIABLE_STORE=Ubuntu_AAVMF_VARS_64M; upstream UTM 334848B store incompatible with this QEMU firmware' | tee -a "$EVIDENCE/result.txt"

VM_URL="https://github.com/jqssun/android-lineage-qemu/releases/download/v2026.09.17/UTM-VM-lineage-23.2-20260917-jqssun-virtio_arm64only.zip"
echo 'HOST_PREFLIGHT=PASS_VM_DOWNLOAD_AUTHORIZED_ONCE' | tee -a "$EVIDENCE/result.txt"
echo 'VM_DOWNLOAD_STARTED=yes' | tee -a "$EVIDENCE/result.txt"
curl --fail --location --show-error --silent --retry 1 --connect-timeout 12 \
  --max-time 200 --max-filesize 1250000000 "$VM_URL" -o "$tmp/lineage-arm64-virtio.zip"
actual="$(stat -c '%s' "$tmp/lineage-arm64-virtio.zip")"
echo "VM_DOWNLOADED_BYTES=$actual" | tee -a "$EVIDENCE/result.txt"
expected="$(python3 -c 'import json; print(json.load(open("evidence/oxide-lineage-virt-arm64-tcg/upstream-asset-metadata.json"))["bytes"])')"
test "$actual" -eq "$expected"
echo "0a50afc821d905848b4560c33a5a706e6aee5ac414f85b31f9e52fc1ffabbd61  $tmp/lineage-arm64-virtio.zip" | sha256sum --check
echo 'VM_ARCHIVE_SHA256_VERIFIED=yes' | tee -a "$EVIDENCE/result.txt"

unzip -Z -1 "$tmp/lineage-arm64-virtio.zip" | head -60 > "$EVIDENCE/archive-first-60-paths.txt"
mkdir -p "$tmp/guest"
timeout 90s unzip -q "$tmp/lineage-arm64-virtio.zip" -d "$tmp/guest"
vda="$(find "$tmp/guest" -name vda.qcow2 -print -quit)"
vdb="$(find "$tmp/guest" -name vdb.qcow2 -print -quit)"
efi_vars="$(find "$tmp/guest" -name efi_vars.fd -print -quit)"
for path in "$vda" "$vdb" "$efi_vars"; do
  if [ ! -f "$path" ]; then echo 'MISSING_REQUIRED_VM_DISK_OR_EFI_VARS' | tee -a "$EVIDENCE/result.txt"; exit 1; fi
done
echo "VDA_DISK_BYTES=$(stat -c '%s' "$vda")" | tee -a "$EVIDENCE/result.txt"
echo "VDB_DISK_BYTES=$(stat -c '%s' "$vdb")" | tee -a "$EVIDENCE/result.txt"
echo "EFI_VARS_BYTES=$(stat -c '%s' "$efi_vars")" | tee -a "$EVIDENCE/result.txt"

adb_bin="${ANDROID_HOME:-/usr/local/lib/android/sdk}/platform-tools/adb"
if [ ! -x "$adb_bin" ]; then adb_bin="$(command -v adb || true)"; fi
if [ -z "$adb_bin" ]; then echo 'MISSING_ADB_CLIENT' | tee -a "$EVIDENCE/result.txt"; exit 1; fi

qemu-system-aarch64 \
  -machine virt,accel=tcg -cpu max -smp 2 -m 3072 \
  -drive "if=pflash,unit=0,format=raw,readonly=on,file=$fw_code" \
  -drive "if=pflash,unit=1,format=raw,file=$tmp/aavmf-vars-64m.fd" \
  -device virtio-blk-pci,drive=vda,bootindex=0 \
  -device virtio-blk-pci,drive=vdb,bootindex=1 \
  -drive "file=$vda,if=none,id=vda,format=qcow2,snapshot=on" \
  -drive "file=$vdb,if=none,id=vdb,format=qcow2,snapshot=on" \
  -device virtio-gpu-pci -display none \
  -device virtio-net-pci,netdev=net0 \
  -netdev user,id=net0,restrict=on,hostfwd=tcp:127.0.0.1:5555-:5555 \
  -device virtio-rng-pci \
  -monitor none -serial "file:$tmp/guest-serial.log" -no-reboot \
  >"$tmp/qemu-stderr.log" 2>&1 &
qpid="$!"
echo "QEMU_PID=$qpid" | tee -a "$EVIDENCE/result.txt"
started="$(date +%s)"
booted=0
while (( $(date +%s) - started < 360 )); do
  sleep 6
  if ! kill -0 "$qpid" 2>/dev/null; then
    echo 'QEMU_PROCESS_EXITED_BEFORE_BOOT_PROOF=yes' | tee -a "$EVIDENCE/result.txt"
    break
  fi
  timeout 4 "$adb_bin" connect 127.0.0.1:5555 >/dev/null 2>&1 || true
  abi="$(timeout 4 "$adb_bin" -s 127.0.0.1:5555 shell getprop ro.product.cpu.abi 2>/dev/null | tr -d '\r' || true)"
  complete="$(timeout 4 "$adb_bin" -s 127.0.0.1:5555 shell getprop sys.boot_completed 2>/dev/null | tr -d '\r' || true)"
  if [[ "$abi" == arm64-v8a* ]]; then
    echo 'ANDROID_ADB_ARM64_OBSERVED=yes' | tee -a "$EVIDENCE/result.txt"
  fi
  if [ "$complete" = 1 ] && [ "$abi" = arm64-v8a ]; then
    booted=1
    echo "ANDROID_BOOT_COMPLETED_SECONDS=$(( $(date +%s) - started ))" | tee -a "$EVIDENCE/result.txt"
    printf '%s\n' "$abi" > "$EVIDENCE/abi.txt"
    printf '%s\n' "$complete" > "$EVIDENCE/boot.txt"
    timeout 7 "$adb_bin" -s 127.0.0.1:5555 shell getprop ro.build.version.release > "$EVIDENCE/version.txt" || true
    timeout 7 "$adb_bin" -s 127.0.0.1:5555 shell pm path com.google.android.gms > "$EVIDENCE/gms-path.txt" 2>&1 || true
    timeout 7 "$adb_bin" -s 127.0.0.1:5555 shell pm path com.android.vending > "$EVIDENCE/play-path.txt" 2>&1 || true
    timeout 10 "$adb_bin" -s 127.0.0.1:5555 exec-out screencap -p > "$EVIDENCE/boot-screenshot.png" || true
    echo 'RESULT=PASS_LINEAGE_ANDROID_ARM64_SYS_BOOT_COMPLETED_NO_OXIDE_TEST' | tee -a "$EVIDENCE/result.txt"
    break
  fi
done
echo "ANDROID_BOOT_PROBE_WALL_SECONDS=$(( $(date +%s) - started ))" | tee -a "$EVIDENCE/result.txt"
if [ "$booted" -ne 1 ]; then
  echo 'RESULT=NO_ANDROID_BOOT_COMPLETION_WITHIN_360_SECONDS' | tee -a "$EVIDENCE/result.txt"
  exit 1
fi
