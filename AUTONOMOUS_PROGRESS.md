## Continuação — 2026-10-29, ciclo seguinte (America/Recife)

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
