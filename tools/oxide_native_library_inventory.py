#!/usr/bin/env python3
"""Read-only ZIP index and pinned-hash native library metadata for Oxide APK splits.

Never extracts, executes, patches, republishes or signs any game content.
The large asset-pack split is explicitly outside the inspected scope.
"""
import argparse
import hashlib
import json
from collections import Counter
from pathlib import Path
from zipfile import BadZipFile, ZipFile

EXPECTED_PINNED_HASHES = {
    "base.apk": "86fb9f053266f3797e38d1da9903695e6b6e3cf3df86c55791e56cbe97416afa",
    "split_config.arm64_v8a.apk": "badd1db3bd588393bb12ff1463d587bce84c1dee58a7a18f8c3b3b41740b666c",
    "split_config.xxhdpi.apk": "244b24fd30969251c23fc833c4181d1a5e2d0e5cb0b7f5b82712f23aba1a62cb",
}
LIBRARY_NAMES = (
    "lib_burst_generated.so", "liblib_burst_0_0.so", "lib_burst_0_0.so",
)


def sha256_file(path):
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def inspect_apk_index(directory, expected_hashes=EXPECTED_PINNED_HASHES):
    """Fail closed on mismatched inputs; otherwise read ZIP metadata only."""
    directory = Path(directory)
    checks = {}
    for apk, expected in expected_hashes.items():
        path = directory / apk
        if not path.is_file():
            checks[apk] = {"present": False, "sha256_matches": False}
            continue
        checks[apk] = {
            "present": True, "size_bytes": path.stat().st_size,
            "sha256_matches": sha256_file(path) == expected,
        }
    proven = bool(checks) and all(v["present"] and v["sha256_matches"]
                                  for v in checks.values())
    result = {
        "status": "UNVERIFIED_OR_MISSING_ORIGINAL_SPLITS",
        "inspected_apks": checks,
        "pinned_hashes_match": proven,
        "large_uninspected_split": "split_UnityDataAssetPack.apk",
        "all_four_splits_inspected": False,
        "original_apks_modified": False,
        "original_gameplay_verified": False,
        "causal_link_to_original_exit": "NOT_ESTABLISHED",
    }
    if not proven:
        return result
    native_entries = {}
    counts = Counter()
    try:
        for apk in expected_hashes:
            with ZipFile(directory / apk, "r") as archive:
                entries = archive.infolist()  # Central directory only; no decompression.
                names = [entry.filename for entry in entries]
                if len(names) != len(set(names)):
                    result["status"] = "AMBIGUOUS_DUPLICATE_ZIP_ENTRIES"
                    return result
                library_paths = [name for name in names
                                 if name.startswith("lib/") and name.endswith(".so")
                                 and not name.endswith("/.so")]
                native_entries[apk] = len(library_paths)
                for path in library_paths:
                    if path.rsplit("/", 1)[-1] in LIBRARY_NAMES:
                        counts[(apk, path)] += 1
    except (BadZipFile, OSError, ValueError):
        result["status"] = "APK_CENTRAL_DIRECTORY_UNREADABLE"
        return result
    exact_matches = {
        name: [{"apk": apk, "zip_entry": path}
               for (apk, path), amount in sorted(counts.items())
               if path.rsplit("/", 1)[-1] == name for _ in range(amount)]
        for name in LIBRARY_NAMES
    }
    missing_lookups = all(not exact_matches[name] for name in LIBRARY_NAMES[1:])
    result.update({
        "status": "THREE_PINNED_ORIGINAL_APK_ZIP_INDEXES_VERIFIED",
        "native_library_entry_count": native_entries,
        "exact_library_matches": exact_matches,
        "both_lookup_names_absent_from_inspected_splits": missing_lookups,
        "absence_in_large_uninspected_split": "UNKNOWN",
        "loadable_at_runtime": "NOT_ESTABLISHED",
        "interpretation": (
            "ZIP filename absence in the three inspected splits cannot establish "
            "that a required dependency is missing, nor distinguish fallback "
            "from policy-triggered app exit. A fourth large split was not read."
        ),
    })
    return result


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--directory", type=Path, default=Path("."))
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    verdict = inspect_apk_index(args.directory)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(verdict, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(json.dumps(verdict, sort_keys=True))
    return 0 if verdict["status"] == "THREE_PINNED_ORIGINAL_APK_ZIP_INDEXES_VERIFIED" else 2


if __name__ == "__main__":
    raise SystemExit(main())
