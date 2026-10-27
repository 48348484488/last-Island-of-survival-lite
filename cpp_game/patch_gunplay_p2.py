import os

path = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\viewer3d.py'
with open(path, 'r', encoding='utf-8') as f:
    text = f.read()

# 1. In main(), load wq_m4_1 model and texture
old_load = '''    axe_tex        = load_texture(p('wq_shifu_ba.png'))
    iron_ore_tex   = load_texture(p('ore_tiekuang_01_d.png'))'''

new_load = '''    axe_tex        = load_texture(p('wq_shifu_ba.png'))
    gun_tex        = load_texture(p('wq_m4_1_ba.png'))
    iron_ore_tex   = load_texture(p('ore_tiekuang_01_d.png'))'''

assert old_load in text, 'old_load not found'
text = text.replace(old_load, new_load)

old_mesh = '''    axe_dl        = load_obj(p('wq_shifu.obj'), swap_yz=False)
    iron_ore_dl   = load_obj(p('ore_tiekuang_01.obj'), swap_yz=False)'''

new_mesh = '''    axe_dl        = load_obj(p('wq_shifu.obj'), swap_yz=False)
    gun_dl        = load_obj(p('wq_m4_1.obj'), swap_yz=False)
    iron_ore_dl   = load_obj(p('ore_tiekuang_01.obj'), swap_yz=False)'''

assert old_mesh in text, 'old_mesh not found'
text = text.replace(old_mesh, new_mesh)

# 2. Add weapon & shooting state variables before game loop
old_state = '''    speed = 7.5
    walk_phase = 0.0
    is_moving, is_running = False, False
    attack_t, is_attacking = 0.0, False
    health, stamina = 100.0, 100.0'''

new_state = '''    speed = 7.5
    walk_phase = 0.0
    is_moving, is_running = False, False
    attack_t, is_attacking = 0.0, False
    equipped_slot = 0    # 0 = Stone Axe, 1 = M4A1 Assault Rifle
    is_aiming = False    # ADS (Aim Down Sights) zoom & centering
    ammo_cur, ammo_max = 30, 30
    health, stamina = 100.0, 100.0'''

assert old_state in text, 'old_state not found'
text = text.replace(old_state, new_state)

# 3. Add hotbar slot switching (1, 2, 3...) and reload (R) in KEYDOWN
old_keys = '''                elif ev.key in (K_TAB, K_v):
                    third_p = not third_p
                elif ev.key == K_F4:
                    running = False'''

new_keys = '''                elif ev.key in (K_TAB, K_v):
                    third_p = not third_p
                elif ev.key == K_1:
                    equipped_slot = 0
                    msg = 'Machadinha Equipada'; msg_t = 1.5
                elif ev.key == K_2:
                    equipped_slot = 1
                    msg = 'M4A1 Equipada'; msg_t = 1.5
                elif ev.key == K_r and equipped_slot == 1:
                    ammo_cur = ammo_max
                    msg = 'Recarregado (30/30)'; msg_t = 1.5
                elif ev.key == K_F4:
                    running = False'''

assert old_keys in text, 'old_keys not found'
text = text.replace(old_keys, new_keys)

# 4. Handle Right-Click ADS and Left-Click firing/chopping
old_mouse = '''            elif ev.type == MOUSEBUTTONDOWN and locked:
                if ev.button == 1 and not is_attacking:
                    is_attacking = True; attack_t = 0.0
                    anim.on_attack_start()
                    btn_pressed.add('attack')
                    hit = False
                    for tx, tz, _ in TREES:
                        if math.hypot(px - tx, pz - tz) < 4.5:
                            wood += 15; msg = '+15 Madeira!'; msg_t = 2.0
                            hit = True
                            break
                    if not hit:
                        for ox, oz, otype, _, _, _, _, omsg in ORES:
                            if math.hypot(px - ox, pz - oz) < 4.5:
                                if otype == 'metal':   metal += 10
                                elif otype == 'stone':  stone += 10
                                elif otype == 'sulfur': sulfur += 10
                                msg = omsg; msg_t = 2.0
                                break
                elif ev.button == 4: cam_dist = max(1.5, cam_dist - 0.4)
                elif ev.button == 5: cam_dist = min(9.0, cam_dist + 0.4)'''

new_mouse = '''            elif ev.type == MOUSEBUTTONDOWN and locked:
                if ev.button == 1 and not is_attacking:
                    if equipped_slot == 1:
                        # Gunfire logic (M4A1)
                        if ammo_cur > 0:
                            is_attacking = True; attack_t = 0.0
                            ammo_cur -= 1
                            anim.on_attack_start()
                            btn_pressed.add('attack')
                            # Check raycast / target hit in front of player
                            hit = False
                            for tx, tz, _ in TREES:
                                d_tree = math.hypot(px - tx, pz - tz)
                                if d_tree < 25.0:
                                    # Angle check to see if looking at tree
                                    ang = math.atan2(tx - px, tz - pz)
                                    diff = (ang - yaw + math.pi) % (2 * math.pi) - math.pi
                                    if abs(diff) < 0.25:
                                        wood += 20; msg = '+20 Madeira (Tiro)!'; msg_t = 1.5
                                        hit = True
                                        break
                            if not hit:
                                for ox, oz, otype, _, _, _, oname, omsg in ORES:
                                    d_ore = math.hypot(px - ox, pz - oz)
                                    if d_ore < 25.0:
                                        ang = math.atan2(ox - px, oz - pz)
                                        diff = (ang - yaw + math.pi) % (2 * math.pi) - math.pi
                                        if abs(diff) < 0.25:
                                            if otype == 'metal':   metal += 15
                                            elif otype == 'stone':  stone += 15
                                            elif otype == 'sulfur': sulfur += 15
                                            msg = omsg; msg_t = 1.5
                                            hit = True
                                            break
                        else:
                            msg = 'Sem Municao! Aperte R para recarregar'; msg_t = 2.0
                    else:
                        # Melee Axe logic
                        is_attacking = True; attack_t = 0.0
                        anim.on_attack_start()
                        btn_pressed.add('attack')
                        hit = False
                        for tx, tz, _ in TREES:
                            if math.hypot(px - tx, pz - tz) < 4.5:
                                wood += 15; msg = '+15 Madeira!'; msg_t = 2.0
                                hit = True
                                break
                        if not hit:
                            for ox, oz, otype, _, _, _, _, omsg in ORES:
                                if math.hypot(px - ox, pz - oz) < 4.5:
                                    if otype == 'metal':   metal += 10
                                    elif otype == 'stone':  stone += 10
                                    elif otype == 'sulfur': sulfur += 10
                                    msg = omsg; msg_t = 2.0
                                    break
                elif ev.button == 3:
                    # Right-click toggle ADS Aim
                    is_aiming = not is_aiming
                    btn_pressed.add('aim') if is_aiming else btn_pressed.discard('aim')
                elif ev.button == 4: cam_dist = max(1.5, cam_dist - 0.4)
                elif ev.button == 5: cam_dist = min(9.0, cam_dist + 0.4)'''

assert old_mouse in text, 'old_mouse not found'
text = text.replace(old_mouse, new_mouse)

# 5. Attack duration tuning (shorter for gun fire recoil, 1.0 for axe swing)
old_att_update = '''        if is_attacking:
            attack_t += dt * 1.0
            if attack_t >= 1.0:
                is_attacking = False; attack_t = 0.0
                btn_pressed.discard('attack')'''

new_att_update = '''        if is_attacking:
            att_spd = 5.0 if equipped_slot == 1 else 1.0
            attack_t += dt * att_spd
            if attack_t >= 1.0:
                is_attacking = False; attack_t = 0.0
                btn_pressed.discard('attack')'''

assert old_att_update in text, 'old_att_update not found'
text = text.replace(old_att_update, new_att_update)

# 6. Pass equipped_slot & is_aiming to anim.update
old_anim_call = '''        # Update authentic animation with real Unity clip data
        anim.update(character, dt, is_moving, is_running, is_grounded,
                    is_attacking, attack_t, jump_t, _local_fwd, _local_right)'''

new_anim_call = '''        # Update authentic animation with real Unity clip data
        anim.update(character, dt, is_moving, is_running, is_grounded,
                    is_attacking, attack_t, jump_t, _local_fwd, _local_right,
                    equipped_slot=equipped_slot, is_aiming=is_aiming)'''

assert old_anim_call in text, 'old_anim_call not found'
text = text.replace(old_anim_call, new_anim_call)

# 7. Camera FOV / distance adjustment for ADS Zoom
old_cam_call = '''            cx = tx - lx * cam_dist
            cy = py + 1.65 - ly * cam_dist
            cz = tz - lz * cam_dist
            gluLookAt(cx, cy, cz, tx + lx * 2.0, ty + ly * 2.0, tz + lz * 2.0, 0, 1, 0)'''

new_cam_call = '''            effective_cam_dist = cam_dist * (0.6 if (is_aiming and equipped_slot == 1) else 1.0)
            cx = tx - lx * effective_cam_dist
            cy = py + (1.55 if is_aiming else 1.65) - ly * effective_cam_dist
            cz = tz - lz * effective_cam_dist
            gluLookAt(cx, cy, cz, tx + lx * 2.0, ty + ly * 2.0, tz + lz * 2.0, 0, 1, 0)'''

assert old_cam_call in text, 'old_cam_call not found'
text = text.replace(old_cam_call, new_cam_call)

# 8. Character & 1st-person weapon render calls
old_char_render = '''        # Rigged Character (animation already applied by anim.update above)
        if third_p:
            glPushMatrix()
            glTranslatef(px, py, pz)
            draw_rigged_character(character, char_textures, head_dl, hair_dl, axe_dl, axe_tex, yaw)
            glPopMatrix()

        # 1st-person axe
        if not third_p:
            bob_x = math.cos(walk_phase * 3.0) * 0.015 if is_moving else 0.0
            bob_y = math.sin(walk_phase * 6.0) * 0.015 if is_moving else 0.0
            draw_weapon_fp(axe_dl, axe_tex, bob_x, bob_y, attack_t, is_attacking, W, H)'''

new_char_render = '''        # Rigged Character (animation already applied by anim.update above)
        if third_p:
            glPushMatrix()
            glTranslatef(px, py, pz)
            draw_rigged_character(character, char_textures, head_dl, hair_dl, axe_dl, axe_tex,
                                  gun_dl=gun_dl, gun_tex=gun_tex, equipped_slot=equipped_slot, yaw=yaw)
            glPopMatrix()

        # 1st-person weapon (Axe or M4A1)
        if not third_p:
            bob_x = math.cos(walk_phase * 3.0) * 0.015 if is_moving else 0.0
            bob_y = math.sin(walk_phase * 6.0) * 0.015 if is_moving else 0.0
            draw_weapon_fp(axe_dl, axe_tex, gun_dl, gun_tex, equipped_slot,
                           bob_x, bob_y, attack_t, is_attacking, is_aiming, W, H)'''

assert old_char_render in text, 'old_char_render not found'
text = text.replace(old_char_render, new_char_render)

# 9. Update hud.draw call with weapon state & ammo
old_hud_call = '''        # HUD
        hud.draw(W, H, int(clock.get_fps()), health, stamina, wood, metal, stone, sulfur,
                 px, pz, yaw, third_p, msg, msg_t, dt, food, water,
                 joy_input=(joy_dx_main, joy_dy_main), btn_pressed=btn_pressed)'''

new_hud_call = '''        # HUD
        hud.draw(W, H, int(clock.get_fps()), health, stamina, wood, metal, stone, sulfur,
                 px, pz, yaw, third_p, msg, msg_t, dt, food, water,
                 joy_input=(joy_dx_main, joy_dy_main), btn_pressed=btn_pressed,
                 equipped_slot=equipped_slot, is_aiming=is_aiming,
                 ammo_cur=ammo_cur, ammo_max=ammo_max)'''

assert old_hud_call in text, 'old_hud_call not found'
text = text.replace(old_hud_call, new_hud_call)

with open(path, 'w', encoding='utf-8') as f:
    f.write(text)

print('Successfully integrated M4A1 gunplay, aiming ADS, recoil, and hotbar switching!')
