#!/usr/bin/env python3
"""Locate exact source equip/unequip tracks and their Unity Animator states.
Uses the exported original Unity runtime Player code as the state selector.
No inference from similar-looking poses.
"""
from pathlib import Path
import json
import UnityPy

R=Path(__file__).resolve().parents[1]
O=R/"equip_original_audit";O.mkdir(exist_ok=True)
bund=UnityPy.load(str(R/"raw_assets/assets/role/role.ab"))
ctrl=next(o.read_typetree() for o in bund.objects
          if o.type.name=="AnimatorController" and o.read().m_Name=="man")
names={int(k):v for k,v in ctrl["m_TOS"]}
clipmap={}
clips=UnityPy.load(str(R/"raw_assets/assets/role/animationbundle.ab"))
for obj in clips.objects:
 if obj.type.name=="AnimationClip":
  clipmap[str(obj.path_id)]=obj
all_states=[]
for li,machine in enumerate(ctrl["m_Controller"]["m_StateMachineArray"]):
 for state in machine["data"]["m_StateConstantArray"]:
  s=state["data"];full=names.get(int(s["m_FullPathID"]),"")
  if not ("Nawuqi." in full or "Shouwuqi." in full):continue
  items=[]
  for bt in s["m_BlendTreeConstantArray"]:
   for child in bt["data"]["m_NodeArray"]:
    i=int(child["data"]["m_ClipID"])
    if i==4294967295:continue
    id=str(ctrl["m_AnimationClips"][i]["m_PathID"])
    obj=clipmap.get(id)
    if obj is None:items.append({"path_id":id,"missing":True});continue
    d=obj.read_typetree();m=d["m_MuscleClip"]["m_Clip"]["data"]
    dense=m["m_DenseClip"]
    items.append({"path_id":id,"name":obj.read().m_Name,"frames":dense["m_FrameCount"],
                  "fps":dense["m_SampleRate"],"curve_counts":{
                   "stream":m["m_StreamedClip"]["curveCount"],
                   "dense":dense["m_CurveCount"],
                   "constant":len(m["m_ConstantClip"]["data"])
                   }})
  if items: all_states.append({"state":full,"layer":li,"clips":items})
want=["Nawuqi.dao_zhan_nawuqi01","Shouwuqi.dao_zhan_shouwuqi01",
 "Nawuqi.chongfengqiang_hengwo_zhan_right_nawuqi01",
 "Shouwuqi.chongfengqiang_hengwo_zhan_right_shouwuqi01"]
found={}
for x in all_states:
 for suffix in want:
  if x["state"].endswith("."+suffix):
   found[suffix]=x
assert set(found)==set(want),(set(want)-set(found),list(found))
assert all(len(x["clips"]) for x in found.values())
report={"source_controller":"raw_assets/assets/role/role.ab",
        "source_clips":"raw_assets/assets/role/animationbundle.ab",
        "selector":"NawuqiState.cs / ShouwuqiState.cs",
        "selected":found,
        "all_equip_states":all_states}
(O/"report.json").write_text(json.dumps(report,indent=2,ensure_ascii=False))
for nm,x in found.items():
 print("SOURCE_WEAPON_STATE",nm,json.dumps(x,ensure_ascii=False))
print("TOTAL_EQUIP_STATES",len(all_states))
