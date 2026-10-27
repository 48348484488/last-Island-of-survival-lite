"""
Extract exact UV rects for all HUD-relevant sprites from the APK atlas metadata.
This tells us exactly where each named sprite sits in its atlas texture.
"""
import UnityPy
import json
import os

APK = os.path.join(os.path.dirname(__file__), '..', 'xapk_content', 'install_time_asset_pack.apk')

KEYWORDS = [
    'xue', 'buff', 'di_cb', 'kedu', 'map_di', 'kuang', 'beibao',
    'tili', 'cxfmt', 'zhunxing', 'jie', 'shui', 'tiao', 'xinxidi',
    'hotbar', 'bag', 'chose', 'wupin', 'item', 'slot', 'bar'
]

sprites_found = {}
env = UnityPy.load(APK)

for obj in env.objects:
    if obj.type.name == 'Sprite':
        try:
            d = obj.read()
            name = d.name
            low = name.lower()
            if any(k in low for k in KEYWORDS):
                r = d.rect
                try:
                    tex_name = d.m_RD.texture.read().name
                except:
                    tex_name = '?'
                sprites_found[name] = {
                    'x': r.x, 'y': r.y, 'w': r.width, 'h': r.height,
                    'atlas': tex_name
                }
        except Exception as e:
            pass

print("=== HUD SPRITES WITH UV RECTS ===")
for k, v in sorted(sprites_found.items()):
    x, y, w, h = int(v['x']), int(v['y']), int(v['w']), int(v['h'])
    print(f"{k}: ({x},{y},{w},{h}) in '{v['atlas']}'")

print(f"\nTotal: {len(sprites_found)} sprites")
