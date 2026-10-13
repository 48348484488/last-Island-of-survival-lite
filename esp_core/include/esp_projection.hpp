#pragma once
#include <algorithm>
#include <array>
#include <cmath>

namespace oxide_lab {
struct Vec3 { float x, y, z; };
struct Vec2 { float x, y; };
struct Mat4 { std::array<float,16> m; }; // Row-major view-projection matrix
struct Viewport { float width, height; };
struct Rect { float left, top, right, bottom; };
enum class DepthRange { OpenGL, ZeroToOne };

inline bool finite(float x) noexcept { return std::isfinite(x); }
inline bool finite(const Vec3& v) noexcept { return finite(v.x) && finite(v.y) && finite(v.z); }
inline bool valid(const Viewport& v) noexcept {
    return finite(v.width) && finite(v.height) && v.width >= 1.0f && v.height >= 1.0f &&
           v.width <= 16384.0f && v.height <= 16384.0f;
}
inline bool valid(const Mat4& m) noexcept {
    for (float f : m.m) if (!finite(f)) return false;
    return true;
}

// Mathematical projection only. Requires genuine world coordinates and
// view-projection matrix from an authorized in-game data provider.
inline bool world_to_screen(const Mat4& vp, const Viewport& viewport,
                            const Vec3& p, Vec2& screen,
                            DepthRange depth_range = DepthRange::OpenGL) noexcept {
    if (!valid(vp) || !valid(viewport) || !finite(p)) return false;
    const auto& m = vp.m;
    const double x = double(m[0])*p.x + double(m[1])*p.y + double(m[2])*p.z + m[3];
    const double y = double(m[4])*p.x + double(m[5])*p.y + double(m[6])*p.z + m[7];
    const double z = double(m[8])*p.x + double(m[9])*p.y + double(m[10])*p.z + m[11];
    const double w = double(m[12])*p.x + double(m[13])*p.y + double(m[14])*p.z + m[15];
    if (!std::isfinite(x) || !std::isfinite(y) || !std::isfinite(z) || !std::isfinite(w) || w <= 1e-6) return false;
    const double near_clip = depth_range == DepthRange::OpenGL ? -w : 0.0;
    if (z < near_clip || z > w || std::abs(x) > w || std::abs(y) > w) return false;
    const double sx = (x/w*0.5 + 0.5)*viewport.width;
    const double sy = (0.5 - y/w*0.5)*viewport.height;
    if (!std::isfinite(sx) || !std::isfinite(sy)) return false;
    screen = {float(sx),float(sy)};
    return finite(screen.x) && finite(screen.y);
}

// Converts visible head/foot coordinates to a clipped rectangle.
// Fails closed: missing/invalid data results in no box, never fake ESP.
inline bool player_box(const Mat4& vp, const Viewport& viewport,
                       const Vec3& head, const Vec3& foot, Rect& out,
                       DepthRange depth_range = DepthRange::OpenGL) noexcept {
    Vec2 a{}, b{};
    if (!world_to_screen(vp, viewport, head, a, depth_range) ||
        !world_to_screen(vp, viewport, foot, b, depth_range)) return false;
    const float h = std::abs(a.y-b.y);
    if (!finite(h) || h < 2.0f || h > viewport.height) return false;
    const float cx = (a.x+b.x)/2.0f;
    const float half_w = h*0.225f;
    const Rect r{cx-half_w, std::min(a.y,b.y), cx+half_w, std::max(a.y,b.y)};
    if (!finite(r.left) || !finite(r.top) || !finite(r.right) || !finite(r.bottom)) return false;
    if (r.right < 0.0f || r.left > viewport.width) return false;
    out = {std::max(0.0f,r.left),std::max(0.0f,r.top),
           std::min(viewport.width,r.right),std::min(viewport.height,r.bottom)};
    return out.left < out.right && out.top < out.bottom;
}
}
