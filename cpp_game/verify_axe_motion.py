import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *
from PIL import Image
import math
import numpy as np

from viewer3d import load_texture, load_obj, draw_ground
from skinned_character import SkinnedCharacter

def verify_axe_motion():
    pygame.init()
    W, H = 960, 540
    screen = pygame.display.set_mode((W, H), OPENGL | DOUBLEBUF)
    
    glViewport(0, 0, W, H)
    glMatrixMode(GL_PROJECTION); glLoadIdentity()
    gluPerspective(60, W / H, 0.1, 500)
    glMatrixMode(GL_MODELVIEW)

    glEnable(GL_DEPTH_TEST)
    glEnable(GL_LIGHTING); glEnable(GL_LIGHT0)
    glLightfv(GL_LIGHT0, GL_POSITION, [100, 180, 100, 1])
    glLightfv(GL_LIGHT0, GL_AMBIENT,  [0.6, 0.6, 0.6, 1])
    glLightfv(GL_LIGHT0, GL_DIFFUSE,  [0.8, 0.8, 0.8, 1])
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

    character = SkinnedCharacter()

    shots = [
        ('dao_zhan_rush_front01', 0, 'axe_verif_rush_f0.png', 2.8, 0.4, 0.1),
        ('dao_zhan_rush_front01', 8, 'axe_verif_rush_f8.png', 2.8, 0.4, 0.1),
        ('fu_zhan_attack_01', 15, 'axe_verif_attack_f15.png', 2.8, 0.4, 0.1),
        ('zhan_stand01', 0, 'axe_verif_stand_f0.png', 2.8, 0.4, 0.1),
    ]

    target = (0.0, 1.1, 0.0)

    for clip, frame, out_name, cdist, cyaw, cpitch in shots:
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
        glMatrixMode(GL_MODELVIEW); glLoadIdentity()

        cx = target[0] - cdist * math.sin(cyaw) * math.cos(cpitch)
        cy = target[1] + cdist * math.sin(cpitch)
        cz = target[2] - cdist * math.cos(cyaw) * math.cos(cpitch)

        gluLookAt(cx, cy, cz, target[0], target[1], target[2], 0, 1, 0)
        draw_ground()

        glEnable(GL_TEXTURE_2D)
        glColor3f(1.0, 1.0, 1.0)
        character.set_pose_frame(clip, frame)
        character.render(char_textures, head_dl, hair_dl, None, None, equipped_slot=99)

        # Verified authentic socket render: glRotatef(-90, 0, 1, 0)
        M_axe_gl = character.get_bone_world_gl('wuqi_guadian')
        glPushMatrix()
        glMultMatrixf(M_axe_gl.T)
        glRotatef(-90, 0, 1, 0)
        glBindTexture(GL_TEXTURE_2D, axe_tex)
        glCallList(axe_dl)
        glPopMatrix()

        pygame.display.flip()

        pixels = glReadPixels(0, 0, W, H, GL_RGBA, GL_UNSIGNED_BYTE)
        img = Image.frombytes('RGBA', (W, H), pixels)
        img = img.transpose(Image.FLIP_TOP_BOTTOM)
        
        out_path = os.path.join(r'C:\Users\Admin\.gemini\antigravity\brain\abe31cb3-7e86-4ae0-9d8a-5b30d0f9ed06', out_name)
        img.save(out_path)
        print(f"Saved: {out_path}")

    pygame.quit()

if __name__ == '__main__':
    verify_axe_motion()
