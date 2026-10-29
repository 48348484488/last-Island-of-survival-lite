#!/usr/bin/env python3
"""EdgeAI: decode LITE 7.3 Unity cfg.DrawingCfg.oc / cfg.ItemCfg.oc.

Serialization source: exported cfg.DrawingCfg, cfg.DrawingNeedMaterial,
cfg.ItemCfg and Share.Octets. All integers are little-endian signed 32-bit,
booleans are one byte, strings have a 32-bit byte-length UTF-8 prefix.
Rejects corrupt or incompatible versions instead of guessing recipes.
"""
from __future__ import annotations
import argparse
import collections
import hashlib
import json
from pathlib import Path

from edgeai_unity_buildpart_decode import Octets, read_source

DRAWING_SOURCE = "assets/cfg/cfg.DrawingCfg.oc"
ITEM_SOURCE = "assets/cfg/cfg.ItemCfg.oc"
MAX_ROWS = 100000
TAB_NAMES = {
    1: "basic", 2: "tools", 3: "materials", 4: "structures",
    5: "cooking", 6: "medical", 7: "equipment", 8: "weapons",
    9: "ammunition", 10: "traps", 12: "items", 13: "combat",
    14: "vehicles", 15: "transport", 16: "electrical"
}

def _read_drawing_record(reader, variant):
    """The 7.3 XAPK has a different prefix from the older C#.
    Version is established against actual source bytes, not guessed fields:
    id,name,bigType,smallType,10 bool bytes,6 ints,materialCount,pairs,
    targetItemId,onceMaxCreate,order,baseNumber,productNum.
    The unmapped flags/ints are preserved as raw evidence, not interpreted.
    """
    start = reader.off
    row = {"id": reader.integer(), "name": reader.string()}
    if variant == "lite73":
        row["bigType"] = reader.integer()
        row["smallType"] = reader.integer()
        row["raw_flags"] = [reader.boolean() for _ in range(10)]
        row["raw_numbers"] = [reader.integer() for _ in range(6)]
        row["isNormal"] = row["raw_flags"][0]
        row["isShowLocked"] = row["raw_flags"][1]
        row["isDefault"] = row["raw_flags"][2]
        row["levelLimit"] = row["raw_numbers"][0]
        row["needWorkbench"] = None  # Prefix differs; no unproven guess.
        row["needWorkbenchLevel"] = None
        row["needTime"] = row["raw_numbers"][-1]
    else:
        for key in ("noUseId", "typeShow", "bigType", "smallType"):
            row[key] = reader.integer()
        for key in ("isNormal", "isShowLocked", "isDefault"):
            row[key] = reader.boolean()
        for key in ("levelLimit", "needWorkbench",
                    "needWorkbenchLevel", "needTime"):
            row[key] = reader.integer()
    count = reader.integer()
    if count < 0 or count > 64:
        raise ValueError("Invalid DrawingCfg material count at "+str(start))
    row["materials"] = []
    for _ in range(count):
        row["materials"].append({
            "itemId": reader.integer(), "num": reader.integer()})
    for key in ("targetItemId", "onceMaxCreate", "order",
                "baseNumber", "productNum"):
        row[key] = reader.integer()
    row["byte_span"] = [start, reader.off]
    row["record_variant"] = variant
    if (row["id"] <= 0 or
        row["smallType"] < 0 or row["smallType"] > 64 or
        row["bigType"] < 0 or row["bigType"] > 64 or
        row["needTime"] < 0 or row["needTime"] > 60*60*24*365 or
        row["levelLimit"] < 0 or row["levelLimit"] > 100000 or
        not (0 <= row["onceMaxCreate"] <= 1000000) or
        row["targetItemId"] <= 0 or
        any(m["itemId"] <= 0 or not (0 <= m["num"] <= 10000000)
            for m in row["materials"])):
        raise ValueError("Invalid DrawingCfg record at "+str(start)+
                         " id="+str(row["id"]))
    return row

def parse_drawing(payload):
    reader = Octets(payload)
    rows = []
    seen = set()
    while reader.off < len(reader.data):
        # Prefer exact confirmed legacy when valid, otherwise the source
        # 7.3 record prefix. No scanning for a lucky-looking next header.
        start = reader.off
        failures = []
        row = None
        for variant in ("legacy_csharp", "lite73"):
            reader.off = start
            try:
                candidate = _read_drawing_record(reader, variant)
                if candidate["id"] in seen:
                    raise ValueError("Duplicate original cfg recipe id")
                row = candidate
                break
            except (UnicodeError, ValueError) as exc:
                failures.append(str(exc))
        if row is None:
            raise ValueError("No verified DrawingCfg 7.3 record at "+str(start)+
                             " errors="+str(failures))
        seen.add(row["id"])
        rows.append(row)
        if len(rows) > MAX_ROWS:
            raise ValueError("Unbounded original DrawingCfg rows")
    if reader.off != len(payload) or not rows:
        raise ValueError("Incomplete DrawingCfg decode")
    return rows

def parse_items(payload):
    reader = Octets(payload)
    rows = []
    seen = set()
    while reader.off < len(reader.data):
        start = reader.off
        row = {"id": reader.integer(), "name": reader.string()}
        for k in ("type","childType","quality","itemLevel"):
            row[k] = reader.integer()
        row["isCanDiscard"] = reader.boolean()
        for k in ("durability","durabilityDownValue"):
            row[k] = reader.integer()
        row["toQuickUse"] = reader.boolean()
        row["canUse"] = reader.boolean()
        row["icon"] = reader.string()
        row["desc"] = reader.string()
        for k in ("sellType","price","itemId"):
            row[k] = reader.integer()
        row["isPileAble"] = reader.boolean()
        for k in ("dropId","level"):
            row[k] = reader.integer()
        for k in ("modelPath","womanModelPath","root"):
            row[k] = reader.string()
        for k in ("maxPileNum","addCapacityValue"):
            row[k] = reader.integer()
        row["dropModel"] = reader.string()
        row["useTime"] = reader.pop("f")
        n = reader.integer()
        if not 0 <= n <= 64: raise ValueError("Bad extras at "+str(start))
        row["extras"] = [reader.pop("f") for _ in range(n)]
        n = reader.integer()
        if not 0 <= n <= 64: raise ValueError("Bad extra strings at "+str(start))
        row["extrasstring"] = [reader.string() for _ in range(n)]
        row["SexLimit"] = reader.integer()
        row["byte_span"] = [start, reader.off]
        if row["id"] <= 0 or row["id"] in seen:
            raise ValueError("Bad/duplicate ItemCfg.id at "+str(start))
        seen.add(row["id"])
        rows.append(row)
        if len(rows) > MAX_ROWS: raise ValueError("Unbounded ItemCfg rows")
    if not rows or reader.off != len(payload): raise ValueError("Incomplete ItemCfg decode")
    return rows

def report_from_bytes(draw_bytes, item_bytes=None):
    drawings = parse_drawing(draw_bytes)
    items = parse_items(item_bytes) if item_bytes is not None else []
    byitem = {x["id"]: x for x in items}
    for row in drawings:
        item = byitem.get(row["targetItemId"])
        row["target"] = ({
            "id": item["id"], "name": item["name"], "icon": item["icon"]}
            if item else None)
        for material in row["materials"]:
            rec = byitem.get(material["itemId"])
            material["name"] = rec["name"] if rec else None
            material["icon"] = rec["icon"] if rec else None
    groups = collections.defaultdict(list)
    for row in drawings:
        groups[str(row["smallType"])].append(row)
    for group in groups.values():
        # SC.UI.BagProducePage SortDrawingList is runtime lock state,
        # then the original DrawingCfg.order. Preserve this source order
        # to avoid inventing unlock state.
        group.sort(key=lambda x: (x["order"], x["byte_span"][0]))
    return {
        "schema": "edgeai.unity-craft-config.v1",
        "verified": True,
        "source": DRAWING_SOURCE,
        "sha256": hashlib.sha256(draw_bytes).hexdigest(),
        "bytes": len(draw_bytes),
        "record_count": len(drawings),
        "item_source": ITEM_SOURCE if item_bytes is not None else None,
        "item_sha256": hashlib.sha256(item_bytes).hexdigest() if item_bytes is not None else None,
        "item_count": len(items),
        "item_linked": sum(x["target"] is not None for x in drawings),
        "groups": {key: rows for key, rows in sorted(groups.items(), key=lambda p:int(p[0]))},
        "all_records": drawings,
        "notes": [
            "Source configuration values, not claims about native game support",
            "Runtime sorting additionally considers actual player unlock state",
            "Icons require separate verification against Unity texture bundles",
            "Decoded materials and times must not overwrite test-only offline economy without a matching item implementation"
        ]
    }

def main():
    ap = argparse.ArgumentParser(description=__doc__)
    src = ap.add_mutually_exclusive_group(required=True)
    src.add_argument("--xapk-parts", type=Path)
    src.add_argument("--drawing-binary", type=Path)
    ap.add_argument("--item-binary", type=Path)
    ap.add_argument("--output", type=Path, required=True)
    args = ap.parse_args()
    if args.xapk_parts:
        draw = read_source(args.xapk_parts, DRAWING_SOURCE)
        items = read_source(args.xapk_parts, ITEM_SOURCE)
    else:
        draw = args.drawing_binary.read_bytes()
        items = args.item_binary.read_bytes() if args.item_binary else None
    # Hex source prefix is needed to diagnose field-layout drift from the
    # older decompiled C# (as already observed for BuildPart.oc).
    print("EDGEAI_DRAWING_SOURCE_PREFIX",len(draw),draw[:128].hex(),flush=True)
    print("EDGEAI_ITEM_SOURCE_PREFIX",len(items) if items else 0,
          items[:64].hex() if items else "",flush=True)
    result = report_from_bytes(draw, items)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, ensure_ascii=False)+"\n")
    print("EDGEAI_SOURCE_CRAFT_CONFIG_PASS",
          "recipes", result["record_count"], "items", result["item_count"],
          "linked", result["item_linked"],
          "groups", [(k,len(v)) for k,v in result["groups"].items()], flush=True)

if __name__ == "__main__":
    main()
