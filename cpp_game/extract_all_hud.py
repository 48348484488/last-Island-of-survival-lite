"""
Full APK asset extractor for Last Day Rules: Survival
Extracts ALL HUD/Battle panel sprites directly from the original APK.
"""
import zipfile, io, os, UnityPy

apk_path = r'C:/Users/Admin/.gemini/antigravity/scratch/last-day-rules-survival/xapk_content/install_time_asset_pack.apk'
out_dir = r'C:/Users/Admin/.gemini/antigravity/scratch/last-day-rules-survival/cpp_game/assets/ui_hud'
os.makedirs(out_dir, exist_ok=True)

TARGET_BUNDLES = [
    'assets/uiatlas/battlepanel.ab',
    'assets/uiatlas/battlepanel2.ab',
    'assets/uiatlas/common.ab',
    'assets/uiatlas/newbattlepanel.ab',
    'assets/texture/battlepanel_kedu_yw.ab',
    'assets/icon/buff.ab',
    'assets/icon/buffnew.ab',
    'assets/uiatlas/deathpanel.ab',
]

extracted = 0
skipped = 0

with zipfile.ZipFile(apk_path, 'r') as z:
    for ab_name in TARGET_BUNDLES:
        try:
            data = z.read(ab_name)
            env = UnityPy.load(io.BytesIO(data))
            for obj in env.objects:
                if obj.type.name in ['Texture2D', 'Sprite']:
                    d = obj.read()
                    name = getattr(d, 'm_Name', getattr(d, 'name', None))
                    if not name:
                        continue
                    try:
                        img = d.image
                        out_path = os.path.join(out_dir, name + '.png')
                        img.save(out_path)
                        extracted += 1
                        print(f'[OK] {name}.png  {img.size}')
                    except Exception as e:
                        skipped += 1
                        print(f'[SKIP] {name}: {e}')
        except Exception as e:
            print(f'[ERR] {ab_name}: {e}')

print(f'\nTotal extracted: {extracted}  skipped: {skipped}')
