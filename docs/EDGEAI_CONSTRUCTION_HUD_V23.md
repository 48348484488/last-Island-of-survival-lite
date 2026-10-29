# LITE V23 — HUD de construção auditado com EdgeAI

**Estado:** versão Android ARM64 compilada e publicada, não existe prova de fidelidade completa do HUD Unity. APK: [v0.23.0](https://github.com/48348484488/last-Island-of-survival-lite/releases/download/v0.23.0/LastIslandLite-v0.23.0-ARM64.apk). Build aprovado: [Actions #40841951821](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40841951821).

## Engenharia reversa real

Em vez de classificar imagens pelo nome aproximado, usamos o binário **original** `com.herogame.gplay.survival.lite.apk:assets/cfg/cfg.BuildPart.oc` (138485 bytes). O decodificador do projeto EdgeAI reconstituiu 638 registros com IDs e liga cada `BuildPart.icon` ao Sprite do exato `assets/icon/buildpart*.ab`, com PathID original.

O `BuildManager.GetBasicPartByType` original filtra `basic && lv==1`, preservando a ordem do arquivo (não ordenação alfabética ou pelos IDs). Assim, são 35 itens em seis categorias:

| Unity BuildPart.type | Categoria | Itens |
|---|---|---:|
| 2 | Fundação | 4 |
| 3 | Parede | 9 |
| 4 | Escada | 3 |
| 6 | Janela / porta | 8 |
| 7 | Obstáculo | 3 |
| 8 | Piso / teto | 8 |

**Resultado:** seis atlas `hud_original_build_items_type_N.png`, 35 sprites originais, `original_buildpart_v7.hpp` com ID, tipo, índice, sprite, prefab e `OnceAngle`. Artifacts e evidências do [Actions #40841337262](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40841337262). A importação no APK é verificada pelo CI.

O `tools/edgeai_unity_ui_behavior_audit.py` inspeciona os callbacks registrados em `BattlePanel.cs`, e o [Actions #40838822697](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40838822697) confirmou 14 controles de construção e diagnosticou divergências da V22, inclusive a ação inventada de salvar projeto atribuída ao `btn_lantu`. A ação fictícia foi removida. O menu Android agora mantém as identidades das peças em vez de confundir meia parede com batente de porta.

## Comportamento de seleção recuperado

1. `btn_fundation`, `btn_wall`, `btn_stair`, `btn_floor`, `btn_window` e `btn_obstacle` abrem a segunda lista `m_buildChilds`.
2. O atlas correto usa a ordenação da lista **original**, inclusive as diferentes variantes de triângulo, teto, porta, cerca e arco.
3. O clique no subitem recupera o `BuildPart.id` original e seu `OnceAngle`. Peças cuja geometria nativa está implementada mostram prévia e podem ser posicionadas; as demais são explicitamente identificadas como *modelo não portado*, nunca trocadas silenciosamente por outra peça.
4. Rotação usa `OnceAngle` quando compatível com o renderizador atual (múltiplos de 90°), sem afirmar que os casos de 120°/curvas são suportados.
5. `btn_null` abre/fecha a lista de itens de construção, e `btn_changeButton` sai do estado de construção.

## Ainda não é fiel ao original

- **Malhas e UVs:** `BuildPart.model` identifica prefabs `part/*.ab`, mas não há reprodução comprovada de todos os meshes. Geometria de cubos remanescentes é provisória.
- **Controles contextuais:** origem e semântica de `btn_opUp`, `btn_opMove`, `btn_opRotate`, `btn_opRepair`, `btn_opDestory` foram analisadas; persistência, redes, reparo, upgrades e ações sobre estruturas já posicionadas não têm paridade completa na recriação offline. O botão Girar para *prévia* está parcialmente implementado.
- **Textos, fontes e layout:** `CanvasScaler`, textos localizados, listas dinâmicas `m_buildBaseBtns` e pixels do Unity UGUI requerem validação visual de execução original vs APK em dispositivo/emulador. `m_buildChilds` utiliza os controles e sprites de origem, mas não está provada equivalência pixel a pixel.
- **Config 7.3:** o binário difere de parte dos campos C# descompilados, e a extensão de registros tem bytes mantidos com SHA-256, não decodificados por palpite.
- **Validação Android:** CI comprovou compilação ARM64 e presença dos recursos. Ainda não há ensaio de toque/pixel parity com a interface Unity original.

Esse documento distingue artefatos recuperados, botões efetivamente implementados e limitações, para evitar afirmar que uma interface ainda provisória é idêntica.
