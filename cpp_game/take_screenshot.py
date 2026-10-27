"""
Screenshot tool for Last Day Rules: Survival.
Renders the authentic character at multiple animation frames and saves screenshots.
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

from viewer3d import load_texture, load_obj, draw_ground, FastHUD
from skinned_character import SkinnedCharacter


def take_screenshot(clip='kongshou_zhan_run_front01', frame=13, out='screenshot.png'):
    pygame.init()
    W, H = 960, 540
    screen = pygame.display.set_mode((W, H), OPENGL | DOUBLEBUF)
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

    char_textures = {
        'head':    load_texture(p('man_head_chushijuese_01_ba.png')),
        'hair':    load_texture(p('man_hair_chushijuese_01_ba.png')),
        'clothes': load_texture(p('man_clothes_chushijuese_01_ba.png')),
        'skin':    load_texture(p('man_clothes_yf_0_ba.png')),
        'pants':   load_texture(p('man_trousers_chushijuese_01_ba.png')),
        'shoes':   load_texture(p('man_shoes_chushijuese_01_ba.png')),
    }
    axe_tex = load_texture(p('wq_shifu_ba.png'))

    head_dl = load_obj(p('man_head_chushijuese_01.obj'), swap_yz=True)
    hair_dl = load_obj(p('man_hair_chushijuese_01.obj'), swap_yz=True)
    axe_dl  = load_obj(p('wq_shifu.obj'), swap_yz=True)

    house_tex = load_texture(p('Building_baopo_tiefang_ba.png'))
    tree_tex  = load_texture(p('tree_baihuashu_03_d.png'))
    house_dl  = load_obj(p('Building_baopo_tiefang.obj'), swap_yz=False)
    tree_dl   = load_obj(p('tree_baihuashu_03.obj'), swap_yz=False)
    TREES = [(-12, -8, 1.2), (15, 22, 1.1), (18, -18, 1.3)]

    print(f"Loading authentic skinned character...")
    character = SkinnedCharacter()
    print(f"Setting pose: clip='{clip}' frame={frame}")
    character.set_pose_frame(clip, frame)

    px, py, pz = 0.0, 0.0, 0.0
    yaw = 0.0
    pitch = 0.12
    cam_dist = 3.6

    for _ in range(5):  # Warm up
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
        glEnable(GL_TEXTURE_2D); glColor3f(1.0, 1.0, 1.0)
        for tx_pos, tz_pos, sc in TREES:
            glPushMatrix(); glTranslatef(tx_pos, 0, tz_pos); glScalef(sc, sc, sc)
            if tree_dl: glBindTexture(GL_TEXTURE_2D, tree_tex); glCallList(tree_dl)
            glPopMatrix()

        glPushMatrix()
        glTranslatef(px, py, pz)
        glRotatef(math.degrees(yaw), 0, 1, 0)
        character.render(char_textures, head_dl, hair_dl, axe_dl, axe_tex)
        glPopMatrix()
        pygame.display.flip()

    # Save screenshot
    data = glReadPixels(0, 0, W, H, GL_RGB, GL_UNSIGNED_BYTE)
    img = Image.frombytes('RGB', (W, H), data)
    img = img.transpose(Image.FLIP_TOP_BOTTOM)
    out_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), out)
    img.save(out_path)
    print(f"[OK] Screenshot saved: {out_path}")
    pygame.quit()


if __name__ == '__main__':
    import sys
    clip  = sys.argv[1] if len(sys.argv) > 1 else 'kongshou_zhan_run_front01'
    frame = int(sys.argv[2]) if len(sys.argv) > 2 else 13
    out   = sys.argv[3] if len(sys.argv) > 3 else 'screenshot.png'
    take_screenshot(clip, frame, out)
