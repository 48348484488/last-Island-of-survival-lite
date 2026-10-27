import sys, os
sys.path.insert(0, r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game')
import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *
from PIL import Image
import math
import numpy as np

from viewer3d import load_texture, load_obj, draw_ground, draw_rigged_character
from skinned_character import SkinnedCharacter

def snapshot(clip_name, frame_idx, out_file):
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

    A = os.path.join(r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\assets')
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
    character.set_pose_frame(clip_name, frame_idx)

    # 3rd person behind right shoulder
    px, py, pz = 0.0, 0.0, 0.0
    yaw, pitch, cam_dist = 0.0, 0.10, 2.6
    lx = math.sin(yaw) * math.cos(pitch)
    ly = math.sin(pitch)
    lz = math.cos(yaw) * math.cos(pitch)
    rx = math.cos(yaw)
    rz = -math.sin(yaw)
    shoulder_x = rx * 0.40
    shoulder_z = rz * 0.40
    tx, ty, tz = px + shoulder_x, py + 1.25, pz + shoulder_z
    cx = tx - lx * cam_dist
    cy = py + 1.5 - ly * cam_dist
    cz = tz - lz * cam_dist

    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glMatrixMode(GL_MODELVIEW); glLoadIdentity()
    gluLookAt(cx, cy, cz, tx + lx * 1.5, ty + ly * 1.5, tz + lz * 1.5, 0, 1, 0)
    draw_ground()

    glPushMatrix()
    draw_rigged_character(character, char_textures, head_dl, hair_dl, axe_dl, axe_tex, equipped_slot=0, yaw=yaw)
    glPopMatrix()

    pygame.display.flip()
    pixels = glReadPixels(0, 0, W, H, GL_RGBA, GL_UNSIGNED_BYTE)
    img = Image.frombytes('RGBA', (W, H), pixels).transpose(Image.FLIP_TOP_BOTTOM)
    out_path = os.path.join(r'C:\Users\Admin\.gemini\antigravity\brain\abe31cb3-7e86-4ae0-9d8a-5b30d0f9ed06', out_file)
    img.save(out_path)
    print(f"Saved: {out_path}")
    pygame.quit()

snapshot('zhan_stand01', 0, 'verified_idle_with_axe.png')
snapshot('zhan_run_front01', 10, 'verified_running_with_axe.png')
