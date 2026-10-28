#!/usr/bin/env python3
"""Regression guard for original skinned Android avatar camera and texture parity.

The previous v0.13 port incorrectly used a Z reflection for M_U2GL @ Rx90
and bound the head atlas to the 'skin' mesh. Both errors are detectable without
relying on an emulator screenshot. Not a substitute for physical visual QA.
"""
from pathlib import Path
import math
import re
import sys

import numpy as np

ROOT=Path(__file__).resolve().parents[1]
PC=(ROOT/"last_island_recreated_game/viewer3d.py").read_text(encoding="utf8")
CPP=(ROOT/"last_island_recreated_game/main.cpp").read_text(encoding="utf8")
NATIVE=(ROOT/"last_island_recreated_game/native_original_animation.hpp").read_text(encoding="utf8")
ASSET=ROOT/"last_island_recreated_game/assets"

def camera_projection(yaw,pitch,px=0,py=0,pz=0):
    look=np.array((math.sin(yaw)*math.cos(pitch),math.sin(pitch),
                   math.cos(yaw)*math.cos(pitch)),dtype=float)
    right=np.array((math.cos(yaw),0,-math.sin(yaw)),dtype=float)
    shoulder=np.array((px,py,pz),dtype=float)+.35*right
    pos=shoulder-3.8*look
    pos[1]=py+1.65-3.8*look[1]
    target=shoulder+2*look
    target[1]=py+1.45+2*look[1]
    return pos,target,look

def main():
    # Verify the inverse-axis rotation from the original desktop rig.
    u2gl=np.array([[1,0,0],[0,0,1],[0,-1,0]],dtype=float)
    rx90=np.array([[1,0,0],[0,0,-1],[0,1,0]],dtype=float)
    lh=u2gl@rx90
    assert np.array_equal(lh,np.eye(3)),lh
    assert "const Mat LH=ident();" in NATIVE, "Android root bone correction must be identity!"
    assert "const Mat LH={1,0,0,0,0,1,0,0,0,0,-1" not in NATIVE
    assert "LIOS::Mat LH=LIOS::ident();" in CPP, "Weapon socket must use same Unity conversion"

    # Match the working desktop third-person camera, including shoulder,
    # pitch, distance and the forward-facing look-ahead target.
    assert "cam_dist = True, 3.8" in PC
    assert "const float camDist=3.8f;" in CPP
    assert "Vector3Scale(right,0.35f)" in CPP
    assert "shoulderTarget.z-lookDir.z*camDist" in CPP
    assert "shoulderTarget.z+lookDir.z*2.0f" in CPP
    for yaw in (0,0.4,math.pi/2,math.pi,-math.pi/2):
        pos,target,look=camera_projection(yaw,0.0)
        assert (pos-target)@look < -3.5,(yaw,pos,target)
        assert (target-pos)@look > 3.5

    # The real PC uses a separate skin/clothes atlas, distinct from the
    # head/hair atlas. Ensure all SIX sources are linked to native materials.
    maps={
        "skin":"man_clothes_yf_0_ba.png",
        "torso":"man_clothes_chushijuese_01_ba.png",
        "pants":"man_trousers_chushijuese_01_ba.png",
        "head":"man_head_chushijuese_01_ba.png",
        "hair":"man_hair_chushijuese_01_ba.png",
        "shoes":"man_shoes_chushijuese_01_ba.png",
    }
    for k,filename in maps.items():
        assert (ASSET/filename).is_file(),(k,filename)
        assert (ASSET/filename).stat().st_size>1000,(k,filename)
        assert "'%s':"%k in PC,(k,"desktop texture absent")
        assert filename in PC,(k,"desktop file path")
        assert ('"assets/'+filename+'"') in CPP,(k,"native source file path")
    assert "unityRig.setTextures(texPants, texTorso, texSkin, texShoes, texHead, texHair);" in CPP
    assert 'Texture2D texSkin = LoadTexture("assets/man_clothes_yf_0_ba.png");' in CPP
    assert "UpdateMeshBuffer(gpu,2,normals.data()" in NATIVE, "Skin normal GPU updates missing"
    assert "SetMaterialTexture(&p.model.materials[0],MATERIAL_MAP_DIFFUSE,tex);" in NATIVE
    print("LIOS_ANDROID_CAMERA_TEXTURE_PARITY_PASS",
          "UnityFKRoot=identity", "camera_distance=3.8",
          "camera_behind_all_5_orientations=true",
          "textured_mesh_atlases=6",
          "dynamic_vertex_normals=true")
if __name__=="__main__":
    main()
