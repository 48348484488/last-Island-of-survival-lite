# Oxide: Survival Island (APK & Split Packages)

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
