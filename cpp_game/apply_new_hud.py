"""
Enhanced authentic BattlePanel HUD with crisp text, clean POT textures, and OpenGL 1.1 compatibility.
"""
import os

HUD_CODE = '''pygame.font.init()
FONT_TITLE = pygame.font.SysFont('segoeui', 16, bold=True)
FONT_TEXT  = pygame.font.SysFont('segoeui', 13, bold=True)
FONT_NUM   = pygame.font.SysFont('segoeui', 11, bold=True)
FONT_SM    = pygame.font.SysFont('segoeui', 11)

class FastHUD:
    def __init__(self, asset_dir):
        u = lambda f: os.path.join(asset_dir, 'ui_hud', f)
        self.tex_cross   = load_texture(u('BattlePanel_cxfmt_zhunxing.png'))
        self.tex_map     = load_texture(u('1BattlePanel_map_di_2_1.png'))
        self.tex_map_djs = load_texture(u('BattlePanel2_map_djs.png'))
        self.tex_needle  = load_texture(u('1BattlePanel_sy_zaiju_jt.png'))
        self.tex_compass = load_texture(u('battlepanel_kedu_yw.png'))

        # 100% Authentic Original BattlePanel HUD Sprites (from assets/uiatlas/battlepanel.ab):
        self.tex_hp      = load_texture(u('1BattlePanel_buff_xue.png'))
        self.tex_food    = load_texture(u('1BattlePanel_buff_jie.png'))
        self.tex_drop    = load_texture(u('1BattlePanel_buff_shui.png'))
        self.tex_stam    = load_texture(u('new_common_tili.png'))
        self.tex_plus    = load_texture(u('1BattlePanel_buff_xue_jia.png'))

        # Authentic BattlePanel Hotbar Sprites
        self.tex_slot    = load_texture(u('1BattlePanel_buttle_fang_1.png'))
        self.tex_sel     = load_texture(u('com_chose_kuang.png'))
        self.tex_bag     = load_texture(u('1BattlePanell_bb.png'))

        # Authentic Original Item Icons
        self.tex_axe     = load_texture(u('wq_shifu.png'))
        self.tex_wood    = load_texture(u('tb_cl_mt.png'))
        self.tex_metal   = load_texture(u('tb_cl_tiek.png'))
        self.tex_stone   = load_texture(u('ks_st.png'))
        self.tex_sulfur  = load_texture(u('ks_lhk.png'))

        # 100% Authentic Status Bar Textures (1BattlePanel_xue_bg_1 & 1BattlePanel_di_cb)
        sliced_path = u('1BattlePanel_xue_bg_1_sliced.png')
        if not os.path.exists(sliced_path) or os.path.getsize(sliced_path) == 0:
            try:
                from PIL import Image
                src_bg = Image.open(u('1BattlePanel_xue_bg_1.png')).convert('RGBA')
                b_l, b_r, b_t, b_b = 10, 10, 10, 10
                sw, sh = src_bg.size
                tw, th = 150, 24
                top_left = src_bg.crop((0, 0, b_l, b_t))
                top_right = src_bg.crop((sw - b_r, 0, sw, b_t))
                bot_left = src_bg.crop((0, sh - b_b, b_l, sh))
                bot_right = src_bg.crop((sw - b_r, sh - b_b, sw, sh))
                top_edge = src_bg.crop((b_l, 0, sw - b_r, b_t)).resize((tw - b_l - b_r, b_t), Image.BILINEAR)
                bot_edge = src_bg.crop((b_l, sh - b_b, sw - b_r, sh)).resize((tw - b_l - b_r, b_b), Image.BILINEAR)
                left_edge = src_bg.crop((0, b_t, b_l, sh - b_b)).resize((b_l, th - b_t - b_b), Image.BILINEAR)
                right_edge = src_bg.crop((sw - b_r, b_t, sw, sh - b_b)).resize((b_r, th - b_t - b_b), Image.BILINEAR)
                center = src_bg.crop((b_l, b_t, sw - b_r, sh - b_b)).resize((tw - b_l - b_r, th - b_t - b_b), Image.BILINEAR)
                res = Image.new('RGBA', (tw, th), (0, 0, 0, 0))
                res.paste(top_left, (0, 0)); res.paste(top_edge, (b_l, 0)); res.paste(top_right, (tw - b_r, 0))
                res.paste(left_edge, (0, b_t)); res.paste(center, (b_l, b_t)); res.paste(right_edge, (tw - b_r, b_t))
                res.paste(bot_left, (0, th - b_b)); res.paste(bot_edge, (b_l, th - b_b)); res.paste(bot_right, (tw - b_r, th - b_b))
                res.save(sliced_path)
            except Exception as e:
                print("Could not slice 9-slice bg:", e)
        self.tex_bar_bg   = load_texture(sliced_path)
        self.tex_bar_fill = load_texture(u('1BattlePanel_di_cb.png'))

        self.tex_info     = glGenTextures(1)
        self.tex_hotbar   = glGenTextures(1)
        self.tex_msg      = glGenTextures(1)
        self.tex_num_hp   = glGenTextures(1)
        self.tex_num_food = glGenTextures(1)
        self.tex_num_wat  = glGenTextures(1)
        self.tex_num_stam = glGenTextures(1)
        self.tex_map_pos  = glGenTextures(1)

        self.last_wood = -1
        self.last_metal = -1
        self.last_stone = -1
        self.last_sulfur = -1
        self.last_hp = -1
        self.last_stam = -1
        self.last_food = -1
        self.last_water = -1
        self.last_px = None
        self.last_pz = None
        self.info_timer = 999.0
        self.last_3p = None
        self.cached_fps = 60
        self.cached_px = 0.0
        self.cached_pz = 0.0
        self.last_msg = None

    def _render_text_to_tex(self, tid, surf, pot_w, pot_h):
        glBindTexture(GL_TEXTURE_2D, tid)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
        data = pygame.image.tostring(surf, 'RGBA', True)
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, pot_w, pot_h, 0, GL_RGBA, GL_UNSIGNED_BYTE, data)

    def draw(self, W, H, fps, health, stamina, wood, metal, stone, sulfur, px, pz, yaw, is_3p, msg, msg_t, dt=0.016, food=100.0, water=100.0):
        glDisable(GL_DEPTH_TEST)
        glDisable(GL_LIGHTING)

        glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
        glOrtho(0, W, 0, H, -1, 1)
        glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()

        glEnable(GL_BLEND)
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA)

        SCALE = H / 750.0

        # 1. Authentic Diamond Crosshair (Center)
        if self.tex_cross:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_cross)
            ccx, ccy = W // 2, H // 2
            cs = int(14 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(ccx - cs, ccy - cs)
            glTexCoord2f(1, 0); glVertex2f(ccx + cs, ccy - cs)
            glTexCoord2f(1, 1); glVertex2f(ccx + cs, ccy + cs)
            glTexCoord2f(0, 1); glVertex2f(ccx - cs, ccy + cs)
            glEnd()

        # 2. Authentic Top Compass Bar (m_kedu from BattlePanel.ab)
        if self.tex_compass:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_compass)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT)

            comp_w = int(480 * SCALE)
            comp_h = max(18, int(30 * SCALE))
            comp_x = W // 2 - comp_w // 2
            comp_y = H - comp_h - 4

            # Map yaw (radians) to normalized texture coordinates [0, 1]
            u_center = (-yaw / (2.0 * math.pi)) % 1.0
            u_span = 0.20  # ~72 degrees FOV display
            u0 = u_center - u_span / 2.0
            u1 = u_center + u_span / 2.0

            glBegin(GL_QUADS)
            glTexCoord2f(u0, 0.0); glVertex2f(comp_x, comp_y)
            glTexCoord2f(u1, 0.0); glVertex2f(comp_x + comp_w, comp_y)
            glTexCoord2f(u1, 1.0); glVertex2f(comp_x + comp_w, comp_y + comp_h)
            glTexCoord2f(u0, 1.0); glVertex2f(comp_x, comp_y + comp_h)
            glEnd()

            # Authentic yellow compass pointer needle (1BattlePanel_sy_zaiju_jt.png)
            if self.tex_needle:
                glBindTexture(GL_TEXTURE_2D, self.tex_needle)
                glColor4f(1.0, 1.0, 1.0, 1.0)
                nw, nh = int(14 * SCALE), int(12 * SCALE)
                nx = comp_x + comp_w // 2 - nw // 2
                ny = comp_y - nh + 2
                glBegin(GL_QUADS)
                glTexCoord2f(0, 1); glVertex2f(nx, ny)
                glTexCoord2f(1, 1); glVertex2f(nx + nw, ny)
                glTexCoord2f(1, 0); glVertex2f(nx + nw, ny + nh)
                glTexCoord2f(0, 0); glVertex2f(nx, ny + nh)
                glEnd()

        # 3. Authentic Circular Minimap (Top-Right, m_map from BattlePanel.ab)
        mw = mh = int(115 * SCALE)
        mx = W - mw - int(12 * SCALE)
        my = H - mh - int(12 * SCALE)
        if self.tex_map:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_map)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(mx, my)
            glTexCoord2f(1, 0); glVertex2f(mx + mw, my)
            glTexCoord2f(1, 1); glVertex2f(mx + mw, my + mh)
            glTexCoord2f(0, 1); glVertex2f(mx, my + mh)
            glEnd()

            # Player center marker (green dot) + heading direction
            mcx, mcy = mx + mw // 2, my + mh // 2
            glDisable(GL_TEXTURE_2D)
            glColor4f(0.2, 0.9, 0.3, 1.0)
            glPointSize(max(4.0, 6.0 * SCALE))
            glBegin(GL_POINTS); glVertex2f(mcx, mcy); glEnd()
            nx_p = mcx + math.sin(-yaw) * (mw * 0.38)
            ny_p = mcy + math.cos(-yaw) * (mh * 0.38)
            glColor4f(0.95, 0.25, 0.25, 1.0)
            glPointSize(max(5.0, 7.0 * SCALE))
            glBegin(GL_POINTS); glVertex2f(nx_p, ny_p); glEnd()

            # Minimap coordinates bar (BattlePanel2_map_djs.png)
            djs_w = mw
            djs_h = max(12, int(18 * SCALE))
            djs_x = mx
            djs_y = my - djs_h - 2
            if self.tex_map_djs:
                glEnable(GL_TEXTURE_2D)
                glColor4f(1.0, 1.0, 1.0, 0.92)
                glBindTexture(GL_TEXTURE_2D, self.tex_map_djs)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(djs_x, djs_y)
                glTexCoord2f(1, 0); glVertex2f(djs_x + djs_w, djs_y)
                glTexCoord2f(1, 1); glVertex2f(djs_x + djs_w, djs_y + djs_h)
                glTexCoord2f(0, 1); glVertex2f(djs_x, djs_y + djs_h)
                glEnd()

            # Coordinates text inside djs bar
            ipx = int(px); ipz = int(pz)
            if ipx != self.last_px or ipz != self.last_pz:
                self.last_px = ipx; self.last_pz = ipz
                surf_pos = pygame.Surface((128, 32), pygame.SRCALPHA)
                surf_pos.fill((0, 0, 0, 0))
                p_text = FONT_NUM.render(f"X:{ipx}  Z:{ipz}", True, (240, 240, 240))
                surf_pos.blit(p_text, (64 - p_text.get_width() // 2, 2))
                self._render_text_to_tex(self.tex_map_pos, surf_pos, 128, 32)

            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 1.0)
            glBindTexture(GL_TEXTURE_2D, self.tex_map_pos)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(djs_x, djs_y)
            glTexCoord2f(1, 0); glVertex2f(djs_x + djs_w, djs_y)
            glTexCoord2f(1, 1); glVertex2f(djs_x + djs_w, djs_y + djs_h)
            glTexCoord2f(0, 1); glVertex2f(djs_x, djs_y + djs_h)
            glEnd()

        # 4. Authentic Vital Status Bars (xue_je_fs cluster from BattlePanel.ab)
        # Positioned to the left of the minimap
        bar_w   = int(140 * SCALE)
        bar_h   = max(15, int(21 * SCALE))
        icon_w  = int(23 * SCALE)
        icon_h  = bar_h
        spacing = bar_h + int(5 * SCALE)

        cluster_x = mx - bar_w - icon_w - int(14 * SCALE)
        cluster_y = H - int(20 * SCALE) - bar_h

        # Update per-bar numbers when values change
        ihp = int(health); ifood = int(food); iwater = int(water); istam = int(stamina)
        if ihp != self.last_hp:
            self.last_hp = ihp
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{ihp}", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 2))
            self._render_text_to_tex(self.tex_num_hp, s, 64, 32)

        if ifood != self.last_food:
            self.last_food = ifood
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{ifood}", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 2))
            self._render_text_to_tex(self.tex_num_food, s, 64, 32)

        if iwater != self.last_water:
            self.last_water = iwater
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{iwater}", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 2))
            self._render_text_to_tex(self.tex_num_wat, s, 64, 32)

        if istam != self.last_stam:
            self.last_stam = istam
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{istam}", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 2))
            self._render_text_to_tex(self.tex_num_stam, s, 64, 32)

        # Health fill color transitions
        hp_color = (0.21, 0.48, 0.31, 0.95) if health >= 40.0 else ((1.0, 0.59, 0.19, 0.95) if health >= 20.0 else (0.89, 0.20, 0.20, 0.95))

        vitals = [
            (self.tex_hp,   health,  100.0, hp_color, self.tex_num_hp, True),
            (self.tex_food, food,    100.0, (1.00, 0.59, 0.19, 0.95), self.tex_num_food, False),
            (self.tex_drop, water,   100.0, (0.18, 0.62, 0.92, 0.95) if water >= 20.0 else (0.89, 0.20, 0.20, 0.95), self.tex_num_wat, False),
            (self.tex_stam, stamina, 100.0, (0.96, 0.75, 0.14, 0.95), self.tex_num_stam, False),
        ]

        glEnable(GL_TEXTURE_2D)
        for i, (icon_tex, cur_val, max_val, col, num_tex, has_plus) in enumerate(vitals):
            by = cluster_y - i * spacing
            fill_pct = max(0.0, min(1.0, cur_val / max_val))

            # 1. Left badge icon (1BattlePanel_buff_*)
            if icon_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, icon_tex)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(cluster_x, by)
                glTexCoord2f(1, 0); glVertex2f(cluster_x + icon_w, by)
                glTexCoord2f(1, 1); glVertex2f(cluster_x + icon_w, by + icon_h)
                glTexCoord2f(0, 1); glVertex2f(cluster_x, by + icon_h)
                glEnd()

            bar_x0 = cluster_x + icon_w + 3

            # 2. 9-sliced bar frame (1BattlePanel_xue_bg_1)
            if self.tex_bar_bg:
                glColor4f(1.0, 1.0, 1.0, 0.95)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_bg)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x0, by)
                glTexCoord2f(1, 0); glVertex2f(bar_x0 + bar_w, by)
                glTexCoord2f(1, 1); glVertex2f(bar_x0 + bar_w, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(bar_x0, by + bar_h)
                glEnd()

            # 3. Colored bar fill (1BattlePanel_di_cb)
            if self.tex_bar_fill and fill_pct > 0:
                pad_x = max(2, int(3 * SCALE))
                pad_y = max(2, int(2 * SCALE))
                fw = int((bar_w - pad_x * 2 - (int(14 * SCALE) if has_plus else 0)) * fill_pct)
                glColor4f(*col)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_fill)
                fx0 = bar_x0 + pad_x
                fy0 = by + pad_y
                fy1 = by + bar_h - pad_y
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(fx0, fy0)
                glTexCoord2f(1, 0); glVertex2f(fx0 + fw, fy0)
                glTexCoord2f(1, 1); glVertex2f(fx0 + fw, fy1)
                glTexCoord2f(0, 1); glVertex2f(fx0, fy1)
                glEnd()

            # 4. Plus button for HP bar (1BattlePanel_buff_xue_jia)
            if has_plus and self.tex_plus:
                glColor4f(1.0, 1.0, 1.0, 0.95)
                glBindTexture(GL_TEXTURE_2D, self.tex_plus)
                pw = ph = max(10, int(13 * SCALE))
                px_pos = bar_x0 + bar_w - pw - 4
                py_pos = by + (bar_h - ph) // 2
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(px_pos, py_pos)
                glTexCoord2f(1, 0); glVertex2f(px_pos + pw, py_pos)
                glTexCoord2f(1, 1); glVertex2f(px_pos + pw, py_pos + ph)
                glTexCoord2f(0, 1); glVertex2f(px_pos, py_pos + ph)
                glEnd()

            # 5. Clean value text centered in the bar
            if num_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, num_tex)
                tw_b = int(48 * SCALE)
                tx0 = bar_x0 + (bar_w - (int(14 * SCALE) if has_plus else 0)) // 2 - tw_b // 2
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(tx0, by)
                glTexCoord2f(1, 0); glVertex2f(tx0 + tw_b, by)
                glTexCoord2f(1, 1); glVertex2f(tx0 + tw_b, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(tx0, by + bar_h)
                glEnd()

        # 5. Authentic Hotbar Quick-Slots (Bottom-Center, wupin from BattlePanel.ab)
        num_slots = 6
        slot_size = max(42, int(54 * SCALE))
        gap = max(4, int(6 * SCALE))
        total_hotbar_w = num_slots * slot_size + (num_slots - 1) * gap
        hx = W // 2 - total_hotbar_w // 2
        hy = int(14 * SCALE)

        # Authentic backpack icon on left (1BattlePanell_bb.png)
        if self.tex_bag:
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_bag)
            bw = bh = int(slot_size * 0.9)
            bx = hx - bw - int(8 * SCALE)
            by_pos = hy + (slot_size - bh) // 2
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx, by_pos)
            glTexCoord2f(1, 0); glVertex2f(bx + bw, by_pos)
            glTexCoord2f(1, 1); glVertex2f(bx + bw, by_pos + bh)
            glTexCoord2f(0, 1); glVertex2f(bx, by_pos + bh)
            glEnd()

        item_icons = [
            self.tex_axe,
            self.tex_wood if wood > 0 else 0,
            self.tex_metal if metal > 0 else 0,
            self.tex_stone if stone > 0 else 0,
            self.tex_sulfur if sulfur > 0 else 0,
            0
        ]

        for idx in range(num_slots):
            sx = hx + idx * (slot_size + gap)

            # Slot square frame (1BattlePanel_buttle_fang_1.png)
            if self.tex_slot:
                glColor4f(1.0, 1.0, 1.0, 0.95)
                glBindTexture(GL_TEXTURE_2D, self.tex_slot)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx, hy)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_size, hy)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_size, hy + slot_size)
                glTexCoord2f(0, 1); glVertex2f(sx, hy + slot_size)
                glEnd()

            # Authentic original APK item icon
            if item_icons[idx]:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, item_icons[idx])
                pad = max(4, int(6 * SCALE))
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx + pad, hy + pad)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_size - pad, hy + pad)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_size - pad, hy + slot_size - pad)
                glTexCoord2f(0, 1); glVertex2f(sx + pad, hy + slot_size - pad)
                glEnd()

            # Selected slot border (com_chose_kuang.png)
            if idx == 0 and self.tex_sel:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, self.tex_sel)
                pad_s = 2
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx - pad_s, hy - pad_s)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_size + pad_s, hy - pad_s)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_size + pad_s, hy + slot_size + pad_s)
                glTexCoord2f(0, 1); glVertex2f(sx - pad_s, hy + slot_size + pad_s)
                glEnd()

        # Hotbar text labels (only updated on count change)
        if (wood != self.last_wood or metal != self.last_metal or
            stone != self.last_stone or sulfur != self.last_sulfur):
            self.last_wood = wood; self.last_metal = metal
            self.last_stone = stone; self.last_sulfur = sulfur
            surf_hot = pygame.Surface((512, 64), pygame.SRCALPHA)
            surf_hot.fill((0, 0, 0, 0))
            items = [
                ('1', ''),
                ('2', str(wood) if wood > 0 else ''),
                ('3', str(metal) if metal > 0 else ''),
                ('4', str(stone) if stone > 0 else ''),
                ('5', str(sulfur) if sulfur > 0 else ''),
                ('6', '')
            ]
            for idx, (snum, icnt) in enumerate(items):
                sx = idx * (slot_size + gap)
                s_num = FONT_NUM.render(snum, True, (200, 200, 200))
                surf_hot.blit(s_num, (sx + 4, 38))
                if icnt:
                    s_cnt = FONT_TEXT.render(icnt, True, (255, 230, 80))
                    surf_hot.blit(s_cnt, (sx + slot_size - s_cnt.get_width() - 4, 34))
            self._render_text_to_tex(self.tex_hotbar, surf_hot, 512, 64)

        glEnable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 1.0)
        glBindTexture(GL_TEXTURE_2D, self.tex_hotbar)
        glBegin(GL_QUADS)
        glTexCoord2f(0.0, (64 - slot_size)/64.0); glVertex2f(hx, hy)
        glTexCoord2f(total_hotbar_w / 512.0, (64 - slot_size)/64.0); glVertex2f(hx + total_hotbar_w, hy)
        glTexCoord2f(total_hotbar_w / 512.0, 1.0); glVertex2f(hx + total_hotbar_w, hy + slot_size)
        glTexCoord2f(0.0, 1.0); glVertex2f(hx, hy + slot_size)
        glEnd()

        # 6. Sleek Top-Left Status Indicator (FPS & Controls) - POT texture 512x64
        self.info_timer += dt
        if self.info_timer >= 0.4 or is_3p != self.last_3p:
            self.info_timer = 0.0
            self.last_3p = is_3p
            self.cached_fps = fps
            self.cached_px = px
            self.cached_pz = pz

            surf = pygame.Surface((512, 64), pygame.SRCALPHA)
            surf.fill((0, 0, 0, 0))
            pygame.draw.rect(surf, (0, 0, 0, 130), (0, 0, 270, 42), border_radius=5)
            t1 = FONT_TEXT.render(f"FPS: {self.cached_fps} | Camera: {'3a Pessoa' if is_3p else '1a Pessoa'} [TAB]", True, (255, 230, 90))
            t2 = FONT_SM.render("WASD: Mover | SHIFT: Correr | Click: Coletar", True, (210, 210, 210))
            surf.blit(t1, (8, 4))
            surf.blit(t2, (8, 22))
            self._render_text_to_tex(self.tex_info, surf, 512, 64)

        glEnable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 1.0)
        glBindTexture(GL_TEXTURE_2D, self.tex_info)
        ix, iy = 12, H - 52
        glBegin(GL_QUADS)
        glTexCoord2f(0.0, (64 - 42)/64.0); glVertex2f(ix, iy)
        glTexCoord2f(270.0/512.0, (64 - 42)/64.0); glVertex2f(ix + 270, iy)
        glTexCoord2f(270.0/512.0, 1.0); glVertex2f(ix + 270, iy + 42)
        glTexCoord2f(0.0, 1.0); glVertex2f(ix, iy + 42)
        glEnd()

        # 7. Center Floating Notification (+10 Metal!, +15 Madeira!, etc.) - POT texture 256x64
        if msg_t > 0 and msg:
            if msg != self.last_msg:
                self.last_msg = msg
                surf_msg = pygame.Surface((256, 64), pygame.SRCALPHA)
                surf_msg.fill((0, 0, 0, 0))
                pygame.draw.rect(surf_msg, (0, 0, 0, 180), (0, 0, 220, 45), border_radius=8)
                t_msg = FONT_TITLE.render(msg, True, (255, 240, 80))
                surf_msg.blit(t_msg, (15, 10))
                self._render_text_to_tex(self.tex_msg, surf_msg, 256, 64)

            glBindTexture(GL_TEXTURE_2D, self.tex_msg)
            mx_pos = W // 2 - 110
            my_pos = H // 2 - 80
            alpha = min(1.0, msg_t * 2.0)
            glColor4f(1.0, 1.0, 1.0, alpha)
            glBegin(GL_QUADS)
            glTexCoord2f(0.0, (64 - 45)/64.0); glVertex2f(mx_pos, my_pos)
            glTexCoord2f(220.0/256.0, (64 - 45)/64.0); glVertex2f(mx_pos + 220, my_pos)
            glTexCoord2f(220.0/256.0, 1.0); glVertex2f(mx_pos + 220, my_pos + 45)
            glTexCoord2f(0.0, 1.0); glVertex2f(mx_pos, my_pos + 45)
            glEnd()

        glDisable(GL_BLEND)
        glEnable(GL_DEPTH_TEST)
        glEnable(GL_LIGHTING)
        glMatrixMode(GL_PROJECTION); glPopMatrix()
        glMatrixMode(GL_MODELVIEW); glPopMatrix()
'''

viewer_path = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\viewer3d.py'
with open(viewer_path, 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('\r\n', '\n')

start_idx = content.find('pygame.font.init()')
main_idx  = content.find('def main():')

if start_idx == -1 or main_idx == -1:
    print('Error: Could not find markers')
    exit(1)

sep_idx = content.rfind('# ------------------------------------------------------------------ #', start_idx, main_idx)
prefix = content[:start_idx]
suffix = content[sep_idx:] if sep_idx != -1 else content[main_idx:]

new_content = prefix + HUD_CODE + '\n' + suffix

with open(viewer_path, 'w', encoding='utf-8', newline='\n') as f:
    f.write(new_content)

print('Updated FastHUD with POT textures successfully!')
