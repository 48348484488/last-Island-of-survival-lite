#pragma once
// Audio: source-named Unity M4 outdoor loop and loop-end samples, plus
// previously recovered axe samples. Steps/jump/landing are TEMPORARY
// synthesized placeholders pending source AudioClip verification.
#include "raylib.h"
#include <algorithm>
#include <cmath>
#include <cstdint>
#include <vector>
namespace LIOS_AUDIO {
struct Audio {
 bool initialized=false,enabled=true,m4Shooting=false;
 Sound m4={},m4End={},axe={},wood={},ore={},step={},jump={},land={};
 static Sound synth(bool jumpSound,bool landing){
    constexpr int hz=22050;
    const float duration=landing?0.15f:(jumpSound?0.19f:0.085f);
    int n=int(hz*duration);
    std::vector<int16_t> buffer(n);
    uint32_t noise=0x92B4C8u;
    for(int i=0;i<n;i++){
        const float t=float(i)/hz,fade=std::pow(std::max(0.f,1.f-t/duration),2.f);
        noise=noise*1664525u+1013904223u;
        float hiss=(float((noise>>16)&65535)/32767.5f-1.f);
        float pitch=landing?65.f:jumpSound?(280.f-170.f*t/duration):95.f;
        float tone=std::sin(6.2831853f*pitch*t);
        const float amplitude=landing?0.24f:jumpSound?0.14f:0.18f;
        float signal=(jumpSound?tone*.88f+hiss*.12f:tone*.25f+hiss*.75f);
        buffer[i]=int16_t(std::max(-32767.f,std::min(32767.f,signal*fade*amplitude*32767.f)));
    }
    Wave w={};w.frameCount=n;w.sampleRate=hz;w.sampleSize=16;w.channels=1;w.data=buffer.data();
    return LoadSoundFromWave(w);
 }
 void init(){
    InitAudioDevice();
    initialized=IsAudioDeviceReady();
    if(!initialized){TraceLog(LOG_WARNING,"LIOS_AUDIO_DEVICE_NOT_READY");return;}
    // Exact source-labeled Unity weapon event: wq_m4_1 outdoor fire loop
    // and its release tail. Do not play a 2.62s burst for each single shot.
    m4=LoadSound("assets/wq_m4_1_gunshot outdoor loop.wav");
    m4End=LoadSound("assets/wq_m4_1_gunshot outdoor loop end.wav");
    axe=LoadSound("assets/axe_swing_miss.wav");
    wood=LoadSound("assets/axe_chop_tree.wav");
    ore=LoadSound("assets/axe_mine_ore.wav");
    step=synth(false,false);jump=synth(true,false);land=synth(false,true);
    SetMasterVolume(.78f);
    TraceLog(LOG_INFO,"LIOS_AUDIO_READY unity_m4_outdoor_loop=%u loop_end=%u axe=%u wood=%u ore=%u; step_jump=synthesized",
             m4.frameCount,m4End.frameCount,axe.frameCount,wood.frameCount,ore.frameCount);
 }
 void play(Sound &s,float volume=1.f){
    if(initialized&&enabled&&s.frameCount){
       SetSoundVolume(s,volume);PlaySound(s);
    }
 }
 void fireM4(){
    if(!initialized||!enabled||!m4.frameCount)return;
    if(!IsSoundPlaying(m4))play(m4,.8f);
    m4Shooting=true;
 }
 void releaseM4(){
    if(!m4Shooting)return;
    if(m4.frameCount&&IsSoundPlaying(m4))StopSound(m4);
    if(m4End.frameCount)play(m4End,.72f);
    m4Shooting=false;
 }
 void shutdown(){
    if(!initialized)return;
    for(Sound *s:{&m4,&m4End,&axe,&wood,&ore,&step,&jump,&land})
        if(s->frameCount)UnloadSound(*s);
    CloseAudioDevice();initialized=false;
 }
};
}
