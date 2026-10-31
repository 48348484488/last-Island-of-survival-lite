#include "native_exact_sample_cache.hpp"
#include <cassert>

int main() {
    ExactSampleCache2D<2> cache;
    bool found = true;
    float height = -99.f;
    assert(!cache.lookup(1.f, 2.f, found, height));
    cache.store(1.f, 2.f, true, 3.25f);
    assert(cache.lookup(1.f, 2.f, found, height));
    assert(found && height == 3.25f);
    // Nearby coordinates MUST NOT be rounded into an incorrect collision.
    assert(!cache.lookup(1.f, 2.0001f, found, height));
    cache.store(3.f, 4.f, false, 100.f);
    height = -5.f;
    assert(cache.lookup(3.f, 4.f, found, height));
    assert(!found && height == -5.f);
    cache.store(5.f, 6.f, true, 7.f);
    assert(!cache.lookup(1.f, 2.f, found, height));
    assert(cache.lookup(5.f, 6.f, found, height) && found && height == 7.f);
    cache.clear();
    assert(!cache.lookup(5.f, 6.f, found, height));
    return 0;
}
