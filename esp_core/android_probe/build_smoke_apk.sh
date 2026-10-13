#!/usr/bin/env bash
# Build an ISOLATED diagnostic JNI+Canvas overlay app. Never packages the game.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
if [[ $# -ne 1 ]]; then echo "Usage: $0 OUTPUT_DIRECTORY" >&2; exit 64; fi
OUT="$1"
SDK="$ANDROID_HOME"
NDK_VERSION="27.2.12479018"
BT_VERSION="35.0.0"
BIN="$SDK/ndk/$NDK_VERSION/toolchains/llvm/prebuilt/linux-x86_64/bin"
BT="$SDK/build-tools/$BT_VERSION"
PLATFORM="$SDK/platforms/android-35/android.jar"
for tool in "$BIN/aarch64-linux-android26-clang++" "$BIN/llvm-readelf" "$BT/d8" "$BT/aapt" "$BT/zipalign" "$BT/apksigner" "$PLATFORM"; do
  if [[ ! -e "$tool" ]]; then echo "Missing Android build dependency: $tool" >&2; exit 69; fi
done
mkdir -p "$OUT/build/classes" "$OUT/build/dex" "$OUT/build/lib/arm64-v8a"
LIBDIR="$OUT/build/lib/arm64-v8a"
"$BIN/aarch64-linux-android26-clang++" -O2 -std=c++17 -fPIC -shared \
  -fstack-protector-strong -Wl,-z,relro -Wl,-z,now -Wl,--no-undefined \
  -I"$ROOT/esp_core/include" "$ROOT/esp_core/src/esp_bridge.cpp" \
  "$ROOT/esp_core/src/esp_timed_bridge.cpp" \
  -o "$LIBDIR/liboxide_esp_bridge.so"
"$BIN/aarch64-linux-android26-clang++" -O2 -std=c++17 -fPIC -shared \
  -fstack-protector-strong -Wl,-z,relro -Wl,-z,now -Wl,--no-undefined \
  -I"$ROOT/esp_core/include" "$ROOT/esp_core/android_probe/bridge_probe_jni.cpp" \
  -L"$LIBDIR" -loxide_esp_bridge \
  -o "$LIBDIR/liboxide_esp_probe.so"
"$BIN/llvm-readelf" -h "$LIBDIR/liboxide_esp_probe.so" > "$OUT/elf_header.txt"
"$BIN/llvm-readelf" --wide --dyn-syms "$LIBDIR/liboxide_esp_probe.so" > "$OUT/symbols.txt"
"$BIN/llvm-readelf" -d "$LIBDIR/liboxide_esp_probe.so" > "$OUT/dynamic.txt"
grep -q 'Machine:.*AArch64' "$OUT/elf_header.txt"
grep -q 'liboxide_esp_bridge.so' "$OUT/dynamic.txt"
grep -q 'Java_com_oxide_bridgeprobe_MainActivity_runNativeSmoke' "$OUT/symbols.txt"
if grep -q 'libc++_shared.so' "$OUT/dynamic.txt"; then
  cp "$SDK/ndk/$NDK_VERSION/toolchains/llvm/prebuilt/linux-x86_64/sysroot/usr/lib/aarch64-linux-android/libc++_shared.so" "$LIBDIR/"
fi
javac --release 8 -cp "$PLATFORM" -d "$OUT/build/classes" \
  "$ROOT"/esp_core/android_probe/src/com/oxide/bridgeprobe/*.java
"$BT/d8" --min-api 26 --lib "$PLATFORM" \
  --output "$OUT/build/dex" "$OUT"/build/classes/com/oxide/bridgeprobe/*.class
"$BT/aapt" package -f -M "$ROOT/esp_core/android_probe/AndroidManifest.xml" \
  -I "$PLATFORM" -F "$OUT/build/unsigned.apk"
cp "$OUT/build/dex/classes.dex" "$OUT/build/classes.dex"
(cd "$OUT/build" && zip -q -u unsigned.apk classes.dex)
(cd "$OUT/build" && zip -q -u unsigned.apk lib/arm64-v8a/*.so)
"$BT/zipalign" -f 4 "$OUT/build/unsigned.apk" "$OUT/build/aligned.apk"
keytool -genkeypair -noprompt -keystore "$OUT/build/debug.jks" \
  -storepass android -keypass android -alias androiddebugkey \
  -dname "CN=Debug,OU=Testing,O=Oxide Lab,C=BR" \
  -keyalg RSA -keysize 2048 -validity 1000
"$BT/apksigner" sign --ks "$OUT/build/debug.jks" \
  --ks-key-alias androiddebugkey --ks-pass pass:android \
  --key-pass pass:android \
  --out "$OUT/oxide-bridge-jni-smoke.apk" "$OUT/build/aligned.apk"
"$BT/apksigner" verify --verbose "$OUT/oxide-bridge-jni-smoke.apk"
python3 "$ROOT/esp_core/android_probe/check_dex_classes.py" "$OUT/oxide-bridge-jni-smoke.apk"
sha256sum "$OUT/oxide-bridge-jni-smoke.apk" > "$OUT/sha256.txt"
echo "ISOLATED_PROBE_APK_BUILD=PASS"
