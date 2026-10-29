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
// Tab values and nesting copied from SC.UI.BagProducePage.OnInit in the
// original 7.3 Assembly-CSharp source; coordinates are still native layout.
struct Tab {int id;const char* label;};
inline constexpr Tab BigTabs[]={{1,"BASICO"},{12,"ITENS"},{4,"ESTRUTURAS"},
                                {13,"COMBATE"},{14,"VEICULOS"}};
inline constexpr Tab BasicTabs[]={{1,"GERAL"}};
inline constexpr Tab ItemTabs[]={{2,"FERRAMENTAS"},{5,"COZINHA"},
                                 {6,"MEDICINA"},{3,"MATERIAIS"}};
inline constexpr Tab StructureTabs[]={{4,"ESTRUTURAS"},{16,"ELETRICA"}};
inline constexpr Tab CombatTabs[]={{7,"EQUIPAMENTO"},{8,"ARMAS"},
                                   {9,"MUNICAO"},{10,"ARMADILHAS"}};
inline constexpr Tab VehicleTabs[]={{15,"VEICULOS"}};
inline int subTabCount(int big){
    switch(big){case 1:case 14:return 1;case 4:return 2;
                case 12:case 13:return 4;default:return 0;}
}
inline Tab subTab(int big,int at){
    if(at<0||at>=subTabCount(big))return {-1,""};
    switch(big){case 1:return BasicTabs[at];case 12:return ItemTabs[at];
                case 4:return StructureTabs[at];case 13:return CombatTabs[at];
                case 14:return VehicleTabs[at];default:return {-1,""};}
}
inline int firstSubTab(int big){return subTab(big,0).id;}
// These THREE demo recipes are not DrawingCfg-derived. Never display them
// as an authentic original recipe catalog or fill other tabs with fake items.
inline int visibleRecipe(int big,int small,int slot){
    if(big==1&&small==1&&slot==0)return 0;
    if(big==12&&small==2&&slot>=0&&slot<2)return slot+1;
    return -1;
}
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
                   int quantity,int bigTab,int smallTab,int selectedOrder,
                   const std::string &status,
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
    // Original tab hierarchy from BagProducePage.OnInit, not inferred
    // from marketing screenshots. Positions remain provisional until the
    // exact Unity prefab RectTransform export is integrated.
    for(int i=0;i<5;i++){
        float x=220.f+i*294.f;
        Rectangle tab=b(x,179,276,51);
        DrawRectangleRec(tab,bigTab==BigTabs[i].id?
            Color{74,88,85,238}:Color{30,39,41,220});
        DrawRectangleLinesEx(tab,sc*2,Color{152,161,149,220});
        title(BigTabs[i].label,x+15,188,25,sc,ox,oy);
    }
    for(int i=0;i<subTabCount(bigTab);i++){
        float x=232.f+i*198.f;
        const Tab sub=subTab(bigTab,i);
        DrawRectangleRec(b(x,244,189,43),
            sub.id==smallTab?Color{77,97,87,235}:Color{26,33,38,220});
        title(sub.label,x+8,253,20,sc,ox,oy);
    }
    title("RECEITAS",250,305,27,sc,ox,oy);
    title("DETALHES",1110,305,27,sc,ox,oy);
    int recipesOnTab=0;
    for(int slot=0;slot<3;slot++){
        int i=visibleRecipe(bigTab,smallTab,slot);
        if(i<0)continue;
        ++recipesOnTab;
        float x=252+203*slot,y=365;
        sprite(art.slot,b(x,y,160,160));
        if(i==selection)sprite(art.selected,b(x+15,y+14,130,130));
        Texture2D item=i==0?art.buildIcon:i==1?art.hammer:art.pickaxe;
        sprite(item,b(x+35,y+30,92,92));
        title(i==0?"CONSTRUIR":i==1?"MARTELO":"PICARETA",
              x+5,y+176,22,sc,ox,oy);
        if(sys.owns(static_cast<LIOS_BUILD::RecipeType>(i)))
            title("FABRICADO",x+15,y+207,20,sc,ox,oy,LIME);
    }
    if(!recipesOnTab){
        title("RECEITAS ORIGINAIS AINDA NAO PORTADAS",
              254,412,22,sc,ox,oy,GRAY);
    }else title("RECEITAS DE TESTE OFFLINE (CFG PENDENTE)",
                254,641,20,sc,ox,oy,GRAY);
    if(selection<0){
        title("SELECIONE UMA RECEITA DISPONIVEL",
              1110,390,25,sc,ox,oy,GRAY);
    } else {
    // Original source-selected item, cost and queue widgets.
    sprite(art.slot,b(1130,348,185,185));
    Texture2D selected=selection==0?art.buildIcon:
                       selection==1?art.hammer:art.pickaxe;
    sprite(selected,b(1175,387,96,96));
    title(recipeLabel(selection),1330,355,27,sc,ox,oy,WHITE);
    const auto cost=LIOS_BUILD::CRAFT_COSTS[selection];
    struct Mat {Texture2D icon;int available,needed;};
    Mat mat[3]={{art.wood,r.wood,cost.wood},{art.stone,r.stone,cost.stone},
                 {art.metal,r.metal,cost.metal}};
    for(int i=0;i<3;i++){
        if(mat[i].needed<=0)continue;
        float y=440+i*70;
        sprite(mat[i].icon,b(1320,y,61,61));
        title(TextFormat("%d / %d",mat[i].available,mat[i].needed*quantity),
            1403,y+15,27,sc,ox,oy,
            mat[i].available>=mat[i].needed*quantity?RAYWHITE:ORANGE);
    }
    title("QUANTIDADE",1130,675,25,sc,ox,oy);
    sprite(art.minus,b(1360,663,72,72));
    title(TextFormat("%d",quantity),1467,684,31,sc,ox,oy);
    sprite(art.plus,b(1547,663,72,72));
    DrawRectangleRec(b(1642,663,105,72),Color{61,72,63,235});
    title("MAX",1658,682,25,sc,ox,oy);
    const int maxQuantity=sys.maxCraftableQuantity(
        static_cast<LIOS_BUILD::RecipeType>(selection),r);
    title(TextFormat("MAX %d",maxQuantity),1380,728,19,sc,ox,oy,LIGHTGRAY);
    title(TextFormat("TEMPO %.0fs",
        LIOS_BUILD::System::recipeDuration(
            static_cast<LIOS_BUILD::RecipeType>(selection))*quantity),
        1130,735,20,sc,ox,oy,GRAY);
    sprite(art.button,b(1340,770,334,84),maxQuantity>0?WHITE:GRAY);
    title("FABRICAR",1397,795,31,sc,ox,oy,WHITE);
    } // selected recipe details
    title(status.c_str(),1100,860,20,sc,ox,oy,YELLOW);
    if(!sys.queue.empty()){
        title("FILA DE FABRICACAO (SELECIONE PARA CANCELAR)",
            1100,891,19,sc,ox,oy,LIGHTGRAY);
        for(unsigned i=0;i<sys.queue.size();i++){
            const auto &q=sys.queue[i];
            float y=921.f+18.f*i;
            if(int(i)==selectedOrder)
                DrawRectangleRec(b(1100,y-2,520,19),Color{49,77,88,240});
            title(TextFormat("%d. %s  %.0f%%",int(i)+1,recipeLabel(q.type),
                100.f*std::min(1.f,q.elapsed/q.duration)),
                1110,y,17,sc,ox,oy,SKYBLUE);
        }
        sprite(art.close,b(1640,914,63,63));
    }
}
} // namespace LIOS_CRAFT_UI
