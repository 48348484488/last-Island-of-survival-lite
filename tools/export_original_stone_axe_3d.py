#!/usr/bin/env python3
"""Rebuild original stone axe 3D, using Unity source prefab bone/bindpose (no hand-tuned offsets).
Clip mapping comes from original role.ab. This is NOT a game-runtime screenshot.
"""
import json,struct,subprocess,sys
from pathlib import Path
import numpy as np, UnityPy, trimesh
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"original_axe_3d_verified"
OUT.mkdir(exist_ok=True)
def pointer(d):return str(d.get("m_PathID",0))
def transform_matrix(t):
 p=t["m_LocalPosition"];q=t["m_LocalRotation"];s=t["m_LocalScale"]
 x,y,z,w=[q[k] for k in "xyzw"]
 m=np.eye(4)
 m[:3,:3]=np.array([[1-2*(y*y+z*z),2*(x*y-z*w),2*(x*z+y*w)],
                     [2*(x*y+z*w),1-2*(x*x+z*z),2*(y*z-x*w)],
                     [2*(x*z-y*w),2*(y*z+x*w),1-2*(x*x+y*y)]])
 m[:3,:3]@=np.diag([s[k] for k in "xyz"])
 m[:3,3]=[p[k] for k in "xyz"]
 return m
def source_matrix():
 env=UnityPy.load(str(ROOT/"raw_assets/assets/model/wq_shifu.ab"))
 t={str(o.path_id):o.read_typetree() for o in env.objects if o.type.name=="Transform"}
 mesh=next(o.read_typetree() for o in env.objects if o.type.name=="Mesh")
 rend=next(o.read_typetree() for o in env.objects if o.type.name=="SkinnedMeshRenderer")
 assert mesh["m_SubMeshes"][0]["vertexCount"]==408
 assert len(rend["m_Bones"])==len(mesh["m_BindPose"])==1
 bone=pointer(rend["m_Bones"][0])
 assert pointer(rend["m_RootBone"])==bone
 def world(k):
  d=t[k];p=pointer(d["m_Father"])
  return (world(p) if p!="0" else np.eye(4))@transform_matrix(d)
 bp=mesh["m_BindPose"][0]
 inv=np.array([[bp[f"e{i}{j}"] for j in range(4)] for i in range(4)])
 return world(bone)@inv,bone
def audit_controller():
 env=UnityPy.load(str(ROOT/"raw_assets/assets/role/role.ab"))
 obj=next(o for o in env.objects if o.type.name=="AnimatorController" and o.read().m_Name=="man")
 c=obj.read_typetree();tos={int(k):v for k,v in c["m_TOS"]}
 refs={}
 for machine in c["m_Controller"]["m_StateMachineArray"]:
  for state in machine["data"]["m_StateConstantArray"]:
   d=state["data"];name=tos.get(int(d["m_FullPathID"]))
   if name not in ("Base Layer.Stand.jinzhan.f","FullBody Layer.Attack.fu_zhan_attack_01"):continue
   refs[name]=[]
   for tree in d["m_BlendTreeConstantArray"]:
    for node in tree["data"]["m_NodeArray"]:
     i=int(node["data"]["m_ClipID"])
     if i!=4294967295:refs[name].append(str(c["m_AnimationClips"][i]["m_PathID"]))
 assert "2004069720870033944" in refs["Base Layer.Stand.jinzhan.f"]
 assert "4382079750961674410" in refs["FullBody Layer.Attack.fu_zhan_attack_01"]
 return refs
def main():
 m,bone=source_matrix();refs=audit_controller()
 subprocess.run([sys.executable,"tools/export_axe_hold_animation.py"],cwd=ROOT,check=True,stdout=subprocess.DEVNULL)
 src=(ROOT/"axe_hold_extracted/original_axe_holding_candidates.glb").read_bytes()
 assert src[:4]==b"glTF"
 n=struct.unpack_from("<I",src,12)[0];g=json.loads(src[20:20+n])
 off=20+n;L=struct.unpack_from("<I",src,off)[0];binbuf=src[off+8:off+8+L]
 nodes=g["nodes"]
 axe=next(i for i,x in enumerate(nodes) if x["name"]=="StoneAxe_wq_shifu")
 socket=next(i for i,x in enumerate(nodes) if x["name"]=="wuqi_guadian")
 assert axe in nodes[socket]["children"]
 for key in ("translation","rotation","scale"):nodes[axe].pop(key,None)
 nodes[axe]["matrix"]=m.T.flatten().tolist()
 valid={"zhan_stand01","fu_zhan_attack_01"}
 g["animations"]=[a for a in g["animations"] if a["name"] in valid]
 assert {a["name"] for a in g["animations"]}==valid
 # Open the GLB in its correct stone-axe idle pose even in viewers without animation playback
 idle=next(a for a in g["animations"] if a["name"]=="zhan_stand01")
 for channel in idle["channels"]:
  samp=idle["samplers"][channel["sampler"]];acc=g["accessors"][samp["output"]]
  view=g["bufferViews"][acc["bufferView"]]
  nvals=4 if acc["type"]=="VEC4" else 3
  start=view.get("byteOffset",0)+acc.get("byteOffset",0)
  nodes[channel["target"]["node"]][channel["target"]["path"]]=np.frombuffer(binbuf,np.float32,count=nvals,offset=start).tolist()
 g["asset"]["generator"]="Original Last Island stone axe reconstructed from Unity prefab bone and bindpose"
 g["asset"]["extras"]={"source":"wq_shifu.ab / animationbundle.ab / role.ab",
   "idle_clip_path_id":"2004069720870033944",
   "attack_clip_path_id":"4382079750961674410",
   "limitation":"Source-derived scene, not an APK-engine screenshot; live IK not verified"}
 j=json.dumps(g,ensure_ascii=False,separators=(",",":")).encode()
 j+=b" "*((-len(j))%4);binbuf+=b"\0"*((-len(binbuf))%4)
 result=struct.pack("<III",0x46546c67,2,12+8+len(j)+8+len(binbuf))
 result+=struct.pack("<II",len(j),0x4e4f534a)+j
 result+=struct.pack("<II",len(binbuf),0x004e4942)+binbuf
 dest=OUT/"machado_original_postura_e_golpe_3d.glb";dest.write_bytes(result)
 scene=trimesh.load(dest,force="scene")
 assert len(scene.geometry)==8
 assert any(len(v.vertices)==408 for v in scene.geometry.values())
 proof={"source_prefab":"raw_assets/assets/model/wq_shifu.ab",
        "source_controller":"raw_assets/assets/role/role.ab",
        "source_animations":"raw_assets/assets/role/animationbundle.ab",
        "weapon_root_socket":"wuqi_guadian",
        "weapon_root_local_position":[0,0,0],
        "weapon_bone_path_id":bone,"original_weapon_mesh_matrix":m.tolist(),
        "controller_state_references":refs,
        "meshes":len(scene.geometry),"axe_vertices":408,
        "animated_clips":sorted(valid),
        "model_validation":"trimesh load PASS",
        "limitation":"Source assets reconstructed. Not proven pixel-identical to live gameplay."}
 (OUT/"verification.json").write_text(json.dumps(proof,indent=2,ensure_ascii=False))
 print("ORIGINAL_3D_EXPORTED",dest,len(result))
 print("SOURCE_BINDPOSE_AND_CONTROLLER_CHECKS_PASSED",json.dumps(proof,ensure_ascii=False)[:2500])
if __name__=="__main__":main()
