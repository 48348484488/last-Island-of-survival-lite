#!/usr/bin/env bash
# Verify real GUI input and visual feedback in an ARM64 QEMU Android recovery.
# No APK, game, Play services, credential, or Android security changes.
set -Eeuo pipefail
evidence="$PWD/evidence/oxide-android-recovery-input"
mkdir -p "$evidence"
echo 'RESULT=NOT_TESTED' > "$evidence/result.txt"
tmp="$(mktemp -d)"
qpid=''
cleanup() {
  status=$?
  if test -n "$qpid"; then kill "$qpid" 2>/dev/null || true; wait "$qpid" 2>/dev/null || true; fi
  if test -f "$tmp/serial.log"; then tail -c 200000 "$tmp/serial.log" > "$evidence/serial.log"; fi
  if test -f "$tmp/host.log"; then tail -c 200000 "$tmp/host.log" > "$evidence/qemu-host.log"; fi
  echo "SCRIPT_EXIT_CODE=$status" >> "$evidence/result.txt"
  rm -rf "$tmp"
}
trap cleanup EXIT

sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends \
  qemu-system-arm curl lz4 >/dev/null
curl --fail --location --silent --show-error --connect-timeout 10 \
  --max-time 75 --max-filesize 65000000 \
  'https://github.com/jqssun/android-lineage-qemu/releases/download/v2026.09.17/recovery_arm64only.img' \
  -o "$tmp/recovery.img"
test "$(stat -c '%s' "$tmp/recovery.img")" -eq 60821504
echo '08389076ebd424f9626dde48529e1e81a676b864b413d5fd37629a7826ff0158  '"$tmp/recovery.img" | sha256sum --check
python3 tools/oxide_android_bootimg_extract.py "$tmp/recovery.img" "$tmp/extracted"

qemu-system-aarch64 \
  -machine virt,accel=tcg -cpu max -smp 2 -m 2048 \
  -kernel "$tmp/extracted/Image" -initrd "$tmp/extracted/ramdisk.img" \
  -append 'console=ttyAMA0 earlycon=pl011,0x09000000 androidboot.hardware=virtio' \
  -device virtio-gpu-pci \
  -device qemu-xhci,id=usb-bus \
  -device usb-kbd,bus=usb-bus.0 \
  -device usb-tablet,bus=usb-bus.0 \
  -vnc 127.0.0.1:9 -monitor none \
  -serial file:"$tmp/serial.log" -no-reboot -net none \
  > "$tmp/host.log" 2>&1 &
qpid="$!"
sleep 28
kill -0 "$qpid"

python3 tools/oxide_qemu_vnc_capture.py \
  --output "$evidence/before.png" \
  --report "$evidence/before.json"

python3 tools/oxide_qemu_vnc_capture.py \
  --keysym 0xff54 \
  --output "$evidence/after-arrowdown.png" \
  --report "$evidence/after-arrowdown.json"

python3 tools/oxide_qemu_vnc_capture.py \
  --keysym 0x1008ff11 \
  --output "$evidence/after-volumedown.png" \
  --report "$evidence/after-volumedown.json"

python3 - "$evidence" <<'PY'
import json
import sys
from pathlib import Path
path = Path(sys.argv[1])
reports = {
    name: json.loads((path / f"{name}.json").read_text())
    for name in ("before", "after-arrowdown", "after-volumedown")
}
menu = {name: report["recovery_menu_rgb_samples"] for name, report in reports.items()}
changed = any(
    (menu["before"].get("240") != menu[name].get("240")
     and menu["before"].get("290") != menu[name].get("290"))
    for name in ("after-arrowdown", "after-volumedown")
)
result = {
    "recovery_screen_captured": all(x["framebuffer_captured"] for x in reports.values()),
    "usb_keyboard_attached": True,
    "key_events_sent": ["XK_Down", "XF86AudioLowerVolume"],
    "menu_samples": menu,
    "menu_highlight_pixel_change_observed": changed,
    "booted_full_android": False,
    "oxide_apk_tested": False
}
(path / "comparison.json").write_text(json.dumps(result, indent=2, sort_keys=True)+"\n")
if changed:
    status = "PASS_QEMU_ARM64_ANDROID_RECOVERY_GUI_INPUT_VISUAL_CHANGE"
else:
    status = "PARTIAL_GUI_FRAMES_CAPTURED_BUT_KEY_INPUT_NOT_VERIFIED"
with (path / "result.txt").open("a") as f:
    f.write(f"RESULT={status}\n")
print(json.dumps({"status": status, **result}, sort_keys=True))
if not changed:
    raise SystemExit(1)
PY
