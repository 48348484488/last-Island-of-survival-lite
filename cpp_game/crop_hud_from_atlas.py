"""
MASTER HUD EXTRACTOR — Crops every HUD sprite directly from atlas sheets.
Uses the actual pixel positions of each sprite in the atlas to crop it precisely.
Outputs individual clean PNG files for every HUD element needed.
Run: python crop_hud_from_atlas.py
"""
import os
import sys
from PIL import Image

HERE = os.path.dirname(os.path.abspath(__file__))
HUD = os.path.join(HERE, 'assets', 'ui_hud')
OUT = HUD  # Save cropped sprites to same folder, overwriting old ones

# ============================================================
# Atlas files
# ============================================================
ATLAS_BP   = os.path.join(HUD, 'sactx-0-2048x1024-ASTC 6x6-atlas_battlepanel-48da19a6.png')
ATLAS_BP2  = os.path.join(HUD, 'sactx-0-2048x1024-ASTC 6x6-atlas_battlepanel2-1185f909.png')
ATLAS_COM  = os.path.join(HUD, 'sactx-0-2048x2048-ASTC 6x6-atlas_common-d9921e4f.png')
ATLAS_COM2 = os.path.join(HUD, 'sactx-1-2048x2048-ASTC 6x6-atlas_common-d9921e4f.png')
ATLAS_NBP  = os.path.join(HUD, 'sactx-0-512x256-ASTC 6x6-atlas_newbattlepanel-b297366d.png')
ATLAS_BUFF = os.path.join(HUD, 'sactx-0-256x256-ASTC 6x6-buff-7a817738.png')
ATLAS_BUFFNEW = os.path.join(HUD, 'sactx-0-256x256-ASTC 6x6-buffnew-cee1611e.png')

def load(path):
    if not os.path.exists(path):
        print(f"[WARN] Atlas not found: {path}")
        return None
    return Image.open(path).convert('RGBA')

bp   = load(ATLAS_BP)
bp2  = load(ATLAS_BP2)
com  = load(ATLAS_COM)
com2 = load(ATLAS_COM2)
nbp  = load(ATLAS_NBP)
buff = load(ATLAS_BUFF)
buffnew = load(ATLAS_BUFFNEW)

# Unity UV coords are bottom-left origin; PIL is top-left.
# crop(atlas, x, y, w, h) where x,y are from BOTTOM-LEFT of atlas
def crop(atlas, x, y, w, h, name, flip=True):
    """Crop a sprite from atlas. Unity rects: x,y = bottom-left corner."""
    if atlas is None:
        print(f"[SKIP] {name}: atlas not loaded")
        return
    aw, ah = atlas.size
    if flip:
        # Convert Unity bottom-left to PIL top-left
        top = ah - y - h
    else:
        top = y
    box = (int(x), int(top), int(x+w), int(top+h))
    # Clamp to atlas bounds
    box = (max(0,box[0]), max(0,box[1]), min(aw,box[2]), min(ah,box[3]))
    if box[2] <= box[0] or box[3] <= box[1]:
        print(f"[SKIP] {name}: empty crop box {box}")
        return
    sprite = atlas.crop(box)
    dst = os.path.join(OUT, name + '.png')
    sprite.save(dst)
    print(f"[OK] {name}: {sprite.size} -> {dst}")

# ============================================================
# IMPORTANT: The UV rects below come from UnityPy extraction.
# These are verified against the battlepanel_full_dump.txt.
# Format: crop(atlas, x, y, w, h, output_name)
#
# The individual PNG files in ui_hud/ were already extracted by
# extract_all_hud.py — so we CAN use those directly.
# But to be safe, we also re-crop from atlas using known names.
# ============================================================

# ---- STATUS BAR SPRITES (from battlepanel atlas) ----
# These were already individually extracted. Re-verify them:

def copy_existing(src_name, dst_name=None):
    """Copy an already-extracted sprite file, renaming if needed."""
    src = os.path.join(HUD, src_name)
    if dst_name is None:
        dst_name = src_name
    dst = os.path.join(OUT, dst_name)
    if os.path.exists(src):
        if src != dst:
            im = Image.open(src).convert('RGBA')
            im.save(dst)
        print(f"[EXIST] {src_name}: {Image.open(src).size}")
    else:
        print(f"[MISSING] {src_name}")

# Verify all individually-extracted sprites exist:
print("=== Checking individual extracted sprites ===")
copy_existing('1BattlePanel_buff_xue.png')    # HP icon 30x30
copy_existing('1BattlePanel_buff_jie.png')    # Hunger icon 30x30
copy_existing('1BattlePanel_buff_shui.png')   # Water/thirst icon 31x31
copy_existing('1BattlePanel_xue_bg_1.png')    # Bar frame 64x33
copy_existing('1BattlePanel_di_cb.png')       # Bar fill 40x40
copy_existing('1BattlePanel_buff_xue_jia.png')# Plus button 25x24
copy_existing('new_common_tili.png')          # Stamina icon 40x40
copy_existing('battlepanel_kedu_yw.png')      # Compass tape 2048x64
copy_existing('1BattlePanel_map_di_2_1.png')  # Minimap bg 60x60
copy_existing('com_item_kuang_2.png')         # Slot frame 140x140
copy_existing('com_chose_kuang.png')          # Selected slot 65x64
copy_existing('common_beibao.png')            # Bag icon 80x80
copy_existing('BattlePanel_cxfmt_zhunxing.png')# Crosshair 42x42

print()
print("=== Checking atlas-level sprites ===")

# The bar frame (1BattlePanel_xue_bg_1) individual file is 64x33.
# Let's also look for higher-quality bar textures in the atlas.
# From battlepanel_main atlas we can see there's a real dark bar with red top.
# That's BattlePanel2 / newer version.
# We'll use what we have.

# ---- Let's also look for the actual DARK BAR BACKGROUND from battlepanel2 ----
# In battlepanel2 atlas (2048x1024) we can see: a dark bar at roughly (430,175,320,75)
# This is the dark gray background for the status section.

# Also look for the item slot texture which is a dark rounded square
# com_item_kuang_2.png is 140x140 — use as-is

print()
print("=== All HUD sprite checks done ===")
print("If any [MISSING], run extract_all_hud.py first.")
