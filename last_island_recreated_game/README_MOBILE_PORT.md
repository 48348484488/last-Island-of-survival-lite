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


## Porta nativa C++ v0.13 — 33 animações originais

Agora o C++ contém um port efetivo do pipeline do cliente Python:

- Exportador `tools/export_mobile_skeletal_animation.py` lê
  `authentic_animations.npz` e `master_character.npz`. A auditoria encontrou
  **33 clipes**, **64 ossos** e **7 malhas** (pants, torso, skin, shoes, gloves,
  head, hair). O formato `LIOSSKN1` gerado tem ~2,2 MB.
- `native_original_animation.hpp` decodifica o binário no Android,
  interpola as rotações por quaternion SLERP, calcula hierarquia dos ossos (FK),
  aplica pesos/bindposes (LBS) e envia os vértices deformados a buffers
  dinâmicos OpenGL ES do Raylib.
- `main.cpp` substitui o corpo estático pelo esqueleto com os clipes de
  repouso, deslocamento direcional, salto e ataques do machado/M4.
  A arma é anexada ao osso original `wuqi_guadian`.
- Foi integrada a camada de sprites original recuperada do `battlepanel.ab`,
  com ícones `cj_sf` e `m4`, números de recursos e hitboxes de ataque/pulo
  derivadas do Canvas 1920x1080. Esta ainda é uma **adaptação de primeira fase**:
  nem todos os componentes dinâmicos, efeitos, máscaras ou painéis completos
  do HUD do PC estão disponíveis no C++.

O job `build_android_raylib_lite.yml` exporta os dados durante a compilação
e verifica que os 33 clipes, 64 ossos, 7 malhas e PNGs do HUD aparecem no APK.

A aprovação de compilação e verificação de assinatura **não garante** que todos
os clipes estejam corretos visualmente, nem substitui testes de FPS e exibição
em Android real. Verificar o cenário, o corpo animado, os ícones, ações de
pulo/ataque e performance no aparelho ainda é necessário. Sem dados de uma
execução em aparelho, não afirmar compatibilidade 1:1 com o cliente de PC.
