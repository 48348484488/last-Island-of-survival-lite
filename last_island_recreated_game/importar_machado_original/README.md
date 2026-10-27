# Machado original do Last Island — pacote para importar

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
