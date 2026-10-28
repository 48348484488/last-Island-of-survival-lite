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

## Android V16.2 — estado técnico

- **Animação lateral:** seleção do clipe esquerdo/direito pelo vetor local do personagem e correção da orientação UV dos sete meshes do rig Unity.
- **HUD:** dois perfis da UI extraída do `battlepanel.ab`, compasso dinâmico com 72 posições e os **sete destaques `m_heightLignt` reais** aplicados ao slot selecionado. Clicar nos slots 3–7 não abre mais a mochila; itens sem uso nativo ficam desequipados em vez de disparar a arma anterior.
- **Áudio M4:** os WAVs preservados `wq_m4_1_gunshot outdoor loop` e `... loop end` são tocados separadamente; o `SoundCfg` da versão 7.3 referencia `gunshot_m4a1_near.ab`, que não foi localizado nos arquivos do XAPK. **Não há comprovação de que o WAV seja a mesma amostra usada no evento do APK.**
- **Passos, pulo e pouso:** os IDs de eventos da versão original estão documentados em [auditoria de áudio](docs/ORIGINAL_AUDIO_AUDIT.md), mas seus AudioClips não estão no XAPK preservado. Os sons executados pelo protótipo são temporários sintetizados, não originais.
- **Texturas:** são os seis atlas de albedo extraídos e copiados para renderização opaca; o Unity usa informações adicionais de material e mapas normais. A aparência de iluminação no Raylib ainda **não é equivalente 1:1** à original.
- **Validação:** CI verifica empacotamento e assinatura do APK Android ARM64, não fidelidade visual/som em dispositivo.

## Estado e limites

- **Implementado:** APK nativo ARM64, cenário 3D, itens, machado/M4, câmera, 33 clipes importados, toque por dedo e áudio original de M4/machado/coleta.
- **HUD:** sprites e hitboxes do `battlepanel.ab` para machado/M4; controles isolados para não atacar ao tocar em outros lugares. Mochila, craft, configurações e chat ainda são painéis provisórios, não reconstruções completas.
- **Personagem e áudio:** as cópias de textura para Android corrigem a opacidade dos canais alfa de materiais Unity, sem modificar os PNGs originais. Passos/salto/pouso usam sons sintetizados temporários; os clipes originais dessas ações ainda não foram extraídos.
- **Teste:** [compilação Android](https://github.com/48348484488/last-Island-of-survival-lite/actions/workflows/build_android_raylib_lite.yml) e [auditoria do HUD](https://github.com/48348484488/last-Island-of-survival-lite/actions/workflows/test_mobile_original_hud_export.yml).

**Observação:** os assets e o jogo original pertencem aos respectivos titulares; a presença dos arquivos neste repositório não concede direitos de redistribuição comercial.
