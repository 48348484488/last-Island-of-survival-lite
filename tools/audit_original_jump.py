#!/usr/bin/env python3
"""Source-based validation of the original melee jump/fall/land 3D animation clips."""
import json, sys
from pathlib import Path
import UnityPy,numpy as np
from restore_original_melee_run import decode_curves,ROOT,GAME
PHASES={
 "jump_up":("1718510388457734182","kongshou_zhan_jumpback_up02","371640738070943523","kongshou_zhan_jump_up02"),
 "in_air":("2232039169493632919","kongshou_zhan_jumpback_idle02","-2093151076388574590","kongshou_zhan_jump_idle02"),
 "landing":("7879379039287101303","kongshou_zhan_jumpback_down02","7736794628134902726","kongshou_zhan_jump_down02"),
}
with np.load(GAME/"authentic_animations.npz",allow_pickle=True) as a:
 names=[str(x) for x in a["zhan_stand01_bone_names"]]
 parents=json.loads(str(a["_bone_parent_json"][0]))
asset=UnityPy.load(str(ROOT/"raw_assets/assets/role/animationbundle.ab"))
files={str(o.path_id):o for o in asset.objects if o.type.name=="AnimationClip"}
output={"source":"raw_assets/assets/role/animationbundle.ab","phases":[]}
for phase,record in PHASES.items():
 for variant,(pid,expected) in {"back":record[:2],"front":record[2:]}.items():
  o=files[pid];clip=o.read_typetree();nam=o.read().m_Name
  assert nam==expected,(pid,nam,expected)
  d=clip["m_MuscleClip"]["m_Clip"]["data"]
  m=d["m_DenseClip"]
  info={"phase":phase,"variant":variant,"path_id":pid,"clip":nam,
      "frames":m["m_FrameCount"],"fps":m["m_SampleRate"],
      "stream_curve_count":d["m_StreamedClip"]["curveCount"],
      "dense_curve_count":m["m_CurveCount"],
      "constant_curve_count":len(d["m_ConstantClip"]["data"]),
      "binding_count":len(clip.get("m_ClipBindingConstant",{}).get("genericBindings",[]))}
  try:
   tr,rots,nf,fps,angles,scale=decode_curves(clip,names,parents)
   info.update({"decoded":True,"leg_rotation_range_degrees":angles,
        "scale_curve_peak":scale,
        "max_joint_displacement":float(np.max(np.ptp(tr,axis=1))),
        "finite":bool(np.isfinite(rots).all())})
  except Exception as ex:
   info.update({"decoded":False,"error":type(ex).__name__+": "+str(ex)[:220]})
  output["phases"].append(info)
  print("ORIGINAL_JUMP_PHASE",json.dumps(info,ensure_ascii=False))
P=ROOT/"jump_original_audit";P.mkdir(exist_ok=True)
(P/"report.json").write_text(json.dumps(output,indent=2,ensure_ascii=False))
print("SOURCE_JUMP_CLIPS_INSPECTED",len(output["phases"]))
