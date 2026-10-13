#include "esp_projection.hpp"
#include <cassert>
#include <cmath>
#include <limits>
#include <iostream>
using namespace oxide_lab;

static Mat4 perspective(float near=0.1f, float far=100.0f) {
    return {{1,0,0,0, 0,1,0,0, 0,0,(far+near)/(near-far),(2*far*near)/(near-far), 0,0,-1,0}};
}
static bool eq(float a,float b,float tol=0.01f) { return std::abs(a-b)<tol; }

int main() {
    const auto m=perspective();
    const Viewport s{1080,1920};
    Vec2 out{-1,-1};
    assert(world_to_screen(m,s,{0,0,-5},out) && eq(out.x,540) && eq(out.y,960));
    assert(world_to_screen(m,s,{1,0,-5},out) && out.x>540);
    assert(world_to_screen(m,s,{-1,0,-5},out) && out.x<540);
    assert(world_to_screen(m,s,{0,1,-5},out) && out.y<960);
    assert(world_to_screen(m,s,{0,-1,-5},out) && out.y>960);
    assert(!world_to_screen(m,s,{0,0,5},out));
    assert(!world_to_screen(m,s,{0,0,-0.01f},out));
    assert(!world_to_screen(m,s,{200,0,-5},out));
    assert(!world_to_screen(m,{0,1920},{0,0,-5},out));
    assert(!world_to_screen(m,{1080,std::numeric_limits<float>::infinity()},{0,0,-5},out));
    assert(!world_to_screen(m,s,{std::numeric_limits<float>::quiet_NaN(),0,-5},out));
    Mat4 n=m;n.m[3]=std::numeric_limits<float>::infinity();
    assert(!world_to_screen(n,s,{0,0,-5},out));
    Rect r{};
    assert(player_box(m,s,{0,1,-5},{0,-1,-5},r) && r.top<r.bottom && r.left<540 && r.right>540);
    assert(!player_box(m,s,{0,0,-5},{0,0,-5},r));
    assert(!player_box(m,s,{0,1,5},{0,-1,5},r));
    assert(!player_box(m,s,{200,1,-5},{200,-1,-5},r));
    Mat4 d={{1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1}};
    assert(world_to_screen(d,s,{0,0,0.5f},out,DepthRange::ZeroToOne));
    assert(!world_to_screen(d,s,{0,0,-0.5f},out,DepthRange::ZeroToOne));
    assert(world_to_screen(d,s,{0,0,-0.5f},out,DepthRange::OpenGL));
    assert(!world_to_screen(d,s,{0,0,2},out,DepthRange::ZeroToOne));
    std::cout<<"Projection/box regression tests: PASS\n";
}
