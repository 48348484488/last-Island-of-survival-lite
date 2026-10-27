# Etapa 1 — Corrida rápida com machado (extraída do original)

- **Integrado no código do jogo:** \`last_island_recreated_game/viewer3d.py\`
  seleciona \`dao_zhan_rush_front01\` quando o jogador segura machado,
  corre para frente e segura Shift.
- **Arquivo 3D:** \`corrida_original_machado_3d.glb\` inclui a animação
  original de 18 quadros a 30 FPS e o machado alinhado ao
  prefab original \`wq_shifu.ab\`.
- **Estado fonte:** \`FullBody Layer.HoldJinzhanWeapon.dao_zhan_rush_front01\`.
- **Ainda não feito:** a corrida normal \`zhan_run_front02\`, pulo e
  a troca/sacar arma. Não incluir essas etapas sem aprovação da imagem.

O jogo Python não lê GLB diretamente; utiliza \`authentic_animations.npz\`
e \`skinned_character.py\`. O GLB é para inspecionar o resultado e
permitir integração com outros motores. Testes unitários da escolha da
animação e validação dos AssetBundles passaram; validação visual
durante execução do jogo ainda está pendente.
