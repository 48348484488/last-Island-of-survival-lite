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
