#!/usr/bin/env bash
# Standalone Android HardwareBuffer app; no original game files or libraries.
set -euo pipefail
if [[ $# -ne 1 ]]; then echo "Usage: $0 OUTPUT_DIRECTORY" >&2; exit 64; fi
OUT="$1"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SDK="$ANDROID_HOME"
if [[ -z "$SDK" ]]; then echo "ANDROID_HOME required" >&2; exit 69; fi
BT="$SDK/build-tools/35.0.0"
ANDROID_JAR="$SDK/platforms/android-35/android.jar"
for x in "$BT/aapt" "$BT/d8" "$BT/zipalign" "$BT/apksigner" "$ANDROID_JAR"; do
  if [[ ! -e "$x" ]]; then echo "Missing Android SDK $x" >&2; exit 69; fi
done
mkdir -p "$OUT/classes" "$OUT/dex"
javac --release 8 -cp "$ANDROID_JAR" -d "$OUT/classes" \
  "$ROOT/graphics_lab/src/com/oxide/grallocprobe/MainActivity.java"
"$BT/d8" --min-api 29 --lib "$ANDROID_JAR" \
  --output "$OUT/dex" "$OUT/classes/com/oxide/grallocprobe/MainActivity.class"
"$BT/aapt" package -f -M "$ROOT/graphics_lab/AndroidManifest.xml" \
  -I "$ANDROID_JAR" -F "$OUT/unsigned.apk"
cp "$OUT/dex/classes.dex" "$OUT/classes.dex"
(cd "$OUT" && zip -q -u unsigned.apk classes.dex)
"$BT/zipalign" -f 4 "$OUT/unsigned.apk" "$OUT/aligned.apk"
keytool -genkeypair -noprompt -keystore "$OUT/debug.jks" \
  -storepass android -keypass android -alias androiddebugkey \
  -dname "CN=Standalone Graphics Probe,OU=Testing,O=Virtual Android Lab,C=BR" \
  -keyalg RSA -keysize 2048 -validity 1000
"$BT/apksigner" sign --ks "$OUT/debug.jks" \
  --ks-key-alias androiddebugkey --ks-pass pass:android \
  --key-pass pass:android --out "$OUT/gralloc-probe.apk" "$OUT/aligned.apk"
"$BT/apksigner" verify --verbose "$OUT/gralloc-probe.apk"
sha256sum "$OUT/gralloc-probe.apk" > "$OUT/sha256.txt"
echo "INDEPENDENT_ANDROID_GRALLOC_APK_BUILD=PASS"
