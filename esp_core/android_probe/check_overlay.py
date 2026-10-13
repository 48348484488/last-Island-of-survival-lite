#!/usr/bin/env python3
"""Verify a controlled, transparent overlay appears over the launcher and disappears."""
from pathlib import Path
from PIL import Image
import sys

def count_markers(path):
    with Image.open(path) as im:
        im=im.convert("RGB")
        w,h=im.size
        x1,x2=int(w*0.32),int(w*0.68)
        y1,y2=int(h*0.35),int(h*0.72)
        hits=0
        for y in range(y1,y2):
            for x in range(x1,x2):
                r,g,b=im.getpixel((x,y))
                if 15<=r<=80 and g>=220 and 65<=b<=155 and g>r*2 and g>b*1.3:
                    hits+=1
    return hits

def main():
    if len(sys.argv)!=3:raise SystemExit("Need overlay-on and overlay-off screenshots")
    on=count_markers(Path(sys.argv[1]))
    off=count_markers(Path(sys.argv[2]))
    print(f"OVERLAY_ON_MARKER_PIXELS={on}")
    print(f"OVERLAY_REMOVED_MARKER_PIXELS={off}")
    if on<500:raise SystemExit("FAIL: transparent overlay rectangle was not visible")
    if off>max(10,int(on*0.02)):raise SystemExit("FAIL: overlay marker persisted after service was stopped")
    print("PASS: transparent diagnostic overlay visible, then completely removed")

if __name__=="__main__":main()
