#!/usr/bin/env python3
"""Conservative classification of our OWN ARM64 CPU-ID JNI probe on Android.

No original game is installed by the associated CI experiment.
A SIGILL for the probe's PID supports a translator/CPU-ID compatibility
problem, but it does not prove why the original game exits.
"""
import argparse
import json
import re
from pathlib import Path

ROW = re.compile(r"^\d\d-\d\d\s+\d\d:\d\d:\d\d\.\d+\s+(\d+)\s+\d+\s+[A-Z]\s+([^:]+):\s*(.*)$")
GAME = "com.catsbit.oxidesurvivalisland"
MARKER = "OXIDE_CPU_ID_PROBE"
PRE = "STAGE=BEFORE_MRS_ID_AA64PFR1_EL1"
POST = "STAGE=MRS_RETURNED"
FATAL = re.compile(r"\bFatal signal 4 \(SIGILL\)")

def classify(log):
    starts = []
    returned = []
    fatals = []
    for line in log.splitlines():
        m=ROW.match(line)
        if not m: continue
        pid,tag,msg=int(m.group(1)),m.group(2).strip(),m.group(3)
        if tag==MARKER and PRE in msg:
            starts.append(pid)
        if tag==MARKER and POST in msg:
            returned.append(pid)
        if FATAL.search(msg):
            fatals.append(pid)
    pid=starts[-1] if starts else None
    if pid is None:
        status="PROBE_DID_NOT_REACH_MRS"
    elif pid in returned:
        status="ORIGINAL_CRASH_SITE_MRS_RETURNED_IN_INDEPENDENT_APK"
    elif pid in fatals:
        status="INDEPENDENT_CPU_ID_MRS_SIGILL_ON_NATIVE_BRIDGE"
    else:
        status="PROBE_STARTED_NO_RETURN_OR_SIGILL_OBSERVED"
    return {
        "status":status, "diagnostic_apk_pid":pid,
        "mrs_invocation_logged":pid is not None,
        "mrs_returned":bool(pid is not None and pid in returned),
        "sigill_recorded_for_same_pid":bool(pid is not None and pid in fatals),
        "original_game_installed":False,
        "original_game_executed":False,
        "original_game_sigill_causation":"NOT_PROVEN",
        "security_protection_modification":False,
        "play_integrity_verdict":"NOT_MEASURED",
        "meaning":"A separate app executes one matching ARM64 system-register instruction. The test cannot establish original game's root cause.",
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("log",type=Path)
    p.add_argument("--out",required=True,type=Path)
    opt=p.parse_args()
    result=classify(opt.log.read_text(encoding="utf-8", errors="replace"))
    opt.out.mkdir(parents=True,exist_ok=True)
    (opt.out/"cpu_id_probe_verdict.json").write_text(json.dumps(result,indent=2)+"\n")
    print(json.dumps(result,indent=2))
    if not result["mrs_invocation_logged"]:
        raise SystemExit("Our isolated diagnostic did not reach the JNI test marker")

if __name__=="__main__":
    main()
