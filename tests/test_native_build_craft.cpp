#include "../last_island_recreated_game/native_build_craft.hpp"
#include <cassert>
#include <iostream>
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
    assert(craft.requestCraft(StonePickaxe,ingredients,3));
    assert(craft.queue.size()==3);
    assert(ingredients.wood==1000-3*CRAFT_COSTS[StonePickaxe].wood);
    assert(craft.cancelCraft(1,ingredients));
    assert(craft.queue.size()==2);
    assert(ingredients.wood==1000-2*CRAFT_COSTS[StonePickaxe].wood);
    for(int i=0;i<14;i++)craft.tickCraft(1.f);
    assert(craft.queue.empty()&&craft.pickaxe&&craft.craftedCount[StonePickaxe]==2);
    assert(!craft.requestCraft(Blueprint,ingredients,9));
    assert(craft.requestCraft(Blueprint,ingredients,8));
    assert(!craft.requestCraft(Hammer,ingredients));
    assert(craft.cancelCraft(0,ingredients));
    std::cout<<"LIOS_BUILD_CRAFT_NATIVE_LOGIC_PASS placed="<<b.placed.size()
             <<" recipes=3 queued_quantity=true item_sprite_source=true build_categories=8 wall_collision=pass\\n";
}
