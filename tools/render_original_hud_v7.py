#!/usr/bin/env python3
"""V7: original Unity HUD state-driven numbers and inventory contents.

Source of every position: Last Island LITE 7.3 / assets/ui/battlepanel.ab
via full source RectTransforms. Every graphic: exact Unity sprite PNG from
original XAPK. Text values come from explicit runtime JSON. Text font is a
documented FALLBACK because original UI Text components reference m_Font=0.

NEVER uses a cropped screenshot to build the HUD. Does not alter active game.
"""
import argparse
import importlib.util
import json
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT=Path(__file__).resolve().parents[1]
ASSETS=ROOT/"last_island_recreated_game/assets/hud_original_xapk"
module_spec=importlib.util.spec_from_file_location("render_original_hud_v6",
    ROOT/"tools/render_original_hud_v6.py")
v6=importlib.util.module_from_spec(module_spec)
module_spec.loader.exec_module(v6)

class HudV7(v6.HudSourceOrdered):
    TEST_ITEM_NAMES=("tb_cl_mt","tb_cl_st","tb_yurou_1","tb_cl_tiek",
                     "tb_sg","wupin_shuihu_1","tb_cl_st")
    # Only placeholders for the preview; does not assert original gameplay inventory.
    TEST_COUNTS=(1,20,30,10,3,2,8)

    def __init__(self,assets=ASSETS,**kwargs):
        super().__init__(assets,**kwargs)
        self.selected_slot=0
        self.text_draws=[]
        self.item_draws=[]
        self._ui_text_font_name="DejaVu Sans fallback; original Unity m_Font=0"
        self._preview_items=False
        self._inventory=[]

    def _descendant(self,go_id,name):
        """Find an exact named child inside the original m_ItemN hierarchy."""
        seen=set()
        stack=[go_id]
        matches=[]
        while stack:
            current=stack.pop()
            if current in seen:continue
            seen.add(current)
            obj=self.game_objects.get(current)
            if obj and obj["name"]==name:matches.append(current)
            rect=self.bygo.get(current)
            if rect:
                for ptr in reversed(rect["children"]):
                    child=self.t.get(ptr)
                    if child and child["go"] in self.game_objects:
                        stack.append(child["go"])
        if len(matches)!=1:
            raise ValueError(f"Original Unity {name} child not unique under {go_id}: {len(matches)}")
        return matches[0]

    def _source_text(self,go_id,value,*,align="center"):
        assert isinstance(value,str) and value.isdecimal(),value
        go=self.game_objects[go_id]
        comps=[self.source_comps[v["path_id"]] for v in go["components"]
            if v["path_id"] in self.source_comps and
            self.source_comps[v["path_id"]]["type"]=="Text"]
        assert len(comps)==1,(go["name"],len(comps))
        text_component=comps[0]
        assert text_component["enabled"]
        source_rect=self.bygo[go_id]
        x,y,w,h=self.rect(source_rect["path_id"])
        if not (w>0 and h>0):return
        font_size=max(10,min(20,round(h*.70)))
        fontfile=Path("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf")
        if fontfile.is_file():
            font=ImageFont.truetype(str(fontfile),font_size)
        else:
            font=ImageFont.load_default()
        draw=ImageDraw.Draw(self.layer)
        color=text_component.get("color") or {}
        rgb=tuple(int(max(0,min(1,float(color.get(k,1))))*255) for k in ("r","g","b"))
        alpha=int(max(0,min(1,float(color.get("a",1))))*255)
        # PIL uses source-origin top-left; Unity RectTransform origin is bottom-left.
        top=self.canvas[1]-y-h/2
        left=x-w/2
        if align=="right":
            dx=left+w-3
            anchor="ra"
        elif align=="left":
            dx=left+3
            anchor="la"
        else:
            dx=x
            anchor="ma"
        dy=top+(h-font_size)/2
        draw.text((round(dx),round(dy)),value,font=font,fill=(*rgb,alpha),
                  anchor=anchor)
        self.text_draws.append({
            "object":go["name"],"source_gameobject_path_id":go_id,
            "source_component_path_id":text_component["source_path_id"],
            "source_rect_path_id":source_rect["path_id"],
            "value":value,
            "font_origin":"fallback, original m_Font PathID=0",
            "pixel_bounds":[round(left,3),round(top,3),round(left+w,3),round(top+h,3)]
        })

    @staticmethod
    def _parse_inventory(items,demo):
        if items is None:
            if not demo:return [None]*7
            return [{"sprite":name,"count":count} for name,count in
                zip(HudV7.TEST_ITEM_NAMES,HudV7.TEST_COUNTS)]
        assert isinstance(items,list) and len(items)==7,"inventory must contain seven entries"
        result=[]
        for entry in items:
            if entry is None:
                result.append(None)
            elif isinstance(entry,str):
                result.append({"sprite":entry,"count":1})
            elif isinstance(entry,dict):
                assert set(entry)<={"sprite","count"} and "sprite" in entry
                assert isinstance(entry["sprite"],str) and entry["sprite"]
                count=entry.get("count",1)
                assert type(count) is int and 0<=count<=999999,entry
                result.append({"sprite":entry["sprite"],"count":count})
            else:
                raise TypeError("Item must be null, original sprite name, or {sprite,count}")
        return result

    def hotbar(self,samples=False,items=None):
        inventory=self._parse_inventory(items,samples)
        self._inventory=inventory
        centers=[]
        for i,item in enumerate(inventory):
            source_item=self.go("m_Item"+str(i+1))
            rect=self.bygo[source_item["path_id"]]
            x,y,_,_=self.rect(rect["path_id"])
            centers.append([round(x,4),round(y,4)])
            # Every background and selection graphic is drawn from its own
            # Unity GameObject -> Image.m_Sprite FileID / PathID.
            for childid in rect["children"]:
                child=self.t.get(childid)
                if not child:continue
                go=self.g.get(child["go"])
                if not go:continue
                if go["name"]=="Background":
                    self.node(go,depth=0)
                elif go["name"]=="m_heightLignt" and i==self.selected_slot:
                    self.node(go,depth=0)
            if item is None:continue
            sprite_name=item["sprite"]
            choices=self.sprite_names.get(sprite_name)
            if not choices:
                raise KeyError("Unknown source Unity item sprite: "+sprite_name)
            # Runtime icon sprite is selected by inventory item ID in the real
            # game, not serialized as a fixed m_Sprite in the blank m_icon.
            candidates=sorted(choices,key=lambda a:(a.get("bundle",""),a["file"]))
            asset=candidates[0]
            icon_go=self._descendant(source_item["path_id"],"m_icon")
            icon_rect=self.bygo[icon_go]["path_id"]
            assert self.source_comps[next(p["path_id"]
               for p in self.game_objects[icon_go]["components"]
               if p["path_id"] in self.source_comps and
                 self.source_comps[p["path_id"]]["type"]=="Image")]["sprite"]["path_id"]=="0"
            self.stamp(asset,self.rect(icon_rect),
                "m_Item"+str(i+1)+".runtime_m_icon")
            self.item_draws.append({"slot_index":i,"source_slot":source_item["path_id"],
                "source_icon_rect":icon_rect,"original_sprite_file":asset["file"],
                "original_sprite_path_id":asset["path_id"],"count":item["count"]})
            if item["count"]>1:
                qty_go=self._descendant(source_item["path_id"],"txt_num")
                self._source_text(qty_go,str(item["count"]),align="right")
        assert len(centers)==7
        return centers

    def draw_vital_texts(self,values):
        if values is None:return
        mapping={"health":"txt_mybloodNum",
                 "food":"txt_hunger","water":"txt_bodyWater"}
        for key,source_name in mapping.items():
            if key not in values:continue
            value=values[key]
            assert isinstance(value,(int,float)) and not isinstance(value,bool)
            assert 0<=value<=1,(key,value)
            go=self.go(source_name)
            self._source_text(go["path_id"],str(round(value*100)))

    def render_complete(self,*,state,demo=False):
        assert isinstance(state,dict)
        selected=state.get("selected_slot",0)
        assert type(selected) is int and 0<=selected<=6,selected
        self.selected_slot=selected
        inventory=state.get("inventory",None)
        self._preview_items=bool(demo and inventory is None)
        profile=state.get("profile","on-foot-melee")
        # Use the V6 exact source component and sibling order for graphics.
        record=self.render_ordered(profile=profile,
           items=inventory, samples=demo,
           vitals=state.get("vitals"))
        # Text is handled after images: source RectTransforms, true values,
        # fallback glyphs explicitly documented (original font not recovered).
        self.draw_vital_texts(state.get("vitals"))
        record.update({
            "original_text_components_drawn_with_fallback":len(self.text_draws),
            "texts":self.text_draws,
            "original_item_icon_rectangles":self.item_draws,
            "selected_hotbar_slot":selected,
            "preview_inventory_is_demonstrative":self._preview_items,
            "runtime_inventory_values_supplied":inventory is not None,
            "original_unity_font_available":False,
            "font_fidelity_note":"Text is positioned from original Unity RectTransform and colored by original component but rendered with a fallback font because original m_Font PathID=0.",
        })
        return record

def main():
    parser=argparse.ArgumentParser()
    parser.add_argument("--state",type=Path)
    parser.add_argument("--demo-items",action="store_true")
    parser.add_argument("--profile",choices=["on-foot-melee","on-foot-rifle","on-foot-build"],default="on-foot-melee")
    parser.add_argument("--assets",type=Path,default=ASSETS)
    parser.add_argument("--output",type=Path,
       default=ASSETS/"complete_source_mapping/hud_v7_original_dynamic_texts.png")
    args=parser.parse_args()
    state=json.loads(args.state.read_text(encoding="utf8")) if args.state else {}
    state.setdefault("profile",args.profile)
    hud=HudV7(args.assets,map_uv=state.get("map_uv",[.5,.5]),
              map_window=state.get("map_window",896),
              heading_deg=state.get("heading_deg",0))
    result=hud.render_complete(state=state,demo=args.demo_items)
    args.output.parent.mkdir(parents=True,exist_ok=True)
    transparent=args.output.with_name(args.output.stem+"_transparent.png")
    hud.layer.save(transparent,optimize=True)
    bg=Image.new("RGBA",hud.canvas,(28,38,43,255))
    bg.alpha_composite(hud.layer)
    bg.convert("RGB").save(args.output,optimize=True)
    report=args.output.with_suffix(".json")
    result["render_preview"]=str(args.output)
    result["transparent_overlay"]=str(transparent)
    result["draw_calls"]=hud.calls
    report.write_text(json.dumps(result,indent=2,ensure_ascii=False),encoding="utf8")
    print("HUD_V7_ORIGINAL_TEXT_RECTS",len(hud.text_draws),"ORIGINAL_ITEM_ICONS",len(hud.item_draws))
    print("HUD_V7_OUTPUT",str(args.output))
if __name__=="__main__":main()
