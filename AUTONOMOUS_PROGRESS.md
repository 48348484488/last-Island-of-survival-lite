## Build com validação virtual integrada — 2026-10-30 01:35 (America/Recife)

- No [workflow #41003434473](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/41003434473), o job `android-arm64` **passou**, incluindo APK, recursos e testes nativos. O job `Attempt actual Android virtual ARM64 runtime` foi efetivamente iniciado no mesmo workflow (diferentemente dos eventos separados `BuildFailed`), mas a conclusão ainda estava pendente nesta consulta.
- O job de publicação direta da mesma execução falhou com **HTTP 403 Resource not accessible by integration** na API de releases. O APK do commit `630b084` existe como artefato do build; **não há release direta confirmada para este commit**. A release anterior `v0.25.0-b5473200` permanece disponível.
- A falha HTTP 403 de publicação não é falha de compilação nem crash do jogo. Não declarar execução Android ou screenshot antes do job virtual terminar.
- Próxima prioridade: inspecionar resultado e evidência do job virtual, identificar suporte ARM64 do emulador, corrigir publicação se permissões permitirem e comparar screenshots reais.

## Evidência binária e execução virtual — 2026-10-30 01:34 (America/Recife)

- APK `b5473200` foi baixado do artefato verificado, extraído e comparado com `a290f59a`: `libraymob.so` ARM64 mudou de **3.525.008** para **3.525.136 bytes** (+128), SHA-256 anterior `fef5ef63...` e novo `8c4afd0f...`. Auditoria estática registrou 616.657 → 616.689 instruções e 25.188 → 25.187 chamadas; relatório [EdgeAI](https://github.com/48348484488/EdgeAI-Engine/blob/main/docs/last_recreated_arm64_build_diff_2026-10-30.json). Isso comprova diferença binária, **não prova execução nem equivalência funcional**.
- Como o workflow virtual independente recebeu eventos `BuildFailed/startup_failure` sem jobs, o teste foi integrado como job dependente do **mesmo workflow de build Android** no commit [630b084](https://github.com/48348484488/last-Island-of-survival-lite/commit/630b084cd0bcb36cc00927423ea22bc57c94847a). O workflow separado ficou apenas para execução manual, commit [1b1baeb](https://github.com/48348484488/last-Island-of-survival-lite/commit/1b1baebcf4db728f407062819399c05e39e16015).
- Build com novo job virtual [#41003434473](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/41003434473) ainda estava compilando na consulta. **Nenhum screenshot real ou instalação virtual confirmado até aqui**.
- Próxima prioridade: obter conclusão do job virtual e, se o emulador x86 não tiver tradução ARM64, migrar a validação para Android virtual nativo ARM64; depois comparar o mesmo estado da interface com o original LITE 7.3.

## Resultados adicionais — 2026-10-30 01:30 (America/Recife)

- **Build ARM64 [#41002903306](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/41002903306) aprovado integralmente**, incluindo regressão C++, verificação de assinatura/estrutura APK e publicação. Release [v0.25.0-b5473200](https://github.com/48348484488/last-Island-of-survival-lite/releases/tag/v0.25.0-b5473200), APK de **25.013.758 bytes**. Compilação aprovada **não significa execução Android comprovada**.
- Workflow virtual ARM64 criado e reforçado para capturar screenshot/logcat antes de checar processo; commit [dbe6ffa](https://github.com/48348484488/last-Island-of-survival-lite/commit/dbe6ffa5a3447b69975598f11d01a112ad2946b9). Eventos `workflow_run` geraram `startup_failure` antes de criar jobs, com `name=""`, `path="BuildFailed"`, `jobs=0` ([#41003151840](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/41003151840)). **Não foi possível instalar ou executar o APK em Android virtual**, nem capturar screenshot. Causa da falha de inicialização externa não comprovada; não confundir com crash do jogo.
- EdgeAI auditou estaticamente `libraymob.so` de APK anterior `a290f59a`, com 25.188 chamadas detectadas e 674 indiretas; relatório [aqui](https://github.com/48348484488/EdgeAI-Engine/blob/main/docs/last_recreated_arm64_callgraph_2026-10-30.json). Não é auditoria do original nem prova de fidelidade.
- Próxima prioridade: diagnosticar/contornar falha pré-job do GitHub Actions virtual sem mascará-la; executar APK num ambiente virtual ARM64 autorizado, capturar screenshot e comparar o mesmo estado com Unity LITE 7.3.

## Continuação — 2026-10-30 01:25–01:30 (America/Recife), conversão segura de coordenadas

- Corrigido `native_build_craft.hpp`: `grid(float)` não converte NaN, infinito ou coordenadas enormes para `int` (comportamento indefinido); devolve sentinela fora da área permitida. `valid()` evita `std::abs(INT_MIN)` (overflow). Commit [b06e6b9](https://github.com/48348484488/last-Island-of-survival-lite/commit/b06e6b99019c0b2f49d2419a5e4b9a01eba21697).
- Adicionadas regressões C++ para conversão não finita, limites de mundo e INT_MIN, além de impedir colocação e gasto de materiais com posições inválidas. Commit [b547320](https://github.com/48348484488/last-Island-of-survival-lite/commit/b54732000b60fe0c31d682a1bf955a7cd56e0453). A etapa `Native crafting and base-building rule regression` do build [#41002903306](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/41002903306) **passou**; compilação APK ainda em execução na primeira consulta.
- Criado workflow [`last-android-virtual-smoke.yml`](https://github.com/48348484488/last-Island-of-survival-lite/blob/main/.github/workflows/last-android-virtual-smoke.yml), commit [b35504a](https://github.com/48348484488/last-Island-of-survival-lite/commit/b35504a63e328ef9682a9d4c8a8936c696240297): tenta instalar o APK verificado no emulador Android e comprova presença de tradução ARM64 antes de declarar execução; preserva logcat, screenshot e diagnóstico, inclusive na falha. **A execução virtual ainda não foi comprovada.** O workflow roda após builds concluídos com sucesso ou por disparo manual.
- Pesquisa pública da versão LITE 6.7 não comprova identidade com a referência Unity LITE 7.3. Sem screenshots do mesmo estado, não afirmar equivalência de HUD, materiais, sons ou animações.
- Próxima prioridade: confirmar o APK do build #41002903306, verificar o workflow virtual e obter capturas reais; se o emulador não oferecer ARM64, registrar essa limitação e buscar ambiente virtual ARM64 suportado.

## Continuação — 2026-10-30 (America/Recife), validação de coordenadas 3D

- Corrigido o sistema offline de construção em `last_island_recreated_game/native_build_craft.hpp`: `valid()` rejeita coordenadas não finitas do jogador (`NaN`, `Inf`) antes de calcular a distância; `blocks()` rejeita consultas inválidas. Evita que falhas de física/câmera aceitem peças indevidas. Commit [7f3c2ba](https://github.com/48348484488/last-Island-of-survival-lite/commit/7f3c2baa2122ca84d024dbc862a7d19028d3fcc2).
- Adicionadas regressões C++ para impedir criação de peças e gasto de recursos com `NaN`/`Inf`, commit [a290f59](https://github.com/48348484488/last-Island-of-survival-lite/commit/a290f59a404166011b94cb624924b1c44d9c1e81).
- Build Android ARM64 do código corrigido: [#40996851203](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40996851203) **concluído com sucesso**, incluindo publicação da release [v0.25.0-a290f59a](https://github.com/48348484488/last-Island-of-survival-lite/releases/tag/v0.25.0-a290f59a). APK com 25.013.630 bytes. **Sem instalação ou execução Android virtual comprovada.**
- Pipeline de HUD construção [#40996840694](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40996840694) aprovado. Compilação APK para o commit `a290f59`: [#40996851203](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40996851203) iniciada; **resultado ainda pendente neste registro**. Não declarar APK novo como aprovado antes do workflow.
- Pesquisa pública consultou página oficial da App Store e referências de LITE 6.7; nenhuma prova de que correspondam ao APK Unity LITE 7.3. Nenhuma comparação visual, screenshot de execução Android virtual ou teste de som foi realizada neste ciclo.
- Próxima prioridade: verificar build ARM64, instalar APK em Android virtual quando disponível e capturar imagem real de fabricação em 1920×1080; comparar somente com captura autorizada da mesma versão/estado. Corrigir diferenças comprovadas em vez de inferir fidelidade a partir de posições.

## Continuação — 2026-10-29, ciclo seguinte (America/Recife)

### Verificação final da compilação deste ciclo

- O job Android ARM64 [#40989774432](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40989774432) concluiu a compilação **com sucesso**: `BUILD SUCCESSFUL in 1m 5s`, APK de **25.013.534 bytes**, estrutura de APK e assinatura v2 verificadas, 89 recursos empacotados. O teste nativo de construção/fabricação e os **4/4 testes de geometria** passaram.
- Publicação de APK **aprovada** no mesmo workflow: [release v0.25.0-cac03782](https://github.com/48348484488/last-Island-of-survival-lite/releases/tag/v0.25.0-cac03782), [APK ARM64](https://github.com/48348484488/last-Island-of-survival-lite/releases/download/v0.25.0-cac03782/LastIslandLite-v0.25.0-cac03782-ARM64.apk). Publicação não implica instalação/execução.
- Artefato do APK gerado: [GitHub Actions artifact #12948442605](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40989774432/artifacts/12948442605). A publicação de release é um job separado; não confundir artefato compilado com execução em dispositivo.
- **Não foi instalado ou executado em Android virtual** neste ciclo; não há captura de tela real nem prova de equivalência com a UI original.


- Correção funcional publicada: [46fab875](https://github.com/48348484488/last-Island-of-survival-lite/commit/46fab875d42460d454e786c37ac6b9ecc4548664) alinha os botões de quantidade (menos, mais, máximo) aos RectTransforms do Unity LITE 7.3: y=954.3, não y=741. Renderização e toque agora compartilham os mesmos `sourceRect`. O incremento respeita o máximo offline calculado da receita, não um limite fixo de oito.
- A ação `FABRICAR OFFLINE` permanece uma aproximação nativa: sua posição foi movida para x=1510, y=912, w=237, h=72, eliminando a interseção com a área clicável `QuantityMax` (que termina em x=1486.8). **Não afirmar que esse botão usa a posição original**.
- Regressões de geometria adicionadas em [0128281](https://github.com/48348484488/last-Island-of-survival-lite/commit/0128281b566738e21377d79855d6f7724178444b) e regex corrigida em [cac0378](https://github.com/48348484488/last-Island-of-survival-lite/commit/cac03782a92f5e158b76d9653f8ffd33569be45b).
- Workflow Android ARM64 [#40989774432](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40989774432): testes nativos e de geometria **aprovados**; etapa de compilação APK iniciada. Resultado final e release ainda pendentes no momento desta anotação.
- O build intermediário [#40989760191](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40989760191) falhou nos testes por erro de escape na regex; o erro foi corrigido antes do build mais recente. Não confundir falha intermediária com travamento.
- Referências públicas consultadas: App Store oficial e páginas de Last Island LITE, mas nenhuma captura comprovadamente da versão Unity LITE 7.3 foi obtida. A comparação visual com o original e a execução no Android virtual ARM64 continuam **não verificadas**.
- Próxima prioridade: concluir/inspecionar build #40989774432; instalar APK em Android virtual ARM64 quando disponível, obter screenshot real do menu de fabricação e comparar em 1920x1080 com o original. Corrigir a maior diferença demonstrada, sem inferir igualdade visual a partir de coordenadas isoladas.

# AUTONOMOUS_PROGRESS.md

## Estado em 2026-10-29 (America/Recife)

### Último trabalho concluído
- Commit [efe9e2e](https://github.com/48348484488/last-Island-of-survival-lite/commit/efe9e2e09193bfb94705cecc8cd7b85a34535879): corrigida a divergência de 50 px entre o desenho e a área clicável do botão Voltar (316 px na origem Unity, 266 px na implementação anterior).
- Subcategorias de fabricação agora usam o passo de 145 px definido no `mobile_original_craft_layout.hpp`, em vez dos 124 px provisórios. Desenho e toque compartilham a mesma função de retângulo.
- Fila de fabricação **offline** agora distribui o delta de tempo entre pedidos consecutivos, sem descartar frames maiores que 0,1 s ou 1 s. Deltas NaN e negativos são ignorados.
- Adicionados testes de geometria estática e regressões C++ de fila; incluídos no workflow Android.

### Testes e execução
- [Workflow de HUD do commit](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40987298880): concluído com sucesso.
- [Build Android ARM64 do commit](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40987477840): **sucesso** (compilação C++/Gradle, testes nativos, testes de geometria, verificação do APK e publicação da release). A primeira tentativa #40987298840 falhou por omissão do arquivo de teste no sparse checkout; corrigida em [2c76283](https://github.com/48348484488/last-Island-of-survival-lite/commit/2c762836c832c4ab7eb0a4ffaa7162d3348de843).
- APK ARM64 compilado e publicado: [v0.25.0-2c762836](https://github.com/48348484488/last-Island-of-survival-lite/releases/tag/v0.25.0-2c762836), 25.013.486 bytes. **Instalação/execução do APK no Android virtual, captura de screenshots e comparação visual com o Unity original ainda não foram comprovadas** nesta execução.
- EdgeAI recebeu auditor de retângulos reutilizável em [EdgeAI-Engine](https://github.com/48348484488/EdgeAI-Engine/tree/main/tools), para distinguir diferença geométrica de fidelidade visual completa.

### Referências e limitações
- Referência primária: Unity LITE 7.3 `assets/ui/bagandbuildpanel.ab` e os limites extraídos em `mobile_original_craft_layout.hpp`.
- Catálogo original de 519 receitas e 512 ícones permanece integrado pelo workflow V25; produção em rede, níveis, materiais e tempo de servidor **não** são equivalentes ao original comprovadamente.
- Vídeos de gameplay públicos de outras versões servem apenas de contexto, não de prova de identidade do APK LITE 7.3.
- Divergência geométrica específica pendente: `mobile_original_craft_layout.hpp` registra QuantityMinus/Plus/Max na faixa y=954.3, enquanto o catálogo nativo ainda desenha controles de quantidade em y=741. A correção precisa considerar sobreposição com o botão de fabricar e ser comparada com captura real, sem deslocar controles por suposição.
- Restam diferenças em HUD de construção, fontes, textura, câmera, animações, sons, controles e gameplay; sem comparação de screenshots do mesmo estado, nenhuma igualdade deve ser afirmada.

### Próxima prioridade
1. Baixar o APK ARM64 da release #2c762836 e instalar/executar no Android virtual autorizado, coletando logcat e screenshots.
2. Instalar APK no Android virtual ARM64 se disponível e registrar screenshots reais e logs.
3. Capturar mesma tela no original autorizado e medir deltas geométricos/visuais usando EdgeAI.
4. Corrigir a maior diferença comprovada, repetir testes e registrar novo commit.
