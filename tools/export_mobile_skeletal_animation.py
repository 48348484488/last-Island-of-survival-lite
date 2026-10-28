#!/usr/bin/env python3
"""Convert PC Unity extracted clips + original skinned meshes to a native GLES asset.

The binary contains the SAME bone local quaternions, translations, skin weights,
inverse bind matrices, UVs and triangles used by skinned_character.py. This is
NOT a procedural imitation or an animation rendered into screenshots.

Schema LIOSSKN1 little endian:
  magic[8], uint32 version=1, uint32 bone_count, mesh_count, clip_count
  bones: string name, int32 parent index (-1 root)
  meshes: string name, u32 N, u32 triangles, u32 influences,
          u32 palette_count; palette indices u32[palette_count];
          GL vertex float32[N,3], static normal float32[N,3],
          UV float32[N,2], weights float32[N,influences],
          indices uint16[N,influences], skin rhs matrix float32[palette_count,3,4],
          triangles uint16[triangle_count*3]
  clips: string name, u32 frame_count, float32 fps,
         bone local translations float32[frames,bones,3],
         rotations (xyzw) float32[frames,bones,4]

A native runtime computes the same FK/LBS as PC, with quaternion interpolation.
"""
import argparse
import json
import struct
import hashlib
from pathlib import Path

import numpy as np

HERE=Path(__file__).resolve().parents[1]/"last_island_recreated_game"
MESH_NAMES=("pants","torso","skin","shoes","gloves","head","hair")

class Writer:
    def __init__(self,file):self.out=file
    def u32(self,n):
        assert 0<=int(n)<(1<<32),n
        self.out.write(struct.pack("<I",int(n)))
    def i32(self,n):self.out.write(struct.pack("<i",int(n)))
    def f32(self,n):self.out.write(struct.pack("<f",float(n)))
    def name(self,name):
        b=name.encode("utf-8")
        assert 0<len(b)<128,name
        self.u32(len(b))
        self.out.write(b)
    def arr(self,x,dtype):
        self.out.write(np.ascontiguousarray(x,dtype=dtype).tobytes())

def export(path, source=HERE):
    path=Path(path)
    path.parent.mkdir(parents=True,exist_ok=True)
    src=np.load(source/"authentic_animations.npz",allow_pickle=True)
    mesh=np.load(source/"master_character.npz")
    clips=sorted(k[:-5] for k in src.files if k.endswith("_rots"))
    assert len(clips)==23,("Expected source 23 original clips",len(clips))
    original_bones=[str(n) for n in src[clips[0]+"_bone_names"]]
    parents=json.loads(str(src["_bone_parent_json"][0]))
    bone_idx={n:i for i,n in enumerate(original_bones)}
    assert len(bone_idx)==len(original_bones)
    assert "wuqi_guadian" in bone_idx, "Weapon socket required"
    M_GL2U=np.array([[1,0,0,0],[0,0,-1,0],[0,1,0,0],[0,0,0,1]],dtype=np.float32)
    summary={"format":"LIOSSKN1","source_clip_count":len(clips),"bones":len(original_bones),"meshes":{},
             "clips":{},"original_unity_weapon_socket":bone_idx["wuqi_guadian"]}
    with path.open("wb") as out:
        w=Writer(out)
        out.write(b"LIOSSKN1")
        w.u32(1);w.u32(len(original_bones));w.u32(len(MESH_NAMES));w.u32(len(clips))
        for bone in original_bones:
            w.name(bone)
            w.i32(bone_idx[parents[bone]] if parents.get(bone) in bone_idx else -1)
        for key in MESH_NAMES:
            verts=mesh[key+"_verts"].astype(np.float32)
            norms=mesh[key+"_norms"].astype(np.float32)
            uvs=mesh[key+"_uvs"].astype(np.float32)
            tris=mesh[key+"_tris"].reshape(-1)
            weights=mesh[key+"_weights"].astype(np.float32)
            indices=mesh[key+"_indices"].astype(np.int32)
            names=[str(n) for n in mesh[key+"_bone_names"]]
            palette=np.asarray([bone_idx[n] for n in names],dtype=np.uint32)
            bp=mesh[key+"_bind_poses"].astype(np.float32)
            rhs=(bp@M_GL2U)[:,:3,:]
            n=len(verts)
            if weights.ndim==1 or len(weights)==0:
                weights=np.ones((n,1),dtype=np.float32)
                indices=np.zeros((n,1),dtype=np.int32)
            assert verts.shape==(n,3) and norms.shape==(n,3)
            assert uvs.shape==(n,2) and len(tris)%3==0
            assert len(tris)>0 and n<65536 and int(tris.max())<n
            assert weights.shape==indices.shape and weights.shape[0]==n
            assert (indices>=0).all() and (indices<len(names)).all()
            assert np.isfinite(verts).all() and np.isfinite(rhs).all()
            assert (weights>=0).all()
            assert np.allclose(weights.sum(axis=1),1,atol=0.08)
            w.name(key)
            w.u32(n);w.u32(len(tris)//3);w.u32(weights.shape[1]);w.u32(len(names))
            w.arr(palette,"<u4")
            w.arr(verts,"<f4");w.arr(norms,"<f4");w.arr(uvs,"<f4")
            w.arr(weights,"<f4");w.arr(indices,"<u2")
            w.arr(rhs,"<f4")
            w.arr(tris,"<u2")
            summary["meshes"][key]={"vertices":n,"triangles":len(tris)//3,
                                     "influences":weights.shape[1],"bone_palette":len(names)}
        for key in clips:
            bone_names=[str(n) for n in src[key+"_bone_names"]]
            assert set(bone_names)==set(original_bones),key
            remap=[bone_names.index(n) for n in original_bones]
            f=int(src[key+"_n_frames"][0]);fps=float(src[key+"_fps"][0])
            assert 1<f<=500 and 1<=fps<=120,(key,f,fps)
            pos=src[key+"_trans"].astype(np.float32)[remap].transpose(1,0,2).copy()
            rot=src[key+"_rots"].astype(np.float32)[remap].transpose(1,0,2).copy()
            assert pos.shape==(f,len(original_bones),3)
            assert rot.shape==(f,len(original_bones),4)
            assert np.isfinite(pos).all() and np.isfinite(rot).all()
            # Exactly the in-place root correction of skinned_character.py.
            if "Bip001" in bone_idx:
                pos[:,bone_idx["Bip001"],(0,2)]=0
            w.name(key);w.u32(f);w.f32(fps)
            w.arr(pos,"<f4");w.arr(rot,"<f4")
            summary["clips"][key]={"frames":f,"fps":fps}
    data=path.read_bytes()
    summary.update({"binary_bytes":len(data),"binary_sha256":hashlib.sha256(data).hexdigest()})
    assert len(data)>100000
    print("LIOS_MOBILE_SOURCE_SKELETON_EXPORTED",json.dumps(summary,sort_keys=True))
    return summary

if __name__=="__main__":
    ap=argparse.ArgumentParser()
    ap.add_argument("--output",default="last_island_recreated_game/assets/mobile/lios_skeleton_v1.bin")
    args=ap.parse_args()
    export(args.output)
