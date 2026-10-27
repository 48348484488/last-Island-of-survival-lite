# Last Island of Survival (LITE) - Jogo Recriado via Engenharia Reversa

Este diretorio contem o **jogo 3D funcional recriado do zero** atraves da engenharia reversa pura dos binarios e AssetBundles do **Last Island of Survival LITE 7.3**.

---

## Destaques da Engenharia Reversa Implementada

1. **Esqueleto e Deformacao de Malha (LBS)**:
   - Malha do personagem desmembrada em 7 partes com deformacao via *Linear Blend Skinning* matricial no NumPy einsum (~2.5ms por quadro).
   - Esqueleto canonico de 64 ossos decodificado de ip001.ab.
   - 23 animacoes originais do APK decodificadas de nimationbundle.ab (Idle, Corrida direcional, Ataque com Machado, Disparos, etc.).

2. **Encaixe e Alinhamento Autentico das Armas**:
   - **Machado de Pedra (wq_shifu.obj, Item 55001)**: Encaixe perfeito no socket wuqi_guadian derivado matematicamente da bindpose de wq_shifu.ab e matriz {gl}$. Cabo firme na mao direita, empunhadura rente a coxa em descanso e golpe vertical no ataque.
   - **Fuzil M4A1 (wq_m4_1.obj)**: Alinhamento de visada no centro da tela ao mirar (ADS) e postura natural de quadril.

3. **Sistema de Coleta e Combate (HandAttack.cs)**:
   - Janela de impacto real no frame 14-16 do golpe (nao registra dano instantaneo no clique).
   - Deteccao de colisao frontal ( \theta > 0.5$) e alcance de 2.8m.
   - Efeitos sonoros reais: corte de madeira em arvores, mineracao em rochas/minerios e som de golpe no ar.
   - Notificacoes de coleta na HUD (+15 Madeira, Minerio de Ferro, Pedra, Enxofre).

4. **HUD Autentica (1:1 com BattlePanel.prefab)**:
   - Barra de vida, fome e sede posicionadas e escaladas de acordo com as coordenadas originais do Canvas do jogo.
   - Botoes de acao (Ataque, Pulo, Agachar, Deitar, Sprint) e minimapa funcional no canto superior direito.

---

## Como Executar

### No Windows (1 Clique):
Basta dar dois cliques no arquivo:
`cmd
iniciar_jogo.bat
`

### Manualmente pelo Terminal (Windows / Linux / macOS):
1. Instale as dependencias necessarias:
`ash
pip install -r requirements.txt
`

2. Inicie o jogo:
`ash
python viewer3d.py
`

---

## Comandos e Controles

| Tecla / Acao | Funcao |
| --- | --- |
| **W, A, S, D** | Movimentacao direcional do personagem |
| **Shift Esquerdo** | Correr (consome estamina) |
| **Espaco** | Pular |
| **Clique Esquerdo** | Atacar com machado / Coletar arvores ou minerios / Disparar arma |
| **Clique Direito** | Mirar (ADS - Zoom e visada centralizada) |
| **Tecla 1** | Equipar Machado de Pedra (wq_shifu) |
| **Tecla 2** | Equipar Fuzil M4A1 (wq_m4_1) |
| **Tab** | Alternar entre 3a Pessoa (ombro direito) e 1a Pessoa |
| **Esc** | Travar / destravar o cursor do mouse na tela |
