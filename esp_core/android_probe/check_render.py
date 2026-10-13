#!/usr/bin/env python3
"""Pixels-only check of native-to-Java Canvas smoke test screenshots.

The isolated app uses a synthetic test rectangle. This is NOT an in-game ESP.
"""
from pathlib import Path
import sys
from PIL import Image

def green_bounds(path):
    with Image.open(path) as img:
        rgb=img.convert("RGB")
        width,height=rgb.size
        hits=0
        minx=width;miny=height;maxx=-1;maxy=-1
        for y in range(height):
            for x in range(width):
                r,g,b=rgb.getpixel((x,y))
                if g>190 and r<90 and b<160 and g>r*2 and g>b*1.5:
                    hits+=1
                    minx=min(minx,x);maxx=max(maxx,x)
                    miny=min(miny,y);maxy=max(maxy,y)
        return {"pixels":hits,"width":width,"height":height,
                "bounds":[minx,miny,maxx,maxy] if hits else None}

if __name__=="__main__":
    colored=green_bounds(Path(sys.argv[1]))
    empty=green_bounds(Path(sys.argv[2]))
    print("PROJECTION_RECT",colored)
    print("EMPTY_FRAME",empty)
    if not (300 <= colored["pixels"] < 30000):
        raise SystemExit("FAIL: synthetic native rectangle not visible in diagnostic app")
    if empty["pixels"]>0:
        raise SystemExit("FAIL: zero-sample frame showed a green marker")
    a,b,c,d=colored["bounds"]
    if not (a<colored["width"]*0.6 and c>colored["width"]*0.4 and
            b<colored["height"]*0.75 and d>colored["height"]*0.25):
        raise SystemExit("FAIL: native rectangle is not near viewport center")
    print("PASS: native-projected box rendered in Android Canvas; no data => no boxes")
