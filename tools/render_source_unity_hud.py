#!/usr/bin/env python3
"""Reproduce a HUD layer from the ORIGINAL Last Island LITE 7.3 Unity assets.

INPUTS ONLY: XAPK-extracted Unity Sprite PNGs (by PathID), battlepanel.ab
GameObjects, RectTransforms, Image.m_Sprite component data, and Map_map Texture2D.
No web screenshot or a cropped photograph is ever read by this script.

This is an on-foot static RECONSTRUCTION, not a Unity runtime capture. The
dynamic inventory items, map viewport, HUD customization and CanvasScaler
state cannot be recovered solely from an unloaded prefab.
"""
from pathlib import Path
from functools import lru_cache
from PIL import Image, ImageDraw, ImageFont, ImageChops
from collections import defaultdict, Counter
import argparse, json, math

BUNDLE="assets/ui/battlepanel.ab"

class SourceHUD:
    def __init__(self,assets,canvas=(1920,1080)):
        self.assets=Path(assets)
        self.canvas=canvas
        a=json.loads((self.assets/"manifest_original.json").read_text(encoding="utf8"))
        b=json.loads((self.assets/"ui_prefabs_original/original_hud_ui_hierarchy.json").read_text(encoding="utf8"))
        c=json.loads((self.assets/"original_canvas_components.json").read_text(encoding="utf8"))
        self.g={v["path_id"]:v for v in b["gameobjects"] if v["source_bundle"]==BUNDLE}
        self.t={v["path_id"]:v for v in b["recttransforms"] if v["source_bundle"]==BUNDLE}
        self.bygo={v["go"]:v for v in self.t.values()}
        self.images=c["images"];self.active=c["objects"]
        self.sprites={v["path_id"]:v for v in a["sprites"]}
        self.named=defaultdict(list);self.sprite_names=defaultdict(list)
        for v in self.g.values():self.named[v["name"]].append(v)
        for v in a["sprites"]:self.sprite_names[v["name"]].append(v)
        self.layer=Image.new("RGBA",canvas)
        self.calls=[];self.notes=[]

    @lru_cache(None)
    def rect(self,tid):
        t=self.t.get(tid); W,H=self.canvas
        if t is None:return W/2,H/2,W,H
        parent=self.t.get(t["father"])
        if parent:
            cx,cy,pw,ph=self.rect(parent["path_id"])
            pp=parent["pivot"]
        else:
            cx,cy,pw,ph=W/2,H/2,W,H
            pp={"x":.5,"y":.5}
        lo=t["anchor_min"];hi=t["anchor_max"]
        v=t["pivot"];pos=t["anchored_position"];size=t["size_delta"]
        w=pw*(hi["x"]-lo["x"])+size["x"]
        h=ph*(hi["y"]-lo["y"])+size["y"]
        x=cx+((lo["x"]+hi["x"])/2-pp["x"])*pw+pos["x"]+(.5-v["x"])*w
        y=cy+((lo["y"]+hi["y"])/2-pp["y"])*ph+pos["y"]+(.5-v["y"])*h
        return x,y,w,h

    def go(self,name):
        matches=self.named[name]
        if len(matches)!=1:raise ValueError(f"Unexpected original prefab GameObject multiplicity: {name}: {len(matches)}")
        return matches[0]

    def stamp(self,sprite,rect,label,component=None,image=None):
        W,H=self.canvas
        x,y,w,h=rect
        w=int(round(w));h=int(round(h))
        if not (1<=w<=2500 and 1<=h<=2000):return
        if image is None:
            path=self.assets/sprite["file"]
            assert path.is_file(),path
            with Image.open(path) as source:image=source.convert("RGBA")
        image=image.resize((w,h),Image.Resampling.LANCZOS)
        color=component.get("color") if component else None
        if isinstance(color,dict) and all(k in color for k in ("r","g","b","a")):
            rgb=tuple(int(255*max(0,min(1,color[k]))) for k in ("r","g","b"))
            if any(c!=255 for c in rgb):
                multiplied=ImageChops.multiply(image.convert("RGB"),Image.new("RGB",(w,h),rgb))
                image=Image.merge("RGBA",(*multiplied.split(),image.getchannel("A")))
            if color["a"]<1:
                alpha=max(0,min(1,color["a"]))
                image.putalpha(image.getchannel("A").point(lambda v:int(v*alpha)))
        left=int(round(x-w/2));top=int(round(H-y-h/2))
        if left>=W or top>=H or left+w<=0 or top+h<=0:return
        transparent=Image.new("RGBA",(W,H))
        transparent.paste(image,(left,top))
        self.layer=Image.alpha_composite(self.layer,transparent)
        self.calls.append({"go":label,"sprite":sprite["file"] if sprite else "source Map_map",
                           "x":left,"y":top,"w":w,"h":h})

    def node(self,go,depth=3,exclude=()):
        if isinstance(go,str):go=self.go(go)
        if go["name"] in exclude:return
        if self.active.get(go["path_id"],{}).get("is_active") is False:return
        rectref=self.bygo.get(go["path_id"])
        if not rectref:return
        box=self.rect(rectref["path_id"])
        for pid in go["components"]:
            c=self.images.get(pid)
            if not c or c.get("enabled") is False:continue
            spr=self.sprites.get(c.get("sprite"))
            if spr and (not c.get("color") or c["color"].get("a",1)>0):
                self.stamp(spr,box,go["name"],c)
        if depth>0:
            for tid in rectref["children"]:
                child=self.t.get(tid)
                if child and child["go"] in self.g:
                    self.node(self.g[child["go"]],depth-1,exclude)

    def render_map(self):
        # Use the actual Map_map Texture2D as a preview, never a web map crop.
        root=self.go("m_map")
        top=self.bygo[root["path_id"]]
        kids=[self.g[self.t[x]["go"]] for x in top["children"]
              if x in self.t and self.t[x]["go"] in self.g]
        frame=kids[0]
        ft=self.bygo[frame["path_id"]]
        for tid in ft["children"]:
            rect=self.t.get(tid);go=self.g.get(rect["go"]) if rect else None
            if go and go["name"]=="Image":self.node(go,0)
        maprect=next((self.t[tid] for tid in ft["children"] if tid in self.t and
                     self.g.get(self.t[tid]["go"],{}).get("name")=="ditu"),None)
        src=self.assets/"map_original_7_3.png"
        if src.is_file() and maprect is not None:
            with Image.open(src) as f:
                original=f.convert("RGBA")
                box=(576,576,1472,1472)
                tile=original.crop(box)
                tile=tile.resize((260,260),Image.Resampling.LANCZOS)
                mask=Image.new("L",tile.size)
                ImageDraw.Draw(mask).ellipse((1,1,258,258),fill=255)
                tile.putalpha(mask)
                self.stamp(None,self.rect(maprect["path_id"]),"ditu(Map_map original)",image=tile)
            self.notes.append("Map position and crop are demonstrative, actual player XY and zoom are unknown.")
        for go in kids[1:]:
            self.node(go,depth=2,exclude={"m_nightDi","m_huanghunDi","m_qingchenDi","m_nightIcon"})

    def vitals(self):
        for name in ("m_xue","m_jie","m_bodyWater"):
            self.node(name,4,exclude={"btn_smartHp","btn_smartFood","btn_smartWater",
                                      "m_reduceHp10","m_reduceHp","m_myarmorSlilder"})
        self.notes.append("Actual health/food/water values are dynamic, not serialized in this prefab.")

    def hotbar(self,samples=False):
        test_items=("tb_cl_mt","tb_cl_st","tb_yurou_1","tb_cl_tiek",
                    "tb_sg","wupin_shuihu_1","tb_cl_st")
        centers=[]
        for i in range(7):
            go=self.go("m_Item"+str(i+1))
            rect=self.bygo[go["path_id"]]
            x,y,_,_=self.rect(rect["path_id"])
            centers.append([round(x,4),round(y,4)])
            for tid in rect["children"]:
                r=self.t.get(tid)
                if not r:continue
                child=self.g.get(r["go"])
                if child and child["name"]=="Background":self.node(child,0)
                if child and child["name"]=="m_heightLignt" and i==0:self.node(child,0)
            if samples and self.sprite_names.get(test_items[i]):
                s=self.sprite_names[test_items[i]][0]
                self.stamp(s,(x,y,74,74),f"m_Item{i+1}.sample")
        if samples:self.notes.append("Item images are example original Sprite assets, not a real inventory state.")
        assert len(centers)==7
        return centers

    def render(self,samples=False):
        # Deterministic ON-FOOT subset: do not draw mutually exclusive vehicle,
        # build, diving, store and parachute UI states simultaneously.
        for name in ("btn_info","btn_baseUpgrade","btn_Baserank","btn_help","m_kedu"):
            self.node(name,3,exclude={"m_red_info","m_red_baseUpdate"})
        self.render_map()
        self.vitals()
        for name in ("btn_setting","btn_quckChat","btn_diuShiDongXi","btn_quick_mark"):
            self.node(name,3)
        centers=self.hotbar(samples=samples)
        exclude={"m_autoRunChoosed","EF_btn_autoRun","btn_crouching2","btn_paing2",
                 "btn_jump2","m_jindutiao","m_rechargeEffect"}
        for name in ("btn_pack","btn_zhizao","m_JoystackType2","btn_autoRun",
                     "btn_fire","btn_up2","btn_crouch2","btn_pa2",
                     "btn_kaijing","btn_changeBullet"):
            self.node(name,4,exclude=exclude)
        return centers

def main():
    p=argparse.ArgumentParser()
    p.add_argument("--assets",default="last_island_recreated_game/assets/hud_original_xapk")
    p.add_argument("--output",default="last_island_recreated_game/assets/hud_original_xapk/hud_gerado_do_prefab.png")
    p.add_argument("--demo-items",action="store_true")
    args=p.parse_args()
    hud=SourceHUD(args.assets)
    centers=hud.render(samples=args.demo_items)
    dst=Path(args.output);dst.parent.mkdir(parents=True,exist_ok=True)
    # Independent RGBA layer: can later be used as texture over a 3D scene.
    rgba=dst.with_name("hud_original_overlay_rgba.png")
    hud.layer.save(rgba,optimize=True)
    base=Image.new("RGBA",hud.canvas,(28,38,43,255))
    base.alpha_composite(hud.layer)
    base.convert("RGB").save(dst,optimize=True)
    audit={"claim":"source-prefab reconstruction, NOT original Unity runtime screenshot",
       "original_source_bundle":"assets/ui/battlepanel.ab",
       "original_sprite_atlas":"assets/uiatlas/battlepanel*.ab",
       "original_map_bundle":"assets/texture/map_map.ab",
       "reference_canvas":[1920,1080],
       "draw_calls":hud.calls,"draw_count":len(hud.calls),
       "hotbar_slot_centers":centers,"notes":hud.notes,
       "reproducible_script":"tools/render_source_unity_hud.py",
       "known_limits":["UGUI nine-slice not reproduced exactly",
         "runtime CanvasScaler and custom positions unresolved",
         "preview map viewport position/zoom is demonstrative",
         "some state toggles and runtime-generated data not recovered"]}
    dst.with_name("hud_gerado_do_prefab_auditoria.json").write_text(
        json.dumps(audit,indent=2,ensure_ascii=False),encoding="utf8")
    assert len(hud.calls)>=45,len(hud.calls)
    assert rgba.is_file() and dst.is_file()
    print("HUD_SOURCE_RENDER_PASS",len(hud.calls),"draw calls")
    print("HUD_SEVEN_SOURCE_HOTBAR_SLOTS_PASS",centers)
    print("HUD_RENDER",dst,"RGBA",rgba)
if __name__=="__main__":main()
