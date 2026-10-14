# Oxide: Survival Island (APK & Split Packages)

## Direção atual: execução 100% virtual, sem aparelhos físicos


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
