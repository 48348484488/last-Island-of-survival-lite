#!/usr/bin/env bash
# Build an isolated arm64-v8a JNI APK to test x86 Android native translation.
# This is not Oxide, not an APK modification and not a security bypass.
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
SDK="${ANDROID_HOME:-${ANDROID_SDK_ROOT:-}}"
WORK="${1:-${RUNNER_TEMP:-/tmp}/oxide-nativebridge-build}"
OUTPUT="${2:-$WORK/oxide-nativebridge-probe.apk}"
if [ -z "$SDK" ] || [ ! -f "$SDK/platforms/android-35/android.jar" ]; then
  echo 'BLOCKED_ANDROID35_SDK_PLATFORM_MISSING' >&2
  exit 2
fi
build_tools="$(find "$SDK/build-tools" -mindepth 1 -maxdepth 1 -type d -print | sort -V | tail -n 1)"
if [ -z "$build_tools" ]; then
  echo 'BLOCKED_ANDROID_SDK_BUILD_TOOLS_MISSING' >&2
  exit 2
fi
for tool in aapt d8 zipalign apksigner; do
  test -x "$build_tools/$tool" || {
    echo "BLOCKED_MISSING_BUILD_TOOL: $tool" >&2
    exit 2
  }
done
for tool in clang javac keytool readelf; do
  command -v "$tool" >/dev/null || {
    echo "BLOCKED_MISSING_HOST_COMPILER: $tool" >&2
    exit 2
  }
done
mkdir -p "$WORK/classes" "$WORK/dex" "$WORK/stage/lib/arm64-v8a" "$(dirname "$OUTPUT")"
clang --target=aarch64-linux-android24 -shared -fPIC -nostdlib -fuse-ld=lld \
  -Wl,--no-undefined -Wl,-soname,liboxideprobe.so -Wl,-z,max-page-size=16384 \
  "$ROOT/tools/oxide_nativebridge_probe/native_probe.c" \
  -o "$WORK/stage/lib/arm64-v8a/liboxideprobe.so"
readelf -h "$WORK/stage/lib/arm64-v8a/liboxideprobe.so" > "$WORK/arm64-elf-header.txt"
grep -Fq 'AArch64' "$WORK/arm64-elf-header.txt"
javac -source 8 -target 8 -encoding UTF-8 \
  -classpath "$SDK/platforms/android-35/android.jar" \
  -d "$WORK/classes" "$ROOT/tools/oxide_nativebridge_probe/ProbeActivity.java"
"$build_tools/d8" --min-api 23 --lib "$SDK/platforms/android-35/android.jar" \
  --output "$WORK/dex" "$WORK/classes/org/oxideprobe/ProbeActivity.class"
cp "$WORK/dex/classes.dex" "$WORK/stage/classes.dex"
"$build_tools/aapt" package -f \
  -M "$ROOT/tools/oxide_nativebridge_probe/AndroidManifest.xml" \
  -I "$SDK/platforms/android-35/android.jar" \
  -F "$WORK/probe-unsigned.apk"
(
  cd "$WORK/stage"
  "$build_tools/aapt" add "$WORK/probe-unsigned.apk" \
    classes.dex lib/arm64-v8a/liboxideprobe.so
)
"$build_tools/zipalign" -f 4 "$WORK/probe-unsigned.apk" "$WORK/probe-aligned.apk"
keytool -genkeypair -noprompt -alias temporarydiagnostic -keyalg RSA \
  -keysize 2048 -validity 1 -dname "CN=Local Test Only" \
  -keystore "$WORK/temporary-debug-keystore.jks" \
  -storepass android -keypass android > "$WORK/keytool.log" 2>&1
"$build_tools/apksigner" sign --ks "$WORK/temporary-debug-keystore.jks" \
  --ks-key-alias temporarydiagnostic \
  --ks-pass pass:android --key-pass pass:android \
  --out "$OUTPUT" "$WORK/probe-aligned.apk"
"$build_tools/apksigner" verify --verbose "$OUTPUT"
"$build_tools/aapt" dump badging "$OUTPUT" > "$WORK/apk-badging.txt"
grep -Fq "package: name='org.oxideprobe'" "$WORK/apk-badging.txt"
grep -Fq "native-code: 'arm64-v8a'" "$WORK/apk-badging.txt"
python3 - "$OUTPUT" <<'PY'
import json
import sys
import hashlib
from pathlib import Path
from zipfile import ZipFile
apk = Path(sys.argv[1])
with ZipFile(apk) as archive:
    names = archive.namelist()
    assert names.count("classes.dex") == 1
    assert names.count("lib/arm64-v8a/liboxideprobe.so") == 1
    assert not any(x.startswith("lib/") and not x.startswith("lib/arm64-v8a/")
                   for x in names)
report = {
    "status": "INDEPENDENT_ARM64_DIAGNOSTIC_APK_BUILT_NOT_EXECUTED",
    "package": "org.oxideprobe",
    "arm64_native_library": True,
    "apk_sha256": hashlib.sha256(apk.read_bytes()).hexdigest(),
    "apk_size_bytes": apk.stat().st_size,
    "original_oxide_apk_modified": False,
    "arm64_jni_executed": False,
    "gameplay_verified": False,
    "warning": "Only a purpose-built diagnostic APK; build success does not prove translation.",
}
path = apk.with_suffix(".json")
path.write_text(json.dumps(report, indent=2) + "\n")
print(json.dumps(report, sort_keys=True))
PY
# Ephemeral signing material must NEVER be included in CI diagnostic artifacts.
rm -f "$WORK/temporary-debug-keystore.jks"
