#pragma once
// Native offline survival building prototype. The ORIGINAL Unity client has
// BuildState/BuilderBehaviour and a 6000-unit world, but its complete placement
// protocol and cfg recipe costs are not ported. Thus prices below are
// explicitly provisional, NOT represented as original item table values.
// Pure C++ gameplay rules, independently testable without Raylib or Android.
#include <vector>
#include <cmath>
#include <algorithm>
#include <cstdint>

namespace LIOS_BUILD {
enum PieceType {Foundation=0, Wall=1, Doorway=2, Roof=3, Storage=4, Campfire=5, PieceCount=6};
enum RecipeType {Blueprint=0, Hammer=1, StonePickaxe=2, RecipeCount=3};
struct Resources {int wood=0,stone=0,metal=0,sulfur=0;};
struct Cost {int wood,stone,metal;};
struct Piece {
    PieceType type=Foundation;
    int gx=0,gz=0,turn=0;
};
static constexpr Cost BUILD_COSTS[PieceCount]={
    {60,20,0}, // Foundation
    {50,0,0},  // Wall
    {40,15,0}, // Door frame
    {45,5,0},  // Roof
    {55,0,20}, // Storage
    {20,40,0}  // Campfire
};
static constexpr Cost CRAFT_COSTS[RecipeCount]={
    {20,20,0}, // Blueprint
    {30,5,10}, // Hammer
    {25,45,0}  // Stone pickaxe
};
inline bool canPay(const Resources &r,const Cost &c){
    return r.wood>=c.wood && r.stone>=c.stone && r.metal>=c.metal;
}
inline bool spend(Resources &r,const Cost &c){
    if(!canPay(r,c))return false;
    r.wood-=c.wood;r.stone-=c.stone;r.metal-=c.metal;
    return true;
}
inline int grid(float position){return int(std::round(position/3.f));}
inline float world(int cell){return float(cell)*3.f;}
inline int turn(int rotation){return (rotation%4+4)%4;}
class System {
public:
    std::vector<Piece> placed;
    bool blueprint=false,hammer=false,pickaxe=false;
    bool building=false;
    int selected=0,orientation=0;
    static constexpr int MaxPlaced=300;
    bool craft(RecipeType type,Resources &r){
        if(type<Blueprint||type>=RecipeCount)return false;
        bool *owned=type==Blueprint?&blueprint:type==Hammer?&hammer:&pickaxe;
        if(*owned||!spend(r,CRAFT_COSTS[type]))return false;
        *owned=true;return true;
    }
    bool occupies(PieceType type,int gx,int gz,int rot)const{
        for(const auto &p:placed){
            if(p.gx!=gx||p.gz!=gz)continue;
            if(p.type==type&&(((type!=Wall&&type!=Doorway)||turn(p.turn)==turn(rot))))return true;
        }
        return false;
    }
    bool foundationAt(int gx,int gz)const {
        for(const auto &p:placed)
            if(p.gx==gx&&p.gz==gz&&p.type==Foundation)return true;
        return false;
    }
    bool valid(PieceType type,int gx,int gz,int rot,float playerX,float playerZ)const{
        if(!blueprint||type<Foundation||type>=PieceCount||
           placed.size()>=MaxPlaced||std::abs(gx)>1000||std::abs(gz)>1000)return false;
        float dx=world(gx)-playerX,dz=world(gz)-playerZ;
        if(dx*dx+dz*dz>64.f)return false; // 8 metres max placement range
        if(type==Foundation) {
            if(occupies(Foundation,gx,gz,rot))return false;
            // No foundation directly through player's feet.
            if(dx*dx+dz*dz<1.f)return false;
            return true;
        }
        if(type==Campfire){
            if(occupies(Campfire,gx,gz,rot))return false;
            if(dx*dx+dz*dz<2.f)return false;
            return !foundationAt(gx,gz);
        }
        if(!foundationAt(gx,gz))return false;
        if(type==Wall||type==Doorway){
            // Same edge cannot be both a wall and a door opening.
            for(const auto &p:placed)
                if(p.gx==gx&&p.gz==gz&&turn(p.turn)==turn(rot)&&
                    (p.type==Wall||p.type==Doorway))return false;
        }else if(occupies(type,gx,gz,rot))return false;
        return true;
    }
    bool place(PieceType type,int gx,int gz,int rot,float playerX,float playerZ,Resources &r){
        if(!valid(type,gx,gz,rot,playerX,playerZ))return false;
        if(!spend(r,BUILD_COSTS[type]))return false;
        placed.push_back({type,gx,gz,turn(rot)});
        return true;
    }
    bool blocks(float x,float z)const{
        for(const auto &p:placed){
            if(p.type!=Wall)continue; // Doorway has a walkable opening.
            float dx=x-world(p.gx),dz=z-world(p.gz);
            int rot=turn(p.turn);
            bool alongZ=(rot%2)==0;
            float wallCoord=(rot==0?-1.5f:rot==1?1.5f:rot==2?1.5f:-1.5f);
            float normal=alongZ?dz:dx;
            float tangent=alongZ?dx:dz;
            if(std::abs(normal-wallCoord)<.38f&&std::abs(tangent)<1.42f)return true;
        }
        return false;
    }
};
} // namespace LIOS_BUILD
