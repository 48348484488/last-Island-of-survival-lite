#!/usr/bin/env python3
"""Extract TRUE UnityEngine.UI.Text m_FontData from original LITE 7.3 battlepanel.ab.

Earlier JSON exporter queried top-level m_FontSize/m_Alignment, which do not
exist in UGUI Text. Source fields are m_FontData.m_FontSize, m_Alignment etc.
No invented fonts or artwork: these are the serialized source values.
"""
from pathlib import Path
from collections import Counter
import json
import UnityPy

R=Path(__file__).resolve().parents[1]
A=R/"last_island_recreated_game/assets/hud_original_xapk"
SOURCE=A/"ui_prefabs_original/assets/ui/battlepanel.ab"
GRAPH=A/"complete_source_mapping/original_unity_all_hud_components.json"
assert SOURCE.is_file() and GRAPH.is_file()
graph=json.loads(GRAPH.read_text(encoding="utf8"))
env=UnityPy.load(str(SOURCE))
fields=("m_FontSize","m_FontStyle","m_BestFit","m_MinSize","m_MaxSize",
 "m_Alignment","m_AlignByGeometry","m_RichText",
 "m_HorizontalOverflow","m_VerticalOverflow","m_LineSpacing")
def ptr(v):
    if not isinstance(v,dict):return {"file_id":0,"path_id":"0"}
    return {"file_id":int(v.get("m_FileID",0)),"path_id":str(v.get("m_PathID",0))}
entries={}
for obj in env.objects:
    if obj.type.name!="MonoBehaviour":continue
    try:d=obj.read_typetree()
    except Exception:continue
    if "m_Text" not in d:continue
    fd=d.get("m_FontData") or {}
    entry={"go_path_id":ptr(d.get("m_GameObject"))["path_id"],
           "source_text":d.get("m_Text"),
           "font":ptr(fd.get("m_Font")),
           "font_data":{k:fd.get(k) for k in fields},
           "color":d.get("m_Color")}
    entries[str(obj.path_id)]=entry
    existing=graph["components"].get(str(obj.path_id))
    assert existing is not None and existing["type"]=="Text",(obj.path_id,existing)
    existing.update({"font":entry["font"],"font_data":entry["font_data"],
     "font_size":fd.get("m_FontSize"),
     "font_style":fd.get("m_FontStyle"),
     "alignment":fd.get("m_Alignment"),
     "support_rich_text":fd.get("m_RichText"),
     "vertical_overflow":fd.get("m_VerticalOverflow"),
     "horizontal_overflow":fd.get("m_HorizontalOverflow")})
assert len(entries)>=250,len(entries)
assert sum(isinstance(v["font_data"].get("m_FontSize"),int) for v in entries.values())>=250
original_font_refs=Counter(str(v["font"]) for v in entries.values())
alignment_counts=Counter(v["font_data"].get("m_Alignment") for v in entries.values())
size_counts=Counter(v["font_data"].get("m_FontSize") for v in entries.values())
report={"source":"assets/ui/battlepanel.ab original Unity UGUI Text.m_FontData",
 "text_components":len(entries),"font_reference_distribution":dict(original_font_refs),
 "alignment_distribution":dict(alignment_counts),
 "size_distribution":dict(size_counts),
 "entries":entries,
 "limit":"Serialized typography is known; actual glyph rasterization depends on the original Unity builtin/external Font and rendering."}
OUT=A/"complete_source_mapping"
(OUT/"original_unity_text_font_data.json").write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding="utf8")
GRAPH.write_text(json.dumps(graph,ensure_ascii=False,separators=(",",":")),encoding="utf8")
print("ORIGINAL_UNITY_TEXT_FONT_DATA",json.dumps({k:report[k] for k in
 ("text_components","font_reference_distribution","alignment_distribution","size_distribution")},ensure_ascii=False)[:4200],flush=True)
