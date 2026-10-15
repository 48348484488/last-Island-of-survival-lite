# Oxide: Survival Island (APK & Split Packages)

## Metas M1-M6 — acompanhamento automatizado (14/10/2026)

**Progresso real, não apenas planos.** [M1: correlacionar SIGKILL](../../issues/1) ·
[M2: regressão de inicialização](../../issues/2) ·
[M3: ambientes virtuais ARM64](../../issues/3) ·
[M4: Activity original ativa por 35s](../../issues/4) ·
[M5: overlay independente no mesmo Android](../../issues/5) ·
[M6: telemetria legítima e validação final](../../issues/6).

- **M1 — CONCLUÍDA:** [execução final #38640613860](../../actions/runs/38640613860) comprovou ao vivo `NSpid: 12281 2121` e a syscall real `kill(SIGKILL)` emitida pelo mesmo host PID **12281** contra seu PID Android **2121**, **4.455,974 ms** após mapear o processo pelo relógio `CLOCK_BOOTTIME`. O Android confirmou saída `SIGNALED`, status 9. Entre 12 eventos, um correspondeu ao jogo; **38 testes** (16 correlator + 12 namespace + 10 parser) passaram. Os critérios da [issue #1](../../issues/1) foram cumpridos e ela foi fechada. [Relatório técnico e artefato](reports/1.14.11949-original-android16-same-run-kernel-nspid-sigkill.md). **Não foi identificado quem pediu Activity.finish() nem a causa da proteção.**
- **M1 — nova prova de ordem de encerramento:** [auditoria #38617234603](../../actions/runs/38617234603) aprovou **15 testes** e cruzou em logs reais dos Androids 13/15/16 o PID da Activity, evento de finalização, autossinal SIGKILL e histórico Android `ApplicationExitInfo` status 9. O fechamento da Activity antecede o sinal por **45, 2 e 3 ms**, respectivamente. **Não se identificou quem solicitou o fechamento nem a causa do alerta**. [Relatório](reports/1.14.11949-original-activity-finish-sigkill-order.md).
- **M1 — prova simultanea no kernel e Android 16:** [teste real #38639519858](../../actions/runs/38639519858) aprovou **34 regressões específicas** e correlacionou a cadeia `NSpid: 12351 2120` (host Linux → guest Android) ao evento de kernel `kill(SIGKILL)` emitido pelo host PID **12351** para o guest PID **2120**, no **mesmo experimento**. O Android confirmou autossinal e saída status 9. Foram coletados 12 eventos, 1 atribuído estritamente ao jogo; os demais sinais não foram confundidos. [Relatório com artefatos e limites](reports/1.14.11949-original-android16-same-run-kernel-nspid-sigkill.md). A [execução posterior #38640613860](../../actions/runs/38640613860) também comprovou a janela temporal com relógio do kernel; causa de segurança desconhecida.
- **M2 — concluída:** o parser de sinais diferencia PID host/Android e falha fechado sem mapeamento. A [regressão #38601989458](../../actions/runs/38601989458) aprovou os testes, incluindo casos truncados, e reprocessou os seis eventos reais. O workflow roda semanalmente sem baixar o APK; [issue #2 encerrada](../../issues/2).
- **M3 — comparação de versões verificada:** [replay #38606700050](../../actions/runs/38606700050) validou a Activity original em Android ARM64 virtual 12 (PID 1829), 13 (2232) e 14 (1876). Os três apresentaram autossinal SIGKILL; [matriz reproduzível](reports/1.14.11949-m3-original-android12-13-14-matrix.md). A meta continua aberta até encontrar/validar outro ambiente virtual legítimo; trocar só a versão do Redroid não resolveu.
- **M3 — Android 15 executado realmente:** [boot comprovado #38610334731](../../actions/runs/38610334731), CPU ARM64 nativa e OpenGL ES 3.2 ANGLE/SwiftShader; [quatro APKs originais testados #38610951671](../../actions/runs/38610951671). A Activity PID **1983** enviou SIGKILL para si aos **+4.619 ms**; ausente aos 10/35 s. [Relatório detalhado](reports/1.14.11949-original-android15-native-arm64.md). Android 16 teve imagem ARM64 disponível, mas primeiro boot incompleto (`sys.boot_completed=0`); [diagnóstico novo](../../actions/runs/38611754822). Esses resultados não identificam a causa da rejeição de segurança.
- **M4 — origem do pedido de encerramento melhor delimitada:** [execução real Android16 #38645776428](../../actions/runs/38645776428) identificou a ultima transacao Binder do **processo original Oxide** ao `system_server`, codigo `15`, no **mesmo no de destino 11600** observado ao finalizar normalmente a Activity nativa de Configuracoes do Android (`app-request`). A transacao do jogo ocorreu **4,265 ms antes do autossinal SIGKILL**. O [replay independente #38646383777](../../actions/runs/38646383777) aprovou **18 testes**. [Relatorio](reports/1.14.11949-original-binder-close-candidate-android16.md). Isso corrobora um pedido de `finishActivity` originado no processo do jogo; **a classe/função e o motivo continuam desconhecidos, jogo ainda nao jogavel por 35s**.
- **M4 — task-removal agora distinguida de encerramento simples:** [replay dos arquivos reais #38664653490](../../actions/runs/38664653490) passou **27 testes** e comprovou no mesmo Android16 que o Settings comum fechou com `app-request` (task 8), enquanto o Oxide original PID **2161** pediu `finish-activity` (task 9) usando mesmo endpoint Binder `11600` / codigo `15`, enviou SIGKILL **4 ms depois** e teve a task 9 removida **501 ms apos o pedido**. Isso corrobora o ramo AOSP que remove a tarefa, sem observar o argumento `finishTask` nem identificar qual funcao/checagem interna do jogo provocou a chamada. [Relatorio e codigo](reports/1.14.11949-original-binder-close-candidate-android16.md). **Jogo ainda nao jogavel por 35 s.**
- **M4 — pilhas ARM64 reais em Binder e autossinal:** o [experimento original #38665464837](../../actions/runs/38665464837) registrou `ustack(raw)` do PID host **12475** / guest **2182** na transacao Binder final (codigo 15, no 11854) e no `SIGKILL`. A [auditoria corrigida #38665984982](../../actions/runs/38665984982) passou **32 testes** e associou **5 frames Binder** a `libc.so`, `libbinder.so`, `libandroid_runtime.so` e `boot-framework.oat`, e **1 frame SIGKILL** a `libc.so`. Os offsets exatos e limites estao no [relatorio](reports/1.14.11949-original-arm64-binder-sigkill-userstacks.md). **Nao foi recuperada a funcao interna nem a causa do alerta; Android continua saindo no loading.**
- **M4 — reprodutibilidade comprovada:** os mesmos offsets dos **5 frames do caminho Binder Android** e **1 frame `libc.so` no SIGKILL** apareceram em tres testes reais Android16, apesar de PIDs e nos Binder diferentes: [#38665464837](../../actions/runs/38665464837) (replay corrigido), [#38665902829](../../actions/runs/38665902829) e [#38665926223](../../actions/runs/38665926223) (ambos completos e aprovados). [Tabela e evidencias brutas](reports/1.14.11949-original-arm64-binder-sigkill-userstacks.md). **Origem interna da decisao continua desconhecida.**
- **M4–M6:** ainda não concluídas. **Não há confirmação de menu jogável, gameplay, ESP real ou causa da proteção.** Somente ambientes virtuais; APK original preservado.

**Pipeline automático validado (14/10):** sinal ARM64 [#38607509257](../../actions/runs/38607509257) → replay de SIGKILL [#38607625417](../../actions/runs/38607625417) → marcos da Activity [#38607658290](../../actions/runs/38607658290) → matriz Android 12–14 [#38607691912](../../actions/runs/38607691912). As quatro execuções terminaram com sucesso, e a sequência foi acionada sem intervenção entre os jobs. São auditorias de evidência existente; **não** corrigem o encerramento do jogo, não executam código novo autonomamente e não comprovam gameplay. O supervisor separado registra falhas e possíveis travamentos.

**M3 — Android 16 ARM64 nativo foi destravado:** o host GitHub ARM64 kernel 6.17 não tinha `/dev/ashmem`. A revisão fixa do módulo upstream ([compilação #38613318778](../../actions/runs/38613318778)) foi carregada apenas no executor descartável e o Android 16 original iniciou normalmente: `sys.boot_completed=1`, `zygote` e `surfaceflinger` ativos ([boot #38613931053](../../actions/runs/38613931053)). A [execução real com os quatro APKs originais #38614441183](../../actions/runs/38614441183) comprovou PID 2099 e autossinal SIGKILL em **+4.556 ms**, ausente aos 10/35 s — **não há gameplay**. O módulo ainda provém de um PR upstream aberto; [relatório detalhado](reports/1.14.11949-original-android16-ashmem-arm64.md). A correção foi do boot do Android, não da decisão do Oxide de sair.

**M5 — Android 16 com jogo original + overlay no mesmo PNG:** [teste real #38625329989](../../actions/runs/38625329989) instalou os quatro splits originais e executou o app JNI independente na mesma tela virtual ARM64. O dialogo do Android "Viewing full screen" ocultava a caixa; apos corrigir o toque de "Got it", a [captura #38626740958](../../actions/runs/38626740958) registrou **caixa verde sintetica sobre a arte de loading do jogo**, com original Activity PID **2211** em primeiro plano. A [auditoria de arte/PID #38627748396](../../actions/runs/38627748396) passou **22 testes** e detectou **2.260 pixels especificos da caixa JNI**; a classificacao falha com background preto ou verde semelhante que nao forme a caixa. [Relatorio](reports/1.14.11949-original-loading-overlay-android16-verified.md). **Nao ha gameplay nem ESP real comprovado**; Oxide ainda fecha durante o loading.

**M5 — revalidado em Android 16 com quadros multiplos:** o [teste completo #38628601680](../../actions/runs/38628601680) concluiu **2 jobs reais aprovados** (build JNI ARM64 e original Oxide com quatro APKs intocados) e **23 testes**; tres capturas sobre o carregamento original registraram **2.260 / 2.416 / 2.411** pixels do retangulo verde (quadros 3/4/5), todos correlacionados com o PID da Activity. A auditoria evita falsos positivos por fundo preto e pela animacao normal do painel Android. [Relatorio reproduzivel](reports/1.14.11949-original-loading-overlay-android16-verified.md). **Ainda nao e ESP real nem partida jogavel.**

**M5 — overlay JNI independente no Android 15:** [teste real #38613786301](../../actions/runs/38613786301) validou 64 atualizações externas sintéticas e 67 snapshots aceitos no total, o mesmo PID do serviço, rejeições/replay, limpeza stale e remoção da janela. **Não houve dados reais do jogo**.

As issues contêm os critérios de aceitação e evidências. Desenvolvimento futuro e atualizações devem seguir esses critérios e registrar honestamente bloqueios, sem afirmar sucesso apenas porque a CI passou.

## Direção atual: execução 100% virtual, sem aparelhos físicos

### SIGKILL original comprovado tambem pelo kernel ARM64 virtual

A [execucao #38599474610](../../actions/runs/38599474610) observou o APK original inalterado no Redroid Android 13 ARM64 e capturou **a chamada real de `kill(SIGKILL)` no kernel**: emissor host PID **12895** (`esurvivalisland`), alvo PID Android **2232**, sinal **9**. O `logcat` do mesmo processo tambem registra o autossinal. A Activity encerrou em **16:37:45.101 UTC** e o sinal ocorreu em **16:37:45.146 UTC**; o jogo nao continuou aberto.

O preflight [#38599133014](../../actions/runs/38599133014) havia confirmado seis eventos controlados na instrumentacao. O kernel so recuperou `kill+8` na pilha: **a funcao chamadora e o motivo permanecem desconhecidos**. Nao se comprovou gameplay, ESP ou causa especifica de protecao. [Relatorio e evidencia](reports/1.14.11949-original-kernel-self-sigkill-arm64.md).

### Execução original repetida e comparação de três PIDs no Android ARM64 virtual

Na [reexecução real #38574879916](../../actions/runs/38574879916), os **quatro APKs originais** instalaram e o Oxide iniciou o Unity e o IL2CPP, mas o PID original **2168** registrou autossinal **SIGKILL** em **+4,648 segundos**, antes de um menu jogável. O workflow falhou intencionalmente o critério de jogo permanecer aberto; o artefato preserva screenshots e logcat originais.

O [replay CI #38579150920](../../actions/runs/38579150920) aprovou **10 testes de regressão** e comprovou a mesma sequência em três execuções originais ARM64: PID 2197 (+4,895 s), PID 2578 (+4,649 s), PID 2168 (+4,648 s). A análise distingue logs do PID do jogo de erros gráficos emitidos por processos Android diferentes. **Não identifica a regra de segurança que provoca a saída**, nem confirma Play Integrity, gameplay ou ESP real.

[Relatório dos três casos](reports/1.14.11949-original-3-virtual-launch-milestones.md) | [JSON do replay](../../actions/runs/38579150920/artifacts/11900654371)



### EdgeAI: tabela ARM64 real incorporada ao grafo do loader

A [execucao #38570970178](../../actions/runs/38570970178) concluiu **65 testes aprovados**. Usando o `libloader.so` ARM64 original em modo somente leitura, a EdgeAI leu uma tabela com **8 entradas** em `0x870B80` (indice local 0..7) e recuperou **6 destinos distintos** para o despacho `0x4EA1F8`. A ferramenta confirmou que os destinos estao em regioes executaveis, dentro do simbolo `JNI_OnLoad` e em limites de blocos basicos previamente identificados. O grafo passou de **174 para 180 arestas candidatas**; o numero de blocos alcançaveis no modelo parcial a partir da entrada continua em **4 de 147**.

A comparacao com LLVM na [execucao #38569211341](../../actions/runs/38569211341) confirmou **108/108 destinos diretos** de saltos/chamadas na `libloader.so`. As 6 novas arestas sao **candidatas por dados da tabela**: nao sao caminhos executados comprovados e nao recuperam os estados de jogador e camera. [Relatorio tecnico](reports/1.14.11949-edgeai-bounded-jumptable.md) | [JSON e evidencias](../../actions/runs/38570970178/artifacts/11898425583).

### EdgeAI: comparação de argumento recuperada em UnityInitialize real

O [teste #38570345080](../../actions/runs/38570345080) aprovou **59 regressões** e recuperou, sem executar o jogo, uma condição ARM64 verificável em `libunity.so`: `0x935B08: MOV W21,W1` → `0x935B54: CMP W21,#0x2E0` → `0x935B58: B.NE 0x935C6C`. A EdgeAI classifica isso como **segundo argumento diferente de 736**, apoiando-se na preservação do registrador `W21` definida pelo ABI AAPCS64. A análise rejeita sequências com escrita ou efeito desconhecido no registrador e desvios que entrem no meio do intervalo acompanhado.

O significado do segundo argumento **não foi identificado**. A inferência é linear e não prova todos os caminhos de execução. [Relatório](reports/1.14.11949-edgeai-unity-arg-guard.md) | [JSON e logs verificados](../../actions/runs/38570345080/artifacts/11897536364).

### EdgeAI ARM64: comparacao independente com LLVM nas APIs do Unity

[GitHub Actions #38566956506](../../actions/runs/38566956506): **37 testes aprovados**, com **57/57 transferencias de controle** equivalentes por endereco e tipo de instrucao, comparadas ao LLVM em `UnitySendMessage` (3), `UnityInitialize` (32), `UnityShutdown` (5) e `JNI_OnLoad` (17) do `libunity.so` original. Alem disso, [#38565737985](../../actions/runs/38565737985) identificou duas condicoes locais `CMP/B.HI` com maximo de indice 7 nos despachos da `libloader.so`. [Relatorio completo](reports/1.14.11949-edgeai-unity-llvm.md) | [JSON e disassembly](../../actions/runs/38566956506/artifacts/11895846504).

**Nao prova** recuperacao de metodos de gameplay, camera ou jogadores, nem reconstrucao C++. Sao verificacoes estaticas do ELF ARM64 sem executar o jogo.

### Triagem real de simbolos IL2CPP pela EdgeAI

O [workflow #38563095341](../../actions/runs/38563095341) **passou 21 testes** e pesquisou os 1.747 simbolos STT_FUNC nomeados do `libil2cpp.so` original ARM64. Os nomes contendo `camera`, `player`, `position`, `network` ou `entity` tiveram **zero correspondencias**. Os quatro nomes contendo `transform` sao funcoes `std::__ndk1::collate::do_transform`, nao `Unity Transform`. Isso delimita o valor da lista ELF sem provar que metodos de gameplay estejam ausentes do executavel.

[Relatorio detalhado](reports/1.14.11949-edgeai-il2cpp-symbol-triage.md) | [JSON verificado](../../actions/runs/38563095341/artifacts/11892388554).


### Grafo AArch64 e desvios indiretos classificados pela EdgeAI

[Prova no GitHub Actions #38562481574](../../actions/runs/38562481574): **18 testes aprovados** e validacao do `JNI_OnLoad` original com LLVM em `libloader.so`. A ferramenta identificou **117/117** instrucoes de transferencia de controle comparadas ao LLVM, organizou **147 blocos candidatos e 174 arestas** e classificou **7 desvios de tabela (ADR/LDRSW/ADD/BR)** mais **1 chamada indireta (BLR)**. Os destinos indiretos continuam intencionalmente desconhecidos; nao se pode inferir execucao ou alcancabilidade so com esse grafo.

[Relatorio tecnico](reports/1.14.11949-edgeai-indirect-cfg.md) | [JSON e desassembly](../../actions/runs/38562481574/artifacts/11893301719).


### EdgeAI testada contra as bibliotecas ARM64 originais do Oxide

A [execucao #38559496820](../../actions/runs/38559496820) usou o parser ELF da **EdgeAI Engine**, adaptado para bibliotecas grandes por `mmap`, e analisou o split ARM64 original sem executar o jogo. **Quatro testes de regressao** passaram. As tres bibliotecas foram identificadas com SHA-256 verificado: `libil2cpp.so` (236 MB, **1.747 simbolos de funcao nomeados**), `libunity.so` (25 MB, 7 simbolos) e `libloader.so` (10 MB, 1 simbolo).

Isso **comprova utilidade pratica da EdgeAI para inventario ELF64**, nao para extrair automaticamente posicoes de jogadores ou acessar a camera do jogo. Muitos nomes encontrados sao genericos ou opacos. [Relatorio](reports/1.14.11949-edgeai-arm64-audit.md) | [Artefato JSON do teste](../../actions/runs/38559496820/artifacts/11891702226).



### Oito entidades sinteticas e camera externa — 100% virtual, confirmado

O [teste #38557203468](../../actions/runs/38557203468) concluiu os **dois jobs com sucesso** em Android 13 ARM64 Redroid. Um unico Service/JNI recebeu **oito alvos sinteticos simultaneos** em lotes tipados, incluindo matriz 4x4 de camera fornecida pelo remetente. O verificador das capturas reais aprovou **16 verificacoes**, incluindo quatro caixas expiradas independentemente, alteracao de camera, rejeicao de replay, arrays incorretos e IDs duplicados, mesmo PID e limpeza por timeout.

A captura inicial registrou **572 pixels verdes por alvo** em oito faixas da tela; apos expiracao alternada, quatro faixas ficaram com 572 pixels e quatro com **zero**. [Capturas e JSON do Android virtual](../../actions/runs/38557203468/artifacts/11890314399) | [Relatorio detalhado](reports/1.14.11949-virtual-eight-entity-batch.md) | [Codigo do teste](esp_core/android_probe/virtual_overlay_feed_batch.sh).

**Limite:** o APK original Oxide nao foi instalado ou executado nesta validacao. Nao sao jogadores reais; o fluxo de posicoes e a camera continuam controlados pelo laboratorio.




### Duas caixas sinteticas independentes validadas no Android virtual ARM64

A [execucao #38554025039](../../actions/runs/38554025039) aprovou ambos os jobs, inclusive **13 checagens reais de screenshot** no Redroid Android 13 ARM64. Duas caixas receberam coordenadas sinteticas diferentes e foram desenhadas pelo mesmo Service/JNI: **1.878 pixels verdes por caixa**, depois **1.878 + 0** quando uma expirou e **0 + 0** quando ambas expiraram. Ambas reapareceram ao receber novas amostras; replay e campos malformados foram rejeitados, sem troca de PID ou crash registrado.

[Capturas e veredito em JSON](../../actions/runs/38554025039/artifacts/11888845975) |
[Relatorio tecnico](reports/1.14.11949-virtual-multi-target-overlay.md).

**Nao e ESP real:** o teste usa um APK diagnostico independente, sem Oxide instalado e sem posicoes reais de jogadores. A falha separada de screenshot no emulador Android 15 x86_64 permanece sob investigacao.

### Recuperacao automatica quando o feed do overlay para

O teste [#38549706831](../../actions/runs/38549706831) passou
na execucao real Android 13 ARM64 virtual. Agora, quando a fonte
de dados sinteticos **para de enviar mensagens durante quatro
segundos**, o proprio Service apaga a caixa, mesmo sem chegar um
novo pacote. A captura SILENT registrou **zero pixels verdes** e
o PID permaneceu o mesmo. Continuam funcionais as validacoes de
idade, sequencia crescente, rejeicao de replay, restauracao
com dados novos e parada da janela.

[Capturas e registros da execucao](../../actions/runs/38549706831/artifacts/11885992963) |
[Relatorio tecnico atualizado](reports/1.14.11949-virtual-external-synthetic-feed.md).

Tudo isso e validacao do **overlay independente em Android virtual
com dados controlados**. A partida no Oxide original e o ESP de
jogadores reais ainda **nao** foram comprovados; o jogo continua
encerrando ao exibir seu aviso de seguranca.


### Alimentacao externa de amostras no overlay — validada no Android virtual

A [execucao real #38549001618](../../actions/runs/38549001618)
passou os **dois jobs** (compilacao e execucao ARM64 Redroid Android 13),
com **24 testes no build** e **9 verificacoes positivas na
prova de entrada externa**. O proprio shell do Android virtual
enviou dados sinteticos por comandos ADB, para o **mesmo**
servico e PID. A tela foi medida com screenshots reais:

| Comando | Centro X da caixa | Pixels verdes |
|---|---:|---:|
| Inicio | 359,5 | 2112 |
| Pacote 1: movido | 431,5 | 2112 |
| Pacote 2: dados vencidos | — | 0 |
| Replay do pacote 2, rejeitado | — | **0** |
| Pacote 3: restaurado | 287,5 | 2112 |
| Pacote 4: parar | — | 0 |

O receptor de laboratorio requer permissao Android DUMP (shell
privilegiado), e o serviço valida sequencia monotônica, idade e
limites dos dados antes da projecao JNI.

[Relatorio completo](reports/1.14.11949-virtual-external-synthetic-feed.md) |
[Workflow virtual](../../actions/workflows/oxide-virtual-overlay-states.yml) |
[Protocolo ADB sintetico](esp_core/android_probe/virtual_overlay_feed.sh) |
[Artefatos reais](../../actions/runs/38549001618/artifacts/11885558390).

**Importante:** comprovamos atualizacao dinamica do *overlay isolado*,
sem instalar o Oxide original. Nao houve partida multiplayer,
leitura de jogadores reais ou desativacao de protecoes.


### Overlay JNI dinamico funcionando no Android virtual ARM64

A [execucao real #38543062941](../../actions/runs/38543062941)
concluiu o teste do aplicativo de diagnostico no Redroid Android 13
ARM64: **mesmo Service e mesmo PID**, sem reiniciar a janela.
Foram comprovados por **capturas reais, pixels e logs JNI**
os estados de caixa inicial, movimento, dados vencidos,
restauracao com nova posicao e remocao da janela.

| Estado | Centro X | Pixels verdes |
|---|---:|---:|
| Inicial | 359,5 | 2.112 |
| Movido | 431,5 | 2.112 |
| Dados vencidos | sem caixa | 0 |
| Restaurado | 287,5 | 2.112 |
| Servico encerrado | sem caixa | 0 |

**Os 18 testes da compilacao passaram**, e o job Android ARM64
verificou os testes anteriores e a nova sequencia. Nenhum crash
foi registrado. O jogo original **nao foi instalado ou
executado** neste experimento: os dados sao sinteticos e
nao comprovam ESP multiplayer real.

[Relatorio completo](reports/1.14.11949-virtual-continuous-esp-overlay.md) |
[Workflow com capturas reais](../../actions/workflows/oxide-virtual-overlay-states.yml) |
[Codigo do overlay dinamico](esp_core/android_probe/src/com/oxide/bridgeprobe/OverlayProbeService.java).


### Overlay JNI funcionando nos Androids virtuais ARM64 12, 13 e 14

Os testes reais no Redroid provaram o overlay **independente** com o
mesmo APK em **Android 12, 13 e 14 ARM64**, sem aparelho fisico.
O marcador sintetico apareceu com 2.112 pixels verdes por imagem
com dados validos, e com 0 pixels para amostras vazias, antigas
ou apos parar o Service, nas tres versoes.

[Android 12 e 14: execucao aprovada #38543269293](../../actions/runs/38543269293) |
[Android 13: execucao aprovada #38542414642](../../actions/runs/38542414642) |
[Relatorio tecnico e capturas](reports/1.14.11949-native-virtual-overlay-android12-13-14.md).

O [teste adicional #38543062941](../../actions/runs/38543062941)
confirmou que o **mesmo Service Android, com o mesmo PID**, consegue
mover a caixa, apaga-la quando os dados vencem e restaura-la com
dados validos, tambem sem crash observado.

**Limite:** estes sao snapshots sinteticos no Android real
virtualizado; nao sao dados do Oxide multiplayer. O jogo original
nao foi instalado nos testes, e seu encerramento com aviso de
seguranca permanece uma questao separada.


### Overlay real no Android ARM64 virtual: dados validos, vazios, antigos e remocao

A [execucao #38542414642](../../actions/runs/38542414642) instalou
**somente o APK independente de diagnostico** em um Android 13
Redroid nativo ARM64. O codigo C++ foi executado pelo JNI e a janela
transparente foi adicionada pelo WindowManager real do Android.

Os screenshots comprovam **2.112 pixels do marcador sintetico**
quando ha dados validos, e **0 pixels** com dados ausentes, antigos,
ou apos encerrar o servico. Os testes de regressao passaram,
a Activity iniciou nos tres cenarios e nao houve crash do aplicativo
de diagnostico observado no intervalo de teste.

[Relatorio tecnico e limites](reports/1.14.11949-native-virtual-overlay-3state-validation.md) |
[Capturas reais](../../actions/runs/38542414642/artifacts/11883116492) |
[Codigo do servico](esp_core/android_probe/src/com/oxide/bridgeprobe/OverlayProbeService.java) |
[Workflow de validacao](../../actions/workflows/oxide-virtual-overlay-states.yml).

A validacao foi **100% virtual**, sem aparelho fisico e sem instalar
o Oxide original nessa execucao. Ainda **nao** comprova ESP com
jogadores reais ou uma partida multiplayer; o bloqueio de seguranca
do APK original continua separado.


### UnityFS interno decodificado: 277/277 pacotes, somente ambiente virtual

A [execucao real #38539788069](../../actions/runs/38539788069)
concluiu a descompactacao **integral dos 277 UnityFS** do
Oxide original 1.14.11949, apos validar todos os quatro APKs
originais por SHA-256. O processamento, limitado em memoria e
com verificacao dos limites dos blocos, examinou **11.123 blocos
internos e 1.437.758.381 bytes**, catalogou 484 recursos e
nao ignorou nenhum pacote. Foram **22 testes aprovados**.

Uma excecao controlada resolveu o unico pacote pendente: o
bundle original *monoscripts* Unity 6000 declara **16 bytes**
alem do tamanho fisico do arquivo, mas todos os blocos e o
diretorio foram validados dentro do tamanho real, sem modificar
o APK. Ausencia dos literais pesquisados (Security Threat
Detected by App, Security Threat, Threat Detected,
5600:AA4C) nos dados descompactados **nao determina**
qual rotina de protecao produziu o aviso no jogo.

[Relatorio final da decodificacao UnityFS](reports/1.14.11949-complete-unityfs-internal-block-scan.md) |
[Codigo do leitor com limites](tools/audit_original_unityfs_blocks.py) |
[Workflow do teste](../../actions/workflows/oxide-unityfs-internal-audit.yml).

Este resultado **nao** e prova de partida multiplayer ou ESP
com jogadores reais; o APK original ainda se autoencerra
nas execucoes de Redroid. Seguimos 100% virtual, sem aparelho
fisico, sem alteracoes nos APKs ou protecoes.


### Auditoria completa dos recursos Unity (100% virtual)

As execucoes [#38491435687](../../actions/runs/38491435687) e
[#38491935549](../../actions/runs/38491935549) confirmaram todos os
quatro APKs originais por SHA-256 e analisaram **1.741 arquivos internos**,
somando **1.251.260.312 bytes descompactados dos ZIPs**, sem ignorar
arquivos. **Nenhuma** das quatro frases exatas do aviso de seguranca
foi encontrada em UTF-8/UTF-16LE/UTF-16BE nessa camada.

A nova auditoria de formato identificou **277 AssetBundles UnityFS**
com diretorios internos de blocos sinalizados como **LZ4HC** nos
cabecalhos, e dois arquivos GZIP. Isso explica por que a varredura
dos bytes do ZIP nao equivale a ler todo o conteudo descompactado da
Unity. Ainda nao examinamos os blocos internos de recursos dos
277 UnityFS; a causa do aviso de seguranca continua desconhecida.

**Validacao:** 17 testes de regressao passaram na execucao mais
recente. Nenhum APK foi modificado, nenhum aparelho fisico foi
utilizado e nao houve comprovacao de partida multiplayer ou ESP
com dados de jogadores reais.

[Relatorio completo](reports/1.14.11949-full-original-unity-asset-audit.md) |
[Workflow automatizado](../../actions/workflows/oxide-original-full-four-warning-scan.yml) |
[Leitor de formatos Unity](tools/inventory_original_asset_containers.py).


### Sequência de saída do Oxide original confirmada em Android virtual

O [workflow #38491070005](../../actions/runs/38491070005) passou **10 testes**
e reconstruiu a cronologia de duas inicializações do **APK original**
no Android 13 ARM64 virtual. A janela generica de Toast do sistema apareceu
entre 652 e 860 ms apos o evento de criacao do processo. A Activity
terminou apos 4604/4842 ms; o proprio PID do Oxide enviou SIGKILL
apos 4649/4895 ms. Em ambas as tentativas, o Android registrou
em seguida a morte do processo e a limpeza de um Toast pertencente
ao pacote do Oxide.

O registro de criacao da janela Toast nao identifica, isoladamente,
qual pacote a criou; o Android identifica o pacote no registro posterior
de limpeza. O texto especifico do Toast vem das capturas de tela,
**nao** desses logs. Nenhuma dessas evidencias determina qual
verificacao provocou a resposta de seguranca do jogo.

[Relatorio detalhado](reports/1.14.11949-virtual-toast-activity-sigkill-timeline.md) |
[Analisador reproduzivel](tools/analyze_original_toast_timeline.py) |
[Workflow virtual](../../actions/workflows/oxide-original-virtual-toast-timeline.yml).

Nao foi usado aparelho fisico, nem modificacao de APK, nem desbloqueio
de protecoes. Ainda nao ha partida multiplayer ou ESP de jogadores reais.


### Compatibilidade grafica medida em Android virtual ARM64 (12, 13, 14)

As execucoes [#38490256931](../../actions/runs/38490256931) e
[#38490502668](../../actions/runs/38490502668) testaram, em APK de
diagnostico **separado**, as mesmas alocacoes Android HardwareBuffer
de 4x4 pixels com format 43 e 56 e uso 0xb00 observadas nos logs do
Oxide original; tambem testaram RGBA8888 de controle.

| Formato, uso 0xb00 | Android 12 | Android 13 | Android 14 |
|---|---|---|---|
| RGBA1010102 (43) | PASS | FAIL | FAIL |
| R8 (56) | FAIL | FAIL | FAIL |
| RGBA8888 (1) | PASS | PASS | PASS |

Com uso 0x300, os resultados de cada formato permaneceram iguais.
O erro do alocador do Redroid e reproduzivel **sem instalar o Oxide**,
logo ha uma incompatibilidade grafica propria do ambiente. Isso nao
comprova que esse erro provoque o alerta de seguranca; o jogo original
tambem encerrou no Android 12, embora o buffer de formato 43
funcione no diagnostico independente.

[Relatorio e evidencias](reports/1.14.11949-virtual-android-gralloc-independent-reproduction.md) |
[Applet Java isolado](graphics_lab/src/com/oxide/grallocprobe/MainActivity.java) |
[Workflow da comparacao virtual](../../actions/workflows/oxide-virtual-gralloc-versions.yml).

**Nao houve modificacao dos APKs originais, aparelho fisico,
jogadores reais ou ESP em partida comprovado.**



### Diagnóstico de ambiente do Oxide original — execução virtual

A [reanálise dos logs reais das duas inicializações ARM64 (#38489350263)](../../actions/runs/38489350263)
passou **10 testes**. Nos PIDs originais 2197 e 2578 houve avisos
persistentes de API nativa para ro.build.fingerprint (comprimento 113),
falta de serviços Google, falhas de GraphicBuffer e sensores. A Unity e
o IL2CPP iniciaram antes do SIGKILL emitido pelo próprio jogo em ambas
as tentativas. **Esses erros são observações correlacionadas; não foi
identificada qual verificação causou o aviso.**

A frase Security Threat Detected by App também aparece na documentação
de ferramentas comerciais de proteção móvel, mas a [varredura dos três
APKs originais menores (#38489575666)](../../actions/runs/38489575666),
com 4 testes aprovados, não encontrou indicadores explícitos de marca
Appdome, ONEShield ou Fusion Set. **O fornecedor da proteção do Oxide
não está confirmado.**

[Relatório completo](reports/1.14.11949-virtual-preexit-security-evidence.md) |
[Comparador de eventos por PID](tools/compare_original_preexit.py) |
[Auditoria passiva dos APKs](tools/scan_original_security_vendor.py).

A execução segue 100% virtual e sem mudanças nos APKs originais; o
aviso de segurança e o autoencerramento continuam sem solução.



### Android 12 ARM64: PID original e encerramento comprovados

A [nova execução 100% virtual #38488270142](../../actions/runs/38488270142)
e a [reanalise dos artefatos #38488773446](../../actions/runs/38488773446)
resolveram a falha do analisador antigo: a Activity do Oxide
estava em primeiro plano e associada ao **PID 1829**, visto em 31
amostras consecutivas e depois encerrado por **SIGKILL**. O Android
registrou o mesmo PID no historico de saida com status **9**.
Sao **29 testes de regressao aprovados nas duas execucoes** (16+13);
nenhum aparelho fisico ou APK alterado foi utilizado.

Com isso, os Androids virtuais ARM64 **12, 13 e 14** possuem registros de
inicializacao do Oxide seguidos de autoencerramento do processo.
**O motivo especifico da mensagem de seguranca ainda e desconhecido**;
nenhuma partida multiplayer ou ESP de jogadores reais foi validado.

[Relatorio tecnicamente verificado](reports/1.14.11949-android12-virtual-pid-exit-confirmed.md).


**Regra deste projeto:** os próximos testes do Oxide devem ocorrer em ambientes
Android virtuais no GitHub Actions. Não será solicitado telefone pessoal,
aluguel de aparelho físico, serviço de device farm nem credencial de ADB físico.

O foco é usar **Android ARM64 nativo em contêiner Redroid**. Isso evita a
incompatibilidade já demonstrada do tradutor Berberis nos emuladores oficiais
x86_64 (Android 14, 15 e 16). O [workflow de comparação Android 12/14 ARM64
virtual](../../actions/workflows/oxide-virtual-arm64-versions.yml) baixa os
arquivos do Oxide **uma vez**, instala os quatro splits originais intactos em
ambos os ambientes e compara o PID, os eventos de morte do processo e o
estado da Activity após dez segundos.

O workflow de dispositivos físicos desenvolvido anteriormente é **legado** e
não faz parte do plano de execução atual. Seu teste de contrato passado não
significa que algum dispositivo físico foi usado, e não é necessário conectar
nada físico para continuar a investigação.

**Estado conhecido:** o Oxide chega à tela de carregamento no Redroid ARM64
Android 13, mas se autoencerra; o motivo específico do alerta de segurança
continua não identificado. O marcador verde do overlay é sintético e só
foi demonstrado no carregamento, não em partida multiplayer.

---


Arquivos completos do jogo **Oxide: Survival Island** extraídos diretamente do dispositivo via ADB.

---

## 📦 Arquivos no Repositório (Branch `main`)

| Arquivo | Tamanho | Função |
| :--- | :--- | :--- |
| **`base.apk`** | ~83.6 MB | Código Java/Kotlin, AndroidManifest e inicialização |
| **`split_config.arm64_v8a.apk`** | ~80.2 MB | Bibliotecas nativas ARM64 (`libil2cpp.so`, `libunity.so`, etc.) |
| **`split_config.xxhdpi.apk`** | ~217 KB | Recursos visuais e layout de tela |

---

## 🎮 Arquivos Pesados (Recursos Unity)

Devido ao limite de tamanho do GitHub (>100 MB), o pacote principal de assets e gráficos do Unity está disponível na aba de Releases:

* 📦 **`split_UnityDataAssetPack.apk`** (~867 MB - Recursos Unity, modelos 3D, prefabs e mapas):
  👉 **[Download na Release v1.0.0](https://github.com/48348484488/oxide/releases/tag/v1.0.0)**

---

## 📲 Como Instalar Todos os Splits no Celular

Como o jogo utiliza **Android App Bundles (Split APKs)**, para instalar todas as partes juntas no celular via ADB:

```bash
adb install-multiple base.apk split_config.arm64_v8a.apk split_config.xxhdpi.apk split_UnityDataAssetPack.apk
```
Ou utilizando o aplicativo **SAI (Split APKs Installer)** no próprio Android.


---

## Auditoria offline automática (sem modificar o jogo)

O workflow [Oxide APK offline audit](../../actions/workflows/apk-audit.yml) analisa os quatro APKs, inclusive o pacote da Release.

- Executa testes de regressão do analisador.
- Confere SHA-256 e consistência entre os quatro splits.
- Registra versão do AndroidManifest, bibliotecas ARM64 e metadados IL2CPP.
- Publica audit.json e REPORT.md em *Actions → execução → Artifacts* (sem reenviar APKs aos artifacts).

Para repetir: abra **Actions → Oxide APK offline audit → Run workflow**.

**Limitação:** este procedimento faz apenas auditoria estática. Resultado PASS não significa que o jogo iniciou, que uma versão modificada seja estável nem que um ESP esteja funcional. Um teste de inicialização requer ambiente Android compatível com ARM64 e validação separada.


## Teste de abertura do jogo original (Android 15)

O workflow [Original app boot smoke test](../../actions/workflows/android-smoke.yml) tenta instalar os quatro APKs no emulador Android API 35, sem repacotar ou injetar código. O ambiente x86_64 precisa oferecer tradução para bibliotecas ARM64; se não oferecer, o teste informa incompatibilidade do emulador.

O teste registra PID após 15 e 45 segundos, logs de crash, atividade Android e capturas de tela. Os diagnósticos são guardados em **Actions → execução → Artifacts → oxide-original-app-smoke**.

**Não é um teste de ESP.** Mesmo que o processo permaneça aberto por 45 segundos, isso não comprova uma partida jogável nem ausência de crashes no aparelho real.

---

## Android ARM64 real no GitHub (APK original)

O workflow [Original Oxide ARM64](../../actions/workflows/redroid-original-smoke.yml) inicia um Android 13 ARM64 nativo e instala os quatro APKs originais, sem modificar o jogo.

**Resultado confirmado:** o jogo abre a Activity/Unity, apresenta um aviso de segurança e se encerra. Logs mostram `SIGKILL` emitido pelo próprio processo após a finalização da Activity. **Não** foi alcançado o menu jogável nem houve captura de posições multiplayer. Portanto **não existe ESP real validado** no jogo original.

A ferramenta [diagnose_original_exit.py](tools/diagnose_original_exit.py) correlaciona automaticamente os PIDs, a saída da Activity e os registros de encerramento, gerando `forensic_summary.json` e `forensic_summary.md` em *Actions → Artifacts*. O teste separado de código C++/JNI/Canvas não equivale a integração com o APK original.

[Relatório técnico detalhado](reports/1.14.11949-native-arm64-original-boot.md) | [Execução com o analisador](../../actions/runs/38471501701)


## Execução em Android físico na nuvem

Foi preparado um [smoke test não invasivo por ADB](tools/smoke_original_on_connected_device.sh) para instalar os **quatro APKs originais** em dispositivo físico remoto autorizado e registrar a Activity, screenshots e encerramentos. Há também um [guia de conexão remota](docs/physical-android-test.md). O Google oferece dispositivos físicos de nuvem via Android Device Remote, mas é preciso um projeto Google Cloud com acesso autorizado; ainda **não há dispositivo remoto conectado nem teste físico concluído**.

A [auditoria automática do ambiente Android](tools/audit_android_environment.py) registra somente propriedades do sistema, disponibilidade de pacotes Google e GLES. Isso **não comprova certificação, integridade, segurança aprovada, conexão multiplayer ou funcionamento do ESP**.

**Integridade e CI:** o script físico agora exige SHA-256 exato dos quatro APKs, verifica a continuidade do PID original e só classifica uma Activity como viva se ela continuar em primeiro plano. A execução [#38473888348](../../actions/runs/38473888348) aprovou **17 testes** mais a rejeição efetiva de APKs ausentes **antes de qualquer ADB**. Esses são testes do procedimento, não de uma partida.

O [workflow rápido de regressão](.github/workflows/physical-adb-contract.yml) roda automaticamente; o [teste Redroid com APKs de aproximadamente 1 GB](.github/workflows/redroid-original-smoke.yml) ficou manual para evitar novas execuções redundantes enquanto a proteção original encerra o jogo.

## Prova visual ARM64 sobre o Oxide original

A execução [#38476139597](../../actions/runs/38476139597) **passou nos dois jobs**: um APK independente de diagnóstico JNI/Canvas desenhou **2.416 pixels de contorno verde** sobre a imagem de carregamento da Activity do **Oxide 1.14.11949 original, com todos os quatro splits intactos**, em Android 13 ARM64 nativo. A janela foi removida sem deixar pixels residuais. O processo do jogo em primeiro plano foi verificado, não confundido com serviço de analytics.

- [Capturas de tela reais e logs do teste](../../actions/runs/38476139597/artifacts/11853614134)
- [Relatório da prova visual](reports/1.14.11949-original-overlay-native.md)

**Limite essencial:** isso valida a sobreposição de uma caixa de **dados artificiais de diagnóstico apenas durante o carregamento**. O jogo continua exibindo alerta de segurança e encerra seu próprio processo. **Não existe ESP multiplayer validado nem garantia de funcionamento ou estabilidade durante uma partida.** O teste pesado ficou manual para não repetir downloads e execuções desnecessárias.

## Teste A/B de permissões do Android virtual

O [experimento real em Redroid ARM64 #38478578089](../../actions/runs/38478578089) concedeu com sucesso as quatro permissões comuns antes negadas: câmera, microfone, Bluetooth e notificações. **Mesmo com todas essas permissões concedidas**, a segunda execução do jogo original encerrou por SIGKILL emitido pelo próprio processo (PID 2578), assim como a primeira (PID 2197).

O comparador inicial exibiu um **falso negativo** porque leu um PID antigo no logcat. A falha foi corrigida e a [validação rápida #38479050657](../../actions/runs/38479050657) aprovou 23 testes. [Relatório com horários e PIDs](reports/1.14.11949-android-permission-ab.md).

Conclusão: as permissões normais não bastam para o Oxide completar o boot nesse Android virtual. Os serviços Google continuam ausentes, a causa exata da verificação de segurança segue desconhecida e **não existe ESP em partida validado**.

## Android virtual com Google Play: prova de compatibilidade e limite atual

Em 13/10/2026, o [emulador Google Play oficial, Android 15](../../actions/runs/38479923337) foi executado no GitHub Actions. Ele contém Play Store, Play Services e Google Services Framework, anuncia ABI arm64-v8a e usa a ponte libndk_translation.so em host x86_64.

O [teste com os quatro APKs originais e verificados](../../actions/runs/38480303555) confirmou INSTALL_SUCCESS, mas o processo real PID 4078 encerrou com crash nativo **SIGILL (sinal 4; SI_TKILL)** durante os construtores de **libloader.so**, antes da inicialização da Unity. Os endereços de pilha do módulo original foram 0x830BEC e 0x830D98; a [classificação reproduzida dos logs](../../actions/runs/38480931360) passou nos testes. Isso é um resultado diferente do Redroid ARM64 sem GMS, onde a Unity abriu antes de SIGKILL.

O [catálogo oficial de SDK e host ARM64](../../actions/runs/38481155935) contém imagens Play ARM64 nativas para Android 13 a 16, mas o runner Linux ARM64 não possui KVM. O [ensaio Apple Silicon nativo](../../actions/runs/38481294705) verifica uma possível alternativa sem tradução ARM; o resultado desse ensaio deve ser consultado no run, sem presumir boot.

Relatórios: [linha do tempo original](reports/1.14.11949-original-process-boot-timeline.md) e [crash no emulador Google Play](reports/1.14.11949-official-google-play-emulator-boot.md).

**Não há prova de jogo em partida, ESP real, token Play Integrity ou garantia de compatibilidade.** Nenhum APK foi modificado ou qualquer verificação de integridade contornada.

## Causa do crash do Oxide no Android Google Play x86_64: identificada

[Verificacao do APK original mais dois controles, 13 testes aprovados](../../actions/runs/38484496716).

O Oxide original (PID 4078) registrou Berberis: Undefined instruction 0xd5380429 e, em seguida, SIGILL. A tombstone localizou a falha em libloader.so +0x830BEC, onde o ELF original contem a instrucao ARM64 MRS x9, ID_AA64PFR1_EL1. Um teste JNI isolado reproduziu o erro no mesmo tipo de tradutor (opcode 0xd5380420, apenas com registrador de destino diferente); no Android ARM64 nativo, essa leitura funcionou.

**Resultado:** incompatibilidade real do tradutor ARM64 Berberis com essa instrução no Android 15 Google Play x86_64. [Relatorio tecnico detalhado](reports/1.14.11949-arm64-system-register-reproduction.md).

**Ainda nao resolvido:** no Android ARM64 nativo, o Oxide ultrapassa o inicio da Unity, mas o aplicativo original se autoencerra com SIGKILL por motivo distinto ainda nao estabelecido. Nenhuma partida multiplayer nem ESP de jogadores reais foi validado.


## Estado do Oxide Android 14–16 e da mensagem de segurança (13/10/2026)

O [teste em Android 14 e Android 16 com Google Play](../../actions/runs/38485084410) passou na infraestrutura, mas **a instrução ARM64 MRS continuou produzindo SIGILL nos dois**. Android 15 já havia falhado da mesma forma. Portanto, nenhuma dessas três imagens x86_64 com tradução ARM64 resolveu o carregamento do Oxide; o APK do jogo não foi instalado nas duas variações mais recentes.

Também foi feita uma [varredura somente de leitura nos três APKs originais menores](../../actions/runs/38485899858): 1.454 entradas, 384.032.039 bytes descompactados, hashes íntegros, cinco testes aprovados e **nenhuma ocorrência em texto simples** da frase “Security Threat Detected by App”. Isso não demonstra ausência do aviso nem identifica sua origem: o texto pode estar codificado, ser gerado em runtime ou residir no grande pacote de assets ainda não examinado.

O bloqueio restante é o encerramento por SIGKILL emitido pelo Oxide em ARM64 nativo, após iniciar a Unity e apresentar a mensagem de segurança. Uma investigação conclusiva da compatibilidade exige Android ARM64 legítimo e autorizado, idealmente um dispositivo físico remoto que atenda aos requisitos da instalação; não basta conceder permissões do Android. [Relatório do aviso](reports/1.14.11949-original-security-warning-literal-audit.md).

**Limite verificável:** overlay verde de diagnóstico no carregamento original; nenhum ESP com posições de jogadores ou partida jogável comprovado.


## Teste do APK original em Android ARM64 fisico remoto (sem usar celular pessoal)

Foi criado um workflow com **ADB remoto por SSH autenticado e host pinado** para um dispositivo Android ARM64 fisico autorizado. O teste preserva os 4 APKs originais, baixa os recursos grandes somente depois de validar a conexao e coleta o PID/Activity aos 10 e 35 segundos. Nenhum mecanismo de seguranca do Oxide e modificado.

**Resultado real atual:** [GitHub Actions #38486718125](../../actions/runs/38486718125) passou **18 testes de contrato**, mas o job no aparelho fisico ficou SKIPPED porque o repositorio ainda nao possui as cinco credenciais para um laboratorio remoto. **Nao houve dispositivo fisico executado, gameplay ou ESP validado.**

Para habilitar o dispositivo remoto (uma unica configuracao do provedor), adicionar nos **GitHub Actions Secrets** as chaves: OXIDE_REMOTE_HOST, OXIDE_REMOTE_USER, OXIDE_REMOTE_SSH_KEY, OXIDE_REMOTE_KNOWN_HOSTS e OXIDE_REMOTE_ADB_SERIAL. O provedor deve fornecer um telefone fisico autorizado acessivel por ADB no host SSH; esta integracao nao cria um aparelho nem contrata servicos pagos sozinha.

[Instrucoes e limites](reports/1.14.11949-remote-physical-adb-integration.md) | [Workflow](../../actions/workflows/oxide-remote-physical-arm64.yml)

**Privacidade:** logs completos, capturas e identificadores do aparelho nao sao publicados automaticamente nos artefatos. ADB sideload dos quatro splits nao garante licenciamento Play ou um veredito de integridade positivo.


## Novos resultados reais: Android 12/14 virtual ARM64 (13/10/2026)

A [execucao virtual ARM64 Android 12 vs 14 #38487286930](../../actions/runs/38487286930) terminou com infraestrutura aprovada. **Os dois Androids virtuais iniciaram e instalaram os quatro APKs originais com sucesso, sem modificar o jogo.** O pacote grande foi baixado apenas uma vez, e 13 testes de regressao passaram.

As capturas reais de ambos os Androids mostram a tela de carregamento original do Oxide com a mensagem Security Threat Detected by App. Portanto, o jogo **tambem carregou a interface no Android 12**, mesmo que o parser nao tenha encontrado o evento am_proc_start nesta versao. Em Android 14, o PID original 1876 terminou por SIGKILL emitido pelo proprio processo.

[Relatorio detalhado e explicacao dos limites](reports/1.14.11949-native-virtual-arm64-12-14.md) | [Screenshots e dados reais da CI](../../actions/runs/38487286930/artifacts/11859419695).

**Escopo exclusivamente virtual:** nao e preciso aparelho fisico. A limitacao restante e o aviso de seguranca e encerramento do APK original nestes ambientes, sem causa especifica comprovada. Nenhum ESP de jogadores reais ou partida multiplayer foi validado.
