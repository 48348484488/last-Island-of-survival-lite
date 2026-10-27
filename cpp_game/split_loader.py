"""
Test of Split Character + High-Quality Assembly + Fast OpenGL HUD
"""
import math
import os
import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *
from PIL import Image

def _next_pot(n):
    p = 1
    while p < n: p <<= 1
    return p

def load_texture(path):
    if not os.path.exists(path):
        return 0
    im = Image.open(path).convert('RGBA')
    w2 = _next_pot(im.width)
    h2 = _next_pot(im.height)
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

def load_obj(path, swap_yz=True):
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

def load_split_obj(path, split_x=0.02, swap_yz=True):
    """Split symmetrical meshes into right, left, and center display lists."""
    if not os.path.exists(path):
        return None, None, None
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

    faces_r, faces_l, faces_c = [], [], []
    for poly in faces:
        for i in range(1, len(poly) - 1):
            tri = [poly[0], poly[i], poly[i+1]]
            cx = (verts[tri[0][0]][0] + verts[tri[1][0]][0] + verts[tri[2][0]][0]) / 3.0
            if cx > split_x:
                faces_r.append(tri)
            elif cx < -split_x:
                faces_l.append(tri)
            else:
                faces_c.append(tri)

    def make_dl(tri_list):
        if not tri_list: return None
        dl = glGenLists(1)
        glNewList(dl, GL_COMPILE)
        glBegin(GL_TRIANGLES)
        for tri in tri_list:
            for vi, ti, ni in tri:
                if ni >= 0 and ni < len(norms): glNormal3f(*norms[ni])
                if ti >= 0 and ti < len(uvs):   glTexCoord2f(*uvs[ti])
                if vi < len(verts):              glVertex3f(*verts[vi])
        glEnd()
        glEndList()
        return dl

    return make_dl(faces_r), make_dl(faces_l), make_dl(faces_c)

print('Split OBJ loader ready!')
