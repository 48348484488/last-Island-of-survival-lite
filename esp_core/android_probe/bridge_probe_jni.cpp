#include <jni.h>
#include <cmath>
#include <cstdint>
#include <limits>
#include "esp_bridge.h"

namespace {
oxide_esp_frame camera() noexcept {
    oxide_esp_frame f{};
    f.abi_version=OXIDE_ESP_ABI_VERSION;
    f.width=1080;
    f.height=1920;
    f.player_count=1;
    constexpr float n=0.1f, far=100.f;
    const float data[16]={
        1,0,0,0,
        0,1,0,0,
        0,0,(far+n)/(n-far),(2.f*far*n)/(n-far),
        0,0,-1,0
    };
    for (unsigned i=0;i<16;++i)f.view_projection[i]=data[i];
    return f;
}
bool near(float x,float y) noexcept {
    return std::isfinite(x) && std::abs(x-y)<0.08f;
}
int verify() noexcept {
    oxide_esp_player p{};
    p.entity_id=0x123456789abcdULL;
    p.head[1]=1.f;p.head[2]=-5.f;
    p.feet[1]=-1.f;p.feet[2]=-5.f;
    p.flags=OXIDE_ESP_PLAYER_VALID;
    oxide_esp_box out[2]{};
    for(int i=0;i<20000;++i) {
        auto f=camera();
        p.entity_id=static_cast<uint64_t>(i+1);
        p.head[0]=static_cast<float>(i%11)*0.01f;
        p.feet[0]=p.head[0];
        const int n=oxide_esp_project_v1(&f,&p,out,2);
        if(n!=1 || out[0].entity_id!=p.entity_id) return 1;
        if(!(out[0].left<out[0].right && out[0].top<out[0].bottom))return 2;
        if(!near((out[0].left+out[0].right)/2.f,540.f+108.f*p.head[0]))return 3;
        if(oxide_esp_project_v1(&f,&p,out,0)!=OXIDE_ESP_OUTPUT_TOO_SMALL)return 4;
        p.head[2]=5.f;
        if(oxide_esp_project_v1(&f,&p,out,2)!=0)return 5;
        p.head[2]=-5.f;
        auto invalid=f;
        invalid.view_projection[3]=std::numeric_limits<float>::quiet_NaN();
        if(oxide_esp_project_v1(&invalid,&p,out,2)!=OXIDE_ESP_INVALID_FRAME)return 6;
        invalid=f; invalid.width=0;
        if(oxide_esp_project_v1(&invalid,&p,out,2)!=OXIDE_ESP_INVALID_FRAME)return 7;
    }
    return 0;
}
}
extern "C" JNIEXPORT jint JNICALL
Java_com_oxide_bridgeprobe_MainActivity_runNativeSmoke(JNIEnv*, jclass) {
    return static_cast<jint>(verify());
}
