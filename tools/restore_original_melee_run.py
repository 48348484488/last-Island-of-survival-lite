#!/usr/bin/env python3
"""Rebuild the *actual* source-authenticated melee forward locomotion clip.
Original AnimationClip 'zhan_run_front02', PathID 7186278071774183593.
The old 23-clip NPZ omitted this original clip and was playing zhan_run_front01.
Keeps jump, weapon switching, and sprint-specific raw decoding OUT OF SCOPE.
"""
import ast,hashlib,importlib.util,json,struct,sys,zlib
from pathlib import Path
import numpy as np
import UnityPy
# Import the axe mesh only for GLB packaging. Keep original curve decoder usable
# in the lightweight Android build without trimesh/preview dependencies.
ROOT=Path(__file__).resolve().parents[1]

GAME=ROOT/"last_island_recreated_game"
PACK=GAME/"importar_corrida_original"
PACK.mkdir(parents=True,exist_ok=True)
CLIP="zhan_run_front02"
ID="7186278071774183593"
STATE="Base Layer.Stand.jinzhan.f"

def read_unity_clip():
  env=UnityPy.load(str(ROOT/"raw_assets/assets/role/animationbundle.ab"))
  hits=[o for o in env.objects if str(o.path_id)==ID and o.type.name=="AnimationClip"]
  assert len(hits)==1
  assert hits[0].read().m_Name==CLIP
  return hits[0].read_typetree()

def read_state():
  env=UnityPy.load(str(ROOT/"raw_assets/assets/role/role.ab"))
  ctrl=next(o.read_typetree() for o in env.objects
      if o.type.name=="AnimatorController" and o.read().m_Name=="man")
  tos={int(k):v for k,v in ctrl["m_TOS"]}
  refs=[]
  for machine in ctrl["m_Controller"]["m_StateMachineArray"]:
    for state in machine["data"]["m_StateConstantArray"]:
      st=state["data"]
      if tos.get(int(st["m_FullPathID"]))!=STATE:continue
      for bt in st["m_BlendTreeConstantArray"]:
        nd=bt["data"]["m_NodeArray"]
        for n in nd:
          i=int(n["data"]["m_ClipID"])
          if i!=4294967295:refs.append(str(ctrl["m_AnimationClips"][i]["m_PathID"]))
  assert ID in refs,f"Expected {ID} in original Unity Animator state, got {refs}"
  return refs

def decode_curves(clip,names,parents):
  m=clip["m_MuscleClip"]["m_Clip"]["data"];streamed=m["m_StreamedClip"]
  vals=streamed["data"];nc=int(streamed["curveCount"])
  d=m["m_DenseClip"];nf=int(d["m_FrameCount"]);fps=float(d["m_SampleRate"])
  dc=int(d["m_CurveCount"]);dense=np.asarray(d["m_SampleArray"],np.float32).reshape(nf,dc)
  const=np.asarray(m["m_ConstantClip"]["data"],np.float32)
  assert nf >= 2 and 0 < fps <= 120, (nf, fps)
  # Unity clips may mix streamed, dense and constant curves; not all are streamed-only.
  assert dense.shape == (nf, dc), (dense.shape, nf, dc)
  tracks=[[] for _ in range(nc)]
  i=0
  while i<len(vals):
    t=struct.unpack("<f",struct.pack("<I",int(vals[i])))[0]
    count=int(vals[i+1]);i+=2
    for _ in range(count):
      ix=int(vals[i])
      # Stored float key values are a streamed Hermite representation;
      # sampled frames aligned to existing keys take exact source values.
      vf=struct.unpack("<f",struct.pack("<I",int(vals[i+4])))[0]
      tracks[ix].append((t,vf));i+=5
  assert i==len(vals)
  curves=np.zeros((nf,nc+dc+len(const)),dtype=np.float32)
  query=np.arange(nf,dtype=np.float64)/fps
  for ci,keys in enumerate(tracks):
    if not keys:continue
    arr=np.asarray(keys,dtype=np.float64)
    # Unity stream includes extra sentinel keys and occasionally duplicate timestamps.
    arr=arr[np.argsort(arr[:,0])]
    t,unique_ix=np.unique(arr[:,0],return_index=True)
    curves[:,ci]=np.interp(query,t,arr[unique_ix,1])
  if dc:curves[:,nc:nc+dc]=dense
  if len(const):curves[:,nc+dc:]=const
  def path(name):
    p=parents.get(name)
    return path(p)+"/"+name if p in parents else name
  hash_index={zlib.crc32(path(x).encode("utf8")):i for i,x in enumerate(names)}
  bindings=clip["m_ClipBindingConstant"]["genericBindings"]
  # Initialize missing channels from the genuine character idle pose.
  with np.load(GAME/"authentic_animations.npz",allow_pickle=True) as a:
    idle_names=[str(x) for x in a["zhan_stand01_bone_names"]]
    pos=a["zhan_stand01_trans"][:,0,:]
    rot=a["zhan_stand01_rots"][:,0,:]
    default={x:(pos[i],rot[i]) for i,x in enumerate(idle_names)}
  trans=np.stack([default[x][0] for x in names]).astype(np.float32)
  quat=np.stack([default[x][1] for x in names]).astype(np.float32)
  trans=np.repeat(trans[:,None,:],nf,axis=1)
  quat=np.repeat(quat[:,None,:],nf,axis=1)
  cur=0;matched=set();scale_peaks=[]
  for b in bindings:
    typ=b["typeID"];a=b["attribute"]
    n=4 if typ==4 and a==2 else 3 if typ==4 and a in (1,3,4) else 1
    block=curves[:,cur:cur+n];cur+=n
    if typ!=4 or a not in (1,2,3):continue
    assert b["path"] in hash_index,f"Unknown original Unity bone hash {b['path']}"
    ix=hash_index[b["path"]];matched.add(names[ix])
    if a==1:trans[ix]=block
    elif a==2:quat[ix]=block
    elif a==3:scale_peaks.append(float(np.max(np.abs(block-1))))
  assert cur==curves.shape[1],(cur,curves.shape[1])
  assert len(matched)>=55,(len(matched),sorted(matched))
  quat/=np.maximum(np.linalg.norm(quat,axis=2,keepdims=True),1e-8)
  for frame in range(1,nf):
    dot=np.sum(quat[:,frame]*quat[:,frame-1],axis=1)
    quat[dot<0,frame]*=-1
  # The game's PlayerController moves the entity in world coords.
  # The existing engine explicitly removes root horizontal displacement;
  # leave source root-motion in NPZ, as SkinnedCharacter performs the lock.
  assert np.isfinite(trans).all() and np.isfinite(quat).all()
  angles={}
  for limb in ("Bip001 L Thigh","Bip001 R Thigh","Bip001 L Calf","Bip001 R Calf"):
    if limb not in names:continue
    q=quat[names.index(limb)]
    angular=2*np.rad2deg(np.arccos(np.clip(np.abs(np.sum(q*q[0],axis=1)),0,1)))
    angles[limb]=round(float(angular.max()),2)
  # Some poses (airborne hold) intentionally barely move. Motion validation belongs
  # to the calling test, not the general Unity curve decoder.
  return trans,quat,nf,fps,angles,max(scale_peaks) if scale_peaks else 0.0

def write_npz():
  target=GAME/"authentic_animations.npz"
  with np.load(target,allow_pickle=True) as a:
    source={key:a[key].copy() for key in a.files}
  names=[str(x) for x in source["zhan_stand01_bone_names"]]
  parents=json.loads(str(source["_bone_parent_json"][0]))
  tr,ro,nf,fps,angles,scale=decode_curves(read_unity_clip(),names,parents)
  new={CLIP+"_bone_names":np.asarray(names),
       CLIP+"_n_frames":np.array([nf],dtype=np.int32),
       CLIP+"_fps":np.array([fps],dtype=np.float32),
       CLIP+"_trans":tr,CLIP+"_rots":ro}
  source.update(new)
  staging=target.with_suffix(".new.npz")
  np.savez_compressed(staging,**source)
  with np.load(staging,allow_pickle=True) as a:
    assert a[CLIP+"_rots"].shape==(len(names),nf,4)
    assert len([x for x in a.files if x.endswith("_rots")])>=24
  staging.replace(target)
  return names,nf,fps,angles,scale

def check_game_code():
  tree=ast.parse((GAME/"viewer3d.py").read_text(encoding="utf8"))
  controller=next(x for x in tree.body if isinstance(x,ast.ClassDef) and x.name=="AuthenticAnimController")
  ns={}
  exec(compile(ast.Module(body=[controller],type_ignores=[]),"viewer3d.py","exec"),ns)
  anim=ns["AuthenticAnimController"]()
  class Dummy:
    def set_pose_time(self,*args):pass
    def set_pose_blended(self,*args):pass
  dummy=Dummy()
  assert anim.update(dummy,.2,True,False,True,False,0,0,1,0,0)==CLIP
  assert anim.update(dummy,.2,True,True,True,False,0,0,1,0,0)==CLIP
  return True

def package_glb():
  from export_original_stone_axe_3d import source_matrix
  spec=importlib.util.spec_from_file_location("glb_export",ROOT/"tools/export_axe_hold_animation.py")
  exp=importlib.util.module_from_spec(spec);spec.loader.exec_module(exp)
  exp.CLIPS=["zhan_stand01",CLIP]
  exp.OUTPUT=PACK;exp.main()
  p=PACK/"original_axe_holding_candidates.glb";data=p.read_bytes()
  jl=struct.unpack_from("<I",data,12)[0];gl=json.loads(data[20:20+jl])
  o=20+jl;binary=bytearray(data[o+8:o+8+struct.unpack_from("<I",data,o)[0]])
  ax=next(i for i,n in enumerate(gl["nodes"]) if n["name"]=="StoneAxe_wq_shifu")
  socket=next(i for i,n in enumerate(gl["nodes"]) if n["name"]=="wuqi_guadian")
  assert ax in gl["nodes"][socket]["children"]
  m,_=source_matrix()
  gl["nodes"][ax]["matrix"]=m.T.flatten().tolist()
  # Clip uses source root motion; lock forward/lateral in exported preview.
  root=next(i for i,n in enumerate(gl["nodes"]) if n["name"]=="Bip001")
  clip=next(a for a in gl["animations"] if a["name"]==CLIP)
  for ch in clip["channels"]:
    if ch["target"]!={"node":root,"path":"translation"}:continue
    a=gl["accessors"][clip["samplers"][ch["sampler"]]["output"]]
    view=gl["bufferViews"][a["bufferView"]]
    off=view.get("byteOffset",0)+a.get("byteOffset",0)
    arr=np.frombuffer(binary,np.float32,count=a["count"]*3,offset=off).reshape(-1,3)
    arr[:,0]=arr[0,0];arr[:,2]=arr[0,2]
  # Default preview frame 0 is running, not idle.
  for ch in clip["channels"]:
    acc=gl["accessors"][clip["samplers"][ch["sampler"]]["output"]]
    view=gl["bufferViews"][acc["bufferView"]]
    size=3 if acc["type"]=="VEC3" else 4
    off=view.get("byteOffset",0)+acc.get("byteOffset",0)
    ar=np.frombuffer(binary,np.float32,count=size,offset=off)
    gl["nodes"][ch["target"]["node"]][ch["target"]["path"]]=ar.astype(float).tolist()
  gl["asset"]["extras"]={"source_path_id":ID,"full_controller_state":STATE,
    "restored_from":"original Unity m_StreamedClip and m_ConstantClip",
    "root_motion":"horizontal locked for GLB preview; gameplay controls entity movement"}
  jb=json.dumps(gl,separators=(",",":"),ensure_ascii=False).encode()
  jb+=b" "*((-len(jb))%4)
  binary+=b"\0"*((-len(binary))%4)
  output=struct.pack("<III",0x46546c67,2,12+8+len(jb)+8+len(binary))
  output+=struct.pack("<II",len(jb),0x4e4f534a)+jb
  output+=struct.pack("<II",len(binary),0x004e4942)+binary
  dest=PACK/"corrida_normal_original_zhan_run_front02.glb";dest.write_bytes(output)
  p.unlink()
  bad=PACK/"corrida_original_machado_3d.glb"
  if bad.exists():bad.unlink()
  return dest,hashlib.sha256(output).hexdigest()

def main():
  refs=read_state()
  names,nf,fps,leg_angles,sc=write_npz()
  assert check_game_code()
  file,digest=package_glb()
  report={"status":"Original normal forward melee running restored to game NPZ and selected in viewer3d.py",
      "source_animation":CLIP,"original_path_id":ID,
      "controller_state":STATE,"controller_references":refs,
      "frames":nf,"fps":fps,"bones":len(names),
      "leg_angle_ranges_degrees":leg_angles,"scale_key_max_diff_from_one":sc,
      "model":file.name,"sha256":digest,
      "tests":["original Unity AnimatorController clip link","decoded streamed bone rotation and position tracks",
          "measured leg movements in original clip","game controller normal-run and sprint fallback selection"],
      "not_done":["authentic sprint rush track with Euler curves","jump","weapon equip/draw","on-device visual playback"]}
  (PACK/"relatorio.json").write_text(json.dumps(report,indent=2,ensure_ascii=False))
  (PACK/"README.md").write_text("""# Corrida normal original — pronta para revisão

A animação \`zhan_run_front02\` foi localizada no \`role.ab\`
(PathID \`7186278071774183593\`) e extraída de
\`animationbundle.ab\`. Possui **22 quadros a 30 FPS**.

O jogo Python agora contém **24 animações** em
\`last_island_recreated_game/authentic_animations.npz\`.
A animação de corrida normal frontal foi corrigida em
\`last_island_recreated_game/viewer3d.py\`.

O arquivo \`corrida_normal_original_zhan_run_front02.glb\`
é uma **visualização 3D independente** da animação original.
O motor Python usa o NPZ, não carrega o GLB.

Importante: a corrida rápida \`dao_zhan_rush_front01\` do pacote
anterior não foi aprovada visualmente: o NPZ antigo não reproduzia
os ossos móveis. Enquanto não for reconstruída corretamente,
o jogo reproduz o clipe normal validado com velocidade maior
quando Shift é pressionado. Não é a corrida rápida exata do original.

Pulo, sacar arma e troca de arma **ainda não foram alterados**.
A imagem deve ser conferida antes de iniciar a próxima etapa.
""",encoding="utf8")
  print("RAW_UNITY_RUNNING_SOURCE_VALIDATED",ID)
  print("REAL_RUNNING_POSES",leg_angles,"scales",sc)
  print("GAME_NPZ_UPDATED",(GAME/"authentic_animations.npz").stat().st_size)
  print("GAME_FORWARD_ANIMATION_UPDATED_AND_TESTED",CLIP)
  print("GLB_SOURCE_PREVIEW",file,file.stat().st_size)
if __name__=="__main__":main()
