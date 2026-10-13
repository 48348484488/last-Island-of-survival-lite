#include "esp_bridge.h"
#include <array>
#include <cassert>
#include <cmath>
#include <cstdio>
#include <cstdint>
#include <limits>
#include <random>
#include <type_traits>
static_assert(std::is_standard_layout<oxide_esp_frame>::value,"frame layout");
static_assert(std::is_standard_layout<oxide_esp_player>::value,"player layout");
static_assert(std::is_standard_layout<oxide_esp_box>::value,"box layout");
static_assert(sizeof(oxide_esp_frame)==84,"frame ABI changed");
static_assert(sizeof(oxide_esp_player)==40,"player ABI changed");
static_assert(sizeof(oxide_esp_box)==24,"box ABI changed");

static oxide_esp_frame frame() {
    oxide_esp_frame f{};
    f.abi_version=OXIDE_ESP_ABI_VERSION;
    f.width=1080;f.height=1920;f.player_count=1;
    constexpr float n=0.1f,far=100.f;
    const float v[16]={1,0,0,0,0,1,0,0,0,0,(far+n)/(n-far),(2*far*n)/(n-far),0,0,-1,0};
    for(int i=0;i<16;++i)f.view_projection[i]=v[i];
    return f;
}
static oxide_esp_player player() {
    oxide_esp_player p{};p.entity_id=0x1122334455667788ULL;
    p.head[1]=1.0f;p.head[2]=-5.0f;
    p.feet[1]=-1.0f;p.feet[2]=-5.0f;
    p.flags=OXIDE_ESP_PLAYER_VALID;
    return p;
}
static bool eq(float a,float b,float tol=0.02f){return std::abs(a-b)<tol;}
int main(){
    auto f=frame();auto p=player();
    oxide_esp_box output[2]{};
    assert(oxide_esp_project_v1(&f,&p,output,2)==1);
    assert(output[0].entity_id==p.entity_id);
    assert(eq((output[0].left+output[0].right)/2.0f,540.f));
    assert(output[0].top<960 && output[0].bottom>960);
    auto old=output[0];
    assert(oxide_esp_project_v1(&f,&p,output,0)==OXIDE_ESP_OUTPUT_TOO_SMALL);
    assert(output[0].entity_id==old.entity_id && eq(output[0].left,old.left));
    assert(oxide_esp_project_v1(nullptr,&p,output,2)==OXIDE_ESP_BAD_INPUT);
    assert(oxide_esp_project_v1(&f,nullptr,output,2)==OXIDE_ESP_BAD_INPUT);
    assert(oxide_esp_project_v1(&f,&p,nullptr,2)==OXIDE_ESP_BAD_INPUT);
    f.abi_version=2;
    assert(oxide_esp_project_v1(&f,&p,output,2)==OXIDE_ESP_ABI_MISMATCH);f.abi_version=1;
    f.player_count=257;
    assert(oxide_esp_project_v1(&f,&p,output,2)==OXIDE_ESP_TOO_MANY_PLAYERS);f.player_count=1;
    f.width=0;
    assert(oxide_esp_project_v1(&f,&p,output,2)==OXIDE_ESP_INVALID_FRAME);f.width=1080;
    f.flags=128;
    assert(oxide_esp_project_v1(&f,&p,output,2)==OXIDE_ESP_INVALID_FRAME);f.flags=0;
    auto q=p;q.flags=0;assert(oxide_esp_project_v1(&f,&q,output,0)==0);
    q=p;q.head[2]=5;q.feet[2]=5;
    assert(oxide_esp_project_v1(&f,&q,output,0)==0);
    q=p;q.head[0]=std::numeric_limits<float>::quiet_NaN();
    assert(oxide_esp_project_v1(&f,&q,output,0)==0);
    f.view_projection[3]=std::numeric_limits<float>::infinity();
    assert(oxide_esp_project_v1(&f,&p,output,2)==OXIDE_ESP_INVALID_FRAME);f=frame();
    auto col=f;
    for(int r=0;r<4;++r)for(int c=0;c<4;++c)col.view_projection[c*4+r]=f.view_projection[r*4+c];
    col.flags=OXIDE_ESP_FRAME_COLUMN_MAJOR;
    oxide_esp_box b{};
    assert(oxide_esp_project_v1(&col,&p,&b,1)==1);
    assert(eq(b.left,old.left)&&eq(b.right,old.right));
    f.player_count=0;
    assert(oxide_esp_project_v1(&f,nullptr,nullptr,0)==0);

    std::mt19937 gen(0x12345678u);
    std::uniform_real_distribution<float> dist(-40.f,40.f);
    std::array<oxide_esp_player,OXIDE_ESP_MAX_PLAYERS> players{};
    std::array<oxide_esp_box,OXIDE_ESP_MAX_PLAYERS+1> boxes{};
    for(unsigned iteration=0;iteration<20000;++iteration){
        auto scene=frame();
        scene.player_count=iteration%257;
        for(uint32_t i=0;i<scene.player_count;++i){
            auto& item=players[i];
            item.flags=i%13==0 ? 0 : OXIDE_ESP_PLAYER_VALID;
            item.entity_id=i+1;
            item.head[0]=dist(gen);item.head[1]=dist(gen);item.head[2]=dist(gen);
            item.feet[0]=item.head[0];item.feet[1]=item.head[1]-1.7f;item.feet[2]=item.head[2];
        }
        boxes[OXIDE_ESP_MAX_PLAYERS].entity_id=0xdeadbeef;
        const int n=oxide_esp_project_v1(&scene,players.data(),boxes.data(),OXIDE_ESP_MAX_PLAYERS);
        assert(n>=0 && n<=int(scene.player_count));
        assert(boxes[OXIDE_ESP_MAX_PLAYERS].entity_id==0xdeadbeef);
        for(int i=0;i<n;++i){
            const auto& rect=boxes[i];
            assert(std::isfinite(rect.left) && std::isfinite(rect.right));
            assert(rect.left>=0 && rect.top>=0 && rect.right<=scene.width && rect.bottom<=scene.height);
            assert(rect.left<rect.right && rect.top<rect.bottom);
        }
    }
    std::puts("C ABI projection PASS (20,000 randomized frames, sanitizer compatible)");
}
