#!/usr/bin/env python3
"""Export Sprite.m_Border and image fill metadata from ORIGINAL Unity UI bundles.

The source .ab files were copied byte-for-byte from the original 7.3 XAPK.
Only original Unity serialized properties are recorded; no guessed borders.
"""
import json,hashlib
from pathlib import Path
import UnityPy
R=Path(__file__).resolve().parents[1]
assets=R/"last_island_recreated_game/assets/hud_original_xapk"
manifest=json.loads((assets/"manifest_original.json").read_text(encoding="utf8"))
lookup={(s["bundle"],s["path_id"]):s for s in manifest["sprites"]}
abroot=assets/"ui_prefabs_original/assets/uiatlas"
out={}; source={}
for filename in ("battlepanel.ab","battlepanel2.ab"):
    ab=abroot/filename
    assert ab.is_file(),("missing source Unity atlas",ab)
    asset=UnityPy.load(str(ab))
    source[filename]={"sha256":hashlib.sha256(ab.read_bytes()).hexdigest(),"sprites":0}
    for obj in asset.objects:
        if obj.type.name!="Sprite":continue
        row=lookup.get((filename,str(obj.path_id)))
        if row is None:continue
        try:
            data=obj.read_typetree()
        except Exception:
            data={}
        b=data.get("m_Border")
        if not isinstance(b,dict):
            sprite=obj.read()
            bb=getattr(sprite,"m_Border",None)
            if bb is not None:
                b={k:float(getattr(bb,k,0)) for k in ("x","y","z","w")}
        if isinstance(b,dict):
            b={k:float(b.get(k,0)) for k in ("x","y","z","w")}
            out[row["file"]]={"border":b,
                "original_bundle":filename,"unity_path_id":str(obj.path_id),
                "source_sha256":source[filename]["sha256"]}
            source[filename]["sprites"]+=1
assert source["battlepanel.ab"]["sprites"]>70
assert source["battlepanel2.ab"]["sprites"]>30
target=assets/"hud_source_sprite_borders.json"
target.write_text(json.dumps(out,indent=2,ensure_ascii=False),encoding="utf8")
sliced=[(k,v["border"]) for k,v in out.items() if any(v["border"].values())]
print("ORIGINAL_UGUI_SPRITE_BORDERS",len(out),"NONZERO",len(sliced),
      "SAMPLE",sliced[:8],"BUNDLES",source,flush=True)
