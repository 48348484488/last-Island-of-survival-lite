#!/usr/bin/env python3
"""Restore original Last Island LITE melee jump: up, air, down, front/back.

All 6 source clips are selected by the actual original Unity 'man' AnimatorController.
No synthetic keyframes; mixed Streamed/Dense/Constant source curves.
The Python game's world-space jump height is still simulated by physics, as in source.
"""
import ast,hashlib,importlib.util,json,struct
from pathlib import Path
import numpy as np,UnityPy
from restore_original_melee_run import decode_curves,ROOT,GAME
from export_original_stone_axe_3d import source_matrix

CLIPS=[
 ("up","back","1718510388457734182","kongshou_zhan_jumpback_up02","Base Layer.Jump.kongshou"),
 ("up","front","371640738070943523","kongshou_zhan_jump_up02","Base Layer.Jump.kongshou"),
 ("air","back","2232039169493632919","kongshou_zhan_jumpback_idle02","Base Layer.Fall.kongshou"),
 ("air","front","-2093151076388574590","kongshou_zhan_jump_idle02","Base Layer.Fall.kongshou"),
 ("down","back","7879379039287101303","kongshou_zhan_jumpback_down02","Base Layer.Land.kongshou"),
 ("down","front","7736794628134902726","kongshou_zhan_jump_down02","Base Layer.Land.kongshou"),
]
OUT=GAME/"importar_pulo_original"
OUT.mkdir(parents=True,exist_ok=True)

def validate_controller():
    env=UnityPy.load(str(ROOT/"raw_assets/assets/role/role.ab"))
    ctrl=next(o.read_typetree() for o in env.objects
              if o.type.name=="AnimatorController" and o.read().m_Name=="man")
    t={int(k):v for k,v in ctrl["m_TOS"]}
    mapping={}
    for mach in ctrl["m_Controller"]["m_StateMachineArray"]:
        for state in mach["data"]["m_StateConstantArray"]:
            d=state["data"]
            nm=t.get(int(d["m_FullPathID"]),"")
            if nm not in {x[4] for x in CLIPS}: continue
            ids=[]
            for tr in d["m_BlendTreeConstantArray"]:
                for node in tr["data"]["m_NodeArray"]:
                    i=int(node["data"]["m_ClipID"])
                    if i != 4294967295:
                        ids.append(str(ctrl["m_AnimationClips"][i]["m_PathID"]))
            mapping[nm]=ids
    for _,_,pid,_,state in CLIPS: assert pid in mapping[state],(state,pid,mapping)
    return mapping

def recover_clips():
    asset=UnityPy.load(str(ROOT/"raw_assets/assets/role/animationbundle.ab"))
    objects={str(o.path_id):o for o in asset.objects if o.type.name=="AnimationClip"}
    target=GAME/"authentic_animations.npz"
    with np.load(target,allow_pickle=True) as f:
        all_assets={k:f[k].copy() for k in f.files}
    names=[str(x) for x in all_assets["zhan_stand01_bone_names"]]
    parent=json.loads(str(all_assets["_bone_parent_json"][0]))
    frames={}
    for phase,direction,pid,name,_ in CLIPS:
        o=objects[pid]
        assert o.read().m_Name==name,(pid,name)
        tr,rot,n,fps,angles,scale=decode_curves(o.read_typetree(),names,parent)
        assert n>=5 and fps==30 and np.isfinite(tr).all() and np.isfinite(rot).all()
        assert np.max(abs(np.linalg.norm(rot,axis=2)-1))<0.0002
        all_assets[name+"_bone_names"]=np.asarray(names)
        all_assets[name+"_n_frames"]=np.asarray([n],np.int32)
        all_assets[name+"_fps"]=np.asarray([fps],np.float32)
        all_assets[name+"_trans"]=tr
        all_assets[name+"_rots"]=rot
        frames[name]={"phase":phase,"direction":direction,"source_path_id":pid,
                      "frames":n,"fps":fps,"leg_motion_deg":angles,
                      "scale_key_peak":scale}
    stage=target.with_name("authentic_animations.jumptest.npz")
    np.savez_compressed(stage,**all_assets)
    with np.load(stage,allow_pickle=True) as a:
        assert all(name+"_rots" in a for _,_,_,name,_ in CLIPS)
        total=sum(k.endswith("_rots") for k in a.files)
        assert total>=30,total
    stage.replace(target)
    return frames,total

def check_controller_runtime():
    src=(GAME/"viewer3d.py").read_text(encoding="utf8")
    mod=ast.parse(src)
    node=next(x for x in mod.body if isinstance(x,ast.ClassDef) and x.name=="AuthenticAnimController")
    ns={}
    exec(compile(ast.Module(body=[node],type_ignores=[]),"<game-animation-controller>","exec"),ns)
    class Dummy:
        def set_pose_blended(self,*args):pass
        def set_pose_time(self,*args):pass
    actor=Dummy()
    a=ns["AuthenticAnimController"]()
    def run(dt,grounded,jump_t,direction=1.0):
        return a.update(actor,dt,False,False,grounded,False,0.0,jump_t,
                        local_fwd=direction,local_right=0.0,equipped_slot=0)
    forward=[run(.016,True,0.0),run(.10,False,.10),run(.10,False,.67),run(.10,True,.72)]
    assert forward==["zhan_stand01","kongshou_zhan_jump_up02",
                     "kongshou_zhan_jump_idle02","kongshou_zhan_jump_down02"],forward
    b=ns["AuthenticAnimController"]()
    def runback(dt,grounded,jump_t):
        return b.update(actor,dt,False,False,grounded,False,0.0,jump_t,
                        local_fwd=-1.0,local_right=0.0,equipped_slot=0)
    back=[runback(.016,True,0),runback(.10,False,.10),runback(.10,False,.55),runback(.10,True,.6)]
    assert back==["zhan_stand01","kongshou_zhan_jumpback_up02",
                  "kongshou_zhan_jumpback_idle02","kongshou_zhan_jumpback_down02"],back
    return {"forward_phase_sequence":forward,"backward_phase_sequence":back}

def package_glb():
    spec=importlib.util.spec_from_file_location("legacy_gltf",ROOT/"tools/export_axe_hold_animation.py")
    exp=importlib.util.module_from_spec(spec);spec.loader.exec_module(exp)
    exp.CLIPS=["zhan_stand01"]+[x[3] for x in CLIPS]
    exp.OUTPUT=OUT
    exp.main()
    p=OUT/"original_axe_holding_candidates.glb"
    blob=p.read_bytes()
    nj=struct.unpack_from("<I",blob,12)[0]
    gl=json.loads(blob[20:20+nj])
    off=20+nj
    nb=struct.unpack_from("<I",blob,off)[0]
    binary=blob[off+8:off+8+nb]
    nodes=gl["nodes"]
    ax=next(i for i,n in enumerate(nodes) if n.get("name")=="StoneAxe_wq_shifu")
    socket=next(i for i,n in enumerate(nodes) if n.get("name")=="wuqi_guadian")
    assert ax in nodes[socket]["children"]
    mat,_=source_matrix()
    nodes[ax]["matrix"]=mat.T.flatten().tolist()
    # Set default GLB stance to first source jump frame for non-animating viewers.
    clip=next(x for x in gl["animations"] if x["name"]=="kongshou_zhan_jump_up02")
    for ch in clip["channels"]:
        sm=clip["samplers"][ch["sampler"]];ac=gl["accessors"][sm["output"]]
        v=gl["bufferViews"][ac["bufferView"]]
        size=3 if ac["type"]=="VEC3" else 4
        pos=v.get("byteOffset",0)+ac.get("byteOffset",0)
        arr=np.frombuffer(binary,np.float32,count=size,offset=pos)
        nodes[ch["target"]["node"]][ch["target"]["path"]]=arr.astype(float).tolist()
    gl["asset"]["generator"]="Reconstructed direct Unity source animation curves; original jump/fall/land clips"
    gl["asset"]["extras"]={
        "original_bundles":["role/role.ab","role/animationbundle.ab","model/wq_shifu.ab"],
        "warning":"Source-based 3D reconstruction; engine IK and humanoid controller transitions not fully reproduced."}
    j=json.dumps(gl,ensure_ascii=False,separators=(",",":")).encode()
    j+=b" "*((-len(j))%4)
    binary+=b"\0"*((-len(binary))%4)
    data=struct.pack("<III",0x46546C67,2,12+8+len(j)+8+len(binary))
    data+=struct.pack("<II",len(j),0x4E4F534A)+j
    data+=struct.pack("<II",len(binary),0x004E4942)+binary
    dest=OUT/"pulo_original_machado_3d.glb"
    dest.write_bytes(data)
    p.unlink()
    assert {a["name"] for a in gl["animations"]}=={"zhan_stand01"}|{x[3] for x in CLIPS}
    return dest,hashlib.sha256(data).hexdigest()

def main():
    refs=validate_controller()
    frames,total=recover_clips()
    runtime=check_controller_runtime()
    glb,digest=package_glb()
    report={"source":"Last Island of Survival LITE 7.3 Unity AssetBundles","status":"Jump, Fall, Land installed into recreated game",
        "controller_states":refs,"clips":frames,"npz_total_clips":total,
        "runtime_test":runtime,"model":glb.name,"sha256":digest,
        "status_limitations":"No verified visual screenshot from running Unity APK; source curves were resampled by decoder."}
    (OUT/"relatorio.json").write_text(json.dumps(report,ensure_ascii=False,indent=2))
    (OUT/"README.md").write_text("""# Pulo original (3 fases) — versão para revisão

Este pacote contém os **6 clipes originais do Unity** (pulo para frente e para trás):
- **Impulsão:** \`kongshou_zhan_jump_up02\` (19 frames) / \`kongshou_zhan_jumpback_up02\` (15 frames)
- **No ar:** \`kongshou_zhan_jump_idle02\` / \`kongshou_zhan_jumpback_idle02\` (22 frames)
- **Aterrissagem:** \`kongshou_zhan_jump_down02\` (11 frames) / \`kongshou_zhan_jumpback_down02\` (19 frames)

Os clipes foram extraídos de \`raw_assets/assets/role/animationbundle.ab\` usando
os PathIDs referenciados no \`role.ab\` do personagem masculino.

**Integração:** \`viewer3d.py\` agora seleciona as 3 fases ao saltar com machado;
\`authentic_animations.npz\` contém todos os 6 clipes novos. O jogo Python usa
o NPZ. O GLB **é independente, para inspecionar ou importar em outro motor**.

A física do salto continua no código do jogo. A seleção da fase aérea e da
aterrissagem é aproximada ao fluxo original: não replica todo o motor Unity/IK.
Ainda não validado visualmente durante uma partida. Não alterei sacar/trocar arma.
""",encoding="utf8")
    print("ORIGINAL_JUMP_IN_GAME",json.dumps(runtime,ensure_ascii=False))
    print("SOURCE_CLIPS",json.dumps(frames,ensure_ascii=False))
    print("NPZ_TOTAL_CLIPS",total)
    print("SOURCE_JUMP_GLB_READY",glb,glb.stat().st_size)
if __name__=="__main__":main()
