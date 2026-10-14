#!/usr/bin/env python3
"""Read only independent app's Android HardwareBuffer allocation result.

Never equate generic Redroid graphics compatibility with the original
Oxide application's security rejection reason.
"""
import argparse
import json
import re
from pathlib import Path

ROW=re.compile(r"^\d\d-\d\d\s+\d\d:\d\d:\d\d\.\d+\s+(\d+)\s+\d+\s+[A-Z]\s+([^:]+):\s*(.*)$")
CASE=re.compile(
    r"CASE=(\S+)\s+FORMAT=(\d+)\s+WIDTH=4 HEIGHT=4 LAYERS=1 "
    r"USAGE=0x([a-f0-9]+)\s+SUPPORTED=(true|false)\s+CREATE=(PASS|FAIL|UNKNOWN)\s+FAILURE=(\S+)"
)
EXPECTED={
    "RGBA1010102_EXACT":(43,0xb00),
    "R8_EXACT":(56,0xb00),
    "RGBA8888_EXACT_USAGE_CONTROL":(1,0xb00),
    "RGBA8888_GPU_CONTROL":(1,0x300),
    "RGBA1010102_GPU_CONTROL":(43,0x300),
    "R8_GPU_CONTROL":(56,0x300),
}
def classify(text):
    starts=[]
    completes=set()
    candidates=[]
    gfx=[]
    for line in text.splitlines():
        m=ROW.match(line)
        if not m:continue
        pid=int(m.group(1))
        tag=m.group(2).strip()
        msg=m.group(3)
        if tag=="OXIDE_GRALLOC_PROBE":
            if "STAGE=START" in msg and "ORIGINAL_GAME_INSTALLED=false" in msg:
                starts.append(pid)
            if "STAGE=COMPLETE CASES=6 ORIGINAL_GAME_TOUCHED=false" in msg:
                completes.add(pid)
            match=CASE.search(msg)
            if match:
                candidates.append({
                    "pid":pid,"name":match.group(1),
                    "format":int(match.group(2)),
                    "usage":int(match.group(3),16),
                    "supported":match.group(4)=="true",
                    "create":match.group(5),
                    "failure":match.group(6),
                })
        if tag=="GraphicBufferAllocator" and "Failed to allocate (4 x 4)" in msg:
            gfx.append({"pid":pid, "format":int(v) if (v:=re.search(r"format (\d+)",msg)) else None})
    pid=starts[-1] if starts else None
    trials=[x for x in candidates if x["pid"]==pid] if pid else []
    names=[x["name"] for x in trials]
    valid=(pid is not None and pid in completes and
           len(trials)==len(EXPECTED) and
           len(set(names))==len(EXPECTED) and
           all(x["name"] in EXPECTED and
               (x["format"],x["usage"])==EXPECTED[x["name"]]
               for x in trials))
    cases={x["name"]:{k:v for k,v in x.items() if k not in ("pid","name")}
           for x in trials} if valid else {}
    exact_fail=bool(cases and all(cases[x]["create"]=="FAIL" for x in
                                 ("RGBA1010102_EXACT","R8_EXACT")))
    rgba_success=bool(cases and cases["RGBA8888_GPU_CONTROL"]["create"]=="PASS")
    if not valid:
        status="INDEPENDENT_GRAPHICS_PROBE_INCOMPLETE"
    elif exact_fail and rgba_success:
        status="VIRTUAL_ANDROID_FORMAT_LIMITATION_REPRODUCED_IN_SEPARATE_APP"
    elif exact_fail:
        status="EXACT_FORMATS_FAILED_BUT_RGBA_CONTROL_UNAVAILABLE"
    else:
        status="ORIGINAL_GRAPHICS_ERRORS_NOT_REPRODUCED_IN_SEPARATE_APP"
    return {
        "status":status,
        "independent_app_pid":pid,
        "probe_complete":valid,
        "cases":cases,
        "exact_original_formats_43_and_56_fail":exact_fail,
        "rgba8888_gpu_control_success":rgba_success,
        "graphicbuffer_allocator_failures_same_probe_pid":
            sum(x["pid"]==pid for x in gfx) if pid else 0,
        "game_installed":False,
        "game_executed":False,
        "security_check_rejection_cause_proven":False,
        "virtual_gpu_compatibility_not_game_security":True,
        "limitations":[
            "A generic buffer allocation failure does not explain the original game's security toast.",
            "Each case uses the exact (4 x 4 x 1) request, known formats and usage flags.",
            "Results apply only to this virtual Android image's allocator and app context.",
            "The original game was never installed in this independent experiment.",
        ],
    }
def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("log",type=Path)
    p.add_argument("--out",type=Path,required=True)
    a=p.parse_args()
    result=classify(a.log.read_text(encoding="utf-8",errors="replace"))
    a.out.mkdir(parents=True,exist_ok=True)
    (a.out/"gralloc_probe_verdict.json").write_text(
        json.dumps(result,indent=2)+"\n",encoding="utf-8")
    print(json.dumps(result,indent=2))
    if not result["probe_complete"]:
        raise SystemExit("Incomplete hardware buffer probe logs")
if __name__=="__main__":
    main()
