# Audio original — auditoria LITE 7.3 (XAPK)

Fonte: **XAPK original recomposto de 17 partes** (1.534.140.778 bytes) e
`assets/cfg/cfg.SoundCfg.oc`, interpretado segundo
`ExportedProject/Assets/MonoScript/Assembly-CSharp/Share/Octets.cs`.
Foram identificadas **1.058 entradas** de som por ID, nome, volume e raio.

| Evento | ID Unity | Arquivo requerido na pasta `sound/` |
|---|---:|---|
| M4 perto | 215 | `gunshot_m4a1_near.ab` |
| M4 longe | 214 | `gunshot_m4a1_far.ab` |
| M4 silenciada | 216 | `gunshot_m4a1_xiaoyin.ab` |
| Passo grama | 320 | `footsteps_move_caodi.ab` |
| Passo pedra | 318 | `footsteps_move_shiban.ab` |
| Passo madeira | 323 | `footsteps_move_muban.ab` |
| Passo metal | 324 | `footsteps_move_gangban.ab` |
| Passo areia | 162 | `footsteps_move_shamo.ab` |
| Passo grama, amostras | 2004–2007 | `footsteps_move_caodi_01..04.ab` |
| Pulo | 330 | `footsteps_tiao.ab` |
| Pulo baixo | 331 | `footsteps_xiaotiao.ab` |
| Aterrissagem forte | 332 | `footsteps_luodi_zhong.ab` |
| Aterrissagem leve | 333 | `footsteps_luodi_qing.ab` |

**Limite comprovado:** O XAPK distribuído contém `assets/sound/bgm_loading.ab`
no APK principal, mas **não contém os AssetBundles** listados acima
para passos, pulo e tiro. O pacote `install_time_asset_pack.apk`
também não possui esses arquivos. O cliente original requisita
`sound/<SoundCfg.path>.ab` via `ResMgr`, portanto esses bundles
podem ter sido baixados separadamente. Não inventar WAVs originais.

Já estão no repositório arquivos WAV recuperados anteriormente,
incluindo `wq_m4_1_gunshot outdoor loop.wav` e
`wq_m4_1_gunshot outdoor loop end.wav`, usados no Android V16
para disparo prolongado e soltura do gatilho. A correspondência
exata entre esses WAVs e as entradas `gunshot_m4a1_near` e
`gunshot_m4a1_far` ainda não foi provada; os nomes são diferentes.

`tools/audit_original_xapk_sound.py` e a Action
`audit_original_xapk_sound.yml` reproduzem o inventário de áudio sem
alterar/excluir o XAPK. O áudio de passos, salto e pouso no Android
continua sintetizado até que os AudioClips originais sejam recuperados.

**Dados localizados, mas ainda sem equivalência audiovisual comprovada:** 
IDs de eventos, nomes de AssetBundles e parâmetros de volume/distância.
