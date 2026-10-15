#!/usr/bin/env python3
"""Read-only inventory for original Oxide APK splits and optional release asset."""
import argparse
import hashlib
import json
from pathlib import Path

NAMES = ("base.apk", "split_config.arm64_v8a.apk", "split_config.xxhdpi.apk",
         "split_UnityDataAssetPack.apk")

def audit(directory):
    files = {}
    for name in NAMES:
        p = directory / name
        if not p.is_file():
            files[name] = {"present": False}
            continue
        h = hashlib.sha256()
        with p.open("rb") as f:
            for chunk in iter(lambda: f.read(1024 * 1024), b""):
                h.update(chunk)
        files[name] = {"present": True, "size_bytes": p.stat().st_size, "sha256": h.hexdigest()}
    return {"files": files, "complete": all(x["present"] for x in files.values()),
            "original_files_modified": False, "runtime_tested": False,
            "note": "Hash inventory only; does not prove provenance, install, boot or gameplay."}

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--directory", type=Path, default=Path("."))
    parser.add_argument("--output", type=Path, default=Path("reports/oxide-1.15.11966-apk-inventory.json"))
    args = parser.parse_args()
    result = audit(args.directory)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, indent=2, sort_keys=True))

if __name__ == "__main__":
    main()
