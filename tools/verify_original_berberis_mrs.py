#!/usr/bin/env python3
"""Verify original Oxide Berberis ARM64 MRS crash with independent controls.

All inputs are archived, unmodified Android logcat. No APK execution,
patch, signing change, hook, Play Integrity token, or security bypass.
"""
import argparse
import json
import re
from pathlib import Path
from correlate_arm64_cpu_id import analyze

ORIGINAL_OPCODE = 0xD5380429
ORIGINAL_PC = 0x830BEC
LOG = re.compile(r"^\d\d-\d\d\s+\d\d:\d\d:\d\d\.\d+\s+(\d+)\s+\d+\s+[A-Z]\s+([^:]+):\s*(.*)$")
BAD_OPCODE = re.compile(r"Undefined instruction 0x([0-9a-fA-F]+)")
FATAL = re.compile(r"Fatal signal 4 \(SIGILL\), code -6 \(SI_TKILL\)")
ORIGINAL_GUEST_FRAME = re.compile(
    r"#00 pc ([0-9a-fA-F]+)\s+\S+libloader[.]so")
  
def verify(original_log, original_forensic, translated_log, native_log):
    original_pid = original_forensic.get("initial_activity_pid")
    original_bad=[]
    original_signals=[]
    for line in original_log.splitlines():
        match=LOG.match(line)
        if not match or int(match.group(1)) != original_pid:
            continue
        tag=match.group(2).strip()
        message=match.group(3)
        bad=BAD_OPCODE.search(message)
        if tag=="berberis" and bad:
            original_bad.append(int(bad.group(1),16))
        if tag=="libc" and FATAL.search(message):
            original_signals.append(line)
    frame_addresses = []
    for line in original_log.splitlines():
        m=ORIGINAL_GUEST_FRAME.search(line)
        if m:
            frame_addresses.append(int(m.group(1),16))
    independent = analyze(translated_log,native_log)
    native_bridge_support_failure=independent["status"]=="ARM64_TRANSLATOR_MRS_INCOMPATIBILITY_REPRODUCED"
    matched=(isinstance(original_pid,int)
             and original_pid>0
             and ORIGINAL_OPCODE in original_bad
             and bool(original_signals)
             and ORIGINAL_PC in frame_addresses
             and native_bridge_support_failure)
    return {
        "status":("ORIGINAL_OXIDE_BERBERIS_MRS_CRASH_WITH_INDEPENDENT_CONTROL_CONFIRMED"
                  if matched else "INCOMPLETE_DO_NOT_CLAIM_ORIGINAL_CAUSAL_MATCH"),
        "original_game_pid":original_pid,
        "original_libloader_pc":"0x830bec",
        "original_berberis_undefined_opcodes":["0x%08x"%x for x in original_bad],
        "original_game_has_sigill_si_tkill":bool(original_signals),
        "original_game_guest_top_frame_pc_confirmed":ORIGINAL_PC in frame_addresses,
        "independent_native_bridge_unsupported_mrs_proven":native_bridge_support_failure,
        "independent_translated_pid":independent["translated_pid"],
        "independent_native_pid":independent["native_pid"],
        "independent_translated_instruction":"0xd5380420",
        "original_instruction":"0xd5380429",
        "instruction_system_register_match":(
            (ORIGINAL_OPCODE & ~0x1f)==(0xd5380420 & ~0x1f)),
        "original_android_environment":"Google Play Android 15 x86_64 + Berberis ARM64 translation",
        "native_control_environment":"Redroid Android 13 native AArch64",
        "original_game_playable":False,
        "redroid_native_game_self_sigkill_resolved":False,
        "player_positions_or_esp_verified":False,
        "remaining_limits":[
            "The original game on x86_64/ARM64 translation crashed after Berberis explicitly rejected the exact MRS opcode.",
            "This establishes the immediate translated-game crash site, not all game security or environment requirements.",
            "Native Redroid boots Unity but its original game still self-terminates by SIGKILL.",
            "The independent controls ran different Android images and contained no original Oxide binaries.",
            "No game's protection, APK, or Play Integrity system was bypassed.",
        ],
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("--original",type=Path,required=True)
    p.add_argument("--original-forensic",type=Path,required=True)
    p.add_argument("--translated",type=Path,required=True)
    p.add_argument("--native",type=Path,required=True)
    p.add_argument("--out",type=Path,required=True)
    args=p.parse_args()
    result=verify(args.original.read_text(errors="replace"),
                  json.loads(args.original_forensic.read_text()),
                  args.translated.read_text(errors="replace"),
                  args.native.read_text(errors="replace"))
    args.out.mkdir(parents=True,exist_ok=True)
    (args.out/"original_game_translator_crash.json").write_text(
        json.dumps(result,indent=2)+"\n")
    lines=["# Oxide original Android Google Play ARM64 translation crash — real evidence",
           "", "Verdict: "+result["status"],"",
           "Original game PID: "+str(result["original_game_pid"]),
           "Original guest loader top frame: "+result["original_libloader_pc"],
           "Original Berberis opcode(s): "+", ".join(result["original_berberis_undefined_opcodes"]),
           "Original SIGILL SI_TKILL: "+str(result["original_game_has_sigill_si_tkill"]),
           "Independent translated diagnostic PID: "+str(result["independent_translated_pid"]),
           "Independent native ARM64 diagnostic PID: "+str(result["independent_native_pid"]),
           "Same system-register instruction (different destination): "+str(result["instruction_system_register_match"]),
           "", "## Limits", ""]
    lines.extend("- "+x for x in result["remaining_limits"])
    (args.out/"original_game_translator_crash.md").write_text("\n".join(lines)+"\n")
    print("\n".join(lines))
    if not result["status"].startswith("ORIGINAL_OXIDE_BERBERIS_MRS_CRASH"):
        raise SystemExit("Insufficient direct game opcode, same-PID signal, guest PC, or control evidence")

if __name__=="__main__":
    main()
