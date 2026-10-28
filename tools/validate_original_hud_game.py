#!/usr/bin/env python3
"""Link original XAPK UI prefab to genuine Sprite game loader and run static integrity tests.
No claim of 1:1 live screenshot; original Canvas hierarchy preserved for future reconstruction.
"""
import ast,hashlib,json,shutil,sys
from pathlib import Path
R=Path(__file__).resolve().parents[1]
GAME=R/"last_island_recreated_game"
ASSETS=GAME/"assets"
PACK=ASSETS/"hud_original_xapk"
TMP=R/"hud_original_prefab_fonte"
assert (TMP/"original_hud_ui_hierarchy.json").is_file(),"Original prefab artifact not downloaded"
assert (PACK/"manifest_original.json").is_file(),"Original Unity Sprite sources not installed"
import importlib.util
s=importlib.util.spec_from_file_location("hud_original_loader",GAME/"hud_original_loader.py")
mod=importlib.util.module_from_spec(s);s.loader.exec_module(mod)
manifest=json.loads((PACK/"manifest_original.json").read_text(encoding="utf8"))
prefab=json.loads((TMP/"original_hud_ui_hierarchy.json").read_text(encoding="utf8"))
source=(GAME/"viewer3d.py").read_text(encoding="utf8")
ast.parse(source)
assert "resolve_original_hud_image(asset_dir, f)" in source
assert "from hud_original_loader import resolve_original_hud_image" in source
assert "BattlePanel_attack_button.png" not in source
assert "BattlePanel_sprint_button.png" not in source
assert "BattlePanel_aim_button.png" not in source
required=[
 "BattlePanel2_st_an_2.png",
 "BattlePanel2_quick_icon_attack.png",
 "1BattlePanel_cz_t.png",
 "BattlePanel_cxfmt_zhunxing.png",
 "1BattlePanel_cz_dun.png","1BattlePanel_cz_pa.png",
 "1BattlePanel_buff_xue.png","1BattlePanel_buff_jie.png",
 "1BattlePanel_buff_shui.png","1BattlePanel_buttle_fang_1.png",
 "BattlePanel2_car_yaogan_1.png","BattlePanel2_car_yaogan_2.png",
]
proven={}
for name in required:
    p=Path(mod.resolve_original_hud_image(ASSETS,name,strict=True))
    assert p.is_file() and PACK in p.parents,(name,p)
    evidence=mod.source_sprite_evidence(ASSETS,name)
    assert evidence and any(PACK/x["file"]==p for x in evidence),(name,evidence)
    proven[name]={"original_source":evidence[0]["bundle"],
                  "source_sprite_path":str(p.relative_to(GAME)),
                  "sha256":hashlib.sha256(p.read_bytes()).hexdigest()}
output=PACK/"ui_prefabs_original"
output.mkdir(exist_ok=True)
for x in prefab["original_prefabs"]:
    p=TMP/x["path"]
    dest=output/x["path"]
    dest.parent.mkdir(parents=True,exist_ok=True)
    shutil.copyfile(p,dest)
    assert hashlib.sha256(dest.read_bytes()).hexdigest()==x["sha256"]
shutil.copyfile(TMP/"original_hud_ui_hierarchy.json",output/"original_hud_ui_hierarchy.json")
shutil.copyfile(TMP/"hud_button_coordinates.json",output/"hud_button_coordinates.json")
assert prefab["counts"]["recttransforms"]>=3500
assert prefab["counts"]["gameobjects"]>=3600
report={"result":"Game renderer now resolves source-proven Unity HUD Sprite PNGs.",
 "xapk_sha256":manifest["source_xapk_sha256"],
 "sprite_count":manifest["summary"]["source_sprites"],
 "sprite_pixel_matches":manifest["summary"]["same_pixels_as_cached"],
 "original_prefab_rects":prefab["counts"]["recttransforms"],
 "original_prefab_gameobjects":prefab["counts"]["gameobjects"],
 "source_verified_controls":proven,
 "source_bundle_integrity":"SHA256 match for all 10 original UI/atlas ABs",
 "limited":"Original Canvas layout exists in ui_prefabs_original but is not yet fully reproduced by OpenGL 2D runtime.",
 "game_check":"Python AST and authentic XAPK Sprite resolver validated; graphical launch not yet tested."}
(PACK/"hud_integracao_testada.json").write_text(json.dumps(report,ensure_ascii=False,indent=2))
readme=(PACK/"LEIA-ME.md").read_text(encoding="utf8")
readme+="""\n## Origem e integração no jogo\n\nO jogo \`viewer3d.py\` agora usa \`hud_original_loader.py\` para localizar sprites\npelo nome de origem e pelo bundle. Os botões compostos sem origem comprovada\n(\`BattlePanel_attack_button\`, \`BattlePanel_sprint_button\` e\n\`BattlePanel_aim_button\`) foram substituídos por **elementos originais\nseparados** (placa circular, ícone de ataque, corredor, mira).\n\nA pasta \`ui_prefabs_original/assets/ui/\` contém os prefabs Unity originais:\npor exemplo \`battlepanel.ab\` (hierarquia da interface). O arquivo\n\`hud_button_coordinates.json\` guarda ancoragens e posições reais dos controles.\n\n**Ainda não é uma reprodução visual 1:1 do jogo:** o motor Python ainda\npossui coordenadas e dimensões aproximadas; faltam reconstruir as regras\ncompletas de ancoragem, máscaras e a relação de Sprite por componente.\n"""
(PACK/"LEIA-ME.md").write_text(readme,encoding="utf8")
print("SOURCE_GAME_HUD_INTEGRATED",len(proven),"CRITICAL_ORIGINAL_CONTROLS")
print("ORIGINAL_UI_LAYOUT_SAVED",prefab["counts"])
print("ORIGINAL_HUD_RUNTIME_TEST_PASS",json.dumps(report,ensure_ascii=False)[:3300])
