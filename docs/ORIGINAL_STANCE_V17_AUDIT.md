# V17.0 — animações originais de agachar e deitar (Unity `man`)

## Evidência da extração

A Action `audit_original_stances.yml` abriu o AssetBundle de **72.423.924 bytes** em `raw_assets/assets/role/animationbundle.ab` e listou **1.381 objetos Unity AnimationClip**. O `AnimatorController` original `man` em `raw_assets/assets/role/role.ab` confirma os estados na **Base Layer**:

- `Base Layer.Crouch`: `kongshou_dun_stand01`, `dun_run_front01`, `dun_run_back01`, `dun_run_left01`, `dun_run_right01`, corridas diagonais e `dun_rush_front01`;
- `Base Layer.Pa.Pa`: `pa_stand01`, `pa_walk_front01`, `pa_walk_back01`, `pa_walk_left01`, `pa_walk_right01`;
- `Base Layer.CrouchDown.kongshou`: `kongshou_dun_qiepa01`;
- `Base Layer.PaUp.PaUp_KongShou`: `kongshou_pa_qiedun01`;
- `Base Layer.PaUp.kongshou_pa_qiezhan01`: `kongshou_pa_qiezhan01`.

São dados de **clipes e controlador originais**, não animações geradas artificialmente.

## Conversão Android

O script `tools/export_original_stances.py` utiliza `tools/restore_original_melee_run.py` para ler curvas Unity `m_StreamedClip`, `m_DenseClip` e `m_ConstantClip`, reconstruir canais `position/quaternion` nos **64 ossos** do personagem original e anexar **26 clipes adicionais** aos 33 já existentes. O binário LIOSSKN1 passa de 33 para **59 clipes**.

A máquina de estados em `last_island_recreated_game/main.cpp` seleciona movimentos por postura (em pé, agachado, deitado), direção do analógico, arma equipada e transições `CrouchDown/PaUp`. O cliente limita movimento enquanto os clipes de transição estão ativos e suaviza a altura da câmera.

## Diferença importante para a versão comercial

A recompilação, a estrutura dos dados e a presença dos 59 clipes podem ser provadas em CI. Isso **não** demonstra fidelidade perfeita do movimento durante a execução em Android físico. Ainda são necessários captura de quadros da versão original e do port, comparação das posições dos pés, caixa de colisão/agachamento, curvas de BlendTree (não apenas seleção dominante) e áudio real de passos/rastejo.

Os AssetBundles originais `footsteps_move_*.ab`, `footsteps_tiao.ab` e `footsteps_luodi_*.ab` referenciados em `docs/ORIGINAL_AUDIO_AUDIT.md` **não estão disponíveis no XAPK preservado**, portanto a recriação ainda não reproduz seus AudioClips exatos.
