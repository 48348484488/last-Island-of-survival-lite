#!/usr/bin/env python3
"""Read-only Redroid host capability report (stdlib only).

A Linux Android container needs Binder kernel devices; /dev/kvm is NOT required
for Redroid. This probe does not claim that an APK or even Android has booted.
"""
import argparse
import json
import os
import platform
import stat
from pathlib import Path

BINDER_DEVICES = ("binder", "hwbinder", "vndbinder")


def classify(nodes, binderfs_available=False, binderfs_binder_ready=False):
    missing = [name for name in BINDER_DEVICES if not nodes.get(name, False)]
    # Proven Android 15/16 workflow uses mounted /dev/binderfs/binder.
    ready = not missing or bool(binderfs_binder_ready)
    return {
        "binder_ready": ready,
        "missing_binder_devices": missing,
        "binderfs_available": bool(binderfs_available),
        "binderfs_binder_ready": bool(binderfs_binder_ready),
        "next_action": "BOOT_ANDROID" if ready else "HOST_NO_BINDER",
        "android_booted": False,
        "game_tested": False,
    }


def inspect(dev="/dev", proc="/proc"):
    dev, proc = Path(dev), Path(proc)
    def is_char(path):
        try:
            return stat.S_ISCHR(path.stat().st_mode)
        except OSError:
            return False

    nodes = {n: is_char(dev / n) for n in BINDER_DEVICES}
    nodes_in_binderfs = {n: is_char(dev / "binderfs" / n) for n in BINDER_DEVICES}
    filesystems = (proc / "filesystems").read_text(errors="replace") if (proc / "filesystems").exists() else ""
    result = classify(
        nodes,
        "binder" in filesystems or (dev / "binderfs" / "binder-control").exists(),
        nodes_in_binderfs.get("binder", False),
    )
    result.update({
        "kernel": platform.release(),
        "machine": platform.machine(),
        "host_device_nodes": nodes,
        "binderfs_nodes": nodes_in_binderfs,
        "binderfs_control": (dev / "binderfs" / "binder-control").exists(),
        "kvm_available": is_char(dev / "kvm"),
        "note": "KVM is informational only; Redroid can use legacy binder devices or mounted binderfs.",
    })
    return result


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--output", required=True)
    args = ap.parse_args()
    result = inspect()
    os.makedirs(os.path.dirname(os.path.abspath(args.output)), exist_ok=True)
    with open(args.output, "w", encoding="utf-8") as f:
        json.dump(result, f, indent=2, sort_keys=True)
        f.write("\n")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["binder_ready"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
