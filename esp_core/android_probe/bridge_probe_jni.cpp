#include <jni.h>
#include <array>
#include <cstring>
#include <cmath>
#include <cstdint>
#include <limits>
#include "esp_bridge.h"
#include "esp_timed_bridge.h"

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
        oxide_esp_timed_frame timed{};
        timed.abi_version=OXIDE_ESP_TIMED_ABI_VERSION;
        timed.now_ms=100000u+static_cast<uint64_t>(i);
        timed.camera_sample_ms=timed.now_ms-5;
        timed.max_age_ms=250;
        timed.camera=f;
        oxide_esp_timed_player observed{};
        observed.sample=p;
        observed.sample_ms=timed.now_ms-9;
        if(oxide_esp_project_timed_v2(&timed,&observed,out,2)!=1) return 8;
        if(out[0].entity_id!=p.entity_id) return 9;
        observed.sample_ms=timed.now_ms-251;
        if(oxide_esp_project_timed_v2(&timed,&observed,out,2)!=0) return 10;
        observed.sample_ms=timed.now_ms-9;
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


/*
 * Caller-provided snapshots (Java → JNI → C++). No process scanning.
 * A null return denotes invalid data, while an empty array denotes
 * a valid snapshot with no visible, fresh players.
 * All arrays are copied into bounded native buffers.
 */
extern "C" JNIEXPORT jfloatArray JNICALL
Java_com_oxide_bridgeprobe_MainActivity_nativeProjectSnapshot(
    JNIEnv* env, jclass,
    jfloatArray matrix, jint width, jint height,
    jlong now_ms, jlong camera_ms, jint max_age_ms, jint flags,
    jlongArray ids, jfloatArray head_feet, jlongArray sample_times) {
    if (!matrix || !ids || !head_feet || !sample_times ||
        width<=0 || height<=0 || now_ms<0 || camera_ms<0 ||
        max_age_ms<=0 || max_age_ms>static_cast<jint>(OXIDE_ESP_MAX_AGE_MS) ||
        (flags & ~(static_cast<jint>(OXIDE_ESP_FRAME_COLUMN_MAJOR | OXIDE_ESP_FRAME_Z_ZERO_TO_ONE)))!=0) {
        return nullptr;
    }
    if (env->GetArrayLength(matrix)!=16) return nullptr;
    const jsize count=env->GetArrayLength(ids);
    if (count<0 || count>static_cast<jsize>(OXIDE_ESP_MAX_PLAYERS) ||
        env->GetArrayLength(head_feet)!=count*6 ||
        env->GetArrayLength(sample_times)!=count) return nullptr;

    oxide_esp_timed_frame frame{};
    frame.abi_version=OXIDE_ESP_TIMED_ABI_VERSION;
    frame.now_ms=static_cast<uint64_t>(now_ms);
    frame.camera_sample_ms=static_cast<uint64_t>(camera_ms);
    frame.max_age_ms=static_cast<uint32_t>(max_age_ms);
    frame.camera.abi_version=OXIDE_ESP_ABI_VERSION;
    frame.camera.flags=static_cast<uint32_t>(flags);
    frame.camera.width=static_cast<uint32_t>(width);
    frame.camera.height=static_cast<uint32_t>(height);
    frame.camera.player_count=static_cast<uint32_t>(count);
    env->GetFloatArrayRegion(matrix,0,16,frame.camera.view_projection);
    if (env->ExceptionCheck()) return nullptr;

    std::array<oxide_esp_timed_player,OXIDE_ESP_MAX_PLAYERS> players{};
    std::array<jlong,OXIDE_ESP_MAX_PLAYERS> raw_ids{};
    std::array<jlong,OXIDE_ESP_MAX_PLAYERS> raw_times{};
    std::array<jfloat,OXIDE_ESP_MAX_PLAYERS*6> raw_coords{};
    if(count) {
        env->GetLongArrayRegion(ids,0,count,raw_ids.data());
        env->GetLongArrayRegion(sample_times,0,count,raw_times.data());
        env->GetFloatArrayRegion(head_feet,0,count*6,raw_coords.data());
        if(env->ExceptionCheck()) return nullptr;
        for(jsize i=0;i<count;++i) {
            if(raw_ids[i]<0 || raw_times[i]<0) return nullptr;
            auto& p=players[static_cast<size_t>(i)];
            p.sample.entity_id=static_cast<uint64_t>(raw_ids[i]);
            p.sample.flags=raw_ids[i]!=0 ? OXIDE_ESP_PLAYER_VALID : 0u;
            p.sample_ms=static_cast<uint64_t>(raw_times[i]);
            for(int j=0;j<3;++j) {
                p.sample.head[j]=raw_coords[6*i+j];
                p.sample.feet[j]=raw_coords[6*i+j+3];
            }
        }
    }
    std::array<oxide_esp_box,OXIDE_ESP_MAX_PLAYERS> projected{};
    int result=oxide_esp_project_timed_v2(&frame,players.data(),projected.data(),OXIDE_ESP_MAX_PLAYERS);
    if (result<0) return nullptr;
    jfloatArray out=env->NewFloatArray(static_cast<jsize>(result*4));
    if (!out) return nullptr;
    if (!result) return out;

    std::array<jfloat,OXIDE_ESP_MAX_PLAYERS*4> coords{};
    for(int i=0;i<result;++i) {
        coords[4*i]=projected[static_cast<size_t>(i)].left;
        coords[4*i+1]=projected[static_cast<size_t>(i)].top;
        coords[4*i+2]=projected[static_cast<size_t>(i)].right;
        coords[4*i+3]=projected[static_cast<size_t>(i)].bottom;
    }
    env->SetFloatArrayRegion(out,0,static_cast<jsize>(result*4),coords.data());
    if (env->ExceptionCheck()) return nullptr;
    return out;
}
