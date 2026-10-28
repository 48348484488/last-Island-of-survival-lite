# Last Island of Survival LITE (v7.3) - Reverse Engineering & Animation Dataset

Repositorio completo contendo o pacote original compacto (XAPK v7.3), scripts descompilados em C#, AssetBundles de animacao e modelos 3D originais, alem de ferramentas de engenharia reversa para analise por Inteligencias Artificiais e desenvolvedores.

---

## Estrutura do Repositorio

`
last-island-survival-lite/
├── xapk_original/                      # Pacote original completo (1.5 GB) em partes de 90 MB
│   ├── Last_Island_LITE_7.3.xapk.part00 .. part16
│   ├── reassemble_xapk.py              # Script automatico de reconstrucao do XAPK
│   └── reassemble_xapk.bat             # Reconstrucao em 1 clique no Windows
│
├── ExportedProject/                    # Codigo-fonte C# descompilado do jogo
│   └── Assets/MonoScript/Assembly-CSharp/
│       ├── HandAttack.cs               # Logica de ataque melee, IDs de som e efeitos
│       ├── AttackState.cs              # Maquina de estados de combate e janelas de impacto
│       ├── BasePlayerController.cs     # Encaixe de armas no socket (wuqi_guadian)
│       ├── vThirdPersonCamera.cs       # Posicionamento e mira da camera em 3a pessoa
│       └── ...
│
├── raw_assets/                         # AssetBundles Unity originais extraidos
│   └── assets/
│       ├── role/
│       │   ├── animationbundle.ab      # Todos os AnimationClips autenticos (DenseClip/Euler)
│       │   ├── bip001.ab               # Esqueleto canonico de 64 ossos e bindposes
│       │   └── man_*.ab                # Malhas 3D do personagem e vestimentas
│       └── model/
│           ├── wq_shifu.ab             # Machado de pedra (Item 55001) e matriz de socket
│           ├── ak47_low.ab             # Fuzil AK-47
│           └── tree_*.ab               # Arvores e vegetacao
│
├── tools/                              # Scripts de decodificacao de animacoes Unity
│   └── extract_authentic_animations.py # Parser de curvas DenseClip e quaternions
│
└── last_island_recreated_game/                           # Cliente 3D COMPLETO recriado via engenharia reversa (Pronto para jogar)
    ├── skinned_character.py            # Skinning LBS matricial e socket M_gl autentico
    ├── viewer3d.py                     # Loop do jogo, HUD, combate e coleta
    └── authentic_animations.npz        # 23 animacoes autenticas extraidas
`

---

## Como Reconstruir o XAPK Original (1.5 GB)

O arquivo original Last_Island_of_Survival_LITE_7.3.xapk possui 1,534,140,778 bytes (~1.5 GB). Para cumprir o limite maximo de 100 MB por arquivo do GitHub, ele foi dividido em 17 partes de 90 MB.

### Opcao 1 (Python - Multiplataforma):
`ash
cd xapk_original
python reassemble_xapk.py
`

### Opcao 2 (Windows CMD / Batch):
`cmd
cd xapk_original
reassemble_xapk.bat
`
Ou manualmente no terminal do Windows:
`cmd
copy /b Last_Island_LITE_7.3.xapk.part* Last_Island_of_Survival_LITE_7.3.xapk
`

### Opcao 3 (Linux / macOS):
`ash
cat Last_Island_LITE_7.3.xapk.part* > Last_Island_of_Survival_LITE_7.3.xapk
`

---

## Guia para Inteligencias Artificiais (AI Prompting & Analysis)

Se voce e uma IA analisando este repositorio para auxiliar na extracao e aperfeicoamento de animacoes:

1. **Esqueleto e Hierarquia de Ossos**:
   - Arquivo base: 
aw_assets/assets/role/bip001.ab
   - O esqueleto possui 64 ossos canonicos indexados a partir de Bip001.
   - O socket da arma direita e wuqi_guadian (filho de Bip001 R Hand).
   - A conversao de coordenadas entre o espaco Unity (X direita, Y cima, Z frente) e OpenGL e dada por {u2gl}$ e uma correcao (90^\circ)$ na bindpose.

2. **Extracao de Curvas de Animacao**:
   - Arquivo: 
aw_assets/assets/role/animationbundle.ab
   - Clipes principais:
     - zhan_stand01: Parado / Idle
     - zhan_run_front01: Corrida completa
     - u_zhan_attack_01: Golpe de machado
     - wq_m4_1_zhan_miaozhun_attack01: Disparo de fuzil
   - Formato: Unity DenseClip com canais de translacao (ttribute=1), rotacao em quaternion (ttribute=2) e angulos de Euler (ttribute=4).
   - Use o script em 	ools/extract_authentic_animations.py como referencia de decodificacao de streams.

3. **Logica de Combate e Coleta**:
   - ExportedProject/Assets/MonoScript/Assembly-CSharp/HandAttack.cs:
     - IDs de som: 480 (corte de arvore), 481 (mineracao de rocha/minerio), 163/164 (impacto em jogador).
     - Efeitos visuais: kanshu_01.ab e wakuang_01.ab.
     - Alcance autentico: ~2.8 metros com verificacao vetorial frontal.

---

## HUD original V9 integrado ao jogo recriado

A interface do `last_island_recreated_game/viewer3d.py` agora usa, por padrao, a
renderizacao do `battlepanel.ab` extraido do XAPK original. Diferente do HUD
manual anterior, a camada visual usa os sprites originais identificados por
Unity FileID/PathID, a ordem dos GameObjects e as coordenadas RectTransform.
Nao usa recortes de screenshots.

- `last_island_recreated_game/original_hud_runtime.py`: ponte entre estado real
  do jogo (vida, fome, sede, direcao, inventario coletado) e HUD original.
- `tools/render_original_hud_v7.py`: reconstrucao visual do prefab com numeros
  e quantidades dinamicas, sem itens inventados na partida.
- `tools/test_original_hud_runtime.py`: teste sem GPU que gera uma previa
  *sintetica* usando assets originais (nao e screenshot do jogo).
- `LIOS_HUD_MODE=legacy`: volta ao HUD antigo para diagnostico.

**Limitacoes verificadas:** fonte original nao recuperada (usa glifos de
substituicao), mascara/shader e CanvasScaler/safe area nao equivalentes 1:1;
mapa mostra recorte central de demonstracao ate recuperar a transformacao das
coordenadas de mundo; icones de machado/fuzil ainda nao possuem vinculo
original comprovado no prefab. A camada OpenGL foi ligada ao cliente desktop,
mas esta versao ainda nao foi validada visualmente em uma partida nem
empacotada/testada como APK Android.

[Teste automatizado V9](https://github.com/48348484488/last-Island-of-survival-lite/actions/workflows/validate_original_hud_runtime_v9.yml)

### V10 - renderizacao OpenGL realmente executada e fotografada pelo CI

O jogo Python `viewer3d.py` foi executado com Mesa/OpenGL 2.1 e Xvfb em
GitHub Actions (sem GPU dedicada, sem Android fisico). Nos modos `melee` e
`rifle`, o jogo completou tres frames, desenhou o HUD de sprites originais e
salvou screenshots **do framebuffer real** 960x540. O modo estrito
(`LIOS_STRICT_HUD=1`) rejeita a substituicao silenciosa pelo HUD legado.

- [Execucao completa com screenshots reais](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40631809546) - ambos os jobs verdes; artifacts por modo
- [Execucao otimizada (checkout parcial)](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40631866768) - ambos os jobs verdes
- Workflow: `.github/workflows/smoke_real_original_hud_opengl.yml`
- Ambiente de teste: `LIOS_HUD_MODE=source LIOS_STRICT_HUD=1 LIOS_SMOKE_FRAMES=3 LIOS_SMOKE_PROFILE=melee|rifle`

**Auditoria das imagens:** HUD original visualmente presente, dados de
vida/fome/agua, mapa, controles e icones de materiais. Os slots de machado e
fuzil continuam **sem icones de item**, pois o prefab deixa estes m_Sprite
como zero e o vinculo original de inventario nao foi recuperado. O mapa usa
uma posicao ilustrativa; dados de CanvasScaler, shaders e fonte original
permanecem incompletos. Esta validacao e de desktop/Mesa, nao de APK Android.


### V11 - sprites de machado e M4 recuperados pelo ID original

Os slots 1 e 2 da barra rapida nao precisam mais ficar vazios: a referencia
real do item esta em `assets/cfg/cfg.ItemCfg.oc` do APK base, enquanto o Sprite
e servido pelos AssetBundles no `install_time_asset_pack.apk`.

| Equipamento | Item ID original | Icon de cfg.ItemCfg | Fonte do Sprite | PathID |
| --- | ---: | --- | --- | --- |
| Machado de pedra | 55001 | `tubiao1/cj_sf` | `assets/icon/tubiao1.ab` | `4568438245182409447` |
| M4 | 52231 | `gun/m4` | `assets/icon/gun.ab` | `919133799852272780` |

A verificacao confirma a string de icon, o caminho do modelo 3D e o
encadeamento de dados no arquivo `cfg.ItemCfg.oc`. Os dois sprites sao
decodificados dos arquivos Unity originais, com SHA-256 dos **pixels RGBA**
registrado em
`last_island_recreated_game/assets/hud_original_xapk/complete_source_mapping/verified_equipped_weapon_icons.json`.

O renderer so ativa estes icones quando existe o manifesto de verificacao:
`OriginalUnityHud` importa os sprites autenticados e o jogo preenche os slots
com `cj_sf` e `m4`. Nao usa figuras parecidas nem recortes de screenshots.

**Testes reais:** [machado + M4 no framebuffer OpenGL](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40633463669)
(os dois perfis passaram), e
[teste de integracao Pillow/headless](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40633480170)
(aprovado).
As capturas sao de desktop/Mesa. Ainda nao representam APK Android, nem
demonstram coincidencia pixel-a-pixel com o jogo original executado.


### V12 - botoes de toque e joystick ligados ao gameplay

O jogo recriado agora tem a opcao `LIOS_TOUCH_MODE=1` para receber os eventos
`pygame.FINGERDOWN/FINGERMOTION/FINGERUP` no mesmo Canvas 1920x1080 do HUD
original. `original_hud_touch.py` utiliza **somente** os `RectTransform`s
originais dos controles do `battlepanel.ab`:

- `btn_fire`: ataque corpo-a-corpo / tiro (municao diminui)
- `btn_up2`: salto com fisica existente (altura do jogador muda)
- `btn_kaijing`: mirar com M4, alternando ADS
- `btn_changeBullet`: recarregar M4, restaurando o carregador
- `m_yaoganTrigger` + `m_littleYaogan` + `m_YaoganBG`: direcao
  analogica original do joystick -> deslocamento 3D real
- `btn_autoRun`: alternar corrida; `btn_crouch2` / `btn_pa2`: reducao
  de velocidade (animacao de postura ainda nao recuperada)

O controlador conserva a identidade de cada dedo, permite joystick +
ataque + salto simultaneos e adapta a regiao do Canvas a viewport com
letterboxing. `btn_pack`, `btn_zhizao`, configuracoes e chat registram
o toque, mas **nao abrem interfaces** porque esses paineis ainda nao estao
implementados. O knob grafico do joystick ainda nao acompanha o dedo e
o `CanvasScaler`/safe-area do Android ainda precisa de calibracao.

**Provas**:
- [Hitboxes e joystick Unity auditados](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40635124401)
- [Testes puros de toque e multiplos dedos](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40635246790)
- [Quatro testes do jogo em OpenGL real: machado/M4, toque ligado/desligado](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40635353081)
- [Verificacao OpenGL adicional de mira e recarga](https://github.com/48348484488/last-Island-of-survival-lite/actions/runs/40635435674)

`LIOS_TOUCH_MODE=1 python last_island_recreated_game/viewer3d.py` habilita
o recebimento dos eventos de toque no cliente desktop Python/Pygame. Nao
e um APK Android e os testes OpenGL rodam em Mesa/Xvfb, nao em um aparelho.
