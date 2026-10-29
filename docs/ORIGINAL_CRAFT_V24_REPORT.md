# LITE V24 — análise da tela de fabricação a partir do prefab original

Origem: original LITE 7.3 (XAPK em 17 partes), `assets/ui/bagandbuildpanel.ab`.
Os dados abaixo não vêm de uma imagem gerada nem de um menu semelhante.

## Evidência de extração

Ações aprovadas:
- [Extração do prefab Unity e 73 sprites](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40904727522).
- [Recuperação de dependências CAB e 80 sprites](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40906450676).

Foram indexados **1685 GameObjects**, **1651 RectTransforms** e **1823 componentes UGUI**.
O primeiro processo recuperava 73 sprites e deixava 17 referências não resolvidas. A busca
pelos GUID CAB de `SerializedFile.externals` recuperou mais **7** sprites diretamente
dos AssetBundles a que o prefab apontava; ainda restam **10** referências a resolver.

O mapeamento do menu provém de `SC.UI.BagProducePage.OnInit`, sem adivinhar
os grupos: Básico, Itens, Estruturas, Combate e Veículos, com os subtipos
1,2,3,4,5,6,7,8,9,10,15 e 16 efetivamente presentes.

### Peças visuais identificadas por PathID original

| Elemento | PathID Unity | Sprite verificado | AssetBundle |
| --- | --- | --- | --- |
| Aba principal On/Off | `8081054173552591411` | `ShopOutBattlePanel_zc_an_1` | `assets/uiatlas/shopoutbattlepanel.ab` |
| Indicador da aba | `-2981005624660488701` | `ShopOutBattlePanel_zc_an_1_on` | `assets/uiatlas/shopoutbattlepanel.ab` |
| Aba secundária | `-6746909455405468362` | `BagAndBuildPanel_bg_23` | `assets/uiatlas/bagandbuildpanel.ab` |
| Fabricar | `-6604354997382583515` | `com_anniu_1` | `assets/uiatlas/common.ab` |
| Voltar | `-8671269052031048657` | `com_ty_gb_1` | `assets/uiatlas/common.ab` |

O código nativo carrega sprites originais das abas quando disponíveis no pacote.
A referência de 1920x1080 de parte do layout foi copiada para
`last_island_recreated_game/mobile_original_craft_layout.hpp`, e os eventos
do `main.cpp` usam os mesmos limites dos botões mostrados no
`mobile_craft_ui.hpp`.

## Separação entre dados verificados e aproximações

**Comprovado estaticamente:** sprite original, identificador, arquivo de origem,
hierarquia do prefab, ancoragem e tamanho serializado de botões fixos, callbacks
C# de categorias, sequência de filas, controle de quantidade e cancelamento.

**Ainda NÃO comprovado:** renderização idêntica pixel a pixel, layout dinâmico
dos cinco botões primários (o Unity utiliza LayoutGroup), fonte/idioma exatos,
a base de receitas `cfg.DrawingCfg`, os ícones `cfg.ItemCfg`, os estados
em tempo real e a rede/servidor. A posição estática de `btn_create` no
prefab aponta para fora do canvas antes da atualização runtime, então a
recriação usa ali um botão funcional de dimensões compatíveis sem afirmar
posição exata. Os três itens de fabricação continuam **demonstrações offline**,
não o catálogo original.

Testes de código e compilação não substituem confronto com captura real do
Unity original na mesma resolução/estado. A meta continua paridade de pixels
e comportamento, ainda não atingida.
