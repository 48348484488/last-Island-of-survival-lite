#!/usr/bin/env python3
"""Reassess the already-collected original Oxide x86_64 logcat, without APKs.

A positive verdict is limited to the recorded Berberis/SIGILL sequence.
It never implies Android ARM64-native compatibility, anti-cheat causation,
integrity attestation, stability for 35 seconds, or gameplay.
"""
import argparse
import hashlib
import json
from pathlib import Path

if __package__:
    from .oxide_berberis_logcat_audit import audit_logcat
    from .oxide_exit_timeline import assess as assess_exit
else:
    from oxide_berberis_logcat_audit import audit_logcat
    from oxide_exit_timeline import assess as assess_exit


EXPECTED_PID = 4426
EXPECTED_OPCODE = "0xd5380429"
EXPECTED_ADDRESS = "0x0000737d96904a3c"
EXPECTED_GUEST_PC = "0x700a3c"
EXPECTED_DELTA_MS = 6
EXPECTED_START_TO_SIGNAL_MS = 3058


def verify_existing_logcat(blob):
    """Return a fail-closed, byte-provenanced conclusion from logcat bytes."""
    text = blob.decode("utf-8", errors="replace")
    bridge = audit_logcat(text)
    timeline = assess_exit(text)
    observed = timeline["evidence"]
    checks = {
        "original_pid": observed["original_pid"] == EXPECTED_PID,
        "berberis_same_pid": bridge.get("pid") == EXPECTED_PID,
        "berberis_undefined_opcode": bridge.get("status") ==
            "BERBERIS_UNDEFINED_INSTRUCTION_PRECEDES_SIGILL",
        "opcode": bridge.get("opcode") == EXPECTED_OPCODE,
        "address": bridge.get("address") == EXPECTED_ADDRESS,
        "interval_six_ms": bridge.get("delta_ms") == EXPECTED_DELTA_MS,
        "native_timeline_correlated": observed[
            "original_berberis_undefined_instruction_precedes_sigill"] is True,
        "fatal_sigill_si_tkill": observed["original_fatal_sigill_observed"] is True
            and observed["original_fatal_sigill_code"] == -6
            and observed["original_fatal_sigill_origin"] == "SI_TKILL",
        "constructor_stack": timeline["status"] ==
            "ORIGINAL_SIGILL_DURING_ARM64_LIBLOADER_CONSTRUCTORS",
        "launch_to_signal": observed.get("start_to_sigill_ms") ==
            EXPECTED_START_TO_SIGNAL_MS,
        "no_gameplay_claim": timeline["gameplay_verified"] is False,
    }
    accepted = all(checks.values())
    return {
        "status": ("ORIGINAL_BERBERIS_OPCODE_SIGILL_EVIDENCE_CONFIRMED"
                   if accepted else "ORIGINAL_BERBERIS_EVIDENCE_INCOMPLETE"),
        "all_checks_passed": accepted,
        "checks": checks,
        "source_logcat_sha256": hashlib.sha256(blob).hexdigest(),
        "original_pid": observed.get("original_pid"),
        "reported_opcode": bridge.get("opcode"),
        "reported_instruction_address": bridge.get("address"),
        "observed_signal_delta_ms": bridge.get("delta_ms"),
        "tombstone_guest_pc": EXPECTED_GUEST_PC if checks["constructor_stack"] else None,
        "translated_environment": "OFFICIAL_X86_64_AVD_BERBERIS_AARCH64",
        "original_apks_modified": False,
        "original_gameplay_verified": False,
        "security_trigger": "UNKNOWN",
        "native_arm64_compatibility": "NOT_ESTABLISHED",
        "note": "Translator diagnostic is not proof of native ARM64 instruction accessibility.",
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--logcat", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    report = verify_existing_logcat(args.logcat.read_bytes())
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n",
                           encoding="utf-8")
    print(json.dumps(report, sort_keys=True))
    return 0 if report["all_checks_passed"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
