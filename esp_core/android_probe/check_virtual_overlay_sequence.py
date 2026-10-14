#!/usr/bin/env python3
"""Pixel and JNI logs proof of changing SAME overlay without service restart.

Native ARM64 Android screenshots only. No original game, game process
memory or multiplayer telemetry is used; every entity is synthetic.
"""
import argparse
import json
import re
from pathlib import Path
from PIL import Image

PHASES=("INITIAL","MOVED","STALE","RESTORED","STOPPED")
LOG=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS PHASE=(INITIAL|MOVED|STALE|RESTORED)\s+"
               r"RECT_COUNT=(\d+)\s+SOURCE=SYNTHETIC_JNI")
STOP=re.compile(r"OXIDE_BRIDGE_OVERLAY:\s+RESULT=PASS PHASE=STOPPED "
                r"RECT_COUNT=0 WINDOW_REMOVED=TRUE")

def measure(path):
    with Image.open(path) as source:
        image=source.convert("RGB")
        w,h=image.size
        hits=0
        sum_x=0
        # Match the *actual* green projected box, not white text.
        for y in range(int(h*.30),int(h*.80)):
            for x in range(int(w*.20),int(w*.80)):
                r,g,b=image.getpixel((x,y))
                if 15<=r<=110 and g>=190 and b<=160 and g>r*2 and g>b*1.3:
                    hits+=1
                    sum_x+=x
        return {"green_pixels":hits,"center_x":round(sum_x/hits,2) if hits else None,
                "width":w,"height":h}

def evaluate(folder):
    root=Path(folder)
    images={phase:measure(root/("sequence_"+phase.lower()+".png")) for phase in PHASES}
    log=(root/"sequence_logcat.txt").read_text(errors="replace")
    records=list(LOG.finditer(log))
    by_phase={phase:[int(m.group(2)) for m in records if m.group(1)==phase]
              for phase in PHASES[:-1]}
    pids={phase:(root/("sequence_pid_"+phase.lower()+".txt")).read_text().strip()
          for phase in PHASES}
    checks={}
    checks["native_four_phase_transitions_exactly_once"]=all(
        by_phase[phase]==[0 if phase=="STALE" else 1]
        for phase in PHASES[:-1])
    checks["window_automatically_removed"]=len(STOP.findall(log))==1
    initial=images["INITIAL"]
    moved=images["MOVED"]
    stale=images["STALE"]
    restored=images["RESTORED"]
    stopped=images["STOPPED"]
    checks["initial_visible"]=initial["green_pixels"]>=500
    checks["moved_visible"]=moved["green_pixels"]>=500
    checks["restored_visible"]=restored["green_pixels"]>=500
    checks["stale_cleared"]=stale["green_pixels"]<=max(
        35,int(initial["green_pixels"]*.04),stopped["green_pixels"]+25)
    checks["final_cleared"]=stopped["green_pixels"]<=max(
        35,int(initial["green_pixels"]*.04))
    checks["moves_right"]=bool(initial["center_x"] is not None and
        moved["center_x"] is not None and
        moved["center_x"]-initial["center_x"]>=20)
    checks["restores_left"]=bool(initial["center_x"] is not None and
        restored["center_x"] is not None and
        initial["center_x"]-restored["center_x"]>=20)
    checks["no_crash"]=not re.search(
        r"AndroidRuntime:.*Process: com\.oxide\.bridgeprobe"
        r"|Fatal signal.*com\.oxide\.bridgeprobe"
        r"|OXIDE_BRIDGE_OVERLAY:.*RESULT=FAIL",log)
    live=[pids[p] for p in PHASES[:-1]]
    checks["same_service_pid_during_updates"]=bool(
        all(re.fullmatch(r"\d+",p) for p in live) and len(set(live))==1)
    # Android may cache the stopped app process. Window removal is
    # confirmed by onDestroy log plus actual cleared screenshot, not pidof.
    game=(root/"sequence_game_packages.txt").read_text(errors="replace")
    checks["game_not_installed"] = "package:com.catsbit.oxidesurvivalisland" not in game
    return {
        "status":"NATIVE_VIRTUAL_ARM64_CONTINUOUS_JNI_OVERLAY_PASS" if all(checks.values())
                 else "CONTINUOUS_OVERLAY_NOT_CONFIRMED",
        "checks":checks,
        "screenshot_geometry":images,
        "jni_phase_markers":by_phase,
        "stable_android_service_pids":len(set(live))==1 if live else False,
        "original_game_installed":not checks["game_not_installed"],
        "original_game_executed":False,
        "security_bypass_performed":False,
        "live_multiplayer_or_real_player_esp_verified":False,
        "meaning":"Single synthetic JNI+WindowManager overlay moves, clears stale inputs, "
                  "recovers after fresh input, and removes its window without restart.",
    }

def main():
    a=argparse.ArgumentParser(description=__doc__)
    a.add_argument("folder",type=Path)
    args=a.parse_args()
    result=evaluate(args.folder)
    (args.folder/"sequence_verdict.json").write_text(
        json.dumps(result,indent=2)+"\n",encoding="utf-8")
    print(json.dumps(result,indent=2))
    if result["status"]!="NATIVE_VIRTUAL_ARM64_CONTINUOUS_JNI_OVERLAY_PASS":
        raise SystemExit("Virtual synthetic continuous overlay proof FAILED")

if __name__=="__main__":
    main()
