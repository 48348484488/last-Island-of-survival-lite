# Corrida normal original — pronta para revisão

A animação \`zhan_run_front02\` foi localizada no \`role.ab\`
(PathID \`7186278071774183593\`) e extraída de
\`animationbundle.ab\`. Possui **22 quadros a 30 FPS**.

O jogo Python agora contém **24 animações** em
\`last_island_recreated_game/authentic_animations.npz\`.
A animação de corrida normal frontal foi corrigida em
\`last_island_recreated_game/viewer3d.py\`.

O arquivo \`corrida_normal_original_zhan_run_front02.glb\`
é uma **visualização 3D independente** da animação original.
O motor Python usa o NPZ, não carrega o GLB.

Importante: a corrida rápida \`dao_zhan_rush_front01\` do pacote
anterior não foi aprovada visualmente: o NPZ antigo não reproduzia
os ossos móveis. Enquanto não for reconstruída corretamente,
o jogo reproduz o clipe normal validado com velocidade maior
quando Shift é pressionado. Não é a corrida rápida exata do original.

Pulo, sacar arma e troca de arma **ainda não foram alterados**.
A imagem deve ser conferida antes de iniciar a próxima etapa.
