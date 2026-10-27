"""
Supreme Master Character Extractor for Last Day Rules: Survival
Extracts all vertices, bone weights, bone indices, triangles, and UVs
directly from the official Unity AssetBundles into master_character.npz.
"""
import os
import numpy as np
import UnityPy
from UnityPy.export.MeshExporter import MeshHandler

RAW = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\raw_assets\assets\role'
OUT_DIR = os.path.dirname(os.path.abspath(__file__))

TARGETS = [
    ('man_trousers_chushijuese_01.ab', 'man_trousers_chushijuese_01', 'pants'),
    ('man_clothes_chushijuese_01.ab',  'man_clothes_chushijuese_01',      'torso'),
    ('man_clothes_chushijuese_01.ab',  'man_clothes_chushijuese_01_skin', 'skin'),
    ('man_shoes_chushijuese_01.ab',     'man_shoes_chushijuese_01',        'shoes'),
    ('man_glove_chushijuese_01.ab',     'man_glove_chushijuese_01',        'gloves'),
    ('man_head_chushijuese_01.ab',      'man_head_chushijuese_01',         'head'),
    ('man_hair_chushijuese_01.ab',      'man_hair_chushijuese_01',         'hair'),
]

master_data = {}

for ab_name, mesh_name, label in TARGETS:
    ab_path = os.path.join(RAW, ab_name)
    env = UnityPy.load(ab_path)
    
    # 1. Get SMR bone names
    bone_names = []
    for obj in env.objects:
        if obj.type.name == 'SkinnedMeshRenderer':
            smr = obj.read()
            if smr.m_Mesh and smr.m_Mesh.read().m_Name == mesh_name:
                for b in smr.m_Bones:
                    t = b.read()
                    go = t.m_GameObject.read() if t.m_GameObject else None
                    bone_names.append(go.m_Name if go else 'Unknown')
                break
    
    # 2. Get Mesh vertices, weights, indices, UVs, normals, triangles
    for obj in env.objects:
        if obj.type.name == 'Mesh':
            m = obj.read()
            if m.m_Name != mesh_name: continue
            print(f"Extracting {mesh_name} ({label})...")
            mh = MeshHandler(m)
            mh.process()
            
            verts = np.array(mh.m_Vertices, dtype=np.float32)
            # Unity -> OpenGL coordinates: x, z, -y
            verts_gl = np.column_stack([verts[:, 0], verts[:, 2], -verts[:, 1]])
            
            norms = np.array(mh.m_Normals or [], dtype=np.float32)
            if len(norms) == len(verts):
                norms_gl = np.column_stack([norms[:, 0], norms[:, 2], -norms[:, 1]])
            else:
                norms_gl = np.zeros_like(verts_gl)
            
            uvs = np.array(mh.m_UV0 or [], dtype=np.float32)
            weights = np.array(mh.m_BoneWeights or [], dtype=np.float32)
            indices = np.array(mh.m_BoneIndices or [], dtype=np.int32)
            
            # Flatten submesh triangles
            triangles = []
            for sub in mh.get_triangles():
                triangles.extend(sub)
            triangles = np.array(triangles, dtype=np.int32)
            
            # Bind poses
            bind_poses = []
            for bp in m.m_BindPose:
                bind_poses.append([
                    [bp.e00, bp.e01, bp.e02, bp.e03],
                    [bp.e10, bp.e11, bp.e12, bp.e13],
                    [bp.e20, bp.e21, bp.e22, bp.e23],
                    [bp.e30, bp.e31, bp.e32, bp.e33],
                ])
            bind_poses = np.array(bind_poses, dtype=np.float32)
            
            print(f"  Verts: {len(verts_gl)} | Tris: {len(triangles)} | Bones: {len(bone_names)} | BindPoses: {len(bind_poses)}")
            
            master_data[f"{label}_verts"] = verts_gl
            master_data[f"{label}_norms"] = norms_gl
            master_data[f"{label}_uvs"] = uvs
            master_data[f"{label}_tris"] = triangles
            master_data[f"{label}_weights"] = weights
            master_data[f"{label}_indices"] = indices
            master_data[f"{label}_bind_poses"] = bind_poses
            master_data[f"{label}_bone_names"] = np.array(bone_names)
            break

out_npz = os.path.join(OUT_DIR, 'master_character.npz')
np.savez_compressed(out_npz, **master_data)
print(f"\n[OK] Extracted all skinned meshes into: {out_npz}")
