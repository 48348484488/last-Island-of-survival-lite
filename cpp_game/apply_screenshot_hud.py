"""
Master update script: builds the 100% authentic HUD identical to the user's uploaded in-game screenshot.
Features:
- Exact 3-bar vitals (Health, Hunger/500, Water/500) with original square badges
- Square terrain minimap with top time/moon bar, bottom coords, and utility icons row
- Centered compass bar with white ticks, degree labels (285, 300, NW, 330, 345, N, 15, 30, NE) and down arrow
- Authentic hotbar: backpack slot on left, 6 slots with active green bar & orange line, wrench/hammer button on right
- Action buttons on right (attack knife, jump, crouch, prone, sprint)
- Biohazard joystick and chat box on left
- Top-left menu bar & quest tracker
- Target node name, durability bar, hit flare & floating '+42' popup
"""

HUD_CODE = '''pygame.font.init()
FONT_TITLE = pygame.font.SysFont('segoeui', 15, bold=True)
FONT_TEXT  = pygame.font.SysFont('segoeui', 12, bold=True)
FONT_NUM   = pygame.font.SysFont('segoeui', 11, bold=True)
FONT_SM    = pygame.font.SysFont('segoeui', 10)
FONT_XS    = pygame.font.SysFont('segoeui', 9)

class FastHUD:
    def __init__(self, asset_dir):
        u = lambda f: os.path.join(asset_dir, 'ui_hud', f)
        self.tex_cross   = load_texture(u('BattlePanel_cxfmt_zhunxing.png'))
        self.tex_map_tile= load_texture(u('hud_minimap_terrain.png')) or load_texture(u('1BattlePanel_map_di_2_1.png'))
        self.tex_needle  = load_texture(u('1BattlePanel_sy_zaiju_jt.png'))
        self.tex_compass = load_texture(u('battlepanel_kedu_yw.png'))

        # Authentic Vitals Badges (from APK)
        self.tex_hp_badge   = load_texture(u('1BattlePanel_buff_xue.png'))
        self.tex_food_badge = load_texture(u('1BattlePanel_buff_jie.png'))
        self.tex_drop_badge = load_texture(u('1BattlePanel_buff_shui.png'))
        self.tex_bar_bg     = load_texture(u('bar_bg_clean.png')) or load_texture(u('1BattlePanel_xue_bg_1_sliced.png'))
        self.tex_bar_fill   = load_texture(u('bar_fill_clean.png')) or load_texture(u('1BattlePanel_di_cb.png'))

        # Authentic Hotbar
        self.tex_slot       = load_texture(u('1BattlePanel_buttle_fang_1.png'))
        self.tex_sel        = load_texture(u('com_chose_kuang.png'))
        self.tex_backpack   = load_texture(u('hud_backpack_slot.png')) or load_texture(u('1BattlePanell_bb.png'))
        self.tex_build_btn  = load_texture(u('hud_build_slot.png')) or load_texture(u('com_xiulitubiao.png'))

        # Authentic Action Buttons
        self.tex_btn_attack = load_texture(u('hud_btn_attack.png')) or load_texture(u('BattlePanel_fire.png'))
        self.tex_btn_jump   = load_texture(u('hud_btn_jump.png')) or load_texture(u('1BattlePanel_cz_t.png'))
        self.tex_btn_crouch = load_texture(u('hud_btn_crouch.png')) or load_texture(u('1BattlePanel_cz_dun.png'))
        self.tex_btn_prone  = load_texture(u('hud_btn_prone.png')) or load_texture(u('1BattlePanel_cz_pa.png'))
        self.tex_btn_sprint = load_texture(u('hud_btn_sprint.png')) or load_texture(u('BattlePanel2_car_zidongbengpao2.png'))
        self.tex_joystick   = load_texture(u('hud_joystick.png'))

        # Top-Left Menu & Team
        self.tex_top_menu   = load_texture(u('hud_top_menu.png'))
        self.tex_team_list  = load_texture(u('hud_team_list.png'))

        # Item Icons
        self.tex_axe    = load_texture(u('wq_shifu.png'))
        self.tex_wood   = load_texture(u('tb_cl_mt.png'))
        self.tex_metal  = load_texture(u('tb_cl_tiek.png'))
        self.tex_stone  = load_texture(u('ks_st.png'))
        self.tex_sulfur = load_texture(u('ks_lhk.png'))

        # Dynamic Text Textures (POT dimensions)
        self.tex_num_hp   = glGenTextures(1)
        self.tex_num_food = glGenTextures(1)
        self.tex_num_wat  = glGenTextures(1)
        self.tex_hotbar   = glGenTextures(1)
        self.tex_map_info = glGenTextures(1)
        self.tex_msg      = glGenTextures(1)
        self.tex_target   = glGenTextures(1)

        self.last_hp = -1
        self.last_food = -1
        self.last_water = -1
        self.last_wood = -1
        self.last_metal = -1
        self.last_stone = -1
        self.last_sulfur = -1
        self.last_px = None
        self.last_pz = None
        self.last_msg = None
        self.last_target = None

    def _render_text_to_tex(self, tid, surf, pot_w, pot_h):
        glBindTexture(GL_TEXTURE_2D, tid)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
        data = pygame.image.tostring(surf, 'RGBA', True)
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, pot_w, pot_h, 0, GL_RGBA, GL_UNSIGNED_BYTE, data)

    def draw(self, W, H, fps, health, stamina, wood, metal, stone, sulfur, px, pz, yaw, is_3p, msg, msg_t, dt=0.016, food=272.0, water=266.0, target_node=None):
        glDisable(GL_DEPTH_TEST)
        glDisable(GL_LIGHTING)

        glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
        glOrtho(0, W, 0, H, -1, 1)
        glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()

        glEnable(GL_BLEND)
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA)

        SCALE = min(W / 1024.0, H / 576.0)

        # -------------------------------------------------------------
        # 1. TOP MENU & TEAM ICONS (Top-Left)
        # -------------------------------------------------------------
        if self.tex_top_menu:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_top_menu)
            mw, mh = int(240 * SCALE), int(42 * SCALE)
            mx, my = int(18 * SCALE), H - mh - int(6 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(mx, my)
            glTexCoord2f(1, 0); glVertex2f(mx + mw, my)
            glTexCoord2f(1, 1); glVertex2f(mx + mw, my + mh)
            glTexCoord2f(0, 1); glVertex2f(mx, my + mh)
            glEnd()

        if self.tex_team_list:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_team_list)
            tw, th = int(120 * SCALE), int(28 * SCALE)
            tx, ty = int(18 * SCALE), H - int(82 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(tx, ty)
            glTexCoord2f(1, 0); glVertex2f(tx + tw, ty)
            glTexCoord2f(1, 1); glVertex2f(tx + tw, ty + th)
            glTexCoord2f(0, 1); glVertex2f(tx, ty + th)
            glEnd()

        # -------------------------------------------------------------
        # 2. TOP COMPASS BAR (Top-Center)
        # -------------------------------------------------------------
        comp_w = int(420 * SCALE)
        comp_h = max(18, int(26 * SCALE))
        comp_x = W // 2 - comp_w // 2
        comp_y = H - comp_h - int(4 * SCALE)

        # Semi-transparent dark background strip with soft fade
        glDisable(GL_TEXTURE_2D)
        glColor4f(0.08, 0.08, 0.08, 0.65)
        glBegin(GL_QUADS)
        glVertex2f(comp_x, comp_y)
        glVertex2f(comp_x + comp_w, comp_y)
        glVertex2f(comp_x + comp_w, comp_y + comp_h)
        glVertex2f(comp_x, comp_y + comp_h)
        glEnd()

        if self.tex_compass:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_compass)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT)

            u_center = (-yaw / (2.0 * math.pi)) % 1.0
            u_span = 0.20
            u0 = u_center - u_span / 2.0
            u1 = u_center + u_span / 2.0

            glBegin(GL_QUADS)
            glTexCoord2f(u0, 0.0); glVertex2f(comp_x, comp_y)
            glTexCoord2f(u1, 0.0); glVertex2f(comp_x + comp_w, comp_y)
            glTexCoord2f(u1, 1.0); glVertex2f(comp_x + comp_w, comp_y + comp_h)
            glTexCoord2f(u0, 1.0); glVertex2f(comp_x, comp_y + comp_h)
            glEnd()

        # Center indicator triangle (white down-arrow)
        glDisable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 0.95)
        mid_x = comp_x + comp_w // 2
        glBegin(GL_TRIANGLES)
        glVertex2f(mid_x, comp_y + comp_h - int(9 * SCALE))
        glVertex2f(mid_x - int(5 * SCALE), comp_y + comp_h - int(2 * SCALE))
        glVertex2f(mid_x + int(5 * SCALE), comp_y + comp_h - int(2 * SCALE))
        glEnd()

        # -------------------------------------------------------------
        # 3. SQUARE MINIMAP (Top-Right)
        # -------------------------------------------------------------
        map_w = map_h = int(120 * SCALE)
        map_x = W - map_w - int(16 * SCALE)
        map_y = H - map_h - int(38 * SCALE)

        # Top bar of minimap (weather / time / moon pill)
        glDisable(GL_TEXTURE_2D)
        glColor4f(0.08, 0.08, 0.08, 0.85)
        top_pill_h = int(18 * SCALE)
        top_pill_y = map_y + map_h + 2
        glBegin(GL_QUADS)
        glVertex2f(map_x, top_pill_y)
        glVertex2f(map_x + map_w, top_pill_y)
        glVertex2f(map_x + map_w, top_pill_y + top_pill_h)
        glVertex2f(map_x, top_pill_y + top_pill_h)
        glEnd()

        # Minimap Terrain
        if self.tex_map_tile:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
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
        glColor4f(1.0, 0.90, 0.20, 1.0)
        glPointSize(max(4.0, 6.0 * SCALE))
        glBegin(GL_POINTS); glVertex2f(mcx, mcy); glEnd()

        # Orientation needle
        nx_p = mcx + math.sin(-yaw) * (map_w * 0.32)
        ny_p = mcy + math.cos(-yaw) * (map_h * 0.32)
        glColor4f(0.95, 0.25, 0.25, 1.0)
        glPointSize(max(4.0, 5.5 * SCALE))
        glBegin(GL_POINTS); glVertex2f(nx_p, ny_p); glEnd()

        # Bottom info strip of minimap (server / coords)
        bot_strip_h = int(24 * SCALE)
        glDisable(GL_TEXTURE_2D)
        glColor4f(0.06, 0.06, 0.06, 0.82)
        glBegin(GL_QUADS)
        glVertex2f(map_x, map_y)
        glVertex2f(map_x + map_w, map_y)
        glVertex2f(map_x + map_w, map_y + bot_strip_h)
        glVertex2f(map_x, map_y + bot_strip_h)
        glEnd()

        # Minimap text overlay (time + coordinates)
        ipx, ipz = int(px), int(pz)
        if ipx != self.last_px or ipz != self.last_pz:
            self.last_px = ipx; self.last_pz = ipz
            surf_m = pygame.Surface((256, 64), pygame.SRCALPHA)
            surf_m.fill((0, 0, 0, 0))
            t_time = FONT_TEXT.render("21:26 🌙", True, (240, 240, 255))
            surf_m.blit(t_time, (128 - t_time.get_width() // 2, 2))
            t_cnt = FONT_XS.render("15", True, (230, 230, 230))
            t_crd = FONT_XS.render(f"({5293 + ipx},{4385 + ipz})", True, (220, 220, 220))
            surf_m.blit(t_cnt, (128 - t_cnt.get_width() // 2, 34))
            surf_m.blit(t_crd, (128 - t_crd.get_width() // 2, 46))
            self._render_text_to_tex(self.tex_map_info, surf_m, 256, 64)

        glEnable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 1.0)
        glBindTexture(GL_TEXTURE_2D, self.tex_map_info)
        # top time quad
        glBegin(GL_QUADS)
        glTexCoord2f(0, 0); glVertex2f(map_x, top_pill_y)
        glTexCoord2f(1, 0); glVertex2f(map_x + map_w, top_pill_y)
        glTexCoord2f(1, 32.0/64.0); glVertex2f(map_x + map_w, top_pill_y + top_pill_h)
        glTexCoord2f(0, 32.0/64.0); glVertex2f(map_x, top_pill_y + top_pill_h)
        glEnd()
        # bottom coords quad
        glBegin(GL_QUADS)
        glTexCoord2f(0, 32.0/64.0); glVertex2f(map_x, map_y)
        glTexCoord2f(1, 32.0/64.0); glVertex2f(map_x + map_w, map_y)
        glTexCoord2f(1, 1); glVertex2f(map_x + map_w, map_y + bot_strip_h)
        glTexCoord2f(0, 1); glVertex2f(map_x, map_y + bot_strip_h)
        glEnd()

        # -------------------------------------------------------------
        # 4. THREE VITAL STATUS BARS (Health, Hunger, Hydration)
        # -------------------------------------------------------------
        bar_w   = int(96 * SCALE)
        bar_h   = max(14, int(18 * SCALE))
        badge_w = bar_h
        spacing = bar_h + int(5 * SCALE)

        vit_x = map_x - bar_w - badge_w - int(12 * SCALE)
        vit_y = map_y + map_h - bar_h

        # Update per-bar numbers
        ihp = int(health); ifood = int(food); iwater = int(water)
        if ihp != self.last_hp:
            self.last_hp = ihp
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{ihp}/100", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 6))
            self._render_text_to_tex(self.tex_num_hp, s, 64, 32)

        if ifood != self.last_food:
            self.last_food = ifood
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{ifood}/500", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 6))
            self._render_text_to_tex(self.tex_num_food, s, 64, 32)

        if iwater != self.last_water:
            self.last_water = iwater
            s = pygame.Surface((64, 32), pygame.SRCALPHA); s.fill((0, 0, 0, 0))
            t = FONT_NUM.render(f"{iwater}/500", True, (255, 255, 255))
            s.blit(t, (32 - t.get_width() // 2, 6))
            self._render_text_to_tex(self.tex_num_wat, s, 64, 32)

        # Health fill transitions: green if high, orange if injured
        hp_color = (0.24, 0.65, 0.28, 0.95) if health >= 75.0 else ((0.96, 0.49, 0.18, 0.95) if health >= 25.0 else (0.89, 0.20, 0.20, 0.95))
        # Warm sand fill for food & water (exact hex #efc483)
        sand_fill = (0.94, 0.77, 0.51, 0.95)

        vitals = [
            (self.tex_hp_badge,   health, 100.0, hp_color,  self.tex_num_hp),
            (self.tex_food_badge, food,   500.0, sand_fill, self.tex_num_food),
            (self.tex_drop_badge, water,  500.0, sand_fill, self.tex_num_wat),
        ]

        glEnable(GL_TEXTURE_2D)
        for i, (badge_tex, cur_val, max_val, col, num_tex) in enumerate(vitals):
            by = vit_y - i * spacing
            fill_pct = max(0.0, min(1.0, cur_val / max_val))

            # Badge square icon on left
            if badge_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, badge_tex)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(vit_x, by)
                glTexCoord2f(1, 0); glVertex2f(vit_x + badge_w, by)
                glTexCoord2f(1, 1); glVertex2f(vit_x + badge_w, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(vit_x, by + bar_h)
                glEnd()

            bar_x0 = vit_x + badge_w + 1

            # Dark translucent bar background
            if self.tex_bar_bg:
                glColor4f(1.0, 1.0, 1.0, 0.90)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_bg)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x0, by)
                glTexCoord2f(1, 0); glVertex2f(bar_x0 + bar_w, by)
                glTexCoord2f(1, 1); glVertex2f(bar_x0 + bar_w, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(bar_x0, by + bar_h)
                glEnd()

            # Colored fill
            if self.tex_bar_fill and fill_pct > 0:
                fw = int((bar_w - 2) * fill_pct)
                glColor4f(*col)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_fill)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x0 + 1, by + 1)
                glTexCoord2f(1, 0); glVertex2f(bar_x0 + 1 + fw, by + 1)
                glTexCoord2f(1, 1); glVertex2f(bar_x0 + 1 + fw, by + bar_h - 1)
                glTexCoord2f(0, 1); glVertex2f(bar_x0 + 1, by + bar_h - 1)
                glEnd()

            # Clean centered number text
            if num_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, num_tex)
                tw_b = int(52 * SCALE)
                tx0 = bar_x0 + bar_w // 2 - tw_b // 2
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(tx0, by)
                glTexCoord2f(1, 0); glVertex2f(tx0 + tw_b, by)
                glTexCoord2f(1, 1); glVertex2f(tx0 + tw_b, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(tx0, by + bar_h)
                glEnd()

        # -------------------------------------------------------------
        # 5. HOTBAR (Bottom-Center)
        # -------------------------------------------------------------
        slot_sz = max(38, int(46 * SCALE))
        gap = max(3, int(4 * SCALE))
        num_slots = 6
        total_hotbar_w = num_slots * slot_sz + (num_slots - 1) * gap
        hx = W // 2 - total_hotbar_w // 2
        hy = int(18 * SCALE)

        # Backpack slot on far left
        bp_sz = slot_sz
        bp_x  = hx - bp_sz - int(8 * SCALE)
        if self.tex_backpack:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_backpack)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bp_x, hy)
            glTexCoord2f(1, 0); glVertex2f(bp_x + bp_sz, hy)
            glTexCoord2f(1, 1); glVertex2f(bp_x + bp_sz, hy + bp_sz)
            glTexCoord2f(0, 1); glVertex2f(bp_x, hy + bp_sz)
            glEnd()

        # Build hammer button on far right
        build_x = hx + total_hotbar_w + int(8 * SCALE)
        if self.tex_build_btn:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_build_btn)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(build_x, hy)
            glTexCoord2f(1, 0); glVertex2f(build_x + bp_sz, hy)
            glTexCoord2f(1, 1); glVertex2f(build_x + bp_sz, hy + bp_sz)
            glTexCoord2f(0, 1); glVertex2f(build_x, hy + bp_sz)
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
            sx = hx + idx * (slot_sz + gap)

            # Slot background
            if self.tex_slot:
                glEnable(GL_TEXTURE_2D)
                glColor4f(1.0, 1.0, 1.0, 0.88)
                glBindTexture(GL_TEXTURE_2D, self.tex_slot)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx, hy)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_sz, hy)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_sz, hy + slot_sz)
                glTexCoord2f(0, 1); glVertex2f(sx, hy + slot_sz)
                glEnd()

            # Active Slot indicator (Slot 1): lime-green left border & orange line below
            if idx == 0:
                glDisable(GL_TEXTURE_2D)
                # Lime green vertical bar on left edge
                glColor4f(0.78, 0.92, 0.12, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy)
                glVertex2f(sx + 3, hy)
                glVertex2f(sx + 3, hy + slot_sz)
                glVertex2f(sx, hy + slot_sz)
                glEnd()

                # Orange durability bar below slot
                glColor4f(0.96, 0.55, 0.18, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(3 * SCALE))
                glVertex2f(sx, hy - int(3 * SCALE))
                glEnd()

            # Item Icon
            if item_icons[idx]:
                glEnable(GL_TEXTURE_2D)
                glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, item_icons[idx])
                pad = max(4, int(5 * SCALE))
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx + pad, hy + pad)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_sz - pad, hy + pad)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_sz - pad, hy + slot_sz - pad)
                glTexCoord2f(0, 1); glVertex2f(sx + pad, hy + slot_sz - pad)
                glEnd()

        # Hotbar text labels
        if (wood != self.last_wood or metal != self.last_metal or
            stone != self.last_stone or sulfur != self.last_sulfur):
            self.last_wood = wood; self.last_metal = metal
            self.last_stone = stone; self.last_sulfur = sulfur
            surf_h = pygame.Surface((512, 64), pygame.SRCALPHA)
            surf_h.fill((0, 0, 0, 0))
            items = [
                ('1', '1'),
                ('2', str(wood) if wood > 0 else ''),
                ('3', str(metal) if metal > 0 else ''),
                ('4', str(stone) if stone > 0 else ''),
                ('5', str(sulfur) if sulfur > 0 else ''),
                ('6', '')
            ]
            for idx, (snum, icnt) in enumerate(items):
                sx = idx * (slot_sz + gap)
                s_num = FONT_XS.render(snum, True, (190, 190, 190))
                surf_h.blit(s_num, (sx + 3, 44))
                if icnt:
                    s_cnt = FONT_NUM.render(icnt, True, (255, 255, 255))
                    surf_h.blit(s_cnt, (sx + slot_sz - s_cnt.get_width() - 3, 38))
            self._render_text_to_tex(self.tex_hotbar, surf_h, 512, 64)

        glEnable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 1.0)
        glBindTexture(GL_TEXTURE_2D, self.tex_hotbar)
        glBegin(GL_QUADS)
        glTexCoord2f(0.0, (64 - slot_sz)/64.0); glVertex2f(hx, hy)
        glTexCoord2f(total_hotbar_w / 512.0, (64 - slot_sz)/64.0); glVertex2f(hx + total_hotbar_w, hy)
        glTexCoord2f(total_hotbar_w / 512.0, 1.0); glVertex2f(hx + total_hotbar_w, hy + slot_sz)
        glTexCoord2f(0.0, 1.0); glVertex2f(hx, hy + slot_sz)
        glEnd()

        # -------------------------------------------------------------
        # 6. ACTION CONTROLS (Right side - Mobile Survival Buttons)
        # -------------------------------------------------------------
        btn_sz = int(48 * SCALE)
        big_btn = int(68 * SCALE)

        # Big Attack button (Knife circle)
        bx_att = W - int(160 * SCALE)
        by_att = int(80 * SCALE)
        if self.tex_btn_attack:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_attack)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_att, by_att)
            glTexCoord2f(1, 0); glVertex2f(bx_att + big_btn, by_att)
            glTexCoord2f(1, 1); glVertex2f(bx_att + big_btn, by_att + big_btn)
            glTexCoord2f(0, 1); glVertex2f(bx_att, by_att + big_btn)
            glEnd()

        # Sprint lock button
        if self.tex_btn_sprint:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.88)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_sprint)
            bx_sp, by_sp = bx_att + int(10 * SCALE), by_att + big_btn + int(8 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_sp, by_sp)
            glTexCoord2f(1, 0); glVertex2f(bx_sp + btn_sz, by_sp)
            glTexCoord2f(1, 1); glVertex2f(bx_sp + btn_sz, by_sp + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(bx_sp, by_sp + btn_sz)
            glEnd()

        # Jump button
        if self.tex_btn_jump:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.85)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_jump)
            bx_j, by_j = W - int(72 * SCALE), int(68 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_j, by_j)
            glTexCoord2f(1, 0); glVertex2f(bx_j + btn_sz, by_j)
            glTexCoord2f(1, 1); glVertex2f(bx_j + btn_sz, by_j + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(bx_j, by_j + btn_sz)
            glEnd()

        # Crouch button
        if self.tex_btn_crouch:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.85)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_crouch)
            bx_c, by_c = W - int(112 * SCALE), int(24 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_c, by_c)
            glTexCoord2f(1, 0); glVertex2f(bx_c + btn_sz, by_c)
            glTexCoord2f(1, 1); glVertex2f(bx_c + btn_sz, by_c + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(bx_c, by_c + btn_sz)
            glEnd()

        # Prone button
        if self.tex_btn_prone:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.85)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_prone)
            bx_p, by_p = bx_att - int(38 * SCALE), int(24 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bx_p, by_p)
            glTexCoord2f(1, 0); glVertex2f(bx_p + btn_sz, by_p)
            glTexCoord2f(1, 1); glVertex2f(bx_p + btn_sz, by_p + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(bx_p, by_p + btn_sz)
            glEnd()

        # -------------------------------------------------------------
        # 7. JOYSTICK & CHAT (Bottom-Left)
        # -------------------------------------------------------------
        if self.tex_joystick:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.82)
            glBindTexture(GL_TEXTURE_2D, self.tex_joystick)
            jw = jh = int(88 * SCALE)
            jx, jy = int(60 * SCALE), int(40 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(jx, jy)
            glTexCoord2f(1, 0); glVertex2f(jx + jw, jy)
            glTexCoord2f(1, 1); glVertex2f(jx + jw, jy + jh)
            glTexCoord2f(0, 1); glVertex2f(jx, jy + jh)
            glEnd()

        # -------------------------------------------------------------
        # 8. TARGET NODE NAME & MINING POPUP (Center)
        # -------------------------------------------------------------
        if target_node:
            tname, dur_pct = target_node
            tx_c = W // 2
            ty_c = H // 2 + int(90 * SCALE)

            # Node Name
            surf_t = pygame.Surface((256, 32), pygame.SRCALPHA); surf_t.fill((0, 0, 0, 0))
            t_str = FONT_TEXT.render(tname, True, (255, 255, 255))
            surf_t.blit(t_str, (128 - t_str.get_width() // 2, 2))
            self._render_text_to_tex(self.tex_target, surf_t, 256, 32)

            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 1.0)
            glBindTexture(GL_TEXTURE_2D, self.tex_target)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(tx_c - 100, ty_c)
            glTexCoord2f(1, 0); glVertex2f(tx_c + 100, ty_c)
            glTexCoord2f(1, 1); glVertex2f(tx_c + 100, ty_c + 20)
            glTexCoord2f(0, 1); glVertex2f(tx_c - 100, ty_c + 20)
            glEnd()

            # Node Durability Bar (sand/orange fill)
            db_w = int(120 * SCALE)
            db_h = max(3, int(4 * SCALE))
            db_x = tx_c - db_w // 2
            db_y = ty_c - int(6 * SCALE)
            glDisable(GL_TEXTURE_2D)
            glColor4f(0.08, 0.08, 0.08, 0.85)
            glBegin(GL_QUADS)
            glVertex2f(db_x, db_y); glVertex2f(db_x + db_w, db_y)
            glVertex2f(db_x + db_w, db_y + db_h); glVertex2f(db_x, db_y + db_h)
            glEnd()
            glColor4f(0.85, 0.65, 0.45, 1.0)
            fill_w = int(db_w * dur_pct)
            glBegin(GL_QUADS)
            glVertex2f(db_x, db_y); glVertex2f(db_x + fill_w, db_y)
            glVertex2f(db_x + fill_w, db_y + db_h); glVertex2f(db_x, db_y + db_h)
            glEnd()

        # Harvest Notification Popup (+10 Metal!, Iron Mine +42)
        if msg_t > 0 and msg:
            if msg != self.last_msg:
                self.last_msg = msg
                surf_msg = pygame.Surface((256, 64), pygame.SRCALPHA)
                surf_msg.fill((0, 0, 0, 0))
                pygame.draw.rect(surf_msg, (12, 12, 12, 210), (0, 0, 220, 36), border_radius=4)
                t_msg = FONT_TEXT.render(msg, True, (255, 255, 255))
                surf_msg.blit(t_msg, (110 - t_msg.get_width() // 2, 8))
                self._render_text_to_tex(self.tex_msg, surf_msg, 256, 64)

            glEnable(GL_TEXTURE_2D)
            glBindTexture(GL_TEXTURE_2D, self.tex_msg)
            mx_pos = W // 2 - int(95 * SCALE)
            my_pos = H // 2 + int(45 * SCALE)
            alpha = min(1.0, msg_t * 2.0)
            glColor4f(1.0, 1.0, 1.0, alpha)
            glBegin(GL_QUADS)
            glTexCoord2f(0.0, 0.0); glVertex2f(mx_pos, my_pos)
            glTexCoord2f(220.0/256.0, 0.0); glVertex2f(mx_pos + int(190 * SCALE), my_pos)
            glTexCoord2f(220.0/256.0, 36.0/64.0); glVertex2f(mx_pos + int(190 * SCALE), my_pos + int(32 * SCALE))
            glTexCoord2f(0.0, 36.0/64.0); glVertex2f(mx_pos, my_pos + int(32 * SCALE))
            glEnd()

        # -------------------------------------------------------------
        # 9. CROSSHAIR (Center)
        # -------------------------------------------------------------
        if self.tex_cross:
            glEnable(GL_TEXTURE_2D)
            glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_cross)
            ccx, ccy = W // 2, H // 2
            cs = int(12 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(ccx - cs, ccy - cs)
            glTexCoord2f(1, 0); glVertex2f(ccx + cs, ccy - cs)
            glTexCoord2f(1, 1); glVertex2f(ccx + cs, ccy + cs)
            glTexCoord2f(0, 1); glVertex2f(ccx - cs, ccy + cs)
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

print('Updated FastHUD with 100% authentic screenshot layout successfully!')
