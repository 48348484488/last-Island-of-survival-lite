#!/usr/bin/env python3
"""Extract AUTHENTIC crouch/prone states from original Unity 72 MB animationbundle.ab.

Reuse proven AnimationClip streamed+dense+constant curve decoder and the exact
64-bone native rig. The old 33-clip NPZ is the base; source clips are appended
only inside CI checkout, then serialized to a native Android 64-bone asset.
"""
import argparse
import json
import sys
from pathlib import Path
import numpy as np
import UnityPy
ROOT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(ROOT/"tools"))
from restore_original_melee_run import decode_curves

STANCE_CLIPS=[
    # Original: Base Layer.Crouch (10-branch blend tree)
    "kongshou_dun_stand01",
    "dun_run_front01","dun_run_back01","dun_run_left01","dun_run_right01",
    "dun_run_leftback01","dun_run_leftfront01",
    "dun_run_rightback01","dun_run_rightfront01","dun_rush_front01",
    # Original: Base Layer.Pa.Pa (5-branch prone blend tree)
    "pa_stand01",
    "pa_walk_front01","pa_walk_back01","pa_walk_left01","pa_walk_right01",
    # Original Base Layer.CrouchDown & PaUp
    "kongshou_dun_qiepa01","kongshou_pa_qiedun01",
    "kongshou_pa_qiezhan01",
    # Original rifle-specific crouch & prone upper/full-body source
    "chongfengqiang_hengwo_dun_miaozhun_stand01",
    "chongfengqiang_hengwo_dun_miaozhun_run_front01",
    "chongfengqiang_hengwo_dun_miaozhun_attack01",
    "chongfengqiang_hengwo_pa_miaozhun_stand01",
    "chongfengqiang_hengwo_pa_miaozhun_attack01",
    "chongfengqiang_hengwo_dun_qiepa01",
    "chongfengqiang_hengwo_pa_qiedun01",
    "chiqiang_pa_qiezhan01",
]
def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("--source",type=Path,default=ROOT/"raw_assets/assets/role/animationbundle.ab")
    ap.add_argument("--npz",type=Path,default=ROOT/"last_island_recreated_game/authentic_animations.npz")
    ap.add_argument("--report",type=Path,default=Path("original_stances_export.json"))
    args=ap.parse_args()
    print("SOURCE_STANCE_CONVERSION_START",len(STANCE_CLIPS),flush=True)
    with np.load(args.npz,allow_pickle=True) as a:
        base={key:a[key].copy() for key in a.files}
    base_names=[str(n) for n in base["zhan_stand01_bone_names"]]
    parents=json.loads(str(base["_bone_parent_json"][0]))
    assert len(base_names)==64
    env=UnityPy.load(str(args.source))
    clips={}
    for o in env.objects:
        if o.type.name!="AnimationClip":continue
        name=str(o.read().m_Name)
        if name in STANCE_CLIPS:
            # Ignore duplicates of same name unless we intentionally validate.
            clips.setdefault(name,o)
    missing=sorted(set(STANCE_CLIPS)-set(clips))
    if missing:raise RuntimeError(f"Original animation clips absent in source: {missing}")
    found={}
    for name in STANCE_CLIPS:
        original=clips[name].read_typetree()
        trans,rots,nf,fps,angles,scale=decode_curves(original,base_names,parents,allow_extra_bones=True)
        assert (trans.shape,rots.shape)==((64,nf,3),(64,nf,4))
        assert np.isfinite(trans).all() and np.isfinite(rots).all()
        assert nf>=2 and 1<=fps<=120
        base[name+"_bone_names"]=np.asarray(base_names)
        base[name+"_n_frames"]=np.asarray([nf],np.int32)
        base[name+"_fps"]=np.asarray([fps],np.float32)
        base[name+"_trans"]=trans
        base[name+"_rots"]=rots
        found[name]=dict(source_unity_path_id=str(clips[name].path_id),
                         frames=nf,fps=fps,leg_rotation_max_deg=angles)
        print("EXTRACTED_ORIGINAL_STANCE",name,str(clips[name].path_id),
              "frames",nf,"fps",fps,"leg_angles",angles,flush=True)
    args.npz.parent.mkdir(parents=True,exist_ok=True)
    tmp=args.npz.with_suffix(".staging.npz")
    np.savez_compressed(tmp,**base)
    with np.load(tmp,allow_pickle=True) as a:
        n=sum(1 for name in a.files if name.endswith("_rots"))
        assert n==33+len(STANCE_CLIPS),(n,len(STANCE_CLIPS))
        for name in STANCE_CLIPS:
            assert a[name+"_rots"].shape[0]==64
    tmp.replace(args.npz)
    result=dict(original_clip_count_before=33,source_stances=len(found),
                total_native_clips=n,clips=found)
    args.report.write_text(json.dumps(result,indent=2,ensure_ascii=False))
    print("LIOS_SOURCE_STANCES_EXPORT_PASS",n,len(found),flush=True)
if __name__=="__main__":main()
