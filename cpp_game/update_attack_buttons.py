# -*- coding: utf-8 -*-
with open('viewer3d.py', 'r', encoding='utf-8') as f:
    text = f.read()

# Replace action button textures definition
old_tex = '''        # 5. Authentic Action Buttons (from battlepanel.ab & battlecustompanel.ab)
        self.tex_btn_att_bg = load_texture(u('BattleCustomPanel_tb_d.png'))
        self.tex_btn_att_fg = load_texture(u('BattlePanel_fire.png'))
        self.tex_btn_jump   = load_texture(u('1BattlePanel_cz_t.png'))
        self.tex_btn_crouch = load_texture(u('1BattlePanel_cz_dun.png'))
        self.tex_btn_prone  = load_texture(u('1BattlePanel_cz_pa.png'))
        self.tex_btn_sprint = load_texture(u('BattlePanel2_car_zidongbengpao2.png'))'''

new_tex = '''        # 5. Authentic Action Buttons (from battlepanel.ab, common.ab & battlecustompanel.ab)
        self.tex_btn_attack = load_texture(u('BattlePanel_attack_button.png'))
        self.tex_btn_vault  = load_texture(u('BattlePanel_sprint_button.png'))
        self.tex_btn_jump   = load_texture(u('1BattlePanel_cz_t.png'))
        self.tex_btn_crouch = load_texture(u('1BattlePanel_cz_dun.png'))
        self.tex_btn_prone  = load_texture(u('1BattlePanel_cz_pa.png'))'''

text = text.replace(old_tex, new_tex)

# Replace action button rendering
old_draw = '''        # Attack button: BattleCustomPanel_tb_d plate + BattlePanel_fire knife
        if self.tex_btn_att_bg:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_att_bg)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_att, by_att)
            glTexCoord2f(1, 0); glVertex2f(bx_att + big_sz, by_att)
            glTexCoord2f(1, 1); glVertex2f(bx_att + big_sz, by_att + big_sz)
            glTexCoord2f(0, 1); glVertex2f(bx_att, by_att + big_sz)
            glEnd()

        if self.tex_btn_att_fg:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 1.0)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_att_fg)
            kpad = int(14 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_att + kpad, by_att + kpad)
            glTexCoord2f(1, 0); glVertex2f(bx_att + big_sz - kpad, by_att + kpad)
            glTexCoord2f(1, 1); glVertex2f(bx_att + big_sz - kpad, by_att + big_sz - kpad)
            glTexCoord2f(0, 1); glVertex2f(bx_att + kpad, by_att + big_sz - kpad)
            glEnd()

        btn_sz = int(48 * SCALE)

        # Sprint (BattlePanel2_car_zidongbengpao2.png) inside plate
        if self.tex_btn_att_bg and self.tex_btn_sprint:
            vx = bx_att + int(10 * SCALE)
            vy = by_att + big_sz + int(8 * SCALE)
            glEnable(GL_TEXTURE_2D); glColor4f(0.8, 0.9, 0.2, 0.85)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_att_bg)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(vx, vy)
            glTexCoord2f(1, 0); glVertex2f(vx + btn_sz, vy)
            glTexCoord2f(1, 1); glVertex2f(vx + btn_sz, vy + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(vx, vy + btn_sz)
            glEnd()

            glColor4f(1.0, 1.0, 1.0, 1.0)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_sprint)
            spad = int(8 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(vx + spad, vy + spad)
            glTexCoord2f(1, 0); glVertex2f(vx + btn_sz - spad, vy + spad)
            glTexCoord2f(1, 1); glVertex2f(vx + btn_sz - spad, vy + btn_sz - spad)
            glTexCoord2f(0, 1); glVertex2f(vx + spad, vy + btn_sz - spad)
            glEnd()'''

new_draw = '''        # Big Attack button (circular textured plate + bullet/knife)
        if self.tex_btn_attack:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
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
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_vault)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(vx, vy)
            glTexCoord2f(1, 0); glVertex2f(vx + btn_sz, vy)
            glTexCoord2f(1, 1); glVertex2f(vx + btn_sz, vy + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(vx, vy + btn_sz)
            glEnd()'''

text = text.replace(old_draw, new_draw)

with open('viewer3d.py', 'w', encoding='utf-8') as f:
    f.write(text)

print('Updated action buttons rendering cleanly')
