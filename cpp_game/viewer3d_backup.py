"""
Last Day Rules: Survival â€” True Skeletal Rigged 3D Engine
=========================================================
Features:
- Anatomical articulated skeleton with bending knees (Calf) and bending elbows (Forearm).
- Submeshes extracted directly from Unity SkinnedMeshRenderers using vertex bone weights.
- Driven by actual sampled muscle curves extracted from Last Day Rules: Survival APK:
    * kongshou_zhan_run_front01: Thigh pitch (C31, C47) + Knee bend (C37, C53) + Arm swing (C57, C46)
    * fu_zhan_attack_01: Axe strike swing (C126, C94, C128)
- Axe firmly gripped in right hand and swings realistically with the forearm & upper arm.
- 60 FPS optimized for Intel HD Graphics / OpenGL 1.1 / GDI Generic.
"""
import math
import os
import json
import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *
from PIL import Image
from skinned_character import SkinnedCharacter

def _next_pot(n):
    p = 1
    while p < n: p <<= 1
    return p

def load_texture(path):
    if not os.path.exists(path):
        return 0
    im = Image.open(path).convert('RGBA')
    w2 = min(1024, _next_pot(im.width))
    h2 = min(1024, _next_pot(im.height))
    if im.width != w2 or im.height != h2:
        im = im.resize((w2, h2), Image.LANCZOS)
    data = im.tobytes("raw", "RGBA", 0, -1)
    tid = glGenTextures(1)
    glBindTexture(GL_TEXTURE_2D, tid)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT)
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, im.width, im.height,
                 0, GL_RGBA, GL_UNSIGNED_BYTE, data)
    return tid

def load_obj(path, swap_yz=False):
    """Loads OBJ. Note: parts exported by build_rigged_parts.py are ALREADY converted to OpenGL space."""
    if not os.path.exists(path):
        return None
    verts, uvs, norms, faces = [], [], [], []
    with open(path, 'r', encoding='utf-8', errors='ignore') as f:
        for line in f:
            tok = line.split()
            if not tok: continue
            if tok[0] == 'v':
                x, y, z = float(tok[1]), float(tok[2]), float(tok[3])
                if swap_yz: x, y, z = x, z, -y
                verts.append((x, y, z))
            elif tok[0] == 'vt':
                uvs.append((float(tok[1]), float(tok[2])))
            elif tok[0] == 'vn':
                nx, ny, nz = float(tok[1]), float(tok[2]), float(tok[3])
                if swap_yz: nx, ny, nz = nx, nz, -ny
                norms.append((nx, ny, nz))
            elif tok[0] == 'f':
                poly = []
                for w in tok[1:]:
                    p = w.split('/')
                    vi = int(p[0]) - 1
                    ti = int(p[1]) - 1 if len(p) > 1 and p[1] else -1
                    ni = int(p[2]) - 1 if len(p) > 2 and p[2] else -1
                    poly.append((vi, ti, ni))
                faces.append(poly)

    dl = glGenLists(1)
    glNewList(dl, GL_COMPILE)
    glBegin(GL_TRIANGLES)
    for poly in faces:
        for i in range(1, len(poly) - 1):
            for vi, ti, ni in [poly[0], poly[i], poly[i+1]]:
                if ni >= 0 and ni < len(norms): glNormal3f(*norms[ni])
                if ti >= 0 and ti < len(uvs):   glTexCoord2f(*uvs[ti])
                if vi < len(verts):              glVertex3f(*verts[vi])
    glEnd()
    glEndList()
    return dl

# ------------------------------------------------------------------ #
#  Authentic Animation Controller â€” uses real Unity clip data
# ------------------------------------------------------------------ #
def lerp(a, b, t): return a + (b - a) * t


class AuthenticAnimController:
    """
    Selects the correct authentic animation clip based on game state.
    Provides buttery smooth sub-frame playback and 0.15s cross-fading between states.
    """
    IDLE_CLIP    = 'zhan_stand01'
    RUN_CLIP     = 'zhan_run_front01'
    ATTACK_CLIP  = 'fu_zhan_attack_01'

    def __init__(self):
        self.idle_time      = 0.0
        self.run_time       = 0.0
        self.attack_time    = 0.0
        self.current_clip   = self.IDLE_CLIP
        self.prev_clip      = self.IDLE_CLIP
        self.blend_weight   = 1.0
        self.blend_duration = 0.15
        self.attack_count   = 0

    def update(self, character, dt, is_moving, is_running, is_grounded,
               is_attacking, attack_t, jump_t):
        if is_attacking:
            target_clip = self.ATTACK_CLIP
            self.attack_time += dt * 1.3
        elif is_moving and is_grounded:
            target_clip = self.RUN_CLIP
            speed = 1.4 if is_running else 1.0
            self.run_time += dt * speed
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
            if clip == self.RUN_CLIP:    return self.run_time
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
        self.attack_time = 0.0


# ------------------------------------------------------------------ #
#  Supreme Skinned Mesh Character Renderer (OpenGL 1.1)
# ------------------------------------------------------------------ #
def draw_rigged_character(character, tex, head_dl, hair_dl, axe_dl, axe_tex, yaw):
    glEnable(GL_TEXTURE_2D)
    glColor3f(1.0, 1.0, 1.0)
    glPushMatrix()
    glRotatef(math.degrees(yaw), 0, 1, 0)
    character.render(tex, head_dl, hair_dl, axe_dl, axe_tex)
    glPopMatrix()

# ------------------------------------------------------------------ #
#  First-person Axe Viewmodel
# ------------------------------------------------------------------ #
def draw_weapon_fp(axe_dl, axe_tex, bob_x, bob_y, attack_t, is_attacking, W, H):
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
    glMatrixMode(GL_MODELVIEW)

# ------------------------------------------------------------------ #
#  Ground & Environment
# ------------------------------------------------------------------ #
_ground_dl = None
def draw_ground():
    global _ground_dl
    if _ground_dl is None:
        _ground_dl = glGenLists(1)
        glNewList(_ground_dl, GL_COMPILE)
        glDisable(GL_TEXTURE_2D)
        glColor3f(0.32, 0.52, 0.25)
        glBegin(GL_QUADS)
        glNormal3f(0, 1, 0)
        s = 400
        glVertex3f(-s, 0, -s); glVertex3f(-s, 0, s)
        glVertex3f( s, 0,  s); glVertex3f( s, 0, -s)
        glEnd()
        glColor3f(0.25, 0.44, 0.20)
        glBegin(GL_LINES)
        for i in range(-160, 161, 8):
            glVertex3f(i, 0.02, -160); glVertex3f(i, 0.02, 160)
            glVertex3f(-160, 0.02, i); glVertex3f(160, 0.02, i)
        glEnd()
        glEndList()
    glCallList(_ground_dl)

# ------------------------------------------------------------------ #
#  Fast, Solid 2D OpenGL HUD
# ------------------------------------------------------------------ #
pygame.font.init()
FONT_TITLE = pygame.font.SysFont('Segoe UI', 18, bold=True)
FONT_TEXT  = pygame.font.SysFont('Segoe UI', 14)
FONT_SM    = pygame.font.SysFont('Segoe UI', 12)

class FastHUD:
    def __init__(self, asset_dir):
        u = lambda f: os.path.join(asset_dir, 'ui_hud', f)
        self.tex_cross = load_texture(u('BattlePanel_cxfmt_zhunxing.png'))
        self.tex_map   = load_texture(u('1BattlePanel_map_di_2_1.png'))
        # 100% Authentic Original BattlePanel HUD Sprites (from assets/uiatlas/battlepanel.ab):
        self.tex_hp      = load_texture(u('1BattlePanel_buff_xue.png'))
        self.tex_food    = load_texture(u('1BattlePanel_buff_jie.png'))
        self.tex_drop    = load_texture(u('1BattlePanel_buff_shui.png'))
        self.tex_stam    = load_texture(u('new_common_tili.png'))
        self.tex_plus    = load_texture(u('1BattlePanel_buff_xue_jia.png'))
        self.tex_slot    = load_texture(u('com_item_kuang_2.png'))
        self.tex_sel     = load_texture(u('com_chose_kuang.png'))
        self.tex_bag     = load_texture(u('common_beibao.png'))
        self.tex_compass = load_texture(u('battlepanel_kedu_yw.png'))

        # Authentic Original Item Icons
        self.tex_axe    = load_texture(u('wq_shifu.png'))
        self.tex_wood   = load_texture(u('tb_cl_mt.png'))
        self.tex_metal  = load_texture(u('tb_cl_tiek.png'))
        self.tex_stone  = load_texture(u('ks_st.png'))
        self.tex_sulfur = load_texture(u('ks_lhk.png'))

        # 100% Authentic Status Bar Textures (1BattlePanel_xue_bg_1 & 1BattlePanel_di_cb)
        sliced_path = u('1BattlePanel_xue_bg_1_sliced.png')
        if not os.path.exists(sliced_path) or os.path.getsize(sliced_path) == 0:
            try:
                from PIL import Image
                src_bg = Image.open(u('1BattlePanel_xue_bg_1.png')).convert('RGBA')
                b_l, b_r, b_t, b_b = 10, 10, 10, 10
                sw, sh = src_bg.size
                tw, th = 170, 24
                top_left = src_bg.crop((0, 0, b_l, b_t))
                top_right = src_bg.crop((sw - b_r, 0, sw, b_t))
                bot_left = src_bg.crop((0, sh - b_b, b_l, sh))
                bot_right = src_bg.crop((sw - b_r, sh - b_b, sw, sh))
                top_edge = src_bg.crop((b_l, 0, sw - b_r, b_t)).resize((tw - b_l - b_r, b_t), Image.BILINEAR)
                bot_edge = src_bg.crop((b_l, sh - b_b, sw - b_r, sh)).resize((tw - b_l - b_r, b_b), Image.BILINEAR)
                left_edge = src_bg.crop((0, b_t, b_l, sh - b_b)).resize((b_l, th - b_t - b_b), Image.BILINEAR)
                right_edge = src_bg.crop((sw - b_r, b_t, sw, sh - b_b)).resize((b_r, th - b_t - b_b), Image.BILINEAR)
                center = src_bg.crop((b_l, b_t, sw - b_r, sh - b_b)).resize((tw - b_l - b_r, th - b_t - b_b), Image.BILINEAR)
                res = Image.new('RGBA', (tw, th), (0, 0, 0, 0))
                res.paste(top_left, (0, 0)); res.paste(top_edge, (b_l, 0)); res.paste(top_right, (tw - b_r, 0))
                res.paste(left_edge, (0, b_t)); res.paste(center, (b_l, b_t)); res.paste(right_edge, (tw - b_r, b_t))
                res.paste(bot_left, (0, th - b_b)); res.paste(bot_edge, (b_l, th - b_b)); res.paste(bot_right, (tw - b_r, th - b_b))
                res.save(sliced_path)
            except Exception as e:
                print("Could not slice 9-slice bg:", e)
        self.tex_bar_bg   = load_texture(sliced_path)
        self.tex_bar_fill = load_texture(u('1BattlePanel_di_cb.png'))

        self.tex_info   = glGenTextures(1)
        self.tex_hotbar = glGenTextures(1)
        self.tex_msg    = glGenTextures(1)
        self.tex_vitals = glGenTextures(1)

        self.last_wood = -1
        self.last_metal = -1
        self.last_stone = -1
        self.last_sulfur = -1
        self.last_hp = -1
        self.last_stam = -1
        self.last_food = -1
        self.last_water = -1
        self.info_timer = 999.0  # Force immediate first render
        self.last_3p = None
        self.cached_fps = 60
        self.cached_px = 0.0
        self.cached_pz = 0.0
        self.last_msg = None

    def _render_text_to_tex(self, tid, surf, pot_w, pot_h):
        glBindTexture(GL_TEXTURE_2D, tid)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
        data = pygame.image.tostring(surf, 'RGBA', True)
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, pot_w, pot_h, 0, GL_RGBA, GL_UNSIGNED_BYTE, data)

    def draw(self, W, H, fps, health, stamina, wood, metal, stone, sulfur, px, pz, yaw, is_3p, msg, msg_t, dt=0.016, food=100.0, water=100.0):
        glDisable(GL_DEPTH_TEST)
        glDisable(GL_LIGHTING)

        glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
        glOrtho(0, W, 0, H, -1, 1)
        glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()

        glEnable(GL_BLEND)
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA)

        # 1. Authentic Diamond Crosshair (Center)
        if self.tex_cross:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_cross)
            ccx, ccy = W // 2, H // 2
            cs = 16
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(ccx - cs, ccy - cs)
            glTexCoord2f(1, 0); glVertex2f(ccx + cs, ccy - cs)
            glTexCoord2f(1, 1); glVertex2f(ccx + cs, ccy + cs)
            glTexCoord2f(0, 1); glVertex2f(ccx - cs, ccy + cs)
            glEnd()

        # 2. Authentic Top Compass Bar (m_kedu from BattlePanel.ab)
        if self.tex_compass:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_compass)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT)

            comp_w, comp_h = 360, 24
            comp_x = W // 2 - comp_w // 2
            comp_y = H - comp_h - 10

            # Map yaw (radians) to normalized texture coordinates [0, 1]
            u_center = (-yaw / (2.0 * math.pi)) % 1.0
            u_span = 0.175  # ~63 degrees FOV display
            u0 = u_center - u_span / 2.0
            u1 = u_center + u_span / 2.0

            glBegin(GL_QUADS)
            glTexCoord2f(u0, 0.0); glVertex2f(comp_x, comp_y)
            glTexCoord2f(u1, 0.0); glVertex2f(comp_x + comp_w, comp_y)
            glTexCoord2f(u1, 1.0); glVertex2f(comp_x + comp_w, comp_y + comp_h)
            glTexCoord2f(u0, 1.0); glVertex2f(comp_x, comp_y + comp_h)
            glEnd()

            # Center indicator triangle pointing at heading
            glDisable(GL_TEXTURE_2D)
            glColor4f(0.95, 0.2, 0.2, 1.0)
            mid_x = comp_x + comp_w // 2
            glBegin(GL_TRIANGLES)
            glVertex2f(mid_x, comp_y + 4)
            glVertex2f(mid_x - 5, comp_y - 4)
            glVertex2f(mid_x + 5, comp_y - 4)
            glEnd()

        # 3. Authentic Circular Minimap (Top-Right, m_map from BattlePanel.ab)
        mw, mh = 105, 105
        mx, my = W - mw - 18, H - mh - 18
        if self.tex_map:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_map)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(mx, my)
            glTexCoord2f(1, 0); glVertex2f(mx + mw, my)
            glTexCoord2f(1, 1); glVertex2f(mx + mw, my + mh)
            glTexCoord2f(0, 1); glVertex2f(mx, my + mh)
            glEnd()

            mcx, mcy = mx + mw // 2, my + mh // 2
            glDisable(GL_TEXTURE_2D)
            glColor4f(0.2, 0.9, 0.3, 1.0)
            glPointSize(6.0)
            glBegin(GL_POINTS); glVertex2f(mcx, mcy); glEnd()
            nx = mcx + math.sin(-yaw) * 42
            ny = mcy + math.cos(-yaw) * 42
            glColor4f(0.9, 0.2, 0.2, 1.0)
            glPointSize(8.0)
            glBegin(GL_POINTS); glVertex2f(nx, ny); glEnd()

        # 4. Authentic Vital Status Bars (Top-Right next to Minimap, xue_je_fs from BattlePanel.ab)
        # Replicating the exact decompiled BattlePanel.prefab layout:
        # Health (m_xue), Hunger (m_jie), Water (m_bodyWater), Stamina
        bar_w, bar_h = 150, 22
        badge_w, badge_h = 22, 22
        spacing = 26
        bx = mx - bar_w - badge_w - 14
        by_top = H - 38

        vitals = [
            (self.tex_hp,   health,  100.0, (0.85, 0.20, 0.20, 0.95) if health <= 35 else (0.21, 0.48, 0.31, 0.95)),
            (self.tex_food, food,    100.0, (1.0, 0.59, 0.19, 0.95)),
            (self.tex_drop, water,   100.0, (0.15, 0.65, 0.90, 0.95)),
            (self.tex_stam, stamina, 100.0, (0.96, 0.75, 0.14, 0.95)),
        ]

        glEnable(GL_TEXTURE_2D)
        for i, (icon_tex, cur_val, max_val, col) in enumerate(vitals):
            by = by_top - i * spacing
            fill_pct = max(0.0, min(1.0, cur_val / max_val))

            # Left Badge Icon (1BattlePanel_buff_*)
            if icon_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, icon_tex)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bx, by)
                glTexCoord2f(1, 0); glVertex2f(bx + badge_w, by)
                glTexCoord2f(1, 1); glVertex2f(bx + badge_w, by + badge_h)
                glTexCoord2f(0, 1); glVertex2f(bx, by + badge_h)
                glEnd()

            bar_x = bx + badge_w + 2

            # Authentic Bar Frame (1BattlePanel_xue_bg_1)
            if self.tex_bar_bg:
                glColor4f(1.0, 1.0, 1.0, 0.95)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_bg)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x, by)
                glTexCoord2f(1, 0); glVertex2f(bar_x + bar_w, by)
                glTexCoord2f(1, 1); glVertex2f(bar_x + bar_w, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(bar_x, by + bar_h)
                glEnd()

            # Authentic Bar Fill Texture (1BattlePanel_di_cb)
            if self.tex_bar_fill and fill_pct > 0:
                fw = (bar_w - 26) * fill_pct
                glColor4f(*col)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_fill)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x + 4, by + 2)
                glTexCoord2f(fill_pct * 3.0, 0); glVertex2f(bar_x + 4 + fw, by + 2)
                glTexCoord2f(fill_pct * 3.0, 1); glVertex2f(bar_x + 4 + fw, by + bar_h - 2)
                glTexCoord2f(0, 1); glVertex2f(bar_x + 4, by + bar_h - 2)
                glEnd()

            # Authentic Plus Button on right edge (1BattlePanel_buff_xue_jia)
            if self.tex_plus:
                glColor4f(1.0, 1.0, 1.0, 0.9)
                glBindTexture(GL_TEXTURE_2D, self.tex_plus)
                pw, ph = 13, 13
                px_pos = bar_x + bar_w - pw - 5
                py_pos = by + (bar_h - ph) // 2
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(px_pos, py_pos)
                glTexCoord2f(1, 0); glVertex2f(px_pos + pw, py_pos)
                glTexCoord2f(1, 1); glVertex2f(px_pos + pw, py_pos + ph)
                glTexCoord2f(0, 1); glVertex2f(px_pos, py_pos + ph)
                glEnd()

        # Update numbers overlay on vital bars when values change
        ihp = int(health); istam = int(stamina); ifood = int(food); iwater = int(water)
        if (ihp != self.last_hp or istam != self.last_stam or
            ifood != self.last_food or iwater != self.last_water):
            self.last_hp = ihp; self.last_stam = istam
            self.last_food = ifood; self.last_water = iwater
            surf_vit = pygame.Surface((256, 128), pygame.SRCALPHA)
            surf_vit.fill((0, 0, 0, 0))
            nums = [
                (0, f"{ihp}/100"),
                (1, f"{ifood}/100"),
                (2, f"{iwater}/100"),
                (3, f"{istam}/100"),
            ]
            for idx, text in nums:
                sy = idx * spacing + 3
                stxt = FONT_SM.render(text, True, (255, 255, 255))
                surf_vit.blit(stxt, ((bar_w - 20) // 2 - stxt.get_width() // 2, sy))
            self._render_text_to_tex(self.tex_vitals, surf_vit, 256, 128)

        # Draw vital numbers quad over bars
        glColor4f(1.0, 1.0, 1.0, 1.0)
        glBindTexture(GL_TEXTURE_2D, self.tex_vitals)
        glBegin(GL_QUADS)
        glTexCoord2f(0.0, 4 * spacing / 128.0); glVertex2f(bx + badge_w + 2, by_top - 3 * spacing)
        glTexCoord2f(bar_w / 256.0, 4 * spacing / 128.0); glVertex2f(bx + badge_w + 2 + bar_w, by_top - 3 * spacing)
        glTexCoord2f(bar_w / 256.0, 0.0); glVertex2f(bx + badge_w + 2 + bar_w, by_top + bar_h)
        glTexCoord2f(0.0, 0.0); glVertex2f(bx + badge_w + 2, by_top + bar_h)
        glEnd()

        # 4. Authentic Hotbar Quick-Slots (Bottom-Center)
        num_slots = 6
        slot_size = 52
        gap = 8
        total_hotbar_w = num_slots * slot_size + (num_slots - 1) * gap
        hx = W // 2 - total_hotbar_w // 2
        hy = 18

        if self.tex_bag:
            glBindTexture(GL_TEXTURE_2D, self.tex_bag)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(hx - 48, hy + 6)
            glTexCoord2f(1, 0); glVertex2f(hx - 8, hy + 6)
            glTexCoord2f(1, 1); glVertex2f(hx - 8, hy + 46)
            glTexCoord2f(0, 1); glVertex2f(hx - 48, hy + 46)
            glEnd()

        item_icons = [
            self.tex_axe,
            self.tex_wood if wood > 0 else 0,
            self.tex_metal if metal > 0 else 0,
            self.tex_stone if stone > 0 else 0,
            self.tex_sulfur if sulfur > 0 else 0,
            0
        ]

        for idx in range(num_slots):
            sx = hx + idx * (slot_size + gap)
            if self.tex_slot:
                glBindTexture(GL_TEXTURE_2D, self.tex_slot)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx, hy)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_size, hy)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_size, hy + slot_size)
                glTexCoord2f(0, 1); glVertex2f(sx, hy + slot_size)
                glEnd()

            # Authentic original APK item icon
            if item_icons[idx]:
                glBindTexture(GL_TEXTURE_2D, item_icons[idx])
                pad = 7
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx + pad, hy + pad)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_size - pad, hy + pad)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_size - pad, hy + slot_size - pad)
                glTexCoord2f(0, 1); glVertex2f(sx + pad, hy + slot_size - pad)
                glEnd()

            if idx == 0 and self.tex_sel:
                glBindTexture(GL_TEXTURE_2D, self.tex_sel)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx - 3, hy - 3)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_size + 3, hy - 3)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_size + 3, hy + slot_size + 3)
                glTexCoord2f(0, 1); glVertex2f(sx - 3, hy + slot_size + 3)
                glEnd()

        # Hotbar text labels (only updated on count change)
        if (wood != self.last_wood or metal != self.last_metal or
            stone != self.last_stone or sulfur != self.last_sulfur):
            self.last_wood = wood; self.last_metal = metal
            self.last_stone = stone; self.last_sulfur = sulfur
            surf_hot = pygame.Surface((512, 64), pygame.SRCALPHA)
            surf_hot.fill((0, 0, 0, 0))
            items = [
                ('1', ''),
                ('2', str(wood) if wood > 0 else ''),
                ('3', str(metal) if metal > 0 else ''),
                ('4', str(stone) if stone > 0 else ''),
                ('5', str(sulfur) if sulfur > 0 else ''),
                ('6', '')
            ]
            for idx, (snum, icnt) in enumerate(items):
                sx = idx * (slot_size + gap)
                s_num = FONT_SM.render(snum, True, (180, 180, 180))
                surf_hot.blit(s_num, (sx + 5, 34))
                if icnt:
                    s_cnt = FONT_TEXT.render(icnt, True, (255, 230, 80))
                    surf_hot.blit(s_cnt, (sx + slot_size - s_cnt.get_width() - 5, 30))
            self._render_text_to_tex(self.tex_hotbar, surf_hot, 512, 64)

        glBindTexture(GL_TEXTURE_2D, self.tex_hotbar)
        glBegin(GL_QUADS)
        glTexCoord2f(0.0, (64 - 52)/64.0); glVertex2f(hx, hy)
        glTexCoord2f(total_hotbar_w / 512.0, (64 - 52)/64.0); glVertex2f(hx + total_hotbar_w, hy)
        glTexCoord2f(total_hotbar_w / 512.0, 1.0); glVertex2f(hx + total_hotbar_w, hy + 52)
        glTexCoord2f(0.0, 1.0); glVertex2f(hx, hy + 52)
        glEnd()

        # 5. Top-Left Info Panel (throttled to 0.4s to prevent texture re-allocation stalls)
        self.info_timer += dt
        if self.info_timer >= 0.4 or is_3p != self.last_3p:
            self.info_timer = 0.0
            self.last_3p = is_3p
            self.cached_fps = fps
            self.cached_px = px
            self.cached_pz = pz

            surf = pygame.Surface((512, 128), pygame.SRCALPHA)
            surf.fill((0, 0, 0, 0))
            pygame.draw.rect(surf, (0, 0, 0, 160), (0, 0, 360, 105), border_radius=6)
            t1 = FONT_TITLE.render("LAST DAY RULES: SURVIVAL", True, (255, 215, 0))
            t2 = FONT_TEXT.render(f"FPS: {self.cached_fps}  |  Pos: ({self.cached_px:.1f}, {self.cached_pz:.1f})", True, (120, 255, 120))
            t3 = FONT_SM.render(f"Camera: {'3a Pessoa' if is_3p else '1a Pessoa'} [TAB]  |  Zoom [Scroll]", True, (255, 255, 120))
            t4 = FONT_SM.render("WASD: Mover | SHIFT: Correr | SPACE: Pular | Click: Coletar", True, (210, 210, 210))
            surf.blit(t1, (10, 8))
            surf.blit(t2, (10, 32))
            surf.blit(t3, (10, 56))
            surf.blit(t4, (10, 78))
            self._render_text_to_tex(self.tex_info, surf, 512, 128)

        glEnable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 1.0)
        glBindTexture(GL_TEXTURE_2D, self.tex_info)
        ix, iy = 15, H - 120
        glBegin(GL_QUADS)
        glTexCoord2f(0.0, (128 - 105)/128.0); glVertex2f(ix, iy)
        glTexCoord2f(360.0/512.0, (128 - 105)/128.0); glVertex2f(ix + 360, iy)
        glTexCoord2f(360.0/512.0, 1.0); glVertex2f(ix + 360, iy + 105)
        glTexCoord2f(0.0, 1.0); glVertex2f(ix, iy + 105)
        glEnd()

        # 6. Center Floating Notification (+10 Metal!, +15 Madeira!, etc.)
        if msg_t > 0 and msg:
            if msg != self.last_msg:
                self.last_msg = msg
                surf_msg = pygame.Surface((256, 64), pygame.SRCALPHA)
                surf_msg.fill((0, 0, 0, 0))
                pygame.draw.rect(surf_msg, (0, 0, 0, 180), (0, 0, 220, 45), border_radius=8)
                t_msg = FONT_TITLE.render(msg, True, (255, 240, 80))
                surf_msg.blit(t_msg, (15, 10))
                self._render_text_to_tex(self.tex_msg, surf_msg, 256, 64)

            glBindTexture(GL_TEXTURE_2D, self.tex_msg)
            mx_pos = W // 2 - 110
            my_pos = H // 2 - 80
            alpha = min(1.0, msg_t * 2.0)
            glColor4f(1.0, 1.0, 1.0, alpha)
            glBegin(GL_QUADS)
            glTexCoord2f(0.0, (64 - 45)/64.0); glVertex2f(mx_pos, my_pos)
            glTexCoord2f(220.0/256.0, (64 - 45)/64.0); glVertex2f(mx_pos + 220, my_pos)
            glTexCoord2f(220.0/256.0, 1.0); glVertex2f(mx_pos + 220, my_pos + 45)
            glTexCoord2f(0.0, 1.0); glVertex2f(mx_pos, my_pos + 45)
            glEnd()

        glDisable(GL_BLEND)
        glEnable(GL_DEPTH_TEST)
        glEnable(GL_LIGHTING)
        glMatrixMode(GL_PROJECTION); glPopMatrix()
        glMatrixMode(GL_MODELVIEW); glPopMatrix()

# ------------------------------------------------------------------ #
#  Main Execution
# ------------------------------------------------------------------ #
def main():
    pygame.init()
    W, H = 960, 540
    screen = pygame.display.set_mode((W, H), OPENGL | DOUBLEBUF | RESIZABLE)
    pygame.display.set_caption('Last Day Rules: Survival')

    glViewport(0, 0, W, H)
    glMatrixMode(GL_PROJECTION); glLoadIdentity()
    gluPerspective(65, W / H, 0.1, 500)
    glMatrixMode(GL_MODELVIEW)

    glEnable(GL_DEPTH_TEST)
    glEnable(GL_LIGHTING); glEnable(GL_LIGHT0)
    glLightfv(GL_LIGHT0, GL_POSITION, [100, 180, 100, 1])
    glLightfv(GL_LIGHT0, GL_AMBIENT,  [0.55, 0.55, 0.55, 1])
    glLightfv(GL_LIGHT0, GL_DIFFUSE,  [0.85, 0.85, 0.85, 1])
    glEnable(GL_COLOR_MATERIAL)
    glColorMaterial(GL_FRONT_AND_BACK, GL_AMBIENT_AND_DIFFUSE)
    glClearColor(0.53, 0.81, 0.92, 1.0)

    A = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'assets')
    def p(f): return os.path.join(A, f)

    print('Carregando texturas...')
    char_textures = {
        'head':    load_texture(p('man_head_chushijuese_01_ba.png')),
        'hair':    load_texture(p('man_hair_chushijuese_01_ba.png')),
        'clothes': load_texture(p('man_clothes_chushijuese_01_ba.png')),
        'skin':    load_texture(p('man_clothes_yf_0_ba.png')),
        'pants':   load_texture(p('man_trousers_chushijuese_01_ba.png')),
        'shoes':   load_texture(p('man_shoes_chushijuese_01_ba.png')),
    }
    axe_tex        = load_texture(p('wq_shifu_ba.png'))
    iron_ore_tex   = load_texture(p('ore_tiekuang_01_d.png'))
    sulfur_ore_tex = load_texture(p('ore_liuhuangkuang_01_d.png'))
    stone_ore_tex  = load_texture(p('ore_yanshikuang_01_d.png'))
    tree_tex       = load_texture(p('tree_baihuashu_03_d.png'))

    print('Carregando personagem com animacoes autenticas do APK original...')
    character = SkinnedCharacter()
    head_dl = load_obj(p('man_head_chushijuese_01.obj'), swap_yz=True)
    hair_dl = load_obj(p('man_hair_chushijuese_01.obj'), swap_yz=True)

    axe_dl        = load_obj(p('wq_shifu.obj'), swap_yz=False)
    iron_ore_dl   = load_obj(p('ore_tiekuang_01.obj'), swap_yz=False)
    sulfur_ore_dl = load_obj(p('ore_liuhuangkuang_01.obj'), swap_yz=False)
    stone_ore_dl  = load_obj(p('ore_yanshikuang_01.obj'), swap_yz=False)
    tree_dl       = load_obj(p('tree_baihuashu_03.obj'), swap_yz=False)
    tree_lod2_dl  = load_obj(p('tree_baihuashu_03_LOD2.obj'), swap_yz=False)

    TREES = [(-12, -8, 1.2), (15, 22, 1.1), (18, -18, 1.3), (-20, 10, 1.25), (6, -24, 1.15)]
    ORES = [
        # (x, z, type, dl, tex, scale, name, msg)
        (-4.0,  8.0,  'sulfur', sulfur_ore_dl, sulfur_ore_tex, 1.2, 'Minerio de Enxofre', '+10 Enxofre!'),
        ( 6.0,  7.0,  'metal',  iron_ore_dl,   iron_ore_tex,   1.2, 'Minerio de Ferro',   '+10 Metal!'),
        (-6.0, -10.0, 'stone',  stone_ore_dl,  stone_ore_tex,  1.2, 'Minerio de Pedra',   '+10 Pedra!'),
        (14.0, -8.0,  'metal',  iron_ore_dl,   iron_ore_tex,   1.2, 'Minerio de Ferro',   '+10 Metal!'),
    ]

    anim = AuthenticAnimController()
    hud = FastHUD(A)

    px, py, pz = 0.0, 0.0, 0.0
    yaw, pitch = 0.0, 0.0
    vert_vel, is_grounded = 0.0, True
    third_p, cam_dist = True, 3.8
    speed = 7.5
    walk_phase = 0.0
    is_moving, is_running = False, False
    attack_t, is_attacking = 0.0, False
    health, stamina = 100.0, 100.0
    food, water = 100.0, 100.0
    wood, metal, stone, sulfur = 0, 0, 0, 0
    msg, msg_t = '', 0.0
    jump_t = 0.0

    pygame.mouse.set_visible(False)
    pygame.event.set_grab(True)
    locked = True
    clock = pygame.time.Clock()

    print('Jogo pronto! WASD mover, SHIFT correr, SPACE pular, TAB camera, ESC cursor.')

    running = True
    while running:
        dt = min(clock.tick(60) / 1000.0, 0.05)

        for ev in pygame.event.get():
            if ev.type == QUIT:
                running = False
            elif ev.type == KEYDOWN:
                if ev.key == K_ESCAPE:
                    locked = not locked
                    pygame.mouse.set_visible(not locked)
                    pygame.event.set_grab(locked)
                elif ev.key in (K_TAB, K_v):
                    third_p = not third_p
                elif ev.key == K_F4:
                    running = False
            elif ev.type == MOUSEBUTTONDOWN and locked:
                if ev.button == 1 and not is_attacking:
                    is_attacking = True; attack_t = 0.0
                    anim.on_attack_start()
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
                elif ev.button == 5: cam_dist = min(9.0, cam_dist + 0.4)
            elif ev.type == VIDEORESIZE:
                W, H = ev.w, ev.h
                glViewport(0, 0, W, H)
                glMatrixMode(GL_PROJECTION); glLoadIdentity()
                gluPerspective(65, W / H, 0.1, 500)
                glMatrixMode(GL_MODELVIEW)

        if is_attacking:
            attack_t += dt * 1.0
            if attack_t >= 1.0:
                is_attacking = False; attack_t = 0.0

        if msg_t > 0: msg_t -= dt

        if locked:
            dx, dy = pygame.mouse.get_rel()
            yaw   += dx * 0.003
            pitch  = max(-1.35, min(1.35, pitch - dy * 0.003))

            keys = pygame.key.get_pressed()
            fx = math.sin(yaw); fz = math.cos(yaw)
            rx = math.cos(yaw); rz = -math.sin(yaw)
            mx, mz = 0.0, 0.0
            if keys[K_w]: mx += fx; mz += fz
            if keys[K_s]: mx -= fx; mz -= fz
            if keys[K_a]: mx -= rx; mz -= rz
            if keys[K_d]: mx += rx; mz += rz

            ml = math.hypot(mx, mz)
            is_moving  = ml > 0.01
            is_running = is_moving and keys[K_LSHIFT] and stamina > 5

            if is_running:  stamina = max(0.0, stamina - 22.0 * dt)
            else:           stamina = min(100.0, stamina + 14.0 * dt)
            food  = max(0.0, food  - dt * 0.15)
            water = max(0.0, water - dt * 0.25)

            if is_moving:
                cur_spd = speed * (1.8 if is_running else 1.0)
                mx /= ml; mz /= ml
                px += mx * cur_spd * dt
                pz += mz * cur_spd * dt
                walk_phase += dt * (1.8 if is_running else 1.0)

            if is_grounded and keys[K_SPACE]:
                vert_vel = 8.5; is_grounded = False; jump_t = 0.0
            if not is_grounded:
                jump_t   += dt
                vert_vel -= 24.0 * dt
                py       += vert_vel * dt
                if py <= 0:
                    py = 0; vert_vel = 0; is_grounded = True
        else:
            pygame.mouse.get_rel()

        # Update authentic animation with real Unity clip data
        anim.update(character, dt, is_moving, is_running, is_grounded, is_attacking, attack_t, jump_t)

        # ---- 3D Render ----
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
        glLoadIdentity()

        lx = math.sin(yaw) * math.cos(pitch)
        ly = math.sin(pitch)
        lz = math.cos(yaw) * math.cos(pitch)

        if third_p:
            # Authentic over-the-right-shoulder camera (reverse engineered from vThirdPersonCamera.cs)
            rx = math.cos(yaw)
            rz = -math.sin(yaw)
            shoulder_x = rx * 0.35
            shoulder_z = rz * 0.35
            tx = px + shoulder_x
            ty = py + 1.45
            tz = pz + shoulder_z

            cx = tx - lx * cam_dist
            cy = py + 1.65 - ly * cam_dist
            cz = tz - lz * cam_dist
            gluLookAt(cx, cy, cz, tx + lx * 2.0, ty + ly * 2.0, tz + lz * 2.0, 0, 1, 0)
        else:
            cx = px; cy = py + 1.7; cz = pz
            gluLookAt(cx, cy, cz, cx + lx, cy + ly, cz + lz, 0, 1, 0)

        draw_ground()

        # Authentic Ore Nodes (Iron, Sulfur, Stone)
        glEnable(GL_TEXTURE_2D); glColor3f(1.0, 1.0, 1.0)
        for ox, oz, _, odl, otex, osc, _, _ in ORES:
            if odl and otex:
                glPushMatrix()
                glTranslatef(ox, 0, oz)
                glScalef(osc, osc, osc)
                glBindTexture(GL_TEXTURE_2D, otex)
                glCallList(odl)
                glPopMatrix()

        # Trees with distance LOD
        for tx, tz, sc in TREES:
            d = math.hypot(tx - px, tz - pz)
            dl = tree_dl if d < 18.0 else tree_lod2_dl
            glPushMatrix()
            glTranslatef(tx, 0, tz); glScalef(sc, sc, sc)
            if dl:
                glBindTexture(GL_TEXTURE_2D, tree_tex)
                glCallList(dl)
            glPopMatrix()

        # Rigged Character (animation already applied by anim.update above)
        if third_p:
            glPushMatrix()
            glTranslatef(px, py, pz)
            draw_rigged_character(character, char_textures, head_dl, hair_dl, axe_dl, axe_tex, yaw)
            glPopMatrix()

        # 1st-person axe
        if not third_p:
            bob_x = math.cos(walk_phase * 3.0) * 0.015 if is_moving else 0.0
            bob_y = math.sin(walk_phase * 6.0) * 0.015 if is_moving else 0.0
            draw_weapon_fp(axe_dl, axe_tex, bob_x, bob_y, attack_t, is_attacking, W, H)

        # HUD
        hud.draw(W, H, int(clock.get_fps()), health, stamina, wood, metal, stone, sulfur,
                 px, pz, yaw, third_p, msg, msg_t, dt, food, water)

        pygame.display.flip()

    pygame.quit()

if __name__ == '__main__':
    main()

