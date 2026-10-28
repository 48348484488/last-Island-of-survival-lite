"""Source-grounded HUD touch controls for the recreated Last Island LITE 7.3.

Reads validated Unity RectTransform hitboxes and original Image.m_Sprite IDs.
This does not call OpenGL or use manually placed action buttons.
Unity runtime CanvasScaler and device safe-area still need calibration.
"""
from functools import lru_cache
from pathlib import Path
import json

DEFAULT_LAYOUT=Path(__file__).resolve().parent/"assets/hud_original_xapk/complete_source_mapping/hud_original_all_positions.json"

# Original Unity GameObject names identified in the on-foot battlepanel.ab.
# These are game actions, not invented positions.
ACTIONS={
    "btn_fire":"attack",
    "btn_up2":"jump",
    "btn_crouch2":"crouch",
    "btn_pa2":"prone",
    "btn_autoRun":"auto_run",
    "btn_pack":"inventory",
    "btn_zhizao":"craft",
    "btn_kaijing":"aim",
    "btn_changeBullet":"reload",
    "btn_setting":"settings",
    "btn_quckChat":"quick_chat",
}
RIFLE_ONLY=frozenset({"aim","reload"})

@lru_cache(maxsize=3)
def _layout(path):
    data=json.loads(Path(path).read_text(encoding="utf8"))
    assert len(data["nodes"])==1716
    assert len(data["clickable"])>=100
    return data

def original_touch_rects(*,profile="on-foot-melee",path=DEFAULT_LAYOUT):
    """Get exact source-prefab virtual 1920x1080 touch rects for visible mode.

    Returned bounds have top-left origin; scale to a different viewport only
    after independently validating the original game's CanvasScaler/safe-area.
    """
    if profile not in ("on-foot-melee","on-foot-rifle"):
        raise ValueError("Unknown original Unity on-foot profile: "+str(profile))
    data=_layout(str(path))
    index={}
    for src in data["clickable"]:
        name=src["name"]
        action=ACTIONS.get(name)
        if action is None or (action in RIFLE_ONLY and profile=="on-foot-melee"):
            continue
        if action in index:
            raise ValueError("Ambiguous original source touch button: "+name)
        p=src["bounds"]
        if not (p["left"]<p["right"] and p["top"]<p["bottom"]):
            raise ValueError("Invalid source button rect: "+name)
        index[action]=dict(p,name=name,gameobject_path_id=src["go_path_id"])
    assert {"attack","jump","crouch","prone","auto_run","inventory"}<=index.keys()
    return index

def hit_test(x,y,*,profile="on-foot-melee",path=DEFAULT_LAYOUT):
    """Return action under a 1920x1080 touch point or None.

    Overlapping interactive widgets are intentionally not given an invented
    priority. If multiple different buttons overlap, report the ambiguity.
    """
    hits=[]
    for action,b in original_touch_rects(profile=profile,path=path).items():
        if b["left"]<=x<=b["right"] and b["top"]<=y<=b["bottom"]:
            hits.append(action)
    if len(hits)>1:raise ValueError("Overlapping original HUD touch hitboxes: "+repr(hits))
    return hits[0] if hits else None
