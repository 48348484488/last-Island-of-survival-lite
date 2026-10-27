#!/usr/bin/env python3
"""
fix-decompiled-conflicts.py

AssetRipper extracts every assembly from the original build, including
third-party packages that Unity already ships as managed Packages or as
prebuilt DLLs (TextMeshPro, DOTween, Unity Standard Assets, etc.).

When you open the resulting project in Unity, the decompiled .cs collides
with Unity's prebuilt versions:

  - error CS0246 / CS1061: missing internal types only present in the
    prebuilt DLL
  - error: "Assembly with name 'X' already exists" (asmdef collision)

This script removes the decompiled duplicates so Unity's built-in copy is
the single source of truth.

Usage:
    python3 fix-decompiled-conflicts.py <ExportedProject_path>

Idempotent — safe to run multiple times.
"""
import argparse
import os
import shutil
import sys

# Only directories that have an .asmdef inside *and* are also provided by a
# Unity Package listed in Packages/manifest.json. These conflicts produce the
# error: "Assembly with name 'X' already exists".
#
# DOTween is intentionally kept — Unity ships no built-in equivalent. The few
# decompiled DOTween43/46 errors are minor (missing internal-type references
# from advanced extension methods) and only fail on niche calls; they do not
# prevent the project from opening.
DUPLICATE_DIRS = [
    ("MonoScript/Unity.TextMeshPro",
     "decompiled copy of com.unity.textmeshpro (already in Packages/)"),
    ("MonoScript/Unity.Analytics.DataPrivacy",
     "decompiled copy of com.unity.modules.unityanalytics (already in Packages/)"),
]


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("project", help="Path to ExportedProject root")
    ap.add_argument("--dry-run", action="store_true",
                    help="Don't delete; just print what would be removed")
    args = ap.parse_args()

    assets = os.path.join(args.project, "Assets")
    if not os.path.isdir(assets):
        print(f"ERROR: {assets} not found", file=sys.stderr)
        sys.exit(2)

    removed = 0
    skipped = 0
    bytes_freed = 0

    for rel, reason in DUPLICATE_DIRS:
        path = os.path.join(assets, rel)
        meta = path + ".meta"

        if not os.path.exists(path):
            skipped += 1
            continue

        # Compute size for reporting.
        size = 0
        for root, _, files in os.walk(path):
            for f in files:
                try:
                    size += os.path.getsize(os.path.join(root, f))
                except OSError:
                    pass

        print(f"  REMOVE Assets/{rel}  ({size // 1024} KB) — {reason}")
        bytes_freed += size

        if not args.dry_run:
            shutil.rmtree(path)
            if os.path.exists(meta):
                os.remove(meta)
        removed += 1

    print()
    print(f"Removed   {removed} duplicate directories.")
    print(f"Skipped   {skipped} directories (not present).")
    print(f"Freed     {bytes_freed // 1024} KB ({bytes_freed // 1024 // 1024} MB).")


if __name__ == "__main__":
    main()
