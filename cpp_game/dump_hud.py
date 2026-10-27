import zipfile, io, UnityPy

apk_path = r'C:/Users/Admin/.gemini/antigravity/scratch/last-day-rules-survival/xapk_content/install_time_asset_pack.apk'
with zipfile.ZipFile(apk_path, 'r') as z:
    data = z.read('assets/ui/battlepanel.ab')
    env = UnityPy.load(io.BytesIO(data))
    
    # Load atlas as well so we can look up the path_id directly!
    atlas_data = z.read('assets/uiatlas/battlepanel.ab')
    atlas_env = UnityPy.load(io.BytesIO(atlas_data))
    atlas_sprites = {}
    for obj in atlas_env.objects:
        if obj.type.name == 'Sprite':
            try:
                s = obj.read()
                atlas_sprites[obj.path_id] = getattr(s, 'm_Name', getattr(s, 'name', 'unknown'))
            except:
                pass

    print('Loaded', len(atlas_sprites), 'atlas sprites')

    def dump_rec(tr, indent=0):
        go = tr.m_GameObject.read()
        lp = getattr(tr, 'm_LocalPosition', None)
        sd = getattr(tr, 'm_SizeDelta', None)
        for c in getattr(go, 'm_Components', []):
            if c.type.name == 'MonoBehaviour':
                mb = c.read()
                if hasattr(mb, 'm_Sprite'):
                    s = getattr(mb, 'm_Sprite', None)
                    pid = getattr(s, 'path_id', 0)
                    sn = atlas_sprites.get(pid, f'NOT_IN_ATLAS_pid_{pid}')
                    col = getattr(mb, 'm_Color', None)
                    print('  '*indent + f'[{go.m_Name}] sprite={sn} (pid={pid}) size={sd} col={col}')
        for ch in getattr(tr, 'm_Children', []):
            dump_rec(ch.read(), indent+1)

    for obj in env.objects:
        if obj.type.name == 'GameObject':
            go = obj.read()
            if getattr(go, 'm_Name', '') == 'xue_je_fs':
                dump_rec(go.m_Components[0].read(), 0)
