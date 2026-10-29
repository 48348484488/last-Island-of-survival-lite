#!/usr/bin/env python3
"""Read original Unity animation bundle and original 'man' AnimatorController.
No reliance on clip-name guesses; report ALL crouch/prone state references.
"""
import json
from collections import Counter
from pathlib import Path
import UnityPy

ROOT=Path(__file__).resolve().parents[1]
A=ROOT/"raw_assets/assets/role/animationbundle.ab"
R=ROOT/"raw_assets/assets/role/role.ab"
OUT=Path("stance_source_audit.json")

def main():
    print("ORIGINAL_STANCE_AUDIT_OPEN",str(A),A.stat().st_size,flush=True)
    env=UnityPy.load(str(A))
    clips={}
    for obj in env.objects:
        if obj.type.name=="AnimationClip":
            try:
                name=str(obj.read().m_Name)
                clips[str(obj.path_id)]=name
            except Exception as ex:
                print("CLIP_READ_FAIL",obj.path_id,str(ex)[:180],flush=True)
    print("ORIGINAL_ALL_ANIMATIONCLIPS",len(clips),flush=True)
    print("ORIGINAL_STANCE_CANDIDATES",json.dumps(
        sorted((id,name) for id,name in clips.items()
               if any(t in name.lower() for t in
                      ("dun","pa_","pa01","squat","crouch","prone","lie","pafu","qiepa","qiedun","paidle","papao"))),
        ensure_ascii=False),flush=True)
    cenv=UnityPy.load(str(R))
    controllers=[o for o in cenv.objects if o.type.name=="AnimatorController"]
    matches=[]
    for o in controllers:
        d=o.read_typetree()
        if d.get("m_Name")!="man":continue
        tos={int(k):v for k,v in d.get("m_TOS",[])}
        refs=d.get("m_AnimationClips",[])
        for layer,ma in enumerate(d["m_Controller"]["m_StateMachineArray"]):
            for state in ma["data"]["m_StateConstantArray"]:
                st=state["data"]
                name=tos.get(int(st["m_FullPathID"]),"")
                if not any(t in name.lower() for t in ("crouch","pa.", "paup","crouchdown","standup","sit.","pa_")):
                    continue
                nodes=[]
                for tree in st.get("m_BlendTreeConstantArray",[]):
                    for node in tree["data"].get("m_NodeArray",[]):
                        nd=node["data"];i=int(nd.get("m_ClipID",-1))
                        if i<0 or i>=len(refs):continue
                        ident=str(refs[i]["m_PathID"])
                        nodes.append(dict(id=ident,name=clips.get(ident),threshold=nd.get("m_Threshold")))
                matches.append(dict(state=name,layer=layer,clips=nodes))
        break
    summary=dict(total_animations=len(clips),all_clips=sorted(clips.items(),key=lambda x:x[1]),
                 original_stance_states=matches,assetbundle=str(A))
    OUT.write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding="utf8")
    print("ORIGINAL_STANCE_CONTROLLER_STATES",json.dumps(matches,ensure_ascii=False)[:35000],flush=True)
    print("ORIGINAL_STANCE_AUDIT_DONE",len(matches),flush=True)
if __name__=="__main__":main()
