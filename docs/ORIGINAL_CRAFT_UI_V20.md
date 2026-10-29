# V20 — Origem e estado da interface de fabricação/construção

## Engenharia reversa confirmada
- APK LITE original 7.3 (XAPK em 17 partes) contém **assets/ui/bagandbuildpanel.ab**, com 1685 GameObjects Unity, 1651 RectTransforms e 1244 componentes analisados. Esta é a origem da tela de mochila/fabricação (não o battlepanel.ab usado pelo HUD de combate).
- O prefab inclui `m_produce_page`, `m_produce_list`, `scp_items`, `btn_create`, `btn_back`, `btn_Build`, abas de ferramentas e áreas de receitas. O sistema original `ScProduceMgr.cs` utiliza `cfg.DrawingCfg`, duração de produção, desbloqueios e fila.
- Os atlas **assets/uiatlas/bagandbuildpanel.ab** e **assets/uiatlas/common.ab** forneceram **73 sprites originais** (32 + 41). O pipeline `tools/export_original_craft_ui.py` exporta PNGs pelos PathIDs originais e registra SHA256 de pixels e origem de cada imagem; 17 referências do prefab ainda não foram resolvidas.
- PNGs originais selecionados: `com_anniu_1` (botão produzir), `com_ty_gb_1` (voltar), `com_item_k_3` / `com_item_k_4` (slots), `com_chose_kuang` (seleção), `BagAndBuildPanel_bg_1` (faixa), sinais de adição/subtração.
- Ícones de ferramentas originais do XAPK: `cj_jzc` (martelo de construção), `cj_sg` (picareta de pedra). A identificação exata do ícone de cada receita e os dados de `cfg.ItemCfg`/`cfg.DrawingCfg` ainda requerem recuperação.

## Funcionalidade Android nativa
- `mobile_craft_ui.hpp` desenha os sprites originais encontrados, uma lista de receitas editável, detalhes, materiais, quantidade, botão fabricar e estado da fila. O desenho e a escala de parte do painel são **reconstruções nativas**, não um frame capturado do Unity.
- `native_build_craft.hpp` agenda produção no modo offline, debita recursos por quantidade, conclui itens por temporizador e permite cancelar itens aguardando com devolução dos recursos. Tempos e custos são parâmetros **provisórios**; o sistema original usa `DrawingCfg.needTime`.
- O item de construção pode ativar o estado de construção pela barra rápida (o código original `BuildState` transita por `BattleEvent.OnWantEnterBuildState`); não há mais botão falso de entrada na janela de fabricação.
- Barra de construção do `battlepanel.ab`: categorias originais `btn_null`, `btn_lantu`, `btn_fundation`, `btn_wall`, `btn_floor`, `btn_stair`, `btn_window`, `btn_obstacle`. No modo Android atual, respectivamente: limpar seleção, salvar planta da construção atual na sessão, fundação, parede, teto/piso, escada, janela e barricada.
- `btn_build` posiciona a peça, `btn_opRotate` gira, `btn_freeBuildModel` alterna a validação de encaixe. Janela e obstáculo têm colisões; escada pode ser construída. A geometria é provisória.
- Ao abrir a fabricação, entradas do HUD/arma/câmera não passam por trás da tela.

## Limitações importantes

**Não foi alcançada fidelidade de 100%** na interface de fabricação nem no sistema de construção. Os estados UGUI em tempo de execução, `LayoutGroup` dinâmico, máscaras, fonte de localização, custos/receitas completos `DrawingCfg`, persistência no servidor, upgrades e malhas/texturas de todas as peças ainda precisam ser recuperados e comparados no Android. A V20 contém sprites originais e mecânica offline funcional, mas a organização de parte da tela ainda é adaptada; ela **não deve ser anunciada como uma tela idêntica ao APK original**.

O fluxo de compilação Android testa o C++ nativo e inclui os PNGs com identificadores originais, obtidos do artefato imutável do GitHub Actions da extração do XAPK. Os testes automatizados não substituem testes visuais em aparelho Android.
