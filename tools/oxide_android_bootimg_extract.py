#!/usr/bin/env python3
"""Extract an Android boot/recovery image for a bounded QEMU ARM64 kernel probe.

Supports boot header v0-v4. Does not flash, modify, sign, or install APKs.
Image bytes and digest must be independently checked by the calling workflow.
"""
import argparse
import gzip
import json
import struct
import subprocess
from pathlib import Path

MAGIC = b"ANDROID!"
ARM64_MAGIC = bytes.fromhex("41524d64")  # bytes of 0x644d5241 at Image offset 0x38
MAX_KERNEL = 75_000_000
MAX_RAMDISK = 60_000_000


def u32(data, off):
    return struct.unpack_from("<I", data, off)[0]


def align(num, page):
    return (num + page - 1) // page * page


def extract(image: Path, dest: Path):
    size = image.stat().st_size
    with image.open("rb") as f:
        h = f.read(4096)
        if h[:8] != MAGIC:
            raise ValueError("Not an Android boot image (missing ANDROID! magic)")
        version = u32(h, 40)
        if version in (3, 4) and 1536 <= u32(h, 20) <= 4096:
            page = 4096
            ksize, rsize = u32(h, 8), u32(h, 12)
        elif version in (0, 1, 2):
            page = u32(h, 36)
            if page not in (2048, 4096, 8192, 16384):
                raise ValueError("Unrecognized legacy page size")
            ksize, rsize = u32(h, 8), u32(h, 16)
        else:
            raise ValueError("Unsupported boot header version")
        if not (0 < ksize <= MAX_KERNEL) or not (0 < rsize <= MAX_RAMDISK):
            raise ValueError("No usable kernel and recovery ramdisk")
        kstart = page
        rstart = page + align(ksize, page)
        if rstart + rsize > size:
            raise ValueError("Truncated boot image")
        f.seek(kstart)
        kernel = f.read(ksize)
        f.seek(rstart)
        ramdisk = f.read(rsize)

    dest.mkdir(parents=True, exist_ok=True)
    if kernel.startswith(b"\x1f\x8b"):
        kernel = gzip.decompress(kernel)
        compression = "gzip"
    elif kernel.startswith((b"\x02\x21\x4c\x18", b"\x04\x22\x4d\x18")):
        packed = dest / "kernel.lz4"
        packed.write_bytes(kernel)
        unpacked = dest / "Image"
        subprocess.run(["lz4", "-q", "-d", "-f", str(packed), str(unpacked)],
                       check=True, timeout=25)
        kernel = unpacked.read_bytes()
        packed.unlink()
        compression = "lz4"
    else:
        compression = "none/unknown"
    if len(kernel) < 64 or kernel[0x38:0x3c] != ARM64_MAGIC:
        raise ValueError("Extracted kernel is not a recognized ARM64 Linux Image")

    kernel_path = dest / "Image"
    ramdisk_path = dest / "ramdisk.img"
    kernel_path.write_bytes(kernel)
    ramdisk_path.write_bytes(ramdisk)
    report = {
        "header_version": version,
        "page_size": page,
        "original_size": size,
        "packed_kernel_bytes": ksize,
        "kernel_bytes": len(kernel),
        "kernel_compression": compression,
        "ramdisk_bytes": rsize,
        "ramdisk_magic_hex": ramdisk[:8].hex(),
        "result": "ARM64_ANDROID_RECOVERY_KERNEL_EXTRACTED_BOOT_UNTESTED",
        "next_step": "QEMU virt ARM64 kernel and recovery userspace proof; no APK",
    }
    (dest / "image-metadata.json").write_text(
        json.dumps(report, indent=2, sort_keys=True) + "\n")
    return report


def main():
    p = argparse.ArgumentParser()
    p.add_argument("image", type=Path)
    p.add_argument("output_dir", type=Path)
    args = p.parse_args()
    print(json.dumps(extract(args.image, args.output_dir), sort_keys=True))


if __name__ == "__main__":
    main()
