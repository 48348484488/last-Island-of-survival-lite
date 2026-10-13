#!/usr/bin/env python3
"""Correlate two actually executed independent ARM64 CPU-ID experiments.

x86 Android 15 Google Play uses native bridge (berberis/ndk_translation),
while native Android 13 Redroid runs on an AArch64 host. No original game
APK is needed or installed; this is strictly an environment compatibility
regression that cannot attest original-game security or gameplay.
"""
import argparse
import json
import re
from pathlib import Path
from classify_cpu_id_probe import classify

REGISTER_READ_BASE = 0xD5380420
REGISTER_DEST_MASK = 0x1F
ORIGINAL_OPCODE = 0xD5380429  # libloader.so +0x830BEC: mrs x9,id_aa64pfr1_el1
UNDEFINED = re.compile(r"Undefined instruction 0x([0-9a-fA-F]+)")
LOG_LINE = re.compile(r"^\d\d-\d\d\s+\d\d:\d\d:\d\d\.\d+\s+(\d+)\s+\d+\s+[A-Z]\s+([^:]+):\s*(.*)$")

def analyze(translated_log, native_log):
    translated = classify(translated_log)
    native = classify(native_log)
    errors = []
    for line in translated_log.splitlines():
        m = LOG_LINE.match(line)
        if not m or translated["diagnostic_apk_pid"] is None:
            continue
        pid, tag, msg = int(m.group(1)), m.group(2).strip(), m.group(3)
        bad = UNDEFINED.search(msg)
        if pid == translated["diagnostic_apk_pid"] and tag == "berberis" and bad:
            opcode = int(bad.group(1),16)
            errors.append({"pid":pid,"opcode":"0x%08x"%opcode,
                           "matches_original_instruction_ignoring_destination_register":
                           (opcode & ~REGISTER_DEST_MASK)==(ORIGINAL_OPCODE & ~REGISTER_DEST_MASK)})
    verified = (
        translated["status"] == "INDEPENDENT_CPU_ID_MRS_SIGILL_ON_NATIVE_BRIDGE"
        and native["status"] == "ORIGINAL_CRASH_SITE_MRS_RETURNED_IN_INDEPENDENT_APK"
        and any(x["matches_original_instruction_ignoring_destination_register"] for x in errors)
    )
    return {
        "status": "ARM64_TRANSLATOR_MRS_INCOMPATIBILITY_REPRODUCED" if verified
                  else "EVIDENCE_INCOMPLETE_DO_NOT_CLAIM_COMPARISON",
        "original_game_loader_pc": "0x830bec",
        "original_game_loader_mrs_opcode": "0xd5380429",
        "translated_abi": "x86_64 host / ARM64 guest via native bridge",
        "translated_game_absent": True,
        "translated_pid": translated["diagnostic_apk_pid"],
        "translated_mrs_returned": translated["mrs_returned"],
        "translated_sigill": translated["sigill_recorded_for_same_pid"],
        "native_abi": "AArch64 host / ARM64 Android guest",
        "native_game_absent": True,
        "native_pid": native["diagnostic_apk_pid"],
        "native_mrs_returned": native["mrs_returned"],
        "native_sigill": native["sigill_recorded_for_same_pid"],
        "translated_bridge_errors": errors,
        "original_game_sigill_root_cause_proven": False,
        "original_game_crash_free": False,
        "play_integrity_verified": False,
        "real_player_esp_verified": False,
        "limitations": [
            "Tests ran on Android 15 x86 translated vs Android 13 ARM64 native: image versions differ.",
            "Berberis explicitly reported an undefined ARM64 system register read in the independent probe.",
            "A matching failure mechanism is demonstrated, not necessarily the sole cause of original Oxide termination.",
            "The original game's Redroid native ARM64 security-related SIGKILL remains unresolved.",
            "This tool never modifies the game, native translator, Android certificates or security checks.",
        ],
    }

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--translated", type=Path, required=True)
    parser.add_argument("--native", type=Path, required=True)
    parser.add_argument("--out", type=Path, required=True)
    args = parser.parse_args()
    result=analyze(args.translated.read_text(encoding="utf-8",errors="replace"),
                   args.native.read_text(encoding="utf-8",errors="replace"))
    args.out.mkdir(parents=True,exist_ok=True)
    (args.out/"cpu_id_cross_environment.json").write_text(
        json.dumps(result,indent=2)+"\n",encoding="utf-8")
    lines=["# Independent ARM64 MRS CPU ID test across Android environments","",
           "Status: "+result["status"],"",
           "| Observation | Google Play Android 15 translated ARM64 | Native Android 13 ARM64 |",
           "|---|---|---|",
           "| Diagnostic app PID | "+str(result["translated_pid"])+" | "+str(result["native_pid"])+" |",
           "| Instruction returned | "+str(result["translated_mrs_returned"])+" | "+str(result["native_mrs_returned"])+" |",
           "| Fatal SIGILL | "+str(result["translated_sigill"])+" | "+str(result["native_sigill"])+" |",
           "","Original loader MRS opcode: 0xd5380429 (x9 destination).",
           "Independent Berberis rejected opcode(s): "+", ".join(x["opcode"] for x in result["translated_bridge_errors"]),
           "","The register-field bits match after ignoring only the destination register:",
           str(any(x["matches_original_instruction_ignoring_destination_register"]
                   for x in result["translated_bridge_errors"])),
           "","No original game APK was installed in either independent experiment.",
           "Original game crash root cause: NOT FULLY PROVEN.",""]
    lines += ["- "+x for x in result["limitations"]]
    (args.out/"cpu_id_cross_environment.md").write_text("\n".join(lines)+"\n",encoding="utf-8")
    print("\n".join(lines))
    if not result["status"].startswith("ARM64_TRANSLATOR_MRS_INCOMPATIBILITY"):
        raise SystemExit("Missing PID-matched native bridge undefined instruction evidence")

if __name__ == "__main__":
    main()
