#!/usr/bin/env python3
"""Package and test actual melee sprint from source Unity controller, as an incremental change.
No jump or equip states are modified here.
"""
import ast,importlib.util,json,struct,hashlib
from pathlib import Path
import numpy as np, UnityPy
from export_original_stone_axe_3d import ROOT, source_matrix
G=ROOT/"last_island_recreated_game"
PACK=G/"importar_corrida_original"
PACK.mkdir(exist_ok=True)
ID="-2537489020083551270"
STATE="FullBody Layer.HoldJinzhanWeapon.dao_zhan_rush_front01"
CLIP="dao_zhan_rush_front01"

def controller_sources():
 e=UnityPy.load(str(ROOT/"raw_assets/assets/role/role.ab"))
 c=next(o.read_typetree() for o in e.objects if o.type.name=="AnimatorController" and o.read().m_Name=="man")
 tos={int(k):v for k,v in c["m_TOS"]}
 paths=[]
 for ma in c["m_Controller"]["m_StateMachineArray"]:
  for state in ma["data"]["m_StateConstantArray"]:
   d=state["data"]
   if tos.get(int(d["m_FullPathID"]))!=STATE:continue
   for bt in d["m_BlendTreeConstantArray"]:
    for n in bt["data"]["m_NodeArray"]:
     i=int(n["data"]["m_ClipID"])
     if i!=4294967295:paths.append(str(c["m_AnimationClips"][i]["m_PathID"]))
 assert paths and set(paths)=={ID},f"Original full body sprint points to unexpected clips {paths}"
 a=UnityPy.load(str(ROOT/"raw_assets/assets/role/animationbundle.ab"))
 matching=[o for o in a.objects if str(o.path_id)==ID]
 assert len(matching)==1 and matching[0].read().m_Name==CLIP
 return paths

def test_actual_game_code():
 s=(G/"viewer3d.py").read_text(encoding="utf8")
 module=ast.parse(s)
 c=next(n for n in module.body if isinstance(n,ast.ClassDef) and n.name=="AuthenticAnimController")
 ns={}
 exec(compile(ast.Module(body=[c],type_ignores=[]),"viewer3d.py","exec"),ns)
 obj=ns["AuthenticAnimController"]()
 class Character:
  def __init__(self):self.samples=[]
  def set_pose_time(self,*v):self.samples.append(("direct",v))
  def set_pose_blended(self,*v):self.samples.append(("blend",v))
 dummy=Character()
 def update(moving,running):
  return obj.update(dummy,0.2,moving,running,True,False,0.0,0.0,local_fwd=1.0,local_right=0.0,equipped_slot=0)
 assert update(False,False)=="zhan_stand01"
 assert update(True,True)==CLIP, "Game still uses wrong running clip"
 assert obj.walk_time>0 and len(dummy.samples)>0
 assert update(True,False)=="zhan_run_front01", "Normal run should not silently change before next milestone"
 return {"idle":"zhan_stand01","forward_axe_sprint":CLIP,"normal_move_not_yet_changed":"zhan_run_front01","clip_samples":len(dummy.samples)}

def glb_json(buf):
 assert buf[:4]==b"glTF"
 jl=struct.unpack_from("<I",buf,12)[0]
 g=json.loads(buf[20:20+jl]);p=20+jl
 n=struct.unpack_from("<I",buf,p)[0];binary=buf[p+8:p+8+n]
 return g,binary

def main():
 refs=controller_sources()
 state_test=test_actual_game_code()
 with np.load(G/"authentic_animations.npz",allow_pickle=True) as a:
  n=int(a[CLIP+"_n_frames"][0]);fps=float(a[CLIP+"_fps"][0])
  assert n==18 and abs(fps-30)<0.01, (n,fps)
 spec=importlib.util.spec_from_file_location("original_export",ROOT/"tools/export_axe_hold_animation.py")
 exp=importlib.util.module_from_spec(spec);spec.loader.exec_module(exp)
 exp.CLIPS=["zhan_stand01",CLIP]
 exp.OUTPUT=PACK
 exp.main()
 p=PACK/"original_axe_holding_candidates.glb"
 gl,binary=glb_json(p.read_bytes())
 assert {v["name"] for v in gl["animations"]}=={"zhan_stand01",CLIP}
 matrix,bone=source_matrix()
 names=[n.get("name","") for n in gl["nodes"]]
 axe=names.index("StoneAxe_wq_shifu")
 socket=names.index("wuqi_guadian")
 assert axe in gl["nodes"][socket]["children"]
 gl["nodes"][axe].pop("translation",None)
 gl["nodes"][axe].pop("rotation",None)
 gl["nodes"][axe].pop("scale",None)
 gl["nodes"][axe]["matrix"]=matrix.T.flatten().tolist()
 # Present sample frame 0 from the actual sprint clip as the default scene pose.
 clip=next(a for a in gl["animations"] if a["name"]==CLIP)
 for channel in clip["channels"]:
  sam=clip["samplers"][channel["sampler"]];ac=gl["accessors"][sam["output"]]
  vw=gl["bufferViews"][ac["bufferView"]]
  sz=3 if ac["type"]=="VEC3" else 4
  off=vw.get("byteOffset",0)+ac.get("byteOffset",0)
  v=np.frombuffer(binary,dtype=np.float32,count=sz,offset=off)
  gl["nodes"][channel["target"]["node"]][channel["target"]["path"]]=v.astype(float).tolist()
 gl["asset"]["generator"]="Original Unity source axe sprint; repacked for stand-alone GLB preview"
 gl["asset"]["extras"]={"clip":CLIP,"path_id":ID,"source":STATE,
       "limitation":"Original animation sampled from Unity, runtime IK/visual matching not yet tested."}
 j=json.dumps(gl,separators=(",",":"),ensure_ascii=False).encode()
 j+=b" "*((-len(j))%4)
 binary+=b"\0"*((-len(binary))%4)
 data=struct.pack("<III",0x46546C67,2,12+8+len(j)+8+len(binary))
 data+=struct.pack("<II",len(j),0x4e4f534a)+j
 data+=struct.pack("<II",len(binary),0x004e4942)+binary
 output=PACK/"corrida_original_machado_3d.glb"
 output.write_bytes(data);p.unlink()
 report={"name":"Corrida rápida original do machado", "result":"Installed corrected clip selection into game code; source-verified GLB packaged", "source":STATE,
         "source_clip":CLIP, "original_path_ids":refs, "frames":n,"fps":fps,"bone_count":64,"weapon_attachment":"wuqi_guadian",
         "game_code_test":state_test,
         "sha256":hashlib.sha256(data).hexdigest(),
         "limitations":"Screenshot is reconstructed from source-animated 3D mesh, not gameplay capture. Running animation visually unverified in game.",
         "next_stages":["Corrida normal frente/trás/lados: extract original zhan_run_front02 separately","Pulo","Sacar/trocar arma"]}
 (PACK/"relatorio.json").write_text(json.dumps(report,ensure_ascii=False,indent=2))
 (PACK/"README.md").write_text("""# Etapa 1 — Corrida rápida com machado (extraída do original)

- **Integrado no código do jogo:** \`last_island_recreated_game/viewer3d.py\`
  seleciona \`dao_zhan_rush_front01\` quando o jogador segura machado,
  corre para frente e segura Shift.
- **Arquivo 3D:** \`corrida_original_machado_3d.glb\` inclui a animação
  original de 18 quadros a 30 FPS e o machado alinhado ao
  prefab original \`wq_shifu.ab\`.
- **Estado fonte:** \`FullBody Layer.HoldJinzhanWeapon.dao_zhan_rush_front01\`.
- **Ainda não feito:** a corrida normal \`zhan_run_front02\`, pulo e
  a troca/sacar arma. Não incluir essas etapas sem aprovação da imagem.

O jogo Python não lê GLB diretamente; utiliza \`authentic_animations.npz\`
e \`skinned_character.py\`. O GLB é para inspecionar o resultado e
permitir integração com outros motores. Testes unitários da escolha da
animação e validação dos AssetBundles passaram; validação visual
durante execução do jogo ainda está pendente.
""",encoding="utf8")
 print("RUNNING_SOURCE_STATE",STATE,"PATHID",ID)
 print("GAME_ANIM_TEST_PASS",state_test)
 print("ORIGINAL_3D_GLTF_READY",output,output.stat().st_size)
 print("GAME_SPRINT_18_FRAMES_30FPS_PASS")
if __name__=="__main__":main()
