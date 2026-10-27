import os
import sys
import numpy as np

sys.path.append(r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game')
from extract_authentic_animations import build_bip001_hierarchy, extract_clip, BIP001_AB, ANIM_AB
import UnityPy

bip_info = build_bip001_hierarchy(BIP001_AB)
env = UnityPy.load(ANIM_AB)

test_clips = [
    'dao_zhan_rush_front01',
    'dao_zhan_nawuqi01',
    'dao_zhan_attack01',
    'fu_zhan_attack_01',
    'shuangshouchui_zhan_stand01',
    'caijizhuangzhi_zhan_stand01',
    'tongyong@bishou_zhan_stand01',
    'zhan_stand01'
]

clip_objs = {}
for obj in env.objects:
    if obj.type.name == 'AnimationClip':
        d = obj.read()
        if d.m_Name in test_clips:
            clip_objs[d.m_Name] = d

for name in test_clips:
    if name in clip_objs:
        res = extract_clip(name, clip_objs[name], bip_info)
        if res:
            bnames = list(res['bone_names'])
            rua_idx = bnames.index('Bip001 R UpperArm')
            rfa_idx = bnames.index('Bip001 R Forearm')
            rh_idx  = bnames.index('Bip001 R Hand')
            print(f"Clip: {name}, frames: {res['n_frames']}, fps: {res['fps']}")
            print(f"  R UpperArm rot f0: {np.round(res['rots'][rua_idx, 0], 3)}")
            print(f"  R Forearm  rot f0: {np.round(res['rots'][rfa_idx, 0], 3)}")
            print(f"  R Hand     rot f0: {np.round(res['rots'][rh_idx, 0], 3)}")
            if name == 'dao_zhan_nawuqi01':
                print(f"  (last frame) R UpperArm: {np.round(res['rots'][rua_idx, -1], 3)}")
                print(f"  (last frame) R Forearm:  {np.round(res['rots'][rfa_idx, -1], 3)}")
                print(f"  (last frame) R Hand:     {np.round(res['rots'][rh_idx, -1], 3)}")
