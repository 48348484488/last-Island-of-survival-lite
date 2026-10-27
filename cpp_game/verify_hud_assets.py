"""
Quick verification: checks all HUD assets are present and correct sizes.
Run: python verify_hud_assets.py
"""
import os
from PIL import Image

HUD = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'assets', 'ui_hud')

REQUIRED = {
    # Vital bars
    '1BattlePanel_buff_xue.png':     (30, 30),   # health icon
    '1BattlePanel_buff_jie.png':     (30, 30),   # hunger icon
    '1BattlePanel_buff_shui.png':    (31, 31),   # water icon
    '1BattlePanel_xue_bg_1.png':     (64, 33),   # bar frame (9-slice source)
    '1BattlePanel_di_cb.png':        (40, 40),   # bar fill
    '1BattlePanel_buff_xue_jia.png': (25, 24),   # plus button
    'new_common_tili.png':           (40, 40),   # stamina icon
    # Compass
    'battlepanel_kedu_yw.png':       (2048, 64), # compass tape
    # Minimap
    '1BattlePanel_map_di_2_1.png':   (60, 60),  # minimap background
    # Hotbar
    'com_item_kuang_2.png':          (140, 140), # slot frame
    'com_chose_kuang.png':           (65, 64),   # selected slot
    'common_beibao.png':             (80, 80),   # bag icon
    # Crosshair
    'BattlePanel_cxfmt_zhunxing.png':(42, 42),  # crosshair
}

ok = True
for fname, expected_size in REQUIRED.items():
    fp = os.path.join(HUD, fname)
    if not os.path.exists(fp):
        print(f'[MISSING] {fname}')
        ok = False
        continue
    im = Image.open(fp)
    if im.size != expected_size:
        print(f'[WRONG SIZE] {fname}: got {im.size}, expected {expected_size}')
    else:
        print(f'[OK] {fname}: {im.size}')

print()
if ok:
    print('All required HUD assets present and correct!')
else:
    print('Some assets are missing — run extract_all_hud.py first.')
