#include "esp_timed_bridge.h"

namespace {
// Strictly bounded differences; no subtracting until ordering is established.
bool fresh(uint64_t timestamp, uint64_t now, uint32_t max_age) noexcept {
    return timestamp <= now && now - timestamp <= max_age;
}

bool usable(const oxide_esp_timed_player& p,
            uint64_t now, uint32_t max_age) noexcept {
    return p.sample.entity_id != 0 &&
           p.sample.flags == OXIDE_ESP_PLAYER_VALID &&
           fresh(p.sample_ms, now, max_age);
}
}

extern "C" int oxide_esp_project_timed_v2(
    const oxide_esp_timed_frame* input,
    const oxide_esp_timed_player* players,
    oxide_esp_box* output, uint32_t capacity) {
    if (!input) return OXIDE_ESP_BAD_INPUT;
    if (input->abi_version != OXIDE_ESP_TIMED_ABI_VERSION)
        return OXIDE_ESP_ABI_MISMATCH;
    if (input->reserved != 0 || input->reserved2 != 0 ||
        input->max_age_ms == 0 || input->max_age_ms > OXIDE_ESP_MAX_AGE_MS)
        return OXIDE_ESP_BAD_INPUT;
    if (input->camera.player_count > OXIDE_ESP_MAX_PLAYERS)
        return OXIDE_ESP_TOO_MANY_PLAYERS;
    if (input->camera.player_count && !players)
        return OXIDE_ESP_BAD_INPUT;
    if (!fresh(input->camera_sample_ms,input->now_ms,input->max_age_ms))
        return OXIDE_ESP_CAMERA_STALE;

    // Validate the camera even when no players are in view.
    const oxide_esp_frame& cam=input->camera;
    // The base function expects a count-zero frame for camera-only validation.
    oxide_esp_frame empty_cam=cam;
    empty_cam.player_count=0;
    const int checked_camera=oxide_esp_project_v1(&empty_cam,nullptr,nullptr,0);
    if (checked_camera<0) return checked_camera;

    // Prevent duplicate IDs within a fresh sample set: source data is ambiguous.
    for (uint32_t i=0;i<cam.player_count;++i) {
        if (!usable(players[i],input->now_ms,input->max_age_ms)) continue;
        for (uint32_t j=i+1;j<cam.player_count;++j) {
            if (usable(players[j],input->now_ms,input->max_age_ms) &&
                players[i].sample.entity_id == players[j].sample.entity_id)
                return OXIDE_ESP_DUPLICATE_ENTITY;
        }
    }

    // Pass 1 is non-writing. On overflow, all output remains untouched.
    uint32_t needed=0;
    for (uint32_t i=0;i<cam.player_count;++i) {
        if (!usable(players[i],input->now_ms,input->max_age_ms)) continue;
        oxide_esp_frame one=cam;one.player_count=1;
        oxide_esp_box tmp{};
        int n=oxide_esp_project_v1(&one,&players[i].sample,&tmp,1);
        if (n<0) return n;
        if (n>0) ++needed;
    }
    if (needed>capacity) return OXIDE_ESP_OUTPUT_TOO_SMALL;
    if (needed && !output) return OXIDE_ESP_BAD_INPUT;

    // Pass 2 writes only after every validation and capacity check succeeds.
    uint32_t written=0;
    for (uint32_t i=0;i<cam.player_count;++i) {
        if (!usable(players[i],input->now_ms,input->max_age_ms)) continue;
        oxide_esp_frame one=cam;one.player_count=1;
        oxide_esp_box tmp{};
        if (oxide_esp_project_v1(&one,&players[i].sample,&tmp,1)==1)
            output[written++]=tmp;
    }
    return static_cast<int>(written);
}
