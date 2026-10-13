#ifndef OXIDE_TIMED_BRIDGE_H
#define OXIDE_TIMED_BRIDGE_H
#include "esp_bridge.h"
#ifdef __cplusplus
extern "C" {
#endif
#define OXIDE_ESP_TIMED_ABI_VERSION 2u
#define OXIDE_ESP_MAX_AGE_MS 5000u

// Every timestamp must come from the SAME monotonic millisecond clock.
// Do not pass wall-clock time for one timestamp and elapsed time for another.
typedef struct oxide_esp_timed_frame {
    uint32_t abi_version;
    uint32_t reserved;
    uint64_t now_ms;
    uint64_t camera_sample_ms;
    uint32_t max_age_ms;
    uint32_t reserved2;
    oxide_esp_frame camera;
} oxide_esp_timed_frame;

typedef struct oxide_esp_timed_player {
    oxide_esp_player sample;
    uint64_t sample_ms;
} oxide_esp_timed_player;

// Additional errors beyond those declared in esp_bridge.h:
#define OXIDE_ESP_CAMERA_STALE (-6)
#define OXIDE_ESP_DUPLICATE_ENTITY (-7)

// Return a count >=0 or a negative code. Never writes output on error.
// Expired or future player samples are silently excluded (fail closed).
// Camera timestamp must be current; no extrapolation is performed.
// Does not read third-party processes or obtain positions from a game.
int oxide_esp_project_timed_v2(const oxide_esp_timed_frame* frame,
                               const oxide_esp_timed_player* players,
                               oxide_esp_box* out_boxes,
                               uint32_t out_capacity);
#ifdef __cplusplus
}
#endif
#endif
