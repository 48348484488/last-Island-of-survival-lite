"""
Builds precise anatomical skeletal parts using real vertex bone weights from Unity SkinnedMeshRenderers.
Extracts:
- Pants -> Pelvis, R_Thigh, R_Calf (Knee), L_Thigh, L_Calf (Knee)
- Skin Arms -> Chest, R_UpperArm, R_Forearm (Elbow), L_UpperArm, L_Forearm (Elbow)
- Gloves -> R_Hand, L_Hand
- Shoes -> R_Shoe, L_Shoe
"""
import os
import UnityPy
from UnityPy.export.MeshExporter import MeshHandler

A = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'assets')
RAW = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\raw_assets\assets\role'

def export_submeshes(ab_filename, target_mesh_name, bone_mapping, out_prefix):
    """
    bone_mapping: dict mapping bone_index -> part_name
    """
    ab_path = os.path.join(RAW, ab_filename)
    if not os.path.exists(ab_path):
        print(f"File not found: {ab_path}")
        return
    env = UnityPy.load(ab_path)
    for obj in env.objects:
        if obj.type.name == 'Mesh':
            m = obj.read()
            if m.m_Name != target_mesh_name: continue
            print(f"Processing {m.m_Name} from {ab_filename}...")
            mh = MeshHandler(m)
            mh.process()
            
            verts = mh.m_Vertices
            norms = mh.m_Normals or []
            uvs   = mh.m_UV0 or []
            bone_indices = mh.m_BoneIndices
            submeshes = mh.get_triangles()
            triangles = []
            for sub in submeshes:
                triangles.extend(sub)
            
            # Map each triangle to a part based on majority primary bone
            part_triangles = {name: [] for name in set(bone_mapping.values())}
            
            for tri in triangles:
                b0 = bone_mapping.get(bone_indices[tri[0]][0], 'other')
                b1 = bone_mapping.get(bone_indices[tri[1]][0], 'other')
                b2 = bone_mapping.get(bone_indices[tri[2]][0], 'other')
                
                # Pick dominant part
                chosen = b0 if b0 != 'other' else (b1 if b1 != 'other' else b2)
                if chosen in part_triangles:
                    part_triangles[chosen].append(tri)
            
            # Write each part to OBJ
            for part_name, tris in part_triangles.items():
                if not tris: continue
                out_path = os.path.join(A, f"{out_prefix}_{part_name}.obj")
                # Collect used vertices
                used_verts = {}
                new_verts = []
                new_norms = []
                new_uvs = []
                new_tris = []
                
                for tri in tris:
                    poly = []
                    for vi in tri:
                        if vi not in used_verts:
                            used_verts[vi] = len(new_verts)
                            new_verts.append(verts[vi])
                            if vi < len(norms): new_norms.append(norms[vi])
                            if vi < len(uvs): new_uvs.append(uvs[vi])
                        poly.append(used_verts[vi])
                    new_tris.append(poly)
                
                with open(out_path, 'w') as f:
                    f.write(f"# Part {part_name}\n")
                    for v in new_verts:
                        # Unity space: x, y, z -> GL space: x, z, -y
                        f.write(f"v {v[0]:.6f} {v[2]:.6f} {-v[1]:.6f}\n")
                    for uv in new_uvs:
                        f.write(f"vt {uv[0]:.6f} {uv[1]:.6f}\n")
                    for n in new_norms:
                        f.write(f"vn {n[0]:.6f} {n[2]:.6f} {-n[1]:.6f}\n")
                    for tri in new_tris:
                        # 1-indexed OBJ face: v/vt/vn
                        f.write(f"f {tri[0]+1}/{tri[0]+1}/{tri[0]+1} {tri[1]+1}/{tri[1]+1}/{tri[1]+1} {tri[2]+1}/{tri[2]+1}/{tri[2]+1}\n")
                
                print(f"  Exported {out_path} ({len(new_verts)} verts, {len(new_tris)} tris)")

# 1. Pants: 0: L Calf, 1: L Thigh, 2: Pelvis, 3: R Thigh, 4: Spine1, 5: R Calf
pants_bones = {
    0: 'l_calf',
    1: 'l_thigh',
    2: 'pelvis',
    3: 'r_thigh',
    4: 'pelvis',
    5: 'r_calf',
}
export_submeshes('man_trousers_chushijuese_01.ab', 'man_trousers_chushijuese_01', pants_bones, 'part_pants')

# 2. Skin Arms: 0: R UpperArm, 1: Spine1, 2: Spine2, 3: L Clavicle, 4: L UpperArm, 5: R Clavicle, 6,7: R Forearm, 8: R Hand, 9,10: L Forearm, 11: L Hand, 12: Head, 13: Neck
skin_bones = {
    0: 'r_upperarm',
    4: 'l_upperarm',
    6: 'r_forearm',
    7: 'r_forearm',
    8: 'r_hand',
    9: 'l_forearm',
    10: 'l_forearm',
    11: 'l_hand',
    1: 'chest',
    2: 'chest',
    3: 'chest',
    5: 'chest',
    12: 'chest',
    13: 'chest',
}
export_submeshes('man_clothes_chushijuese_01.ab', 'man_clothes_chushijuese_01_skin', skin_bones, 'part_skin')

# 3. Shoes (Split Left and Right)
def split_mesh_lr(ab_filename, target_mesh_name, out_prefix):
    ab_path = os.path.join(RAW, ab_filename)
    env = UnityPy.load(ab_path)
    for obj in env.objects:
        if obj.type.name == 'Mesh':
            m = obj.read()
            if m.m_Name != target_mesh_name: continue
            mh = MeshHandler(m)
            mh.process()
            verts = mh.m_Vertices
            norms = mh.m_Normals or []
            uvs   = mh.m_UV0 or []
            submeshes = mh.get_triangles()
            triangles = []
            for sub in submeshes:
                triangles.extend(sub)
            
            for side, cond in [('r', lambda x: x > 0), ('l', lambda x: x <= 0)]:
                sub_tris = [t for t in triangles if cond((verts[t[0]][0] + verts[t[1]][0] + verts[t[2]][0])/3.0)]
                used_verts = {}
                new_verts, new_norms, new_uvs, new_tris = [], [], [], []
                for tri in sub_tris:
                    poly = []
                    for vi in tri:
                        if vi not in used_verts:
                            used_verts[vi] = len(new_verts)
                            new_verts.append(verts[vi])
                            if vi < len(norms): new_norms.append(norms[vi])
                            if vi < len(uvs): new_uvs.append(uvs[vi])
                        poly.append(used_verts[vi])
                    new_tris.append(poly)
                
                out_path = os.path.join(A, f"{out_prefix}_{side}.obj")
                with open(out_path, 'w') as f:
                    for v in new_verts:
                        f.write(f"v {v[0]:.6f} {v[2]:.6f} {-v[1]:.6f}\n")
                    for uv in new_uvs:
                        f.write(f"vt {uv[0]:.6f} {uv[1]:.6f}\n")
                    for n in new_norms:
                        f.write(f"vn {n[0]:.6f} {n[2]:.6f} {-n[1]:.6f}\n")
                    for tri in new_tris:
                        f.write(f"f {tri[0]+1}/{tri[0]+1}/{tri[0]+1} {tri[1]+1}/{tri[1]+1}/{tri[1]+1} {tri[2]+1}/{tri[2]+1}/{tri[2]+1}\n")
                print(f"  Exported {out_path} ({len(new_verts)} verts)")

split_mesh_lr('man_shoes_chushijuese_01.ab', 'man_shoes_chushijuese_01', 'part_shoe')
split_mesh_lr('man_glove_chushijuese_01.ab', 'man_glove_chushijuese_01', 'part_glove')
print("All rigged submeshes built successfully!")
