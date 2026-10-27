"""
Supreme Authentic Animation Extractor for Last Day Rules: Survival
Extracts 100% authentic per-frame per-bone LOCAL quaternions and translations
directly from the original APK animationbundle.ab (Unity AssetBundle).

Decodes StreamedClip, DenseClip, and ConstantClip faithfully using the exact
algorithm from uTinyRipper/AssetStudio AnimationClipConverter.cs.

Output: authentic_animations.npz
  - For each clip, stores:
    - {clip}_bone_names:    (B,)  array of bone name strings
    - {clip}_n_frames:      scalar, number of frames
    - {clip}_fps:           scalar, sample rate
    - {clip}_trans:         (B, N, 3) local translations  [x, y, z] Unity space
    - {clip}_rots:          (B, N, 4) local quaternions   [x, y, z, w] Unity space
    
  Also saves:
    - bone_parent_map:      JSON-encoded parent-child hierarchy
"""
import os
import sys
import json
import struct
import binascii
import numpy as np
import UnityPy

RAW = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\raw_assets\assets\role'
OUT_DIR = os.path.dirname(os.path.abspath(__file__))

BIP001_AB = os.path.join(RAW, 'bip001.ab')
ANIM_AB   = os.path.join(RAW, 'animationbundle.ab')

TARGET_CLIPS = [
    'kongshou_zhan_run_front01',    # barehanded run forward
    'kongshou_zhan_run_back01',     # barehanded run backward
    'kongshou_zhan_run_left01',     # barehanded strafe left
    'kongshou_zhan_run_right01',    # barehanded strafe right
    'kongshou_zhan_idle01',         # barehanded idle
    'kongshou_zhan_attack01',       # barehanded attack 1
    'kongshou_zhan_attack02',       # barehanded attack 2
    'fu_zhan_attack_01',            # axe attack
    'dao_zhan_rush_front01',        # authentic melee holding/sprint
    'dao_zhan_nawuqi01',            # authentic melee draw/ready stance
    'dao_zhan_attack01',            # melee swing 1
    'shuangshouchui_zhan_stand01',  # two-handed hammer/axe idle stand
    'shuangshouchui_zhan_run_front01', # two-handed hammer/axe run
    'caijizhuangzhi_zhan_stand01',  # gatherer stance
    'tongyong@bishou_zhan_stand01', # dagger stance
    'zhan_run_front01',             # generic/armed run front
    'zhan_run_back01',              # armed male run back
    'zhan_run_left01',              # armed male run left
    'zhan_run_right01',             # armed male run right
    'zhan_stand01',                 # generic stand
    'wq_m4_1_zhan_miaozhun_stand01',      # M4 aiming / rifle stand idle
    'wq_m4_1_zhan_miaozhun_run_front01',  # M4 aiming run forward
    'wq_m4_1_zhan_miaozhun_attack01',     # M4 shooting recoil
]

ATTR_TRANSLATION = 1
ATTR_ROTATION    = 2
ATTR_SCALE       = 3


def crc32(s: str) -> int:
    return binascii.crc32(s.encode('utf-8')) & 0xFFFFFFFF


def quat_to_mat4(q: np.ndarray) -> np.ndarray:
    x, y, z, w = q
    return np.array([
        [1 - 2*(y*y + z*z), 2*(x*y - z*w),     2*(x*z + y*w),  0],
        [2*(x*y + z*w),     1 - 2*(x*x + z*z), 2*(y*z - x*w),  0],
        [2*(x*z - y*w),     2*(y*z + x*w),     1 - 2*(x*x + y*y), 0],
        [0, 0, 0, 1]
    ], dtype=np.float32)


def build_bip001_hierarchy(bip_path: str) -> dict:
    """
    Loads bip001.ab and extracts:
      - crc_to_name: mapping from path CRC32 to bone name
      - parent_map: bone name -> parent bone name (or None for root)
      - rest_local: bone name -> (pos [3], quat [4]) local rest transforms
    """
    env = UnityPy.load(bip_path)
    transforms = {obj.path_id: obj.read() for obj in env.objects
                  if obj.type.name == 'Transform'}

    def get_path(t) -> str:
        go = t.m_GameObject.read() if t.m_GameObject else None
        name = go.m_Name if go else 'Unknown'
        if t.m_Father and t.m_Father.path_id in transforms:
            p = get_path(transforms[t.m_Father.path_id])
            return f'{p}/{name}' if p else name
        return name

    crc_to_name = {0: 'Bip001'}
    parent_map = {}
    rest_local = {}
    children_map = {}

    for pid, t in transforms.items():
        path = get_path(t)
        go_name = t.m_GameObject.read().m_Name
        lp = t.m_LocalPosition
        lq = t.m_LocalRotation

        rest_local[go_name] = (
            np.array([lp.x, lp.y, lp.z], dtype=np.float32),
            np.array([lq.x, lq.y, lq.z, lq.w], dtype=np.float32),
        )

        # Build CRC mappings for every path variant
        crc_to_name[crc32(path)] = go_name
        crc_to_name[crc32(go_name)] = go_name
        parts = path.split('/')
        for i in range(len(parts)):
            crc_to_name[crc32('/'.join(parts[i:]))] = go_name

        # Parent mapping
        if t.m_Father and t.m_Father.path_id in transforms:
            p_name = transforms[t.m_Father.path_id].m_GameObject.read().m_Name
            parent_map[go_name] = p_name
            children_map.setdefault(p_name, []).append(go_name)
        else:
            parent_map[go_name] = None

    return {
        'crc_to_name': crc_to_name,
        'parent_map': parent_map,
        'children_map': children_map,
        'rest_local': rest_local,
    }


def parse_streamed_clip(sc) -> dict:
    """
    Parses the raw uint32 StreamedClip data into frames.
    Format: [float time, int num_keys, then num_keys*5 (int idx, 4x float coeff)]
    coeff[3] = value
    Returns: {curve_index: {frame_time: float_value}}
    """
    raw_bytes = struct.pack(f'<{len(sc.data)}I', *sc.data)
    curves = {}
    offset = 0
    frames = []
    while offset < len(raw_bytes):
        time, num_keys = struct.unpack_from('<fi', raw_bytes, offset)
        offset += 8
        for _ in range(num_keys):
            idx, c0, c1, c2, c3 = struct.unpack_from('<iffff', raw_bytes, offset)
            offset += 20
            curves.setdefault(idx, {})[time] = c3  # c3 is the value
        frames.append(time)
    return curves, frames


def extract_clip(clip_name: str, anim_data, bip_info: dict) -> dict | None:
    """
    Decodes a single AnimationClip into per-bone per-frame local transforms.
    Uses the exact FindBinding algorithm from AssetStudio/uTinyRipper.
    """
    crc_to_name = bip_info['crc_to_name']
    
    gbs = anim_data.m_ClipBindingConstant.genericBindings
    clip = anim_data.m_MuscleClip.m_Clip

    sc = clip.data.m_StreamedClip
    dense = clip.data.m_DenseClip
    const_clip = clip.data.m_ConstantClip

    stream_count = sc.curveCount if sc else 0
    n_frames = dense.m_FrameCount
    fps = dense.m_SampleRate
    dense_curve_count = dense.m_CurveCount
    
    dense_samples = np.array(dense.m_SampleArray, dtype=np.float32).reshape(n_frames, dense_curve_count)
    const_data = np.array(const_clip.data if const_clip else [], dtype=np.float32)

    dense_start = stream_count
    const_start = stream_count + dense_curve_count

    # Parse StreamedClip
    streamed_by_curve, stream_times = parse_streamed_clip(sc) if sc else ({}, [])

    # The FindBinding algorithm: walk all genericBindings accumulating channel count
    # StreamedClip channels: 0 .. stream_count-1
    # DenseClip channels:    stream_count .. stream_count+dense_curve_count-1
    # ConstantClip channels: stream_count+dense_curve_count .. end

    def get_dim(gb) -> int:
        if gb.attribute == ATTR_TRANSLATION: return 3
        if gb.attribute == ATTR_ROTATION:    return 4
        if gb.attribute == ATTR_SCALE:       return 3
        if gb.attribute == 4:                return 3  # EulerAngles
        return 1

    # bone -> (trans_frames, rot_frames)
    # trans_frames/rot_frames: list of (frame_idx, values) or just ndarray per frame
    # We will accumulate into arrays of shape (n_frames, 3/4)
    bone_trans = {}  # bone_name -> ndarray (n_frames, 3)
    bone_rots  = {}  # bone_name -> ndarray (n_frames, 4)

    cursor = 0
    for gb in gbs:
        dim = get_dim(gb)
        bone_name = crc_to_name.get(gb.path, f'UNK_{gb.path}')
        attr = gb.attribute

        curve_start = cursor
        curve_end   = cursor + dim
        cursor = curve_end

        if attr not in (ATTR_TRANSLATION, ATTR_ROTATION):
            continue  # Skip Scale and other attributes

        # --- DenseClip ---
        if dense_start <= curve_start and curve_end <= dense_start + dense_curve_count:
            d_s = curve_start - dense_start
            d_e = curve_end   - dense_start
            vals = dense_samples[:, d_s:d_e]   # (n_frames, dim)
            if attr == ATTR_ROTATION:
                bone_rots[bone_name]  = vals
            else:
                bone_trans[bone_name] = vals

        # --- ConstantClip ---
        elif const_start <= curve_start and curve_end <= const_start + len(const_data):
            c_s = curve_start - const_start
            c_e = curve_end   - const_start
            vals_const = const_data[c_s:c_e]   # (dim,)
            vals = np.tile(vals_const, (n_frames, 1))
            if attr == ATTR_ROTATION:
                bone_rots[bone_name]  = vals
            else:
                bone_trans[bone_name] = vals

        # --- StreamedClip ---
        elif curve_start < stream_count:
            # Each curve index in streamed_by_curve has time -> value
            # Collect per-frame values by resampling at dense frame times
            frame_times = np.arange(n_frames, dtype=np.float32) / fps
            vals = np.zeros((n_frames, dim), dtype=np.float32)
            for k in range(dim):
                cidx = curve_start + k
                if cidx in streamed_by_curve:
                    times_arr = np.array(sorted(streamed_by_curve[cidx].keys()))
                    values_arr = np.array([streamed_by_curve[cidx][t] for t in sorted(streamed_by_curve[cidx].keys())])
                    vals[:, k] = np.interp(frame_times, times_arr, values_arr)
            if attr == ATTR_ROTATION:
                bone_rots[bone_name]  = vals
            else:
                bone_trans[bone_name] = vals

    # Normalize all quaternions
    for name, rots in bone_rots.items():
        norms = np.linalg.norm(rots, axis=1, keepdims=True)
        norms = np.where(norms < 1e-8, 1.0, norms)
        bone_rots[name] = rots / norms

    # Gather unique bone names
    all_bones = sorted(bip_info['parent_map'].keys())
    B = len(all_bones)

    # Build output arrays (B, N, 3/4) using rest pose as fallback
    rest_local = bip_info['rest_local']

    trans_arr = np.zeros((B, n_frames, 3), dtype=np.float32)
    rots_arr  = np.zeros((B, n_frames, 4), dtype=np.float32)

    for i, bone in enumerate(all_bones):
        if bone in bone_rots:
            rots_arr[i] = bone_rots[bone]
        elif bone in rest_local:
            rots_arr[i] = np.tile(rest_local[bone][1], (n_frames, 1))
        else:
            rots_arr[i, :, 3] = 1.0  # identity

        if bone in bone_trans:
            trans_arr[i] = bone_trans[bone]
        elif bone in rest_local:
            trans_arr[i] = np.tile(rest_local[bone][0], (n_frames, 1))

    return {
        'bone_names': all_bones,
        'n_frames':   n_frames,
        'fps':        fps,
        'trans':      trans_arr,   # (B, N, 3) local translations Unity space
        'rots':       rots_arr,    # (B, N, 4) local quaternions Unity space [x,y,z,w]
    }


def main():
    print("[1/3] Loading bone hierarchy from bip001.ab...")
    bip_info = build_bip001_hierarchy(BIP001_AB)
    print(f"      Found {len(bip_info['parent_map'])} bones")

    print("[2/3] Loading animation clips from animationbundle.ab...")
    env_anim = UnityPy.load(ANIM_AB)
    
    clip_objects = {}
    for obj in env_anim.objects:
        if obj.type.name == 'AnimationClip':
            d = obj.read()
            if d.m_Name not in clip_objects:
                clip_objects[d.m_Name] = d

    print(f"      Found {len(clip_objects)} unique clips in bundle")

    out_data = {}
    out_data['_bone_parent_json'] = np.array([json.dumps(bip_info['parent_map'])], dtype=object)

    print("[3/3] Extracting target clips...")
    found_clips = []
    for clip_name in TARGET_CLIPS:
        if clip_name not in clip_objects:
            print(f"  [SKIP] '{clip_name}' not found in bundle")
            continue
        
        anim_data = clip_objects[clip_name]
        result = extract_clip(clip_name, anim_data, bip_info)
        
        if result is None:
            print(f"  [FAIL] '{clip_name}' extraction failed")
            continue

        safe_name = clip_name.replace('@', '_')
        out_data[f'{safe_name}_bone_names'] = np.array(result['bone_names'], dtype=object)
        out_data[f'{safe_name}_n_frames']   = np.array([result['n_frames']], dtype=np.int32)
        out_data[f'{safe_name}_fps']        = np.array([result['fps']], dtype=np.float32)
        out_data[f'{safe_name}_trans']      = result['trans']
        out_data[f'{safe_name}_rots']       = result['rots']
        
        found_clips.append(clip_name)
        B, N, _ = result['trans'].shape
        print(f"  [OK] '{clip_name}': {B} bones, {N} frames @ {result['fps']}fps")

    out_path = os.path.join(OUT_DIR, 'authentic_animations.npz')
    np.savez_compressed(out_path, **out_data)
    print(f"\n[OK] Saved {len(found_clips)} clips to: {out_path}")


if __name__ == '__main__':
    main()
