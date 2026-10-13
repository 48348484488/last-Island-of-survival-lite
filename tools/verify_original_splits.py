#!/usr/bin/env python3
"""Read-only SHA-256 verification of the four original Oxide 1.14.11949 splits.

Expected digests are from reports/1.14.11949-static-audit.md.
No APK modifications, re-signing, or distribution are performed.
"""
import argparse
import hashlib
import json
from pathlib import Path

EXPECTED_SHA256 = {
    "base.apk": "93deabf8c762defe23321c83fdb89559812bd0c9df851de738070143e99c7e62",
    "split_config.arm64_v8a.apk": "504c7e3ba1bff9fdc7e8a3f963f1a3eccb4b1bf93b093dc855c627d832dc931a",
    "split_config.xxhdpi.apk": "3a54364868fa2a0042105f1799c5ae47b601d8c75361c3763e1fa1a6785086d4",
    "split_UnityDataAssetPack.apk": "2f0f7f1be7d78d269f710edd471c40a9755a93a07481cae0c058356515e19834",
}


def sha256_file(path):
    h = hashlib.sha256()
    with path.open("rb") as stream:
        while True:
            data = stream.read(4 * 1024 * 1024)
            if not data:
                return h.hexdigest()
            h.update(data)


def verify(directory, expected=None):
    directory = Path(directory)
    if expected is None:
        expected = EXPECTED_SHA256
    entries = []
    for name, digest in expected.items():
        target = directory / name
        if not target.is_file():
            entries.append({"file": name, "status": "MISSING", "sha256": None})
            continue
        try:
            actual = sha256_file(target)
        except OSError:
            entries.append({"file": name, "status": "READ_ERROR", "sha256": None})
            continue
        entries.append({
            "file": name,
            "status": "PASS" if actual == digest.lower() else "MISMATCH",
            "sha256": actual,
        })
    return {
        "version": "1.14.11949",
        "status": "PASS" if all(x["status"] == "PASS" for x in entries) else "FAIL",
        "entries": entries,
        "reference": "reports/1.14.11949-static-audit.md",
        "meaning": "Exact-file integrity only, not compatibility, licensing, or game stability",
    }


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("apk_directory", type=Path)
    ap.add_argument("--output", type=Path, help="Optional JSON result path")
    opts = ap.parse_args()
    result = verify(opts.apk_directory)
    summary = json.dumps(result, indent=2, ensure_ascii=False) + "\n"
    if opts.output:
        opts.output.parent.mkdir(parents=True, exist_ok=True)
        opts.output.write_text(summary, encoding="utf-8")
    print(summary)
    return 0 if result["status"] == "PASS" else 2


if __name__ == "__main__":
    raise SystemExit(main())
