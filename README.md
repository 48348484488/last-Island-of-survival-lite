# Oxide: Survival Island (APK & Split Packages)

## Direção atual: execução 100% virtual, sem aparelhos físicos

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
