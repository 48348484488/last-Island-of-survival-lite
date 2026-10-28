#!/usr/bin/env python3
"""Source-derived UGUI multi-touch controller regression (no graphical context)."""
import json
import math
import sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(ROOT/"last_island_recreated_game"))
from original_hud_bindings import original_touch_rects,hit_test
from original_hud_runtime import viewport_bounds
from original_hud_touch import OriginalHudTouch,original_joystick

def screen(x,y,w=960,h=540):
    left,top,width,height=viewport_bounds(w,h)
    return (left+x*width/1920,top+y*height/1080)

def center(rect):
    return ((rect["left"]+rect["right"])/2,(rect["top"]+rect["bottom"])/2)

def put(t,finger,source,wh=(960,540)):
    return t.start(finger,*screen(*source,*wh),*wh)

def test(profile):
    t=OriginalHudTouch(profile=profile)
    actions=original_touch_rects(profile=profile)
    must={"attack","jump","inventory","craft","auto_run","crouch","prone"}
    assert must<=actions.keys()
    assert ("aim" in actions)==(profile=="on-foot-rifle")
    assert ("reload" in actions)==(profile=="on-foot-rifle")
    for key,rect in actions.items():
        p=center(rect)
        assert hit_test(*p,profile=profile)==key, (key,rect)
        assert put(t,101,p)==[(key,"down")],key
        assert t.held(key)
        assert put(t,101,p)==[], "double-down must not fire twice"
        assert put(t,102,p)==[], "two-finger press must not double-fire"
        assert t.end(101)==[], "releasing one of two fingers must keep action"
        assert t.held(key)
        assert t.end(102)==[(key,"up")]
        assert not t.held(key)
    assert not t.held_actions
    joy=original_joystick()
    assert joy["radius"]>100
    assert 0<=joy["center"][0]<960
    assert 500<joy["center"][1]<1080
    assert put(t,1,joy["center"])==[]
    assert t.axis==(0.0,0.0)
    cx,cy=joy["center"]
    t.move(1,*screen(cx+joy["radius"]*.7,cy-joy["radius"]*.5),960,540)
    assert abs(t.axis[0]-.7)<1e-5 and abs(t.axis[1]+.5)<1e-5,t.axis
    # Keep joystick held while firing and jumping with separate fingers.
    assert put(t,2,center(actions["attack"]))==[("attack","down")]
    assert put(t,3,center(actions["jump"]))==[("jump","down")]
    assert t.held("attack") and t.held("jump")
    assert t.axis!=(0,0)
    assert t.end(3)==[("jump","up")]
    assert t.held("attack")
    assert t.end(1)==[]
    assert t.axis==(0,0)
    assert t.end(2)==[("attack","up")]
    assert not t.held_actions
    # A 2340x1080 viewport must honor the same centered 1920x1080 HUD.
    assert viewport_bounds(2340,1080)==(210,0,1920,1080)
    assert put(t,50,center(actions["jump"]),(2340,1080))==[("jump","down")]
    assert t.end(50)==[("jump","up")]
    assert t.start(51,0,540,2340,1080)==[], "outside Canvas must be ignored"
    assert t.end(51)==[]
    if profile=="on-foot-rifle":
        assert put(t,44,center(actions["reload"]))==[("reload","down")]
        assert t.set_profile("on-foot-melee")==[("reload","up")]
        assert not t.held("reload")
        assert put(t,45,center(actions["aim"]))==[], "aim must not exist in melee"
        t.end(45)
    summary={"profile":profile,"buttons":len(actions),"joystick_radius_source_px":joy["radius"],
             "joystick_exact_source_path_ids":joy["source"],
             "multitouch_simultaneous_jump_and_attack":True,
             "letterboxed_2340x1080_test":True}
    print("ORIGINAL_HUD_TOUCH_SOURCE_TEST_PASS",json.dumps(summary))
if __name__=="__main__":
    test("on-foot-melee")
    test("on-foot-rifle")
