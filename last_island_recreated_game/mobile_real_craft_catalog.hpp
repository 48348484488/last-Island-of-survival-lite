#pragma once
// LITE 7.3 source-backed recipe browser. Recipe values and icons are extracted
// from the user's original Unity cfg and Sprite AssetBundles by EdgeAI.
// Native rectangles outside mobile_original_craft_layout.hpp are provisional.
#include "raylib.h"
#include "native_build_craft.hpp"
#include "mobile_original_craft_layout.hpp"
#include "original_craft_catalog_v73.hpp"
#include <algorithm>
#include <deque>
#include <cmath>
#include <map>
#include <string>

namespace LIOS_REAL_CRAFT {
namespace Data=LIOS_ORIGINAL_RECIPES;
struct Tab {int id;const char* label;};
static const Tab MAIN[]={{1,"BASICO"},{12,"ITENS"},{4,"ESTRUTURAS"},
                         {13,"COMBATE"},{14,"VEICULOS"}};
static const Tab BASIC[]={{1,"GERAL"}};
static const Tab ITEMS[]={{2,"FERRAM."},{3,"MATER."},{6,"MEDIC."}};
static const Tab STRUCTURE[]={{21,"ESTRUT."},{16,"ELETR."},{19,"MOVEIS"},
                              {20,"DECOR."},{25,"CERCAS"},{26,"EXTRAS"}};
static const Tab COMBAT[]={{7,"EQUIP."},{8,"ARMAS"},{9,"MUNICAO"},
                           {10,"ARMAD."},{27,"MIRAS"}};
static const Tab VEHICLE[]={{15,"VEICULOS"},{23,"ASA/PLAN."}};
inline int tabCount(int big){
    switch(big){case 1:return 1;case 12:return 3;case 4:return 6;
        case 13:return 5;case 14:return 2;default:return 0;}
}
inline Tab tab(int big,int i){
    if(i<0||i>=tabCount(big))return {-1,""};
    switch(big){case 1:return BASIC[i];case 12:return ITEMS[i];
        case 4:return STRUCTURE[i];case 13:return COMBAT[i];
        case 14:return VEHICLE[i];default:return {-1,""};}
}
inline int firstTab(int big){return tab(big,0).id;}
inline bool includes(const Data::Recipe& r,int big,int small){
    if(big==1)return small==1&&r.isNormal;
    if(big==4)return (r.bigType==4||r.bigType==18)&&r.smallType==small;
    return r.bigType==big&&r.smallType==small;
}
inline int count(int big,int small){
    int n=0;
    for(int i=0;i<Data::RecipeCount;i++)
        if(includes(Data::Recipes[i],big,small))++n;
    return n;
}
inline const Data::Recipe* at(int big,int small,int index){
    if(index<0)return nullptr;
    for(int i=0;i<Data::RecipeCount;i++){
        const auto& r=Data::Recipes[i];
        if(includes(r,big,small)&&index--==0)return &r;
    }
    return nullptr;
}
struct Order {int recipeId,quantity;float elapsed,duration;};
struct State {
    int big=1,small=1,page=0,selectedId=-1,quantity=1,selectedOrder=0;
    std::map<int,int> stock; // Original item IDs produced by this offline demo
    std::deque<Order> queue;
    std::string message="CATALOGO EXTRAIDO DO JOGO ORIGINAL";
    static constexpr unsigned QueueLimit=8;
    void change(int category,int sub){
        big=category;small=sub;page=0;selectedId=-1;quantity=1;
    }
    int available(int itemId,const LIOS_BUILD::Resources& resources)const{
        if(itemId==50001)return resources.wood; // source ItemCfg: wood
        if(itemId==50006)return resources.stone; // source ItemCfg: stone
        auto it=stock.find(itemId);
        return it!=stock.end()?it->second:0;
    }
    bool safeForOffline(const Data::Recipe& recipe)const{
        // Original learning/workbench checks are still unknown: only
        // default-level/basic recipes are offered as explicit offline tests.
        return recipe.isNormal&&recipe.level<=1;
    }
    int maxQuantity(const Data::Recipe& recipe,
                    const LIOS_BUILD::Resources& resources)const{
        if(!safeForOffline(recipe)||queue.size()>=QueueLimit)return 0;
        int value=std::min(int(QueueLimit-queue.size()),
                           std::max(0,recipe.onceMax));
        for(int i=0;i<recipe.ingredientCount;i++){
            const auto& mat=Data::Ingredients[recipe.firstIngredient+i];
            if(mat.amount>0)
                value=std::min(value,available(mat.itemId,resources)/mat.amount);
        }
        return std::max(0,value);
    }
    bool request(const Data::Recipe& recipe,int qty,
                 LIOS_BUILD::Resources& resources){
        if(qty<=0||qty>maxQuantity(recipe,resources))return false;
        for(int i=0;i<recipe.ingredientCount;i++){
            const auto& mat=Data::Ingredients[recipe.firstIngredient+i];
            const int spent=mat.amount*qty;
            if(mat.itemId==50001)resources.wood-=spent;
            else if(mat.itemId==50006)resources.stone-=spent;
            else stock[mat.itemId]-=spent;
        }
        // This is a local test queue, NOT the original server production.
        queue.push_back({recipe.id,qty,0.f,
            std::max(1.f,float(recipe.seconds)*qty)});
        selectedOrder=int(queue.size())-1;
        message="PRODUCAO OFFLINE INICIADA (SEM SERVIDOR)";
        return true;
    }
    bool cancel(unsigned index,LIOS_BUILD::Resources& resources){
        if(index>=queue.size())return false;
        const Order q=queue[index];
        const auto* recipe=Data::find(q.recipeId);
        if(!recipe)return false;
        for(int i=0;i<recipe->ingredientCount;i++){
            const auto& mat=Data::Ingredients[recipe->firstIngredient+i];
            const int refund=mat.amount*q.quantity;
            if(mat.itemId==50001)resources.wood+=refund;
            else if(mat.itemId==50006)resources.stone+=refund;
            else stock[mat.itemId]+=refund;
        }
        queue.erase(queue.begin()+index);
        selectedOrder=std::max(0,std::min(selectedOrder,int(queue.size())-1));
        message="PEDIDO OFFLINE CANCELADO";
        return true;
    }
    void tick(float dt){
        if(queue.empty()||!std::isfinite(dt)||dt<=0.f)return;
        // Consume elapsed wall-clock time across queued offline orders.
        // This is NOT a claim about the original multiplayer production timer.
        while(!queue.empty()&&dt>0.f){
            Order& first=queue.front();
            const float remaining=std::max(0.f,first.duration-first.elapsed);
            const float advance=std::min(dt,remaining);
            first.elapsed+=advance;
            dt-=advance;
            if(advance<remaining)break;
            const Order finished=first;
            const auto* recipe=Data::find(finished.recipeId);
            if(recipe)
                stock[recipe->targetId]+=
                    std::max(1,recipe->productNum)*finished.quantity;
            queue.pop_front();
            selectedOrder=std::max(0,std::min(selectedOrder,int(queue.size())-1));
            message="ITEM ORIGINAL FABRICADO NA MOCHILA OFFLINE";
        }
    }
};
struct Art {
    Texture2D itemAtlases[Data::AtlasCount]={};
    Texture2D mainTab={},subTab={},activeTab={},slot={},selection={},back={};
    void load(){
        for(int i=0;i<Data::AtlasCount;i++)
            itemAtlases[i]=LoadTexture(TextFormat(
                "assets/craft_source_atlas_%d.png",i));
        mainTab=LoadTexture("assets/craft_original_primary_tab.png");
        subTab=LoadTexture("assets/craft_original_secondary_tab.png");
        activeTab=LoadTexture("assets/craft_original_primary_tab_indicator.png");
        slot=LoadTexture("assets/craft_original_item_slot.png");
        selection=LoadTexture("assets/craft_original_selected.png");
        back=LoadTexture("assets/craft_original_back_button.png");
    }
};
inline Rectangle box(float x,float y,float w,float h,float sc,float ox,float oy){
    return {ox+x*sc,oy+y*sc,w*sc,h*sc};
}
inline bool hit(Vector2 p,float x,float y,float w,float h,
                float sc,float ox,float oy){
    return CheckCollisionPointRec(p,box(x,y,w,h,sc,ox,oy));
}
// One source-backed rectangle feeds both rendering and touch detection.
inline Rectangle sourceRect(const LIOS_CRAFT_SOURCE::Bounds& r,
                            float sc,float ox,float oy){
    return box(r.x,r.y,r.w,r.h,sc,ox,oy);
}
inline Rectangle subTabRect(int index,float sc,float ox,float oy){
    const auto& r=LIOS_CRAFT_SOURCE::SubTabOnSprite;
    return box(r.x+LIOS_CRAFT_SOURCE::SubTabStepX*index,
               r.y,r.w,r.h,sc,ox,oy);
}
inline void spr(Texture2D tex,Rectangle dst,Color tint=WHITE){
    if(tex.id)DrawTexturePro(tex,
        {0.f,0.f,float(tex.width),float(tex.height)},dst,{0,0},0,tint);
}
inline void icon(const Art& art,int atlas,int slot,Rectangle dst){
    if(atlas<0||atlas>=Data::AtlasCount||slot<0||slot>=144)return;
    Texture2D tex=art.itemAtlases[atlas];
    if(!tex.id)return;
    Rectangle src={float(slot%12*Data::AtlasTile),
                   float(slot/12*Data::AtlasTile),
                   float(Data::AtlasTile),float(Data::AtlasTile)};
    DrawTexturePro(tex,src,dst,{0,0},0,WHITE);
}
inline void txt(const char* s,float x,float y,int pts,
                float sc,float ox,float oy,Color tint=RAYWHITE){
    DrawText(s,int(ox+x*sc),int(oy+y*sc),
             std::max(11,int(pts*sc)),tint);
}
inline const Data::Recipe* current(const State& state){
    return Data::find(state.selectedId);
}
inline void render(const Art& art,const State& state,
                   const LIOS_BUILD::Resources& resources,
                   float sc,float ox,float oy){
    const auto B=[&](float x,float y,float w,float h){return box(x,y,w,h,sc,ox,oy);};
    DrawRectangle(int(ox),int(oy),int(1920*sc),int(1080*sc),Color{6,11,12,230});
    // Left category column uses actual original Unity Sprite sizes.
    for(int i=0;i<5;i++){
        float x=LIOS_CRAFT_SOURCE::MainTabOnSprite.x;
        float y=LIOS_CRAFT_SOURCE::MainTabOnSprite.y+
                LIOS_CRAFT_SOURCE::MainTabStepY*i;
        bool active=MAIN[i].id==state.big;
        if(art.mainTab.id) spr(art.mainTab,B(x,y,258.8f,86),
            active?WHITE:Color{157,157,157,230});
        else DrawRectangleRec(B(x,y,258.8f,86),
            active?Color{78,88,85,239}:Color{38,39,39,225});
        if(active&&art.activeTab.id){
            const auto in=LIOS_CRAFT_SOURCE::MainTabActiveIndicator;
            spr(art.activeTab,B(in.x,in.y+86.f*i,in.w,in.h));
        }
        txt(MAIN[i].label,x+93,y+30,23,sc,ox,oy);
    }
    spr(art.back,sourceRect(LIOS_CRAFT_SOURCE::Back,sc,ox,oy));
    txt("VOLTAR",1740,28,26,sc,ox,oy);
    txt("FABRICACAO",235,102,34,sc,ox,oy);
    for(int i=0;i<tabCount(state.big);i++){
        const auto cell=tab(state.big,i);
        const auto& base=LIOS_CRAFT_SOURCE::SubTabOnSprite;
        float x=base.x+LIOS_CRAFT_SOURCE::SubTabStepX*i,y=base.y;
        Rectangle tabBox=subTabRect(i,sc,ox,oy);
        if(art.subTab.id)
            spr(art.subTab,tabBox,
                state.small==cell.id?WHITE:Color{155,155,155,229});
        else DrawRectangleRec(tabBox,
            state.small==cell.id?Color{84,100,89,245}:Color{32,38,39,230});
        txt(cell.label,x+5,y+7,17,sc,ox,oy);
    }
    // Unity's real scp_items RectTransform is 238.8/208.2/735.2/640.6.
    // A 4 x 3 virtual item grid is the native scroll fallback; it is
    // not advertised as proof of the original runtime GridLayout.
    const auto viewport=LIOS_CRAFT_SOURCE::RecipeScrollViewport;
    Rectangle clip=B(viewport.x,viewport.y,viewport.w,viewport.h);
    BeginScissorMode(int(clip.x),int(clip.y),
        int(clip.width),int(clip.height));
    for(int i=0;i<12;i++){
        const auto* recipe=at(state.big,state.small,state.page*12+i);
        if(!recipe)continue;
        const float x=250.f+179.f*(i%4),y=221.f+186.f*(i/4);
        spr(art.slot,B(x,y,149.5f,154.1f));
        if(state.selectedId==recipe->id)
            spr(art.selection,B(x+8,y+6,134.f,141.f));
        icon(art,recipe->atlas,recipe->slot,B(x+22,y+12,108,107));
        txt(TextFormat("#%d",recipe->id),x+15,y+126,21,sc,ox,oy);
        if(recipe->isNormal)txt("BASICO",x+77,y+126,13,sc,ox,oy,GREEN);
    }
    EndScissorMode();
    txt(TextFormat("%d RECEITAS",count(state.big,state.small)),
        251,849,19,sc,ox,oy,LIGHTGRAY);
    txt(TextFormat("PAGINA %d/%d",state.page+1,
        std::max(1,(count(state.big,state.small)+11)/12)),
        503,849,20,sc,ox,oy);
    DrawRectangleRec(B(789,845,82,47),Color{48,58,59,230});
    DrawRectangleRec(B(881,845,82,47),Color{48,58,59,230});
    txt("<",819,851,28,sc,ox,oy);
    txt(">",915,851,28,sc,ox,oy);

    const auto* recipe=current(state);
    if(recipe){
        txt(TextFormat("RECEITA ORIGINAL #%d",recipe->id),
            1038,173,24,sc,ox,oy,WHITE);
        icon(art,recipe->atlas,recipe->slot,B(1312,250,130,130));
        txt(TextFormat("ITEM ID %d",recipe->targetId),
            1038,397,25,sc,ox,oy);
        txt(TextFormat("TEMPO %d s / UNIDADE",recipe->seconds),
            1038,430,22,sc,ox,oy,LIGHTGRAY);
        txt(TextFormat("NIVEL %d",recipe->level),
            1038,460,20,sc,ox,oy,LIGHTGRAY);
        for(int i=0;i<recipe->ingredientCount;i++){
            const auto& mat=Data::Ingredients[recipe->firstIngredient+i];
            const float x=1035.f+225.f*(i%3),y=515.f+102.f*(i/3);
            icon(art,mat.atlas,mat.slot,B(x,y,66,66));
            const int owned=state.available(mat.itemId,resources);
            txt(TextFormat("%d/%d",owned,mat.amount*state.quantity),
                x+70,y+7,18,sc,ox,oy,
                owned>=mat.amount*state.quantity?WHITE:ORANGE);
            txt(TextFormat("#%d",mat.itemId),x+70,y+34,17,sc,ox,oy,GRAY);
        }
        // The Unity LITE 7.3 RectTransform bounds are shared by rendering
        // and hit testing. These controls were previously drawn at y=741,
        // despite the original source placing them at y=954.3.
        txt("QUANTIDADE",1038,916,24,sc,ox,oy);
        DrawRectangleRec(sourceRect(LIOS_CRAFT_SOURCE::QuantityMinus,sc,ox,oy),
                         Color{43,50,50,245});
        DrawRectangleRec(sourceRect(LIOS_CRAFT_SOURCE::QuantityPlus,sc,ox,oy),
                         Color{43,50,50,245});
        DrawRectangleRec(sourceRect(LIOS_CRAFT_SOURCE::QuantityMax,sc,ox,oy),
                         Color{43,50,50,245});
        txt("-",1042,968,29,sc,ox,oy);
        txt(TextFormat("%d",state.quantity),1200,968,26,sc,ox,oy);
        txt("+",1325,968,27,sc,ox,oy);
        const int maxCraft=state.maxQuantity(*recipe,resources);
        txt(TextFormat("MAX %d",maxCraft),1382,973,18,sc,ox,oy);
        if(state.safeForOffline(*recipe)){
            // Native fallback position: does not overlap the source MAX hitbox.
            DrawRectangleRec(B(1510,912,237,72),Color{57,80,65,240});
            txt("FABRICAR OFFLINE",1517,931,22,sc,ox,oy);
        }else{
            DrawRectangleRec(B(1510,912,237,72),Color{46,49,51,230});
            txt("AINDA NAO PORTADO",1517,931,20,sc,ox,oy,GRAY);
        }
    }else{
        txt("SELECIONE UMA RECEITA",1038,280,29,sc,ox,oy,LIGHTGRAY);
    }
    txt(state.message.c_str(),1038,838,19,sc,ox,oy,YELLOW);
    if(!state.queue.empty()){
        txt("FILA OFFLINE",1038,865,19,sc,ox,oy,LIGHTGRAY);
        for(unsigned i=0;i<state.queue.size()&&i<4;i++){
            const auto& q=state.queue[i];
            float y=891.f+22.f*i;
            txt(TextFormat("#%d %d%%",q.recipeId,
                int(100.f*std::min(1.f,q.elapsed/q.duration))),
                1040,y,18,sc,ox,oy,int(i)==state.selectedOrder?SKYBLUE:GRAY);
        }
        DrawRectangleRec(B(1310,897,116,52),Color{65,47,47,245});
        txt("CANCEL",1315,912,20,sc,ox,oy);
    }
    txt("CATALOGO FIEL AOS ARQUIVOS / PROTOTIPO OFFLINE",
        1038,1030,17,sc,ox,oy,GRAY);
}
inline bool touch(State& state,Vector2 p,float sc,float ox,float oy,
                  LIOS_BUILD::Resources& resources){
    if(CheckCollisionPointRec(p,sourceRect(LIOS_CRAFT_SOURCE::Back,sc,ox,oy)))
        return false; // caller closes panel; full source-backed 316px hitbox
    for(int i=0;i<5;i++){
        if(hit(p,LIOS_CRAFT_SOURCE::MainTabOnSprite.x,
               LIOS_CRAFT_SOURCE::MainTabOnSprite.y+86.f*i,
               258.8f,86.f,sc,ox,oy)){
            state.change(MAIN[i].id,firstTab(MAIN[i].id));return true;
        }
    }
    for(int i=0;i<tabCount(state.big);i++){
        if(CheckCollisionPointRec(p,subTabRect(i,sc,ox,oy))){
            state.change(state.big,tab(state.big,i).id);return true;
        }
    }
    for(int i=0;i<12;i++){
        const auto* recipe=at(state.big,state.small,state.page*12+i);
        if(!recipe)continue;
        if(hit(p,250.f+179.f*(i%4),221.f+186.f*(i/4),
                149.5f,154.1f,sc,ox,oy)){
            state.selectedId=recipe->id;state.quantity=1;return true;
        }
    }
    if(hit(p,789,845,82,47,sc,ox,oy)){
        state.page=std::max(0,state.page-1);return true;
    }
    if(hit(p,881,845,82,47,sc,ox,oy)){
        int pages=(count(state.big,state.small)+11)/12;
        state.page=std::min(std::max(0,pages-1),state.page+1);return true;
    }
    const auto* recipe=current(state);
    if(recipe){
        if(CheckCollisionPointRec(p,sourceRect(LIOS_CRAFT_SOURCE::QuantityMinus,sc,ox,oy))){
            state.quantity=std::max(1,state.quantity-1);return true;
        }
        if(CheckCollisionPointRec(p,sourceRect(LIOS_CRAFT_SOURCE::QuantityPlus,sc,ox,oy))){
            // Respect source recipe limits and available offline materials.
            state.quantity=std::max(1,std::min(state.maxQuantity(*recipe,resources),
                                               state.quantity+1));
            return true;
        }
        if(CheckCollisionPointRec(p,sourceRect(LIOS_CRAFT_SOURCE::QuantityMax,sc,ox,oy))){
            state.quantity=std::max(1,state.maxQuantity(*recipe,resources));
            return true;
        }
        if(hit(p,1510,912,237,72,sc,ox,oy)){
            if(state.request(*recipe,state.quantity,resources))return true;
            state.message="INDISPONIVEL: MATERIAIS / DESBLOQUEIO OFFLINE";
            return true;
        }
    }
    if(!state.queue.empty()){
        if(hit(p,1310,897,116,52,sc,ox,oy)){
            state.cancel(unsigned(state.selectedOrder),resources);return true;
        }
        for(unsigned i=0;i<state.queue.size()&&i<4;i++){
            if(hit(p,1040,891.f+22.f*i,245,22,sc,ox,oy)){
                state.selectedOrder=int(i);return true;
            }
        }
    }
    return true;
}
} // namespace LIOS_REAL_CRAFT
