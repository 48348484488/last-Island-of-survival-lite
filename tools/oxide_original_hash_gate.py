#!/usr/bin/env python3
"""Pin previously observed original Oxide 1.15.11966 APK split SHA-256 hashes.

These values come from unchanged APK inventory recorded by the real ARM64
runtime #38789434979. Matching hashes prove identical bytes to that inventory,
not their provenance, signature trust, certification, or gameplay.
"""
import argparse
import json
import re
from pathlib import Path

EXPECTED = {
    "base.apk": "86fb9f053266f3797e38d1da9903695e6b6e3cf3df86c55791e56cbe97416afa",
    "split_config.arm64_v8a.apk": "badd1db3bd588393bb12ff1463d587bce84c1dee58a7a18f8c3b3b41740b666c",
    "split_config.xxhdpi.apk": "244b24fd30969251c23fc833c4181d1a5e2d0e5cb0b7f5b82712f23aba1a62cb",
    "split_UnityDataAssetPack.apk": "f06fb7e755014a5df63b5b5cc87ea344d82d852d2748a964d9820e5245c921c5",
}


def verify_inventory(inventory, expected=EXPECTED):
    observed = inventory.get("files", {})
    if not isinstance(observed, dict):
        return {"status": "BLOCKED_INVALID_INVENTORY", "all_hashes_match": False, "files": {}}
    checks = {}
    for name, digest in expected.items():
        info = observed.get(name, {})
        if not isinstance(info, dict):
            info = {}
        actual = info.get("sha256", "")
        checks[name] = (bool(info.get("present")) and
                        type(info.get("size_bytes")) is int and info["size_bytes"] > 0 and
                        isinstance(actual, str) and bool(re.fullmatch(r"[a-f0-9]{64}", actual)) and
                        actual == digest)
    complete = inventory.get("complete") is True and set(observed) == set(expected)
    all_match = complete and all(checks.values())
    return {
        "schema_version": 1,
        "status": "ORIGINAL_SPLITS_MATCH_PINNED_HASHES" if all_match else "BLOCKED_UNVERIFIED_APK_SPLITS",
        "all_hashes_match": bool(all_match),
        "files": checks,
        "provenance_certified": False,
        "apk_files_modified": False,
        "gameplay_verified": False,
        "note": "Hashes pinned to prior observed release inventory; not a signature or origin attestation.",
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--inventory", type=Path, required=True)
    parser.add_argument("--output", type=Path, default=Path("reports/original-sha256-verdict.json"))
    args = parser.parse_args()
    try:
        inventory = json.loads(args.inventory.read_text(encoding="utf8"))
        result = verify_inventory(inventory)
    except (OSError, ValueError, TypeError) as exc:
        result = {"status": "BLOCKED_INVALID_INVENTORY", "all_hashes_match": False,
                  "error": type(exc).__name__, "gameplay_verified": False}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf8")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["all_hashes_match"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
