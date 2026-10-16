#!/usr/bin/env bash
# Evidence-gated VirtIO console probe for LineageOS QEMU ARM64.
# Full Android is NOT downloaded, and no Oxide APK is installed.
set -Eeuo pipefail
evidence="$PWD/evidence/oxide-arm64-virtconsole"
mkdir -p "$evidence"
echo "RESULT=NOT_YET_VERIFIED" > "$evidence/result.txt"
tmp="$(mktemp -d)"
trap 'status=$?; echo "SCRIPT_EXIT=$status" >> "$evidence/result.txt"; rm -rf "$tmp"' EXIT
echo "HOST=$(uname -m)" | tee -a "$evidence/result.txt"
echo "SCOPE=two observation consoles + small Android recovery only" >> "$evidence/result.txt"
echo "GAME_APK_DOWNLOADED=no" >> "$evidence/result.txt"

sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm curl lz4 >/dev/null
qemu-system-aarch64 --version | head -1 | tee -a "$evidence/result.txt"

url="https://github.com/jqssun/android-lineage-qemu/releases/download/v2026.09.17/recovery_arm64only.img"
curl --fail --location --silent --show-error --retry 1 \
  --connect-timeout 10 --max-time 75 --max-filesize 65000000 \
  "$url" -o "$tmp/recovery.img"
test "$(stat -c '%s' "$tmp/recovery.img")" -eq 60821504
echo "08389076ebd424f9626dde48529e1e81a676b864b413d5fd37629a7826ff0158  $tmp/recovery.img" | sha256sum -c
echo 'RECOVERY_IMAGE_SHA256=VERIFIED' | tee -a "$evidence/result.txt"
python3 tools/oxide_android_bootimg_extract.py "$tmp/recovery.img" "$tmp/extracted" > "$evidence/image-extract.json"

start="$(date +%s)"
set +e
timeout --signal=TERM --kill-after=5s 55s \
  qemu-system-aarch64 \
    -machine virt,accel=tcg -cpu max -smp 2 -m 2048 \
    -kernel "$tmp/extracted/Image" -initrd "$tmp/extracted/ramdisk.img" \
    -append 'console=ttyAMA0 earlycon=pl011,0x09000000 androidboot.hardware=virtio loglevel=6' \
    -device virtio-gpu-pci \
    -device virtio-serial-pci,id=vs0 \
    -chardev file,id=guest_console,path="$tmp/hvc0.log" \
    -device virtconsole,chardev=guest_console,bus=vs0.0 \
    -chardev file,id=guest_logcat,path="$tmp/hvc1.log" \
    -device virtconsole,chardev=guest_logcat,bus=vs0.0 \
    -display none -monitor none -serial file:"$tmp/serial.log" \
    -no-reboot -net none \
    > "$tmp/qemu-host.log" 2>&1
rc=$?
set -e
echo "QEMU_EXIT=$rc" | tee -a "$evidence/result.txt"
echo "QEMU_WALL_SECONDS=$(( $(date +%s) - start ))" | tee -a "$evidence/result.txt"
for name in serial hvc0 hvc1; do
  if test -f "$tmp/$name.log"; then
    tail -c 350000 "$tmp/$name.log" > "$evidence/$name.log"
    echo "$name LENGTH=$(stat -c %s "$tmp/$name.log")" | tee -a "$evidence/result.txt"
  else
    : > "$evidence/$name.log"
    echo "$name LENGTH=0" | tee -a "$evidence/result.txt"
  fi
done
tail -c 30000 "$tmp/qemu-host.log" > "$evidence/qemu-host.log"

python3 - "$evidence" <<'PY'
from pathlib import Path
import json, sys
p=Path(sys.argv[1])
serial=(p/"serial.log").read_text(errors="replace")
host=(p/"qemu-host.log").read_text(errors="replace")
a=(p/"hvc0.log").stat().st_size
b=(p/"hvc1.log").stat().st_size
checks={
 "arm64_kernel_executed":"Linux version" in serial,
 "android_recovery_executed":"recovery:" in serial,
 "virtconsole_configuration_rejected":any(s in host.lower() for s in ("invalid","error","failed")),
 "hvc0_output_bytes":a,
 "hvc1_output_bytes":b,
 "android_full_boot_confirmed":False,
 "original_oxide_gameplay_confirmed":False
}
if checks["arm64_kernel_executed"] and checks["android_recovery_executed"] and not checks["virtconsole_configuration_rejected"]:
 status="PASS_RECOVERY_TWO_ATTACHED_VIRTCONSOLES_SHELL_LOGCAT_NOT_YET_PROVEN"
else:status="FAIL_RECOVERY_OR_VIRTCONSOLE_CONFIG"
(p/"classification.json").write_text(json.dumps({"status":status,"checks":checks},indent=2,sort_keys=True)+"\n")
with (p/"result.txt").open("a") as f:f.write("RESULT="+status+"\n")
print(json.dumps({"status":status,"checks":checks},sort_keys=True))
if status.startswith("FAIL"):raise SystemExit(1)
PY
