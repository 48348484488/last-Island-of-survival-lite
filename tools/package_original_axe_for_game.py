#!/usr/bin/env python3
"""Package the source-based original stone axe animation for game + AI handoff.

Pre-requisite: python tools/export_original_stone_axe_3d.py
The 3D model is exported as a standalone GLB; the existing Python game plays
the same original skeletal clips through authentic_animations.npz and uses
the corrected weapon binding transform baked into skinned_character.py.
"""
import ast
import hashlib
import json
import shutil
import struct
from pathlib import Path
import numpy as np

from export_original_stone_axe_3d import ROOT, source_matrix, audit_controller

GAME = ROOT / "last_island_recreated_game"
PACK = GAME / "importar_machado_original"
SOURCE = ROOT / "original_axe_3d_verified"
PACK.mkdir(exist_ok=True, parents=True)

def socket_matrix_in_game():
    src = (GAME / "skinned_character.py").read_text(encoding="utf-8")
    module = ast.parse(src)
    found = [
        n for n in module.body
        if isinstance(n, ast.Assign)
        and any(isinstance(t,ast.Name) and t.id=="_AXE_SOCKET_MATRIX" for t in n.targets)
    ]
    assert len(found)==1, "Expected exactly one _AXE_SOCKET_MATRIX"
    val = found[0].value
    assert isinstance(val, ast.Attribute) and val.attr == "T"
    expr=val.value
    assert isinstance(expr, ast.Call) and isinstance(expr.func, ast.Attribute)
    assert expr.func.attr == "array"
    mat=ast.literal_eval(expr.args[0])
    return np.asarray(mat, dtype=np.float64)

def verify_original_glb(path, matrix):
    blob = path.read_bytes()
    assert blob[:4] == b"glTF"
    length = struct.unpack_from("<I", blob, 8)[0]
    assert length == len(blob)
    jlen = struct.unpack_from("<I", blob, 12)[0]
    assert blob[16:20] == b"JSON"
    meta = json.loads(blob[20:20+jlen])
    assert len(meta["animations"]) == 2
    assert {v["name"] for v in meta["animations"]} == {"zhan_stand01", "fu_zhan_attack_01"}
    names = [n.get("name") for n in meta["nodes"]]
    axe_id = names.index("StoneAxe_wq_shifu")
    socket_id = names.index("wuqi_guadian")
    assert axe_id in meta["nodes"][socket_id]["children"]
    glb_matrix = np.asarray(meta["nodes"][axe_id]["matrix"], dtype=np.float64).reshape(4,4).T
    assert np.allclose(glb_matrix, matrix, atol=1e-6), "GLB matrix differs from source"
    return blob, meta

def main():
    m, bone_id = source_matrix()
    game_socket=socket_matrix_in_game()
    assert np.allclose(m, game_socket, atol=1e-6), \
        "Game uses a different axe matrix from original Unity wq_shifu Bone001"
    viewer = (GAME / "viewer3d.py").read_text(encoding="utf-8")
    assert "AXE_IDLE     = 'zhan_stand01'" in viewer
    assert "AXE_ATTACK   = 'fu_zhan_attack_01'" in viewer
    assert "axe_dl        = load_obj(p('wq_shifu.obj'), swap_yz=False)" in viewer
    assert "glMultMatrixf(self._AXE_SOCKET_MATRIX)" in (GAME/"skinned_character.py").read_text()
    glb_src = SOURCE / "machado_original_postura_e_golpe_3d.glb"
    assert glb_src.is_file(), "Generate the asset first with tools/export_original_stone_axe_3d.py"
    glb_bytes, _ = verify_original_glb(glb_src, m)
    dest = PACK / "machado_original_postura_e_golpe_3d.glb"
    dest.write_bytes(glb_bytes)
    shutil.copyfile(SOURCE/"verification.json", PACK/"prova_origem.json")
    manifest={
        "name":"Last Island of Survival LITE — stone axe 3D source asset handoff",
        "status":"Game Python renderer updated to the original Unity weapon mesh matrix; no visual live game test",
        "package_file":dest.name,
        "sha256":hashlib.sha256(glb_bytes).hexdigest(),
        "bytes":len(glb_bytes),
        "target_game":"last_island_recreated_game/viewer3d.py",
        "game_runtime":"last_island_recreated_game/skinned_character.py",
        "item_id":55001,
        "original_weapon_bundle":"raw_assets/assets/model/wq_shifu.ab",
        "original_animation_bundle":"raw_assets/assets/role/animationbundle.ab",
        "original_animator_bundle":"raw_assets/assets/role/role.ab",
        "weapon_socket":"wuqi_guadian",
        "weapon_prefab_bone_id":bone_id,
        "original_weapon_local_matrix":m.tolist(),
        "animations":[
            {"name":"zhan_stand01","state":"Base Layer.Stand.jinzhan.f","clip_path_id":"2004069720870033944","validated":"melee idle state, not exclusively axe"},
            {"name":"fu_zhan_attack_01","state":"FullBody Layer.Attack.fu_zhan_attack_01","clip_path_id":"4382079750961674410","validated":"axe attack, source AttackState.cs weapon ids 55001..55005"}
        ],
        "limitations":[
            "This GLB is an exported preview/handoff; Python OpenGL game uses NPZ, OBJ and the socket matrix rather than loading GLB.",
            "Not visually verified against in-game Unity runtime: IK, layer masks and extra rig logic may differ."
        ]
    }
    (PACK/"manifest.json").write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
    readme="""# Machado original do Last Island — pacote para importar

Este pacote fica **dentro do jogo recriado**: \`last_island_recreated_game/importar_machado_original/\`.

## Arquivos
- \`machado_original_postura_e_golpe_3d.glb\` — personagem com o machado, esqueleto, texturas e dois clipes originais.
- \`manifest.json\` — nomes e IDs das animações, SHA-256 e matriz original de encaixe.
- \`prova_origem.json\` — relatório de validação dos AssetBundles originais.

## Estado da integração
O jogo Python usa \`authentic_animations.npz\` e \`wq_shifu.obj\` diretamente.
**Já foi atualizado** \`last_island_recreated_game/skinned_character.py\`
para usar a matriz de arma extraída de \`wq_shifu.ab\`, em vez da matriz antiga
estimada. O \`viewer3d.py\` já selecionava \`zhan_stand01\` (parado) e
\`fu_zhan_attack_01\` (golpe). Assim, a correção do encaixe foi incorporada ao
código do jogo, **mas ainda não foi validada visualmente durante uma partida**.

O arquivo \`.glb\` desta pasta é uma **cópia de entrega/importação**; ele
não é carregado automaticamente pelo motor Python/OpenGL do jogo.

## Para outra IA ou desenvolvedor integrar em outro motor
1. Importe o GLB e escolha o clipe \`zhan_stand01\` para repouso de arma
   corpo a corpo, e \`fu_zhan_attack_01\` para o golpe de machado.
2. Prenda o modelo do machado em \`wuqi_guadian\` (mão direita).
   O GLB já tem essa ligação e a transformação original do prefab.
3. Confira a mão e o cabo visualmente, o IK e o peso das camadas,
   porque não se comprovou equivalência total com o motor Unity.
4. Use o \`manifest.json\` e \`prova_origem.json\` para auditar os IDs
   originais; não confunda clipe de repouso *genérico* com uma animação
   exclusiva do machado.

## Recriar este pacote
Na raiz do repositório:
\`\`\`bash
python -m pip install "UnityPy>=1.20,<2" numpy trimesh Pillow
python tools/export_original_stone_axe_3d.py
python tools/package_original_axe_for_game.py
\`\`\`
Não precisa instalar um APK nem realizar testes no celular para recriar
os arquivos. Os testes estruturais não substituem uma avaliação visual.
"""
    (PACK/"README.md").write_text(readme,encoding="utf-8")
    print("GAME_SOCKET_MATCHES_ORIGINAL_PREFAB: PASS (atol=1e-6)")
    print("GAME_ANIMATION_SELECTION: PASS")
    print("GLB_ORIGINAL_CLIPS_AND_WEAPON_ATTACHMENT: PASS")
    print("PACKAGE:",str(PACK.relative_to(ROOT)))
    for f in PACK.iterdir():
        print("  FILE",f.name,f.stat().st_size)
    print("SHA256",manifest["sha256"])

if __name__=="__main__":
    main()
