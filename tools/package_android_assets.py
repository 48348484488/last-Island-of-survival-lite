#!/usr/bin/env python3
"""Package only the OBJ/PNG assets referenced by the native Raylib mobile engine.

The complete 1.5 GB source XAPK, decompiled C# scripts, desktop NumPy NPZ,
and >1000 original Unity HUD sprite files are deliberately not packaged in
this incremental native Android prototype.
"""
import json
import re
import shutil
import sys
from pathlib import Path

def main():
    if len(sys.argv)!=4:
        raise SystemExit("Usage: package_android_assets.py main.cpp asset_source_dir destination")
    source=Path(sys.argv[1])
    root=Path(sys.argv[2])
    dest=Path(sys.argv[3])
    program=source.read_text(encoding="utf8")
    names=sorted(set(re.findall(r'"assets/([^"\\]+\.(?:obj|png|wav|ogg))"',program)))
    if len(names)<20:
        raise ValueError("Native game references fewer assets than expected")
    dest.mkdir(parents=True,exist_ok=True)
    packaged=[]
    for name in names:
        rel=Path(name)
        if rel.is_absolute() or '..' in rel.parts or len(rel.parts)!=1:
            raise ValueError("Unexpected asset path "+name)
        original=root/rel
        if not original.is_file():
            raise FileNotFoundError(original)
        if original.stat().st_size==0:
            raise ValueError("Empty original game asset: "+name)
        new=dest/rel
        shutil.copyfile(original,new)
        packaged.append({"file":name,"bytes":original.stat().st_size})
    result={"native_game_source":str(source),
            "original_asset_count":len(packaged),
            "original_asset_bytes":sum(x["bytes"] for x in packaged),
            "files":packaged,
            "note":"C++ native prototype uses procedural animation; Python 23-clip skeletal system NOT ported"}
    print("LIOS_ANDROID_ASSETS_VALIDATED",json.dumps(result,ensure_ascii=False),flush=True)

if __name__=="__main__":main()
