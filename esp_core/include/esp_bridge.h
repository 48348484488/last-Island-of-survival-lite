#ifndef OXIDE_ESP_BRIDGE_H
#define OXIDE_ESP_BRIDGE_H
#include <stdint.h>
#ifdef __cplusplus
extern "C" {
#endif
#define OXIDE_ESP_ABI_VERSION 1u
#define OXIDE_ESP_MAX_PLAYERS 256u
#define OXIDE_ESP_FRAME_COLUMN_MAJOR (1u << 0)
#define OXIDE_ESP_FRAME_Z_ZERO_TO_ONE (1u << 1)
#define OXIDE_ESP_PLAYER_VALID (1u << 0)

// ABI-compatible snapshots; no game-process pointers.
typedef struct oxide_esp_frame {
    uint32_t abi_version, flags, width, height, player_count;
    float view_projection[16];
} oxide_esp_frame;
typedef struct oxide_esp_player {
    uint64_t entity_id;
    float head[3], feet[3];
    uint32_t flags;
} oxide_esp_player;
typedef struct oxide_esp_box {
    uint64_t entity_id;
    float left, top, right, bottom;
} oxide_esp_box;
enum oxide_esp_result {
    OXIDE_ESP_BAD_INPUT = -1,
    OXIDE_ESP_ABI_MISMATCH = -2,
    OXIDE_ESP_TOO_MANY_PLAYERS = -3,
    OXIDE_ESP_OUTPUT_TOO_SMALL = -4,
    OXIDE_ESP_INVALID_FRAME = -5
};
// Returns the number of rectangles or an error. Does not access the game process.
// On insufficient output capacity, writes zero rectangles.
int oxide_esp_project_v1(const oxide_esp_frame* frame,
                         const oxide_esp_player* players,
                         oxide_esp_box* out_boxes,
                         uint32_t out_capacity);
#ifdef __cplusplus
}
#endif
#endif
