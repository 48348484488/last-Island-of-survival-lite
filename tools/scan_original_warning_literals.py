#!/usr/bin/env python3
"""Locate exact security-warning text inside ORIGINAL signed Oxide APK entries.

Read-only, bounded-memory ZIP entry scanning (UTF-8, UTF-16LE, UTF-16BE). Designed
to identify which asset owns a diagnostic UI string, NOT to patch or
suppress protections, locate security code paths, or infer why a check ran.

A zero-match report must never be called proof that a warning isn't in the game:
strings may be compressed again, encrypted, constructed, or server-supplied.
"""
import argparse
import hashlib
import json
import zipfile
from pathlib import Path
from verify_original_splits import EXPECTED_SHA256, sha256_file

TERMS = (
    "Security Threat Detected by App",
    "Security Threat",
    "Threat Detected",
    "5600:AA4C",
)
CHUNK = 1024 * 1024
MAX_ENTRY = 4 * 1024 * 1024 * 1024
MAX_MATCHES_PER_TERM_PER_ENTRY = 12


def patterns(terms=TERMS):
    result = []
    for term in terms:
        for encoding in ("utf-8", "utf-16le", "utf-16be"):
            result.append((term, encoding, term.encode(encoding)))
    return result


def scan_entry(stream, size, needles=None, chunk_size=CHUNK):
    """Report byte offsets in decompressed ZIP member, including chunk boundaries."""
    needles = needles or patterns()
    max_needle = max(len(x[2]) for x in needles)
    overlap = b""
    consumed = 0
    matches = {}
    while True:
        data = stream.read(chunk_size)
        if not data:
            break
        combined = overlap + data
        start = consumed - len(overlap)
        for term, encoding, needle in needles:
            key = (term, encoding)
            hits = matches.setdefault(key, [])
            off = 0
            while len(hits) < MAX_MATCHES_PER_TERM_PER_ENTRY:
                pos = combined.find(needle, off)
                if pos == -1:
                    break
                absolute = start + pos
                if not hits or absolute != hits[-1]:
                    hits.append(absolute)
                off = pos + 1
        consumed += len(data)
        overlap = combined[-(max_needle - 1):] if max_needle > 1 else b""
    return [{"term": key[0], "encoding": key[1], "offsets": values}
            for key, values in matches.items() if values]


def inspect(apks, expected=None):
    expected = expected or EXPECTED_SHA256
    matches = []
    hashed = {}
    entry_count = 0
    bytes_scanned = 0
    bytes_skipped = 0
    skipped = []
    for apk in apks:
        name = apk.name
        if name not in expected:
            raise ValueError("Unrecognized original APK: "+name)
        digest = sha256_file(apk)
        if digest != expected[name]:
            raise ValueError("APK checksum mismatch: "+name)
        hashed[name] = digest
        with zipfile.ZipFile(apk) as archive:
            for member in archive.infolist():
                if member.is_dir():
                    continue
                entry_count += 1
                if member.file_size > MAX_ENTRY:
                    bytes_skipped += member.file_size
                    skipped.append({"apk": name, "entry": member.filename,
                                    "uncompressed_bytes": member.file_size,
                                    "reason": "MAX_ENTRY_LIMIT"})
                    continue
                with archive.open(member) as stream:
                    hits = scan_entry(stream, member.file_size)
                bytes_scanned += member.file_size
                if hits:
                    matches.append({
                        "apk": name,
                        "entry": member.filename,
                        "size_uncompressed": member.file_size,
                        "matches": hits,
                    })
    return {
        "status": ("EXACT_SECURITY_WARNING_PATTERN_FOUND" if matches
                   else "NO_PLAINTEXT_MATCHES_IN_SCANNED_APKS"),
        "apks_scanned": hashed,
        "entries_seen": entry_count,
        "decompressed_bytes_scanned": bytes_scanned,
        "uncompressed_bytes_not_scanned": bytes_skipped,
        "max_entry_bytes": MAX_ENTRY,
        "scanned_all_four_original_splits": set(hashed) == set(EXPECTED_SHA256),
        "full_plaintext_member_coverage": not skipped and set(hashed) == set(EXPECTED_SHA256),
        "skipped_entries": skipped,
        "matched_entries": matches,
        "interpretation": (
            "A match only identifies raw bytes in an APK entry; it cannot "
            "establish what security condition was triggered. No match is "
            "inconclusive because messages may be generated dynamically, "
            "encrypted, compressed, or retrieved remotely."),
        "game_modified": False,
        "runtime_test_performed": False,
        "specific_security_trigger_identified": False,
    }


def main():
    ap=argparse.ArgumentParser(description=__doc__)
    ap.add_argument("apk",type=Path,nargs="+")
    ap.add_argument("--out",type=Path,required=True)
    opts=ap.parse_args()
    result=inspect(opts.apk)
    opts.out.mkdir(parents=True,exist_ok=True)
    (opts.out/"warning_origin.json").write_text(
        json.dumps(result,indent=2,ensure_ascii=False)+"\n")
    lines=[
        "# Security warning literal ownership in original Oxide APK splits",
        "",
        "Result: "+result["status"],
        "Entries scanned: "+str(result["entries_seen"]),
        "Decompressed bytes scanned: "+str(result["decompressed_bytes_scanned"]),
        "Skipped oversized entries: "+str(len(result["skipped_entries"])),
        "Uncompressed bytes omitted: "+str(result["uncompressed_bytes_not_scanned"]),
        "All four original splits checked: "+str(result["scanned_all_four_original_splits"]),
        "Full ZIP-member plaintext coverage: "+str(result["full_plaintext_member_coverage"]),
        "", "## Matching entries", "",
    ]
    for hit in result["matched_entries"]:
        lines.append("- "+hit["apk"]+" :: "+hit["entry"])
        for sub in hit["matches"]:
            lines.append("  - "+sub["term"]+" ("+sub["encoding"]+"), offsets: "+str(sub["offsets"]))
    if not result["matched_entries"]:
        lines.append("No plain-text matches in these APK members. Other encodings/assets remain untested.")
    lines.extend(["","## Interpretation","",result["interpretation"],"",
                  "No original APK modifications, security bypass, runtime game test, or ESP claim."])
    (opts.out/"warning_origin.md").write_text("\n".join(lines)+"\n")
    print("\n".join(lines))


if __name__=="__main__":
    main()
