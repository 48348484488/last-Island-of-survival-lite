#!/usr/bin/env python3
"""Locate BuildPart.oc and related Unity source config tables, no guessing.

Inspects original 17-part XAPK, both base.apk and install-time pack. Saves
a bounded manifest; does not extract or execute original game code.
"""
from pathlib import Path
import json,re,shutil,tempfile,zipfile,collections
root=Path(__file__).resolve().parents[1]
parts=sorted((root/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(parts)==17, "Original source XAPK parts missing"
out={"source":"LITE 7.3 original 17-part XAPK",
     "archives":[],"possible_config_paths":[],"source_table_found":False}
with tempfile.TemporaryDirectory() as td:
 full=Path(td)/"original.xapk"
 with full.open("wb") as w:
  for part in parts:
   with part.open("rb") as f:shutil.copyfileobj(f,w,8*1024*1024)
 with zipfile.ZipFile(full) as z:
  for name in ["com.herogame.gplay.survival.lite.apk","install_time_asset_pack.apk"]:
   path=Path(td)/Path(name).name
   with z.open(name) as f,path.open("wb") as w:shutil.copyfileobj(f,w,8*1024*1024)
   with zipfile.ZipFile(path) as inner:
    entries=inner.infolist()
    folder=collections.Counter(p.filename.split("/")[0] for p in entries)
    candidates=[x for x in entries if re.search(
      r"(^|/)(?:cfg|config|table|configdata|streamingassets|assetpacks)(/|$)|"
      r"buildpart|\.oc$|\.bytes$|\.csv$|\.json$|\.db$|\.sqlite$|\.dat$",
      x.filename,re.I)]
    if name=="com.herogame.gplay.survival.lite.apk":
     data=inner.read("assets/cfg/cfg.BuildPart.oc")
     if len(data)<10000 or len(data)>2_000_000:
      raise ValueError("Unexpected source BuildPart bytes")
     Path("cfg.BuildPart.oc").write_bytes(data)
     print("EDGEAI_BUILDPART_BINARY_EXTRACTED",len(data),
           data[:48].hex(),flush=True)
    report={"archive":name,"entry_count":len(entries),
            "top_folders":folder.most_common(30),
            "candidate_count":len(candidates),
            "candidates":[{"path":x.filename,"size":x.file_size} for x in candidates[:600]]}
    out["archives"].append(report)
    print("EDGEAI_UNITY_ORIGINAL_TABLE_SCAN",name,
          "candidates",len(candidates),flush=True)
    for entry in candidates:
     p=entry.filename
     if "buildpart" in p.lower() or p.lower().endswith("/cfg.buildpart.oc"):
      out["possible_config_paths"].append({"archive":name,"path":p,
                                           "size":entry.file_size})
      out["source_table_found"]=True
      print("EDGEAI_BUILDPART_CFG_CANDIDATE",name,p,entry.file_size,flush=True)
Path("original_buildpart_cfg_locator.json").write_text(json.dumps(out,indent=2,ensure_ascii=False))
print("EDGEAI_BUILDPART_TABLE_PATH_FOUND",out["source_table_found"],
      "archive_count",len(out["archives"]),flush=True)
