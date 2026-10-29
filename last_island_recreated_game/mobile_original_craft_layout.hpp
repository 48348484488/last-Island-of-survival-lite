#pragma once
// Source-backed RectTransform bounds from original LITE 7.3
// assets/ui/bagandbuildpanel.ab, verified by GitHub Actions #40904727522.
// Units are 1920x1080 reference canvas pixels, top-left origin.
// Bounds for elements animated or driven by Unity layout groups are NOT
// asserted source-exact here.
namespace LIOS_CRAFT_SOURCE {
struct Bounds { float x,y,w,h; };
static constexpr Bounds Back = {1654.0f,0.0f,316.0f,86.0f};
static constexpr Bounds RecipeScrollViewport = {238.8f,208.2f,735.2f,640.6f};
static constexpr Bounds RecipeTemplateCell = {238.8f,208.2f,149.5f,154.1f};
static constexpr Bounds SelectedItemIcon = {1311.9f,249.8f,130.0f,130.0f};
static constexpr Bounds SelectedName = {1037.9f,170.4f,399.4f,57.2f};
static constexpr Bounds SelectedDescription = {1019.8f,397.0f,728.3f,87.9f};
static constexpr Bounds QuantityMinus = {1019.3f,954.3f,70.0f,70.0f};
static constexpr Bounds QuantityPlus = {1302.2f,954.3f,70.0f,70.0f};
static constexpr Bounds QuantityMax = {1376.8f,954.3f,110.0f,70.0f};
// Source positions of category containers; Unity populates their actual
// button sizes with a layout group, so only origins are exact.
static constexpr Bounds MainTabOnSprite = {-33.5f,163.5f,258.8f,86.0f};
static constexpr Bounds MainTabActiveIndicator = {7.4f,192.5f,17.0f,28.0f};
static constexpr Bounds SubTabOnSprite = {255.3f,151.1f,120.0f,35.0f};
static constexpr float MainTabStepY = 86.0f; // Native estimate; Unity dynamically lays out tabs
static constexpr float SubTabStepX = 145.0f; // Source subtype parent positions
static constexpr float SubTabOriginX = 315.3f;
static constexpr float SubTabOriginY = 168.6f;
} // namespace LIOS_CRAFT_SOURCE
