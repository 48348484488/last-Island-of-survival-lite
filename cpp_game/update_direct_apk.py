# -*- coding: utf-8 -*-
with open('viewer3d.py', 'r', encoding='utf-8') as f:
    text = f.read()

replacements = [
    ("self.tex_bar_bg     = load_texture(u('bar_bg_clean.png')) or load_texture(u('1BattlePanel_xue_bg_1_sliced.png'))",
     "self.tex_bar_bg     = load_texture(u('1BattlePanel_xue_bg_1.png')) or load_texture(u('bar_bg_clean.png'))"),
    ("self.tex_bar_fill   = load_texture(u('bar_fill_clean.png')) or load_texture(u('1BattlePanel_di_cb.png'))",
     "self.tex_bar_fill   = load_texture(u('1BattlePanel_di_cb.png')) or load_texture(u('bar_fill_clean.png'))"),
    ("self.tex_backpack   = load_texture(u('1BattlePanell_bb.png')) or load_texture(u('hud_backpack_slot.png'))",
     "self.tex_backpack   = load_texture(u('1BattlePanell_bb.png'))"),
    ("self.tex_build_btn  = load_texture(u('hud_build_slot.png')) or load_texture(u('com_xiulitubiao.png'))",
     "self.tex_build_btn  = load_texture(u('com_xiulitubiao.png'))"),
    ("self.tex_btn_jump   = load_texture(u('authentic_btn_jump.png')) or load_texture(u('1BattlePanel_cz_t.png'))",
     "self.tex_btn_jump   = load_texture(u('1BattlePanel_cz_t.png'))"),
    ("self.tex_btn_crouch = load_texture(u('authentic_btn_crouch.png')) or load_texture(u('1BattlePanel_cz_dun.png'))",
     "self.tex_btn_crouch = load_texture(u('1BattlePanel_cz_dun.png'))"),
    ("self.tex_btn_prone  = load_texture(u('authentic_btn_prone.png')) or load_texture(u('1BattlePanel_cz_pa.png'))",
     "self.tex_btn_prone  = load_texture(u('1BattlePanel_cz_pa.png'))"),
    ("self.tex_juice      = load_texture(u('hud_item_juice.png')) or load_texture(u('BattlePanel2_YINGSHUI2.png'))",
     "self.tex_juice      = load_texture(u('BattlePanel2_YINGSHUI2.png'))"),
]

for old, new in replacements:
    if old in text:
        text = text.replace(old, new)
        print(f"Replaced: {new[:30]}...")

with open('viewer3d.py', 'w', encoding='utf-8') as f:
    f.write(text)

print("Updated viewer3d.py with direct APK assets!")
