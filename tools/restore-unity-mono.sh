#!/usr/bin/env bash
# Reconstruct a Unity project from a Mono-backend Android APK or XAPK.
#
# Usage:
#   ./restore-unity-mono.sh <input.apk|input.xapk> <output_dir>
#
# Output:
#   <output_dir>/work/apk_extracted/   apk contents
#   <output_dir>/work/obb_extracted/   obb contents (if xapk)
#   <output_dir>/ExportedProject/      Unity project to open

set -euo pipefail

if [[ $# -ne 2 ]]; then
    echo "Usage: $0 <input.apk|input.xapk> <output_dir>" >&2
    exit 1
fi

INPUT="$(realpath "$1")"
OUTPUT_DIR="$(realpath "$2")"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [[ ! -f "$INPUT" ]]; then
    echo "Input not found: $INPUT" >&2
    exit 1
fi

# Tooling
DOTNET_INSTALL_DIR="$HOME/.dotnet"
export PATH="$DOTNET_INSTALL_DIR:$DOTNET_INSTALL_DIR/tools:$PATH"
export DOTNET_ROOT="$DOTNET_INSTALL_DIR"
ASSETRIPPER="$SCRIPT_DIR/tools/AssetRipperConsole/AssetRipperConsole"
if [[ ! -x "$ASSETRIPPER" ]]; then
    echo "AssetRipperConsole missing. Run ./setup.sh first." >&2
    exit 1
fi

mkdir -p "$OUTPUT_DIR/work"
WORK="$OUTPUT_DIR/work"
APK_DIR="$WORK/apk_extracted"
OBB_DIR="$WORK/obb_extracted"
AR_INPUT="$WORK/ar_input"

# 1. If xapk: split into apk + obb
INPUT_LOWER="${INPUT,,}"
if [[ "$INPUT_LOWER" == *.xapk ]]; then
    echo "[1/5] Splitting xapk..."
    XAPK_DIR="$WORK/xapk_extracted"
    rm -rf "$XAPK_DIR" && mkdir -p "$XAPK_DIR"
    unzip -q -o "$INPUT" -d "$XAPK_DIR"
    APK_FILE="$(find "$XAPK_DIR" -maxdepth 2 -type f -name '*.apk' | head -1)"
    if [[ -z "$APK_FILE" ]]; then
        echo "No .apk inside xapk." >&2
        exit 1
    fi
elif [[ "$INPUT_LOWER" == *.apk ]]; then
    APK_FILE="$INPUT"
    XAPK_DIR=""
else
    echo "Input must be .apk or .xapk" >&2
    exit 1
fi

# 2. Extract apk
echo "[2/5] Extracting apk..."
rm -rf "$APK_DIR" && mkdir -p "$APK_DIR"
unzip -q -o "$APK_FILE" -d "$APK_DIR"

if [[ ! -d "$APK_DIR/assets/bin/Data/Managed" ]]; then
    echo "ERROR: this apk does not look like a Mono Unity build (no assets/bin/Data/Managed/). Aborting." >&2
    echo "       For IL2CPP builds you'd need a different pipeline (Il2CppDumper)." >&2
    exit 2
fi

if [[ ! -f "$APK_DIR/assets/bin/Data/Managed/Assembly-CSharp.dll" ]]; then
    echo "WARN: Assembly-CSharp.dll not found. Continuing but output will be partial." >&2
fi

# Extract obb if xapk had one
if [[ -n "$XAPK_DIR" ]]; then
    OBB_FILE="$(find "$XAPK_DIR" -type f -name '*.obb' | head -1 || true)"
    if [[ -n "$OBB_FILE" && -f "$OBB_FILE" ]]; then
        echo "      Extracting obb..."
        rm -rf "$OBB_DIR" && mkdir -p "$OBB_DIR"
        unzip -q -o "$OBB_FILE" -d "$OBB_DIR"
    fi
fi

# 3. Detect Unity version from globalgamemanagers
GGM="$APK_DIR/assets/bin/Data/globalgamemanagers"
if [[ ! -f "$GGM" ]]; then
    echo "ERROR: globalgamemanagers not found at $GGM" >&2
    exit 2
fi
UNITY_VERSION="$(python3 - <<'PY' "$GGM"
import re, sys
with open(sys.argv[1], 'rb') as f:
    head = f.read(64)
m = re.search(rb'(\d+\.\d+\.\d+[a-z]\d+)', head)
print(m.group(1).decode() if m else '')
PY
)"
if [[ -z "$UNITY_VERSION" ]]; then
    echo "ERROR: could not detect Unity version" >&2
    exit 2
fi
echo "[3/5] Detected Unity version: $UNITY_VERSION"

# 4. Patch any .ab files that report a different patch-level version. Same-length swap.
if [[ -d "$OBB_DIR" ]]; then
    echo "      Aligning .ab versions to $UNITY_VERSION..."
    python3 - <<'PY' "$OBB_DIR" "$UNITY_VERSION"
import os, re, sys
root, target = sys.argv[1], sys.argv[2].encode()
target_pat = re.compile(rb'^(\d+\.\d+\.)\d+([a-z]\d+)$')
m = target_pat.match(target)
if not m:
    print("Bad target version, skipping patch")
    sys.exit(0)
patched = 0
for r, _, fs in os.walk(root):
    for f in fs:
        if not f.endswith('.ab'):
            continue
        path = os.path.join(r, f)
        with open(path, 'rb') as fh:
            head = fh.read(48)
        m2 = re.search(rb'(\d+\.\d+\.\d+[a-z]\d+)', head)
        if not m2:
            continue
        found = m2.group(1)
        if found == target:
            continue
        # Only swap if same byte length (same major.minor patch family)
        if len(found) != len(target):
            continue
        # Be conservative: only swap inside same major.minor
        a = found.decode().split('.')
        b = target.decode().split('.')
        if a[0] != b[0] or a[1] != b[1]:
            continue
        with open(path, 'r+b') as fh:
            fh.seek(m2.start(1))
            fh.write(target)
        patched += 1
print(f"Patched {patched} .ab files to {target.decode()}")
PY
fi

# 5. Build AssetRipper input + run it
echo "[4/5] Preparing AssetRipper input..."
rm -rf "$AR_INPUT"
mkdir -p "$AR_INPUT"
cp -r "$APK_DIR/assets/bin/Data" "$AR_INPUT/Data"
if [[ -d "$OBB_DIR/assets" ]]; then
    cp -r "$OBB_DIR/assets" "$AR_INPUT/Data/StreamingAssets_obb"
fi

echo "[5/5] Running AssetRipperConsole..."
rm -rf "$OUTPUT_DIR/ExportedProject"
"$ASSETRIPPER" -o "$OUTPUT_DIR/ExportedProject_tmp" -q -v "$AR_INPUT" \
    --logFile "$OUTPUT_DIR/AssetRipperConsole.log" || true

# AssetRipper writes to <out>/ExportedProject. Move that up.
if [[ -d "$OUTPUT_DIR/ExportedProject_tmp/ExportedProject" ]]; then
    mv "$OUTPUT_DIR/ExportedProject_tmp/ExportedProject" "$OUTPUT_DIR/ExportedProject"
    rmdir "$OUTPUT_DIR/ExportedProject_tmp" 2>/dev/null || true
fi

# Optional: also produce a parallel ilspycmd dump for reference
if command -v ilspycmd >/dev/null 2>&1; then
    echo "      Producing reference C# via ilspycmd..."
    mkdir -p "$OUTPUT_DIR/ilspy"
    for dll in "$APK_DIR/assets/bin/Data/Managed/Assembly-CSharp.dll" \
               "$APK_DIR/assets/bin/Data/Managed/Assembly-CSharp-firstpass.dll"; do
        [[ -f "$dll" ]] || continue
        name="$(basename "$dll" .dll)"
        ilspycmd -p -o "$OUTPUT_DIR/ilspy/$name" "$dll" >/dev/null 2>&1 || true
    done
fi

echo
echo "Done."
echo "  Unity project: $OUTPUT_DIR/ExportedProject"
echo "  Open with Unity Hub → 'Open project' → pick 'ExportedProject' folder."
echo "  Editor version pinned to: $UNITY_VERSION (install via Unity download archive)."
