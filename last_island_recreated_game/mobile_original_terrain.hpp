#pragma once
// UnityPy OBJ exporter emits Z-up coordinates. Original Unity scene uses
// an Rx(-90 deg) transform: (x,y,z) -> (x,z,-y). This tile is a bounded
// first step toward a real world, NOT a full scene/physics reconstruction.
#include "raylib.h"
#include "raymath.h"
#include <cmath>
#include <algorithm>

namespace LIOS_TERRAIN {
struct Tile {
    Model model{};
    Texture2D texture{};
    bool ready=false;

    void load() {
        model=LoadModel("assets/original_world/a-57_mesh0.obj");
        // The Unity source image is a splat/material mask, not a diffuse
        // color texture. Its RGB contains layer weights and alpha is NOT
        // opacity (72.1% zero alpha). Render a clearly provisional natural
        // palette from those weights until real Unity terrain layers/shader
        // are recovered; do not show neon splatmap colors as ground albedo.
        Image source=LoadImage("assets/original_world/a-57_tex0.png");
        if(source.data){
            ImageFormat(&source,PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
            Color* pixels=static_cast<Color*>(source.data);
            for(int i=0;i<source.width*source.height;i++){
                const Color mask=pixels[i];
                const float r=float(mask.r),g=float(mask.g),b=float(mask.b);
                const float total=r+g+b;
                if(total<4.f){
                    pixels[i]={74,105,55,255};
                }else{
                    const float inv=1.f/total;
                    pixels[i]={
                        static_cast<unsigned char>((127.f*r+71.f*g+115.f*b)*inv),
                        static_cast<unsigned char>((106.f*r+117.f*g+115.f*b)*inv),
                        static_cast<unsigned char>((82.f*r+57.f*g+105.f*b)*inv),
                        255
                    };
                }
            }
            texture=LoadTextureFromImage(source);
            UnloadImage(source);
            TraceLog(LOG_INFO,"LIOS_TERRAIN_MASK_MATERIAL_PREVIEW alpha_not_opacity");
        }
        if(model.meshCount<=0 || texture.id==0){
            TraceLog(LOG_WARNING,"LIOS_TERRAIN_SOURCE_TILE_MISSING mesh=%d texture=%u",
                     model.meshCount,texture.id);
            return;
        }
        model.transform=MatrixRotateX(-PI/2.0f);
        for(int i=0;i<model.materialCount;i++)
            SetMaterialTexture(&model.materials[i],MATERIAL_MAP_DIFFUSE,texture);
        ready=true;
        TraceLog(LOG_INFO,"LIOS_TERRAIN_SOURCE_TILE_READY meshes=%d material=%u",
                 model.meshCount,texture.id);
    }

    // A vertical ray finds the highest intersected triangle. This is an
    // approximation for local walking, not Unity's original collision mesh.
    bool sample(float x,float z,float& height) const {
        if(!ready || !std::isfinite(x) || !std::isfinite(z) ||
           x < -200.f || x > 200.f || z < -200.f || z > 200.f)
            return false;
        Ray ray={{x,128.f,z},{0.f,-1.f,0.f}};
        bool found=false;
        float top=-100000.f;
        for(int i=0;i<model.meshCount;i++){
            const RayCollision hit=GetRayCollisionMesh(ray,model.meshes[i],model.transform);
            if(hit.hit && std::isfinite(hit.point.y) && (!found || hit.point.y>top)){
                top=hit.point.y;
                found=true;
            }
        }
        if(found)height=top;
        return found;
    }

    float ground(float x,float z) const {
        float h=0.f;
        return sample(x,z,h)?h:0.f;
    }
    void draw() const {
        if(ready)DrawModel(model,{0.f,0.f,0.f},1.f,WHITE);
    }
    void unload(){
        if(model.meshCount>0)UnloadModel(model);
        if(texture.id>0)UnloadTexture(texture);
        ready=false;
    }
};
} // namespace LIOS_TERRAIN
