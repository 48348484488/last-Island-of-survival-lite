import json
from pathlib import Path
p=Path('last_island_recreated_game/assets/hud_original_xapk/complete_source_mapping')
a=json.loads((p/'hud_original_all_positions.json').read_text())
match=lambda s:any(t in s.lower() for t in ('build','jianzao','part','rotate','freebuild','cancel','model'))
for key in ('nodes','clickable'):
 x=[v for v in a[key] if match(str(v.get('name','')))]
 print('BUILD_UI_AUDIT',key,json.dumps(x[:90],ensure_ascii=False),flush=True)

for item in a['nodes']:
 if item.get('parent_name') in ('m_buildPartTypes','m_buildChilds','m_buildBaseBtns','m_buildoperatesbtns','scp_bagBuilds','btn_freeBuildModel','btn_build'):
  print('BUILD_UI_CHILD',item.get('parent_name'),item['name'],item.get('pixel_bounds_top_left'),item.get('components',[])[:2],flush=True)
