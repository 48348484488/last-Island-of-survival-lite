#pragma once
// Original Last Island LITE 7.3 BagAndBuildPanel sprite renderer.
//
// Pixels come DIRECTLY from assets/uiatlas/bagandbuildpanel.ab and common.ab
// and original tubiao1 item icons. Source m_produce_page/m_produce_list,
// scp_items, and btn_back provide hierarchy/anchor references. The original
// Unity runtime scroll group, localization font and network recipe queue are
// NOT preserved; those portions are functional native approximations.
#include "raylib.h"
#include "native_build_craft.hpp"
#include <algorithm>
#include <string>
namespace LIOS_CRAFT_UI {
struct Art {
    Texture2D button={},back={},slot={},smallSlot={},strip={},selected={},
              plus={},minus={},close={},hammer={},pickaxe={},buildIcon={},
              wood={},stone={},metal={};
    void load(){
        button=LoadTexture("assets/craft_original_create_button.png");
        back=LoadTexture("assets/craft_original_back_button.png");
        slot=LoadTexture("assets/craft_original_item_slot.png");
        smallSlot=LoadTexture("assets/craft_original_item_slot_small.png");
        strip=LoadTexture("assets/craft_original_strip.png");
        selected=LoadTexture("assets/craft_original_selected.png");
        plus=LoadTexture("assets/craft_original_plus.png");
        minus=LoadTexture("assets/craft_original_minus.png");
        close=LoadTexture("assets/craft_original_close.png");
        hammer=LoadTexture("assets/icon_source_construction_hammer.png");
        pickaxe=LoadTexture("assets/icon_source_stone_pickaxe.png");
        buildIcon=LoadTexture("assets/icon_source_construction_category.png");
        wood=LoadTexture("assets/icon_resource_wood.png");
        stone=LoadTexture("assets/icon_resource_stone.png");
        metal=LoadTexture("assets/icon_resource_metal.png");
    }
};
inline Rectangle rect(float l,float t,float w,float h,float scale,float left,float top){
    return {left+l*scale,top+t*scale,w*scale,h*scale};
}
inline bool hit(Vector2 p,float l,float t,float w,float h,float sc,float x,float y){
    return CheckCollisionPointRec(p,rect(l,t,w,h,sc,x,y));
}
inline void sprite(Texture2D tx,Rectangle dest,Color tint=WHITE){
    if(!tx.id)return;
    DrawTexturePro(tx,{0,0,float(tx.width),float(tx.height)},dest,{0,0},0,tint);
}
inline void title(const char* val,float x,float y,int pts,float sc,float left,float top,Color color=RAYWHITE){
    DrawText(val,int(left+x*sc),int(top+y*sc),
        std::max(11,int(pts*sc)),color);
}
static const char* recipeLabel(int r){
    static const char* names[]={"FERRAMENTA DE CONSTRUCAO","MARTELO DE CONSTRUCAO",
                                 "PICARETA DE PEDRA"};
    return names[std::max(0,std::min(r,2))];
}
inline void render(const Art &art, const LIOS_BUILD::System &sys,
                   const LIOS_BUILD::Resources &r, int selection,
                   int quantity,const std::string &status,
                   float sc,float ox,float oy){
    const auto b=[&](float x,float y,float w,float h){
        return rect(x,y,w,h,sc,ox,oy);
    };
    // Native dimming is retained because the original m_bag_bg shader is a
    // scene-space quad, not a UGUI image in the source prefab.
    DrawRectangle(int(ox),int(oy),int(1920*sc),int(1080*sc),
                  {6,11,12,230});
    // Real original section-strip + modular slot border textures.
    sprite(art.strip,b(185,95,1540,91));
    title("FABRICACAO",235,114,43,sc,ox,oy);
    sprite(art.back,b(1654,0,266,86));
    title("VOLTAR",1748,24,28,sc,ox,oy);
    title("RECEITAS",250,216,29,sc,ox,oy);
    title("DETALHES",1110,216,29,sc,ox,oy);
    // Source scp_items is at (239,208) 735x641. The real Unity list
    // populates item cards dynamically; here its three available recipes
    // are rendered using the original prefab com_item_k_3/com_item_k_4.
    for(int i=0;i<3;i++){
        float x=252+203*i,y=280;
        sprite(art.slot,b(x,y,160,160));
        if(i==selection) sprite(art.selected,b(x+15,y+14,130,130));
        Texture2D item=i==0?art.buildIcon:i==1?art.hammer:art.pickaxe;
        sprite(item,b(x+35,y+30,92,92));
        title(i==0?"CONSTRUIR":i==1?"MARTELO":"PICARETA",
              x+5,y+176,22,sc,ox,oy);
        bool owned=sys.owns(static_cast<LIOS_BUILD::RecipeType>(i));
        if(owned)title("FABRICADO",x+15,y+207,20,sc,ox,oy,LIME);
    }
    // Original source-selected item, cost and queue widgets.
    sprite(art.slot,b(1130,260,195,195));
    Texture2D selected=selection==0?art.buildIcon:
                       selection==1?art.hammer:art.pickaxe;
    sprite(selected,b(1175,302,100,100));
    title(recipeLabel(selection),1330,285,30,sc,ox,oy,WHITE);
    const auto cost=LIOS_BUILD::CRAFT_COSTS[selection];
    struct Mat {Texture2D icon;int available,needed;};
    Mat mat[3]={{art.wood,r.wood,cost.wood},{art.stone,r.stone,cost.stone},
                 {art.metal,r.metal,cost.metal}};
    for(int i=0;i<3;i++){
        if(mat[i].needed<=0)continue;
        float y=395+i*76;
        sprite(mat[i].icon,b(1320,y,61,61));
        title(TextFormat("%d / %d",mat[i].available,mat[i].needed*quantity),
            1403,y+15,27,sc,ox,oy,
            mat[i].available>=mat[i].needed*quantity?RAYWHITE:ORANGE);
    }
    title("QUANTIDADE",1130,680,25,sc,ox,oy);
    sprite(art.minus,b(1360,663,72,72));
    title(TextFormat("%d",quantity),1467,684,31,sc,ox,oy);
    sprite(art.plus,b(1547,663,72,72));
    sprite(art.button,b(1340,760,334,84));
    title(sys.owns(static_cast<LIOS_BUILD::RecipeType>(selection))?
          "FABRICADO":"FABRICAR",1397,785,31,sc,ox,oy,WHITE);
    title(status.c_str(),1100,886,23,sc,ox,oy,YELLOW);
    if(!sys.queue.empty()){
        const auto &q=sys.queue.front();
        title(TextFormat("PRODUZINDO: %s  %.0f%%",
            recipeLabel(q.type),100.f*std::min(1.f,q.elapsed/q.duration)),
            1110,944,23,sc,ox,oy,SKYBLUE);
        sprite(art.close,b(1600,930,66,66));
    }
}
} // namespace LIOS_CRAFT_UI
