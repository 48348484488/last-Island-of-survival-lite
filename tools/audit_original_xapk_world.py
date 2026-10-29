#!/usr/bin/env python3
"""Audit REAL original LITE 7.3 world assets; never confuse Unity scripts with map.
Streams 17 original XAPK shards to temp ZIP, lists nested APK assets and searches
for sceneinfo/*.oc, scene terrain .ab/.unity, map textures and world meshes.
Does not publish proprietary original package bytes.
"""
from collections import Counter
from pathlib import Path
import json
import re
import shutil
import tempfile
import zipfile

ROOT=Path(__file__).resolve().parents[1]
SOURCE=ROOT/"xapk_original"
TERM=re.compile(r"sceneinfo|sceneprefab|terrain|heightmap|\\bmditu\\b|\\bscene\\b|map_|minimap|world|island|\\.unity$|\\.oc$|\\.ab$|\\.bundle$",re.I)
MAPPATH=re.compile(r"sceneinfo/|sceneprefab/|/scene/|/terrain/|mditu|heightmap",re.I)
def inspect(z,name):
    files=z.infolist()
    hits=[{"path":i.filename,"bytes":i.file_size} for i in files if TERM.search(i.filename)]
    candidates=[x for x in hits if MAPPATH.search(x["path"])]
    folder=Counter(i.filename.split("/",1)[0] for i in files)
    extensions=Counter(Path(i.filename).suffix.lower() for i in files)
    return {"archive":name,"files":len(files),"total_uncompressed_bytes":sum(i.file_size for i in files),
            "top_folders":folder.most_common(28),"extensions":extensions.most_common(28),
            "scene_asset_candidates":candidates[:2500],"scene_asset_count":len(candidates),
            "other_world_named":hits[:120],"other_world_count":len(hits)}
def main():
    parts=sorted(SOURCE.glob("Last_Island_LITE_7.3.xapk.part*"))
    assert len(parts)==17
    with tempfile.TemporaryDirectory() as tmp:
        joined=Path(tmp)/"original.xapk"
        with joined.open("wb") as out:
            for part in parts:
                with part.open("rb") as inp:
                    shutil.copyfileobj(inp,out,length=8*1024*1024)
        assert 1_500_000_000 < joined.stat().st_size < 1_600_000_000
        report={"xapk_bytes":joined.stat().st_size,"parts":len(parts),"archives":[]}
        with zipfile.ZipFile(joined) as outer:
            report["archives"].append(inspect(outer,"xapk"))
            for entry in outer.infolist():
                if not entry.filename.lower().endswith(".apk"):continue
                local=Path(tmp)/Path(entry.filename).name
                with outer.open(entry) as source,local.open("wb") as out:
                    shutil.copyfileobj(source,out,length=8*1024*1024)
                try:
                    with zipfile.ZipFile(local) as z:
                        report["archives"].append(inspect(z,entry.filename))
                finally:
                    local.unlink()
        Path("original_world_xapk_audit.json").write_text(
            json.dumps(report,indent=2,ensure_ascii=False),encoding="utf8")
        print("LIOS_ORIGINAL_WORLD_XAPK_INDEX",json.dumps([
            {"archive":a["archive"],"file_count":a["files"],
             "scene_asset_count":a["scene_asset_count"],
             "world_candidates":a["scene_asset_candidates"][:60],
             "top_folders":a["top_folders"][:12]} for a in report["archives"]
        ],ensure_ascii=False),flush=True)
if __name__=="__main__":main()
