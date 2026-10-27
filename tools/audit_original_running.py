#!/usr/bin/env python3
"""Audit exact run animation selected by original Unity AnimatorController vs local game."""
from pathlib import Path
import json
import numpy as np, UnityPy
R=Path(__file__).resolve().parents[1]
g=R/"last_island_recreated_game"
c=next(o.read_typetree() for o in UnityPy.load(str(R/"raw_assets/assets/role/role.ab")).objects if o.type.name=="AnimatorController" and o.read().m_Name=="man")
ref_name={str(o.path_id):str(o.read().m_Name) for o in UnityPy.load(str(R/"raw_assets/assets/role/animationbundle.ab")).objects if o.type.name=="AnimationClip"}
tos={int(k):v for k,v in c["m_TOS"]}
states=[]
for lay,mach in enumerate(c["m_Controller"]["m_StateMachineArray"]):
 for state in mach["data"]["m_StateConstantArray"]:
  d=state["data"];name=tos.get(int(d["m_FullPathID"]),"")
  if name not in ("Base Layer.Stand.jinzhan.f","Base Layer.Stand.jinzhan.b"):continue
  branches=[]
  for tree in d["m_BlendTreeConstantArray"]:
   for node in tree["data"]["m_NodeArray"]:
    nd=node["data"];i=int(nd["m_ClipID"])
    if i==4294967295:continue
    id=str(c["m_AnimationClips"][i]["m_PathID"])
    branches.append({"clip":ref_name.get(id),"path_id":id,
                     "threshold":nd.get("m_Threshold"),"position":nd.get("m_Position"),
                     "node_keys":list(nd.keys())})
  states.append({"state":name,"layer":lay,"branches":branches})
with np.load(g/"authentic_animations.npz",allow_pickle=True) as a:
 existing=sorted(x[:-5] for x in a.files if x.endswith("_rots"))
 clip_data={}
 for name in existing:
  clip_data[name]={"fps":float(a[name+"_fps"][0]),
                   "frames":int(a[name+"_n_frames"][0])}
out={"original_animator_states":states,"game_extracted_clips":clip_data,
     "critical":"Do not guess forward run; compare original Animator selected running clip with NPZ"}
P=R/"running_original_audit";P.mkdir(exist_ok=True)
(P/"report.json").write_text(json.dumps(out,indent=2,ensure_ascii=False))
print("ORIGINAL_MELEE_STATES",json.dumps(states,ensure_ascii=False)[:10000])
print("AVAILABLE_NPZ_CLIPS",json.dumps(clip_data,ensure_ascii=False))
