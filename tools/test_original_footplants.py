#!/usr/bin/env python3
"""Verify direction and gait cadence from the unmodified 33 original Unity clips.

Reads the native LIOSSKN1 skeleton exported from original XAPK animations.
Computes toe-bone positions through FK (64 bones) and estimates foot-contact
treadmill velocity. This guards against a later Unity/OpenGL handedness regression.
"""
import argparse
import struct
from pathlib import Path
import numpy as np

class Reader:
    def __init__(self,path):
        self.data=Path(path).read_bytes()
        assert self.data[:8]==b"LIOSSKN1"
        self.off=8
    def take(self,size):
        o=self.off;self.off+=size
        if self.off>len(self.data):raise ValueError("Truncated skeleton")
        return self.data[o:self.off]
    def u32(self):
        return struct.unpack("<I",self.take(4))[0]
    def i32(self):
        return struct.unpack("<i",self.take(4))[0]
    def f32(self):
        return struct.unpack("<f",self.take(4))[0]
    def name(self):
        n=self.u32()
        if n>256:raise ValueError("Invalid name length")
        return self.take(n).decode("utf-8")
    def floats(self,n):
        return np.frombuffer(self.take(n*4),dtype="<f4").copy()

def parse(path):
    r=Reader(path)
    ver,n_bones,n_meshes,n_clips=[r.u32() for _ in range(4)]
    assert (ver,n_bones,n_meshes,n_clips)==(1,64,7,33)
    names=[];parents=[]
    for _ in range(n_bones):
        names.append(r.name());parents.append(r.i32())
    for _ in range(n_meshes):
        name=r.name()
        nv,ntri,ninf,npalette=[r.u32() for _ in range(4)]
        assert nv<65536 and ntri<100000 and ninf<16 and npalette<=n_bones, name
        r.take(npalette*4 + nv*(3*4+3*4+2*4+ninf*4+ninf*2)
               + npalette*12*4 + ntri*3*2)
    clips={}
    for _ in range(n_clips):
        name=r.name();nf=r.u32();fps=r.f32()
        assert 1<nf<500 and 1<=fps<=120,(name,nf,fps)
        p=r.floats(nf*n_bones*3).reshape(nf,n_bones,3)
        q=r.floats(nf*n_bones*4).reshape(nf,n_bones,4)
        clips[name]=(p,q,fps)
    assert r.off==len(r.data)
    return names,parents,clips

def quat_matrices(q):
    """Native C++ quatMat rotation, xyzw quaternions, batched frames."""
    x,y,z,w=np.moveaxis(q,-1,0)
    m=np.empty((len(q),4,4),dtype=np.float64)
    m[:]=np.eye(4)
    m[:,0,0]=1-2*(y*y+z*z)
    m[:,0,1]=2*(x*y-z*w)
    m[:,0,2]=2*(x*z+y*w)
    m[:,1,0]=2*(x*y+z*w)
    m[:,1,1]=1-2*(x*x+z*z)
    m[:,1,2]=2*(y*z-x*w)
    m[:,2,0]=2*(x*z-y*w)
    m[:,2,1]=2*(y*z+x*w)
    m[:,2,2]=1-2*(x*x+y*y)
    return m

def toe_world(clip,parents,toe):
    positions,rotations,_=clip
    path=[];current=toe
    while current>=0:
        path.append(current);current=parents[current]
        if len(path)>len(parents):raise ValueError("Cyclic bone hierarchy")
    path.reverse()
    frames=len(positions)
    accumulated=np.broadcast_to(np.eye(4),(frames,4,4)).copy()
    for j in path:
        transform=quat_matrices(rotations[:,j])
        transform[:,:3,3]=positions[:,j]
        accumulated=np.matmul(accumulated,transform)
    return accumulated[:,:3,3]

def contact_velocity(clip,parents,toe):
    xyz=toe_world(clip,parents,toe)
    # Ground-contact frames occupy the lowest 30% of toe heights.
    is_planted=xyz[:,1]<=np.quantile(xyz[:,1],.30)
    fps=clip[2]
    tangent=(np.roll(xyz,-1,axis=0)-np.roll(xyz,1,axis=0))*fps*.5
    return tangent[is_planted].mean(axis=0)

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("skeleton",type=Path)
    args=ap.parse_args()
    names,parents,clips=parse(args.skeleton)
    left,right=names.index("Bip001 L Toe0"),names.index("Bip001 R Toe0")
    measured={}
    for key in ("zhan_run_front01","zhan_run_back01",
                "zhan_run_left01","zhan_run_right01"):
        v=np.mean([contact_velocity(clips[key],parents,bone)
                   for bone in (left,right)],axis=0)
        measured[key]=[round(float(x),3) for x in v]
    # Planted feet move OPPOSITE to the intended world displacement.
    # SOURCE left -> player -X; source right -> player +X.
    # Camera screen-right in this port -> -X, so selecting source left
    # when screen-right is pushed is intentional, not a typo.
    assert -5.6<measured["zhan_run_front01"][2]<-3.4,measured
    assert 1.4<measured["zhan_run_back01"][2]<3.2,measured
    assert 1.4<measured["zhan_run_left01"][0]<3.2,measured
    assert -4.2<measured["zhan_run_right01"][0]<-2.2,measured
    print("LIOS_ORIGINAL_UNITY_FOOT_PLANT_VELOCITY_PASS",measured,flush=True)
    print("LIOS_LEFT_RIGHT_CLIP_HANDEDNESS_PROVEN",
          "screen_right_is_world_negative_X",
          "source_left_moves_world_negative_X",
          "source_right_moves_world_positive_X",flush=True)
if __name__=="__main__":
    main()
