#!/usr/bin/env bash
# A bounded Linux AArch64 full-system TCG boot; NOT an Android or Oxide run.
set -Eeuo pipefail
ROOT="${GITHUB_WORKSPACE:-$PWD}"
EVIDENCE="$ROOT/evidence/oxide-tcg-kernel"
mkdir -p "$EVIDENCE"
echo "RESULT=NOT_YET_PROVEN" > "$EVIDENCE/result.txt"
tmp="$(mktemp -d)"
trap 'status=$?; printf "SCRIPT_EXIT_CODE=%s\n" "$status" >> "$EVIDENCE/result.txt"; rm -rf "$tmp"' EXIT

echo "HOST_ARCH=$(uname -m)" | tee -a "$EVIDENCE/result.txt"
echo "HOST_CPUS=$(nproc)" | tee -a "$EVIDENCE/result.txt"
echo "KVM_PRESENT=$(test -e /dev/kvm && echo yes || echo no)" | tee -a "$EVIDENCE/result.txt"
echo "SCOPE=QEMU system-mode Linux ARM64 kernel and PID1; NOT Android or Oxide." | tee -a "$EVIDENCE/result.txt"

echo "[1/4] Install host dependencies"
sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm gcc-aarch64-linux-gnu libc6-dev-arm64-cross cpio curl >/dev/null
qemu-system-aarch64 --version | head -n 1 | tee -a "$EVIDENCE/result.txt"

echo "[2/4] Download official Alpine Linux 3.24 ARM64 virt kernel (up to 16 MB)"
KERNEL_URL="https://dl-cdn.alpinelinux.org/alpine/v3.24/releases/aarch64/netboot/vmlinuz-virt"
echo "KERNEL_SOURCE=$KERNEL_URL" | tee -a "$EVIDENCE/result.txt"
curl --fail --location --show-error --silent --retry 1 --connect-timeout 10 \
  --max-time 60 --max-filesize 16000000 "$KERNEL_URL" -o "$tmp/vmlinuz-virt"
actual_size="$(stat -c '%s' "$tmp/vmlinuz-virt")"
echo "KERNEL_BYTES=$actual_size" | tee -a "$EVIDENCE/result.txt"
if (( actual_size < 4000000 || actual_size > 16000000 )); then
  echo "RESULT=INVALID_KERNEL_SIZE" | tee -a "$EVIDENCE/result.txt"
  exit 1
fi
sha256sum "$tmp/vmlinuz-virt" > "$EVIDENCE/kernel.sha256"

echo "[3/4] Build static ARM64 /init inside tiny initramfs"
mkdir -p "$tmp/initramfs/dev" "$tmp/initramfs/proc" "$tmp/initramfs/sys"
cat > "$tmp/guest_init.c" <<'EOF'
#include <stdio.h>
#include <unistd.h>
#include <sys/reboot.h>
#include <time.h>
int main(void) {
    struct timespec t = {0, 0};
    clock_gettime(CLOCK_BOOTTIME, &t);
    printf("OXIDE_TCG_ARM64_GUEST_INIT_OK uptime_seconds=%lld\n", (long long)t.tv_sec);
    fflush(stdout);
    sync();
    sleep(1);
    if (reboot(RB_POWER_OFF) != 0)
        perror("guest reboot(RB_POWER_OFF)");
    for (;;) pause();
}
EOF
aarch64-linux-gnu-gcc -Os -static -o "$tmp/initramfs/init" "$tmp/guest_init.c"
file "$tmp/initramfs/init" | tee -a "$EVIDENCE/result.txt"
sudo mknod "$tmp/initramfs/dev/console" c 5 1
sudo chmod 600 "$tmp/initramfs/dev/console"
(
  cd "$tmp/initramfs"
  find . -print0 | cpio --null -o -H newc 2>/dev/null | gzip -1 -n > "$tmp/initramfs.cpio.gz"
)
echo "INITRAMFS_BYTES=$(stat -c '%s' "$tmp/initramfs.cpio.gz")" | tee -a "$EVIDENCE/result.txt"

echo "[4/4] Boot Linux under ARM64 full-system TCG (90-second hard stop)"
started_at="$(date +%s)"
set +e
timeout --signal=TERM --kill-after=5s 90s \
  qemu-system-aarch64 \
    -machine virt,accel=tcg \
    -cpu cortex-a57 \
    -smp 2 \
    -m 768M \
    -kernel "$tmp/vmlinuz-virt" \
    -initrd "$tmp/initramfs.cpio.gz" \
    -append "console=ttyAMA0 rdinit=/init loglevel=6 panic=5" \
    -nographic -monitor none -no-reboot -net none \
  2>&1 | tee "$EVIDENCE/console.log"
qemu_rc="${PIPESTATUS[0]}"
set -e
echo "BOOT_WALL_SECONDS=$(( $(date +%s) - started_at ))" | tee -a "$EVIDENCE/result.txt"
echo "QEMU_EXIT_CODE=$qemu_rc" | tee -a "$EVIDENCE/result.txt"

if grep -aFq "Linux version" "$EVIDENCE/console.log" \
   && grep -aFq "OXIDE_TCG_ARM64_GUEST_INIT_OK" "$EVIDENCE/console.log"; then
  echo "RESULT=PASS_KERNEL_AND_ARM64_PID1" | tee -a "$EVIDENCE/result.txt"
  echo "LIMIT=Linux-only proof; Android and original Oxide APKs UNTESTED" | tee -a "$EVIDENCE/result.txt"
else
  echo "RESULT=FAIL_NO_VERIFIED_ARM64_GUEST_INIT" | tee -a "$EVIDENCE/result.txt"
  exit 1
fi
