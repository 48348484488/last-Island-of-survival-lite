# Guia de Portabilidade para Mobile (Android APK) — C++ / Raylib

O código do jogo foi desenvolvido em **C++ nativo** utilizando a biblioteca **Raylib**, que possui suporte oficial direto para compilação em dispositivos móveis (Android).

---

## 🎮 Controles Mobile Integrados
O arquivo `main.cpp` já vem com suporte nativo a touchscreen e controles virtuais:
- **Analógico Virtual na Esquerda (Left Joystick):** Movimentação 360° com detecção de toque.
- **Swipe na Direita (Right Screen):** Rotação suave da câmera (Yaw / Pitch).
- **Botão [GOLPE] / [ATTACK]:** Desfere ataque com o Machado ou atira com o Rifle.
- **Botão [PULO]:** Pula com física de gravidade.
- **Botão [CORRER]:** Sprint com consumo de stamina.
- **Botão [1P/3P]:** Alterna entre 1ª e 3ª pessoa.
- **Hotbar Inferior Touch:** Toque no slot 1 (Machado) ou slot 2 (M4A1) para trocar de arma instantaneamente.

---

## 📱 Como Gerar o APK para Android

### Método 1: Usando Android Studio + NDK (Recomendado)
1. Instale o **Android Studio** com o **NDK (Side by side)** e **CMake**.
2. Baixe o template oficial do Raylib para Android:
   ```bash
   git clone https://github.com/raysan5/raylib-game-template-android
   ```
3. Copie o arquivo `main.cpp` e a pasta `assets/` para a pasta do projeto:
   - `main.cpp` $\rightarrow$ `app/src/main/cpp/main.cpp`
   - `assets/` $\rightarrow$ `app/src/main/assets/`
4. Abra o projeto no Android Studio e clique em **Build > Build Bundle(s) / APK(s) > Build APK(s)**.
5. O APK estará pronto em `app/build/outputs/apk/debug/app-debug.apk` para instalar no celular!

---

### Método 2: Linha de Comando (Android NDK standalone)
Você também pode compilar diretamente pelo terminal se tiver o Android NDK configurado:
```bash
cmake -B build-android \
  -DCMAKE_TOOLCHAIN_FILE=$ANDROID_NDK_HOME/build/cmake/android.toolchain.cmake \
  -DANDROID_ABI="arm64-v8a" \
  -DANDROID_PLATFORM=android-24

cmake --build build-android
```

---

## 💻 Como Compilar no PC (Windows)
Basta clicar duas vezes em:
- **`compilar_cpp.bat`**: Compila o jogo usando o compilador G++/Clang incluído.
- **`iniciar_jogo_cpp.bat`**: Inicia o executável `game.exe`.
