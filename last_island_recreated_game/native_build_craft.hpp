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
#include <deque>

namespace LIOS_BUILD {
enum PieceType {Foundation=0, Wall=1, Doorway=2, Roof=3, Storage=4, Campfire=5, Stair=6, Window=7, Obstacle=8, PieceCount=9};
enum RecipeType {Blueprint=0, Hammer=1, StonePickaxe=2, RecipeCount=3};
struct Resources {
    int wood,stone,metal,sulfur;
    Resources(int w=0,int s=0,int m=0,int u=0):
        wood(w),stone(s),metal(m),sulfur(u){}
};
struct Cost {int wood,stone,metal;};
struct Piece {
    PieceType type;
    int gx,gz,turn;
    Piece(PieceType t=Foundation,int x=0,int z=0,int r=0):
        type(t),gx(x),gz(z),turn(r){}
};
// Unity SC.UI.BattlePanel.OnClick* opens m_buildChilds (source original):
// BuildPartType: Foundation=2, Wall=3, Stair=4, Window=6,
// Obstacle=7, floor=8. Buttons are categories, NOT direct placement.
// The subset below maps the only parts implemented by the native engine.
inline int partForOriginalCategory(int buttonIndex,int subitemIndex){
    if(subitemIndex<0)return -1;
    switch(buttonIndex){
    case 2:return subitemIndex==0?Foundation:-1;
    case 3:return subitemIndex==0?Wall:-1; // subitem 1 is half-wall
    case 4:return subitemIndex==0?Roof:-1;
    case 5:return subitemIndex==0?Stair:-1;
    case 6:return subitemIndex==0?Window:subitemIndex==3?Doorway:-1;
    case 7:return subitemIndex==0?Obstacle:-1;
    default:return -1;
    }
}
static constexpr Cost BUILD_COSTS[PieceCount]={
    {60,20,0}, // Foundation
    {50,0,0},  // Wall
    {40,15,0}, // Door frame
    {45,5,0},  // Roof
    {55,0,20}, // Storage
    {20,40,0}, // Campfire
    {80,20,0}, // Stairs
    {50,15,0}, // Window frame
    {35,30,0}  // Barricade/obstacle
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
    int craftedCount[RecipeCount]={0,0,0};
    bool building=false;
    bool freePlacement=false;
    int selected=0,orientation=0;
    struct CraftOrder {RecipeType type;float elapsed,duration;};
    std::deque<CraftOrder> queue;
    std::vector<Piece> savedPlan;
    static constexpr unsigned MaxCraftOrders=8;
    // Provisional offline timers (source DrawingCfg.needTime not yet mapped).
    static float recipeDuration(RecipeType r){
        return r==Blueprint?2.f:r==Hammer?4.f:5.f;
    }
    bool owns(RecipeType type)const{
        if(type<Blueprint||type>=RecipeCount)return false;
        return type==Blueprint?blueprint:type==Hammer?hammer:pickaxe;
    }
    bool requestCraft(RecipeType type,Resources &r,int quantity=1){
        if(type<Blueprint||type>=RecipeCount||quantity<1||quantity>8||
           queue.size()+unsigned(quantity)>MaxCraftOrders)return false;
        const Cost c=CRAFT_COSTS[type];
        if(r.wood<c.wood*quantity||r.stone<c.stone*quantity||
           r.metal<c.metal*quantity)return false;
        for(int i=0;i<quantity;i++){
            if(!spend(r,c))return false;
            queue.push_back({type,0.f,recipeDuration(type)});
        }
        return true;
    }
    bool cancelCraft(unsigned index,Resources &r){
        if(index>=queue.size())return false;
        const Cost cost=CRAFT_COSTS[queue[index].type];
        r.wood+=cost.wood;r.stone+=cost.stone;r.metal+=cost.metal;
        queue.erase(queue.begin()+index);
        return true;
    }
    void tickCraft(float dt){
        if(queue.empty()||dt<=0.f)return;
        queue.front().elapsed+=std::min(dt,1.f);
        if(queue.front().elapsed>=queue.front().duration){
            const RecipeType type=queue.front().type;
            if(type==Blueprint)blueprint=true;
            if(type==Hammer)hammer=true;
            if(type==StonePickaxe)pickaxe=true;
            craftedCount[type]++;
            queue.pop_front();
        }
    }
    bool saveCurrentBlueprint(){
        if(!blueprint||placed.empty())return false;
        savedPlan=placed;return true;
    }
    static constexpr int MaxPlaced=300;
    bool craft(RecipeType type,Resources &r){
        if(type<Blueprint||type>=RecipeCount)return false;
        bool *owned=type==Blueprint?&blueprint:type==Hammer?&hammer:&pickaxe;
        if(*owned||!spend(r,CRAFT_COSTS[type]))return false;
        *owned=true;craftedCount[type]++;return true;
    }
    bool occupies(PieceType type,int gx,int gz,int rot)const{
        for(const auto &p:placed){
            if(p.gx!=gx||p.gz!=gz)continue;
            if(p.type==type&&(((type!=Wall&&type!=Doorway&&type!=Window)||turn(p.turn)==turn(rot))))return true;
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
        if(type==Campfire||type==Obstacle){
            if(occupies(type,gx,gz,rot))return false;
            if(dx*dx+dz*dz<2.f)return false;
            return !foundationAt(gx,gz);
        }
        if(!freePlacement&&!foundationAt(gx,gz))return false;
        if(type==Wall||type==Doorway||type==Window){
            // Same edge cannot be both a wall and a door opening.
            for(const auto &p:placed)
                if(p.gx==gx&&p.gz==gz&&turn(p.turn)==turn(rot)&&
                    (p.type==Wall||p.type==Doorway||p.type==Window))return false;
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
            if(p.type!=Wall&&p.type!=Window&&p.type!=Obstacle)continue;
            if(p.type==Obstacle){
                if(std::abs(x-world(p.gx))<.75f&&std::abs(z-world(p.gz))<.75f)return true;
                continue;
            }
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
