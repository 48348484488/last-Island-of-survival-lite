#include "../last_island_recreated_game/native_build_craft.hpp"
#include <cassert>
#include <iostream>
#include <limits>
int main(){
    using namespace LIOS_BUILD;
    System b;
    Resources r{500,500,100,0};
    assert(!b.valid(Foundation,1,0,0,0.f,0.f)); // Blueprint required.
    assert(b.craft(Blueprint,r));
    assert(b.blueprint&&r.wood==480&&r.stone==480);
    assert(!b.craft(Blueprint,r)); // No duplicate purchases.
    assert(b.craft(Hammer,r)&&b.craft(StonePickaxe,r));
    assert(b.hammer&&b.pickaxe);
    auto n=r;
    assert(b.place(Foundation,1,0,0,0.f,0.f,r));
    assert(r.wood==n.wood-BUILD_COSTS[Foundation].wood);
    assert(!b.place(Foundation,1,0,0,0.f,0.f,r)); // No duplicate foundation.
    assert(b.place(Wall,1,0,0,0.f,0.f,r));
    assert(b.blocks(3.f,-1.5f)); // North wall is solid.
    assert(!b.blocks(3.f,0.f));  // Center of foundation is walkable.
    assert(!b.place(Doorway,1,0,0,0.f,0.f,r)); // Same edge conflict.
    assert(b.place(Doorway,1,0,1,0.f,0.f,r)); // East edge permitted.
    assert(!b.blocks(4.5f,0.f)); // Door frame lets avatar through.
    assert(b.place(Roof,1,0,0,0.f,0.f,r));
    assert(!b.place(Roof,1,0,1,0.f,0.f,r));
    assert(b.place(Storage,1,0,0,0.f,0.f,r));
    assert(!b.place(Wall,2,0,0,0.f,0.f,r)); // Unsupported wall.
    assert(b.place(Campfire,2,0,0,0.f,0.f,r));
    assert(!b.place(Campfire,1,0,0,0.f,0.f,r)); // Campfire not inside foundation.
    Resources broke{0,0,0,0};
    int placed=b.placed.size();
    assert(!b.place(Foundation,-1,0,0,0.f,0.f,broke));
    assert(b.placed.size()==size_t(placed)); // No free build.
    assert(!b.valid(Foundation,6000,0,0,0.f,0.f)); // Must be in 6000 world.
    assert(grid(-4.51f)==-2&&grid(4.51f)==2);
    assert(turn(-1)==3&&turn(4)==0);
    // The Unity original selects a category first and a second-level child
    // BuildPart.toggle second; category press NEVER places a wall directly.
    assert(partForOriginalCategory(2,0)==Foundation);
    assert(partForOriginalCategory(3,0)==Wall);
    assert(partForOriginalCategory(3,1)==-1); // source is half-wall, not doorway
    assert(partForOriginalCategory(6,3)==Doorway); // source is doorframe
    assert(partForOriginalCategory(4,0)==Roof);
    assert(partForOriginalCategory(5,0)==Stair);
    assert(partForOriginalCategory(6,0)==Window);
    assert(partForOriginalCategory(7,0)==Obstacle);
    assert(partForOriginalCategory(3,2)==-1);
    assert(partForOriginalCategory(0,0)==-1);
    assert(partForOriginalCategory(2,-1)==-1);
    // All original source building category tabs now have a native action.
    Resources extra{400,400,100,0};
    assert(b.place(Stair,1,0,0,0.f,0.f,extra));
    assert(b.place(Window,1,0,2,0.f,0.f,extra));
    assert(b.blocks(3.f,1.5f)); // Window sill must be collidable.
    assert(b.place(Obstacle,-2,0,0,0.f,0.f,extra));
    assert(b.blocks(-6.f,0.f));
    assert(b.saveCurrentBlueprint()&&b.savedPlan.size()==b.placed.size());
    b.freePlacement=true;
    assert(b.place(Stair,0,2,0,0.f,0.f,extra));
    // Original production queue concept: quantity, timers, refunds, queue cap.
    System craft;
    Resources ingredients{1000,1000,1000,0};
    assert(craft.maxCraftableQuantity(StonePickaxe,ingredients)==8);
    assert(craft.maxCraftableQuantity(static_cast<RecipeType>(-1),ingredients)==0);
    assert(craft.requestCraft(StonePickaxe,ingredients,3));
    assert(craft.maxCraftableQuantity(StonePickaxe,ingredients)==5);
    assert(craft.queue.size()==3);
    assert(ingredients.wood==1000-3*CRAFT_COSTS[StonePickaxe].wood);
    assert(craft.cancelCraft(1,ingredients));
    assert(craft.queue.size()==2);
    assert(craft.maxCraftableQuantity(StonePickaxe,ingredients)==6);
    assert(ingredients.wood==1000-2*CRAFT_COSTS[StonePickaxe].wood);
    for(int i=0;i<14;i++)craft.tickCraft(1.f);
    assert(craft.queue.empty()&&craft.pickaxe&&craft.craftedCount[StonePickaxe]==2);
    assert(!craft.requestCraft(Blueprint,ingredients,9));
    assert(craft.requestCraft(Blueprint,ingredients,8));
    assert(!craft.requestCraft(Hammer,ingredients));
    assert(craft.maxCraftableQuantity(Hammer,ingredients)==0);
    assert(craft.cancelCraft(0,ingredients));
    Resources limited{10,10,10,0};
    assert(craft.maxCraftableQuantity(Hammer,limited)==0);
    Resources picked{75,140,0,0};
    System independent;
    assert(independent.maxCraftableQuantity(StonePickaxe,picked)==3);
    assert(!independent.requestCraft(StonePickaxe,picked,4));
    assert(independent.requestCraft(StonePickaxe,picked,3));
    assert(picked.wood==0&&picked.stone==5);
    // Offline queue must conserve elapsed time across multiple orders.
    System timed;
    Resources enough{1000,1000,1000,0};
    assert(timed.requestCraft(StonePickaxe,enough,3));
    timed.tickCraft(11.25f);
    assert(timed.craftedCount[StonePickaxe]==2);
    assert(timed.queue.size()==1);
    assert(std::abs(timed.queue.front().elapsed-1.25f)<0.0001f);
    timed.tickCraft(-10.f);
    assert(std::abs(timed.queue.front().elapsed-1.25f)<0.0001f);
    timed.tickCraft(3.75f);
    assert(timed.queue.empty()&&timed.craftedCount[StonePickaxe]==3);
    System stalled;
    assert(stalled.requestCraft(Hammer,enough,1));
    stalled.tickCraft(std::numeric_limits<float>::quiet_NaN());
    assert(stalled.queue.size()==1&&stalled.queue.front().elapsed==0.f);
    std::cout<<"LIOS_BUILD_CRAFT_NATIVE_LOGIC_PASS placed="<<b.placed.size()
             <<" recipes=3 queued_quantity=true item_sprite_source=true build_categories=8 wall_collision=pass\\n";
}
