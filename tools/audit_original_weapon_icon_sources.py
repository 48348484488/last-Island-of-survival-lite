#!/usr/bin/env python3
"""Inventory game-origin icon evidence WITHOUT guessing names from a screenshot.

Original code:
  cfg.ItemCfg.icon => sprite name
  SC.UI.GShortcutBarItem.m_shortcut_bar_icon => runtime sprite
An empty original prefab Image.m_Sprite is NORMAL for equipped items. This
audits config archives from the supplied XAPK and only records proven matches.
"""
import hashlib
import json
import os
import re
import shutil
import struct
import tempfile
import zipfile
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"last_island_recreated_game/assets/hud_original_xapk/complete_source_mapping"
PARTS=sorted((ROOT/"xapk_original").glob("Last_Island_LITE_7.3.xapk.part*"))
assert len(PARTS)==17,len(PARTS)
OUT.mkdir(parents=True,exist_ok=True)

def interesting(path):
    name=path.lower()
    return ("itemcfg" in name or "icon/" in name or "tubiao" in name
            or "cfg/" in name or name.endswith(".oc") or "weapon" in name
            or "shifu" in name or "m4a1" in name)

class Reader:
    """Field decoding mirrors Share/Octets.cs, little endian int32/float32."""
    def __init__(self,data):
        self.data=data
        self.pos=0
    def raw(self,n):
        if n<0 or n>len(self.data)-self.pos:
            raise ValueError("Octets overrun")
        b=self.data[self.pos:self.pos+n]
        self.pos+=n
        return b
    def integer(self):
        return struct.unpack("<i",self.raw(4))[0]
    def floating(self):
        return struct.unpack("<f",self.raw(4))[0]
    def boolean(self):
        return self.raw(1)==b"\\x01"
    def string(self):
        n=self.integer()
        if n<0 or n>1048576:
            raise ValueError("Invalid Octets string length")
        return self.raw(n).decode("utf-8",errors="strict")

def parse_itemcfg(data):
    r=Reader(data)
    result=[]
    while r.pos<len(data):
        begin=r.pos
        row={"id":r.integer(),"name":r.string()}
        for name in ("type","childType","quality","itemLevel"):
            r.integer()
        r.boolean()
        r.integer();r.integer()
        r.boolean();r.boolean()
        row["icon"]=r.string()
        r.string()
        for i in range(11):
            r.integer()  # sellType..dropModel? validated below if suspicious
        # Layout from cfg/ItemCfg.cs is:
        # sellType, price, itemId, isPileAble (boolean), dropId,
        # level, modelPath, womanModelPath, root, maxPileNum,
        # addCapacityValue, dropModel, useTime, extras[], extrasstring[], SexLimit.
        # The naive int-only walk above is deliberately rejected. Parse again
        # with the exact typed layout to avoid false-positive icon identities.
        raise AssertionError("internal parser layout guard")
    return result

def parse_itemcfg_exact(data):
    r=Reader(data)
    result=[]
    while r.pos<len(data):
        begin=r.pos
        d={"id":r.integer(),"name":r.string()}
        for _ in range(4):r.integer()
        r.boolean()
        r.integer();r.integer()
        r.boolean();r.boolean()
        d["icon"]=r.string()
        r.string()
        r.integer();r.integer();r.integer()
        r.boolean()
        r.integer();r.integer()
        d["modelPath"]=r.string()
        d["womanModelPath"]=r.string()
        d["root"]=r.string()
        r.integer();r.integer()
        d["dropModel"]=r.string()
        r.floating()
        fc=r.integer()
        if fc<0 or fc>10000:raise ValueError("invalid float array length")
        for _ in range(fc):r.floating()
        sc=r.integer()
        if sc<0 or sc>10000:raise ValueError("invalid string array length")
        for _ in range(sc):r.string()
        r.integer()
        if not (0<d["id"]<100000000 and len(d["icon"])<=200):
            raise ValueError("invalid ItemCfg item identity")
        result.append(d)
        if len(result)>200000:raise ValueError("too many ItemCfg records")
        if r.pos<=begin:raise ValueError("zero-length record")
    return result

def sha(data):return hashlib.sha256(data).hexdigest()

def main():
    report={"source":"XAPK LITE 7.3 from repository xapk_original parts",
            "source_code_reference":"cfg.ItemCfg.icon and SC.UI.GShortcutBarItem",
            "xapk_entries":0,"nested_apks":[],"config_files":[],"other_asset_candidates":[],
            "matches":[],"extraction_warnings":[],"original_icon_proven":False}
    with tempfile.TemporaryDirectory(prefix="lios_icon_") as td:
        merged=Path(td)/"original.xapk"
        with merged.open("wb") as o:
            for part in PARTS:
                with part.open("rb") as f:
                    shutil.copyfileobj(f,o,length=8*1024*1024)
        print("XAPK_REBUILT",merged.stat().st_size,flush=True)
        with zipfile.ZipFile(merged) as outer:
            entries=outer.infolist()
            report["xapk_entries"]=len(entries)
            report["xapk_candidate_paths"]=[i.filename for i in entries if interesting(i.filename)][:200]
            apk_members=[i for i in entries if i.filename.lower().endswith(".apk")]
            print("NESTED_APK_LIST",[(i.filename,i.file_size) for i in apk_members],flush=True)
            for info in apk_members:
                local=Path(td)/("nested_"+Path(info.filename).name)
                try:
                    with outer.open(info) as fi,local.open("wb") as fo:
                        shutil.copyfileobj(fi,fo,length=4*1024*1024)
                    with zipfile.ZipFile(local) as nested:
                        candidates=[i for i in nested.infolist() if interesting(i.filename)]
                        row={"apk":info.filename,"size":info.file_size,
                             "entries":len(nested.infolist()),"candidates":len(candidates),
                             "candidate_paths":[x.filename for x in candidates[:100]]}
                        report["nested_apks"].append(row)
                        print("APK_AUDIT",info.filename, "files",row["entries"],
                            "candidates",len(candidates),flush=True)
                        for item in candidates:
                            label=item.filename.lower()
                            if not (label.endswith(".oc") or "itemcfg" in label):continue
                            itemmeta={"apk":info.filename,"path":item.filename,"bytes":item.file_size}
                            report["config_files"].append(itemmeta)
                            if item.file_size>12*1024*1024:continue
                            data=nested.read(item)
                            if item.filename.endswith("/cfg.ItemCfg.oc"):
                                itemmeta["header_hex"]=data[:64].hex()
                                itemmeta["tail_hex"]=data[-32:].hex()
                                itemmeta["sha256"]=sha(data)
                                (OUT/"original_itemcfg_source.oc").write_bytes(data)
                                print("ORIGINAL_ITEMCFG_BINARY_HEADER",
                                      itemmeta["header_hex"],flush=True)
                            try:
                                decoded=parse_itemcfg_exact(data)
                                itemmeta["parse"]="ItemCfg Octets success"
                                itemmeta["records"]=len(decoded)
                                itemmeta["sha256"]=sha(data)
                                # Report names and IDs, but do not invent equivalences.
                                report["matches"].extend([
                                    {**entry,"cfg_source":item.filename}
                                    for entry in decoded
                                    if ("shifu" in (entry["icon"]+" "+entry["modelPath"]).lower()
                                        or any(w in (entry["icon"]+" "+entry["modelPath"]).lower()
                                               for w in ("wq_m4","m4a1","ak47","axe")))
                                ])
                                print("ITEMCFG_PARSED",item.filename,len(decoded),flush=True)
                            except Exception as e:
                                itemmeta["parse_error"]=str(e)[:160]
                except Exception as e:
                    report["extraction_warnings"].append({ "apk":info.filename,"error":str(e)[:200]})
                finally:local.unlink(missing_ok=True)
    # Source report distinguishes icon identity from a verified rendered Sprite.
    report["original_icon_proven"]=False
    output=OUT/"original_weapon_icon_config_audit.json"
    output.write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding="utf8")
    print("ORIGINAL_ICON_CFG_AUDIT_RESULT",json.dumps({
        "apks":len(report["nested_apks"]),
        "configs":len(report["config_files"]),
        "matches":len(report["matches"]),
        "sample_matches":report["matches"][:20]},ensure_ascii=False),flush=True)

if __name__=="__main__":
    main()
