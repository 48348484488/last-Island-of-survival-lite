#pragma once
// Provisional low-poly placement visualization; the native collision/crafting
// rules are functional, but these shapes are NOT original Unity build meshes.
#include "raylib.h"
#include "native_build_craft.hpp"
namespace LIOS_BUILD {
inline void drawPart(Vector3 position,Vector3 extents,Color tint,bool preview) {
    DrawCubeV(position,extents,tint);
    if(preview)DrawCubeWiresV(position,extents,tint.a>100?DARKGREEN:MAROON);
}
inline void drawPiece(const Piece &piece,bool preview=false,bool possible=true){
    const float x=world(piece.gx),z=world(piece.gz);
    const Color wood=preview?(possible?Color{80,220,110,110}:Color{255,65,70,110}):
                             Color{145,105,65,255};
    const Color stone=preview?(possible?Color{80,220,110,110}:Color{255,65,70,110}):
                              Color{136,138,131,255};
    const Color metal=preview?(possible?Color{80,220,110,110}:Color{255,65,70,110}):
                              Color{114,130,136,255};
    switch(piece.type){
      case Foundation:
          drawPart({x,.2f,z},{3.f,.4f,3.f},stone,preview);break;
      case Wall:
      case Doorway:
      case Window:{
          const int rot=turn(piece.turn);
          const bool alongZ=(rot%2)==0;
          const float normal=rot==0?-1.5f:rot==1?1.5f:rot==2?1.5f:-1.5f;
          const float wx=x+(alongZ?0.f:normal),wz=z+(alongZ?normal:0.f);
          if(piece.type==Wall){
              drawPart({wx,1.5f,wz},alongZ?Vector3{3.f,2.6f,.22f}:
                        Vector3{.22f,2.6f,3.f},wood,preview);
          }else if(piece.type==Window){
              // A window has two posts, header and a waist-high sill.
              for(int sign:{-1,1})
                  drawPart({wx+(alongZ?sign*1.35f:0.f),1.5f,
                            wz+(alongZ?0.f:sign*1.35f)},
                           {.28f,2.6f,.28f},wood,preview);
              drawPart({wx,2.67f,wz},alongZ?Vector3{2.9f,.28f,.28f}:
                        Vector3{.28f,.28f,2.9f},wood,preview);
              drawPart({wx,.78f,wz},alongZ?Vector3{2.9f,1.05f,.28f}:
                        Vector3{.28f,1.05f,2.9f},wood,preview);
          }else{
              // Doorway remains passable through its center.
              for(int sign:{-1,1})
                  drawPart({wx+(alongZ?sign*1.18f:0.f),1.42f,
                            wz+(alongZ?0.f:sign*1.18f)},
                           {.34f,2.45f,.34f},wood,preview);
              drawPart({wx,2.7f,wz},alongZ?Vector3{2.7f,.35f,.38f}:
                        Vector3{.38f,.35f,2.7f},wood,preview);
          }
          break;
      }
      case Stair:{
          for(int step=0;step<5;step++){
              float along=(step-2)*.55f;
              float height=.24f+.42f*step;
              bool rotate=turn(piece.turn)%2==1;
              drawPart({x+(rotate?along:0.f),height/2.f,
                        z+(rotate?0.f:along)},
                       rotate?Vector3{.57f,height,2.7f}:
                              Vector3{2.7f,height,.57f},wood,preview);
          }
          break;
      }
      case Obstacle:
          for(int i=0;i<3;i++){
              float along=(i-1)*.33f;
              drawPart({x+along,.65f,z},{.24f,1.3f,.24f},wood,preview);
          }
          break;
      case Roof:
          drawPart({x,2.9f,z},{3.15f,.25f,3.15f},wood,preview);break;
      case Storage:
          drawPart({x,.66f,z},{1.25f,1.f,1.f},wood,preview);
          drawPart({x,1.20f,z},{1.35f,.12f,1.1f},metal,preview);break;
      case Campfire:
          drawPart({x,.13f,z},{1.0f,.25f,.85f},stone,preview);
          if(!preview){
              DrawSphere({x,.44f,z},.23f,ORANGE);
              DrawSphere({x,.65f,z},.12f,GOLD);
          }
          break;
      default:break;
    }
}
} // namespace LIOS_BUILD
