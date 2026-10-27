"""
Visual verification script for Last Day Rules: Survival.
Runs the complete game loop for 120 frames with continuous smooth animation,
renders the 100% authentic sulfur, iron, and stone ores, and captures screenshots
with the authentic APK HUD.
"""
import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *
from PIL import Image
import math
import numpy as np

from skinned_character import SkinnedCharacter
from viewer3d import load_texture, load_obj, draw_ground, draw_rigged_character, FastHUD, AuthenticAnimController

def test_gameplay_simulation():
    pygame.init()
    W, H = 960, 540
    screen = pygame.display.set_mode((W, H), OPENGL | DOUBLEBUF)
    pygame.display.set_caption('Last Day Rules: Survival - Verification')

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

    character = SkinnedCharacter()
    head_dl = load_obj(p('man_head_chushijuese_01.obj'), swap_yz=True)
    hair_dl = load_obj(p('man_hair_chushijuese_01.obj'), swap_yz=True)

    axe_dl        = load_obj(p('wq_shifu.obj'), swap_yz=False)
    iron_ore_dl   = load_obj(p('ore_tiekuang_01.obj'), swap_yz=False)
    sulfur_ore_dl = load_obj(p('ore_liuhuangkuang_01.obj'), swap_yz=False)
    stone_ore_dl  = load_obj(p('ore_yanshikuang_01.obj'), swap_yz=False)
    tree_dl       = load_obj(p('tree_baihuashu_03.obj'), swap_yz=False)

    TREES = [(-12, -8, 1.2), (15, 22, 1.1), (18, -18, 1.3)]
    ORES = [
        (-4.0,  8.0,  'sulfur', sulfur_ore_dl, sulfur_ore_tex, 1.2, 'Minerio de Enxofre', '+10 Enxofre!'),
        ( 6.0,  7.0,  'metal',  iron_ore_dl,   iron_ore_tex,   1.2, 'Minerio de Ferro',   '+10 Metal!'),
        (-6.0, -10.0, 'stone',  stone_ore_dl,  stone_ore_tex,  1.2, 'Minerio de Pedra',   '+10 Pedra!'),
        (14.0, -8.0,  'metal',  iron_ore_dl,   iron_ore_tex,   1.2, 'Minerio de Ferro',   '+10 Metal!'),
    ]

    anim = AuthenticAnimController()
    hud = FastHUD(A)

    px, py, pz = 0.0, 0.0, 0.0
    yaw, pitch = 0.0, 0.08
    cam_dist = 3.8
    third_p = True
    health, stamina = 50.0, 95.0
    wood, metal, stone, sulfur = 30, 20, 15, 10
    msg, msg_t = 'Iron Mine  +42', 2.0
    target_node = ('Iron Mine', 0.65)

    print("Simulating 60 frames of gameplay...")
    dt = 0.016

    for frame in range(60):
        # Character walking forward towards sulfur ore
        is_moving = frame >= 10
        is_running = frame >= 30
        is_attacking = frame >= 50

        if is_moving:
            pz += dt * (7.0 if is_running else 4.0)

        anim.update(character, dt, is_moving, is_running, True, is_attacking, 0.5, 0.0)

        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
        glLoadIdentity()

        lx = math.sin(yaw) * math.cos(pitch)
        ly = math.sin(pitch)
        lz = math.cos(yaw) * math.cos(pitch)

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

        draw_ground()

        # Draw Ores
        glEnable(GL_TEXTURE_2D); glColor3f(1.0, 1.0, 1.0)
        for ox, oz, _, odl, otex, osc, _, _ in ORES:
            if odl and otex:
                glPushMatrix()
                glTranslatef(ox, 0, oz)
                glScalef(osc, osc, osc)
                glBindTexture(GL_TEXTURE_2D, otex)
                glCallList(odl)
                glPopMatrix()

        # Trees
        for tpos_x, tpos_z, sc in TREES:
            glPushMatrix()
            glTranslatef(tpos_x, 0, tpos_z); glScalef(sc, sc, sc)
            if tree_dl:
                glBindTexture(GL_TEXTURE_2D, tree_tex)
                glCallList(tree_dl)
            glPopMatrix()

        # Rigged Character
        glPushMatrix()
        glTranslatef(px, py, pz)
        draw_rigged_character(character, char_textures, head_dl, hair_dl, axe_dl, axe_tex, yaw)
        glPopMatrix()

        # HUD
        hud.draw(W, H, 60, health, stamina, wood, metal, stone, sulfur,
                 px, pz, yaw, third_p, msg, msg_t, dt, food=272.0, water=266.0, target_node=target_node)

        pygame.display.flip()

    # Capture final screenshot
    data = glReadPixels(0, 0, W, H, GL_RGB, GL_UNSIGNED_BYTE)
    img = Image.frombytes('RGB', (W, H), data)
    img = img.transpose(Image.FLIP_TOP_BOTTOM)
    out_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'screenshot_verified_game.png')
    img.save(out_path)
    print(f"[OK] Verification screenshot saved: {out_path}")
    pygame.quit()

if __name__ == '__main__':
    test_gameplay_simulation()
