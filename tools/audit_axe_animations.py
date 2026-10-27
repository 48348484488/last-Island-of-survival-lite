#!/usr/bin/env python3
"""Audit axe/held-melee clips from the already committed NPZ and source AssetBundle.
Read-only: does not alter any original assets.
"""
import json
from pathlib import Path
import re
import traceback
import numpy as np

ROOT = Path(__file__).resolve().parents[1]
DATA = ROOT / "last_island_recreated_game" / "authentic_animations.npz"
SOURCE = ROOT / "raw_assets" / "assets" / "role" / "animationbundle.ab"
OUT = ROOT / "axe_animation_audit.json"
RX = re.compile(r"fu|dao|axe|hold|jinzhan|near|melee|stand|idle|shifu", re.I)

def serial(v):
    return v.tolist() if hasattr(v, "tolist") else v

def main():
    ret = {"source_npz": str(DATA.relative_to(ROOT)), "source_ab": str(SOURCE.relative_to(ROOT)),
           "stored": [], "raw_candidates": [], "raw_animation_count": 0, "raw_errors": [],
           "preferred_candidates": [], "bone_parent_status": ""}
    with np.load(DATA, allow_pickle=False) as f:
        keys = set(f.files)
        if "_bone_parent_json" in keys:
            par = json.loads(str(f["_bone_parent_json"][0]))
            ret["bone_parent_status"] = f"{len(par)} bones: " + ", ".join(
                f"{n} <- {par.get(n)}" for n in par if "hand" in n.lower() or "guadian" in n.lower())
        clips = sorted(k[:-5] for k in keys if k.endswith("_rots"))
        for name in clips:
            bone_names = [str(x) for x in f[name + "_bone_names"]]
            trans, rots = f[name + "_trans"], f[name + "_rots"]
            nframes, fps = int(f[name + "_n_frames"][0]), float(f[name + "_fps"][0])
            entry = {"name": name, "frames": nframes, "fps": fps,
                     "duration_sec": round(nframes/max(fps,1),3),
                     "bones": len(bone_names), "trans_shape": list(trans.shape),
                     "rots_shape": list(rots.shape),
                     "hand_bones": [x for x in bone_names if re.search(r"hand|finger|thumb|guadian",x,re.I)]}
            for index,bone in enumerate(bone_names):
                if "wuqi_guadian" == bone or bone.lower().endswith("r hand"):
                    entry.setdefault("sample_bones",{})[bone] = {
                      "first_translation": serial(trans[index,0]),
                      "first_quaternion_xyzw": serial(rots[index,0]),
                      "translation_change": float(np.linalg.norm(trans[index]-trans[index,0],axis=1).max())
                    }
            ret["stored"].append(entry)
            if RX.search(name):
                ret["preferred_candidates"].append(name)
    try:
        import UnityPy
        env = UnityPy.load(str(SOURCE))
        for obj in env.objects:
            if obj.type.name != "AnimationClip":
                continue
            ret["raw_animation_count"] += 1
            try:
                data = obj.read()
                name = str(data.m_Name)
                if RX.search(name):
                    ret["raw_candidates"].append({
                        "name": name, "path_id": str(obj.path_id),
                        "sample_rate": getattr(data,"m_SampleRate",None),
                        "length": getattr(getattr(data,"m_MuscleClip",None),"m_StopTime",None)
                    })
            except Exception as e:
                ret["raw_errors"].append(f"clip read: {str(e)[:180]}")
    except Exception as e:
        ret["raw_errors"].append("UnityPy decode: "+str(e)+"\n"+traceback.format_exc(limit=2))
    ret["raw_candidates"].sort(key=lambda x:x["name"])
    OUT.write_text(json.dumps(ret,indent=2,ensure_ascii=False,default=str),encoding="utf-8")
    print("NPZ_ANIMATION_COUNT",len(ret["stored"]))
    for x in ret["stored"]:
        print("STORED",x["name"],"frames",x["frames"],"fps",x["fps"],"bones",x["bones"],"hand",len(x["hand_bones"]))
    print("BONE_PARENTS",ret["bone_parent_status"])
    print("RAW_COUNT",ret["raw_animation_count"],"RAW_RELEVANT_COUNT",len(ret["raw_candidates"]))
    for x in ret["raw_candidates"][:200]:
        print("RAW_RELEVANT",x["name"],"path_id",x["path_id"])
    print("RAW_ERRORS",ret["raw_errors"][:3])
    print("ARTIFACT",OUT)

if __name__ == "__main__":
    main()
