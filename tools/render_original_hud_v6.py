#!/usr/bin/env python3
"""V6: source-ordered Unity on-foot HUD renderer.

All pixels come from original Last Island LITE 7.3 XAPK Texture2D/Sprite objects.
No screenshot crops or artificial sprites. Positions, Image FileID/PathID,
transparency and sibling draw order are read from the original Unity prefab.

Runtime caveats: GameObject activation, localization/Fonts, full Unity
CanvasScaler, native UI shaders, image material effects and gameplay events
cannot be validated from the prefab alone. This renders one SELECTED on-foot
state, not all 1823 source objects simultaneously.
"""
import argparse
import importlib.util
import json
import math
from collections import Counter
from pathlib import Path

from PIL import Image

ROOT=Path(__file__).resolve().parents[1]
ASSETS=ROOT/"last_island_recreated_game/assets/hud_original_xapk"
p=ROOT/"tools/render_source_unity_hud.py"
spec=importlib.util.spec_from_file_location("hud_base",p)
base=importlib.util.module_from_spec(spec)
spec.loader.exec_module(base)

class HudSourceOrdered(base.SourceHUD):
    def __init__(self,assets=ASSETS,**kwargs):
        super().__init__(assets,**kwargs)
        path=self.assets/"complete_source_mapping/original_unity_all_hud_components.json"
        self.graph=json.loads(path.read_text(encoding="utf8"))
        self.source_comps=self.graph["components"]
        self.game_objects=self.graph["objects"]
        self.drawn_components=[]
        self.suppressed=Counter()
        self._tree_index={}
        for tid,t in self.t.items():
            for i,child in enumerate(t["children"]):
                self._tree_index[child]=(tid,i)
        self._source_files={x["file"] for x in self.sprites.values()}

    def z_key(self,name):
        """Exact Unity UI sibling traversal order. No hand-authored coordinates."""
        obj=self.go(name)
        r=self.bygo.get(obj["path_id"])
        if r is None:return (10**9,)
        indexes=[]
        tid=r["path_id"]
        visited=set()
        while tid in self.t:
            if tid in visited:raise RuntimeError("Unity HUD RectTransform loop at "+tid)
            visited.add(tid)
            p=self._tree_index.get(tid)
            if p is None:break
            indexes.append(p[1])
            tid=p[0]
        return tuple(reversed(indexes))

    def node(self,go,depth=3,exclude=()):
        """Draw actual Image PPtr rather than a similarly named Sprite from other AB."""
        if isinstance(go,str):go=self.go(go)
        if go["name"] in exclude:
            self.suppressed["mode_excluded"]+=1
            return
        original=self.game_objects.get(go["path_id"],{})
        if not original.get("active_self",True):
            self.suppressed["prefab_inactive"]+=1
            return
        rect=self.bygo.get(go["path_id"])
        if not rect:return
        bounds=self.rect(rect["path_id"])
        for pptr in original.get("components",[]):
            pid=pptr["path_id"]
            comp=self.source_comps.get(pid)
            if comp is None or not comp.get("enabled",True):continue
            typ=comp.get("type")
            if typ not in ("Image","Text","RawImage"):continue
            if typ=="Text":
                self.suppressed["font_and_runtime_text_not_reproduced"]+=1
                continue
            if typ=="RawImage":
                # Compass and map are handled via their exact Texture2D sources.
                self.suppressed["rawimage_explicit_renderer_only"]+=1
                continue
            ref=comp.get("sprite",{})
            if ref.get("path_id")=="0":
                self.suppressed["image_without_sprite"]+=1
                continue
            relative=comp.get("resolved_sprite_file")
            if not relative:
                raise RuntimeError("Original Image.m_Sprite unresolved: "+pid)
            source_path=self.assets/relative
            if not source_path.is_file():
                raise FileNotFoundError("Unity source texture not in repository: "+str(source_path))
            png={"file":relative,
                "path_id":ref["path_id"],
                "name":source_path.stem}
            settings={"type":comp.get("image_type"),
                      "color":comp.get("color"),
                      "fill_amount":comp.get("fill_amount"),
                      "fill_method":comp.get("fill_method"),
                      "fill_origin":comp.get("fill_origin"),
                      "fill_clockwise":comp.get("fill_clockwise"),
                      "preserve_aspect":comp.get("preserve_aspect")}
            prior=len(self.calls)
            self.stamp(png,bounds,go["name"],settings)
            if len(self.calls)>prior:
                self.calls[-1].update({"original_file_id":ref["file_id"],
                    "original_path_id":ref["path_id"],
                    "original_component_path_id":pid,
                    "original_gameobject_path_id":go["path_id"]})
                self.drawn_components.append(pid)
        if depth>0:
            for tid in rect["children"]:
                c=self.t.get(tid)
                if c and c["go"] in self.g:
                    self.node(self.g[c["go"]],depth-1,exclude)

    def render_ordered(self,*,profile="on-foot-melee",items=None,vitals=None,samples=False):
        if profile not in ("on-foot-melee","on-foot-rifle"):
            raise ValueError("Unknown Unity on-foot profile: "+str(profile))
        self.profile=profile
        self.vitals_values=vitals
        # Select only controls observable in on-foot play. The source prefab
        # stores vehicle / glider / ladder / other state branches concurrently.
        # Never draw all states simultaneously or claim the prefab reveals
        # their runtime activation.
        selected=("btn_info","btn_baseUpgrade","btn_Baserank","btn_help",
          "m_kedu","m_map","m_xue","m_jie","m_bodyWater",
          "btn_setting","btn_quckChat","btn_diuShiDongXi","btn_quick_mark",
          "m_bagItems","btn_pack","btn_zhizao","m_JoystackType2",
          "btn_autoRun","btn_fire","btn_up2","btn_crouch2","btn_pa2")
        if profile=="on-foot-rifle":
            selected+=("btn_kaijing","btn_changeBullet")
        excludes={"m_autoRunChoosed","EF_btn_autoRun","btn_crouching2",
          "btn_paing2","btn_jump2","m_jindutiao","m_rechargeEffect",
          "m_PatiziImage","m_openGliderIcon","m_flyEffect",
          "m_addOilIcon","m_StencilMask","m_pack_slider","m_newbee_effect",
          "m_red_info","m_red_baseUpdate"}
        ordered=sorted(selected,key=self.z_key)
        positions={}
        def draw():
            for name in ordered:
                if name=="m_kedu":
                    self.render_compass()
                elif name=="m_map":
                    self.render_map()
                elif name=="m_xue":
                    # Draw all source vital status groups together because their
                    # slider fill values are injected by the game, not the prefab.
                    self.vitals()
                elif name in ("m_jie","m_bodyWater"):
                    continue
                elif name=="m_bagItems":
                    positions.update({"hotbar":self.hotbar(samples=samples,items=items)})
                else:
                    self.node(name,4,exclude=excludes)
        draw()
        assert len(positions["hotbar"])==7
        # A source-drawn frame must not replace a no-op profile with every
        # vehicle and glider component.
        assert 40<=len(self.calls)<250,len(self.calls)
        source_images={pid:c for pid,c in self.source_comps.items()
             if c["type"]=="Image" and c["sprite"]["path_id"]!="0"}
        selected_unique=set(self.drawn_components)
        summary={
          "source":"Last Island LITE 7.3 original Unity battlepanel.ab",
          "render_mode":profile,
          "drawn_operations":len(self.calls),
          "source_image_components_total":len(source_images),
          "source_image_components_drawn_unique":len(selected_unique),
          "source_image_components_not_drawn":
              len(set(source_images)-selected_unique),
          "all_source_sprite_links_resolved":all(
              bool(v.get("resolved_sprite_file")) for v in source_images.values()),
          "source_hierarchy_order":ordered,
          "source_hierarchy_sort_keys":{name:self.z_key(name) for name in ordered},
          "on_foot_runtime_selection_not_proven":True,
          "unsupported_native_unity_features":[
             "dynamic GameObject state machines",
             "Unity UGUI native shader, mask, rect-clipping and full nine-slice mesh",
             "localized TrueType / bitmap fonts and player event-driven text",
             "Unity CanvasScaler safe-area/orientation adaptation",
             "exact map viewport and UI animation/easing"],
          "suppressed_component_counts":dict(self.suppressed),
          "source_pixel_provenance":"Original Unity sprite files keyed by FileID and PathID",
          "special_source_textures":[
            "Map_map Texture2D from original XAPK",
            "BattlePanel_kedu_zw RawImage exact local PPtr"]
        }
        return summary

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("--profile",choices=("on-foot-melee","on-foot-rifle"),
                    default="on-foot-melee")
    ap.add_argument("--state",type=Path)
    ap.add_argument("--assets",type=Path,default=ASSETS)
    ap.add_argument("--output",type=Path,
       default=ASSETS/"complete_source_mapping/hud_v6_prefab_source_order.png")
    ap.add_argument("--demo-items",action="store_true")
    args=ap.parse_args()
    state=json.loads(args.state.read_text(encoding="utf8")) if args.state else {}
    profile=state.get("profile",args.profile)
    hud=HudSourceOrdered(args.assets,
        heading_deg=state.get("heading_deg",0),
        map_uv=state.get("map_uv",[.5,.5]),
        map_window=state.get("map_window",896))
    report=hud.render_ordered(profile=profile,
        samples=args.demo_items,
        items=state.get("items"),vitals=state.get("vitals"))
    args.output.parent.mkdir(parents=True,exist_ok=True)
    rgba=args.output.with_name(args.output.stem+"_transparent.png")
    hud.layer.save(rgba,optimize=True)
    canvas=Image.new("RGBA",hud.canvas,(28,38,43,255))
    canvas.alpha_composite(hud.layer)
    canvas.convert("RGB").save(args.output,optimize=True)
    report["image"]=str(args.output)
    report["transparent_overlay"]=str(rgba)
    report["map_player_uv_is_simulated"]=True
    report["source_drawing_calls"]=hud.calls
    args.output.with_suffix(".json").write_text(
        json.dumps(report,ensure_ascii=False,indent=2),encoding="utf8")
    print("HUD_V6_SOURCE_HIERARCHY",json.dumps(
        {k:v for k,v in report.items() if k not in ("source_drawing_calls",)},
        ensure_ascii=False)[:4400],flush=True)
    print("HUD_V6_IMAGE",str(args.output),flush=True)

if __name__=="__main__":
    main()
