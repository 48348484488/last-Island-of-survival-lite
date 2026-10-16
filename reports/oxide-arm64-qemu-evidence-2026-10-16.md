# Oxide ARM64 / GitHub Actions — estado verificado em 2026-10-16

**Objetivo:** inicializar Android ARM64 no GitHub gratuito para depois testar os APKs originais do Oxide. **Oxide não foi executado neste caminho.** Testes de kernel, recovery, captura VNC e Android completo são etapas distintas.

## Evidências reais

| Evidência | Antes: [#38935740656](https://github.com/48348484488/oxide/actions/runs/38935740656) | Com AC97 + VirtIO: [#38938610831](https://github.com/48348484488/oxide/actions/runs/38938610831) |
| --- | --- | --- |
| Android ARM64 e kernel | Iniciaram | Iniciaram |
| bootanim iniciado — tempo de relógio *guest* no serial | ~223,7 s | ~153,7 s |
| zygote iniciado — relógio guest | ~190,8 s | ~132,5 s |
| surfaceflinger iniciado — relógio guest | ~200,5 s | ~138,7 s |
| seriallogging | Terminou com status 1 | Registrado como ativo (sem falha observada) |
| vendor.setup_sound_card | SIGABRT | Terminou com código 1, ainda não corrigido |
| Console VirtIO 0 | Não conectado | 414 bytes (UEFI/EFI stub; não é shell comprovado) |
| Console VirtIO 1 | Não conectado | 138 bytes (mensagem libc de crash_dump; não é logcat abrangente) |
| ADB | Não respondeu | Não respondeu; serial mostra init.svc.adbd=stopped |
| Framebuffer, 360 s vs 450 s | Amostras visualmente idênticas | Hashes diferentes; animação continua |
| sys.boot_completed=1 | Não comprovado | Não comprovado |
| Oxide original | Não instalado/não testado | Não instalado/não testado |

**Cuidado:** alterações de áudio, VirtIO e configurações foram introduzidas juntas. A diferença nos tempos observados é real, mas não comprova qual alteração foi responsável. O QEMU por software é lento; a ausência de ADB **não prova**, por si só, falha absoluta de inicialização Android. Tampouco a animação comprova boot completo.

## Diagnóstico mais útil

1. Não repetir simplesmente a imagem de ~1,12 GB por falta de ADB. O Android mostra `init.svc.adbd=stopped` nas mensagens seriais. Precisamos de prova de tela inicial ou outro sinal independente, sem alterar o APK.
2. `seriallogging` deixou de encerrar com status 1 depois dos novos canais VirtIO. No entanto, só chegaram registros mínimos; não declarar shell/logcat funcionais.
3. `vendor.setup_sound_card` permanece com saída não-zero; separar erro de áudio do possível atraso no boot.
4. Usar testes pequenos de compatibilidade e desempenho QEMU antes de outro teste Android pesado.
5. Quando o Android for comprovadamente utilizável, instalar e executar somente os APKs originais, registrar PID >= 35 s, captura real de menu e controles, sem atribuir progresso do boot ao jogo.

## Automação e limites

- [Pós-análise automática após conclusão](https://github.com/48348484488/oxide/actions/runs/38939739512): **sucesso** na leitura e classificação do artefato anterior.
- [Teste isolado do classificador](https://github.com/48348484488/oxide/actions/runs/38939778272): **sucesso** (fixture offline; não prova o boot).
- [Captura e entrada VNC reais na recuperação Android](https://github.com/48348484488/oxide/actions/runs/38935318361): **sucesso**; confirma gráficos e comando de entrada apenas na recuperação.
- [Matriz de hardware sem Android](https://github.com/48348484488/oxide/actions/runs/38938257237): **sucesso** na configuração de áudio, consoles VirtIO e GPU.

**Não concluído:** boot Android completo confirmado, serviços Play/integração de conta, APK original Oxide, estabilidade de PID e gameplay.
