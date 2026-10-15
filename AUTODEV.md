# Oxide AutoDev — metas sem Codex e sem API paga

Objetivo: executar o **Oxide original v1.15.11966** em Android ARM64 inteiramente virtual, manter a Activity original ativa por **35 segundos**, confirmar menu/gameplay e só então validar um overlay separado com dados sinteticos. Nenhum APK original e alterado e nenhuma verificacao de seguranca e contornada.

## Metas com prova obrigatoria

| Meta | Evidencia exigida |
| --- | --- |
| 1. Android ARM64 | `sys.boot_completed=1`, ABI `arm64-v8a` |
| 2. Abrir jogo | Quatro APKs originais, instalacao `Success`, inicio da `OxideActivity` e seu PID |
| 3. 35 segundos | **Mesmo PID inicial** e **Activity original retomada** em t=10 e t=35 |
| 4. Menu e gameplay | Revisao independente das imagens e interacao; PID sozinho nao prova menu |
| 5. Overlay sintetico | Dados de teste independentes, sem injecao no jogo |

O progresso atualizado e os logs estao na [issue #4](https://github.com/48348484488/oxide/issues/4). O catalogo verificavel fica em [goals/oxide-goals.json](goals/oxide-goals.json).

## Cadeia automatica por evento

1. O [runtime](.github/workflows/oxide-115-arm64-runtime.yml) tenta iniciar Android ARM64 e testa o APK original.
2. Ao **terminar**, o [supervisor](.github/workflows/oxide-event-supervisor.yml) recebe `workflow_run.completed` e dispara a analise uma unica vez para aquele run ID.
3. A [analise de metas](.github/workflows/oxide-goal-forensics.yml) baixa as provas do run original, executa [o motor de criterios](tools/oxide_goal_engine.py), publica o resultado e salva `goals.json`/ `goals.md`.
4. Se a **Activity original do Android 15 se autointerromper com SIGKILL**, uma unica comparacao autorizada no Android 16 ARM64 e iniciada automaticamente. Ela usa ashmem real do host com fonte fixada por SHA, sem alteracao do APK.
5. Se nao houver correcao segura predefinida, **interrompe os retries** e registra o bloqueio. Nao existe agente de IA independente capaz de inventar correcoes ilimitadas sem outro servico.

Este fluxo e dirigido **pelo termino do trabalho**, nao por verificacoes a cada hora. A automacao do ChatGPT que revisa o repositorio periodicamente e complementar.

## Limites e custos

- Nao usa Codex nem API paga.
- Usa GitHub Actions: **minutos e armazenamento podem ser cobrados** se a franquia gratuita do repositorio for excedida. Nao ha garantia de custo zero ilimitado.
- Nao reinicia indefinidamente o mesmo teste; a comparacao Android 16 e limitada a uma tentativa.
- `GITHUB_TOKEN` disponibiliza as permissoes minimas de leitura de artefatos e escrita de comentarios/workflow_dispatch; nenhuma chave de IA e necessaria.
- Continua dependendo de um ambiente Android nativo virtual com suporte Binder e, no Android 16, ashmem.

## Como verificar

- [Actions — execucoes reais](https://github.com/48348484488/oxide/actions)
- [Issue #4 — criterios e logs](https://github.com/48348484488/oxide/issues/4)
- [Testes de metas](tests/test_oxide_goal_engine.py)

O ultimo diagnostico conhecido do Android 15 observa o primeiro PID encerrando pouco apos inicializar o Unity, e um **processo diferente** em t=10/35. Nao marcar jogo ou ESP como funcionando ate possuir provas adicionais.
