# Last Island LITE 7.3 — construção, HUD nativo V19

**Fonte comprovada:** `assets/ui/battlepanel.ab` original, extraído e indexado em
`last_island_recreated_game/assets/hud_original_xapk/`. A interface de
construção agora usa as imagens `Image.m_Sprite` por FileID/PathID exatos,
não imagens geradas artificialmente.

## Elementos do prefab original

| GameObject Unity | Função identificada | Canvas 1920x1080, origem superior esquerda |
|---|---|---|
| `btn_build` | Botão de construir/colocar | (1501.59, 674.69)–(1683.61, 856.71) |
| `btn_freeBuildModel` | Botão do modo livre | (1318.15, 817.50)–(1384.65, 884.00) |
| `btn_opRotate` | Girar objeto (operação no objeto) | (1314.30, 340.50)–(1539.30, 410.50) |
| `m_buildPartTypes` | Faixa de categorias | (568.69, 929.13)–(1326.61, 1024.13) |

A ordem registrada dos oito toggles filhos é `btn_null`,
`btn_lantu`, `btn_fundation`, `btn_wall`, `btn_floor`, `btn_stair`,
`btn_window`, `btn_obstacle`. O prefab serializa todos eles na mesma
posição inicial: no Unity a organização visível depende do layout group
e do estado da interface. O exportador coloca as oito células dentro do
Retângulo original `m_buildPartTypes`, repartido em oito células de 95
unidades aproximadas, evitando a sobreposição do prefab não executado.

`tools/export_mobile_original_hud.py` gera:
- `hud_original_build.png` — elementos de construção originais, sem a barra
  do inventário normal nem a faixa de categoria não posicionada;
- `hud_original_build_categories.png` — atlas de oito toggles originais;
- `hud_original_build_selected.png` — marcação `Checkmark` original;
- `hud_original_build_rotate.png` — botão `btn_opRotate` do prefab original;
- `mobile_original_hud_layout.hpp` — retângulos por ID dos objetos Unity.

A classe nativa usa os retângulos exportados tanto para **desenhar** quanto
para o **hit-test**, impedindo desencontro entre o clique e a imagem.

## Estado real de implementação

Os quatro botões artificiais da V18 (`TROCAR`, `GIRAR`,
`CONSTRUIR`, `SAIR`) foram removidos do modo construção.
O clique em `btn_build` posiciona a peça; as categorias de
fundação, parede e piso/teto selecionam os tipos disponíveis; `btn_opRotate`
gira a prévia. `btn_null` sai do modo, e `btn_freeBuildModel` não
anuncia funcionalidade completa onde ela ainda não foi portada.

**Limites que impedem declarar a versão 100% idêntica:**
- o comportamento exato de todos os toggles/categorias e animações de UI
  durante a execução Unity ainda requer comparação com o jogo original;
- geometria dos blocos de construção continua provisória;
- receitas, caixa de colisão e mecânica de construção em servidor ainda
  não foram recuperadas integralmente;
- escada, janela, obstáculo e posicionamento livre permanecem indisponíveis
  (as imagens originais são mostradas, mas o código informa a limitação).
