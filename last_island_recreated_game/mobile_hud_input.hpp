#pragma once
// Original Unity UGUI touch actions with stable per-finger ownership.
// Prevent Android's mouse-synthesized touch from triggering attacks everywhere.
// The Rects are generated from battlepanel.ab (mobile_original_hud_layout.hpp).
#include "raylib.h"
#include "raymath.h"
#include "mobile_original_hud_layout.hpp"
#include <algorithm>
#include <array>
#include <cmath>
#include <map>
#include <set>

namespace LIOS_INPUT {
// Source RectTransforms are in the 1920x1080 prefab canvas. The exported
// gameplay HUD artwork is 200 source pixels higher in the Android viewport:
// verified by matching the real 1080x640 craft/inventory/jump/settings icons
// to their Unity source centers. Apply the SAME offset to hitboxes and
// dynamic HUD graphics. This is an empirical viewport calibration, not proof
// of original Unity CanvasScaler behavior.
static constexpr float HudVisualYOffset = -200.0f;
enum class Action {
 None, Joystick, Look, Attack, Jump, AutoRun, Crouch, Prone,
 Inventory, Craft, Aim, Reload, Settings, QuickChat,
 Slot0, Slot1, Slot2, Slot3, Slot4, Slot5, Slot6
};
inline bool contains(LIOS_HUD::Rect r,float x,float y){
 return x>=r.left&&x<=r.right&&y>=r.top&&y<=r.bottom&&
        r.right>r.left&&r.bottom>r.top;
}
struct Controller {
 std::map<int,Action> owner;
 std::set<Action> held,pressed,released;
 Vector2 stick={0,0},lookDelta={0,0},stickPoint={0,0};
 // Stable touch IDs are stored exclusively in owner/cameraPrevious.
 std::map<int,Vector2> cameraPrevious;
 bool isHeld(Action a)const{return held.count(a)>0;}
 bool justPressed(Action a)const{return pressed.count(a)>0;}
 static Action hit(float x,float y,bool rifle) {
    // Specific UI wins over general actions. Use the FULL original Unity
    // Background RectTransforms, not the smaller icon Rects (and never the
    // zero-sized m_Item anchors), so taps anywhere on a slot change items.
    const struct Candidate{Action a;LIOS_HUD::Rect r;bool rifleOnly;} candidates[]={
      {Action::Settings,LIOS_HUD::Settings,false},
      {Action::QuickChat,LIOS_HUD::QuickChat,false},
      {Action::Craft,LIOS_HUD::Craft,false},
      {Action::Inventory,LIOS_HUD::Inventory,false},
      {Action::Slot0,LIOS_HUD::SlotBox0,false},
      {Action::Slot1,LIOS_HUD::SlotBox1,false},
      {Action::Slot2,LIOS_HUD::SlotBox2,false},
      {Action::Slot3,LIOS_HUD::SlotBox3,false},
      {Action::Slot4,LIOS_HUD::SlotBox4,false},
      {Action::Slot5,LIOS_HUD::SlotBox5,false},
      {Action::Slot6,LIOS_HUD::SlotBox6,false},
      {Action::Aim,LIOS_HUD::Aim,true},
      {Action::Reload,LIOS_HUD::Reload,true},
      {Action::Crouch,LIOS_HUD::Crouch,false},
      {Action::Prone,LIOS_HUD::Prone,false},
      {Action::Jump,LIOS_HUD::Jump,false},
      {Action::AutoRun,LIOS_HUD::Sprint,false},
      {Action::Attack,LIOS_HUD::Attack,false}
    };
    for(const Candidate& v:candidates)
       if((!v.rifleOnly||rifle)&&contains(v.r,x,y))return v.a;
    if(contains(LIOS_HUD::JoystickTrigger,x,y))return Action::Joystick;
    return Action::None;
 }
 void frame(bool rifle,float width,float height,bool modalOpen=false) {
    held.clear();pressed.clear();released.clear();lookDelta={0,0};
    std::set<int> alive;
    float scale=std::min(width/1920.f,height/1080.f);
    if(scale<=0)return;
    float left=(width-1920*scale)*0.5f,top=(height-1080*scale)*0.5f;
    const float centerX=LIOS_HUD::JoystickCenterX,centerY=LIOS_HUD::JoystickCenterY;
    bool stickSeen=false;
    int count=GetTouchPointCount();
    for(int i=0;i<count;i++){
       int id=GetTouchPointId(i);
       Vector2 screen=GetTouchPosition(i);
       float x=(screen.x-left)/scale,y=(screen.y-top)/scale-HudVisualYOffset;
       alive.insert(id);
       auto iter=owner.find(id);
       if(iter==owner.end()){
          Action a=hit(x,y,rifle);
          if(modalOpen && a!=Action::Inventory && a!=Action::Settings && a!=Action::QuickChat) a=Action::None;
          // Only one joystick finger is allowed, regardless of touch count.
          if(a==Action::Joystick)for(auto const& pair:owner)
               if(pair.second==Action::Joystick){a=Action::None;break;}
          if(a==Action::None&&x>=0&&x<=1920&&y>=0&&y<=1080&&x>940)
             a=Action::Look;
          owner[id]=a;iter=owner.find(id);
          // Log only DOWN edges, never every held frame. Runtime evidence can
          // now distinguish a missed hitbox from a later panel-action bug.
          TraceLog(LOG_INFO,
              "LIOS_HUD_TOUCH_DOWN finger=%d action=%d source_x=%.1f source_y=%.1f",
              id,int(a),x,y);
          if(a!=Action::None&&a!=Action::Look&&a!=Action::Joystick)pressed.insert(a);
          if(a==Action::Look)cameraPrevious[id]=screen;
       }
       Action a=iter->second;
       if(a==Action::Joystick){
          float dx=(x-centerX)/LIOS_HUD::JoystickRadius;
          float dy=(y-centerY)/LIOS_HUD::JoystickRadius;
          float length=std::sqrt(dx*dx+dy*dy);
          if(length>1){dx/=length;dy/=length;}
          stick={dx,dy};
          stickPoint={left+(centerX+dx*LIOS_HUD::JoystickRadius)*scale,
                      top+(centerY+dy*LIOS_HUD::JoystickRadius+HudVisualYOffset)*scale};
          stickSeen=true;
       }else if(a==Action::Look){
          Vector2 last=cameraPrevious[id];
          // Android pointer deltas are computed from this finger only.
          lookDelta=Vector2Add(lookDelta,Vector2Subtract(screen,last));
          cameraPrevious[id]=screen;
       }else if(a!=Action::None)held.insert(a);
    }
    for(auto it=owner.begin();it!=owner.end();){
       if(!alive.count(it->first)){
          if(it->second!=Action::None&&it->second!=Action::Look&&it->second!=Action::Joystick)
             released.insert(it->second);
          cameraPrevious.erase(it->first);
          it=owner.erase(it);
       }else ++it;
    }
    if(!stickSeen){
        stick={0,0};
        stickPoint={left+centerX*scale,top+(centerY+HudVisualYOffset)*scale};
    }
 }
};
}
