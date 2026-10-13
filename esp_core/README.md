# Oxide ESP native bridge — 1.14.11949

**Purpose:** Convert authorized camera / player snapshots to screen-space rectangles using deterministic C++17 code.

### Implemented
- `oxide_esp_project_v1`: stable C ABI, no allocations, max 256 player snapshots per call.
- Row-major and column-major view-projection matrices; configurable OpenGL vs zero-to-one clip depth.
- Reject invalid viewport, NaN/Infinity, points behind the camera or outside the view.
- Validate buffer capacity before writing any output (no partial results).
- Native Android ARM64 cross-compiled shared library `liboxide_esp_bridge.so`, verified by GitHub Actions.
- 20,000 randomized input frames tested under ASan/UBSan (plus projection unit tests).

### Frame freshness checks (v2)

`oxide_esp_project_timed_v2` takes bounded frame/player snapshots with timestamps from one monotonic millisecond clock. It rejects stale or future camera samples, discards expired or future player samples, validates entity ID uniqueness, and guarantees that a capacity error cannot partially overwrite the output buffer. The accepted freshness window is 1–5000 ms (caller chooses the actual threshold).

This prevents some classes of flickering or stale markers **only after a legitimate source of real positions and view-projection matrices is available**. The application does not authenticate that source, and these checks cannot manufacture missing player data. In particular, timestamps and positions fabricated for tests are never evidence of multiplayer replication.

### What is NOT implemented
- No player acquisition from Oxide, no reads of third-party process memory.
- No Unity camera discovery, no runtime method hooks, no IPC or network interception.
- No OpenGL overlay or finished Android APK. The library cannot be injected and is not a working game ESP.
- No guarantee of crash-free gameplay: the game has not been tested using this library.

### Intended authorized integration

An in-process integration with approved game code must provide:
- A correct 4 × 4 view-projection matrix, including row/column convention and clip-depth range.
- Timestamped real head/foot positions in the same world coordinate frame.
- A bounded list of valid entity IDs/positions.
- An app-owned renderer to draw returned screen rectangles.

Before any user-facing build: verify camera conventions with real frames, run native ARM64 tests, exercise lifecycle/orientation changes, and check actual process crash logs. Do not infer that an asset string or static prefab field proves runtime player replication.

### CI

- [Native sanitizer and ARM64 build](../../actions/workflows/esp-arm64.yml)
- [UnityFS resource scan](../../actions/workflows/unityfs-audit.yml)

Use the reports and workflow artifacts in GitHub Actions; binary releases are not attached to this repository.
