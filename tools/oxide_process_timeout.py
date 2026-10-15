#!/usr/bin/env python3
"""Cross-platform bounded subprocess for Android test commands on Linux/macOS.

Preserves the executable exit status. Avoids Linux-specific 'timeout' command,
which isn't available by default on standard Apple Silicon runners.
"""
import argparse
import os
import signal
import subprocess
import sys


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--seconds", type=float, required=True)
    parser.add_argument("command", nargs=argparse.REMAINDER)
    args = parser.parse_args()
    command = args.command[1:] if args.command[:1] == ["--"] else args.command
    if args.seconds <= 0 or not command:
        parser.error("positive seconds and a command are required")
    child = subprocess.Popen(
        command,
        start_new_session=True if os.name == "posix" else False,
    )
    try:
        return child.wait(timeout=args.seconds)
    except subprocess.TimeoutExpired:
        if os.name == "posix":
            try:
                os.killpg(child.pid, signal.SIGKILL)
            except ProcessLookupError:
                pass
        else:
            child.kill()
        child.wait()
        print(f"Process exceeded {args.seconds:g}s: {command[0]}", file=sys.stderr)
        return 124


if __name__ == "__main__":
    raise SystemExit(main())
