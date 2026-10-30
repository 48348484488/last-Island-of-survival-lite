#pragma once
// World-space UnityPy OBJ tiles are Z-up. Their native Y-up orientation is
// (x,y,z) -> (x,z,-y). These five sectors are extracted geometry, not proof
// of original Unity material, collision, or scene behavior.
#include "raylib.h"
#include "raymath.h"
#include <cmath>
#include <algorithm>

namespace LIOS_TERRAIN {
struct SourceTile {
    const char* meshPath;
    const char* maskPath;
    float minX, maxX, minZ, maxZ;
    Model model{};
    Texture2D texture{};
    bool ready=false;

    // raymob's Android NDK toolchain does not treat a struct with default
    // member initializers as a six-field aggregate in its C++ mode.
    SourceTile(const char* mesh,const char* mask,float x0,float x1,float z0,float z1)
        :meshPath(mesh),maskPath(mask),minX(x0),maxX(x1),minZ(z0),maxZ(z1){}

    void load() {
        model=LoadModel(meshPath);
        // Unity source RGBA is a packed material/splat mask. The alpha channel
        // is NOT transparency. Use an opaque, explicitly provisional palette
        // until source Unity layer albedos and shader are verified.
        Image source=LoadImage(maskPath);
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
            TraceLog(LOG_WARNING,"LIOS_TERRAIN_SOURCE_TILE_MISSING path=%s mesh=%d texture=%u",
                     meshPath,model.meshCount,texture.id);
            return;
        }
        model.transform=MatrixRotateX(-PI/2.0f);
        for(int i=0;i<model.materialCount;i++)
            SetMaterialTexture(&model.materials[i],MATERIAL_MAP_DIFFUSE,texture);
        ready=true;
        TraceLog(LOG_INFO,"LIOS_TERRAIN_SOURCE_TILE_READY meshes=%d material=%u path=%s",
                 model.meshCount,texture.id,meshPath);
    }

    bool sample(float x,float z,float& height) const {
        if(!ready || x<minX-0.05f || x>maxX+0.05f ||
           z<minZ-0.05f || z>maxZ+0.05f) return false;
        const Ray ray={{x,512.f,z},{0.f,-1.f,0.f}};
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

    void draw() const {
        if(ready)DrawModel(model,{0.f,0.f,0.f},1.f,WHITE);
    }

    void unload(){
        if(model.meshCount>0)UnloadModel(model);
        if(texture.id>0)UnloadTexture(texture);
        model={};
        texture={};
        ready=false;
    }
};

struct Tile {
    // Neighbors selected from the 365-record Unity source world catalog by
    // matching ~400m boundaries and >=80% shared-edge coverage. The map
    // contains large non-terrain meshes: do not load arbitrary bbox overlaps.
    SourceTile sectors[5]={
        {"assets/original_world/a-57_mesh0.obj","assets/original_world/a-57_tex0.png",
         -200.f,200.f,-200.f,200.f},
        {"assets/original_world/a-56_mesh1.obj","assets/original_world/a-56_tex0.png",
         -200.f,200.001f,199.218f,600.f},
        {"assets/original_world/d-01_mesh2.obj","assets/original_world/d-01_tex1.png",
         -200.f,200.f,-600.f,-199.999f},
        {"assets/original_world/c-07_mesh0.obj","assets/original_world/c-07_tex0.png",
         200.f,600.f,-200.f,200.f},
        {"assets/original_world/b-50_mesh2.obj","assets/original_world/b-50_tex2.png",
         -600.001f,-200.f,-200.001f,200.f}
    };
    bool ready=false;

    void load(){
        int loaded=0;
        for(auto& sector:sectors){
            sector.load();
            if(sector.ready)loaded++;
        }
        // Keep spawn availability independent from optional adjacent sectors.
        ready=sectors[0].ready;
        TraceLog(LOG_INFO,"LIOS_TERRAIN_SECTORS_READY count=%d total=5",loaded);
    }

    // Highest vertical intersection is a temporary walking approximation,
    // not Unity's actual collision mesh or navigation rules.
    bool sample(float x,float z,float& height) const {
        if(!ready || !std::isfinite(x) || !std::isfinite(z))return false;
        bool found=false;
        float top=-100000.f;
        for(const auto& sector:sectors){
            float candidate=0.f;
            if(sector.sample(x,z,candidate) && (!found || candidate>top)){
                top=candidate;
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

    void draw(Vector3 observer) const {
        // Distance to each sector's XZ bounding rectangle, rather than its
        // center, avoids discarding a sector when crossing a shared edge.
        // At spawn a 520m radius submits all five large OBJ meshes every
        // frame, even though neighboring tiles are outside the visible area.
        // Keep a 175m border prefetch margin for seamless traversal while
        // skipping distant sectors on mobile GPUs. This is distance culling,
        // not frustum culling; benchmark FPS on device/emulator separately.
        constexpr float drawRadius=175.f;
        unsigned int drawn=0;
        for(const auto& sector:sectors){
            if(!sector.ready)continue;
            const float dx=std::max(sector.minX-observer.x,
                                    std::max(0.f,observer.x-sector.maxX));
            const float dz=std::max(sector.minZ-observer.z,
                                    std::max(0.f,observer.z-sector.maxZ));
            if(dx*dx+dz*dz<=drawRadius*drawRadius){
                sector.draw();
                ++drawn;
            }
        }
        // Observed draw-call count for correlating FPS with tile culling.
        // This measures submitted sectors, not GPU time or visible pixels.
        static unsigned int sampledFrames=0;
        if(++sampledFrames%60==0)
            TraceLog(LOG_INFO,"LIOS_TERRAIN_DRAW sectors=%u radius_m=%.1f",
                     drawn,drawRadius);
    }

    void unload(){
        for(auto& sector:sectors)sector.unload();
        ready=false;
    }
};
} // namespace LIOS_TERRAIN
