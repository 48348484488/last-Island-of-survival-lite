#!/usr/bin/env python3
"""Read-only comparison of THREE existing original Oxide Android executions.

Two Redroid native ARM64 runs and one official translated x86_64 AVD run.
No code execution inside the game, binary changes, attestation spoofing, or
claims about why the app chose to exit or what a security policy detected.
"""
import argparse
import hashlib
import json
from pathlib import Path

if __package__:
    from .oxide_exit_timeline import assess
    from .oxide_berberis_artifact_replay import verify_existing_logcat
else:
    from oxide_exit_timeline import assess
    from oxide_berberis_artifact_replay import verify_existing_logcat

EXPECTED_NATIVE = {
    "android15": (2023, 4480),
    "android16": (2108, 4670),
}


def compare_original_environments(android15, android16, translated):
    """Check known original exit signatures with separate per-input provenance."""
    inputs = {"android15": android15, "android16": android16}
    checks = {}
    summaries = {}
    for key, source in inputs.items():
        info = assess(source.decode("utf-8", errors="replace"))
        ev = info["evidence"]
        pid, duration = EXPECTED_NATIVE[key]
        checks[key] = {
            "expected_original_pid": ev["original_pid"] == pid,
            "task_close_then_sigkill": info["status"] ==
                "TASK_CLOSE_PRECEDES_ORIGINAL_SELF_SIGKILL",
            "expected_exit_delay_ms": ev.get("start_to_self_sigkill_ms") == duration,
            "original_self_sigkill": ev["original_self_sigkill_logcat"] is True,
            "no_original_sigill": ev["original_fatal_sigill_observed"] is False,
            "no_berberis_opcode_diagnostic": ev[
                "original_berberis_undefined_instruction_precedes_sigill"] is False,
            "no_gameplay_claim": info["gameplay_verified"] is False,
        }
        summaries[key] = {
            "original_pid": ev["original_pid"],
            "exit_status": info["status"],
            "start_to_signal_ms": ev.get("start_to_self_sigkill_ms"),
            "sha256_logcat": hashlib.sha256(source).hexdigest(),
        }

    x86 = verify_existing_logcat(translated)
    checks["x86_translated"] = {
        "original_berberis_sigill_replay": x86["all_checks_passed"] is True,
        "same_original_pid_as_guest_evidence": x86["original_pid"] == 4426,
        "no_gameplay_claim": x86["original_gameplay_verified"] is False,
        "no_native_arm64_claim": x86["native_arm64_compatibility"] == "NOT_ESTABLISHED",
    }
    summaries["x86_translated"] = {
        "original_pid": x86["original_pid"],
        "exit_status": x86["status"],
        "start_to_signal_ms": 3058 if x86["checks"]["launch_to_signal"] else None,
        "sha256_logcat": x86["source_logcat_sha256"],
        "opcode": x86["reported_opcode"],
        "berberis_to_signal_ms": x86["observed_signal_delta_ms"],
    }

    passed = all(all(group.values()) for group in checks.values())
    return {
        "status": ("DISTINCT_ORIGINAL_TERMINATION_PATHS_VERIFIED"
                   if passed else "ENVIRONMENT_COMPARISON_INCOMPLETE"),
        "all_checks_passed": passed,
        "checks": checks,
        "executions": summaries,
        "native_redroid_cause": "UNKNOWN",
        "x86_translator_observed_failure": "BERBERIS_UNDEFINED_INSTRUCTION_THEN_SIGILL",
        "shared_causality": "NOT_ESTABLISHED",
        "original_gameplay_verified": False,
        "original_pid_stable_35_seconds": False,
        "original_apks_modified": False,
        "conclusion": (
            "Native ARM64 and translated x86_64 show different observable "
            "termination paths; this does not identify any security policy "
            "or demonstrate the game is playable."
        ),
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--android15-logcat", type=Path, required=True)
    parser.add_argument("--android16-logcat", type=Path, required=True)
    parser.add_argument("--x86-logcat", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    report = compare_original_environments(
        args.android15_logcat.read_bytes(),
        args.android16_logcat.read_bytes(),
        args.x86_logcat.read_bytes(),
    )
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n",
                           encoding="utf-8")
    print(json.dumps(report, sort_keys=True))
    return 0 if report["all_checks_passed"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
