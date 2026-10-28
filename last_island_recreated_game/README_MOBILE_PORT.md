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


## Android v0.13.1: correcoes orientacao e materiais

Corrigidos dois erros da v0.13.0 relatados no teste em telefone:

- `native_original_animation.hpp` tinha uma reflexao incorreta `Z=-Z`
  no termo esquerdo da matriz de skinning. O cliente PC usa
  `M_U2GL @ RX90 == Identidade`, o que foi restaurado no corpo e no
  encaixe original da arma. O espelhamento anterior podia mostrar a
  frente do personagem para uma camera corretamente posicionada atras,
  bem como inverter o sentido dos triangulos.
- `main.cpp` agora usa a mesma camera de ombro do PC (`3.8` unidades
  para tras, `0.35` unidade a direita e alvo 2 unidades para frente).
- A malha `skin` recebe `man_clothes_yf_0_ba.png` (que ja era usada
  pela versao de PC), nao a textura da cabeca. As 6 texturas do
  personagem sao empacotadas, e os vertices **e normais** sao
  atualizados nos buffers dinamicos durante as animacoes.
- `tools/test_android_character_parity.py` confere as matrizes, os
  5 casos de orientacao da camera, nomes dos atlas e vinculacao
  das texturas antes de compilar o APK.
- A Action confere o asset de pele dentro do APK final.

A compilacao ARM64 e os testes de correspondencia nao substituem
uma verificacao visual no proprio Android; ajustes de orientacao do
personagem, shaders e desempenho podem continuar necessarios.


## Android v0.14.0 — HUD Unity gerado do proprio battlepanel.ab

A HUD anterior ainda tinha **erros de posicionamento**:
- O C++ desenhava os icones do machado/M4 em X=791/885,
  mas os `m_Item1.m_icon` / `m_Item2.m_icon` originais comecam
  respectivamente em X=634.60 / 729.61 (Canvas 1920x1080).
- Os valores de vida, fome e sede eram desenhados perto do rodape;
  no prefab original os `txt_mybloodNum`, `txt_hunger` e
  `txt_bodyWater` estao no topo direito, em torno de X=1463.
- O contorno dourado de selecao era manual e as imagens de controles
  eram iguais para machado e fuzil. A interface do Unity usa
  `m_heightLignt` e perfis de combate diferentes.

**Correcao implementada:** `tools/export_mobile_original_hud.py`
usa o mesmo rasterizador de sprites/PPtr/RectTransform da versao PC e
exporta dois PNGs, `hud_original_melee.png` e
`hud_original_rifle.png`, com realce original do slot 1/2.
O proprio script gera o header `mobile_original_hud_layout.hpp` com
11 hitboxes Unity, 7 slots e geometria de texto do prefab — o C++ nao
precisa de coordenadas anotadas a mao. Agora as quantidades dos
recursos coletados ocupam os respectivos slots 3 a 6, com sprites
originais do XAPK (`tb_cl_mt`, `tb_cl_tiek`, `tb_cl_st`, `ks_lhk`).
O centro do joystick conserva sua imagem original e acompanha o dedo;
o uso de `GetTouchPointId` evita ficar preso quando o Android
reorganiza o indice dos dedos. Toques nos botoes nao sao interpretados
como deslize de camera.

**Limites de fidelidade ainda em aberto:** a fonte do Unity nao foi
recuperada, entao o texto usa fonte substituta do Raylib; a textura
do minimapa e uma imagem original mas a posicao real do jogador e
sua escala dinamica nao estao implementadas; o compasso, mascaras e
animacoes de alguns elementos precisam de logica Unity de runtime;
nem todos os paineis (mochila/receitas/configuracoes) estao implementados.
O CanvasScaler do aplicativo original e a safe area do aparelho
ainda exigem uma calibracao independente. Nao chamar esta versao de
"100% identica" sem essas comprovacoes.

O job `test_mobile_original_hud_export.yml` verifica a origem dos
dois perfis, 11 botoes, 7 slots e 4 itens extraidos. O job de Android
compila o C++ com header e overlays gerados, valida o conteudo
ARM64 e produz um novo APK de teste.
