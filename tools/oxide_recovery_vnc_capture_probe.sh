#!/usr/bin/env bash
# Verify QEMU VirtIO-GPU / VNC capture without requiring an Android ADB service.
# Only a checksum-pinned 60.8 MB community Android recovery image is downloaded.
set -Eeuo pipefail
evidence="$PWD/evidence/oxide-android-recovery-vnc"
mkdir -p "$evidence"
printf '%s\n' 'RESULT=NOT_PROVEN' 'GAME_APK_DOWNLOADED=no' 'FULL_ANDROID_BOOT_PROVEN=no' > "$evidence/result.txt"
tmp="$(mktemp -d)"
qpid=''
cleanup() {
  status=$?
  if test -n "$qpid"; then kill "$qpid" 2>/dev/null || true; wait "$qpid" 2>/dev/null || true; fi
  if test -f "$tmp/serial.log"; then tail -c 1000000 "$tmp/serial.log" > "$evidence/serial.log"; fi
  if test -f "$tmp/host.log"; then tail -c 200000 "$tmp/host.log" > "$evidence/qemu-host.log"; fi
  echo "SCRIPT_EXIT_CODE=$status" >> "$evidence/result.txt"
  rm -rf "$tmp"
}
trap cleanup EXIT

sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm lz4 curl >/dev/null
echo "QEMU=$(qemu-system-aarch64 --version | head -n1)" | tee -a "$evidence/result.txt"
url="https://github.com/jqssun/android-lineage-qemu/releases/download/v2026.09.17/recovery_arm64only.img"
curl --fail --location --silent --show-error --connect-timeout 10 \
  --max-time 75 --max-filesize 65000000 "$url" -o "$tmp/recovery.img"
test "$(stat -c '%s' "$tmp/recovery.img")" -eq 60821504
echo '08389076ebd424f9626dde48529e1e81a676b864b413d5fd37629a7826ff0158  '"$tmp/recovery.img" | sha256sum --check
python3 tools/oxide_android_bootimg_extract.py "$tmp/recovery.img" "$tmp/extracted"
echo 'RECOVERY_SHA256_VERIFIED=yes' | tee -a "$evidence/result.txt"

qemu-system-aarch64 \
  -machine virt,accel=tcg -cpu max -smp 2 -m 2048 \
  -kernel "$tmp/extracted/Image" -initrd "$tmp/extracted/ramdisk.img" \
  -append 'console=ttyAMA0 earlycon=pl011,0x09000000 androidboot.hardware=virtio' \
  -device virtio-gpu-pci -vnc 127.0.0.1:9 \
  -monitor unix:"$tmp/monitor.sock",server=on,wait=off \
  -serial file:"$tmp/serial.log" -no-reboot -net none \
  > "$tmp/host.log" 2>&1 &
qpid="$!"
echo "QEMU_PID=$qpid" | tee -a "$evidence/result.txt"
sleep 27
if ! kill -0 "$qpid" 2>/dev/null; then
  echo 'RESULT=QEMU_EXITED_EARLY' | tee -a "$evidence/result.txt"
  exit 1
fi

python3 - "$tmp/monitor.sock" "$evidence/recovery-vnc.ppm" "$evidence" <<'PY'
import json
import socket
import sys
from pathlib import Path

monitor, output, root = Path(sys.argv[1]), Path(sys.argv[2]), Path(sys.argv[3])
record = {"vnc_banner": None, "hmp_response": None, "screenshot_created": False}
try:
    with socket.create_connection(("127.0.0.1", 5909), timeout=3) as s:
        record["vnc_banner"] = s.recv(20).decode("ascii", "replace").strip()
except OSError as exc:
    record["vnc_error"] = type(exc).__name__ + ": " + str(exc)
try:
    with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as s:
        s.settimeout(5)
        s.connect(str(monitor))
        banner = s.recv(8192).decode("utf8", "replace")
        s.sendall(("screendump " + str(output) + "\n").encode())
        reply = s.recv(8192).decode("utf8", "replace")
        record["hmp_banner"] = banner[:500]
        record["hmp_response"] = reply[:1200]
except OSError as exc:
    record["hmp_error"] = type(exc).__name__ + ": " + str(exc)
record["screenshot_created"] = output.is_file() and output.stat().st_size > 64
if record["screenshot_created"]:
    record["screenshot_bytes"] = output.stat().st_size
    record["screenshot_magic"] = output.open("rb").read(2).decode("ascii","replace")
(root / "vnc-capture.json").write_text(json.dumps(record, indent=2, sort_keys=True)+"\n")
print(json.dumps(record, sort_keys=True))
PY

if test -s "$evidence/recovery-vnc.ppm" && head -c2 "$evidence/recovery-vnc.ppm" | grep -Fq 'P6'; then
  echo 'RESULT=PASS_GRAPHICS_FRAMEBUFFER_CAPTURED_RECOVERY_ONLY' | tee -a "$evidence/result.txt"
elif grep -Fq 'RFB ' "$evidence/vnc-capture.json"; then
  echo 'RESULT=PARTIAL_VNC_AVAILABLE_NO_IMAGE' | tee -a "$evidence/result.txt"
else
  echo 'RESULT=FAIL_NO_GRAPHICS_CAPTURE_OR_VNC' | tee -a "$evidence/result.txt"
  exit 1
fi
