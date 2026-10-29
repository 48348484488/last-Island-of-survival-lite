# LITE V25 — Catálogo real de fabricação e modo Android

## Evidência diretamente extraída do XAPK original 7.3

- `assets/cfg/cfg.DrawingCfg.oc` (71.773 bytes): **519 registros decodificados**, incluindo identificador, tipos, ordem, materiais e quantidades, `targetItemId` e `needTime`. O formato binário real possui diferenças em relação ao `DrawingCfg.cs` descompilado; campos não resolvidos ficam preservados como valores brutos. Teste: [Actions #40910102539](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40910102539).
- `assets/cfg/cfg.ItemCfg.oc`: **542 de 543** IDs referenciados foram associados a prefixos únicos de item com nome e caminho de ícone; o decodificador de registros completos permanece incompleto. O item não identificado continua sem substituição inventada.
- **512 de 512** ícones únicos requisitados encontrados por caminho de bundle, nome exato de Sprite e PathID, distribuídos entre as famílias `tubiao`, `gun`, `buildpart`, `zaiju`, `equip` e outras. [Actions #40910516838](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40910516838).
- `original_craft_catalog_v73.hpp` possui 519 receitas, 1.406 linhas de ingredientes, e 4 atlas PNG de sprites originais (128 px por célula), empacotados e validados. [Actions #40910945599](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40910945599).

## Integração Android

`mobile_real_craft_catalog.hpp` é o novo painel principal de receitas. Permite abrir categorias e subcategorias, percorrer páginas, selecionar receitas, inspecionar os ingredientes e identificar as imagens originais. Usa a hierarquia Unity já recuperada para as áreas verificadas, enquanto o fluxo e o posicionamento do grid paginado permanecem uma adaptação nativa.

O modo de teste antigo, com apenas três receitas demonstrativas, foi **separado** e pode ser aberto pelo botão `TESTE OFFLINE`. Ele mantém a possibilidade de testar a mecânica de construção anterior, sem se confundir com o catálogo extraído.

Há uma fila offline de produção para uma seleção restrita de receitas básicas, usando custos e tempo decodificados. Consome madeira (ID 50001) e pedra (ID 50006) dos recursos nativos e suporta ingredientes já produzidos na própria simulação. Itens concluídos ficam em inventário de teste, não equipados ou posicionados automaticamente no mundo. Receitas dependentes de desbloqueios, bancada, servidor ou itens ainda não representados não devem ser anunciadas como plenamente jogáveis.

## Limites para uma paridade real 1:1

1. A lista paginada 4x3, textos e estados de seleção **não** reproduzem ainda os cálculos Unity de `UIScrollPanel`, `RectTransform` e `LayoutGroup` em tempo real.
2. Textos originais do `ItemCfg` são majoritariamente chineses; enquanto a fonte/localização original não for restaurada, a versão Android identifica receitas pelos IDs originais em vez de criar traduções não verificadas.
3. Os 116 registros que usam `bigType=18` estão agrupados em categorias de estrutura e decoração para inspeção; a apresentação original desse grupo não está comprovada pelo `BagProducePage` encontrado.
4. Falta testar visualmente o APK reconstruído contra uma execução do mesmo XAPK original, incluindo gestos, escala Android e tamanho de fonte.
5. A produção Unity original pode depender de estado multiplayer, fila de rede e inventário. A fila nativa é uma **simulação offline** e não reivindica comportamento idêntico.

A versão 25 não deve ser descrita como **100% idêntica** enquanto esses pontos não forem validados.
