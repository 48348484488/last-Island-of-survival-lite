#!/usr/bin/env python3
"""Read saved RectTransforms to select real HUD interactive controls."""
import json
import sys
from pathlib import Path
R=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(R/"last_island_recreated_game"))
from original_hud_bindings import _layout,original_touch_rects,hit_test
p=R/"last_island_recreated_game/assets/hud_original_xapk/complete_source_mapping/hud_original_all_positions.json"
data=_layout(str(p))
print("SOURCE_TOUCH_DATA_TYPES",type(data["nodes"]).__name__,type(data["clickable"]).__name__,flush=True)
nodes=list(data["nodes"].values()) if isinstance(data["nodes"],dict) else data["nodes"]
print("SOURCE_NODE_SAMPLE",json.dumps(nodes[:2],ensure_ascii=False)[:4000],flush=True)
joysticks=[x for x in nodes if isinstance(x,dict) and
           any(s in str(x.get("name","")).lower() for s in ("joystack","joystick","yaogan"))]
print("SOURCE_JOYSTICKS",json.dumps(joysticks,ensure_ascii=False)[:10000],flush=True)
for profile in ("on-foot-melee","on-foot-rifle"):
    controls=original_touch_rects(profile=profile)
    result={}
    for action,b in controls.items():
        p=((b["left"]+b["right"])/2,(b["top"]+b["bottom"])/2)
        try:hit=hit_test(*p,profile=profile)
        except ValueError as e:hit=str(e)
        result[action]={"rect":b,"center":p,"center_hit":hit}
    print("SOURCE_TOUCH_PROFILE",profile,json.dumps(result,ensure_ascii=False),flush=True)
