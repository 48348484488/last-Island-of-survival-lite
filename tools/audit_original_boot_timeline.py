#!/usr/bin/env python3
"""Read-only, PID-scoped timeline of the original Oxide Android startup.

This diagnoses the ORIGINAL, unmodified app. It neither evaluates Play
Integrity tokens nor changes security checks, device identity or game code.
An Android event in a different PID cannot be called a game-process failure.
"""
import argparse
import json
from pathlib import Path
from diagnose_original_exit import diagnose, event_ms, log_lines

RULES = (
    ("play_store_missing", "GooglePlayServicesUtil", "requires the Google Play Store"),
    ("play_services_missing", "GooglePlayServicesUtil", "Google Play services is missing"),
    ("gmscore_invalid", "DynamiteModule", "Invalid GmsCore APK"),
    ("firebase_gms_unavailable", "FirebaseInstanceId", "Google Play services missing"),
    ("gms_package_not_found", "Metadata", "NameNotFoundException: com.google.android.gms"),
    ("il2cpp_jni_loaded", "IL2CPP", "JNI_OnLoad"),
    ("unity_context_initialized", "Unity", "Context Type:"),
    ("unity_memory_initialized", "Unity", "MemoryManager:"),
    ("unity_application_information", "Unity", "ApplicationInfo "),
    ("unity_product_information", "Unity", "Product Name:"),
    ("sensor_list_errors", "Sensors", "Failed to get Sensor list"),
    ("graphic_buffer_errors", "GraphicBufferAllocator", "Failed to allocate"),
    ("security_message_in_game_log", None, "Security Threat Detected by App"),
)

def analyze(folder):
    folder = Path(folder)
    result = diagnose(folder)
    pid, stamp = result["initial_activity_pid"], result["initial_activity_start"]
    first_sigkill = result["first_self_sigkill"]
    buckets = {name: {"count": 0, "first_after_launch_ms": None, "last_after_launch_ms": None}
               for name, _, _ in RULES}
    if pid is not None and stamp:
        for row in log_lines(folder / "all_buffers.txt"):
            if row["pid"] != pid or row["timestamp"] < stamp:
                continue
            # A late unrelated PID reuse after the app dies is not game output.
            if first_sigkill and row["timestamp"] > first_sigkill:
                continue
            for name, tag, marker in RULES:
                if (tag is None or row["tag"] == tag) and marker in row["raw"]:
                    offset = event_ms(stamp, row["timestamp"])
                    if offset < 0:
                        continue
                    slot = buckets[name]
                    slot["count"] += 1
                    if slot["first_after_launch_ms"] is None:
                        slot["first_after_launch_ms"] = offset
                    slot["last_after_launch_ms"] = offset
    return {
        "scope": "ORIGINAL_OXIDE_PROCESS_ONLY",
        "activity_pid": pid,
        "activity_start": stamp,
        "process_exit_classification": result["classification"],
        "self_sigkill": first_sigkill,
        "time_to_activity_finish_ms": (
            event_ms(stamp, result["activity_finish"]) if stamp and result["activity_finish"] else None),
        "time_to_self_sigkill_ms": (
            event_ms(stamp, first_sigkill) if stamp and first_sigkill else None),
        "events": buckets,
        "play_integrity_token": "NOT_OBSERVED",
        "security_warning_text_in_game_log": (
            "OBSERVED" if buckets["security_message_in_game_log"]["count"] else "NOT_OBSERVED"),
        "security_rejection_cause": "NOT_IDENTIFIED",
        "gameplay_verified": False,
        "notes": [
            "Logcat errors before an exit establish sequence, not causation.",
            "A toast's text may appear in screenshot without being logged in the game PID.",
            "Third-party SDK errors may be nonfatal.",
            "Missing Play Services does not by itself establish a Play Integrity verdict.",
            "This analysis selects the most recent original Activity PID, ignoring earlier attempts.",
        ],
    }

def compare(before, after):
    events = sorted(set(before["events"]) | set(after["events"]))
    changed = []
    for name in events:
        a = before["events"].get(name, {"count": 0, "first_after_launch_ms": None})
        b = after["events"].get(name, {"count": 0, "first_after_launch_ms": None})
        changed.append({
            "event": name,
            "before_count": a["count"], "after_count": b["count"],
            "before_first_ms": a["first_after_launch_ms"],
            "after_first_ms": b["first_after_launch_ms"],
        })
    return {
        "before_pid": before["activity_pid"],
        "after_pid": after["activity_pid"],
        "before_termination": before["process_exit_classification"],
        "after_termination": after["process_exit_classification"],
        "before_exit_ms": before["time_to_self_sigkill_ms"],
        "after_exit_ms": after["time_to_self_sigkill_ms"],
        "both_self_terminated": (
            before["process_exit_classification"] == "ORIGINAL_ACTIVITY_SELF_SIGKILL" and
            after["process_exit_classification"] == "ORIGINAL_ACTIVITY_SELF_SIGKILL"),
        "event_comparison": changed,
        "play_integrity_verdict": "NOT_MEASURED",
        "root_cause_identified": False,
        "original_gameplay_verified": False,
    }

def markdown(out):
    p = out["comparison"]
    lines = [
        "# Original Oxide — Android ARM64 process-scoped startup",
        "",
        "Verified original Activity PID: before " + str(p["before_pid"]) +
        ", after " + str(p["after_pid"]),
        "Self termination: before " + str(p["before_termination"]) +
        ", after " + str(p["after_termination"]),
        "Elapsed time until self-SIGKILL (ms): " + str(p["before_exit_ms"]) +
        " / " + str(p["after_exit_ms"]),
        "Both original Activity processes self-terminated: " + str(p["both_self_terminated"]),
        "", "| Event emitted by ORIGINAL game PID | Before count | After count | First ms (before/after) |",
        "|---|---:|---:|---|",
    ]
    for item in p["event_comparison"]:
        lines.append("| " + item["event"] + " | " + str(item["before_count"]) +
                     " | " + str(item["after_count"]) + " | " +
                     str(item["before_first_ms"]) + " / " + str(item["after_first_ms"]) + " |")
    lines += [
        "", "Game process stdout is not a Play Integrity token.",
        "Play Integrity verdict: NOT MEASURED.",
        "Exact cause of Oxide security response: NOT IDENTIFIED.",
        "Playable multiplayer or real-player ESP: NOT VERIFIED.",
        "A logged warning preceding a signal does not prove it caused the signal.",
    ]
    return "\n".join(lines) + "\n"

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--before", type=Path, required=True)
    p.add_argument("--after", type=Path, required=True)
    p.add_argument("--out", type=Path, required=True)
    options = p.parse_args()
    before = analyze(options.before)
    after = analyze(options.after)
    result = {"before": before, "after": after, "comparison": compare(before, after)}
    options.out.mkdir(parents=True, exist_ok=True)
    (options.out / "startup_timeline.json").write_text(
        json.dumps(result, indent=2) + "\n", encoding="utf-8")
    report = markdown(result)
    (options.out / "startup_timeline.md").write_text(report, encoding="utf-8")
    print(report)
    if not result["comparison"]["before_pid"] or not result["comparison"]["after_pid"]:
        raise SystemExit("Missing original process evidence; cannot claim an A/B comparison.")

if __name__ == "__main__":
    main()
