"""Faithful Unity UGUI Image basic raster operations used by the HUD renderer.

Image.Type=Simple/Sliced/Filled, Image.preserveAspect, FillMethod Horizontal/Vertical
and the exact source FillAmount/FillOrigin. Radial fill is an explicitly
documented Pillow geometric approximation to Unity's radial mesh, not claimed
pixel identical. No textures are generated: only masks on original Sprites.
"""
import math
from PIL import Image, ImageChops, ImageDraw

def preserve_aspect(sprite, size):
    """Reproduce centered Unity Image preserveAspect geometry in its RectTransform."""
    w,h=map(int,size)
    if w<=0 or h<=0:raise ValueError(size)
    srcw,srch=sprite.size
    if srcw<=0 or srch<=0:raise ValueError(sprite.size)
    factor=min(w/srcw,h/srch)
    rw=max(1,min(w,int(round(srcw*factor))))
    rh=max(1,min(h,int(round(srch*factor))))
    out=Image.new("RGBA",(w,h),(0,0,0,0))
    img=sprite.convert("RGBA").resize((rw,rh),Image.Resampling.LANCZOS)
    out.paste(img,((w-rw)//2,(h-rh)//2))
    return out

def _radial_mask(size,amount,method,origin,clockwise):
    """Approximate Unity radial mesh using a deterministic alpha mask.

    Source supports 90, 180 and 360 degree methods; raster mask edges will
    not be identical to native Unity's triangle clipping and AA.
    """
    w,h=size
    # Unity radial origins: 0=Bottom,1=Right,2=Top,3=Left for 360.
    if method==4:
        n=4
    elif method==3:n=2
    else:n=1
    origin=int(origin)%n
    cx=(w-1)/2;cy=(h-1)/2
    # raster screen y grows down. Reference angular zero points to bottom.
    angles=[math.pi/2,0,-math.pi/2,math.pi]
    base=angles[origin if method==4 else origin*(4//n)]
    sweep=math.pi*n/2
    amount=min(1,max(0,float(amount)))
    img=Image.new("L",(w,h),0)
    pixels=img.load()
    for y in range(h):
        for x in range(w):
            if x==cx and y==cy:
                pixels[x,y]=255
                continue
            direction=math.atan2(y-cy,x-cx)
            delta=((base-direction) if clockwise else (direction-base))%(2*math.pi)
            if delta<=sweep*amount+1e-12:
                pixels[x,y]=255
    return img

def apply_fill(image,amount,method=0,origin=0,clockwise=True):
    """Apply original Unity Image.m_Fill* direction to source Sprite transparency."""
    image=image.convert("RGBA")
    w,h=image.size
    value=float(amount)
    if not 0<=value<=1:raise ValueError("Unity FillAmount not 0..1: "+str(amount))
    if value==1:return image
    if value==0:return Image.new("RGBA",(w,h))
    method=int(method or 0)
    origin=int(origin or 0)
    if method==0:
        # Image.FillMethod.Horizontal: Left=0, Right=1
        if origin not in (0,1):raise ValueError(origin)
        mask=Image.new("L",(w,h),0)
        pixels=int(round(w*value))
        if origin==0:
            ImageDraw.Draw(mask).rectangle((0,0,max(0,pixels-1),h-1),fill=255)
        else:
            ImageDraw.Draw(mask).rectangle((max(0,w-pixels),0,w-1,h-1),fill=255)
    elif method==1:
        # Image.FillMethod.Vertical: Bottom=0, Top=1
        if origin not in (0,1):raise ValueError(origin)
        mask=Image.new("L",(w,h),0)
        pixels=int(round(h*value))
        if origin==0:
            ImageDraw.Draw(mask).rectangle((0,max(0,h-pixels),w-1,h-1),fill=255)
        else:
            ImageDraw.Draw(mask).rectangle((0,0,w-1,max(0,pixels-1)),fill=255)
    elif method in (2,3,4):
        mask=_radial_mask((w,h),value,method,origin,clockwise)
    else:
        raise ValueError("Unknown original Unity Image.FillMethod: "+str(method))
    image.putalpha(ImageChops.multiply(image.getchannel("A"),mask))
    return image
