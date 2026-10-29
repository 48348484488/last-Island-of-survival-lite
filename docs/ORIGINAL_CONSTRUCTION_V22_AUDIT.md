# V22 — auditoria de construção original LITE 7.3

O objetivo da engenharia reversa é reconstituir o `BattlePanel` original e as peças reais de base do Unity, em vez de declarar idêntico um conjunto de cubos com cores artificiais.

## Origens verificadas

Os seis albedos são `Texture2D` do XAPK original, nas dependências `assets/partinfo_material/*.ab`:

| Peça | Bundle original | Texture2D original |
|---|---|---|
| Fundação | `component_diji_lv1.ab` | `component_diji_lv1_d` |
| Parede | `component_qiangbi_lv1.ab` | `component_qiangbi_lv1_d` |
| Piso/cobertura | `component_wuding_lv1.ab` | `component_wuding_lv1_d` |
| Janela | `component_chuanghu_lv1.ab` | `component_chuanghu_lv1_d` |
| Batente de porta | `component_menkuang_lv1_d.ab` | `component_menkuang_lv1_d` |
| Escada | `component_louti_d.ab` | `component_louti_d` |

Extraídos por `tools/export_original_build_surfaces.py`, trabalho aprovado em [GitHub Actions #40815593060](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40815593060). Os originais RGBA ficam intactos no APK com prefixo `build_original_`. Janela e batente possuem alfa de máscara de shader muito baixo; para o renderizador nativo foram criadas **cópias** com RGB idêntico e opacidade total, `build_native_opaque_window.png` e `build_native_opaque_doorway.png`. A cópia não modifica o arquivo original.

Os ícones de peças da lista são recuperados dos `assets/icon/buildpart*.ab`; o pipeline `tools/audit_original_build_part_icons.py` extraiu os ícones de 17 bundles usando os PathIDs Unity, preservando os nomes. A correspondência exata com `cfg.BuildPart.icon` para **todas** as peças depende do arquivo de configuração `cfg.BuildPart.oc`, ainda não recuperado.

## Menu original

Decompilação de `ExportedProject/Assets/MonoScript/Assembly-CSharp/SC/UI/BattlePanel.cs`:

- `OnClickFundation` → `FillBuildChildsIcons(2)`
- `OnClickWall` → `FillBuildChildsIcons(3)`
- `OnClickStair` → `FillBuildChildsIcons(4)`
- `OnClickFloor` → `FillBuildChildsIcons(8)`
- `OnClickWindow` → `FillBuildChildsIcons(6)`
- `OnClickObstacle` → `FillBuildChildsIcons(7)`
- `FillBuildChildsIcons` popula `m_buildChilds` com `BuildPart.icon` e `BuildPart.name`, e só após o clique em uma peça chama `BuilderBehaviour.ChangePrefab`.
- `btn_null` abre/fecha `scp_bagBuilds`, não sai do BuildState.
- `btn_changeButton` entra/sai do modo de construção.
- `btn_opRotate` é um botão condicional de operação; não fica sempre visível no menu principal.

O exportador `tools/export_mobile_original_hud.py` reconstrói as 11 posições de `m_buildChilds` e recupera seus toggles originais em `hud_original_build_children.png`. A verificação do HUD passou em [Actions #40816196425](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40816196425).

O motor Android agora exibe a segunda etapa de escolha da peça; antes selecionava parede/fundação automaticamente no clique da categoria. O status dos controles foi corrigido para respeitar o tipo de interação. O menu não deve desenhar operações condicionais sempre sobrepostas.

## O que NÃO está igual ao Unity ainda

As malhas e UVs dos módulos de parede, janela, telhado e escada **ainda são geométricas temporárias**: o original é servido por `cfg.BuildPart.model` via `ResMgr.CreateFromAB` e a correspondência completa com malhas e configurações está ausente da extração local. O resultado atual tem texturas originais RGB com geometria aproximada. A prévia/validação de encaixe, upgrades de nível, durabilidade, reparos e custo original dos módulos também exigem as configurações `cfg.BuildPart.oc` e implementação posterior; não equivalem ao runtime Unity. Não afirmar fidelidade visual perfeita nem funcionamento de todas as operações de servidor.

O APK é compilado em GitHub Actions, com verificações de presença de materiais, sprites, layout e código ARM64. O sucesso da compilação não demonstra equivalência visual no aparelho.
