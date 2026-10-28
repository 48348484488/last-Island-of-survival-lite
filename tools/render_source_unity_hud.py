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
from PIL import Image, ImageDraw, ImageFont, ImageChops, ImageOps
from unity_ugui_raster import apply_fill, preserve_aspect
from collections import defaultdict, Counter
import argparse, json, math

BUNDLE="assets/ui/battlepanel.ab"

class SourceHUD:
    def __init__(self,assets,canvas=(1920,1080),map_uv=(0.5,0.5),map_window=896,heading_deg=0.0):
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
        # Resolve remaining game assets by exact Unity Sprite PPtr fileID and PathID.
        # Entries are direct exports from matching CAB bundles inside the original XAPK.
        # Never substitute another asset merely because the name looks similar.
        external=self.assets/"complete_source_mapping/external_hud_sprites_index.json"
        self.external_original_sprites=0
        if external.is_file():
            evidence=json.loads(external.read_text(encoding="utf8"))
            for v in evidence.get("sprites",[]):
                if not (self.assets/v["file"]).is_file():
                    raise FileNotFoundError("Source Unity HUD dependency missing: "+v["file"])
                if v["path_id"] in self.sprites:
                    # A PathID collision means source binding also needs m_FileID.
                    existing=self.sprites[v["path_id"]]
                    if existing["file"]!=v["file"]:
                        raise ValueError("Duplicate Unity Sprite PathID across CABs: "+v["path_id"])
                self.sprites[v["path_id"]]=v
                self.sprite_names[v["name"]].append(v)
                self.external_original_sprites+=1
        # A few Sprite objects live INSIDE the original UI prefab but their
        # Texture2D is in a different original CAB. Once decoded, use those
        # exact local PathIDs and never a lookalike texture.
        embedded=self.assets/"complete_source_mapping/original_unity_all_hud_components.json"
        self.embedded_original_sprites=0
        if embedded.is_file():
            graph=json.loads(embedded.read_text(encoding="utf8"))
            for pid,v in graph.get("sprite_assets",{}).items():
                if not v.get("file"):continue
                if not (self.assets/v["file"]).is_file():
                    raise FileNotFoundError("Original HUD embedded source missing: "+v["file"])
                if pid in self.sprites and self.sprites[pid]["file"]!=v["file"]:
                    raise ValueError("Conflicting source local Sprite ID "+pid)
                self.sprites[pid]=v
                self.sprite_names[v["name"]].append(v)
                self.embedded_original_sprites+=1
        self.layer=Image.new("RGBA",canvas)
        self.calls=[];self.notes=[]
        self._source_hits=Counter()
        self._skipped=Counter()
        self.profile="on-foot-melee"
        assert len(map_uv)==2 and all(0.0<=float(v)<=1.0 for v in map_uv),map_uv
        assert 64<=int(map_window)<=2048,map_window
        self.map_uv=tuple(map(float,map_uv))
        self.map_window=int(map_window)
        self.heading_deg=float(heading_deg)%360.0
        meta=self.assets/"hud_source_sprite_borders.json"
        self.sprite_borders=json.loads(meta.read_text(encoding="utf8")) if meta.exists() else {}
        self.vitals_values=None

    @staticmethod
    def _sliced_sprite(img,size,border):
        """UGUI Image.Type.Sliced: preserve original Sprite.m_Border corner pixels."""
        W,H=map(int,size);sw,sh=img.size
        left,bottom,right,top=(max(0,round(border.get(k,0))) for k in ("x","y","z","w"))
        left=min(left,sw//2,W//2);right=min(right,sw-left,W-left)
        top=min(top,sh//2,H//2);bottom=min(bottom,sh-top,H-top)
        if not any((left,bottom,right,top)):
            return img.resize((W,H),Image.Resampling.LANCZOS)
        result=Image.new("RGBA",(W,H))
        src_x=(0,left,sw-right,sw);src_y=(0,top,sh-bottom,sh)
        dst_x=(0,left,W-right,W);dst_y=(0,top,H-bottom,H)
        for row in range(3):
            for col in range(3):
                src_box=(src_x[col],src_y[row],src_x[col+1],src_y[row+1])
                dst_box=(dst_x[col],dst_y[row],dst_x[col+1],dst_y[row+1])
                width,height=dst_box[2]-dst_box[0],dst_box[3]-dst_box[1]
                if width<=0 or height<=0 or src_box[2]<=src_box[0] or src_box[3]<=src_box[1]:continue
                patch=img.crop(src_box)
                if patch.size!=(width,height):patch=patch.resize((width,height),Image.Resampling.BILINEAR)
                result.paste(patch,dst_box[:2])
        return result

    @lru_cache(None)
    def scale(self,tid):
        t=self.t.get(tid)
        if not t:return (1.,1.)
        p=self.t.get(t["father"])
        psx,psy=self.scale(p["path_id"]) if p else (1.,1.)
        ls=t.get("local_scale") or {}
        return (psx*ls.get("x",1.),psy*ls.get("y",1.))

    @lru_cache(None)
    def rect(self,tid):
        t=self.t.get(tid); W,H=self.canvas
        if t is None:return W/2,H/2,W,H
        parent=self.t.get(t["father"])
        if parent:
            cx,cy,pw,ph=self.rect(parent["path_id"])
            pp=parent["pivot"]
            parent_scale=self.scale(parent["path_id"])
        else:
            cx,cy,pw,ph=W/2,H/2,W,H
            pp={"x":.5,"y":.5}
            parent_scale=(1.,1.)
        lo=t["anchor_min"];hi=t["anchor_max"]
        v=t["pivot"];pos=t["anchored_position"];size=t["size_delta"]
        lx,ly=(t.get("local_scale") or {}).get("x",1.),(t.get("local_scale") or {}).get("y",1.)
        w=(pw*(hi["x"]-lo["x"])+size["x"]*parent_scale[0])*lx
        h=(ph*(hi["y"]-lo["y"])+size["y"]*parent_scale[1])*ly
        x=cx+((lo["x"]+hi["x"])/2-.5)*pw+pos["x"]*parent_scale[0]+(.5-v["x"])*w
        y=cy+((lo["y"]+hi["y"])/2-.5)*ph+pos["y"]*parent_scale[1]+(.5-v["y"])*h
        return x,y,w,h

    def go(self,name):
        matches=self.named[name]
        if len(matches)!=1:raise ValueError(f"Unexpected original prefab GameObject multiplicity: {name}: {len(matches)}")
        return matches[0]

    def stamp(self,sprite,rect,label,component=None,image=None,circle_mask=False):
        W,H=self.canvas
        x,y,w,h=rect
        w=int(round(w));h=int(round(h))
        if not (1<=w<=2500 and 1<=h<=2000):return
        if image is None:
            path=self.assets/sprite["file"]
            assert path.is_file(),path
            with Image.open(path) as source:image=source.convert("RGBA")
        if component and component.get("type")==1 and sprite:
            m=self.sprite_borders.get(sprite["file"])
            if m and m.get("border"):
                image=self._sliced_sprite(image,(w,h),m["border"])
            else:
                image=image.resize((w,h),Image.Resampling.LANCZOS)
        elif component and component.get("preserve_aspect") and sprite:
            image=preserve_aspect(image,(w,h))
        else:
            image=image.resize((w,h),Image.Resampling.LANCZOS)
        # Unity uses a different mesh for each m_FillMethod. The previous
        # renderer incorrectly clipped ALL fills horizontally left-to-right.
        if component and component.get("type")==3:
            amount=float(component.get("fill_amount") or 0)
            if amount<=0:return
            image=apply_fill(image,amount,
                  method=component.get("fill_method") or 0,
                  origin=component.get("fill_origin") or 0,
                  clockwise=bool(component.get("fill_clockwise",True)))
        color=component.get("color") if component else None
        if isinstance(color,dict) and all(k in color for k in ("r","g","b","a")):
            rgb=tuple(int(255*max(0,min(1,color[k]))) for k in ("r","g","b"))
            if any(c!=255 for c in rgb):
                multiplied=ImageChops.multiply(image.convert("RGB"),Image.new("RGB",(w,h),rgb))
                image=Image.merge("RGBA",(*multiplied.split(),image.getchannel("A")))
            if color["a"]<1:
                alpha=max(0,min(1,color["a"]))
                image.putalpha(image.getchannel("A").point(lambda v:int(v*alpha)))
        if circle_mask:
            mask=Image.new("L",(w,h),0)
            ImageDraw.Draw(mask).ellipse((1,1,w-2,h-2),fill=255)
            image.putalpha(ImageChops.multiply(image.getchannel("A"),mask))
        left=int(round(x-w/2));top=int(round(H-y-h/2))
        if left>=W or top>=H or left+w<=0 or top+h<=0:return
        # Clip and alpha-composite only the affected rectangle. The earlier
        # implementation allocated TWO full 1920x1080 canvases for EVERY
        # original sprite (roughly 60 per HUD refresh), preventing the
        # source-correct HUD from being usable in the live 3D render loop.
        x0=max(0,left);y0=max(0,top)
        x1=min(W,left+w);y1=min(H,top+h)
        if x0>=x1 or y0>=y1:return
        patch=image.crop((x0-left,y0-top,x1-left,y1-top))
        self.layer.alpha_composite(patch,dest=(x0,y0))
        self.calls.append({"go":label,"sprite":sprite["file"] if sprite else "source Map_map",
                           "x":left,"y":top,"w":w,"h":h})
        if sprite:self._source_hits[sprite["path_id"]]+=1
        if label=="m_compassImage":self.calls[-1]["sprite"]="compass_original_source.png"

    def node(self,go,depth=3,exclude=()):
        if isinstance(go,str):go=self.go(go)
        if go["name"] in exclude:return
        if self.active.get(go["path_id"],{}).get("is_active") is False:
            self._skipped["inactive_in_prefab"]+=1
            return
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

    def render_compass(self):
        """Draw original m_compassImage RawImage, masked with original UI fade.

        Never typeset compass degrees by hand: the Texture2D is taken from the
        exact m_Texture local PathID in original assets/ui/battlepanel.ab.
        """
        imgfile=self.assets/"compass_original_source.png"
        metadata=self.assets/"compass_original_source.json"
        if not (imgfile.is_file() and metadata.is_file()):
            self.node("m_kedu",3,exclude={"m_vehicleSound","m_gunSound","m_wurenjiSound"})
            self.notes.append("Original compass RawImage not yet extracted")
            return
        doc=json.loads(metadata.read_text(encoding="utf8"))
        raw=doc["rawimage_component"]
        assert raw["m_Texture"]["m_PathID"]==int(doc["textures"][0]["path_id"])
        compass=self.go("m_compassImage")
        t=self.bygo[compass["path_id"]]
        parent=self.t[t["father"]]
        box=self.rect(parent["path_id"])
        _,_,cw,ch=box
        cw,ch=max(1,round(cw)),max(1,round(ch))
        _,_,fullw,fullh=self.rect(t["path_id"])
        fw,fh=max(cw,round(abs(fullw))),max(ch,round(abs(fullh)))
        with Image.open(imgfile) as rawimg:
            full=rawimg.convert("RGBA").resize((fw,fh),Image.Resampling.LANCZOS)
        # Heading is a runtime camera input; 0 degrees is the test fallback.
        xshift=round(self.heading_deg/360.0*fw)
        full=ImageChops.offset(full,-xshift,0)
        left=(fw-cw)//2;top=max(0,(fh-ch)//2)
        tile=full.crop((left,top,left+cw,top+ch))
        srclist=self.sprite_names.get("mask_leftrightfade",[])
        assert srclist,"Original Unity source compass fade mask absent"
        fade=next((r for r in srclist if r["bundle"]=="common.ab"),srclist[0])
        with Image.open(self.assets/fade["file"]) as png:
            alpha=png.convert("RGBA").getchannel("A").resize((cw,ch),Image.Resampling.BILINEAR)
        tile.putalpha(ImageChops.multiply(tile.getchannel("A"),alpha))
        self.stamp(None,box,"m_compassImage",image=tile)
        self.notes.append("Compass RawImage from exact source m_Texture PathID")
        root=self.go("m_kedu")
        for childtid in self.bygo[root["path_id"]]["children"]:
            rt=self.t.get(childtid)
            child=self.g.get(rt["go"]) if rt else None
            if child and child["name"]=="Image (2)":self.node(child,0)

    def render_map(self):
        # Use the actual Map_map Texture2D as a preview, never a web map crop.
        root=self.go("m_map")
        top=self.bygo[root["path_id"]]
        kids=[self.g[self.t[x]["go"]] for x in top["children"]
              if x in self.t and self.t[x]["go"] in self.g]
        frame=kids[0]
        ft=self.bygo[frame["path_id"]]
        # Unity's Image background is inside a circular mask; drawing the PNG
        # as a normal rectangle used to produce an incorrect black square.
        for tid in ft["children"]:
            rr=self.t.get(tid);go=self.g.get(rr["go"]) if rr else None
            if not go or go["name"]!="Image":continue
            for pid in go["components"]:
                comp=self.images.get(pid)
                spr=self.sprites.get(comp.get("sprite")) if comp else None
                if spr:self.stamp(spr,self.rect(rr["path_id"]),go["name"],comp)
        maprect=next((self.t[tid] for tid in ft["children"] if tid in self.t and
                     self.g.get(self.t[tid]["go"],{}).get("name")=="ditu"),None)
        src=self.assets/"map_original_7_3.png"
        if src.is_file() and maprect is not None:
            with Image.open(src) as f:
                original=f.convert("RGBA")
                # Position/zoom are runtime inputs, not a pasted screenshot.
                mw,mh=original.size
                cx,cy=int(self.map_uv[0]*mw),int(self.map_uv[1]*mh)
                r=min(self.map_window,mw,mh)
                x0=max(0,min(mw-r,cx-r//2));y0=max(0,min(mh-r,cy-r//2))
                tile=original.crop((x0,y0,x0+r,y0+r))
                rectbox=self.rect(maprect["path_id"])
                tw,th=max(1,abs(round(rectbox[2]))),max(1,abs(round(rectbox[3])))
                tile=tile.resize((tw,th),Image.Resampling.LANCZOS)
                # 'ditu' is bound to source mask_sidesfade, a soft square alpha,
                # not a circular cutout inferred from an arbitrary screenshot.
                comp=next((self.images[c] for c in self.g[maprect["go"]]["components"]
                           if c in self.images and self.images[c].get("sprite") in self.sprites),None)
                mask_spr=self.sprites.get(comp["sprite"]) if comp else None
                if mask_spr and mask_spr["name"]=="mask_sidesfade":
                    with Image.open(self.assets/mask_spr["file"]) as source_mask:
                        mask=source_mask.convert("RGBA").getchannel("A").resize((tw,th),Image.Resampling.BILINEAR)
                    tile.putalpha(ImageChops.multiply(tile.getchannel("A"),mask))
                else:
                    self.notes.append("Source map edge mask missing: showing unmasked original map")
                self.stamp(None,rectbox,"ditu(Map_map original)",image=tile)
            self.notes.append("Map position and crop are demonstrative, actual player XY and zoom are unknown.")
        for go in kids[1:]:
            self.node(go,depth=2,exclude={"m_nightDi","m_huanghunDi","m_qingchenDi","m_nightIcon",
                "m_turretSoundInMap","m_vehicleSoundInMap","m_gunSoundInMap",
                "m_footSoundInMap","m_airdrop_pos_map"})

    def vitals(self):
        for name in ("m_xue","m_jie","m_bodyWater"):
            self.node(name,4,exclude={"btn_smartHp","btn_smartFood","btn_smartWater",
                                      "m_reduceHp10","m_reduceHp","m_myarmorSlilder"})
        # Original Slider fill Images use source Sprite, tint and runtime value.
        # Their rect may be zero width in unloaded prefabs, so use source
        # background Image bounds only when a real state ratio is supplied.
        if self.vitals_values is not None:
            mapping=(("health","m_xue","m_mybloodtmpFillImage"),
                     ("food","m_jie","m_myhungerFillImage"),
                     ("water","m_bodyWater","m_myWaterFillImage"))
            for key,rootname,fillname in mapping:
                if key not in self.vitals_values:continue
                pct=float(self.vitals_values[key])
                assert 0<=pct<=1,(key,pct)
                root=self.go(rootname)
                refs=[]
                for child in self.bygo[root["path_id"]]["children"]:
                    rt=self.t.get(child)
                    if rt and self.g.get(rt["go"],{}).get("name")=="Image":refs.append(rt)
                if not refs:continue
                box=self.rect(refs[0]["path_id"])
                fillgo=self.go(fillname)
                comp=next((self.images[c] for c in fillgo["components"] if c in self.images and self.images[c].get("sprite") in self.sprites),None)
                if not comp:continue
                source=self.sprites[comp["sprite"]]
                bx,by,bw,bh=box
                bx+=2.5;bw=max(1,bw-5)
                if pct>0:
                    self.stamp(source,(bx-bw*(1-pct)/2,by,bw*pct,bh-2),
                               fillname+"_runtime",{**comp,"type":0})
            self.notes.append("Vital ratios supplied by explicit runtime state, using original Sprite colors.")
        else:
            self.notes.append("Actual health/food/water values are dynamic, not serialized in this prefab.")

    def hotbar(self,samples=False,items=None):
        test_items=("tb_cl_mt","tb_cl_st","tb_yurou_1","tb_cl_tiek",
                    "tb_sg","wupin_shuihu_1","tb_cl_st")
        if items is not None:
            assert isinstance(items,list) and len(items)==7,                 "HUD state.items must contain seven original Sprite names or null"
            active_items=items
        else:
            active_items=test_items if samples else [None]*7
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
            requested=active_items[i]
            if requested:
                if requested not in self.sprite_names:
                    raise KeyError(f"Item sprite is not in original XAPK source: {requested}")
                sp=sorted(self.sprite_names[requested],key=lambda row:row["file"])[0]
                self.stamp(sp,(x,y,74,74),f"m_Item{i+1}.source_item")
        if samples and items is None:
            self.notes.append("Item images are example original Sprite assets, not a real inventory state.")
        assert len(centers)==7
        return centers

    def render(self,samples=False,profile="on-foot-melee",items=None,vitals=None):
        if profile not in ("on-foot-melee","on-foot-rifle"):
            raise ValueError("Unknown Unity gameplay mode: "+str(profile))
        self.profile=profile
        if vitals is not None:
            assert isinstance(vitals,dict)
            self.vitals_values=vitals
        # Deterministic ON-FOOT subset: do not draw mutually exclusive vehicle,
        # build, diving, store and parachute UI states simultaneously.
        for name in ("btn_info","btn_baseUpgrade","btn_Baserank","btn_help"):
            self.node(name,3,exclude={"m_red_info","m_red_baseUpdate"})
        self.render_compass()
        self.render_map()
        self.vitals()
        for name in ("btn_setting","btn_quckChat","btn_diuShiDongXi","btn_quick_mark"):
            self.node(name,3)
        centers=self.hotbar(samples=samples,items=items)
        exclude={"m_autoRunChoosed","EF_btn_autoRun","btn_crouching2","btn_paing2",
                 "btn_jump2","m_jindutiao","m_rechargeEffect",
                 "m_PatiziImage","m_openGliderIcon","m_flyEffect",
                 "m_addOilIcon","m_StencilMask","m_pack_slider","m_newbee_effect"}
        base=("btn_pack","btn_zhizao","m_JoystackType2","btn_autoRun",
              "btn_fire","btn_up2","btn_crouch2","btn_pa2")
        weapon=("btn_kaijing","btn_changeBullet") if profile=="on-foot-rifle" else ()
        for name in base+weapon:
            self.node(name,4,exclude=exclude)
        return centers

def main():
    p=argparse.ArgumentParser()
    p.add_argument("--assets",default="last_island_recreated_game/assets/hud_original_xapk")
    p.add_argument("--output",default="last_island_recreated_game/assets/hud_original_xapk/hud_gerado_do_prefab.png")
    p.add_argument("--demo-items",action="store_true",
                   help="Display example original item sprites, not a real inventory")
    p.add_argument("--profile",choices=("on-foot-melee","on-foot-rifle"),default="on-foot-melee")
    p.add_argument("--state",help="JSON state: optional items[7], map_uv[2], map_window, profile")
    args=p.parse_args()
    state=json.loads(Path(args.state).read_text(encoding="utf8")) if args.state else {}
    assert isinstance(state,dict),"HUD state must be a JSON object"
    hud=SourceHUD(args.assets,map_uv=state.get("map_uv",[.5,.5]),
                  map_window=state.get("map_window",896),heading_deg=state.get("heading_deg",0))
    profile=state.get("profile",args.profile)
    centers=hud.render(samples=args.demo_items,profile=profile,items=state.get("items"),vitals=state.get("vitals"))
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
       "reference_canvas":[1920,1080],"profile":profile,
       "map_uv_source":"provided at runtime" if args.state and "map_uv" in state else "illustrative center",
       "map_uv":hud.map_uv,"map_window":hud.map_window,
       "camera_heading_degrees":hud.heading_deg,
       "sprites_from_original_xapk":len(hud._source_hits),
       "additional_original_CAB_sprites_loaded":hud.external_original_sprites,
       "additional_prefab_embedded_sprites_loaded":hud.embedded_original_sprites,
       "sample_items_are_illustrative":args.demo_items,
       "recttransform_local_scale_enabled":True,
       "recttransform_anchors_calculated_from_parent_rect_center":True,
       "source_9slice_border_metadata":len(hud.sprite_borders),
       "vital_ratios_from_runtime_state":bool(hud.vitals_values),
       "unity_image_filled_alpha_enabled":True,
       "unity_minimap_stencil_approximation":"source original mask_sidesfade alpha; native Unity shader still incomplete",
       "draw_calls":hud.calls,"draw_count":len(hud.calls),
       "hotbar_slot_centers":centers,"notes":hud.notes,
       "reproducible_script":"tools/render_source_unity_hud.py",
       "known_limits":["UGUI nine-slice when original Sprite.m_Border absent, native shader and runtime masking not exactly reproduced",
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
