# Last Island of Survival LITE — reconstrução Android

Recriação experimental em **C++/Raylib para Android ARM64**, baseada na engenharia reversa de *Last Island of Survival LITE 7.3*. O jogo original foi preservado para consulta e extração de dados.

## Baixar para Android

Acesse [**Actions → Build Android ARM64**](https://github.com/48348484488/last-Island-of-survival-lite/actions/workflows/build_android_raylib_lite.yml), abra a execução mais recente **aprovada (✓)** e baixe o artefato **Last-Island-LITE-Recriado-Android-ARM64-APK**. Extraia o ZIP e instale o arquivo `.apk`.

> É uma versão de teste, não um port oficial. A execução no celular e a fidelidade visual precisam de validação; nem todas as funções do original estão implementadas.

## Organização

| Pasta / arquivo | Conteúdo |
|---|---|
| `last_island_recreated_game/main.cpp` | Jogo nativo Android: controles, câmera, coleta, armas e HUD |
| `last_island_recreated_game/native_original_animation.hpp` | Skinning C++ do personagem, 64 ossos e **33 animações extraídas** |
| `tools/export_mobile_original_hud.py` | Gera o HUD a partir dos sprites e `RectTransform` do Unity |
| `last_island_recreated_game/viewer3d.py` | Referência visual e funcional de PC (código-fonte Python) |
| `xapk_original/` | **XAPK original 7.3, preservado em partes** |
| `raw_assets/` | AssetBundles e recursos originais |
| `ExportedProject/` | Scripts C# descompilados para estudo |
| `tools/` | Extração, conversão e verificações automatizadas |

Para reconstruir o XAPK original: `python xapk_original/reassemble_xapk.py` (consulte o script para as opções). **Não excluir o XAPK, AssetBundles ou o projeto descompilado**: são fontes necessárias à engenharia reversa.

## Estado e limites

- **Implementado:** APK nativo ARM64, cenário 3D, itens, machado/M4, câmera, controles e 33 clipes de animação importados.
- **HUD:** sprites originais reconstruídos do `battlepanel.ab`, perfis machado/M4 e áreas de toque de origem Unity. Ainda faltam fontes originais, alguns painéis, efeitos e calibração completa de `CanvasScaler`.
- **Teste:** [compilação Android](https://github.com/48348484488/last-Island-of-survival-lite/actions/workflows/build_android_raylib_lite.yml) e [auditoria do HUD](https://github.com/48348484488/last-Island-of-survival-lite/actions/workflows/test_mobile_original_hud_export.yml).

**Observação:** os assets e o jogo original pertencem aos respectivos titulares; a presença dos arquivos neste repositório não concede direitos de redistribuição comercial.
