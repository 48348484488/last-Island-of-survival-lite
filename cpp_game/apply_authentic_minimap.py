import sys

path = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\viewer3d.py'
with open(path, 'r', encoding='utf-8') as f:
    text = f.read()

# 1. In __init__, load 1BattlePanel_zhouye_yl.png (moon) and 1BattlePanel_zhouye_ty.png (sun)
old_load = '''        self.tex_map_tile   = load_texture(u('island_map_2048.png'))
        self.tex_map_di     = load_texture(u('1BattlePanel_map_di_2_1.png'))
        self.tex_needle     = load_texture(u('1BattlePanel_sy_zaiju_jt.png'))
        self.tex_tq_bg      = load_texture(u('1BattlePanel_tq_bg.png'))'''

new_load = '''        self.tex_map_tile   = load_texture(u('island_map_2048.png'))
        self.tex_map_di     = load_texture(u('1BattlePanel_map_di_2_1.png'))
        self.tex_needle     = load_texture(u('1BattlePanel_sy_zaiju_jt.png'))
        self.tex_tq_bg      = load_texture(u('1BattlePanel_tq_bg.png'))
        self.tex_moon       = load_texture(u('1BattlePanel_zhouye_yl.png'))
        self.tex_sun        = load_texture(u('1BattlePanel_zhouye_ty.png'))'''

assert old_load in text, 'old_load not found'
text = text.replace(old_load, new_load)

# 2. Find start and end of minimap block
idx1 = text.find('# 3. AUTHENTIC MINIMAP RADAR')
idx2 = text.find('# 4. THREE VITAL STATUS BARS')
assert idx1 != -1 and idx2 != -1, 'Minimap block delimiters not found'

new_minimap = '''# 3. AUTHENTIC MINIMAP RADAR (Reverse-engineered from Unity BattlePanel.prefab)
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

        '''

text = text[:idx1] + new_minimap + text[idx2:]

# 3. Remove fake chat and fake Civilization Restart text
old_fake = '''        # Text labels (Civilization Restart & Chat)
        if not self.static_init:
            self.static_init = True
            s_civ = pygame.Surface((256, 32), pygame.SRCALPHA); s_civ.fill((0, 0, 0, 0))
            t_civ = FONT_XS.render("Civilization Restart 135h23m54s", True, (220, 140, 60))
            s_civ.blit(t_civ, (0, 0))
            self._render_text_to_tex(self.tex_civ, s_civ, 256, 32)

            s_ch = pygame.Surface((512, 32), pygame.SRCALPHA); s_ch.fill((0, 0, 0, 0))
            t_ch = FONT_XS.render("[Battle] Albedo7:join telegram jebwuax", True, (240, 240, 240))
            s_ch.blit(t_ch, (0, 0))
            self._render_text_to_tex(self.tex_chat, s_ch, 512, 32)

        glColor4f(1.0, 1.0, 1.0, 0.9)
        self._draw_sub_tex(self.tex_civ, W - int(190 * SCALE), int(4 * SCALE), int(180 * SCALE), int(14 * SCALE), 180, 16, 256, 32)

        glColor4f(1.0, 1.0, 1.0, 0.85)
        self._draw_sub_tex(self.tex_chat, hx, hy + slot_sz + int(4 * SCALE), int(220 * SCALE), int(15 * SCALE), 220, 16, 512, 32)'''

new_fake = '''        # Clean HUD - authentic BattlePanel elements only'''

assert old_fake in text, 'old_fake not found'
text = text.replace(old_fake, new_fake)

with open(path, 'w', encoding='utf-8') as f:
    f.write(text)

print('Successfully applied authentic minimap and cleaned fake elements!')
