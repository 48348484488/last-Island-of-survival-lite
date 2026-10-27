#!/usr/bin/env python3
"""Audits real Unity AnimatorController-to-AnimationClip references for axe vs melee.
It does not label generic melee or heavy-weapon poses as 'axe idle' without code linkage.
"""
import json
from pathlib import Path
import UnityPy

root=Path(__file__).resolve().parents[1]
controller_asset=root/"raw_assets/assets/role/role.ab"
source_asset=root/"raw_assets/assets/role/animationbundle.ab"
out=root/"axe_original_mapping_review"
out.mkdir(exist_ok=True)
clips={}
for obj in UnityPy.load(str(source_asset)).objects:
    if obj.type.name=="AnimationClip":
        clip=obj.read()
        clips[str(obj.path_id)]=str(clip.m_Name)
controller=None
for obj in UnityPy.load(str(controller_asset)).objects:
    if obj.type.name=="AnimatorController" and obj.read().m_Name=="man":
        controller=obj.read_typetree()
        break
assert controller is not None,"original 'man' AnimatorController missing"
tos={int(k):v for k,v in controller["m_TOS"]}
external=controller["m_AnimationClips"]
machines=controller["m_Controller"]["m_StateMachineArray"]
wanted=(
    "Base Layer.Stand.jinzhan.f",
    "Base Layer.Stand.jinzhan.b",
    "FullBody Layer.HoldJinzhanWeapon.shuangshouchui_stand",
    "FullBody Layer.Attack.fu_zhan_attack_01",
    "Base Layer.MainPanel.shuangshouchui_zhan_stand01",
)
found=[]
for mi,machine in enumerate(machines):
    for state in machine["data"]["m_StateConstantArray"]:
        st=state["data"]
        full_name=tos.get(int(st["m_FullPathID"]),"")
        if full_name not in wanted:
            continue
        tracks=[]
        for blend_tree in st["m_BlendTreeConstantArray"]:
            for node in blend_tree["data"]["m_NodeArray"]:
                nd=node["data"]
                clip_id=int(nd["m_ClipID"])
                if clip_id==4294967295:continue
                asset=external[clip_id]
                original_id=str(asset["m_PathID"])
                tracks.append({
                    "clip_index":clip_id,
                    "path_id":original_id,
                    "asset_name":clips.get(original_id,"not in animationbundle.ab"),
                })
        found.append({"state":full_name,"layer":mi,"clip_refs":tracks})
assert len(found)==len(wanted),f"Only {len(found)}/{len(wanted)} target states found"
byname={x["state"]:x for x in found}
assert any(x["path_id"]=="2004069720870033944" for x in byname["Base Layer.Stand.jinzhan.f"]["clip_refs"])
assert any(x["path_id"]=="4382079750961674410" for x in byname["FullBody Layer.Attack.fu_zhan_attack_01"]["clip_refs"])
assert any(x["path_id"]=="-4139790898113436251" for x in byname["FullBody Layer.HoldJinzhanWeapon.shuangshouchui_stand"]["clip_refs"])
report={
 "source_assets":[str(controller_asset.relative_to(root)),str(source_asset.relative_to(root))],
 "scope":"Source asset references only: weapon-specific selection requires runtime/config validation.",
 "confirmed":{
  "melee_base_idle":"Stand.jinzhan.f -> zhan_stand01; this is a shared melee movement state, NOT proven axe-only holding pose",
  "axe_attack":"Attack.fu_zhan_attack_01 is used by weapon IDs 55001..55005 in original AttackState.cs",
  "heavy_stand":"HoldJinzhanWeapon.shuangshouchui_stand contains original shuangshouchui_zhan_stand01, but has NOT been proven to run for stone axe",
  "visual_alignment":"Unconfirmed: third-party reconstruction includes character skinning and weapon-socket transform assumptions"
 },
 "states":found,
 "not_verified":["Exact in-game stone axe idle posture","Correct loaded Animator layer weights at runtime","Grip with weapon prefab's original child transforms and bone animation","First-person axe-specific view"]
}
(out/"source_state_mapping.json").write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding="utf-8")
for item in found:
    print("STATE",item["state"],"layer",item["layer"])
    for clip in item["clip_refs"]:
        print(" ORIGINAL_ASSET",clip["asset_name"],"path_id",clip["path_id"])
print("IMPORTANT Original axe-holding idle not yet uniquely verified.")
