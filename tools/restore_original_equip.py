#!/usr/bin/env python3
"""Restore ORIGINAL draw/holster source clips, not a simulated weapon-swap pose.

Sources: original role.ab AnimatorController, original animationbundle.ab,
original MonoScript NawuqiState/ShouwuqiState, original stone axe prefab.
No changes to run/jump phases or weapon-switch runtime until user reviews images.
"""
import hashlib,importlib.util,json,struct
from pathlib import Path
import numpy as np,UnityPy
from restore_original_melee_run import decode_curves,ROOT,GAME
from export_original_stone_axe_3d import source_matrix
OUT=GAME/"importar_sacar_trocar_arma_original"
OUT.mkdir(parents=True,exist_ok=True)
ORIGINAL={
 "axe_holster":("Shouwuqi.dao_zhan_shouwuqi01","-292624572431209125","dao_zhan_shouwuqi01"),
 "axe_draw":("Nawuqi.dao_zhan_nawuqi01","-2420338730600833779","dao_zhan_nawuqi01"),
 "rifle_holster":("Shouwuqi.chongfengqiang_hengwo_zhan_right_shouwuqi01","5714370143701496654","chongfengqiang_hengwo_zhan_right_shouwuqi01"),
 "rifle_draw":("Nawuqi.chongfengqiang_hengwo_zhan_right_nawuqi01","463748819163163571","chongfengqiang_hengwo_zhan_right_nawuqi01"),
}
def verify_controller():
 obj=next(o for o in UnityPy.load(str(ROOT/"raw_assets/assets/role/role.ab")).objects
          if o.type.name=="AnimatorController" and o.read().m_Name=="man")
 tree=obj.read_typetree()
 tos={int(k):v for k,v in tree["m_TOS"]}
 refs={}
 for machine in tree["m_Controller"]["m_StateMachineArray"]:
  for state in machine["data"]["m_StateConstantArray"]:
   d=state["data"];name=tos.get(int(d["m_FullPathID"]),"")
   if not any(name.endswith("."+suffix) for suffix,_,_ in ORIGINAL.values()):continue
   ids=[]
   for bt in d["m_BlendTreeConstantArray"]:
    for child in bt["data"]["m_NodeArray"]:
     idx=int(child["data"]["m_ClipID"])
     if idx!=4294967295:ids.append(str(tree["m_AnimationClips"][idx]["m_PathID"]))
   refs[name]=ids
 for role,(suffix,id,n) in ORIGINAL.items():
  match=[v for k,v in refs.items() if k.endswith("."+suffix)]
  assert len(match)==1 and id in match[0],(role,match)
 return refs
def recover():
 asset=UnityPy.load(str(ROOT/"raw_assets/assets/role/animationbundle.ab"))
 objects={str(o.path_id):o for o in asset.objects if o.type.name=="AnimationClip"}
 target=GAME/"authentic_animations.npz"
 with np.load(target,allow_pickle=True) as a: d={k:a[k].copy() for k in a.files}
 names=[str(x) for x in d["zhan_stand01_bone_names"]]
 parents=json.loads(str(d["_bone_parent_json"][0]))
 reports={}
 for role,(_,id,name) in ORIGINAL.items():
  obj=objects[id];assert obj.read().m_Name==name
  tr,rot,frames,fps,angles,scale=decode_curves(obj.read_typetree(),names,parents)
  assert frames==14 and fps==30 and np.isfinite(tr).all() and np.isfinite(rot).all()
  assert np.max(np.abs(np.linalg.norm(rot,axis=2)-1))<0.0002
  d[name+"_bone_names"]=np.asarray(names)
  d[name+"_trans"]=tr
  d[name+"_rots"]=rot
  d[name+"_n_frames"]=np.array([frames],np.int32)
  d[name+"_fps"]=np.array([fps],np.float32)
  arm_motion={}
  for bone in ("Bip001 R UpperArm","Bip001 R Forearm","Bip001 R Hand",
               "Bip001 L UpperArm","Bip001 L Forearm"):
   if bone not in names:continue
   q=rot[names.index(bone)]
   rad=2*np.rad2deg(np.arccos(np.clip(np.abs(np.sum(q*q[0],axis=1)),0,1)))
   arm_motion[bone]=round(float(rad.max()),2)
  reports[role]={"name":name,"path_id":id,"frames":frames,"fps":fps,
                 "arm_motion_deg":arm_motion,"scale_curve_peak":scale}
  print("EXTRACTED_SOURCE_CLIP",role,json.dumps(reports[role]))
 assert any(max(v["arm_motion_deg"].values(),default=0)>5 for v in reports.values()),"No articulated movements detected"
 tmp=target.with_name("authentic_animations.equip_staging.npz")
 np.savez_compressed(tmp,**d)
 with np.load(tmp,allow_pickle=True) as f:
  assert all(x[2]+"_rots" in f for x in ORIGINAL.values())
  count=sum(k.endswith("_rots") for k in f.files)
  assert count==34, count
 tmp.replace(target)
 return reports,count
def export_preview():
 sp=importlib.util.spec_from_file_location("gltf",ROOT/"tools/export_axe_hold_animation.py")
 exp=importlib.util.module_from_spec(sp);sp.loader.exec_module(exp)
 exp.CLIPS=["zhan_stand01"]+[x[2] for x in ORIGINAL.values()]
 exp.OUTPUT=OUT;exp.main()
 src=OUT/"original_axe_holding_candidates.glb";data=src.read_bytes()
 jlen=struct.unpack_from("<I",data,12)[0];gl=json.loads(data[20:20+jlen])
 o=20+jlen;length=struct.unpack_from("<I",data,o)[0];binary=data[o+8:o+8+length]
 nodes=gl["nodes"]
 ax=next(i for i,x in enumerate(nodes) if x.get("name")=="StoneAxe_wq_shifu")
 sock=next(i for i,x in enumerate(nodes) if x.get("name")=="wuqi_guadian")
 assert ax in nodes[sock]["children"]
 mat,_=source_matrix()
 nodes[ax]["matrix"]=mat.T.flatten().tolist()
 # Viewer default pose is first original axe draw frame; other clips selectable.
 clip=next(x for x in gl["animations"] if x["name"]=="dao_zhan_nawuqi01")
 for ch in clip["channels"]:
  acc=gl["accessors"][clip["samplers"][ch["sampler"]]["output"]]
  view=gl["bufferViews"][acc["bufferView"]]
  size=4 if acc["type"]=="VEC4" else 3
  off=view.get("byteOffset",0)+acc.get("byteOffset",0)
  v=np.frombuffer(binary,np.float32,count=size,offset=off)
  nodes[ch["target"]["node"]][ch["target"]["path"]]=v.astype(float).tolist()
 gl["asset"]["extras"]={
  "original":"Unity source AnimationClip data and stone axe prefab",
  "caution":"M4A1 animation tracks in this preview use the stone axe prop for reference; do not treat as a correct gun mesh. For axe draw/holster the actual stone axe mesh and socket are used.",
  "visual_validation":"not yet tested in live game"}
 j=json.dumps(gl,separators=(",",":"),ensure_ascii=False).encode()
 j+=b" "*((-len(j))%4)
 binary+=b"\0"*((-len(binary))%4)
 output=struct.pack("<III",0x46546c67,2,12+8+len(j)+8+len(binary))
 output+=struct.pack("<II",len(j),0x4e4f534a)+j
 output+=struct.pack("<II",len(binary),0x004e4942)+binary
 dest=OUT/"sacar_guardar_original_machado_3d.glb";dest.write_bytes(output)
 src.unlink()
 return dest,hashlib.sha256(output).hexdigest()
def main():
 refs=verify_controller()
 reports,count=recover()
 path,digest=export_preview()
 report={"stage":"3: draw/holster and weapon switch","source_controller":"role.ab",
  "source_anim":"animationbundle.ab","source_code":"NawuqiState.cs / ShouwuqiState.cs",
  "state_clips":refs,"clips":reports,"game_npz_total":count,"glb_name":path.name,
  "glb_sha256":digest,
  "runtime_status":"Only source curves added to game NPZ. Runtime button/transition logic NOT yet changed; awaiting visual review.",
  "original_event_times":{"holster_weapon_slot_seconds":0.4,"draw_weapon_slot_seconds":0.2}}
 (OUT/"relatorio.json").write_text(json.dumps(report,indent=2,ensure_ascii=False))
 (OUT/"README.md").write_text("""# Etapa 3: Sacar e guardar arma (fonte original)

O arquivo \`sacar_guardar_original_machado_3d.glb\` contém o personagem com
o machado de pedra e as curvas originais dos clipes de empunhar e guardar.

As **quatro animações de 14 quadros / 30 FPS** estão agora no
\`authentic_animations.npz\` do jogo:
- \`dao_zhan_shouwuqi01\` (guardar machado)
- \`dao_zhan_nawuqi01\` (sacar machado)
- \`chongfengqiang_hengwo_zhan_right_shouwuqi01\` (guardar fuzil)
- \`chongfengqiang_hengwo_zhan_right_nawuqi01\` (sacar fuzil)

O GLB é só para visualizar: a malha de arma incluída é o machado.
**Não** use o GLB como prévia da arma M4A1, embora seus clipes de
movimento estejam presentes. Os estados/IDs vêm do Unity \`role.ab\`.
O código original troca a arma da mão em 0,4s guardando e 0,2s sacando.

Ainda não foi ativada a troca animada no jogo Python; aguardamos
avaliação visual do saque/guarda antes de alterar a jogabilidade.
""",encoding="utf8")
 print("ORIGINAL_EQUIP_4_CLIPS_RESTORED",count,"GLB",path)
if __name__=="__main__":main()
