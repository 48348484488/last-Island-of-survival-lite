#!/usr/bin/env python3
"""Export the already-extracted original Unity animation curves + skinned hero + axe
to glTF 2.0 (.glb). No synthetic keyframes. All tracks from authentic_animations.npz.
Hold poses are CANDIDATES: the original AnimatorController has not been recovered.
"""
import json, struct, pathlib, numpy as np
R=pathlib.Path(__file__).resolve().parents[1]
G=R/"last_island_recreated_game"
OUTPUT=R/"axe_hold_extracted"
OUTPUT.mkdir(exist_ok=True)
CLIPS=["tongyong_bishou_zhan_stand01","shuangshouchui_zhan_stand01",
       "zhan_stand01","dao_zhan_nawuqi01","dao_zhan_shouwuqi01",
       "fu_zhan_attack_01"]
# Original GL socket calibration, from skinned_character.py (not estimated here)
SOCKET=np.array([[-.3420,.9397,0,-.0290],
                 [0,0,-1,.0006],
                 [-.9397,-.3420,0,-.1261],
                 [0,0,0,1]],dtype=np.float32)
# Inverse of original mesh-side Unity-to-GL transform.
GL2U=np.array([[1,0,0,0],[0,0,-1,0],[0,1,0,0],[0,0,0,1]],dtype=np.float32)

class Builder:
    def __init__(self):
        self.data=bytearray()
        self.gl={"asset":{"version":"2.0","generator":"Original Unity NPZ axe-held animation exporter"},
                 "scene":0,"scenes":[{"nodes":[]}],"nodes":[],
                 "buffers":[{"byteLength":0}],"bufferViews":[],"accessors":[],
                 "materials":[],"meshes":[],"skins":[],"animations":[],
                 "images":[],"textures":[],"samplers":[{"magFilter":9729,"minFilter":9987,
                                                        "wrapS":10497,"wrapT":10497}]}
    def view(self, raw, target=None):
        while len(self.data)%4:self.data.append(0)
        off=len(self.data)
        self.data.extend(raw)
        item={"buffer":0,"byteOffset":off,"byteLength":len(raw)}
        if target:item["target"]=target
        i=len(self.gl["bufferViews"]);self.gl["bufferViews"].append(item);return i
    def acc(self, arr, component=5126, kind="SCALAR", target=None, bounds=False):
        dtype={5126:np.float32,5123:np.uint16,5125:np.uint32}[component]
        ar=np.asarray(arr,dtype=dtype)
        if kind=="MAT4":ar=ar.reshape(-1,16)
        elif kind!="SCALAR":ar=ar.reshape(-1,{"VEC2":2,"VEC3":3,"VEC4":4}[kind])
        else:ar=ar.reshape(-1)
        ar=np.ascontiguousarray(ar)
        vi=self.view(ar.tobytes(),target)
        item={"bufferView":vi,"componentType":component,"count":len(ar),"type":kind}
        if bounds and len(ar):
            if ar.ndim>1:
                item["min"]=ar.min(axis=0).astype(float).tolist()
                item["max"]=ar.max(axis=0).astype(float).tolist()
            else:
                item["min"]=[float(ar.min())];item["max"]=[float(ar.max())]
        i=len(self.gl["accessors"]);self.gl["accessors"].append(item);return i
    def texture(self, path):
        if not path.exists():return None
        vi=self.view(path.read_bytes())
        idx=len(self.gl["images"]);self.gl["images"].append({"bufferView":vi,"mimeType":"image/png"})
        ti=len(self.gl["textures"]);self.gl["textures"].append({"sampler":0,"source":idx});return ti
    def material(self, tex, name):
        mat={"name":name,"doubleSided":True,"pbrMetallicRoughness":
             {"baseColorFactor":[1,1,1,1],"metallicFactor":0.0,"roughnessFactor":1.0}}
        if tex is not None:mat["pbrMetallicRoughness"]["baseColorTexture"]={"index":tex}
        i=len(self.gl["materials"]);self.gl["materials"].append(mat);return i
    def mesh(self, pos, normals, uvs, tris, material, joint=None, weights=None):
        attrs={"POSITION":self.acc(pos,kind="VEC3",target=34962,bounds=True)}
        if normals is not None and len(normals)==len(pos):
            attrs["NORMAL"]=self.acc(normals,kind="VEC3",target=34962)
        if uvs is not None and len(uvs)==len(pos):
            attrs["TEXCOORD_0"]=self.acc(uvs,kind="VEC2",target=34962)
        if joint is not None:
            attrs["JOINTS_0"]=self.acc(joint,5123,"VEC4",target=34962)
            attrs["WEIGHTS_0"]=self.acc(weights,5126,"VEC4",target=34962)
        prim={"attributes":attrs,"indices":self.acc(tris,5125,"SCALAR",target=34963),
              "mode":4,"material":material}
        index=len(self.gl["meshes"]);self.gl["meshes"].append({"primitives":[prim]})
        return index
    def finish(self, path):
        self.gl["buffers"][0]["byteLength"]=len(self.data)
        js=json.dumps(self.gl,separators=(",",":"),ensure_ascii=False).encode()
        js+=b" " * ((4-len(js)%4)%4)
        chunk=bytes(self.data);chunk+=b"\x00" * ((4-len(chunk)%4)%4)
        total=12+8+len(js)+8+len(chunk)
        with path.open("wb") as f:
            f.write(struct.pack("<4sII",b"glTF",2,total))
            f.write(struct.pack("<I4s",len(js),b"JSON"));f.write(js)
            f.write(struct.pack("<I4s",len(chunk),b"BIN\x00"));f.write(chunk)

def obj_mesh(path):
    positions=[];normals=[];uvs=[];indices=[];vers=[];nors=[];tex=[]
    unique={}
    for line in path.read_text(encoding="utf-8",errors="ignore").splitlines():
        t=line.split()
        if not t:continue
        if t[0]=="v":vers.append([float(x) for x in t[1:4]])
        elif t[0]=="vn":nors.append([float(x) for x in t[1:4]])
        elif t[0]=="vt":tex.append([float(x) for x in t[1:3]])
        elif t[0]=="f":
            face=[]
            for s in t[1:]:
                ix=s.split("/")
                def ix0(v,length):
                    i=int(v)
                    return i-1 if i>0 else length+i
                vi=ix0(ix[0],len(vers))
                ti=ix0(ix[1],len(tex)) if len(ix)>1 and ix[1] else -1
                ni=ix0(ix[2],len(nors)) if len(ix)>2 and ix[2] else -1
                key=(vi,ti,ni)
                if key not in unique:
                    unique[key]=len(positions)
                    positions.append(vers[vi])
                    uv=tex[ti] if ti>=0 else (0,0)
                    uvs.append([uv[0],1-uv[1]])
                    normals.append(nors[ni] if ni>=0 else [0,1,0])
                face.append(unique[key])
            for i in range(1,len(face)-1):indices.extend((face[0],face[i],face[i+1]))
    return positions,normals,uvs,indices

def main():
    b=Builder()
    with np.load(G/"authentic_animations.npz",allow_pickle=True) as a, \
         np.load(G/"master_character.npz",allow_pickle=True) as c:
        parent=json.loads(str(a["_bone_parent_json"][0]))
        # Every known bone is preserved, including the weapon socket.
        bones=list(parent)
        selected=[name for name in CLIPS if name+"_rots" in a]
        if not selected:raise ValueError("No source clips from candidate list in NPZ")
        first=selected[0]
        clip_bones=[str(n) for n in a[first+"_bone_names"]]
        # Cover bones used by skin even if they lack animation tracks.
        for meshname in ["pants","torso","skin","shoes","gloves","head","hair"]:
            for n in c[meshname+"_bone_names"]:
                n=str(n)
                if n not in bones:bones.append(n)
        for n in clip_bones:
            if n not in bones:bones.append(n)
        index={}
        for n in bones:
            index[n]=len(b.gl["nodes"])
            b.gl["nodes"].append({"name":n})
        for n in bones:
            p=parent.get(n)
            if p in index:
                b.gl["nodes"][index[p]].setdefault("children",[]).append(index[n])
            else:b.gl["scenes"][0]["nodes"].append(index[n])
        # Seed a true source pose, not an invented T-pose.
        trans=a[first+"_trans"];rots=a[first+"_rots"]
        for i,n in enumerate(clip_bones):
            b.gl["nodes"][index[n]]["translation"]=trans[i,0,:].astype(float).tolist()
            b.gl["nodes"][index[n]]["rotation"]=rots[i,0,:].astype(float).tolist()
        matfiles={
           "pants":"man_trousers_chushijuese_01_ba.png",
           "torso":"man_clothes_chushijuese_01_ba.png",
           "skin":"man_head_chushijuese_01_ba.png",
           "shoes":"man_shoes_chushijuese_01_ba.png",
           "gloves":"man_head_chushijuese_01_ba.png",
           "head":"man_head_chushijuese_01_ba.png",
           "hair":"man_hair_chushijuese_01_ba.png"}
        for m in ["pants","torso","skin","shoes","gloves","head","hair"]:
            verts=c[m+"_verts"];nr=c[m+"_norms"];uv=c[m+"_uvs"]
            tri=c[m+"_tris"].flatten()
            weights=np.asarray(c[m+"_weights"],dtype=np.float32)
            jidx=np.asarray(c[m+"_indices"],dtype=np.int32)
            names=[str(v) for v in c[m+"_bone_names"]]
            bp=np.asarray(c[m+"_bind_poses"],dtype=np.float32)
            if len(weights.shape)==1 or weights.shape[1]==0:
                weights=np.ones((len(verts),1),dtype=np.float32)
                jidx=np.zeros((len(verts),1),dtype=np.int32)
            if jidx.shape[1]<4:
                jidx=np.pad(jidx,((0,0),(0,4-jidx.shape[1])))
                weights=np.pad(weights,((0,0),(0,4-weights.shape[1])))
            jidx=jidx[:,:4];weights=weights[:,:4]
            sums=weights.sum(axis=1,keepdims=True)
            weights=weights/np.where(sums>0,sums,1.0)
            if np.any(jidx<0) or np.any(jidx>=len(names)):
                raise ValueError(m+" contains invalid skin bone indexes")
            tex=b.texture(G/"assets"/matfiles[m])
            material=b.material(tex,m)
            mesh=b.mesh(verts,nr,uv,tri,material,jidx,weights)
            # glTF column-major inverse-bind matrices; identical to rendering pipeline.
            invbind=np.stack([v@GL2U for v in bp]).astype(np.float32)
            invbind=np.transpose(invbind,(0,2,1)).reshape(-1,16)
            ib=b.acc(invbind,5126,"MAT4")
            si=len(b.gl["skins"])
            b.gl["skins"].append({"joints":[index[x] for x in names],
                                 "inverseBindMatrices":ib,"name":m+"_skin"})
            ni=len(b.gl["nodes"])
            b.gl["nodes"].append({"name":m,"mesh":mesh,"skin":si})
            b.gl["scenes"][0]["nodes"].append(ni)
        axefile=G/"assets"/"wq_shifu.obj"
        if not axefile.exists():raise FileNotFoundError(axefile)
        pos,norm,uv,tri=obj_mesh(axefile)
        axe_tex=b.texture(G/"assets"/"wq_shifu_ba.png")
        axe_mat=b.material(axe_tex,"Original stone axe")
        axe_mesh=b.mesh(pos,norm,uv,tri,axe_mat)
        axe_node=len(b.gl["nodes"])
        b.gl["nodes"].append({"name":"StoneAxe_wq_shifu","mesh":axe_mesh,
                              "matrix":SOCKET.T.flatten().astype(float).tolist()})
        if "wuqi_guadian" not in index:
            raise ValueError("Original right-hand weapon socket missing")
        b.gl["nodes"][index["wuqi_guadian"]].setdefault("children",[]).append(axe_node)
        clip_report=[]
        for name in selected:
            names=[str(n) for n in a[name+"_bone_names"]]
            tr=np.asarray(a[name+"_trans"],dtype=np.float32)
            qs=np.asarray(a[name+"_rots"],dtype=np.float32)
            fps=float(a[name+"_fps"][0]);nframes=int(a[name+"_n_frames"][0])
            if tr.shape!=(len(names),nframes,3) or qs.shape!=(len(names),nframes,4):
                raise ValueError("Malformed transform arrays for "+name)
            times=np.arange(nframes,dtype=np.float32)/fps
            tidx=b.acc(times,5126,"SCALAR",bounds=True)
            ani={"name":name,"samplers":[],"channels":[]}
            for i,bone in enumerate(names):
                if bone not in index:raise ValueError("Unknown bone "+bone)
                for path,arr,kind in (("translation",tr[i],"VEC3"),("rotation",qs[i],"VEC4")):
                    if path=="rotation":
                        arr=arr/np.maximum(np.linalg.norm(arr,axis=1,keepdims=True),1e-8)
                        # Quaternion sign continuity for interpolation between frames.
                        arr=arr.copy()
                        for f in range(1,len(arr)):
                            if np.dot(arr[f-1],arr[f])<0:arr[f]*=-1
                    out=b.acc(arr,5126,kind)
                    samp=len(ani["samplers"])
                    ani["samplers"].append({"input":tidx,"output":out,"interpolation":"LINEAR"})
                    ani["channels"].append({"sampler":samp,
                                              "target":{"node":index[bone],"path":path}})
            b.gl["animations"].append(ani)
            clip_report.append({"name":name,"frames":nframes,"fps":fps,
                                "duration_s":round(float(times[-1]),3),
                                "animated_bones":len(names),
                                "source":"Original decoded Unity AnimationClip"})
        path=OUTPUT/"original_axe_holding_candidates.glb"
        b.finish(path)
        report={"result":"Exported authentic sampled animation curves (NOT new synthesized animations)",
                "original_asset":"raw_assets/assets/role/animationbundle.ab",
                "derived_source":"last_island_recreated_game/authentic_animations.npz",
                "original_weapon":"raw_assets/assets/model/wq_shifu.ab",
                "weapon_mesh":"last_island_recreated_game/assets/wq_shifu.obj",
                "attachment_bone":"wuqi_guadian",
                "original_socket_parent":parent["wuqi_guadian"],
                "pose_validation":"Original AnimatorController layer selection not independently proven",
                "important":"First two are HOLDING STANCE CANDIDATES; compare them to original gameplay before claiming exact axe hold.",
                "animations":clip_report,"meshes":["pants","torso","skin","shoes","gloves","head","hair","axe"],
                "file":path.name,"bytes":path.stat().st_size}
        (OUTPUT/"report.json").write_text(json.dumps(report,indent=2),encoding="utf-8")
        print(json.dumps(report,indent=2))
        print("OUTPUT",path)
if __name__=="__main__":main()
