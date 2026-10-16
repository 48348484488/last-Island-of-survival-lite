#!/usr/bin/env bash
# Distinct hypothesis from run #38930455465:
# Android recovery stalls because QEMU virt has neither VirtIO block devices,
# Ethernet nor GPU. Add minimal GPT-labeled synthetic disks + devices and
# compare evidence. These disks contain NO Android system image.
set -Eeuo pipefail
evidence="$PWD/evidence/oxide-android-recovery-virtio"
mkdir -p "$evidence"
echo "RESULT=PRECHECK_STARTED_NOT_GAMEPLAY" > "$evidence/result.txt"
tmp="$(mktemp -d)"
trap 'status=$?; echo "SCRIPT_EXIT=$status" >> "$evidence/result.txt"; rm -rf "$tmp"' EXIT
echo "HOST_ARCH=$(uname -m)" | tee -a "$evidence/result.txt"
echo "STRATEGY=QEMU_AARCH64_VIRT_TCG_VIRTIO_GPT_NET_GPU" | tee -a "$evidence/result.txt"
echo "NO_ANDROID_FULL_IMAGE_DOWNLOADED=yes" | tee -a "$evidence/result.txt"
echo "NO_GAME_APK_DOWNLOADED=yes" | tee -a "$evidence/result.txt"

sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm gdisk lz4 curl >/dev/null
qemu-system-aarch64 --version | head -n 1 | tee -a "$evidence/result.txt"

recovery_url="https://github.com/jqssun/android-lineage-qemu/releases/download/v2026.09.17/recovery_arm64only.img"
curl --fail --location --silent --show-error --retry 1 \
  --connect-timeout 10 --max-time 75 --max-filesize 65000000 \
  "$recovery_url" -o "$tmp/recovery.img"
test "$(stat -c '%s' "$tmp/recovery.img")" -eq 60821504
echo '08389076ebd424f9626dde48529e1e81a676b864b413d5fd37629a7826ff0158  '"$tmp/recovery.img" | sha256sum -c
echo "RECOVERY_ORIGIN=$recovery_url" | tee -a "$evidence/result.txt"
echo "RECOVERY_SHA256_VERIFIED=yes" | tee -a "$evidence/result.txt"
python3 tools/oxide_android_bootimg_extract.py "$tmp/recovery.img" "$tmp/extracted"
cp "$tmp/extracted/image-metadata.json" "$evidence/"

# First VirtIO disk: tiny synthetic GPT partition placeholders.
# Placeholders are intentionally NOT LineageOS system images or writable updates.
truncate -s 256M "$tmp/vda.raw"
sgdisk --clear \
  --new=1:2048:+8M --change-name=1:misc \
  --new=2:0:+48M --change-name=2:cache \
  --new=3:0:+32M --change-name=3:persist \
  --new=4:0:+96M --change-name=4:userdata "$tmp/vda.raw"
sgdisk -p "$tmp/vda.raw" | tee "$evidence/synthetic-gpt.txt"
truncate -s 64M "$tmp/vdb.raw"
echo "VDA_BYTES=$(stat -c %s "$tmp/vda.raw")" | tee -a "$evidence/result.txt"
echo "VDB_BYTES=$(stat -c %s "$tmp/vdb.raw")" | tee -a "$evidence/result.txt"

# Physical host's /dev/kvm is irrelevant: force software ARM64 TCG explicitly.
# Capture serial to bounded artifact rather than putting all console output in CI.
start="$(date +%s)"
set +e
timeout --signal=TERM --kill-after=5s 75s \
  qemu-system-aarch64 \
    -machine virt,accel=tcg -cpu max -smp 2 -m 2048 \
    -kernel "$tmp/extracted/Image" \
    -initrd "$tmp/extracted/ramdisk.img" \
    -append 'console=ttyAMA0 earlycon=pl011,0x09000000 androidboot.hardware=virtio loglevel=6' \
    -drive if=none,id=drv_a,file="$tmp/vda.raw",format=raw \
    -device virtio-blk-pci,drive=drv_a \
    -drive if=none,id=drv_b,file="$tmp/vdb.raw",format=raw \
    -device virtio-blk-pci,drive=drv_b \
    -netdev user,id=n0,restrict=on \
    -device virtio-net-pci,netdev=n0,romfile= \
    -device virtio-gpu-pci \
    -device virtio-rng-pci \
    -display none -monitor none -serial file:"$tmp/serial.log" \
    -no-reboot
qemu_rc=$?
set -e
echo "QEMU_EXIT_CODE=$qemu_rc" | tee -a "$evidence/result.txt"
echo "QEMU_WALL_SECONDS=$(( $(date +%s) - start ))" | tee -a "$evidence/result.txt"
if test -f "$tmp/serial.log"; then
  tail -c 2000000 "$tmp/serial.log" > "$evidence/console.log"
else
  : > "$evidence/console.log"
fi

# Each condition is independently observable. A Linux kernel alone is not Android.
python3 - "$evidence" <<'PY'
from pathlib import Path
import json
import re
import sys
root = Path(sys.argv[1])
log = (root / "console.log").read_text(errors="replace")
checks = {
    "arm64_linux_kernel_booted": "Linux version" in log,
    "android_init_process": bool(re.search(r"\]\[\s*T1\] init:", log)) or "init: " in log,
    "android_recovery_process": bool(re.search(r"\brecovery: ", log)),
    "recovery_uses_block_device": "/dev/block/vdb" in log,
    "gpt_by_name_misc_absent": "failed to stat /dev/block/by-name/misc" in log,
    "gpt_by_name_cache_absent": "/dev/block/by-name/cache: Can't lookup blockdev" in log,
    "eth0_absent": "Failed to get interface active flags: No such device" in log,
    "recovery_graphics_timeout": "timeout wait for 'graphics'" in log,
    "kernel_device_mapper_absent": "Failed to open device-mapper" in log,
    "full_android_boot_confirmed": False,
    "oxide_original_activity_confirmed": False,
}
status = (
    "PASS_RECOVERY_EXECUTED_WITH_SYNTHETIC_DEVICES_NOT_ANDROID_BOOTED"
    if checks["arm64_linux_kernel_booted"] and checks["android_recovery_process"]
    else "FAIL_NO_RECOVERY_PROCESS_EVIDENCE"
)
(root / "comparison.json").write_text(json.dumps({
    "result": status,
    "baseline_run_id": 38930455465,
    "baseline_observed": {
        "gpt_by_name_misc_absent": True,
        "gpt_by_name_cache_absent": True,
        "eth0_absent": True,
        "recovery_graphics_timeout": True,
        "kernel_device_mapper_absent": True,
        "android_recovery_process": True
    },
    "current_checks": checks,
    "scope": "Read-only boot with synthetic blank GPT disks and virtual network/GPU. Not full LineageOS, Google Play, or Oxide.",
}, indent=2, sort_keys=True) + "\n")
with (root / "result.txt").open("a") as fp:
    fp.write(f"RESULT={status}\n")
print(json.dumps({"result": status, "checks": checks}, sort_keys=True))
PY
grep -Fq "PASS_RECOVERY_EXECUTED_WITH_SYNTHETIC_DEVICES_NOT_ANDROID_BOOTED" "$evidence/result.txt"
