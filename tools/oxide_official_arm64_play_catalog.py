#!/usr/bin/env python3
"""Inspect Google's official SDK catalog for ARM64 Play Store AVD images.

Metadata-only; downloads no Android image, emulator, Play services or game.
An image listing does not establish emulator boot, certification or gameplay.
"""
import argparse
import json
import re
import urllib.error
import urllib.request
import xml.etree.ElementTree as ET
from pathlib import Path

BASE = "https://dl.google.com/android/repository/sys-img/google_apis_playstore/"
CATALOGS = tuple(BASE + f"sys-img2-{n}.xml" for n in (3, 2, 1))
NAME = re.compile(r"^system-images;android-(\d+)(?:-ext\d+)?;google_apis_playstore;arm64-v8a$")


def local_name(tag):
    return tag.rsplit("}", 1)[-1]


def parse_catalog(data):
    root = ET.fromstring(data)
    images = []
    for node in root.iter():
        if local_name(node.tag) != "remotePackage":
            continue
        path = node.attrib.get("path", "")
        match = NAME.fullmatch(path)
        if not match:
            continue
        if any(local_name(x.tag) == "obsolete" and (x.text or "").strip() == "true"
               for x in node):
            continue
        channel = next((x.attrib.get("ref", "") for x in node
                        if local_name(x.tag) == "channelRef"), "channel-0")
        if channel != "channel-0":
            continue
        version = next((x for x in node if local_name(x.tag) == "revision"), None)
        major = None
        if version is not None:
            major = next((int(x.text) for x in version if local_name(x.tag) == "major"
                          and (x.text or "").isdigit()), None)
        sizes = []
        for x in node.iter():
            if local_name(x.tag) == "size" and (x.text or "").strip().isdigit():
                sizes.append(int(x.text.strip()))
        images.append({"path": path, "api_level": int(match.group(1)),
                       "revision": major,
                       "min_archive_size_bytes": min(sizes) if sizes else None,
                       "channel": "stable"})
    return sorted(images, key=lambda x: (x["api_level"], x["revision"] or 0),
                  reverse=True)


def probe(opener=None):
    if opener is None:
        def opener(url):
            request = urllib.request.Request(url, headers={
                "User-Agent": "Oxide-open-Android-SDK-metadata-only/1.0"})
            with urllib.request.urlopen(request, timeout=25) as resp:
                return resp.read(16 * 1024 * 1024 + 1)

    errors = []
    for url in CATALOGS:
        try:
            data = opener(url)
            if len(data) > 16 * 1024 * 1024:
                raise ValueError("catalog too large")
            images = parse_catalog(data)
        except (OSError, ValueError, ET.ParseError, urllib.error.URLError) as exc:
            errors.append({"catalog": url, "error": type(exc).__name__})
            continue
        candidates = [x for x in images if x["api_level"] >= 35]
        return {
            "status": "OFFICIAL_ARM64_PLAY_IMAGE_CATALOGED_BOOT_UNTESTED" if candidates
                      else "NO_RECENT_ARM64_PLAY_IMAGE_IN_THIS_CATALOG",
            "official_catalog_url": url,
            "matching_images": images,
            "recent_api_35_or_newer": candidates,
            "catalog_checks_skipped_due_to_errors": errors,
            "android_emulator_installed": False,
            "android_booted": False,
            "play_store_installed_verified": False,
            "play_integrity_verdict": "NOT_OBSERVED",
            "apk_downloaded": False,
            "gameplay_verified": False,
            "note": "ARM64 Play image metadata only, not proof of legitimate device, emulator compatibility or Play Integrity.",
        }
    return {
        "status": "OFFICIAL_CATALOG_UNAVAILABLE",
        "catalog_checks_skipped_due_to_errors": errors,
        "matching_images": [],
        "recent_api_35_or_newer": [],
        "android_booted": False,
        "apk_downloaded": False,
        "gameplay_verified": False,
    }


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--output", type=Path, default=Path("reports/official-arm64-play-catalog.json"))
    args = p.parse_args()
    result = probe()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, sort_keys=True))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
