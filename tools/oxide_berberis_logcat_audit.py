"""Read-only correlation of a Berberis undefined opcode and subsequent SIGILL.

Consumes existing logcat text only. This does not determine anti-cheat causation.
"""
import re

BERBERIS = re.compile(r"(?P<time>\d\d:\d\d:\d\d\.\d{3})\s+(?P<pid>\d+)\s+\d+\s+E\s+berberis:\s+Undefined instruction (?P<opcode>0x[0-9a-fA-F]+) at (?P<address>0x[0-9a-fA-F]+)", re.I)
SIGILL = re.compile(r"(?P<time>\d\d:\d\d:\d\d\.\d{3})\s+(?P<pid>\d+)\s+\d+\s+F\s+libc\s*:\s+Fatal signal 4 \(SIGILL\), code -6 \(SI_TKILL\)", re.I)


def _milliseconds(value):
    hours, minutes, seconds = value.split(":")
    whole, fraction = seconds.split(".")
    return ((int(hours) * 60 + int(minutes)) * 60 + int(whole)) * 1000 + int(fraction)


def audit_logcat(text):
    candidates = [m for line in text.splitlines() if (m := BERBERIS.search(line))]
    signals = [m for line in text.splitlines() if (m := SIGILL.search(line))]
    for instruction in candidates:
        for signal in signals:
            delta = _milliseconds(signal["time"]) - _milliseconds(instruction["time"])
            if signal["pid"] == instruction["pid"] and 0 <= delta <= 1000:
                return {
                    "status": "BERBERIS_UNDEFINED_INSTRUCTION_PRECEDES_SIGILL",
                    "pid": int(instruction["pid"]),
                    "opcode": instruction["opcode"].lower(),
                    "address": instruction["address"].lower(),
                    "delta_ms": delta,
                    "signal": "SIGILL/SI_TKILL",
                    "causal_security_trigger": "UNKNOWN",
                    "original_gameplay_verified": False,
                }
    return {"status": "NO_MATCHING_BERBERIS_SIGILL_SEQUENCE", "original_gameplay_verified": False}


if __name__ == "__main__":
    import argparse
    import json
    from pathlib import Path
    parser = argparse.ArgumentParser()
    parser.add_argument("logcat", type=Path)
    args = parser.parse_args()
    print(json.dumps(audit_logcat(args.logcat.read_text(errors="replace")), sort_keys=True))
