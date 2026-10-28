# Pulo original (3 fases) — versão para revisão

Este pacote contém os **6 clipes originais do Unity** (pulo para frente e para trás):
- **Impulsão:** \`kongshou_zhan_jump_up02\` (19 frames) / \`kongshou_zhan_jumpback_up02\` (15 frames)
- **No ar:** \`kongshou_zhan_jump_idle02\` / \`kongshou_zhan_jumpback_idle02\` (22 frames)
- **Aterrissagem:** \`kongshou_zhan_jump_down02\` (11 frames) / \`kongshou_zhan_jumpback_down02\` (19 frames)

Os clipes foram extraídos de \`raw_assets/assets/role/animationbundle.ab\` usando
os PathIDs referenciados no \`role.ab\` do personagem masculino.

**Integração:** \`viewer3d.py\` agora seleciona as 3 fases ao saltar com machado;
\`authentic_animations.npz\` contém todos os 6 clipes novos. O jogo Python usa
o NPZ. O GLB **é independente, para inspecionar ou importar em outro motor**.

A física do salto continua no código do jogo. A seleção da fase aérea e da
aterrissagem é aproximada ao fluxo original: não replica todo o motor Unity/IK.
Ainda não validado visualmente durante uma partida. Não alterei sacar/trocar arma.
