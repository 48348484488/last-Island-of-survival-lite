# Oxide: Survival Island v1.15.11966 (APK & Split Packages)

Arquivos completos da versão atualizada (**v1.15.11966**) do jogo **Oxide: Survival Island** extraídos diretamente do dispositivo via ADB.

---

## 📦 Arquivos no Repositório (Branch `main`)

| Arquivo | Tamanho | Função |
| :--- | :--- | :--- |
| **`base.apk`** | ~82.6 MB | Código Java/Kotlin, AndroidManifest e inicialização |
| **`split_config.arm64_v8a.apk`** | ~80.2 MB | Bibliotecas nativas ARM64 (`libil2cpp.so`, `libunity.so`, etc.) |
| **`split_config.xxhdpi.apk`** | ~217 KB | Recursos visuais e layout de tela |

---

## 🎮 Arquivos Pesados (Recursos Unity)

Devido ao limite de tamanho do GitHub (>100 MB), o pacote principal de assets e gráficos do Unity está disponível na aba de Releases:

* 📦 **`split_UnityDataAssetPack.apk`** (~909 MB - Recursos Unity, modelos 3D, prefabs e mapas):
  👉 **[Download na Release v1.15.11966](https://github.com/48348484488/oxide/releases/tag/v1.15.11966)**

---

## 📲 Como Instalar Todos os Splits no Celular

Como o jogo utiliza **Android App Bundles (Split APKs)**, para instalar todas as partes juntas no celular via ADB:

```bash
adb install-multiple base.apk split_config.arm64_v8a.apk split_config.xxhdpi.apk split_UnityDataAssetPack.apk
```
Ou utilizando o aplicativo **SAI (Split APKs Installer)** no próprio Android.

---

## ☁️ Teste automatizado em Android ARM64 virtual

Para testar o **Oxide original sem celular físico**, a alternativa cloud preparada é o [workflow manual Genymotion SaaS ARM64](https://github.com/48348484488/oxide/blob/main/.github/workflows/oxide-managed-cloud-arm64-trial.yml). Ele **não cria nem inicia instâncias** e conecta somente a um dispositivo virtual já autorizado; o uso desse dispositivo pode consumir minutos gratuitos ou gerar cobranças conforme o plano, verifica Google Play packages e ABI antes do download, confere os quatro SHA-256, instala os splits com ADB e captura evidências de 35 segundos. A parada após a execução vem habilitada por padrão.

**Atenção:** ainda depende de uma conta Genymotion com avaliação gratuita aprovada (60 minutos sujeitos ao fornecedor), token salvo como GitHub Actions Secret `GENYMOTION_API_TOKEN` e UUID da instância virtual em execução. [Configuração completa e limitações](https://github.com/48348484488/oxide/blob/main/reports/1.15.11966-managed-arm64-cloud-provider-trial.md).

**Situação atual:** os testes de código passaram, mas o jogo ainda não foi executado neste novo provedor. O erro de encerramento após aproximadamente 4,5 segundos permanece reproduzido no Redroid Android 15/16. Não há prova de menu, partida ou ESP real funcionando.
