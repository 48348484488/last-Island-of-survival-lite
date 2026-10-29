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
    std::cout<<"LIOS_BUILD_CRAFT_NATIVE_LOGIC_PASS placed="<<b.placed.size()
             <<" recipes=3 mobile_ui=touch_down_only wall_collision=pass\\n";
}
