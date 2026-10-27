"""
Animation Curve Extractor for Last Day Rules: Survival
Extracts the real sampled muscle curves from Unity AnimationClips.
"""
import os
import json
import UnityPy

ab_path = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\raw_assets\assets\role\animationbundle.ab'
env = UnityPy.load(ab_path)

TARGETS = [
    'kongshou_zhan_run_front01',
    'kongshou_zhan_idle01',
    'fu_zhan_attack_01'
]

anim_data = {}

for obj in env.objects:
    if obj.type.name == 'AnimationClip':
        data = obj.read()
        name = getattr(data, 'm_Name', '')
        if name in TARGETS:
            mc = getattr(data, 'm_MuscleClip', None)
            if not mc: continue
            clip = getattr(mc, 'm_Clip', None)
            if not clip: continue
            dense = getattr(clip.data, 'm_DenseClip', None)
            if not dense: continue

            curve_count = dense.m_CurveCount
            frame_count = dense.m_FrameCount
            samples = dense.m_SampleArray
            sample_rate = getattr(data, 'm_SampleRate', 30.0)

            print(f"Clip: {name} | Curves: {curve_count} | Frames: {frame_count} | FPS: {sample_rate} | Total Samples: {len(samples)}")

            # Reshape into frames [frame_idx][curve_idx]
            frames = []
            for f in range(frame_count):
                frame_samples = samples[f * curve_count : (f + 1) * curve_count]
                frames.append(frame_samples)

            anim_data[name] = {
                'fps': sample_rate,
                'frame_count': frame_count,
                'curve_count': curve_count,
                'frames': frames
            }

out_file = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'extracted_animations.json')
with open(out_file, 'w') as fp:
    json.dump(anim_data, fp)

print(f"Extracted {len(anim_data)} clips to {out_file}!")
