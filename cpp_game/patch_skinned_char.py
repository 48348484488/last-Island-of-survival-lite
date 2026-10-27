import sys

p = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\skinned_character.py'
with open(p, 'r', encoding='utf-8') as f:
    text = f.read()

idx = text.find('def render(')
assert idx != -1, 'render method not found'

new_render = '''def render(self, textures: dict, head_dl=None, hair_dl=None,
               axe_dl=None, axe_tex=None,
               gun_dl=None, gun_tex=None, equipped_slot=0):
        """Renders all skinned meshes + rigid head/hair + equipped weapon (axe or gun) via OpenGL 1.1."""
        glEnableClientState(GL_VERTEX_ARRAY)
        glEnableClientState(GL_TEXTURE_COORD_ARRAY)

        tex_map = {
            'pants' : textures.get('pants'),
            'torso' : textures.get('clothes'),
            'skin'  : textures.get('skin'),
            'shoes' : textures.get('shoes'),
            'gloves': textures.get('skin'),
            'head'  : textures.get('head'),
            'hair'  : textures.get('hair'),
        }

        for m_name in ['pants', 'torso', 'skin', 'shoes', 'gloves', 'head', 'hair']:
            m   = self.meshes[m_name]
            tid = tex_map.get(m_name)
            if tid:
                glBindTexture(GL_TEXTURE_2D, tid)
            glVertexPointer(3, GL_FLOAT, 0, m['deformed'])
            glTexCoordPointer(2, GL_FLOAT, 0, m['uvs'])
            glDrawElements(GL_TRIANGLES, len(m['tris']), GL_UNSIGNED_INT, m['tris'])

        glDisableClientState(GL_VERTEX_ARRAY)
        glDisableClientState(GL_TEXTURE_COORD_ARRAY)

        # Render equipped weapon in weapon socket
        if equipped_slot == 0 and axe_dl:
            # Slot 0: Stone Axe
            M_axe_gl = self.get_bone_world_gl('wuqi_guadian')
            glPushMatrix()
            glMultMatrixf(M_axe_gl.T)
            glRotatef(90, 0, 1, 0)    # swing axe upright (blade forward)
            glRotatef(-90, 1, 0, 0)   # tilt handle down from grip
            glTranslatef(0.0, 0.4, 0.0)
            glScalef(1.0, 1.0, 1.0)
            if axe_tex:
                glBindTexture(GL_TEXTURE_2D, axe_tex)
            glCallList(axe_dl)
            glPopMatrix()
        elif equipped_slot == 1 and gun_dl:
            # Slot 1: M4A1 Assault Rifle
            M_gun_gl = self.get_bone_world_gl('wuqi_guadian')
            glPushMatrix()
            glMultMatrixf(M_gun_gl.T)
            # Align M4 barrel pointing forward along weapon socket
            glRotatef(90, 0, 1, 0)
            glRotatef(-90, 1, 0, 0)
            glTranslatef(0.0, 0.15, 0.05)
            glScalef(1.0, 1.0, 1.0)
            if gun_tex:
                glBindTexture(GL_TEXTURE_2D, gun_tex)
            glCallList(gun_dl)
            glPopMatrix()
'''

text = text[:idx] + new_render
with open(p, 'w', encoding='utf-8') as f:
    f.write(text)
print('Updated skinned_character.py render method!')
