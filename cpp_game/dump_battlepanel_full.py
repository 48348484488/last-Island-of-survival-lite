"""
Full BattlePanel.prefab hierarchy dumper.
Extracts EXACT positions, sizes, sprites, colors and anchors for every UI element.
"""
import zipfile, io, UnityPy
from UnityPy.classes import PPtr

apk_path = r'C:/Users/Admin/.gemini/antigravity/scratch/last-day-rules-survival/xapk_content/install_time_asset_pack.apk'

def get_sprite_name(pptr, atlas_env):
    """Resolve a PPtr sprite ref across environments."""
    if pptr is None:
        return 'None'
    try:
        pid = getattr(pptr, 'path_id', 0)
        if pid == 0:
            return 'None'
        # try main env
        obj = atlas_env.files.get(pid) or atlas_env._objects.get(pid)
        if obj:
            d = obj.read()
            return getattr(d, 'm_Name', getattr(d, 'name', str(pid)))
        # scan all objects
        for o in atlas_env.objects:
            if o.path_id == pid:
                d = o.read()
                return getattr(d, 'm_Name', getattr(d, 'name', str(pid)))
        return f'pid_{pid}'
    except Exception as e:
        return f'err_{e}'

lines = []

def dump_go(go, tr, indent, atlas_env):
    name = getattr(go, 'm_Name', '?')
    lp  = getattr(tr, 'm_AnchoredPosition', getattr(tr, 'm_LocalPosition', None))
    sd  = getattr(tr, 'm_SizeDelta', None)
    amin = getattr(tr, 'm_AnchorMin', None)
    amax = getattr(tr, 'm_AnchorMax', None)
    pivot = getattr(tr, 'm_Pivot', None)

    comps = []
    for c in getattr(go, 'm_Components', []):
        if c.type.name == 'MonoBehaviour':
            try:
                mb = c.read()
                if hasattr(mb, 'm_Sprite'):
                    sn = get_sprite_name(getattr(mb, 'm_Sprite', None), atlas_env)
                    col = getattr(mb, 'm_Color', None)
                    fill_m = getattr(mb, 'm_FillMethod', None)
                    comps.append(f'Image(sprite={sn}, color={col}, fill_method={fill_m})')
                if hasattr(mb, 'm_Text'):
                    comps.append(f'Text("{getattr(mb, "m_Text","")}", size={getattr(mb,"m_FontSize","")})')
            except:
                pass

    line = '  '*indent + f'[{name}] pos={lp} size={sd} anchorMin={amin} anchorMax={amax} pivot={pivot} comps=[{", ".join(comps)}]'
    lines.append(line)

    for ch in getattr(tr, 'm_Children', []):
        try:
            ch_tr = ch.read()
            ch_go = ch_tr.m_GameObject.read()
            dump_go(ch_go, ch_tr, indent+1, atlas_env)
        except:
            pass

with zipfile.ZipFile(apk_path, 'r') as z:
    # Load atlas for sprite resolution
    atlas_data = z.read('assets/uiatlas/battlepanel.ab')
    atlas_env = UnityPy.load(io.BytesIO(atlas_data))

    # Also load battlepanel2 and common to cover all sprite refs
    for extra in ['assets/uiatlas/battlepanel2.ab', 'assets/uiatlas/common.ab']:
        extra_data = z.read(extra)
        extra_env = UnityPy.load(io.BytesIO(extra_data))
        # Merge objects into atlas_env (just iterate separately)

    data = z.read('assets/ui/battlepanel.ab')
    env = UnityPy.load(io.BytesIO(data))

    for obj in env.objects:
        if obj.type.name == 'GameObject':
            go = obj.read()
            if go.m_Name == 'BattlePanel':
                tr = go.m_Components[0].read()
                dump_go(go, tr, 0, atlas_env)
                break

with open(r'C:/Users/Admin/.gemini/antigravity/scratch/last-day-rules-survival/cpp_game/battlepanel_full_dump.txt', 'w', encoding='utf-8') as f:
    f.write('\n'.join(lines))

print(f'Dumped {len(lines)} lines to battlepanel_full_dump.txt')
