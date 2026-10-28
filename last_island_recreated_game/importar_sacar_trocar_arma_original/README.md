# Etapa 3: Sacar e guardar arma (fonte original)

O arquivo \`sacar_guardar_original_machado_3d.glb\` contém o personagem com
o machado de pedra e as curvas originais dos clipes de empunhar e guardar.

As **quatro animações de 14 quadros / 30 FPS** estão no
\`authentic_animations.npz\` do jogo:
- \`dao_zhan_shouwuqi01\` (guardar machado)
- \`dao_zhan_nawuqi01\` (sacar machado)
- \`chongfengqiang_hengwo_zhan_right_shouwuqi01\` (guardar fuzil)
- \`chongfengqiang_hengwo_zhan_right_nawuqi01\` (sacar fuzil)

O GLB é só para visualizar: a malha de arma incluída é o machado.
**Não** use o GLB como prévia da arma M4A1, embora seus clipes de
movimento estejam presentes. Os estados/IDs vêm do Unity \`role.ab\`.
Um dos clipes de sacar machado já existia no NPZ, portanto são **33 clipes únicos** após importar os três restantes e atualizar o quarto.
O código original troca a arma da mão em 0,4s guardando e 0,2s sacando.

Ainda não foi ativada a troca animada no jogo Python; aguardamos
avaliação visual do saque/guarda antes de alterar a jogabilidade.
