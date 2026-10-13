#!/usr/bin/env python3
"""Read-only diagnosis of original Oxide native tombstones, without bypasses.

SIGILL with SI_TKILL is a process-directed signal, not by itself proof
of an unsupported ARM instruction, nor proof of the security check's logic.
"""
import argparse
import json
import re
from pathlib import Path

FATAL = re.compile(r"\bsignal\s+(\d+)\s+\((SIG[A-Z]+)\),\s+code\s+(-?\d+)\s+\((SI_[A-Z_]+)\)")
GUEST = re.compile(r"Guest architecture:\s*'([^']+)'")
FRAME = re.compile(r"#\d+\s+pc\s+([0-9a-fA-F]+)\s+(.+)")
PID = re.compile(r"\bpid[: ]+(\d+)")
NATIVE_EXIT = re.compile(r"reason=\d+\s+\(APP CRASH\(NATIVE\)\)")
UNITY = re.compile(r"\b(Unity|IL2CPP)\s*:\s*(Context Type:|JNI_OnLoad|MemoryManager:)")
LOG_LINE = re.compile(r"^\d\d-\d\d\s+\d\d:\d\d:\d\d\.\d{3}\s+(\d+)\s+\d+\s+\w\s+(.+)$")

def classify(crash="", exit_info="", runtime="", forensic=None):
    fatal = FATAL.search(crash)
    guest = GUEST.search(crash)
    original_pid = (forensic or {}).get("initial_activity_pid")
    frames = []
    for line in crash.splitlines():
        m = FRAME.search(line)
        if m:
            frames.append({"pc": "0x" + m.group(1).lstrip("0").lower(),
                           "object": m.group(2).strip()})
    loader = [x for x in frames if "libloader.so" in x["object"]]
    linker = [x for x in frames if "linker64" in x["object"]]
    game_unity_init = False
    for line in runtime.splitlines():
        row = LOG_LINE.match(line)
        if row and original_pid and int(row.group(1)) == original_pid and UNITY.search(row.group(2)):
            game_unity_init = True
            break
    killed_from_app = bool((forensic or {}).get("first_self_sigkill"))
    fatal_signal = fatal.group(2) if fatal else None
    signal_delivery = fatal.group(4) if fatal else None
    native_exit = bool(NATIVE_EXIT.search(exit_info))
    if fatal_signal == "SIGILL" and signal_delivery == "SI_TKILL" and loader and linker:
        status = "NATIVE_SIGILL_SI_TKILL_DURING_LIBLOADER_LINKER_CONSTRUCTORS"
    elif fatal_signal and native_exit:
        status = "NATIVE_CRASH_OTHER_ORIGIN"
    elif killed_from_app:
        status = "SELF_SIGKILL_OBSERVED_NO_NATIVE_TOMBSTONE"
    else:
        status = "CRASH_ORIGIN_NOT_CLASSIFIED"
    return {
        "status": status,
        "original_activity_pid": original_pid,
        "native_crash_confirmed_by_android_exit_info": native_exit,
        "fatal_signal": fatal_signal,
        "fatal_signal_number": int(fatal.group(1)) if fatal else None,
        "signal_code": signal_delivery,
        "guest_architecture": guest.group(1) if guest else None,
        "library_frames": loader[:8],
        "linker_frames_count": len(linker),
        "unity_il2cpp_start_logged_for_game_pid": game_unity_init,
        "redroid_style_self_sigkill_observed": killed_from_app,
        "specific_security_response_cause": "NOT_DETERMINED",
        "play_integrity_verdict": "NOT_MEASURED",
        "multiplayer_verified": False,
        "esp_verified": False,
        "interpretation": (
            "The native loader appears in the guest stack while Android reports SIGILL "
            "delivered using SI_TKILL. This does not distinguish intentional app "
            "termination from emulator/native-bridge compatibility without additional evidence."
        ),
    }

def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("evidence", type=Path)
    p.add_argument("--out", type=Path, required=True)
    args = p.parse_args()
    folder = args.evidence
    def read(name):
        target = folder / name
        return target.read_text(encoding="utf-8", errors="replace") if target.is_file() else ""
    path = folder / "forensic_summary.json"
    forensic = json.loads(read("forensic_summary.json")) if path.is_file() else {}
    result = classify(read("crash_buffer.txt"), read("exit_info.txt"),
                      read("all_buffers.txt"), forensic)
    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "native_crash.json").write_text(json.dumps(result, indent=2)+"\n")
    lines = ["# Original Oxide native Android crash audit", "",
             "Classification: " + result["status"],
             "Original game PID: " + str(result["original_activity_pid"]),
             "Android native crash confirmed: " +
                str(result["native_crash_confirmed_by_android_exit_info"]),
             "Signal: " + str(result["fatal_signal"]) + " / " + str(result["signal_code"]),
             "Guest architecture: " + str(result["guest_architecture"]),
             "Unity or IL2CPP startup already logged: " +
                str(result["unity_il2cpp_start_logged_for_game_pid"]),
             "", "## Original loader backtrace frames", ""]
    lines.extend("- " + r["pc"] + " " + r["object"] for r in result["library_frames"])
    lines += ["", result["interpretation"], "",
              "No verified Play Integrity verdict, multiplayer gameplay, or ESP."]
    (args.out / "native_crash.md").write_text("\n".join(lines) + "\n")
    print("\n".join(lines))
    if not result["original_activity_pid"] or not result["native_crash_confirmed_by_android_exit_info"]:
        raise SystemExit("Insufficient game crash evidence")

if __name__ == "__main__":
    main()
