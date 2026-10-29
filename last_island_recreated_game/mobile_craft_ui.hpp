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
#include "mobile_original_craft_layout.hpp"
#include <algorithm>
#include <string>
namespace LIOS_CRAFT_UI {
// Tab values and nesting copied from SC.UI.BagProducePage.OnInit in the
// original 7.3 Assembly-CSharp source; coordinates are still native layout.
struct Tab {int id;const char* label;};
static constexpr Tab BigTabs[]={{1,"BASICO"},{12,"ITENS"},{4,"ESTRUTURAS"},
                                {13,"COMBATE"},{14,"VEICULOS"}};
static constexpr Tab BasicTabs[]={{1,"GERAL"}};
static constexpr Tab ItemTabs[]={{2,"FERRAMENTAS"},{5,"COZINHA"},
                                 {6,"MEDICINA"},{3,"MATERIAIS"}};
static constexpr Tab StructureTabs[]={{4,"ESTRUTURAS"},{16,"ELETRICA"}};
static constexpr Tab CombatTabs[]={{7,"EQUIPAMENTO"},{8,"ARMAS"},
                                   {9,"MUNICAO"},{10,"ARMADILHAS"}};
static constexpr Tab VehicleTabs[]={{15,"VEICULOS"}};
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
inline Rectangle rect(LIOS_CRAFT_SOURCE::Bounds v,float scale,float left,float top){
    return rect(v.x,v.y,v.w,v.h,scale,left,top);
}
inline bool hit(Vector2 p,LIOS_CRAFT_SOURCE::Bounds v,float sc,float x,float y){
    return CheckCollisionPointRec(p,rect(v,sc,x,y));
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
    const auto sb=[&](LIOS_CRAFT_SOURCE::Bounds v){return rect(v,sc,ox,oy);};
    // Native dimming is retained because the original m_bag_bg shader is a
    // scene-space quad, not a UGUI image in the source prefab.
    DrawRectangle(int(ox),int(oy),int(1920*sc),int(1080*sc),
                  {6,11,12,230});
    // Real original section-strip + modular slot border textures.
    sprite(art.strip,b(185,95,1540,91));
    title("FABRICACAO",235,114,43,sc,ox,oy);
    sprite(art.back,sb(LIOS_CRAFT_SOURCE::Back));
    title("VOLTAR",1748,24,28,sc,ox,oy);
    // The source canvas positions main categories at the left of
    // scp_items and its dynamically positioned subtypes above that list.
    // The 120x58 / 134x43 button sizes below are provisional, since the
    // serialized source tabs have zero size before Unity LayoutGroup runs.
    for(int i=0;i<5;i++){
        float x=76.0f,y=230.0f+76.0f*i;
        Rectangle tab=b(x,y,148,60);
        DrawRectangleRec(tab,bigTab==BigTabs[i].id?
            Color{74,88,85,238}:Color{30,39,41,220});
        DrawRectangleLinesEx(tab,sc*2,Color{152,161,149,220});
        title(BigTabs[i].label,x+12,y+14,22,sc,ox,oy);
    }
    for(int i=0;i<subTabCount(bigTab);i++){
        float x=LIOS_CRAFT_SOURCE::SubTabOriginX+
            LIOS_CRAFT_SOURCE::SubTabStepX*i;
        float y=LIOS_CRAFT_SOURCE::SubTabOriginY;
        const Tab sub=subTab(bigTab,i);
        DrawRectangleRec(b(x,y,136,43),
            sub.id==smallTab?Color{77,97,87,235}:Color{26,33,38,220});
        title(sub.label,x+6,y+10,19,sc,ox,oy);
    }
    int recipesOnTab=0;
    for(int slot=0;slot<3;slot++){
        int i=visibleRecipe(bigTab,smallTab,slot);
        if(i<0)continue;
        ++recipesOnTab;
        // Original scp_items is x=238.8..974.0, y=208.2..848.8.
        float x=LIOS_CRAFT_SOURCE::RecipeTemplateCell.x+180.f*slot;
        float y=LIOS_CRAFT_SOURCE::RecipeTemplateCell.y+12.f;
        sprite(art.slot,b(x,y,149.5f,154.1f));
        if(i==selection)sprite(art.selected,b(x+12,y+11,125,130));
        Texture2D item=i==0?art.buildIcon:i==1?art.hammer:art.pickaxe;
        sprite(item,b(x+32,y+25,85,94));
        title(i==0?"CONSTRUIR":i==1?"MARTELO":"PICARETA",
              x+5,y+166,21,sc,ox,oy);
        if(sys.owns(static_cast<LIOS_BUILD::RecipeType>(i)))
            title("FABRICADO",x+15,y+196,20,sc,ox,oy,LIME);
    }
    if(!recipesOnTab){
        title("RECEITAS ORIGINAIS AINDA NAO PORTADAS",
              254,400,22,sc,ox,oy,GRAY);
    }else title("RECEITAS DE TESTE OFFLINE (CFG PENDENTE)",
                254,790,20,sc,ox,oy,GRAY);
    if(selection<0){
        title("SELECIONE UMA RECEITA DISPONIVEL",
              1110,360,25,sc,ox,oy,GRAY);
    } else {
    // Original source-selected item, cost and queue widgets.
    sprite(art.slot,sb(LIOS_CRAFT_SOURCE::SelectedItemIcon));
    Texture2D selected=selection==0?art.buildIcon:
                       selection==1?art.hammer:art.pickaxe;
    sprite(selected,b(1336,274,80,80));
    title(recipeLabel(selection),1038,183,26,sc,ox,oy,WHITE);
    const auto cost=LIOS_BUILD::CRAFT_COSTS[selection];
    struct Mat {Texture2D icon;int available,needed;};
    Mat mat[3]={{art.wood,r.wood,cost.wood},{art.stone,r.stone,cost.stone},
                 {art.metal,r.metal,cost.metal}};
    for(int i=0;i<3;i++){
        if(mat[i].needed<=0)continue;
        float y=480+i*74;
        sprite(mat[i].icon,b(1320,y,61,61));
        title(TextFormat("%d / %d",mat[i].available,mat[i].needed*quantity),
            1403,y+15,27,sc,ox,oy,
            mat[i].available>=mat[i].needed*quantity?RAYWHITE:ORANGE);
    }
    title("QUANTIDADE",1019,917,24,sc,ox,oy);
    sprite(art.minus,sb(LIOS_CRAFT_SOURCE::QuantityMinus));
    title(TextFormat("%d",quantity),1201,968,29,sc,ox,oy);
    sprite(art.plus,sb(LIOS_CRAFT_SOURCE::QuantityPlus));
    DrawRectangleRec(sb(LIOS_CRAFT_SOURCE::QuantityMax),Color{61,72,63,235});
    title("MAX",1399,973,22,sc,ox,oy);
    const int maxQuantity=sys.maxCraftableQuantity(
        static_cast<LIOS_BUILD::RecipeType>(selection),r);
    title(TextFormat("MAX %d",maxQuantity),1390,925,19,sc,ox,oy,LIGHTGRAY);
    title(TextFormat("TEMPO %.0fs",
        LIOS_BUILD::System::recipeDuration(
            static_cast<LIOS_BUILD::RecipeType>(selection))*quantity),
        1038,738,20,sc,ox,oy,GRAY);
    // btn_create has an off-canvas static prefab position before runtime
    // layout; this visible 200x75 fallback is explicitly native, not exact.
    sprite(art.button,b(1510,951,200,75),maxQuantity>0?WHITE:GRAY);
    title("FABRICAR",1537,974,26,sc,ox,oy,WHITE);
    } // selected recipe details
    title(status.c_str(),1038,822,20,sc,ox,oy,YELLOW);
    if(!sys.queue.empty()){
        title("FILA DE FABRICACAO",1520,638,20,sc,ox,oy,LIGHTGRAY);
        for(unsigned i=0;i<sys.queue.size();i++){
            const auto &q=sys.queue[i];
            float y=676.f+25.f*i;
            if(int(i)==selectedOrder)
                DrawRectangleRec(b(1510,y-2,340,24),Color{49,77,88,240});
            title(TextFormat("%d. %s %.0f%%",int(i)+1,recipeLabel(q.type),
                100.f*std::min(1.f,q.elapsed/q.duration)),
                1517,y,14,sc,ox,oy,SKYBLUE);
        }
        sprite(art.close,b(1780,627,60,60));
    }
}
} // namespace LIOS_CRAFT_UI
