#include "esp_timed_bridge.h"
#include <array>
#include <cassert>
#include <cmath>
#include <cstdio>
#include <cstdint>
#include <limits>
#include <random>
#include <type_traits>

static_assert(std::is_standard_layout<oxide_esp_timed_frame>::value,"frame ABI");
static_assert(std::is_standard_layout<oxide_esp_timed_player>::value,"player ABI");
static_assert(sizeof(oxide_esp_timed_frame)==120,"timed frame ABI changed");
static_assert(sizeof(oxide_esp_timed_player)==48,"timed player ABI changed");

static oxide_esp_timed_frame frame() {
    oxide_esp_timed_frame f{};
    f.abi_version=OXIDE_ESP_TIMED_ABI_VERSION;
    f.now_ms=10000;f.camera_sample_ms=9995;f.max_age_ms=250;
    f.camera.abi_version=OXIDE_ESP_ABI_VERSION;
    f.camera.width=1080;f.camera.height=1920;f.camera.player_count=1;
    constexpr float n=0.1f,far=100.f;
    const float mat[16]={1,0,0,0, 0,1,0,0, 0,0,(far+n)/(n-far),
                         (2*far*n)/(n-far), 0,0,-1,0};
    for (int i=0;i<16;++i)f.camera.view_projection[i]=mat[i];
    return f;
}
static oxide_esp_timed_player player(uint64_t id=91) {
    oxide_esp_timed_player p{};
    p.sample.entity_id=id;
    p.sample.head[1]=1.f;p.sample.head[2]=-5.f;
    p.sample.feet[1]=-1.f;p.sample.feet[2]=-5.f;
    p.sample.flags=OXIDE_ESP_PLAYER_VALID;
    p.sample_ms=9990;
    return p;
}
static bool near(float a,float b) {return std::abs(a-b)<0.02f;}
int main() {
    auto f=frame();
    auto p=player();
    std::array<oxide_esp_box,258> boxes{};
    boxes[0].entity_id=0xbeefface;
    assert(oxide_esp_project_timed_v2(&f,&p,boxes.data(),1)==1);
    assert(boxes[0].entity_id==91);
    assert(near((boxes[0].left+boxes[0].right)/2,540.f));
    auto good=boxes[0];
    // Atomic capacity failure: output cannot be partially overwritten.
    boxes[0].entity_id=0xbeefface;
    assert(oxide_esp_project_timed_v2(&f,&p,boxes.data(),0)==OXIDE_ESP_OUTPUT_TOO_SMALL);
    assert(boxes[0].entity_id==0xbeefface);
    boxes[0]=good;

    assert(oxide_esp_project_timed_v2(nullptr,&p,boxes.data(),1)==OXIDE_ESP_BAD_INPUT);
    assert(oxide_esp_project_timed_v2(&f,nullptr,boxes.data(),1)==OXIDE_ESP_BAD_INPUT);
    assert(oxide_esp_project_timed_v2(&f,&p,nullptr,1)==OXIDE_ESP_BAD_INPUT);
    auto bad=f;bad.abi_version=1;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_ABI_MISMATCH);
    bad=f;bad.reserved=1;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_BAD_INPUT);
    bad=f;bad.reserved2=1;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_BAD_INPUT);
    bad=f;bad.max_age_ms=0;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_BAD_INPUT);
    bad=f;bad.max_age_ms=5001;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_BAD_INPUT);
    bad=f;bad.camera_sample_ms=10001;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_CAMERA_STALE);
    bad=f;bad.camera_sample_ms=9749;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_CAMERA_STALE);
    bad=f;bad.camera_sample_ms=9750;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==1);
    bad=f;bad.camera.width=0;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_INVALID_FRAME);
    bad=f;bad.camera.view_projection[5]=std::numeric_limits<float>::quiet_NaN();
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_INVALID_FRAME);
    bad=f;bad.camera.abi_version=11;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_ABI_MISMATCH);
    bad=f;bad.camera.player_count=257;
    assert(oxide_esp_project_timed_v2(&bad,&p,boxes.data(),1)==OXIDE_ESP_TOO_MANY_PLAYERS);

    auto stale=p;stale.sample_ms=9749;
    assert(oxide_esp_project_timed_v2(&f,&stale,boxes.data(),0)==0);
    stale=p;stale.sample_ms=10001;
    assert(oxide_esp_project_timed_v2(&f,&stale,boxes.data(),0)==0);
    stale=p;stale.sample.entity_id=0;
    assert(oxide_esp_project_timed_v2(&f,&stale,boxes.data(),0)==0);
    stale=p;stale.sample.flags=0;
    assert(oxide_esp_project_timed_v2(&f,&stale,boxes.data(),0)==0);

    std::array<oxide_esp_timed_player,2> pair={p,player(92)};
    bad=f;bad.camera.player_count=2;
    assert(oxide_esp_project_timed_v2(&bad,pair.data(),boxes.data(),2)==2);
    pair[1].sample.entity_id=91;
    boxes[0].entity_id=0xbeefface;
    assert(oxide_esp_project_timed_v2(&bad,pair.data(),boxes.data(),2)==OXIDE_ESP_DUPLICATE_ENTITY);
    assert(boxes[0].entity_id==0xbeefface);
    // An expired duplicate is not a conflict.
    pair[1].sample_ms=9749;
    assert(oxide_esp_project_timed_v2(&bad,pair.data(),boxes.data(),1)==1);
    f.camera.player_count=0;
    assert(oxide_esp_project_timed_v2(&f,nullptr,nullptr,0)==0);

    std::array<oxide_esp_timed_player,OXIDE_ESP_MAX_PLAYERS> players{};
    std::mt19937 rng(0x611B1A);
    std::uniform_real_distribution<float> xdist(-15,15);
    for(uint32_t iter=0;iter<20000;++iter) {
        auto scene=frame();
        scene.now_ms=10000+iter;
        scene.camera_sample_ms=scene.now_ms-(iter%251);
        scene.camera.player_count=iter%257;
        for(uint32_t i=0;i<scene.camera.player_count;++i) {
            auto& entry=players[i];
            entry=player(uint64_t(i)+1);
            entry.sample.head[0]=entry.sample.feet[0]=xdist(rng);
            entry.sample_ms=scene.now_ms-(i%400);
            if(i%17==0)entry.sample.flags=0;
        }
        boxes[256].entity_id=0xdeadbeef;
        const int result=oxide_esp_project_timed_v2(
            &scene,players.data(),boxes.data(),OXIDE_ESP_MAX_PLAYERS);
        assert(result>=0 && result<=int(scene.camera.player_count));
        assert(boxes[256].entity_id==0xdeadbeef);
        for(int j=0;j<result;++j) {
            const auto& b=boxes[j];
            assert(b.entity_id!=0);
            assert(std::isfinite(b.left) && std::isfinite(b.right));
            assert(b.left>=0 && b.top>=0 && b.right<=scene.camera.width && b.bottom<=scene.camera.height);
            assert(b.left<b.right && b.top<b.bottom);
        }
    }
    std::puts("Timed camera/player snapshot bridge: PASS, 20,000 bounded frames");
}
