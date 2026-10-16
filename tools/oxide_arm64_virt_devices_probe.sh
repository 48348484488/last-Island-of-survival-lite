#!/usr/bin/env bash
# Cheap device CLI parity preflight for QEMU-virt ARM64 LineageOS, no Android image.
set -Eeuo pipefail
EVIDENCE="$PWD/evidence/oxide-arm64-virt-devices"
mkdir -p "$EVIDENCE"
echo 'RESULT=NOT_VERIFIED' > "$EVIDENCE/result.txt"
echo 'ANDROID_IMAGE_DOWNLOADED=no' >> "$EVIDENCE/result.txt"
echo 'ORIGINAL_APK_DOWNLOADED=no' >> "$EVIDENCE/result.txt"
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT

sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm qemu-efi-aarch64 qemu-utils >/dev/null
fw_code="$(find /usr/share -name AAVMF_CODE.fd -print -quit 2>/dev/null)"
fw_vars="$(find /usr/share -name AAVMF_VARS.fd -print -quit 2>/dev/null)"
for f in "$fw_code" "$fw_vars"; do
  if [ -z "$f" ] || [ "$(stat -Lc '%s' "$f")" -ne 67108864 ]; then
    echo 'RESULT=INVALID_64M_UEFI_IMAGE' | tee -a "$EVIDENCE/result.txt"
    exit 1
  fi
done
cp "$fw_vars" "$tmp/vars.fd"
qemu-img create -q -f qcow2 "$tmp/vda.qcow2" 64M
qemu-img create -q -f qcow2 "$tmp/vdb.qcow2" 32M

started="$(date +%s)"
set +e
timeout --signal=TERM --kill-after=2s 8s qemu-system-aarch64 \
  -machine virt,accel=tcg -cpu max -smp 2 -m 768 \
  -drive "if=pflash,unit=0,format=raw,readonly=on,file=$fw_code" \
  -drive "if=pflash,unit=1,format=raw,file=$tmp/vars.fd" \
  -device virtio-blk-pci,drive=vda,bootindex=0 \
  -device virtio-blk-pci,drive=vdb,bootindex=1 \
  -drive "file=$tmp/vda.qcow2,if=none,id=vda,format=qcow2" \
  -drive "file=$tmp/vdb.qcow2,if=none,id=vdb,format=qcow2" \
  -device virtio-gpu-pci -display none \
  -device virtio-net-pci,netdev=net0,romfile= \
  -netdev user,id=net0,restrict=on,hostfwd=tcp:127.0.0.1:5555-:5555 \
  -device virtio-rng-pci \
  -monitor none -serial "file:$tmp/serial.log" -no-reboot \
  > "$EVIDENCE/qemu-stderr.log" 2>&1
rc=$?
set -e
printf 'QEMU_EXIT_CODE=%s\nWALL_SECONDS=%s\n' "$rc" "$(( $(date +%s) - started ))" | tee -a "$EVIDENCE/result.txt"
if [ -f "$tmp/serial.log" ]; then tail -c 120000 "$tmp/serial.log" > "$EVIDENCE/serial.log"; fi
if [ "$rc" = 124 ] && ! grep -aEq 'failed|Error|error|could not' "$EVIDENCE/qemu-stderr.log"; then
  echo 'RESULT=PASS_QEMU_ARM64_VIRT_FIRMWARE_AND_DEVICE_CLI_NO_ANDROID' | tee -a "$EVIDENCE/result.txt"
  echo 'LIMIT=This uses tiny blank qcow2 disks, not Android; not a gameplay proof.' | tee -a "$EVIDENCE/result.txt"
else
  echo 'RESULT=BLOCKED_QEMU_ARM64_VIRT_DEVICE_CLI' | tee -a "$EVIDENCE/result.txt"
  tail -n 30 "$EVIDENCE/qemu-stderr.log"
  exit 1
fi
