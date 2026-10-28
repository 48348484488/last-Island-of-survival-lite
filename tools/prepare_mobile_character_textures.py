#!/usr/bin/env python3
"""Adapt original Unity texture channels for the native Raylib material shader.

Unity's original character albedo PNG *alpha* packs non-opacity material
properties (smoothness), which Raylib's default alpha blending interprets
as transparency. E.g., the chest armor has alpha ~65 throughout the image.
Original PNGs remain untouched. Build opaque render copies for torso, pants,
skin, shoes and head. Hair uses alpha cutout to retain hair shape without
see-through ghosting in the standard mobile renderer.
"""
import argparse
from pathlib import Path
from PIL import Image

OPAQUE=(
  "man_clothes_chushijuese_01_ba.png",
  "man_clothes_yf_0_ba.png",
  "man_trousers_chushijuese_01_ba.png",
  "man_shoes_chushijuese_01_ba.png",
  "man_head_chushijuese_01_ba.png",
)
HAIR="man_hair_chushijuese_01_ba.png"
def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("source",type=Path)
    ap.add_argument("destination",type=Path)
    args=ap.parse_args()
    args.destination.mkdir(parents=True,exist_ok=True)
    for name in (*OPAQUE,HAIR):
        src=args.source/name
        if not src.is_file():raise FileNotFoundError(src)
        with Image.open(src) as orig:
            rgba=orig.convert("RGBA")
        before=rgba.getchannel("A")
        if name==HAIR:
            # Original Unity hair uses cutout. Raylib's default transparent
            # blending would mix many overlapping transparent strands.
            rgba.putalpha(before.point(lambda x: 255 if x>=128 else 0))
        else:
            rgba.putalpha(Image.new("L",rgba.size,255))
        out=args.destination/("native_"+name)
        rgba.save(out,optimize=True)
        result=rgba.getchannel("A").getextrema()
        assert result==(0,255) if name==HAIR else result==(255,255), (name,result)
        print("LIOS_NATIVE_MATERIAL_ALPHA",name,"source_alpha_range",before.getextrema(),
              "render_alpha_range",result,"bytes",out.stat().st_size,flush=True)
if __name__=="__main__":main()
