# Android locomotion V16.4 — auditoria dos clipes originais

Fonte de reprodução: `lios_skeleton_v1.bin`, gerado de `authentic_animations.npz` (Unity `role/animationbundle.ab`); **64 ossos**, **7 malhas**, **33 clipes**, frames a **30 Hz**. O teste automático do repositório está em `tools/test_original_footplants.py`.

## Análise da passada

Para cada animação, reconstruímos as transformações de cada dedo do pé (`Bip001 L Toe0` e `Bip001 R Toe0`) por cinemática direta. Os 30% de frames mais baixos de cada pé são tratados como contato aproximado com o chão. A velocidade do pé parado em relação à raiz do personagem permite estimar o movimento contrário do personagem no mundo.

| Clipe Unity | Velocidade do pé apoiado (média aproximada) | Movimento do corpo | Calibração |
|---|---|---|---|
| `zhan_run_front01` | Z -4,37 u/s | +Z | 4,35 u/s |
| `zhan_run_back01` | Z +2,15 u/s | -Z | 2,15 u/s |
| `zhan_run_left01` | X +2,20 u/s | -X | 2,25 u/s |
| `zhan_run_right01` | X -3,02 u/s | +X | 3,00 u/s |

Os sinais e os valores vêm da amostragem dos clipes, **não** de frames de vídeo. Esta é uma estimativa de foot-plant, não uma captura certificada do movimento da versão original rodando no celular.

## Correção Android

O cliente C++ usa uma câmera que olha para +Z, portanto o vetor **direita da tela** é `(-cos(yaw),0,+sin(yaw))`. O personagem e a animação usam os eixos de mundo do mesh exportado. Depois da conversão Unity → OpenGL, os nomes dos clipes `left`/`right` são invertidos **em relação à direita/esquerda que o usuário vê na tela**. O controle Android seleciona `zhan_run_left01` ao andar para a direita da tela, e `zhan_run_right01` ao andar para a esquerda da tela, para acompanhar o movimento real dos pés.

A velocidade de deslocamento é agora calibrada ao movimento dos pés de cada clipe, com interpolação de direção diagonal, aceleração de sprint, intensidade analógica e ritmo da animação ajustado pela distância percorrida. Pequenas oscilações laterais do dedo durante um gesto vertical são suprimidas para evitar deslize lateral acidental.

## Limites

Aprovado nos testes matemáticos/asset, quando a Action estiver verde. Não equivale a inspeção visual do APK executado no aparelho Android. A simulação usa contato aproximado do dedo do pé e terreno plano; sistema completo de física, IK de pés, terreno inclinado e reprodução exata dos parâmetros do Unity ainda não foram reconstruídos.
