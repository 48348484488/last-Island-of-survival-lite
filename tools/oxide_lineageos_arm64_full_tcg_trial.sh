#!/usr/bin/env bash
# One bounded Linux x86_64 -> ARM64 QEMU/TCG boot of the complete third-party
# LineageOS UTM QEMU-virt release. It does NOT run/download the original Oxide APK.
set -Eeuo pipefail
evidence="$PWD/evidence/oxide-lineageos-arm64-full-tcg"
mkdir -p "$evidence"
printf '%s\n' "RESULT=NOT_VERIFIED" "APKS_DOWNLOADED=no" \
  "GOOGLE_PLAY_CERTIFIED=NOT_TESTED" "ORIGINAL_GAMEPLAY=NOT_TESTED" > "$evidence/result.txt"
tmp="$(mktemp -d)"
qpid=""
cleanup() {
  status=$?
  if test -n "$qpid"; then
    kill "$qpid" 2>/dev/null || true
    wait "$qpid" 2>/dev/null || true
  fi
  echo "SCRIPT_EXIT_CODE=$status" >> "$evidence/result.txt"
  rm -rf "$tmp"
}
trap cleanup EXIT

# Fail closed before the 1.12 GB image download if this host lacks headroom.
free_bytes="$(df -PB1 "$PWD" | awk 'END {print $4}')"
mem_kib="$(awk '/MemAvailable:/ {print $2}' /proc/meminfo)"
echo "HOST=$(uname -m) CPUS=$(nproc) FREE_BYTES=$free_bytes MEM_AVAILABLE_KIB=$mem_kib" | tee -a "$evidence/result.txt"
if (( free_bytes < 8000000000 || mem_kib < 6000000 || $(nproc) < 2 )); then
  echo "RESULT=DENIED_INSUFFICIENT_HOST_RESOURCES" | tee -a "$evidence/result.txt"
  exit 1
fi
sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm qemu-efi-aarch64 qemu-utils adb curl >/dev/null
qemu-system-aarch64 --version | head -n 1 | tee -a "$evidence/result.txt"
firmware=""
for f in /usr/share/AAVMF/AAVMF_CODE.fd /usr/share/qemu-efi-aarch64/QEMU_EFI.fd /usr/share/qemu-efi-aarch64/QEMU_EFI-pflash.raw; do
  if test -s "$f"; then firmware="$f"; break; fi
done
if test -z "$firmware"; then
  echo "RESULT=DENIED_MISSING_AARCH64_UEFI_FIRMWARE" | tee -a "$evidence/result.txt"
  exit 1
fi
echo "FIRMWARE=$firmware" | tee -a "$evidence/result.txt"

release_url="https://github.com/jqssun/android-lineage-qemu/releases/download/v2026.09.17/UTM-VM-lineage-23.2-20260917-jqssun-virtio_arm64only.zip"
echo "RELEASE=$release_url" | tee -a "$evidence/result.txt"
curl --fail --location --silent --show-error --retry 1 \
  --connect-timeout 15 --max-time 180 --max-filesize 1150000000 \
  "$release_url" -o "$tmp/lineage-arm64-virt.zip"
size="$(stat -c '%s' "$tmp/lineage-arm64-virt.zip")"
echo "ZIP_BYTES=$size" | tee -a "$evidence/result.txt"
test "$size" -eq 1124762006
echo '0a50afc821d905848b4560c33a5a706e6aee5ac414f85b31f9e52fc1ffabbd61  '"$tmp/lineage-arm64-virt.zip" | sha256sum --check
echo "ZIP_SHA256_VERIFIED=yes" | tee -a "$evidence/result.txt"

# No blind extraction: only three explicitly named QEMU images, refuse oversize,
# symlinks, missing duplicates, and paths outside the archive.
python3 - "$tmp/lineage-arm64-virt.zip" "$tmp" "$evidence" <<'PY'
from pathlib import Path
from zipfile import ZipFile
import json
import shutil
import stat
import sys
archive, target, evidence = Path(sys.argv[1]), Path(sys.argv[2]), Path(sys.argv[3])
expected = ("vda.qcow2", "vdb.qcow2", "efi_vars.fd")
entries = {}
with ZipFile(archive) as z:
    for member in z.infolist():
        if not "/Data/" in member.filename or member.is_dir():
            continue
        name = member.filename.rsplit("/", 1)[-1]
        if name not in expected:
            continue
        if (member.external_attr >> 16) & 0o170000 == stat.S_IFLNK:
            raise RuntimeError("Archive contains a symlink instead of disk image")
        if name in entries:
            raise RuntimeError("Duplicate VM component " + name)
        entries[name] = member
    if set(entries) != set(expected):
        raise RuntimeError("Required QEMU VM components unavailable: " + repr(sorted(entries)))
    sum_size = sum(m.file_size for m in entries.values())
    space = shutil.disk_usage(target).free
    if sum_size > 9_000_000_000 or sum_size + 2_000_000_000 > space:
        raise RuntimeError(f"Unsafe extracted size or insufficient space: {sum_size} available={space}")
    for name, member in entries.items():
        with z.open(member, "r") as source, (target / name).open("wb") as output:
            shutil.copyfileobj(source, output, 1 << 20)
report = {name: {"zip_member": m.filename, "uncompressed_bytes": m.file_size}
          for name, m in entries.items()}
(evidence / "vm-archive-metadata.json").write_text(json.dumps(report, indent=2) + "\n")
print(json.dumps(report, sort_keys=True))
PY

qemu-img info --output=json "$tmp/vda.qcow2" > "$evidence/vda-qemu-info.json"
qemu-img info --output=json "$tmp/vdb.qcow2" > "$evidence/vdb-qemu-info.json"
echo 'BOOT_STRATEGY=real LineageOS QEMU-virt qcow2 disks + guest EFI variables; software ARM64 TCG only' | tee -a "$evidence/result.txt"

# Boot complete Android (not recovery) with the matching UTM VM disks.
# Restrict guest network to outbound NAT and localhost ADB forwarding.
timeout --signal=TERM --kill-after=5s 315s \
  qemu-system-aarch64 \
    -machine virt,accel=tcg -cpu max -smp 2 -m 3072 \
    -drive if=pflash,unit=0,file="$firmware",format=raw,readonly=on \
    -drive if=pflash,unit=1,file="$tmp/efi_vars.fd",format=raw \
    -drive if=none,id=vda,file="$tmp/vda.qcow2",format=qcow2 \
    -device virtio-blk-pci,drive=vda,bootindex=0 \
    -drive if=none,id=vdb,file="$tmp/vdb.qcow2",format=qcow2 \
    -device virtio-blk-pci,drive=vdb,bootindex=1 \
    -netdev user,id=n0,restrict=on,hostfwd=tcp:127.0.0.1:5555-:5555 \
    -device virtio-net-pci,netdev=n0,romfile= \
    -device virtio-gpu-pci \
    -device virtio-rng-pci \
    -device virtio-serial \
    -device qemu-xhci,id=usb-bus \
    -device usb-tablet,bus=usb-bus.0 \
    -vnc 127.0.0.1:9 \
    -monitor unix:"$tmp/monitor.sock",server=on,wait=off \
    -serial file:"$tmp/serial.log" \
    -no-reboot \
    > "$evidence/qemu-host-stderr.log" 2>&1 &
qpid="$!"
echo "QEMU_STARTED_PID=$qpid" | tee -a "$evidence/result.txt"

booted=0
last_abi=""
started="$(date +%s)"
while (( $(date +%s) - started < 285 )); do
  if ! kill -0 "$qpid" 2>/dev/null; then
    echo "QEMU_EXITED_BEFORE_VERIFIED_BOOT=yes" | tee -a "$evidence/result.txt"
    break
  fi
  # The upstream VM documents adbd over Ethernet port 5555.
  timeout 4s adb connect 127.0.0.1:5555 >/dev/null 2>&1 || true
  boot="$(timeout 4s adb -s 127.0.0.1:5555 shell getprop sys.boot_completed 2>/dev/null | tr -d '\r\n' || true)"
  abi="$(timeout 4s adb -s 127.0.0.1:5555 shell getprop ro.product.cpu.abi 2>/dev/null | tr -d '\r\n' || true)"
  if test -n "$abi"; then last_abi="$abi"; fi
  if test "$boot" = 1 && test "$abi" = arm64-v8a; then
    booted=1
    break
  fi
  echo "PROBE_SECONDS=$(( $(date +%s) - started )) BOOT_COMPLETED=$boot ABI=$abi" >> "$evidence/poll.log"
  sleep 8
done
echo "WALL_BOOT_PROBE_SECONDS=$(( $(date +%s) - started ))" | tee -a "$evidence/result.txt"
echo "LAST_REPORTED_ABI=$last_abi" | tee -a "$evidence/result.txt"
if test "$booted" = 1; then
  echo 'BOOT_COMPLETED=1' | tee -a "$evidence/result.txt"
  timeout 8s adb -s 127.0.0.1:5555 shell getprop > "$evidence/guest-getprop.txt" 2>&1 || true
  timeout 8s adb -s 127.0.0.1:5555 shell pm path com.android.vending > "$evidence/guest-play-store.txt" 2>&1 || true
  timeout 8s adb -s 127.0.0.1:5555 shell pm path com.google.android.gms > "$evidence/guest-gms.txt" 2>&1 || true
  timeout 8s adb -s 127.0.0.1:5555 exec-out screencap -p > "$evidence/guest-screen.png" 2>/dev/null || true
fi

# Best-effort screenshot even on boot failure, using HMP (no display client).
python3 - "$tmp/monitor.sock" "$evidence/last-frame.ppm" <<'PY'
from pathlib import Path
import socket
import sys
sockpath = Path(sys.argv[1])
if sockpath.is_socket():
    try:
        s=socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
        s.settimeout(3)
        s.connect(str(sockpath))
        s.recv(8192)
        s.sendall(f"screendump {sys.argv[2]}\n".encode())
        s.recv(8192)
        s.close()
        print("HMP_SCREENSHOT_REQUESTED")
    except (OSError, TimeoutError) as exc:
        print("HMP_SCREENSHOT_UNAVAILABLE", type(exc).__name__)
PY
sleep 2
if test -f "$tmp/serial.log"; then
  tail -c 1500000 "$tmp/serial.log" > "$evidence/serial-console.log"
fi
echo '--- serial excerpts ---'
grep -aE 'Linux version|init:|Fatal|ERROR|mount|error|boot|Starting' "$evidence/serial-console.log" 2>/dev/null | tail -n 30 || true
if test "$booted" = 1; then
  echo "RESULT=PASS_ANDROID_ARM64_FULL_BOOT_NO_GAME" | tee -a "$evidence/result.txt"
else
  echo "RESULT=FAIL_ANDROID_ARM64_FULL_BOOT_NOT_CONFIRMED" | tee -a "$evidence/result.txt"
  exit 1
fi
