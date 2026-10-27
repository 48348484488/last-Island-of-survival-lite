import os

path = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\viewer3d.py'
with open(path, 'r', encoding='utf-8') as f:
    text = f.read()

# -------------------------------------------------------------
# 1. Update AuthenticAnimController with weapon modes (Axe vs Gun)
# -------------------------------------------------------------
old_controller = '''class AuthenticAnimController:
    """
    Selects the correct authentic animation clip based on game state.
    Provides buttery smooth sub-frame playback and 0.15s cross-fading between states.
    Directional walk clips are chosen by projecting world-space movement into
    character local space (local_fwd, local_right) supplied by main().
    """
    IDLE_CLIP    = 'zhan_stand01'
    RUN_FRONT    = 'zhan_run_front01'  # Authentic male armed run forward
    RUN_BACK     = 'zhan_run_back01'   # Authentic male armed run backward (from APK)
    # Swapped so D (right) plays zhan_run_right01 visual and A (left) plays zhan_run_left01 visual
    RUN_LEFT     = 'zhan_run_right01'
    RUN_RIGHT    = 'zhan_run_left01' 
    ATTACK_CLIP  = 'fu_zhan_attack_01'

    def __init__(self):
        self.idle_time      = 0.0
        self.walk_time      = 0.0   # shared accumulator for all directional run clips
        self.attack_time    = 0.0
        self.current_clip   = self.IDLE_CLIP
        self.prev_clip      = self.IDLE_CLIP
        self.blend_weight   = 1.0
        self.blend_duration = 0.15
        self.attack_count   = 0

    def update(self, character, dt, is_moving, is_running, is_grounded,
               is_attacking, attack_t, jump_t, local_fwd=1.0, local_right=0.0):
        if is_attacking:
            target_clip = self.ATTACK_CLIP
            self.attack_time += dt * 1.3
        elif is_moving and is_grounded:
            # Pick directional clip based on dominant local-space movement axis
            if abs(local_fwd) >= abs(local_right):
                target_clip = self.RUN_FRONT if local_fwd >= 0.0 else self.RUN_BACK
            else:
                target_clip = self.RUN_RIGHT if local_right >= 0.0 else self.RUN_LEFT
            speed = 1.4 if is_running else 1.0
            self.walk_time += dt * speed
        else:
            target_clip = self.IDLE_CLIP
            self.idle_time += dt

        # State transition
        if target_clip != self.current_clip:
            self.prev_clip = self.current_clip
            self.current_clip = target_clip
            self.blend_weight = 0.0
            if target_clip == self.ATTACK_CLIP:
                self.attack_time = 0.0

        if not is_attacking:
            self.attack_time = 0.0

        self.blend_weight = min(1.0, self.blend_weight + dt / self.blend_duration)

        def get_clip_time(clip):
            if clip == self.ATTACK_CLIP: return self.attack_time
            if clip in (self.RUN_FRONT, self.RUN_BACK,
                        self.RUN_LEFT,  self.RUN_RIGHT): return self.walk_time
            return self.idle_time

        if self.blend_weight < 1.0 and self.prev_clip != self.current_clip:
            character.set_pose_blended(
                self.prev_clip, get_clip_time(self.prev_clip),
                self.current_clip, get_clip_time(self.current_clip),
                self.blend_weight
            )
        else:
            character.set_pose_time(self.current_clip, get_clip_time(self.current_clip))

        return self.current_clip

    def on_attack_start(self):
        self.attack_count += 1
        self.attack_time = 0.0'''

new_controller = '''class AuthenticAnimController:
    """
    Selects the correct authentic animation clip based on game state and equipped weapon.
    Supports seamless transitions between Axe (melee) and M4A1 (rifle aiming/shooting).
    Provides buttery smooth sub-frame playback and 0.15s cross-fading between states.
    """
    # Axe animations
    AXE_IDLE     = 'zhan_stand01'
    AXE_RUN_FWD  = 'zhan_run_front01'
    AXE_RUN_BACK = 'zhan_run_back01'
    AXE_RUN_LEFT = 'zhan_run_right01'
    AXE_RUN_RGHT = 'zhan_run_left01'
    AXE_ATTACK   = 'fu_zhan_attack_01'

    # M4A1 Rifle animations (extracted directly from role/animationbundle.ab)
    GUN_IDLE     = 'wq_m4_1_zhan_miaozhun_stand01'
    GUN_RUN_FWD  = 'wq_m4_1_zhan_miaozhun_run_front01'
    GUN_RUN_BACK = 'zhan_run_back01'
    GUN_RUN_LEFT = 'zhan_run_right01'
    GUN_RUN_RGHT = 'zhan_run_left01'
    GUN_SHOOT    = 'wq_m4_1_zhan_miaozhun_attack01'

    def __init__(self):
        self.idle_time      = 0.0
        self.walk_time      = 0.0
        self.attack_time    = 0.0
        self.current_clip   = self.AXE_IDLE
        self.prev_clip      = self.AXE_IDLE
        self.blend_weight   = 1.0
        self.blend_duration = 0.15
        self.attack_count   = 0

    def update(self, character, dt, is_moving, is_running, is_grounded,
               is_attacking, attack_t, jump_t, local_fwd=1.0, local_right=0.0,
               equipped_slot=0, is_aiming=False):
        # Select active animation palette based on equipped weapon
        if equipped_slot == 1:
            idle_clip   = self.GUN_IDLE
            run_fwd     = self.GUN_RUN_FWD
            run_back    = self.GUN_RUN_BACK
            run_left    = self.GUN_RUN_LEFT
            run_right   = self.GUN_RUN_RGHT
            attack_clip = self.GUN_SHOOT
        else:
            idle_clip   = self.AXE_IDLE
            run_fwd     = self.AXE_RUN_FWD
            run_back    = self.AXE_RUN_BACK
            run_left    = self.AXE_RUN_LEFT
            run_right   = self.AXE_RUN_RGHT
            attack_clip = self.AXE_ATTACK

        if is_attacking:
            target_clip = attack_clip
            self.attack_time += dt * (3.5 if equipped_slot == 1 else 1.3)
        elif is_moving and is_grounded:
            if abs(local_fwd) >= abs(local_right):
                target_clip = run_fwd if local_fwd >= 0.0 else run_back
            else:
                target_clip = run_right if local_right >= 0.0 else run_left
            speed = 1.4 if is_running else 1.0
            self.walk_time += dt * speed
        else:
            target_clip = idle_clip
            self.idle_time += dt

        # State transition
        if target_clip != self.current_clip:
            self.prev_clip = self.current_clip
            self.current_clip = target_clip
            self.blend_weight = 0.0
            if is_attacking:
                self.attack_time = 0.0

        if not is_attacking:
            self.attack_time = 0.0

        self.blend_weight = min(1.0, self.blend_weight + dt / self.blend_duration)

        def get_clip_time(clip):
            if clip in (self.AXE_ATTACK, self.GUN_SHOOT): return self.attack_time
            if clip in (self.AXE_RUN_FWD, self.AXE_RUN_BACK, self.AXE_RUN_LEFT, self.AXE_RUN_RGHT,
                        self.GUN_RUN_FWD, self.GUN_RUN_BACK, self.GUN_RUN_LEFT, self.GUN_RUN_RGHT):
                return self.walk_time
            return self.idle_time

        if self.blend_weight < 1.0 and self.prev_clip != self.current_clip:
            character.set_pose_blended(
                self.prev_clip, get_clip_time(self.prev_clip),
                self.current_clip, get_clip_time(self.current_clip),
                self.blend_weight
            )
        else:
            character.set_pose_time(self.current_clip, get_clip_time(self.current_clip))

        return self.current_clip

    def on_attack_start(self):
        self.attack_count += 1
        self.attack_time = 0.0'''

assert old_controller in text, 'old_controller not found'
text = text.replace(old_controller, new_controller)

# -------------------------------------------------------------
# 2. Update draw_rigged_character and draw_weapon_fp
# -------------------------------------------------------------
old_draw_rigged = '''def draw_rigged_character(character, tex, head_dl, hair_dl, axe_dl, axe_tex, yaw):
    glEnable(GL_TEXTURE_2D)
    glColor3f(1.0, 1.0, 1.0)
    glPushMatrix()
    glRotatef(math.degrees(yaw), 0, 1, 0)
    character.render(tex, head_dl, hair_dl, axe_dl, axe_tex)
    glPopMatrix()'''

new_draw_rigged = '''def draw_rigged_character(character, tex, head_dl, hair_dl, axe_dl, axe_tex,
                          gun_dl, gun_tex, equipped_slot, yaw):
    glEnable(GL_TEXTURE_2D)
    glColor3f(1.0, 1.0, 1.0)
    glPushMatrix()
    glRotatef(math.degrees(yaw), 0, 1, 0)
    character.render(tex, head_dl, hair_dl, axe_dl, axe_tex,
                     gun_dl=gun_dl, gun_tex=gun_tex, equipped_slot=equipped_slot)
    glPopMatrix()'''

assert old_draw_rigged in text, 'old_draw_rigged not found'
text = text.replace(old_draw_rigged, new_draw_rigged)

old_draw_fp = '''def draw_weapon_fp(axe_dl, axe_tex, bob_x, bob_y, attack_t, is_attacking, W, H):
    if not axe_dl: return
    glClear(GL_DEPTH_BUFFER_BIT)
    glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
    gluPerspective(50, W / H, 0.01, 10)
    glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()

    swing = math.sin(attack_t * math.pi) * -70.0 if is_attacking else 0.0
    glTranslatef(0.24 + bob_x, -0.28 + bob_y, -0.48)
    glRotatef(swing, 1, 0, 0)
    glRotatef(-15, 0, 1, 0)
    glRotatef(75, 1, 0, 0)
    glScalef(0.9, 0.9, 0.9)

    glEnable(GL_TEXTURE_2D)
    glBindTexture(GL_TEXTURE_2D, axe_tex)
    glColor3f(1.0, 1.0, 1.0)
    glCallList(axe_dl)

    glPopMatrix()
    glMatrixMode(GL_PROJECTION); glPopMatrix()
    glMatrixMode(GL_MODELVIEW)'''

new_draw_fp = '''def draw_weapon_fp(axe_dl, axe_tex, gun_dl, gun_tex, equipped_slot,
                   bob_x, bob_y, attack_t, is_attacking, is_aiming, W, H):
    glClear(GL_DEPTH_BUFFER_BIT)
    glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
    fov = 32 if is_aiming else 50
    gluPerspective(fov, W / H, 0.01, 10)
    glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()

    if equipped_slot == 1 and gun_dl:
        # M4A1 First-Person Viewmodel
        recoil = math.sin(attack_t * math.pi) * 0.04 if is_attacking else 0.0
        recoil_rot = math.sin(attack_t * math.pi) * 12.0 if is_attacking else 0.0
        
        if is_aiming:
            # Centered ADS sight picture
            glTranslatef(0.0 + bob_x * 0.2, -0.16 + bob_y * 0.2, -0.32 + recoil)
            glRotatef(-recoil_rot, 1, 0, 0)
            glRotatef(90, 0, 1, 0)
            glRotatef(-90, 1, 0, 0)
        else:
            # Hip-fire over-the-right-hand viewmodel
            glTranslatef(0.22 + bob_x, -0.22 + bob_y, -0.42 + recoil)
            glRotatef(-recoil_rot, 1, 0, 0)
            glRotatef(90, 0, 1, 0)
            glRotatef(-90, 1, 0, 0)
            glRotatef(-8, 0, 0, 1)

        glScalef(0.85, 0.85, 0.85)
        glEnable(GL_TEXTURE_2D)
        glBindTexture(GL_TEXTURE_2D, gun_tex)
        glColor3f(1.0, 1.0, 1.0)
        glCallList(gun_dl)
    elif axe_dl:
        # Axe First-Person Viewmodel
        swing = math.sin(attack_t * math.pi) * -70.0 if is_attacking else 0.0
        glTranslatef(0.24 + bob_x, -0.28 + bob_y, -0.48)
        glRotatef(swing, 1, 0, 0)
        glRotatef(-15, 0, 1, 0)
        glRotatef(75, 1, 0, 0)
        glScalef(0.9, 0.9, 0.9)

        glEnable(GL_TEXTURE_2D)
        glBindTexture(GL_TEXTURE_2D, axe_tex)
        glColor3f(1.0, 1.0, 1.0)
        glCallList(axe_dl)

    glPopMatrix()
    glMatrixMode(GL_PROJECTION); glPopMatrix()
    glMatrixMode(GL_MODELVIEW)'''

assert old_draw_fp in text, 'old_draw_fp not found'
text = text.replace(old_draw_fp, new_draw_fp)

# -------------------------------------------------------------
# 3. Update FastHUD with weapon textures, aim button, ammo counter
# -------------------------------------------------------------
old_hud_tex = '''        self.tex_btn_prone  = load_texture(u('1BattlePanel_cz_pa.png'))'''
new_hud_tex = '''        self.tex_btn_prone  = load_texture(u('1BattlePanel_cz_pa.png'))
        self.tex_btn_aim    = load_texture(u('BattlePanel_aim_button.png'))'''

assert old_hud_tex in text, 'old_hud_tex not found'
text = text.replace(old_hud_tex, new_hud_tex)

old_hud_guns = '''        self.tex_axe        = load_texture(u('wq_shifu.png'))
        self.tex_juice      = load_texture(u('BattlePanel2_YINGSHUI2.png'))'''

new_hud_guns = '''        self.tex_axe        = load_texture(u('wq_shifu.png'))
        self.tex_m4         = load_texture(u('m4_icon.png'))
        self.tex_juice      = load_texture(u('BattlePanel2_YINGSHUI2.png'))'''

assert old_hud_guns in text, 'old_hud_guns not found'
text = text.replace(old_hud_guns, new_hud_guns)

old_ammo_tex = '''        self.tex_chat     = glGenTextures(1)

        self.last_hp = -1'''

new_ammo_tex = '''        self.tex_chat     = glGenTextures(1)
        self.tex_ammo     = glGenTextures(1)

        self.last_hp = -1
        self.last_ammo = -1'''

assert old_ammo_tex in text, 'old_ammo_tex not found'
text = text.replace(old_ammo_tex, new_ammo_tex)

# FastHUD draw signature update
old_draw_sig = '''    def draw(self, W, H, fps, health, stamina, wood, metal, stone, sulfur, px, pz, yaw, is_3p, msg, msg_t, dt=0.016, food=272.0, water=266.0, target_node=None, joy_input=(0.0, 0.0), btn_pressed=None):'''

new_draw_sig = '''    def draw(self, W, H, fps, health, stamina, wood, metal, stone, sulfur, px, pz, yaw, is_3p, msg, msg_t, dt=0.016, food=272.0, water=266.0, target_node=None, joy_input=(0.0, 0.0), btn_pressed=None, equipped_slot=0, is_aiming=False, ammo_cur=30, ammo_max=30):'''

assert old_draw_sig in text, 'old_draw_sig not found'
text = text.replace(old_draw_sig, new_draw_sig)

# Hotbar item list update
old_hotbar_items = '''        # Pure APK items: 1:Axe, 2:Juice, 3:Fish meat, 4:Bandage, 5:Sulfur, 6:Wood/Metal
        item_icons = [
            self.tex_axe,
            self.tex_juice,
            self.tex_meat,
            self.tex_bandage,
            self.tex_sulfur if sulfur > 0 else self.tex_metal,
            self.tex_wood if wood > 0 else 0
        ]'''

new_hotbar_items = '''        # Pure APK items: 1:Stone Axe, 2:M4A1 Rifle, 3:Juice, 4:Bandage, 5:Sulfur, 6:Wood/Metal
        item_icons = [
            self.tex_axe,
            self.tex_m4,
            self.tex_juice,
            self.tex_bandage,
            self.tex_sulfur if sulfur > 0 else self.tex_metal,
            self.tex_wood if wood > 0 else 0
        ]'''

assert old_hotbar_items in text, 'old_hotbar_items not found'
text = text.replace(old_hotbar_items, new_hotbar_items)

# Hotbar active indicator update
old_hotbar_active = '''            # Active Slot indicator (Slot 1): lime-green left border & orange line below
            if idx == 0:
                glDisable(GL_TEXTURE_2D)
                glColor4f(0.78, 0.92, 0.12, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy)
                glVertex2f(sx + 3, hy)
                glVertex2f(sx + 3, hy + slot_sz)
                glVertex2f(sx, hy + slot_sz)
                glEnd()

                glColor4f(0.96, 0.55, 0.18, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(3 * SCALE))
                glVertex2f(sx, hy - int(3 * SCALE))
                glEnd()'''

new_hotbar_active = '''            # Active Slot indicator (highlights equipped slot): lime-green left border & orange line below
            if idx == equipped_slot:
                glDisable(GL_TEXTURE_2D)
                glColor4f(0.78, 0.92, 0.12, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy)
                glVertex2f(sx + 3, hy)
                glVertex2f(sx + 3, hy + slot_sz)
                glVertex2f(sx, hy + slot_sz)
                glEnd()

                glColor4f(0.96, 0.55, 0.18, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(3 * SCALE))
                glVertex2f(sx, hy - int(3 * SCALE))
                glEnd()'''

assert old_hotbar_active in text, 'old_hotbar_active not found'
text = text.replace(old_hotbar_active, new_hotbar_active)

# Action controls (add Aim button and ammo counter display)
old_action_controls = '''        # Prone (1BattlePanel_cz_pa.png)
        if self.tex_btn_prone:
            glEnable(GL_TEXTURE_2D)
            _pr_c = 0.55 if 'prone' in btn_pressed else 1.0
            glColor4f(_pr_c, _pr_c, _pr_c, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_prone)
            px_b = bx_att - int(44 * SCALE)
            py_b = int(22 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(px_b, py_b)
            glTexCoord2f(1, 0); glVertex2f(px_b + btn_sz, py_b)
            glTexCoord2f(1, 1); glVertex2f(px_b + btn_sz, py_b + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(px_b, py_b + btn_sz)
            glEnd()'''

new_action_controls = '''        # Prone (1BattlePanel_cz_pa.png)
        if self.tex_btn_prone:
            glEnable(GL_TEXTURE_2D)
            _pr_c = 0.55 if 'prone' in btn_pressed else 1.0
            glColor4f(_pr_c, _pr_c, _pr_c, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_prone)
            px_b = bx_att - int(44 * SCALE)
            py_b = int(22 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(px_b, py_b)
            glTexCoord2f(1, 0); glVertex2f(px_b + btn_sz, py_b)
            glTexCoord2f(1, 1); glVertex2f(px_b + btn_sz, py_b + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(px_b, py_b + btn_sz)
            glEnd()

        # ADS Aim button (appears/highlights when weapon equipped or aiming)
        if self.tex_btn_aim and equipped_slot == 1:
            glEnable(GL_TEXTURE_2D)
            _aim_c = 0.55 if is_aiming or 'aim' in btn_pressed else 1.0
            glColor4f(_aim_c, _aim_c, _aim_c, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_aim)
            ax_pos = bx_att - int(54 * SCALE)
            ay_pos = by_att + int(42 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(ax_pos, ay_pos)
            glTexCoord2f(1, 0); glVertex2f(ax_pos + btn_sz, ay_pos)
            glTexCoord2f(1, 1); glVertex2f(ax_pos + btn_sz, ay_pos + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(ax_pos, ay_pos + btn_sz)
            glEnd()

        # Ammo Counter display for Rifle
        if equipped_slot == 1:
            if ammo_cur != self.last_ammo:
                self.last_ammo = ammo_cur
                s_ammo = pygame.Surface((128, 32), pygame.SRCALPHA); s_ammo.fill((0, 0, 0, 0))
                t_ammo = FONT_TEXT.render(f"{ammo_cur} / {ammo_max}", True, (255, 230, 120))
                s_ammo.blit(t_ammo, (0, 0))
                self._render_text_to_tex(self.tex_ammo, s_ammo, 128, 32)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            self._draw_sub_tex(self.tex_ammo, bx_att - int(10 * SCALE), by_att - int(18 * SCALE),
                               int(80 * SCALE), int(16 * SCALE), 80, 20, 128, 32)'''

assert old_action_controls in text, 'old_action_controls not found'
text = text.replace(old_action_controls, new_action_controls)

with open(path, 'w', encoding='utf-8') as f:
    f.write(text)

print('Successfully updated AuthenticAnimController and FastHUD in viewer3d.py!')
