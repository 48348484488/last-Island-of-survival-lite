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

def test_stances():
    pygame.init()
    W, H = 960, 540
    screen = pygame.display.set_mode((W, H), OPENGL | DOUBLEBUF)
    
    glViewport(0, 0, W, H)
    glMatrixMode(GL_PROJECTION); glLoadIdentity()
    gluPerspective(50, W / H, 0.1, 500)
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

    # Camera from side-back showing full character in 3rd person
    cam_dist = 2.4
    cam_yaw = 0.4
    cam_pitch = 0.1
    target = (0.0, 1.0, 0.0)

    cx = target[0] - cam_dist * math.sin(cam_yaw) * math.cos(cam_pitch)
    cy = target[1] + cam_dist * math.sin(cam_pitch)
    cz = target[2] - cam_dist * math.cos(cam_yaw) * math.cos(cam_pitch)

    # Test running pose (zhan_run_front01) frame 10
    configs = [
        ('stance_run_rot_y_neg90', 'zhan_run_front01', 10, 0, -90, 0, 0, 0, 0),
        ('stance_run_rot_y_pos90', 'zhan_run_front01', 10, 0,  90, 0, 0, 0, 0),
        ('stance_run_rot_z_neg90', 'zhan_run_front01', 10, 0, 0, -90, 0, 0, 0),
        ('stance_run_rot_z_pos90', 'zhan_run_front01', 10, 0, 0,  90, 0, 0, 0),
        ('stance_stand_rot_y_neg90', 'zhan_stand01', 0, 0, -90, 0, 0, 0, 0),
        ('stance_stand_rot_y_pos90', 'zhan_stand01', 0, 0,  90, 0, 0, 0),
    ]

    for name, clip, frame, rx, ry, rz, tx, ty, tz in configs:
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
        glMatrixMode(GL_MODELVIEW); glLoadIdentity()

        gluLookAt(cx, cy, cz, target[0], target[1], target[2], 0, 1, 0)
        draw_ground()

        glEnable(GL_TEXTURE_2D)
        glColor3f(1.0, 1.0, 1.0)
        character.set_pose_frame(clip, frame)
        character.render(char_textures, head_dl, hair_dl, None, None, equipped_slot=99)

        M_axe_gl = character.get_bone_world_gl('wuqi_guadian')
        glPushMatrix()
        glMultMatrixf(M_axe_gl.T)
        glTranslatef(tx, ty, tz)
        if rx: glRotatef(rx, 1, 0, 0)
        if ry: glRotatef(ry, 0, 1, 0)
        if rz: glRotatef(rz, 0, 0, 1)
        glBindTexture(GL_TEXTURE_2D, axe_tex)
        glCallList(axe_dl)
        glPopMatrix()

        pygame.display.flip()

        pixels = glReadPixels(0, 0, W, H, GL_RGBA, GL_UNSIGNED_BYTE)
        img = Image.frombytes('RGBA', (W, H), pixels)
        img = img.transpose(Image.FLIP_TOP_BOTTOM)
        
        out_path = os.path.join(r'C:\Users\Admin\.gemini\antigravity\brain\abe31cb3-7e86-4ae0-9d8a-5b30d0f9ed06', f'{name}.png')
        img.save(out_path)
        print(f"Saved: {out_path}")

    pygame.quit()

if __name__ == '__main__':
    test_stances()
