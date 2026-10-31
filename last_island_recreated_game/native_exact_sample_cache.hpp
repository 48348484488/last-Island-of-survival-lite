#pragma once
// Exact-coordinate memoization for immutable geometry queries.
// Never quantizes positions: cached and uncached calls return identical values.
// Invalidate after changing the underlying geometry or coordinate transform.
#include <array>
#include <cstddef>

template <std::size_t Capacity>
class ExactSampleCache2D {
    static_assert(Capacity > 0, "Cache capacity must be positive");
    struct Entry {
        float x = 0, z = 0, value = 0;
        bool valid = false, found = false;
    };
    std::array<Entry, Capacity> entries_{};
    std::size_t next_ = 0;
public:
    void clear() noexcept {
        entries_ = {};
        next_ = 0;
    }
    bool lookup(float x, float z, bool &found, float &value) const noexcept {
        for (const auto &entry : entries_) {
            if (entry.valid && entry.x == x && entry.z == z) {
                found = entry.found;
                if (found) value = entry.value;
                return true;
            }
        }
        return false;
    }
    void store(float x, float z, bool found, float value) noexcept {
        // Android raymob currently builds this translation unit as gnu++11.
        // An Entry with default initializers is not an aggregate in C++11.
        Entry &entry = entries_[next_];
        entry.x = x;
        entry.z = z;
        entry.value = value;
        entry.valid = true;
        entry.found = found;
        next_ = (next_ + 1) % Capacity;
    }
};
