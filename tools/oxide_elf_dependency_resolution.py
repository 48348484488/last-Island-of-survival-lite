#!/usr/bin/env python3
"""Reconcile prior original Oxide DT_NEEDED JSON without reading APKs or ELF again.

Matches ELF-declared names to packaged files and documented Android NDK
system-library names. Does not prove availability in Redroid/AVD, application
initialization, crash causation, security verdict, or gameplay.
"""
import argparse
import hashlib
import json
from collections import Counter
from pathlib import Path

if __package__:
    from .oxide_native_library_inventory import EXPECTED_PINNED_HASHES
else:
    from oxide_native_library_inventory import EXPECTED_PINNED_HASHES

# Name allowlist grounded in Android NDK documentation, not guest runtime checks.
ANDROID_NDK_SYSTEM_NAMES = frozenset({
    "libEGL.so", "libOpenSLES.so", "libandroid.so", "libc.so", "libdl.so",
    "liblog.so", "libm.so", "libmediandk.so", "libz.so",
})
BURST_LOOKUPS = ("lib_burst_0_0.so", "liblib_burst_0_0.so")


def summarize_dependency_edges(source_bytes):
    try:
        report = json.loads(source_bytes)
    except (UnicodeDecodeError, ValueError, TypeError):
        return {"status": "BLOCKED_INVALID_SOURCE_JSON", "gameplay_verified": False}
    if not isinstance(report, dict):
        return {"status": "BLOCKED_INVALID_SOURCE_STRUCTURE", "gameplay_verified": False}
    originals = report.get("original_apk_sha256_verified")
    if (report.get("status") != "ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_READ_ONLY_VERIFIED"
            or report.get("all_packaged_arm64_libraries_examined") is not True
            or report.get("large_asset_pack_inspected") is not False
            or report.get("large_libraries_not_examined") != []
            or not isinstance(originals, dict)
            or set(originals) != set(EXPECTED_PINNED_HASHES)):
        return {"status": "BLOCKED_PARTIAL_OR_UNVERIFIED_ELF_COVERAGE",
                "gameplay_verified": False}
    for name, digest in EXPECTED_PINNED_HASHES.items():
        metadata = originals[name]
        if not isinstance(metadata, dict) or metadata.get("sha256") != digest:
            return {"status": "BLOCKED_ORIGINAL_APK_SHA_MISMATCH",
                    "gameplay_verified": False}
    libs = report.get("libraries")
    if (not isinstance(libs, list) or not libs
            or report.get("arm64_libraries_inspected") != len(libs)):
        return {"status": "BLOCKED_LIBRARY_COUNT_MISMATCH", "gameplay_verified": False}
    names = []
    edges = Counter()
    for lib in libs:
        if not isinstance(lib, dict) or not isinstance(lib.get("path"), str):
            return {"status": "BLOCKED_LIBRARY_METADATA", "gameplay_verified": False}
        path = lib["path"]
        name = path.rsplit("/", 1)[-1]
        if not path.startswith("lib/arm64-v8a/") or not name.endswith(".so"):
            return {"status": "BLOCKED_LIBRARY_METADATA", "gameplay_verified": False}
        names.append(name)
        needed = lib.get("dt_needed")
        if not isinstance(needed, list) or any(
                not isinstance(n, str) or not n.endswith(".so") or "/" in n
                for n in needed):
            return {"status": "BLOCKED_DYNAMIC_LIBRARY_NAMES", "gameplay_verified": False}
        edges.update(needed)
    if len(names) != len(set(names)):
        return {"status": "BLOCKED_DUPLICATE_LIBRARY_BASENAME", "gameplay_verified": False}
    packaged = set(names)
    packaged_edges = sorted((name, count) for name, count in edges.items()
                            if name in packaged)
    ndk_edges = sorted((name, count) for name, count in edges.items()
                       if name not in packaged and name in ANDROID_NDK_SYSTEM_NAMES)
    unresolved = sorted((name, count) for name, count in edges.items()
                        if name not in packaged and name not in ANDROID_NDK_SYSTEM_NAMES)
    return {
        "status": ("ELF_DECLARED_DEPENDENCY_NAMES_ACCOUNTED_FOR"
                   if not unresolved else "UNACCOUNTED_FOR_DECLARED_DEPENDENCY_NAMES"),
        "source_artifact_sha256": hashlib.sha256(source_bytes).hexdigest(),
        "original_arm64_libraries": len(libs),
        "distinct_declared_names": len(edges),
        "total_declared_dependency_edges": sum(edges.values()),
        "packaged_dependency_edges": sum(x[1] for x in packaged_edges),
        "expected_ndk_system_dependency_edges": sum(x[1] for x in ndk_edges),
        "unaccounted_for_dependency_edges": sum(x[1] for x in unresolved),
        "packaged_dependency_names": dict(packaged_edges),
        "android_ndk_system_library_names_not_runtime_verified": dict(ndk_edges),
        "unaccounted_for_names": dict(unresolved),
        "burst_lookup_names_declared_as_dt_needed": {
            name: edges.get(name, 0) for name in BURST_LOOKUPS
        },
        "actual_android_linker_success": "NOT_ESTABLISHED",
        "security_trigger": "UNKNOWN",
        "original_gameplay_verified": False,
        "large_asset_pack_examined": False,
        "note": "NDK platform name is not proof of runtime linker access. "
                "Missing dlopen names without DT_NEEDED do not establish optional "
                "fallback or a cause of app exit.",
    }


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--input", type=Path, required=True)
    p.add_argument("--output", type=Path, required=True)
    args = p.parse_args()
    result = summarize_dependency_edges(args.input.read_bytes())
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, sort_keys=True))
    return 0 if result["status"] == "ELF_DECLARED_DEPENDENCY_NAMES_ACCOUNTED_FOR" else 2


if __name__ == "__main__":
    raise SystemExit(main())
