"""
Last Day Rules: Survival — True Skeletal Rigged 3D Engine
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
#  Authentic Animation Controller — uses real Unity clip data
# ------------------------------------------------------------------ #
def lerp(a, b, t): return a + (b - a) * t


class AuthenticAnimController:
    """
    Selects the correct authentic animation clip based on game state and equipped weapon.
    Supports seamless transitions between Axe (melee) and M4A1 (rifle aiming/shooting).
    Provides buttery smooth sub-frame playback and 0.15s cross-fading between states.
    """
    # Axe animations (original APK, verified from animationbundle.ab)
    AXE_IDLE     = 'zhan_stand01'
    AXE_RUN_FWD  = 'zhan_run_front01'
    AXE_RUN_FWD2 = 'zhan_run_front02'       # Alternate running cycle (ChatGPT extracted)
    AXE_RUN_BACK = 'zhan_run_back01'
    AXE_RUN_LEFT = 'zhan_run_left01'
    AXE_RUN_RGHT = 'zhan_run_right01'
    AXE_ATTACK   = 'fu_zhan_attack_01'
    AXE_DRAW     = 'dao_zhan_nawuqi01'      # Draw/equip axe
    AXE_HOLSTER  = 'dao_zhan_shouwuqi01'    # Holster/unequip axe (ChatGPT extracted)

    # Jump animations (original APK, 3-phase: up / airborne / land)
    JUMP_UP      = 'kongshou_zhan_jump_up02'      # Jump-up phase
    JUMP_AIR     = 'kongshou_zhan_jump_idle02'    # Airborne phase
    JUMP_LAND    = 'kongshou_zhan_jump_down02'    # Landing phase
    JUMP_BACK_UP  = 'kongshou_zhan_jumpback_up02'
    JUMP_BACK_AIR = 'kongshou_zhan_jumpback_idle02'
    JUMP_BACK_LND = 'kongshou_zhan_jumpback_down02'

    # M4A1 Rifle animations (extracted directly from role/animationbundle.ab)
    GUN_IDLE     = 'wq_m4_1_zhan_miaozhun_stand01'
    GUN_RUN_FWD  = 'wq_m4_1_zhan_miaozhun_run_front01'
    GUN_RUN_BACK = 'zhan_run_back01'
    GUN_RUN_LEFT = 'zhan_run_left01'
    GUN_RUN_RGHT = 'zhan_run_right01'
    GUN_SHOOT    = 'wq_m4_1_zhan_miaozhun_attack01'
    GUN_DRAW     = 'chongfengqiang_hengwo_zhan_right_nawuqi01'    # Draw rifle (ChatGPT extracted)
    GUN_HOLSTER  = 'chongfengqiang_hengwo_zhan_right_shouwuqi01'  # Holster rifle (ChatGPT extracted)'

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
        elif not is_grounded:
            # Authentic 3-phase jump: up -> airborne -> landing
            if jump_t < 0.25:
                target_clip = self.JUMP_UP
            elif jump_t < 0.55:
                target_clip = self.JUMP_AIR
            else:
                target_clip = self.JUMP_LAND
            self.idle_time += dt
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
            if clip in (self.AXE_RUN_FWD, self.AXE_RUN_FWD2, self.AXE_RUN_BACK, self.AXE_RUN_LEFT, self.AXE_RUN_RGHT,
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
        self.attack_time = 0.0


# ------------------------------------------------------------------ #
#  Supreme Skinned Mesh Character Renderer (OpenGL 1.1)
# ------------------------------------------------------------------ #
def draw_rigged_character(character, tex, head_dl, hair_dl, axe_dl, axe_tex,
                          gun_dl=None, gun_tex=None, equipped_slot=0, yaw=0.0):
    glEnable(GL_TEXTURE_2D)
    glColor3f(1.0, 1.0, 1.0)
    glPushMatrix()
    glRotatef(math.degrees(yaw), 0, 1, 0)
    character.render(tex, head_dl, hair_dl, axe_dl, axe_tex,
                     gun_dl=gun_dl, gun_tex=gun_tex, equipped_slot=equipped_slot)
    glPopMatrix()

# ------------------------------------------------------------------ #
#  First-person Axe Viewmodel
# ------------------------------------------------------------------ #
def draw_weapon_fp(axe_dl, axe_tex, gun_dl, gun_tex, equipped_slot,
                   bob_x, bob_y, attack_t, is_attacking, is_aiming, W, H):
    glClear(GL_DEPTH_BUFFER_BIT)
    glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
    fov = 32 if is_aiming else 50
    gluPerspective(fov, W / H, 0.01, 10)
    glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()

    if equipped_slot == 1 and gun_dl:
        # M4A1 First-Person Viewmodel (Verified REA alignment: peep sight dead center on ADS)
        recoil = math.sin(attack_t * math.pi) * 0.035 if is_attacking else 0.0
        recoil_rot = math.sin(attack_t * math.pi) * 8.0 if is_attacking else 0.0

        if is_aiming:
            # ADS: Iron sight dead-center (peep aperture at 0, 0)
            glTranslatef(0.0 + bob_x * 0.15, -0.118 + bob_y * 0.15, -0.32 + recoil)
            glRotatef(-recoil_rot, 1, 0, 0)
            glRotatef(-90, 0, 0, 1)
            glRotatef(-90, 0, 1, 0)
        else:
            # Hip-fire: natural lower-right stance, tilted towards center
            glTranslatef(0.18 + bob_x, -0.16 + bob_y, -0.42 + recoil)
            glRotatef(-recoil_rot, 1, 0, 0)
            glRotatef(-4, 0, 1, 0)
            glRotatef(-90, 0, 0, 1)
            glRotatef(-90, 0, 1, 0)

        glScalef(0.65, 0.65, 0.65)
        glEnable(GL_TEXTURE_2D)
        glBindTexture(GL_TEXTURE_2D, gun_tex)
        glColor3f(1.0, 1.0, 1.0)
        glCallList(gun_dl)
    elif axe_dl:
        # Axe First-Person Viewmodel (Stone Axe ready stance)
        swing = math.sin(attack_t * math.pi) * -65.0 if is_attacking else 0.0
        glTranslatef(0.20 + bob_x, -0.15 + bob_y, -0.42)
        glRotatef(swing, 1, 0, 0)
        glRotatef(115, 1, 0, 0)
        glRotatef(-25, 0, 1, 0)
        glRotatef(20, 0, 0, 1)
        glScalef(0.85, 0.85, 0.85)

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
        glColor3f(0.12, 0.20, 0.10)
        glBegin(GL_QUADS)
        glNormal3f(0, 1, 0)
        s = 400
        glVertex3f(-s, 0, -s); glVertex3f(-s, 0, s)
        glVertex3f( s, 0,  s); glVertex3f( s, 0, -s)
        glEnd()
        glColor3f(0.08, 0.14, 0.07)
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
FONT_TITLE = pygame.font.SysFont('segoeui', 15, bold=True)
FONT_TEXT  = pygame.font.SysFont('segoeui', 12, bold=True)
FONT_NUM   = pygame.font.SysFont('segoeui', 11, bold=True)
FONT_SM    = pygame.font.SysFont('segoeui', 10)
FONT_XS    = pygame.font.SysFont('segoeui', 9)

class FastHUD:
    def __init__(self, asset_dir):
        u = lambda f: os.path.join(asset_dir, 'ui_hud', f)

        # 1. Authentic Crosshair & Compass (from battlepanel.ab)
        self.tex_cross      = load_texture(u('BattlePanel_cxfmt_zhunxing.png'))
        self.tex_aim_sight  = load_texture(u('BattlePlane2_zhunxing.png'))
        self.tex_compass    = load_texture(u('battlepanel_kedu_yw.png'))

        # 2. Authentic Minimap: Island texture (map_map.ab) & Frame (battlepanel.ab)
        self.tex_map_tile   = load_texture(u('island_map_2048.png'))
        self.tex_map_di     = load_texture(u('1BattlePanel_map_di_2_1.png'))
        self.tex_needle     = load_texture(u('1BattlePanel_sy_zaiju_jt.png'))
        self.tex_tq_bg      = load_texture(u('1BattlePanel_tq_bg.png'))
        self.tex_moon       = load_texture(u('1BattlePanel_zhouye_yl.png'))
        self.tex_sun        = load_texture(u('1BattlePanel_zhouye_ty.png'))

        # 3. Authentic Vitals Badges & 9-slice Bars (from battlepanel.ab)
        self.tex_hp_badge   = load_texture(u('1BattlePanel_buff_xue.png'))
        self.tex_food_badge = load_texture(u('1BattlePanel_buff_jie.png'))
        self.tex_drop_badge = load_texture(u('1BattlePanel_buff_shui.png'))
        self.tex_bar_bg     = load_texture(u('1BattlePanel_xue_bg_1.png'))
        self.tex_bar_fill   = load_texture(u('1BattlePanel_di_cb.png'))

        # 4. Authentic Hotbar Slots & Icons (from battlepanel.ab & common.ab)
        self.tex_slot       = load_texture(u('1BattlePanel_buttle_fang_1.png'))
        self.tex_backpack   = load_texture(u('1BattlePanell_bb.png'))
        self.tex_build_btn  = load_texture(u('com_xiulitubiao.png'))

        # 5. Authentic Action Buttons (from battlepanel.ab, common.ab & battlecustompanel.ab)
        self.tex_btn_attack = load_texture(u('BattlePanel_attack_button.png'))
        self.tex_btn_vault  = load_texture(u('BattlePanel_sprint_button.png'))
        self.tex_btn_jump   = load_texture(u('1BattlePanel_cz_t.png'))
        self.tex_btn_crouch = load_texture(u('1BattlePanel_cz_dun.png'))
        self.tex_btn_prone  = load_texture(u('1BattlePanel_cz_pa.png'))
        self.tex_btn_aim    = load_texture(u('BattlePanel_aim_button.png'))

        # 6. Authentic Virtual Joystick (from battlepanel2.ab)
        self.tex_joy_base   = load_texture(u('BattlePanel2_car_yaogan_1.png'))
        self.tex_joy_knob   = load_texture(u('BattlePanel2_car_yaogan_2.png'))

        # 7. Authentic Top-Left Menu & Team (from battlepanel.ab)
        self.tex_top_bg     = load_texture(u('1BattlePanel_zuo_shang_infoBG_1.png'))
        self.tex_top_team   = load_texture(u('1BattlePanel_zuo_shang_info.png'))
        self.tex_top_gl     = load_texture(u('1BattlePanel_zuo_shang_infogl.png'))
        self.tex_top_phb    = load_texture(u('1BattlePanel_zuo_shang_infophb.png'))
        self.tex_top_zz     = load_texture(u('1BattlePanel_zuo_shang_infoZZ.png'))
        self.tex_top_quit   = load_texture(u('1BattlePanel_quit.png'))

        # 8. Authentic Inventory Item Icons (from APK gun.ab, tubiao.ab, wq_shifu)
        self.tex_axe        = load_texture(u('wq_shifu.png'))
        self.tex_m4         = load_texture(u('m4_icon.png'))
        self.tex_juice      = load_texture(u('BattlePanel2_YINGSHUI2.png'))
        self.tex_meat       = load_texture(u('tb_yurou_1.png'))
        self.tex_bandage    = load_texture(u('bengdai.png'))
        self.tex_sulfur     = load_texture(u('ks_lhk.png'))
        self.tex_wood       = load_texture(u('tb_cl_mt.png'))
        self.tex_metal      = load_texture(u('tb_cl_tiek.png'))
        self.tex_stone      = load_texture(u('ks_st.png'))

        # Dynamic Text Textures (POT dimensions)
        self.tex_num_hp   = glGenTextures(1)
        self.tex_num_food = glGenTextures(1)
        self.tex_num_wat  = glGenTextures(1)
        self.tex_map_top  = glGenTextures(1)
        self.tex_map_bot  = glGenTextures(1)
        self.tex_msg      = glGenTextures(1)
        self.tex_target   = glGenTextures(1)
        self.tex_civ      = glGenTextures(1)
        self.tex_chat     = glGenTextures(1)
        self.tex_ammo     = glGenTextures(1)

        self.last_hp = -1
        self.last_ammo = -1
        self.last_food = -1
        self.last_water = -1
        self.last_px = None
        self.last_pz = None
        self.last_msg = None
        self.last_target = None
        self.static_init = False

    def _render_text_to_tex(self, tid, surf, pot_w, pot_h):
        glBindTexture(GL_TEXTURE_2D, tid)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
        data = pygame.image.tostring(surf, 'RGBA', False)
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, pot_w, pot_h, 0, GL_RGBA, GL_UNSIGNED_BYTE, data)

    def _draw_sub_tex(self, tid, x, y, w, h, u_w, v_h, pot_w, pot_h):
        glEnable(GL_TEXTURE_2D)
        glBindTexture(GL_TEXTURE_2D, tid)
        u0, u1 = 0.0, float(u_w) / pot_w
        v_top, v_bot = 0.0, float(v_h) / pot_h
        glBegin(GL_QUADS)
        glTexCoord2f(u0, v_bot); glVertex2f(x, y)
        glTexCoord2f(u1, v_bot); glVertex2f(x + w, y)
        glTexCoord2f(u1, v_top); glVertex2f(x + w, y + h)
        glTexCoord2f(u0, v_top); glVertex2f(x, y + h)
        glEnd()

    def draw(self, W, H, fps, health, stamina, wood, metal, stone, sulfur, px, pz, yaw, is_3p, msg, msg_t, dt=0.016, food=272.0, water=266.0, target_node=None, joy_input=(0.0, 0.0), btn_pressed=None, equipped_slot=0, is_aiming=False, ammo_cur=30, ammo_max=30):
        glDisable(GL_DEPTH_TEST)
        glDisable(GL_LIGHTING)

        glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
        glOrtho(0, W, 0, H, -1, 1)
        glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()

        glEnable(GL_BLEND)
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA)

        SCALE = min(W / 1024.0, H / 576.0)
        if btn_pressed is None:
            btn_pressed = set()
        joy_dx, joy_dy = joy_input[0], joy_input[1]

        # -------------------------------------------------------------
        # 1. TOP-LEFT MENU & TEAM SLOTS (Raw APK battlepanel.ab)
        # -------------------------------------------------------------
        if self.tex_top_bg:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_top_bg)
            tw, th = int(220 * SCALE), int(28 * SCALE)
            tx, ty = int(12 * SCALE), H - th - int(8 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(tx, ty)
            glTexCoord2f(1, 0); glVertex2f(tx + tw, ty)
            glTexCoord2f(1, 1); glVertex2f(tx + tw, ty + th)
            glTexCoord2f(0, 1); glVertex2f(tx, ty + th)
            glEnd()

        if self.tex_top_team:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_top_team)
            tm_w, tm_h = int(125 * SCALE), int(56 * SCALE)
            tm_x, tm_y = int(12 * SCALE), H - int(96 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(tm_x, tm_y)
            glTexCoord2f(1, 0); glVertex2f(tm_x + tm_w, tm_y)
            glTexCoord2f(1, 1); glVertex2f(tm_x + tm_w, tm_y + tm_h)
            glTexCoord2f(0, 1); glVertex2f(tm_x, tm_y + tm_h)
            glEnd()

        # -------------------------------------------------------------
        # 2. TOP COMPASS BAR (Raw APK battlepanel_kedu_yw.png)
        # -------------------------------------------------------------
        comp_w = int(320 * SCALE)
        comp_h = max(16, int(22 * SCALE))
        comp_x = W // 2 - comp_w // 2
        comp_y = H - comp_h - int(4 * SCALE)

        glDisable(GL_TEXTURE_2D)
        glColor4f(0.04, 0.05, 0.08, 0.60)
        glBegin(GL_QUADS)
        glVertex2f(comp_x, comp_y)
        glVertex2f(comp_x + comp_w, comp_y)
        glVertex2f(comp_x + comp_w, comp_y + comp_h)
        glVertex2f(comp_x, comp_y + comp_h)
        glEnd()

        if self.tex_compass:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_compass)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP)

            u_center = (-yaw / (2.0 * math.pi)) % 1.0
            u_span = 0.16
            u0 = u_center - u_span / 2.0
            u1 = u_center + u_span / 2.0

            glBegin(GL_QUADS)
            glTexCoord2f(u0, 0.0); glVertex2f(comp_x, comp_y)
            glTexCoord2f(u1, 0.0); glVertex2f(comp_x + comp_w, comp_y)
            glTexCoord2f(u1, 1.0); glVertex2f(comp_x + comp_w, comp_y + comp_h)
            glTexCoord2f(u0, 1.0); glVertex2f(comp_x, comp_y + comp_h)
            glEnd()

        # White down-arrow indicator at center
        glDisable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 0.95)
        mid_x = comp_x + comp_w // 2
        arr_sz = int(4 * SCALE)
        glBegin(GL_TRIANGLES)
        glVertex2f(mid_x, comp_y + comp_h - int(8 * SCALE))
        glVertex2f(mid_x - arr_sz, comp_y + comp_h - 1)
        glVertex2f(mid_x + arr_sz, comp_y + comp_h - 1)
        glEnd()

        # -------------------------------------------------------------
        # 3. AUTHENTIC MINIMAP RADAR (Reverse-engineered from Unity BattlePanel.prefab)
        # -------------------------------------------------------------
        # Canvas m_map size is 260x260, anchored top-right: pos=(10, -32.4)
        map_w = map_h = int(120 * SCALE)
        map_x = W - map_w - int(12 * SCALE)
        map_y = H - map_h - int(24 * SCALE)

        # Top pill for Game Time and Day/Night Icon (zhou_ye in BattlePanel.prefab)
        # Uses 1BattlePanel_tq_bg with genuine 1BattlePanel_zhouye_yl sprite
        pill_w = int(64 * SCALE)
        pill_h = int(18 * SCALE)
        pill_x = map_x + map_w - pill_w
        pill_y = map_y + map_h + int(2 * SCALE)

        if self.tex_tq_bg:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_tq_bg)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(pill_x, pill_y)
            glTexCoord2f(1, 0); glVertex2f(pill_x + pill_w, pill_y)
            glTexCoord2f(1, 1); glVertex2f(pill_x + pill_w, pill_y + pill_h)
            glTexCoord2f(0, 1); glVertex2f(pill_x, pill_y + pill_h)
            glEnd()

        # Authentic Moon/Sun Sprite from APK (1BattlePanel_zhouye_yl)
        if self.tex_moon:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_moon)
            icon_sz = int(14 * SCALE)
            ix = pill_x + pill_w - icon_sz - int(3 * SCALE)
            iy = pill_y + (pill_h - icon_sz) // 2
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(ix, iy)
            glTexCoord2f(1, 0); glVertex2f(ix + icon_sz, iy)
            glTexCoord2f(1, 1); glVertex2f(ix + icon_sz, iy + icon_sz)
            glTexCoord2f(0, 1); glVertex2f(ix, iy + icon_sz)
            glEnd()

        # txt_gameTime text (authentic format from APK)
        if not hasattr(self, '_time_rendered'):
            self._time_rendered = True
            s_top = pygame.Surface((96, 24), pygame.SRCALPHA); s_top.fill((0, 0, 0, 0))
            t_str = FONT_TEXT.render("21:26", True, (245, 245, 255))
            s_top.blit(t_str, (4, 1))
            self._render_text_to_tex(self.tex_map_top, s_top, 128, 32)

        glColor4f(1.0, 1.0, 1.0, 1.0)
        self._draw_sub_tex(self.tex_map_top, pill_x + int(4 * SCALE), pill_y + 1, int(38 * SCALE), pill_h - 2, 60, 22, 128, 32)

        # Minimap Terrain (using authentic Map_map texture from APK centered on player coords)
        if self.tex_map_tile:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_map_tile)
            
            # Map coordinates offset
            u_c = (0.5 + px / 4000.0) % 1.0
            v_c = (0.5 + pz / 4000.0) % 1.0
            span = 0.075
            glBegin(GL_QUADS)
            glTexCoord2f(u_c - span, v_c - span); glVertex2f(map_x, map_y)
            glTexCoord2f(u_c + span, v_c - span); glVertex2f(map_x + map_w, map_y)
            glTexCoord2f(u_c + span, v_c + span); glVertex2f(map_x + map_w, map_y + map_h)
            glTexCoord2f(u_c - span, v_c + span); glVertex2f(map_x, map_y + map_h)
            glEnd()

        # Minimap Frame Border (1BattlePanel_map_di_2_1.png)
        if self.tex_map_di:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.85)
            glBindTexture(GL_TEXTURE_2D, self.tex_map_di)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(map_x, map_y)
            glTexCoord2f(1, 0); glVertex2f(map_x + map_w, map_y)
            glTexCoord2f(1, 1); glVertex2f(map_x + map_w, map_y + map_h)
            glTexCoord2f(0, 1); glVertex2f(map_x, map_y + map_h)
            glEnd()

        # Player location center marker with genuine 1BattlePanel_sy_zaiju_jt.png orientation needle
        mcx, mcy = map_x + map_w // 2, map_y + map_h // 2
        if self.tex_needle:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 0.9, 0.2, 1.0)
            glBindTexture(GL_TEXTURE_2D, self.tex_needle)
            glPushMatrix()
            glTranslatef(mcx, mcy, 0)
            glRotatef(math.degrees(yaw), 0, 0, 1)
            ns = int(8 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(-ns, -ns)
            glTexCoord2f(1, 0); glVertex2f(ns, -ns)
            glTexCoord2f(1, 1); glVertex2f(ns, ns)
            glTexCoord2f(0, 1); glVertex2f(-ns, ns)
            glEnd()
            glPopMatrix()
        else:
            glDisable(GL_TEXTURE_2D)
            glColor4f(1.0, 0.92, 0.25, 1.0)
            glPointSize(max(4.0, 6.0 * SCALE))
            glBegin(GL_POINTS); glVertex2f(mcx, mcy); glEnd()

        # Authentic txt_coordinate strip at bottom of map
        # Matches BattlePanel.cs UpdateCoordinate(): Utils.GetString(263, x + 3000, z + 3000)
        bot_strip_h = int(16 * SCALE)
        glDisable(GL_TEXTURE_2D)
        glColor4f(0.04, 0.04, 0.06, 0.80)
        glBegin(GL_QUADS)
        glVertex2f(map_x, map_y)
        glVertex2f(map_x + map_w, map_y)
        glVertex2f(map_x + map_w, map_y + bot_strip_h)
        glVertex2f(map_x, map_y + bot_strip_h)
        glEnd()

        ipx, ipz = int(px), int(pz)
        if ipx != self.last_px or ipz != self.last_pz:
            self.last_px = ipx; self.last_pz = ipz
            s_bot = pygame.Surface((128, 32), pygame.SRCALPHA); s_bot.fill((0, 0, 0, 0))
            # Authentic format from BattlePanel.cs: (3000 + x, 3000 + z)
            crd_str = "(" + str(3000 + ipx) + "," + str(3000 + ipz) + ")"
            t_crd = FONT_XS.render(crd_str, True, (230, 230, 230))
            s_bot.blit(t_crd, (64 - t_crd.get_width() // 2, 6))
            self._render_text_to_tex(self.tex_map_bot, s_bot, 128, 32)

        glColor4f(1.0, 1.0, 1.0, 1.0)
        self._draw_sub_tex(self.tex_map_bot, map_x, map_y, map_w, bot_strip_h, 128, 28, 128, 32)

        # 4. THREE VITAL STATUS BARS (Health, Hunger, Hydration)
        # -------------------------------------------------------------
        bar_w   = int(84 * SCALE)
        bar_h   = max(14, int(18 * SCALE))
        badge_w = bar_h
        gap_v   = int(2 * SCALE)

        vit_x = map_x - bar_w - badge_w - int(8 * SCALE)
        vit_y = map_y + map_h - bar_h

        ihp = int(health); ifood = int(food); iwater = int(water)
        if ihp != self.last_hp:
            self.last_hp = ihp
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{ihp}/100", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 6))
            self._render_text_to_tex(self.tex_num_hp, s, 64, 32)

        if ifood != self.last_food:
            self.last_food = ifood
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{ifood}/500", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 6))
            self._render_text_to_tex(self.tex_num_food, s, 64, 32)

        if iwater != self.last_water:
            self.last_water = iwater
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{iwater}/500", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 6))
            self._render_text_to_tex(self.tex_num_wat, s, 64, 32)

        hp_color = (0.24, 0.65, 0.28, 0.95) if health >= 75.0 else ((0.96, 0.49, 0.18, 0.95) if health >= 25.0 else (0.89, 0.20, 0.20, 0.95))
        sand_fill = (0.94, 0.77, 0.51, 0.95)

        vitals = [
            (self.tex_hp_badge,   health, 100.0, hp_color,  self.tex_num_hp),
            (self.tex_food_badge, food,   500.0, sand_fill, self.tex_num_food),
            (self.tex_drop_badge, water,  500.0, sand_fill, self.tex_num_wat),
        ]

        glEnable(GL_TEXTURE_2D)
        for i, (badge_tex, cur_val, max_val, col, num_tex) in enumerate(vitals):
            by = vit_y - i * (bar_h + gap_v)
            fill_pct = max(0.0, min(1.0, cur_val / max_val))

            if i == 2:
                glDisable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.9)
                arr_x = vit_x - int(8 * SCALE)
                glBegin(GL_TRIANGLES)
                glVertex2f(arr_x + int(5 * SCALE), by + bar_h // 2)
                glVertex2f(arr_x, by + bar_h // 2 - int(3 * SCALE))
                glVertex2f(arr_x, by + bar_h // 2 + int(3 * SCALE))
                glEnd()
                glEnable(GL_TEXTURE_2D)

            # Badge square icon (1BattlePanel_buff_*.png)
            if badge_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, badge_tex)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(vit_x, by)
                glTexCoord2f(1, 0); glVertex2f(vit_x + badge_w, by)
                glTexCoord2f(1, 1); glVertex2f(vit_x + badge_w, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(vit_x, by + bar_h)
                glEnd()

            bar_x0 = vit_x + badge_w + 1

            # 9-slice Bar Frame (1BattlePanel_xue_bg_1.png)
            if self.tex_bar_bg:
                glColor4f(1.0, 1.0, 1.0, 0.90)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_bg)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x0, by)
                glTexCoord2f(1, 0); glVertex2f(bar_x0 + bar_w, by)
                glTexCoord2f(1, 1); glVertex2f(bar_x0 + bar_w, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(bar_x0, by + bar_h)
                glEnd()

            # Colored Fill (1BattlePanel_di_cb.png)
            if self.tex_bar_fill and fill_pct > 0:
                fw = int((bar_w - 2) * fill_pct)
                glColor4f(*col)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_fill)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x0 + 1, by + 1)
                glTexCoord2f(1, 0); glVertex2f(bar_x0 + 1 + fw, by + 1)
                glTexCoord2f(1, 1); glVertex2f(bar_x0 + 1 + fw, by + 1 + bar_h - 2)
                glTexCoord2f(0, 1); glVertex2f(bar_x0 + 1, by + 1 + bar_h - 2)
                glEnd()

            # Number text overlay
            if num_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                tw_b = int(48 * SCALE)
                tx0 = bar_x0 + bar_w // 2 - tw_b // 2
                self._draw_sub_tex(num_tex, tx0, by, tw_b, bar_h, 64, 32, 64, 32)

        # -------------------------------------------------------------
        # 5. HOTBAR (Bottom-Center)
        # -------------------------------------------------------------
        slot_sz = max(36, int(42 * SCALE))
        gap = max(3, int(4 * SCALE))
        num_slots = 6
        total_hotbar_w = num_slots * slot_sz + (num_slots - 1) * gap
        hx = W // 2 - total_hotbar_w // 2
        hy = int(18 * SCALE)

        # Backpack slot on far left (1BattlePanell_bb.png)
        bp_sz = slot_sz
        bp_x  = hx - bp_sz - int(8 * SCALE)
        if self.tex_backpack:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_backpack)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bp_x, hy)
            glTexCoord2f(1, 0); glVertex2f(bp_x + bp_sz, hy)
            glTexCoord2f(1, 1); glVertex2f(bp_x + bp_sz, hy + bp_sz)
            glTexCoord2f(0, 1); glVertex2f(bp_x, hy + bp_sz)
            glEnd()

        # Build hammer button on far right (com_xiulitubiao.png)
        build_x = hx + total_hotbar_w + int(8 * SCALE)
        if self.tex_build_btn:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_build_btn)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(build_x, hy)
            glTexCoord2f(1, 0); glVertex2f(build_x + bp_sz, hy)
            glTexCoord2f(1, 1); glVertex2f(build_x + bp_sz, hy + bp_sz)
            glTexCoord2f(0, 1); glVertex2f(build_x, hy + bp_sz)
            glEnd()

        # Pure APK items: 1:Stone Axe, 2:M4A1 Rifle, 3:Juice, 4:Bandage, 5:Sulfur, 6:Wood/Metal
        item_icons = [
            self.tex_axe,
            self.tex_m4,
            self.tex_juice,
            self.tex_bandage,
            self.tex_sulfur if sulfur > 0 else self.tex_metal,
            self.tex_wood if wood > 0 else 0
        ]

        for idx in range(num_slots):
            sx = hx + idx * (slot_sz + gap)

            # Slot box (1BattlePanel_buttle_fang_1.png)
            if self.tex_slot:
                glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.88)
                glBindTexture(GL_TEXTURE_2D, self.tex_slot)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx, hy)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_sz, hy)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_sz, hy + slot_sz)
                glTexCoord2f(0, 1); glVertex2f(sx, hy + slot_sz)
                glEnd()

            # Active Slot indicator (highlights equipped slot): lime-green left border & orange line below
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
                glEnd()

            # Item Icon inside slot
            if item_icons[idx]:
                glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, item_icons[idx])
                pad = max(3, int(4 * SCALE))
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx + pad, hy + pad)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_sz - pad, hy + pad)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_sz - pad, hy + slot_sz - pad)
                glTexCoord2f(0, 1); glVertex2f(sx + pad, hy + slot_sz - pad)
                glEnd()

        # Clean HUD - authentic BattlePanel elements only

        # -------------------------------------------------------------
        # 6. ACTION CONTROLS (Raw APK Circular Sprites)
        # -------------------------------------------------------------
        big_sz = int(68 * SCALE)
        bx_att = W - int(170 * SCALE)
        by_att = int(82 * SCALE)

        # Big Attack button (circular textured plate + bullet/knife)
        if self.tex_btn_attack:
            glEnable(GL_TEXTURE_2D)
            _att_c = 0.55 if 'attack' in btn_pressed else 1.0
            glColor4f(_att_c, _att_c, _att_c, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_attack)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_att, by_att)
            glTexCoord2f(1, 0); glVertex2f(bx_att + big_sz, by_att)
            glTexCoord2f(1, 1); glVertex2f(bx_att + big_sz, by_att + big_sz)
            glTexCoord2f(0, 1); glVertex2f(bx_att, by_att + big_sz)
            glEnd()

        btn_sz = int(48 * SCALE)

        # Sprint / Vault (circular plate + runner)
        if self.tex_btn_vault:
            vx = bx_att + int(10 * SCALE)
            vy = by_att + big_sz + int(8 * SCALE)
            glEnable(GL_TEXTURE_2D)
            _vlt_c = 0.55 if 'sprint' in btn_pressed else 1.0
            glColor4f(_vlt_c, _vlt_c, _vlt_c, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_vault)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(vx, vy)
            glTexCoord2f(1, 0); glVertex2f(vx + btn_sz, vy)
            glTexCoord2f(1, 1); glVertex2f(vx + btn_sz, vy + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(vx, vy + btn_sz)
            glEnd()

        # Jump (1BattlePanel_cz_t.png)
        if self.tex_btn_jump:
            glEnable(GL_TEXTURE_2D)
            _jmp_c = 0.55 if 'jump' in btn_pressed else 1.0
            glColor4f(_jmp_c, _jmp_c, _jmp_c, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_jump)
            jx = W - int(72 * SCALE)
            jy = int(76 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(jx, jy)
            glTexCoord2f(1, 0); glVertex2f(jx + btn_sz, jy)
            glTexCoord2f(1, 1); glVertex2f(jx + btn_sz, jy + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(jx, jy + btn_sz)
            glEnd()

        # Crouch / Aim (1BattlePanel_cz_dun.png)
        if self.tex_btn_crouch:
            glEnable(GL_TEXTURE_2D)
            _cr_c = 0.55 if 'crouch' in btn_pressed else 1.0
            glColor4f(_cr_c, _cr_c, _cr_c, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_crouch)
            cx = W - int(116 * SCALE)
            cy = int(22 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(cx, cy)
            glTexCoord2f(1, 0); glVertex2f(cx + btn_sz, cy)
            glTexCoord2f(1, 1); glVertex2f(cx + btn_sz, cy + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(cx, cy + btn_sz)
            glEnd()

        # Prone (1BattlePanel_cz_pa.png)
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
                               int(80 * SCALE), int(16 * SCALE), 80, 20, 128, 32)

        # -------------------------------------------------------------
        # 7. AUTHENTIC JOYSTICK (BattlePanel2_car_yaogan_1 + 2)
        # -------------------------------------------------------------
        if self.tex_joy_base:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.80)
            glBindTexture(GL_TEXTURE_2D, self.tex_joy_base)
            jw = jh = int(96 * SCALE)
            jx, jy = int(45 * SCALE), int(35 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(jx, jy)
            glTexCoord2f(1, 0); glVertex2f(jx + jw, jy)
            glTexCoord2f(1, 1); glVertex2f(jx + jw, jy + jh)
            glTexCoord2f(0, 1); glVertex2f(jx, jy + jh)
            glEnd()

            if self.tex_joy_knob:
                glBindTexture(GL_TEXTURE_2D, self.tex_joy_knob)
                kw = kh = int(48 * SCALE)
                max_offset = int((jw - kw) // 2 * 0.85)
                kx = jx + (jw - kw) // 2 + int(joy_dx * max_offset)
                ky = jy + (jh - kh) // 2 + int(joy_dy * max_offset)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(kx, ky)
                glTexCoord2f(1, 0); glVertex2f(kx + kw, ky)
                glTexCoord2f(1, 1); glVertex2f(kx + kw, ky + kh)
                glTexCoord2f(0, 1); glVertex2f(kx, ky + kh)
                glEnd()

        # -------------------------------------------------------------
        # 8. TARGET NODE NAME & MINING POPUP
        # -------------------------------------------------------------
        if target_node:
            tname, dur_pct = target_node
            tx_c = W // 2
            ty_c = H // 2 + int(60 * SCALE)

            if tname != self.last_target:
                self.last_target = tname
                surf_t = pygame.Surface((256, 32), pygame.SRCALPHA); surf_t.fill((0, 0, 0, 0))
                t_str = FONT_TEXT.render(tname, True, (255, 255, 255))
                surf_t.blit(t_str, (128 - t_str.get_width() // 2, 2))
                self._render_text_to_tex(self.tex_target, surf_t, 256, 32)

            glColor4f(1.0, 1.0, 1.0, 1.0)
            self._draw_sub_tex(self.tex_target, tx_c - int(80 * SCALE), ty_c, int(160 * SCALE), int(20 * SCALE), 256, 24, 256, 32)

            db_w = int(110 * SCALE)
            db_h = max(3, int(4 * SCALE))
            db_x = tx_c - db_w // 2
            db_y = ty_c - int(6 * SCALE)
            glDisable(GL_TEXTURE_2D)
            glColor4f(0.04, 0.04, 0.04, 0.85)
            glBegin(GL_QUADS)
            glVertex2f(db_x, db_y); glVertex2f(db_x + db_w, db_y)
            glVertex2f(db_x + db_w, db_y + db_h); glVertex2f(db_x, db_y + db_h)
            glEnd()
            glColor4f(0.85, 0.65, 0.42, 1.0)
            fill_w = int(db_w * dur_pct)
            glBegin(GL_QUADS)
            glVertex2f(db_x, db_y); glVertex2f(db_x + fill_w, db_y)
            glVertex2f(db_x + fill_w, db_y + db_h); glVertex2f(db_x, db_y + db_h)
            glEnd()

        # Harvest Notification Popup (Iron Mine +42)
        if msg_t > 0 and msg:
            if msg != self.last_msg:
                self.last_msg = msg
                surf_msg = pygame.Surface((256, 64), pygame.SRCALPHA); surf_msg.fill((0, 0, 0, 0))
                pygame.draw.rect(surf_msg, (12, 12, 12, 220), (0, 0, 200, 36), border_radius=4)
                t_msg = FONT_TEXT.render(msg, True, (255, 255, 255))
                surf_msg.blit(t_msg, (100 - t_msg.get_width() // 2, 7))
                self._render_text_to_tex(self.tex_msg, surf_msg, 256, 64)

            mx_pos = W // 2 - int(85 * SCALE)
            my_pos = H // 2 + int(24 * SCALE)
            alpha = min(1.0, msg_t * 2.0)
            glColor4f(1.0, 1.0, 1.0, alpha)
            self._draw_sub_tex(self.tex_msg, mx_pos, my_pos, int(170 * SCALE), int(30 * SCALE), 200, 36, 256, 64)

        # -------------------------------------------------------------
        # 9. CROSSHAIR (Center)
        # -------------------------------------------------------------
        if is_aiming and equipped_slot == 1:
            # ADS Authentic Aim Sight Overlay (from BattlePlane2_zhunxing.png)
            tex_to_use = self.tex_aim_sight if self.tex_aim_sight else self.tex_cross
            if tex_to_use:
                glEnable(GL_TEXTURE_2D); glColor4f(0.2, 1.0, 0.4, 0.95)
                glBindTexture(GL_TEXTURE_2D, tex_to_use)
                ccx, ccy = W // 2, H // 2
                cs = int(36 * SCALE)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(ccx - cs, ccy - cs)
                glTexCoord2f(1, 0); glVertex2f(ccx + cs, ccy - cs)
                glTexCoord2f(1, 1); glVertex2f(ccx + cs, ccy + cs)
                glTexCoord2f(0, 1); glVertex2f(ccx - cs, ccy + cs)
                glEnd()
        elif self.tex_cross:
            # Hip-fire crosshair (from BattlePanel_cxfmt_zhunxing.png)
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.85)
            glBindTexture(GL_TEXTURE_2D, self.tex_cross)
            ccx, ccy = W // 2, H // 2
            cs = int(14 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(ccx - cs, ccy - cs)
            glTexCoord2f(1, 0); glVertex2f(ccx + cs, ccy - cs)
            glTexCoord2f(1, 1); glVertex2f(ccx + cs, ccy + cs)
            glTexCoord2f(0, 1); glVertex2f(ccx - cs, ccy + cs)
            glEnd()

        glDisable(GL_BLEND)
        glEnable(GL_DEPTH_TEST)
        glEnable(GL_LIGHTING)
        glMatrixMode(GL_PROJECTION); glPopMatrix()
        glMatrixMode(GL_MODELVIEW); glPopMatrix()

def main():
    pygame.init()
    try:
        pygame.mixer.init(frequency=44100, size=-16, channels=2, buffer=512)
    except Exception:
        pass
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
    glLightfv(GL_LIGHT0, GL_AMBIENT,  [0.32, 0.36, 0.45, 1])
    glLightfv(GL_LIGHT0, GL_DIFFUSE,  [0.65, 0.70, 0.82, 1])
    glEnable(GL_COLOR_MATERIAL)
    glColorMaterial(GL_FRONT_AND_BACK, GL_AMBIENT_AND_DIFFUSE)
    glClearColor(0.04, 0.07, 0.14, 1.0)

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
    gun_tex        = load_texture(p('wq_m4_1_ba.png'))
    iron_ore_tex   = load_texture(p('ore_tiekuang_01_d.png'))
    sulfur_ore_tex = load_texture(p('ore_liuhuangkuang_01_d.png'))
    stone_ore_tex  = load_texture(p('ore_yanshikuang_01_d.png'))
    tree_tex       = load_texture(p('tree_baihuashu_03_d.png'))
    try:
        snd_gun_fire  = pygame.mixer.Sound(p('m4a1_fire.wav'))
        snd_axe_swing = pygame.mixer.Sound(p('axe_swing_miss.wav'))
        snd_chop_tree = pygame.mixer.Sound(p('axe_chop_tree.wav'))
        snd_mine_ore  = pygame.mixer.Sound(p('axe_mine_ore.wav'))
    except Exception:
        snd_gun_fire = snd_axe_swing = snd_chop_tree = snd_mine_ore = None

    print('Carregando personagem com animacoes autenticas do APK original...')
    character = SkinnedCharacter()
    head_dl = load_obj(p('man_head_chushijuese_01.obj'), swap_yz=True)
    hair_dl = load_obj(p('man_hair_chushijuese_01.obj'), swap_yz=True)

    axe_dl        = load_obj(p('wq_shifu.obj'), swap_yz=False)
    gun_dl        = load_obj(p('wq_m4_1.obj'), swap_yz=False)
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
    attack_hit_done = False
    equipped_slot = 0    # 0 = Stone Axe, 1 = M4A1 Assault Rifle
    is_aiming = False    # ADS (Aim Down Sights) zoom & centering
    ammo_cur, ammo_max = 30, 30
    health, stamina = 100.0, 100.0
    food, water = 100.0, 100.0
    wood, metal, stone, sulfur = 0, 0, 0, 0
    msg, msg_t = '', 0.0
    jump_t = 0.0

    btn_pressed = set()   # which HUD buttons are visually pressed right now
    joy_dx_main, joy_dy_main = 0.0, 0.0  # joystick knob offset -1..1

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
                    running = False
            elif ev.type == MOUSEBUTTONDOWN and locked:
                if ev.button == 1 and not is_attacking:
                    if equipped_slot == 1:
                        # Gunfire logic (M4A1)
                        if ammo_cur > 0:
                            is_attacking = True; attack_t = 0.0
                            ammo_cur -= 1
                            if snd_gun_fire:
                                snd_gun_fire.play()
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
                        # Authentic Melee Axe attack start
                        is_attacking = True
                        attack_t = 0.0
                        attack_hit_done = False
                        anim.on_attack_start()
                        btn_pressed.add('attack')
                        if snd_axe_swing:
                            snd_axe_swing.play()
                elif ev.button == 3:
                    # Right-click toggle ADS Aim
                    is_aiming = not is_aiming
                    btn_pressed.add('aim') if is_aiming else btn_pressed.discard('aim')
                elif ev.button == 4: cam_dist = max(1.5, cam_dist - 0.4)
                elif ev.button == 5: cam_dist = min(9.0, cam_dist + 0.4)
            elif ev.type == VIDEORESIZE:
                W, H = ev.w, ev.h
                glViewport(0, 0, W, H)
                glMatrixMode(GL_PROJECTION); glLoadIdentity()
                gluPerspective(65, W / H, 0.1, 500)
                glMatrixMode(GL_MODELVIEW)

        if is_attacking:
            att_spd = 5.0 if equipped_slot == 1 else 1.3
            attack_t += dt * att_spd

            # Authentic REA Melee Impact Detection (HandAttack.cs / AttackState.cs)
            # Impact triggers at frame ~14-16 (t=0.45) of swing, not on initial click
            if equipped_slot == 0 and attack_t >= 0.45 and not attack_hit_done:
                attack_hit_done = True
                fx = math.sin(yaw); fz = math.cos(yaw)
                hit = False
                for tx, tz, _ in TREES:
                    dx_t = tx - px; dz_t = tz - pz
                    d_tree = math.hypot(dx_t, dz_t)
                    if d_tree < 2.8:
                        dot = (dx_t * fx + dz_t * fz) / (d_tree + 1e-6)
                        if dot > 0.5:
                            wood += 15; msg = '+15 Madeira!'; msg_t = 2.0
                            if snd_chop_tree: snd_chop_tree.play()
                            hit = True
                            break
                if not hit:
                    for ox, oz, otype, _, _, _, _, omsg in ORES:
                        dx_o = ox - px; dz_o = oz - pz
                        d_ore = math.hypot(dx_o, dz_o)
                        if d_ore < 2.8:
                            dot = (dx_o * fx + dz_o * fz) / (d_ore + 1e-6)
                            if dot > 0.5:
                                if otype == 'metal':   metal += 10
                                elif otype == 'stone':  stone += 10
                                elif otype == 'sulfur': sulfur += 10
                                msg = omsg; msg_t = 2.0
                                if snd_mine_ore: snd_mine_ore.play()
                                break

            if attack_t >= 1.0:
                is_attacking = False
                attack_t = 0.0
                attack_hit_done = False
                btn_pressed.discard('attack')

        if msg_t > 0: msg_t -= dt

        if locked:
            dx, dy = pygame.mouse.get_rel()
            yaw   += dx * 0.003
            pitch  = max(-1.35, min(1.35, pitch - dy * 0.003))

            keys = pygame.key.get_pressed()
            # Forward and Screen-Right direction vectors in world space:
            # Looking down +Z with gluLookAt: Forward is (sin(yaw), cos(yaw))
            # Screen-Right in world is (-cos(yaw), sin(yaw))
            fx = math.sin(yaw); fz = math.cos(yaw)
            s_rx = -math.cos(yaw); s_rz = math.sin(yaw)  # screen right
            
            # Local input:
            input_fwd = 0.0
            input_right = 0.0
            if keys[K_w]: input_fwd += 1.0
            if keys[K_s]: input_fwd -= 1.0
            if keys[K_d]: input_right += 1.0
            if keys[K_a]: input_right -= 1.0

            # World movement vector:
            mx = input_fwd * fx + input_right * s_rx
            mz = input_fwd * fz + input_right * s_rz

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
            input_fwd = 0.0
            input_right = 0.0
            is_moving = False

        # Compute local-space movement direction for directional animation selection.
        if is_moving:
            _local_fwd   = input_fwd
            _local_right = input_right
            # Joystick knob offset directly tracks screen input: Right=+X, Up=+Y
            joy_dx_main = input_right
            joy_dy_main = input_fwd
            jl = math.hypot(joy_dx_main, joy_dy_main)
            if jl > 1.0:
                joy_dx_main /= jl; joy_dy_main /= jl
        else:
            _local_fwd, _local_right = 1.0, 0.0
            joy_dx_main, joy_dy_main = 0.0, 0.0

        # ---- Per-frame btn_pressed from keyboard ----
        if locked:
            _kn = pygame.key.get_pressed()
            if _kn[K_SPACE]:   btn_pressed.add('jump')
            else:              btn_pressed.discard('jump')
            if _kn[K_c]:       btn_pressed.add('crouch')
            else:              btn_pressed.discard('crouch')
            if _kn[K_z]:       btn_pressed.add('prone')
            else:              btn_pressed.discard('prone')
            if is_running:     btn_pressed.add('sprint')
            else:              btn_pressed.discard('sprint')
        else:
            for _b in ('jump', 'crouch', 'prone', 'sprint'):
                btn_pressed.discard(_b)

        # Update authentic animation with real Unity clip data
        anim.update(character, dt, is_moving, is_running, is_grounded,
                    is_attacking, attack_t, jump_t, _local_fwd, _local_right,
                    equipped_slot=equipped_slot, is_aiming=is_aiming)

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

            effective_cam_dist = cam_dist * (0.6 if (is_aiming and equipped_slot == 1) else 1.0)
            cx = tx - lx * effective_cam_dist
            cy = py + (1.55 if is_aiming else 1.65) - ly * effective_cam_dist
            cz = tz - lz * effective_cam_dist
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
            draw_rigged_character(character, char_textures, head_dl, hair_dl, axe_dl, axe_tex,
                                  gun_dl=gun_dl, gun_tex=gun_tex, equipped_slot=equipped_slot, yaw=yaw)
            glPopMatrix()

        # 1st-person weapon (Axe or M4A1)
        if not third_p:
            bob_x = math.cos(walk_phase * 3.0) * 0.015 if is_moving else 0.0
            bob_y = math.sin(walk_phase * 6.0) * 0.015 if is_moving else 0.0
            draw_weapon_fp(axe_dl, axe_tex, gun_dl, gun_tex, equipped_slot,
                           bob_x, bob_y, attack_t, is_attacking, is_aiming, W, H)

        # HUD
        hud.draw(W, H, int(clock.get_fps()), health, stamina, wood, metal, stone, sulfur,
                 px, pz, yaw, third_p, msg, msg_t, dt, food, water,
                 joy_input=(joy_dx_main, joy_dy_main), btn_pressed=btn_pressed,
                 equipped_slot=equipped_slot, is_aiming=is_aiming,
                 ammo_cur=ammo_cur, ammo_max=ammo_max)

        pygame.display.flip()

    pygame.quit()

if __name__ == '__main__':
    main()
