# -*- coding: utf-8 -*-
"""
Update FastHUD to use 100% RAW extracted APK assets:
- No cropped images.
- Pure Unity assets from install_time_asset_pack.apk.
"""
import re

new_fasthud = '''class FastHUD:
    def __init__(self, asset_dir):
        u = lambda f: os.path.join(asset_dir, 'ui_hud', f)

        # 1. Authentic Crosshair & Compass (from battlepanel.ab)
        self.tex_cross      = load_texture(u('BattlePanel_cxfmt_zhunxing.png'))
        self.tex_compass    = load_texture(u('battlepanel_kedu_yw.png'))

        # 2. Authentic Minimap: Island texture (map_map.ab) & Frame (battlepanel.ab)
        self.tex_map_tile   = load_texture(u('island_map_2048.png'))
        self.tex_map_di     = load_texture(u('1BattlePanel_map_di_2_1.png'))
        self.tex_needle     = load_texture(u('1BattlePanel_sy_zaiju_jt.png'))
        self.tex_tq_bg      = load_texture(u('1BattlePanel_tq_bg.png'))

        # 3. Authentic Vitals Badges & 9-slice Bars (from battlepanel.ab)
        self.tex_hp_badge   = load_texture(u('1BattlePanel_buff_xue.png'))
        self.tex_food_badge = load_texture(u('1BattlePanel_buff_jie.png'))
        self.tex_drop_badge = load_texture(u('1BattlePanel_buff_shui.png'))
        self.tex_bar_bg     = load_texture(u('1BattlePanel_xue_bg_1.png'))
        self.tex_bar_fill   = load_texture(u('1BattlePanel_di_cb.png'))

        # 4. Authentic Hotbar Slots & Icons (from battlepanel.ab & common.ab)
        self.tex_slot       = load_texture(u('1BattlePanel_buttle_fang_1.png'))
        self.tex_backpack   = load_texture(u('1BattlePanell_bb.png'))
        self.tex_build_btn  = load_texture(u('com_xiulitubiao.png'))

        # 5. Authentic Action Buttons (from battlepanel.ab & battlecustompanel.ab)
        self.tex_btn_att_bg = load_texture(u('BattleCustomPanel_tb_d.png'))
        self.tex_btn_att_fg = load_texture(u('BattlePanel_fire.png'))
        self.tex_btn_jump   = load_texture(u('1BattlePanel_cz_t.png'))
        self.tex_btn_crouch = load_texture(u('1BattlePanel_cz_dun.png'))
        self.tex_btn_prone  = load_texture(u('1BattlePanel_cz_pa.png'))
        self.tex_btn_sprint = load_texture(u('BattlePanel2_car_zidongbengpao2.png'))

        # 6. Authentic Virtual Joystick (from battlepanel2.ab)
        self.tex_joy_base   = load_texture(u('BattlePanel2_car_yaogan_1.png'))
        self.tex_joy_knob   = load_texture(u('BattlePanel2_car_yaogan_2.png'))

        # 7. Authentic Top-Left Menu & Team (from battlepanel.ab)
        self.tex_top_bg     = load_texture(u('1BattlePanel_zuo_shang_infoBG_1.png'))
        self.tex_top_team   = load_texture(u('1BattlePanel_zuo_shang_info.png'))
        self.tex_top_gl     = load_texture(u('1BattlePanel_zuo_shang_infogl.png'))
        self.tex_top_phb    = load_texture(u('1BattlePanel_zuo_shang_infophb.png'))
        self.tex_top_zz     = load_texture(u('1BattlePanel_zuo_shang_infoZZ.png'))
        self.tex_top_quit   = load_texture(u('1BattlePanel_quit.png'))

        # 8. Authentic Inventory Item Icons (from APK gun.ab, tubiao.ab, wq_shifu)
        self.tex_axe        = load_texture(u('wq_shifu.png'))
        self.tex_juice      = load_texture(u('BattlePanel2_YINGSHUI2.png'))
        self.tex_meat       = load_texture(u('tb_yurou_1.png'))
        self.tex_bandage    = load_texture(u('bengdai.png'))
        self.tex_sulfur     = load_texture(u('ks_lhk.png'))
        self.tex_wood       = load_texture(u('tb_cl_mt.png'))
        self.tex_metal      = load_texture(u('tb_cl_tiek.png'))
        self.tex_stone      = load_texture(u('ks_st.png'))

        # Dynamic Text Textures (POT dimensions)
        self.tex_num_hp   = glGenTextures(1)
        self.tex_num_food = glGenTextures(1)
        self.tex_num_wat  = glGenTextures(1)
        self.tex_map_top  = glGenTextures(1)
        self.tex_map_bot  = glGenTextures(1)
        self.tex_msg      = glGenTextures(1)
        self.tex_target   = glGenTextures(1)
        self.tex_civ      = glGenTextures(1)
        self.tex_chat     = glGenTextures(1)

        self.last_hp = -1
        self.last_food = -1
        self.last_water = -1
        self.last_px = None
        self.last_pz = None
        self.last_msg = None
        self.last_target = None
        self.static_init = False

    def _render_text_to_tex(self, tid, surf, pot_w, pot_h):
        glBindTexture(GL_TEXTURE_2D, tid)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR)
        data = pygame.image.tostring(surf, 'RGBA', False)
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, pot_w, pot_h, 0, GL_RGBA, GL_UNSIGNED_BYTE, data)

    def _draw_sub_tex(self, tid, x, y, w, h, u_w, v_h, pot_w, pot_h):
        glEnable(GL_TEXTURE_2D)
        glBindTexture(GL_TEXTURE_2D, tid)
        u0, u1 = 0.0, float(u_w) / pot_w
        v_top, v_bot = 0.0, float(v_h) / pot_h
        glBegin(GL_QUADS)
        glTexCoord2f(u0, v_bot); glVertex2f(x, y)
        glTexCoord2f(u1, v_bot); glVertex2f(x + w, y)
        glTexCoord2f(u1, v_top); glVertex2f(x + w, y + h)
        glTexCoord2f(u0, v_top); glVertex2f(x, y + h)
        glEnd()

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
        # 1. TOP-LEFT MENU & TEAM SLOTS (Raw APK battlepanel.ab)
        # -------------------------------------------------------------
        if self.tex_top_bg:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_top_bg)
            tw, th = int(220 * SCALE), int(28 * SCALE)
            tx, ty = int(12 * SCALE), H - th - int(8 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(tx, ty)
            glTexCoord2f(1, 0); glVertex2f(tx + tw, ty)
            glTexCoord2f(1, 1); glVertex2f(tx + tw, ty + th)
            glTexCoord2f(0, 1); glVertex2f(tx, ty + th)
            glEnd()

        if self.tex_top_team:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_top_team)
            tm_w, tm_h = int(125 * SCALE), int(56 * SCALE)
            tm_x, tm_y = int(12 * SCALE), H - int(96 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(tm_x, tm_y)
            glTexCoord2f(1, 0); glVertex2f(tm_x + tm_w, tm_y)
            glTexCoord2f(1, 1); glVertex2f(tm_x + tm_w, tm_y + tm_h)
            glTexCoord2f(0, 1); glVertex2f(tm_x, tm_y + tm_h)
            glEnd()

        # -------------------------------------------------------------
        # 2. TOP COMPASS BAR (Raw APK battlepanel_kedu_yw.png)
        # -------------------------------------------------------------
        comp_w = int(320 * SCALE)
        comp_h = max(16, int(22 * SCALE))
        comp_x = W // 2 - comp_w // 2
        comp_y = H - comp_h - int(4 * SCALE)

        glDisable(GL_TEXTURE_2D)
        glColor4f(0.04, 0.05, 0.08, 0.60)
        glBegin(GL_QUADS)
        glVertex2f(comp_x, comp_y)
        glVertex2f(comp_x + comp_w, comp_y)
        glVertex2f(comp_x + comp_w, comp_y + comp_h)
        glVertex2f(comp_x, comp_y + comp_h)
        glEnd()

        if self.tex_compass:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_compass)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP)

            u_center = (-yaw / (2.0 * math.pi)) % 1.0
            u_span = 0.16
            u0 = u_center - u_span / 2.0
            u1 = u_center + u_span / 2.0

            glBegin(GL_QUADS)
            glTexCoord2f(u0, 0.0); glVertex2f(comp_x, comp_y)
            glTexCoord2f(u1, 0.0); glVertex2f(comp_x + comp_w, comp_y)
            glTexCoord2f(u1, 1.0); glVertex2f(comp_x + comp_w, comp_y + comp_h)
            glTexCoord2f(u0, 1.0); glVertex2f(comp_x, comp_y + comp_h)
            glEnd()

        # White down-arrow indicator at center
        glDisable(GL_TEXTURE_2D)
        glColor4f(1.0, 1.0, 1.0, 0.95)
        mid_x = comp_x + comp_w // 2
        arr_sz = int(4 * SCALE)
        glBegin(GL_TRIANGLES)
        glVertex2f(mid_x, comp_y + comp_h - int(8 * SCALE))
        glVertex2f(mid_x - arr_sz, comp_y + comp_h - 1)
        glVertex2f(mid_x + arr_sz, comp_y + comp_h - 1)
        glEnd()

        # -------------------------------------------------------------
        # 3. AUTHENTIC MINIMAP RADAR (Island map_map.ab + battlepanel.ab)
        # -------------------------------------------------------------
        map_w = map_h = int(112 * SCALE)
        map_x = W - map_w - int(12 * SCALE)
        map_y = H - map_h - int(32 * SCALE)

        # Top pill for Time & Moon (21:26 🌙)
        pill_w = int(68 * SCALE)
        pill_h = int(16 * SCALE)
        pill_x = map_x + map_w - pill_w
        pill_y = map_y + map_h + int(2 * SCALE)

        if self.tex_tq_bg:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.85)
            glBindTexture(GL_TEXTURE_2D, self.tex_tq_bg)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(pill_x, pill_y)
            glTexCoord2f(1, 0); glVertex2f(pill_x + pill_w, pill_y)
            glTexCoord2f(1, 1); glVertex2f(pill_x + pill_w, pill_y + pill_h)
            glTexCoord2f(0, 1); glVertex2f(pill_x, pill_y + pill_h)
            glEnd()
        else:
            glDisable(GL_TEXTURE_2D); glColor4f(0.04, 0.06, 0.10, 0.85)
            glBegin(GL_QUADS)
            glVertex2f(pill_x, pill_y); glVertex2f(pill_x + pill_w, pill_y)
            glVertex2f(pill_x + pill_w, pill_y + pill_h); glVertex2f(pill_x, pill_y + pill_h)
            glEnd()

        if not hasattr(self, '_time_rendered'):
            self._time_rendered = True
            s_top = pygame.Surface((128, 32), pygame.SRCALPHA); s_top.fill((0, 0, 0, 0))
            t_str = FONT_TEXT.render("21:26 🌙", True, (245, 245, 255))
            s_top.blit(t_str, (4, 4))
            self._render_text_to_tex(self.tex_map_top, s_top, 128, 32)

        glColor4f(1.0, 1.0, 1.0, 1.0)
        self._draw_sub_tex(self.tex_map_top, pill_x + 2, pill_y + 1, pill_w - 4, pill_h - 2, 70, 22, 128, 32)

        # Minimap Terrain (using authentic Map_map texture from APK centered on player coords)
        if self.tex_map_tile:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_map_tile)
            
            # Map coordinates offset
            u_c = (0.5 + px / 4000.0) % 1.0
            v_c = (0.5 + pz / 4000.0) % 1.0
            span = 0.08
            glBegin(GL_QUADS)
            glTexCoord2f(u_c - span, v_c - span); glVertex2f(map_x, map_y)
            glTexCoord2f(u_c + span, v_c - span); glVertex2f(map_x + map_w, map_y)
            glTexCoord2f(u_c + span, v_c + span); glVertex2f(map_x + map_w, map_y + map_h)
            glTexCoord2f(u_c - span, v_c + span); glVertex2f(map_x, map_y + map_h)
            glEnd()

        # Minimap Frame Border (1BattlePanel_map_di_2_1.png)
        if self.tex_map_di:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.75)
            glBindTexture(GL_TEXTURE_2D, self.tex_map_di)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(map_x, map_y)
            glTexCoord2f(1, 0); glVertex2f(map_x + map_w, map_y)
            glTexCoord2f(1, 1); glVertex2f(map_x + map_w, map_y + map_h)
            glTexCoord2f(0, 1); glVertex2f(map_x, map_y + map_h)
            glEnd()

        # Player location center dot
        mcx, mcy = map_x + map_w // 2, map_y + map_h // 2
        glDisable(GL_TEXTURE_2D)
        glColor4f(1.0, 0.92, 0.25, 1.0)
        glPointSize(max(4.0, 6.0 * SCALE))
        glBegin(GL_POINTS); glVertex2f(mcx, mcy); glEnd()

        # Orientation cone needle (from 1BattlePanel_sy_zaiju_jt.png or vector arrow)
        nx_p = mcx + math.sin(-yaw) * (map_w * 0.26)
        ny_p = mcy + math.cos(-yaw) * (map_h * 0.26)
        glColor4f(0.95, 0.25, 0.25, 1.0)
        glPointSize(max(4.0, 5.5 * SCALE))
        glBegin(GL_POINTS); glVertex2f(nx_p, ny_p); glEnd()

        # Bottom info strip (15 / coords)
        bot_strip_h = int(22 * SCALE)
        glDisable(GL_TEXTURE_2D)
        glColor4f(0.04, 0.04, 0.06, 0.85)
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
        self._draw_sub_tex(self.tex_map_bot, map_x, map_y, map_w, bot_strip_h, 128, 28, 128, 32)

        # -------------------------------------------------------------
        # 4. THREE VITAL STATUS BARS (Health, Hunger, Hydration)
        # -------------------------------------------------------------
        bar_w   = int(84 * SCALE)
        bar_h   = max(14, int(18 * SCALE))
        badge_w = bar_h
        gap_v   = int(2 * SCALE)

        vit_x = map_x - bar_w - badge_w - int(8 * SCALE)
        vit_y = map_y + map_h - bar_h

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

        hp_color = (0.24, 0.65, 0.28, 0.95) if health >= 75.0 else ((0.96, 0.49, 0.18, 0.95) if health >= 25.0 else (0.89, 0.20, 0.20, 0.95))
        sand_fill = (0.94, 0.77, 0.51, 0.95)

        vitals = [
            (self.tex_hp_badge,   health, 100.0, hp_color,  self.tex_num_hp),
            (self.tex_food_badge, food,   500.0, sand_fill, self.tex_num_food),
            (self.tex_drop_badge, water,  500.0, sand_fill, self.tex_num_wat),
        ]

        glEnable(GL_TEXTURE_2D)
        for i, (badge_tex, cur_val, max_val, col, num_tex) in enumerate(vitals):
            by = vit_y - i * (bar_h + gap_v)
            fill_pct = max(0.0, min(1.0, cur_val / max_val))

            if i == 2:
                glDisable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.9)
                arr_x = vit_x - int(8 * SCALE)
                glBegin(GL_TRIANGLES)
                glVertex2f(arr_x + int(5 * SCALE), by + bar_h // 2)
                glVertex2f(arr_x, by + bar_h // 2 - int(3 * SCALE))
                glVertex2f(arr_x, by + bar_h // 2 + int(3 * SCALE))
                glEnd()
                glEnable(GL_TEXTURE_2D)

            # Badge square icon (1BattlePanel_buff_*.png)
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

            # 9-slice Bar Frame (1BattlePanel_xue_bg_1.png)
            if self.tex_bar_bg:
                glColor4f(1.0, 1.0, 1.0, 0.90)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_bg)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x0, by)
                glTexCoord2f(1, 0); glVertex2f(bar_x0 + bar_w, by)
                glTexCoord2f(1, 1); glVertex2f(bar_x0 + bar_w, by + bar_h)
                glTexCoord2f(0, 1); glVertex2f(bar_x0, by + bar_h)
                glEnd()

            # Colored Fill (1BattlePanel_di_cb.png)
            if self.tex_bar_fill and fill_pct > 0:
                fw = int((bar_w - 2) * fill_pct)
                glColor4f(*col)
                glBindTexture(GL_TEXTURE_2D, self.tex_bar_fill)
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(bar_x0 + 1, by + 1)
                glTexCoord2f(1, 0); glVertex2f(bar_x0 + 1 + fw, by + 1)
                glTexCoord2f(1, 1); glVertex2f(bar_x0 + 1 + fw, by + 1 + bar_h - 2)
                glTexCoord2f(0, 1); glVertex2f(bar_x0 + 1, by + 1 + bar_h - 2)
                glEnd()

            # Number text overlay
            if num_tex:
                glColor4f(1.0, 1.0, 1.0, 1.0)
                tw_b = int(48 * SCALE)
                tx0 = bar_x0 + bar_w // 2 - tw_b // 2
                self._draw_sub_tex(num_tex, tx0, by, tw_b, bar_h, 64, 32, 64, 32)

        # -------------------------------------------------------------
        # 5. HOTBAR (Bottom-Center)
        # -------------------------------------------------------------
        slot_sz = max(36, int(42 * SCALE))
        gap = max(3, int(4 * SCALE))
        num_slots = 6
        total_hotbar_w = num_slots * slot_sz + (num_slots - 1) * gap
        hx = W // 2 - total_hotbar_w // 2
        hy = int(18 * SCALE)

        # Backpack slot on far left (1BattlePanell_bb.png)
        bp_sz = slot_sz
        bp_x  = hx - bp_sz - int(8 * SCALE)
        if self.tex_backpack:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_backpack)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(bp_x, hy)
            glTexCoord2f(1, 0); glVertex2f(bp_x + bp_sz, hy)
            glTexCoord2f(1, 1); glVertex2f(bp_x + bp_sz, hy + bp_sz)
            glTexCoord2f(0, 1); glVertex2f(bp_x, hy + bp_sz)
            glEnd()

        # Build hammer button on far right (com_xiulitubiao.png)
        build_x = hx + total_hotbar_w + int(8 * SCALE)
        if self.tex_build_btn:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.95)
            glBindTexture(GL_TEXTURE_2D, self.tex_build_btn)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(build_x, hy)
            glTexCoord2f(1, 0); glVertex2f(build_x + bp_sz, hy)
            glTexCoord2f(1, 1); glVertex2f(build_x + bp_sz, hy + bp_sz)
            glTexCoord2f(0, 1); glVertex2f(build_x, hy + bp_sz)
            glEnd()

        # Pure APK items: 1:Axe, 2:Juice, 3:Fish meat, 4:Bandage, 5:Sulfur, 6:Wood/Metal
        item_icons = [
            self.tex_axe,
            self.tex_juice,
            self.tex_meat,
            self.tex_bandage,
            self.tex_sulfur if sulfur > 0 else self.tex_metal,
            self.tex_wood if wood > 0 else 0
        ]

        for idx in range(num_slots):
            sx = hx + idx * (slot_sz + gap)

            # Slot box (1BattlePanel_buttle_fang_1.png)
            if self.tex_slot:
                glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.88)
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
                glColor4f(0.78, 0.92, 0.12, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy)
                glVertex2f(sx + 3, hy)
                glVertex2f(sx + 3, hy + slot_sz)
                glVertex2f(sx, hy + slot_sz)
                glEnd()

                glColor4f(0.96, 0.55, 0.18, 1.0)
                glBegin(GL_QUADS)
                glVertex2f(sx, hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(5 * SCALE))
                glVertex2f(sx + int(slot_sz * 0.85), hy - int(3 * SCALE))
                glVertex2f(sx, hy - int(3 * SCALE))
                glEnd()

            # Item Icon inside slot
            if item_icons[idx]:
                glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 1.0)
                glBindTexture(GL_TEXTURE_2D, item_icons[idx])
                pad = max(3, int(4 * SCALE))
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(sx + pad, hy + pad)
                glTexCoord2f(1, 0); glVertex2f(sx + slot_sz - pad, hy + pad)
                glTexCoord2f(1, 1); glVertex2f(sx + slot_sz - pad, hy + slot_sz - pad)
                glTexCoord2f(0, 1); glVertex2f(sx + pad, hy + slot_sz - pad)
                glEnd()

        # Text labels (Civilization Restart & Chat)
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
        self._draw_sub_tex(self.tex_chat, hx, hy + slot_sz + int(4 * SCALE), int(220 * SCALE), int(15 * SCALE), 220, 16, 512, 32)

        # -------------------------------------------------------------
        # 6. ACTION CONTROLS (Raw APK Circular Sprites)
        # -------------------------------------------------------------
        big_sz = int(68 * SCALE)
        bx_att = W - int(170 * SCALE)
        by_att = int(82 * SCALE)

        # Attack button: BattleCustomPanel_tb_d plate + BattlePanel_fire knife
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
            glEnd()

        # Jump (1BattlePanel_cz_t.png)
        if self.tex_btn_jump:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_jump)
            jx = W - int(72 * SCALE)
            jy = int(76 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(jx, jy)
            glTexCoord2f(1, 0); glVertex2f(jx + btn_sz, jy)
            glTexCoord2f(1, 1); glVertex2f(jx + btn_sz, jy + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(jx, jy + btn_sz)
            glEnd()

        # Crouch / Aim (1BattlePanel_cz_dun.png)
        if self.tex_btn_crouch:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_crouch)
            cx = W - int(116 * SCALE)
            cy = int(22 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(cx, cy)
            glTexCoord2f(1, 0); glVertex2f(cx + btn_sz, cy)
            glTexCoord2f(1, 1); glVertex2f(cx + btn_sz, cy + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(cx, cy + btn_sz)
            glEnd()

        # Prone (1BattlePanel_cz_pa.png)
        if self.tex_btn_prone:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.90)
            glBindTexture(GL_TEXTURE_2D, self.tex_btn_prone)
            px_b = bx_att - int(44 * SCALE)
            py_b = int(22 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(px_b, py_b)
            glTexCoord2f(1, 0); glVertex2f(px_b + btn_sz, py_b)
            glTexCoord2f(1, 1); glVertex2f(px_b + btn_sz, py_b + btn_sz)
            glTexCoord2f(0, 1); glVertex2f(px_b, py_b + btn_sz)
            glEnd()

        # -------------------------------------------------------------
        # 7. AUTHENTIC JOYSTICK (BattlePanel2_car_yaogan_1 + 2)
        # -------------------------------------------------------------
        if self.tex_joy_base:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.80)
            glBindTexture(GL_TEXTURE_2D, self.tex_joy_base)
            jw = jh = int(96 * SCALE)
            jx, jy = int(45 * SCALE), int(35 * SCALE)
            glBegin(GL_QUADS)
            glTexCoord2f(0, 0); glVertex2f(jx, jy)
            glTexCoord2f(1, 0); glVertex2f(jx + jw, jy)
            glTexCoord2f(1, 1); glVertex2f(jx + jw, jy + jh)
            glTexCoord2f(0, 1); glVertex2f(jx, jy + jh)
            glEnd()

            if self.tex_joy_knob:
                glBindTexture(GL_TEXTURE_2D, self.tex_joy_knob)
                kw = kh = int(48 * SCALE)
                kx = jx + (jw - kw) // 2
                ky = jy + (jh - kh) // 2
                glBegin(GL_QUADS)
                glTexCoord2f(0, 0); glVertex2f(kx, ky)
                glTexCoord2f(1, 0); glVertex2f(kx + kw, ky)
                glTexCoord2f(1, 1); glVertex2f(kx + kw, ky + kh)
                glTexCoord2f(0, 1); glVertex2f(kx, ky + kh)
                glEnd()

        # -------------------------------------------------------------
        # 8. TARGET NODE NAME & MINING POPUP
        # -------------------------------------------------------------
        if target_node:
            tname, dur_pct = target_node
            tx_c = W // 2
            ty_c = H // 2 + int(60 * SCALE)

            if tname != self.last_target:
                self.last_target = tname
                surf_t = pygame.Surface((256, 32), pygame.SRCALPHA); surf_t.fill((0, 0, 0, 0))
                t_str = FONT_TEXT.render(tname, True, (255, 255, 255))
                surf_t.blit(t_str, (128 - t_str.get_width() // 2, 2))
                self._render_text_to_tex(self.tex_target, surf_t, 256, 32)

            glColor4f(1.0, 1.0, 1.0, 1.0)
            self._draw_sub_tex(self.tex_target, tx_c - int(80 * SCALE), ty_c, int(160 * SCALE), int(20 * SCALE), 256, 24, 256, 32)

            db_w = int(110 * SCALE)
            db_h = max(3, int(4 * SCALE))
            db_x = tx_c - db_w // 2
            db_y = ty_c - int(6 * SCALE)
            glDisable(GL_TEXTURE_2D)
            glColor4f(0.04, 0.04, 0.04, 0.85)
            glBegin(GL_QUADS)
            glVertex2f(db_x, db_y); glVertex2f(db_x + db_w, db_y)
            glVertex2f(db_x + db_w, db_y + db_h); glVertex2f(db_x, db_y + db_h)
            glEnd()
            glColor4f(0.85, 0.65, 0.42, 1.0)
            fill_w = int(db_w * dur_pct)
            glBegin(GL_QUADS)
            glVertex2f(db_x, db_y); glVertex2f(db_x + fill_w, db_y)
            glVertex2f(db_x + fill_w, db_y + db_h); glVertex2f(db_x, db_y + db_h)
            glEnd()

        # Harvest Notification Popup (Iron Mine +42)
        if msg_t > 0 and msg:
            if msg != self.last_msg:
                self.last_msg = msg
                surf_msg = pygame.Surface((256, 64), pygame.SRCALPHA); surf_msg.fill((0, 0, 0, 0))
                pygame.draw.rect(surf_msg, (12, 12, 12, 220), (0, 0, 200, 36), border_radius=4)
                t_msg = FONT_TEXT.render(msg, True, (255, 255, 255))
                surf_msg.blit(t_msg, (100 - t_msg.get_width() // 2, 7))
                self._render_text_to_tex(self.tex_msg, surf_msg, 256, 64)

            mx_pos = W // 2 - int(85 * SCALE)
            my_pos = H // 2 + int(24 * SCALE)
            alpha = min(1.0, msg_t * 2.0)
            glColor4f(1.0, 1.0, 1.0, alpha)
            self._draw_sub_tex(self.tex_msg, mx_pos, my_pos, int(170 * SCALE), int(30 * SCALE), 200, 36, 256, 64)

        # -------------------------------------------------------------
        # 9. CROSSHAIR (Center)
        # -------------------------------------------------------------
        if self.tex_cross:
            glEnable(GL_TEXTURE_2D); glColor4f(1.0, 1.0, 1.0, 0.90)
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

with open('viewer3d.py', 'r', encoding='utf-8') as f:
    text = f.read()

pattern = r'class FastHUD:.*?(?=\n# -{10,}\ndef main\(\):|\ndef main\(\):)'
new_text = re.sub(pattern, new_fasthud, text, flags=re.DOTALL)

with open('viewer3d.py', 'w', encoding='utf-8') as f:
    f.write(new_text)

print("FastHUD rewritten with 100% pure raw APK assets!")
