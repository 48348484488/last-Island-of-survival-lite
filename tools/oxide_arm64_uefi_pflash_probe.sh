#!/usr/bin/env bash
# Validate the exact ARM64 UEFI pflash size required by GitHub-hosted QEMU.
# No Android, VM archive, game APK, account, or heavy download.
set -Eeuo pipefail
mkdir -p evidence/oxide-arm64-uefi
evidence="$PWD/evidence/oxide-arm64-uefi"
echo 'RESULT=NOT_PROVEN' > "$evidence/result.txt"
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT

sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends qemu-system-arm qemu-efi-aarch64 >/dev/null
fw_code="$(find /usr/share -name AAVMF_CODE.fd -print -quit 2>/dev/null)"
fw_vars="$(find /usr/share -name AAVMF_VARS.fd -print -quit 2>/dev/null)"
if [ -z "$fw_code" ] || [ -z "$fw_vars" ]; then
  echo 'RESULT=BLOCKED_MISSING_AAVMF_CODE_OR_VARS' | tee -a "$evidence/result.txt"
  exit 1
fi
code_bytes="$(stat -c '%s' "$fw_code")"
vars_bytes="$(stat -c '%s' "$fw_vars")"
echo "UEFI_CODE_BYTES=$code_bytes" | tee -a "$evidence/result.txt"
echo "UEFI_VARS_BYTES=$vars_bytes" | tee -a "$evidence/result.txt"
if [ "$code_bytes" -ne 67108864 ] || [ "$vars_bytes" -ne 67108864 ]; then
  echo 'RESULT=BLOCKED_AAVMF_PFLASH_NOT_64M' | tee -a "$evidence/result.txt"
  exit 1
fi
cp "$fw_vars" "$tmp/vars.fd"
set +e
timeout --signal=TERM --kill-after=3s 8s qemu-system-aarch64 \
  -machine virt,accel=tcg -cpu cortex-a57 -smp 1 -m 512 \
  -drive "if=pflash,unit=0,format=raw,readonly=on,file=$fw_code" \
  -drive "if=pflash,unit=1,format=raw,file=$tmp/vars.fd" \
  -nographic -monitor none -serial file:"$tmp/uefi-serial.log" \
  -no-reboot -net none \
  >"$tmp/qemu-stderr.log" 2>&1
rc=$?
set -e
echo "QEMU_EXIT_CODE=$rc" | tee -a "$evidence/result.txt"
cat "$tmp/qemu-stderr.log" > "$evidence/qemu-stderr.log"
tail -c 30000 "$tmp/uefi-serial.log" > "$evidence/uefi-serial.log"
if [ "$rc" -eq 124 ] && ! grep -aq 'device requires .* bytes' "$evidence/qemu-stderr.log"; then
  echo 'RESULT=PASS_AAVMF_64M_QEMU_ARM64_PFLASH_STARTED_NO_DISKS' | tee -a "$evidence/result.txt"
  echo 'LIMIT=Firmware-only proof; Android, original game, gameplay untested' | tee -a "$evidence/result.txt"
else
  echo 'RESULT=FAIL_UEFI_VM_FIRMWARE_PRECHECK' | tee -a "$evidence/result.txt"
  exit 1
fi
