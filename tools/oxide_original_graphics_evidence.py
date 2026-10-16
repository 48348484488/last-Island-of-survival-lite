#!/usr/bin/env python3
"""Read-only original Oxide ARM64 graphics initialization and screenshot evidence.

Uses existing unchanged Redroid artifacts only. Graphics driver warnings before
ANGLE renderer and Unity logs do not prove successful gameplay or causality.
Known screenshot annotations are manual and apply ONLY to exact pinned hashes.
No APK patching, game launch, OCR, security intervention, or device spoofing.
"""
import argparse
import hashlib
import json
from datetime import datetime
from pathlib import Path
import re

PKG = "com.catsbit.oxidesurvivalisland"
ACTIVITY = PKG + "/.OxideActivity"
SHA256_LOGCAT = {
    "android15": "bcc57f460584553fe860fab3995f4d930b88ce9e8e6eb370ff116a9932d70c4c",
    "android16": "776a2394656e5cc802c1f267aae7b36fd4c90b295f7b9a0527ef86ec96b8f120",
}
EXPECTED_PIDS = {"android15": 2023, "android16": 2108}
# Human-reviewed ORIGINAL screenshots. Do not generalize beyond these hashes.
REVIEWED_IMMERSIVE_DIALOG_PNG_SHA256 = {
    "screen-2.png": "bce5e031114128114e40136b32a45eaad3d447f93c97f93ca0af403294e0d302",
    "screen-3.png": "43046a1e4e7c7451be0ff8e9de1d047d7f4d1f9acd4a7802a6fea87e9e5ae1e7",
    "screen-4.png": "95a08684dd1ada11c46db45e354b39f3d4284cd41be1d9c0b58367846cb01d33",
}
LINE = re.compile(
    r"^(?P<date>\d\d-\d\d) (?P<clock>\d\d:\d\d:\d\d\.\d+)\s+"
    r"(?P<pid>\d+)\s+(?P<tid>\d+)\s+(?P<level>[A-Z])\s+"
    r"(?P<tag>[^:]+):\s?(?P<message>.*)$")
START = re.compile(r"START u0.*cmp=" + re.escape(ACTIVITY))
PROC = re.compile(r"Start proc (?P<pid>\d+):" + re.escape(PKG) + r"/")
FAIL_BUFFER = re.compile(
    r"Failed to allocate \(4 x 4\) layerCount 1 format (?P<format>43|56|59) usage b00:")
SIGKILL = re.compile(r"Sending signal\. PID: (?P<pid>\d+) SIG: 9(?:\D|$)")


def timestamp(date, clock):
    return datetime.strptime("2026-" + date + " " + clock, "%Y-%m-%d %H:%M:%S.%f")


def diff_ms(start, finish):
    return round((finish - start).total_seconds() * 1000, 3)


def sha256(data):
    return hashlib.sha256(data).hexdigest()


def review_exact_screen(data, filename):
    """Recognize ONLY a manually reviewed, pinned ORIGINAL image."""
    expected = REVIEWED_IMMERSIVE_DIALOG_PNG_SHA256.get(filename)
    if not expected or not data.startswith(b"\x89PNG\r\n\x1a\n") or sha256(data) != expected:
        return {"status": "NOT_VISUALLY_REVIEWED", "sha256": sha256(data),
                "gameplay_evidence": False}
    return {
        "status": "EXACT_ORIGINAL_SCREEN_REVIEWED_ANDROID_IMMERSIVE_DIALOG",
        "sha256": expected,
        "observation": "Android system Viewing full screen prompt partly obscures app",
        "gameplay_evidence": False,
    }


def analyze_original(logcat, badging, expected_pid, expected_sha):
    """Fail closed unless original PID, bytes and chronological milestones agree."""
    if sha256(logcat) != expected_sha:
        return {"status": "BLOCKED_ORIGINAL_LOGCAT_SHA_MISMATCH", "validated": False}
    manifest = badging.decode("utf8", errors="replace")
    manifest_good = (
        "versionName='1.15.11966'" in manifest
        and f"package: name='{PKG}'" in manifest
        and "  uses-gl-es: '0x30002'" in manifest
        and "uses-feature-not-required: name='android.hardware.vulkan.version'" in manifest
    )
    if not manifest_good:
        return {"status": "BLOCKED_ORIGINAL_GRAPHICS_MANIFEST_MISMATCH", "validated": False}

    start = process = None
    events = {}
    format_failures = set()
    angle_tid = None
    for line in logcat.decode("utf8", errors="replace").splitlines():
        m = LINE.match(line)
        if not m:
            continue
        at = timestamp(m["date"], m["clock"])
        msg = m["message"]
        pid, tid = int(m["pid"]), int(m["tid"])
        tag = m["tag"].strip()
        if start is None and START.search(msg):
            start = at
            events["original_activity_start"] = at
        match_proc = PROC.search(msg)
        if start is not None and process is None and match_proc:
            process = int(match_proc["pid"])
        if pid != expected_pid or process != expected_pid:
            continue
        if tag == "libEGL" and "ANGLE Warn:vulkan_icd.cpp" in msg:
            events.setdefault("preferred_vulkan_icd_not_found", at)
            angle_tid = tid
        if tag == "SwiftShader" and "UNSUPPORTED: curExtension->sType" in msg:
            events.setdefault("swiftshader_unsupported_extension_warning", at)
        if tag == "Gralloc4" and "mapper 4.x is not supported" in msg:
            events.setdefault("gralloc_mapper_4_unsupported", at)
        if tag == "GraphicBufferAllocator":
            f = FAIL_BUFFER.search(msg)
            if f:
                format_failures.add(int(f["format"]))
                events.setdefault("hardware_buffer_format_probe_failed", at)
        if tag == "ANGLE" and "Renderer (Vulkan 1.3.0 (SwiftShader Device" in msg:
            if angle_tid == tid:
                events.setdefault("angle_swiftshader_renderer_reported", at)
        if tag == "Unity":
            if "Built from" in msg and "Scripting Backend 'il2cpp'" in msg:
                events.setdefault("unity_il2cpp_identified", at)
            if "Product Name: OXIDE" in msg:
                events.setdefault("unity_product_reported", at)
        kill = SIGKILL.search(msg)
        if kill and int(kill["pid"]) == expected_pid and tid == expected_pid:
            events.setdefault("original_self_sigkill", at)

    required = ("original_activity_start", "preferred_vulkan_icd_not_found",
                "swiftshader_unsupported_extension_warning", "gralloc_mapper_4_unsupported",
                "hardware_buffer_format_probe_failed", "angle_swiftshader_renderer_reported",
                "unity_il2cpp_identified", "unity_product_reported", "original_self_sigkill")
    correct = (process == expected_pid and set((43, 56, 59)) == format_failures
               and all(item in events for item in required)
               and all(events[a] <= events[b] for a, b in zip(required, required[1:])))
    return {
        "status": ("GRAPHICS_WARNINGS_PRECEDE_ANGLE_UNITY_THEN_ORIGINAL_EXIT"
                   if correct else "INCOMPLETE_ORIGINAL_GRAPHICS_CHRONOLOGY"),
        "validated": bool(correct),
        "original_pid": process,
        "original_logcat_sha256": expected_sha,
        "original_manifest_gles_requirement": "OpenGL ES 3.2",
        "original_manifest_vulkan_feature": "NOT_REQUIRED",
        "graphics_buffer_probe_formats_failed": sorted(format_failures),
        "angle_swiftshader_software_renderer_reported": "angle_swiftshader_renderer_reported" in events,
        "unity_il2cpp_reported_after_graphics_warnings": bool(correct),
        "events_ms_since_activity_start": {
            name: diff_ms(start, value) for name, value in events.items()
            if start is not None
        },
        "graphics_warnings_immediately_blocked_unity_identification": False if correct else "UNKNOWN",
        "renderer_supports_original_gameplay": "NOT_ESTABLISHED",
        "same_pid_activity_stable_35_seconds": False,
        "original_gameplay_verified": False,
        "security_trigger": "UNKNOWN",
    }


def analyze_two_existing_reports(a15, a16):
    runs = {}
    for key, folder in (("android15", Path(a15)), ("android16", Path(a16))):
        logs = folder / "logcat.txt"
        badging = folder / "apk-badging.txt"
        if not logs.is_file() or not badging.is_file():
            return {"status": "BLOCKED_MISSING_ORIGINAL_ARTIFACT", "validated": False}
        analysis = analyze_original(logs.read_bytes(), badging.read_bytes(),
                                    EXPECTED_PIDS[key], SHA256_LOGCAT[key])
        if key == "android15":
            screens = {}
            for filename in REVIEWED_IMMERSIVE_DIALOG_PNG_SHA256:
                path = folder / filename
                if not path.is_file():
                    return {"status": "BLOCKED_MISSING_MANUALLY_REVIEWED_SCREENSHOT",
                            "validated": False}
                screens[filename] = review_exact_screen(path.read_bytes(), filename)
            analysis["original_screenshots"] = screens
        runs[key] = analysis
    ok = (all(v["validated"] for v in runs.values())
          and all(x["status"] == "EXACT_ORIGINAL_SCREEN_REVIEWED_ANDROID_IMMERSIVE_DIALOG"
                  for x in runs["android15"]["original_screenshots"].values()))
    return {
        "status": ("GRAPHICS_WARNINGS_NONBLOCKING_TO_UNITY_SCREENSHOTS_OCCLUDED"
                   if ok else "GRAPHICS_EVIDENCE_INCOMPLETE"),
        "validated": ok,
        "runs": runs,
        "visual_evidence_review_scope": "ONLY_EXACT_PINNED_ANDROID15_SCREEN_2_3_4",
        "original_gameplay_verified": False,
        "cause_of_original_self_sigkill": "UNKNOWN",
        "original_apks_modified": False,
        "android_or_game_executed_now": False,
    }


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--android15-reports", type=Path, required=True)
    p.add_argument("--android16-reports", type=Path, required=True)
    p.add_argument("--output", type=Path, required=True)
    a = p.parse_args()
    report = analyze_two_existing_reports(a.android15_reports, a.android16_reports)
    a.output.parent.mkdir(parents=True, exist_ok=True)
    a.output.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n", encoding="utf8")
    print(json.dumps(report, sort_keys=True))
    return 0 if report["validated"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
