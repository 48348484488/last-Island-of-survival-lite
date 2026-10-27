# -*- coding: utf-8 -*-
import re

with open('viewer3d.py', 'r', encoding='utf-8') as f:
    text = f.read()

# Replace minimap section
old_map = '''        # -------------------------------------------------------------
        # 3. SQUARE MINIMAP (Top-Right)
        # -------------------------------------------------------------
        map_w = map_h = int(112 * SCALE)
        map_x = W - map_w - int(14 * SCALE)
        map_y = H - map_h - int(34 * SCALE)

        # Top pill for Time & Moon (21:26 🌙)
        pill_w = int(68 * SCALE)
        pill_h = int(16 * SCALE)
        pill_x = map_x + map_w - pill_w
        pill_y = map_y + map_h + int(4 * SCALE)

        glDisable(GL_TEXTURE_2D)
        glColor4f(0.04, 0.06, 0.10, 0.85)
        glBegin(GL_QUADS)
        glVertex2f(pill_x, pill_y)
        glVertex2f(pill_x + pill_w, pill_y)
        glVertex2f(pill_x + pill_w, pill_y + pill_h)
        glVertex2f(pill_x, pill_y + pill_h)
        glEnd()

        # Render time text if needed
        if not hasattr(self, '_time_rendered'):
            self._time_rendered = True
            s_top = pygame.Surface((128, 32), pygame.SRCALPHA); s_top.fill((0, 0, 0, 0))
            t_str = FONT_TEXT.render("21:26 🌙", True, (245, 245, 255))
            s_top.blit(t_str, (4, 4))
            self._render_text_to_tex(self.tex_map_top, s_top, 128, 32)

        glColor4f(1.0, 1.0, 1.0, 1.0)
        self._draw_sub_tex(self.tex_map_top, pill_x + 2, pill_y + 1, pill_w - 4, pill_h - 2, 70, 22, 128, 32)

        # Minimap Terrain
        if self.tex_map_tile:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_map_tile)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(map_x, map_y)
            glTexCoord2f(1, 0); glVertex2f(map_x + map_w, map_y)
            glTexCoord2f(1, 1); glVertex2f(map_x + map_w, map_y + map_h)
            glTexCoord2f(0, 1); glVertex2f(map_x, map_y + map_h)
            glEnd()

        # Player location cone & dot
        mcx, mcy = map_x + map_w // 2, map_y + map_h // 2
        glDisable(GL_TEXTURE_2D)
        glColor4f(1.0, 0.92, 0.25, 1.0)
        glPointSize(max(4.0, 6.0 * SCALE))
        glBegin(GL_POINTS); glVertex2f(mcx, mcy); glEnd()

        # Orientation needle
        nx_p = mcx + math.sin(-yaw) * (map_w * 0.28)
        ny_p = mcy + math.cos(-yaw) * (map_h * 0.28)
        glColor4f(0.95, 0.25, 0.25, 1.0)
        glPointSize(max(4.0, 5.5 * SCALE))
        glBegin(GL_POINTS); glVertex2f(nx_p, ny_p); glEnd()

        # Bottom info strip of minimap (server 15 / coords)
        bot_strip_h = int(22 * SCALE)
        glDisable(GL_TEXTURE_2D)
        glColor4f(0.04, 0.04, 0.06, 0.82)
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
            t_cnt = FONT_XS.render("15", True, (240, 240, 240))
            t_crd = FONT_XS.render(f"({5293 + ipx},{4385 + ipz})", True, (220, 220, 220))
            s_bot.blit(t_cnt, (64 - t_cnt.get_width() // 2, 0))
            s_bot.blit(t_crd, (64 - t_crd.get_width() // 2, 13))
            self._render_text_to_tex(self.tex_map_bot, s_bot, 128, 32)

        glColor4f(1.0, 1.0, 1.0, 1.0)
        self._draw_sub_tex(self.tex_map_bot, map_x, map_y, map_w, bot_strip_h, 128, 28, 128, 32)'''

new_map = '''        # -------------------------------------------------------------
        # 3. AUTHENTIC RADAR MINIMAP (Top-Right)
        # -------------------------------------------------------------
        map_w = int(114 * SCALE)
        map_h = int(148 * SCALE)
        map_x = W - map_w - int(12 * SCALE)
        map_y = H - map_h - int(10 * SCALE)

        # Top pill for Time & Moon (21:26 🌙)
        pill_w = int(72 * SCALE)
        pill_h = int(16 * SCALE)
        pill_x = map_x + map_w - pill_w
        pill_y = map_y + map_h + int(2 * SCALE)

        glDisable(GL_TEXTURE_2D)
        glColor4f(0.04, 0.06, 0.10, 0.85)
        glBegin(GL_QUADS)
        glVertex2f(pill_x, pill_y)
        glVertex2f(pill_x + pill_w, pill_y)
        glVertex2f(pill_x + pill_w, pill_y + pill_h)
        glVertex2f(pill_x, pill_y + pill_h)
        glEnd()

        if not hasattr(self, '_time_rendered'):
            self._time_rendered = True
            s_top = pygame.Surface((128, 32), pygame.SRCALPHA); s_top.fill((0, 0, 0, 0))
            t_str = FONT_TEXT.render("21:26 🌙", True, (245, 245, 255))
            s_top.blit(t_str, (4, 4))
            self._render_text_to_tex(self.tex_map_top, s_top, 128, 32)

        glColor4f(1.0, 1.0, 1.0, 1.0)
        self._draw_sub_tex(self.tex_map_top, pill_x + 2, pill_y + 1, pill_w - 4, pill_h - 2, 70, 22, 128, 32)

        # Minimap Terrain (authentic 118x154 tile)
        if self.tex_map_tile:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_map_tile)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(map_x, map_y)
            glTexCoord2f(1, 0); glVertex2f(map_x + map_w, map_y)
            glTexCoord2f(1, 1); glVertex2f(map_x + map_w, map_y + map_h)
            glTexCoord2f(0, 1); glVertex2f(map_x, map_y + map_h)
            glEnd()

        # Player location cone & dot (centered in upper terrain area)
        mcx = map_x + map_w // 2
        mcy = map_y + map_h - int(54 * SCALE)
        glDisable(GL_TEXTURE_2D)
        glColor4f(1.0, 0.92, 0.25, 1.0)
        glPointSize(max(4.0, 6.0 * SCALE))
        glBegin(GL_POINTS); glVertex2f(mcx, mcy); glEnd()

        # Orientation cone needle
        nx_p = mcx + math.sin(-yaw) * (map_w * 0.26)
        ny_p = mcy + math.cos(-yaw) * (map_h * 0.26)
        glColor4f(0.95, 0.25, 0.25, 1.0)
        glPointSize(max(4.0, 5.5 * SCALE))
        glBegin(GL_POINTS); glVertex2f(nx_p, ny_p); glEnd()'''

text = text.replace(old_map, new_map)
with open('viewer3d.py', 'w', encoding='utf-8') as f:
    f.write(text)
print('Updated minimap layout perfectly')
