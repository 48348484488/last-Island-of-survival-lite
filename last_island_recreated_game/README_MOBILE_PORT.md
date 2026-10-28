# Last Island of Survival LITE — Android ARM64 (teste nativo)

## O que está no jogo

Existem dois clientes diferentes no mesmo repositório.

- `viewer3d.py`: cliente 3D para **PC**, com 23 animações originais
  amostradas do `authentic_animations.npz` e deformação esquelética por
  `skinned_character.py`. Sua renderização usa OpenGL de desktop; esse código
  **não é compilável diretamente para APK Android**.
- `main.cpp`: cliente C++/Raylib, com cenário 3D, modelos/texturas extraídos,
  machado, M4, recursos, coleta e controles de toque. **Usa animações
  procedurais**; ainda não recebeu a conversão das 23 animações para Android.
  Esta é a base do primeiro APK de teste.

Não afirme que as duas versões têm paridade visual ou funcional.

## APK ARM64 e testes

Workflow: `.github/workflows/build_android_raylib_lite.yml`.

A Action compila a biblioteca nativa do C++ no Android NDK, utiliza Raymob
e Raylib como host Android, copia apenas os 24 OBJ/PNG usados no motor
C++ e gera APK de debug assinado para **arm64-v8a**, nome:
`Last-Island-LITE-Recriado-Android-ARM64-v0.12.1-preview.apk`.

**Como baixar**: abra a execução mais recente do workflow no GitHub Actions,
abra **Artifacts**, escolha `Last-Island-LITE-Recriado-Android-ARM64-APK`
e extraia o APK do ZIP. Ou use o arquivo anexo do chat quando disponível.

**O que foi verificado**: compilação/assinatura/estrutura do APK,
se a Action estiver verde. Ainda não equivale a teste real no seu aparelho,
nem garante desempenho adequado.

## O que falta

- Converter as 23 animações originais do esqueleto Python para um
  `ModelAnimation` ou skinning runtime GLES compatível com Android.
- Reaproveitar o HUD Unity V12 completo no Raylib Android.
  A HUD e os cliques originais V12 foram verificados no cliente Python do PC,
  **não** foram automaticamente transferidos ao C++.
- Validar resolução, FPS, colisões e controles em aparelho Android real
  ou em Android virtual ARM64 com EGL funcional.

O código e os assets recuperados do jogo original são usados para fins de
análise e testes neste projeto; distribuição pública requer avaliação dos
direitos sobre os materiais.
