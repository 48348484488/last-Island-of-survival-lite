#include "esp_bridge.h"
#include "esp_projection.hpp"

namespace {
using namespace oxide_lab;
bool decode_camera(const oxide_esp_frame& frame, Mat4& camera) noexcept {
    if (frame.flags & ~(OXIDE_ESP_FRAME_COLUMN_MAJOR | OXIDE_ESP_FRAME_Z_ZERO_TO_ONE))
        return false;
    for (unsigned row=0; row<4; ++row) {
        for (unsigned col=0; col<4; ++col) {
            const unsigned idx = (frame.flags & OXIDE_ESP_FRAME_COLUMN_MAJOR) ?
                                 col*4+row : row*4+col;
            camera.m[row*4+col] = frame.view_projection[idx];
        }
    }
    return valid(camera);
}
bool get_box(const oxide_esp_frame& frame, const Mat4& camera,
             const oxide_esp_player& p, oxide_esp_box& out) noexcept {
    if (!(p.flags & OXIDE_ESP_PLAYER_VALID) || (p.flags & ~OXIDE_ESP_PLAYER_VALID))
        return false;
    const Viewport viewport{static_cast<float>(frame.width),static_cast<float>(frame.height)};
    const Vec3 head{p.head[0],p.head[1],p.head[2]};
    const Vec3 feet{p.feet[0],p.feet[1],p.feet[2]};
    const DepthRange depth = (frame.flags & OXIDE_ESP_FRAME_Z_ZERO_TO_ONE) ?
                            DepthRange::ZeroToOne : DepthRange::OpenGL;
    Rect rect{};
    if (!player_box(camera, viewport, head, feet, rect, depth)) return false;
    out={p.entity_id,rect.left,rect.top,rect.right,rect.bottom};
    return true;
}
}
extern "C" int oxide_esp_project_v1(const oxide_esp_frame* frame,
                                     const oxide_esp_player* players,
                                     oxide_esp_box* output, uint32_t capacity) {
    if (!frame) return OXIDE_ESP_BAD_INPUT;
    if (frame->abi_version != OXIDE_ESP_ABI_VERSION) return OXIDE_ESP_ABI_MISMATCH;
    if (frame->player_count > OXIDE_ESP_MAX_PLAYERS) return OXIDE_ESP_TOO_MANY_PLAYERS;
    if (frame->player_count && !players) return OXIDE_ESP_BAD_INPUT;
    if (!frame->width || !frame->height || frame->width>16384 || frame->height>16384)
        return OXIDE_ESP_INVALID_FRAME;
    Mat4 camera{};
    if (!decode_camera(*frame, camera)) return OXIDE_ESP_INVALID_FRAME;
    uint32_t n=0;
    for (uint32_t i=0;i<frame->player_count;++i) {
        oxide_esp_box box{};
        if (get_box(*frame,camera,players[i],box)) ++n;
    }
    if (n > capacity) return OXIDE_ESP_OUTPUT_TOO_SMALL;
    if (n && !output) return OXIDE_ESP_BAD_INPUT;
    uint32_t written=0;
    for (uint32_t i=0;i<frame->player_count;++i) {
        oxide_esp_box box{};
        if (get_box(*frame,camera,players[i],box)) output[written++]=box;
    }
    return static_cast<int>(written);
}
