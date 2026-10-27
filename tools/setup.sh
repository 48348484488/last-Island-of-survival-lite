#!/usr/bin/env bash
# Set up the toolchain. One-time. Idempotent.
set -euo pipefail

# Resolve script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TOOLS_DIR="$SCRIPT_DIR/tools"
mkdir -p "$TOOLS_DIR"

ASSETRIPPER_VERSION="0.2.1.1"
ILSPYCMD_VERSION="8.2.0.7535"
DOTNET_INSTALL_DIR="$HOME/.dotnet"

# OS / arch detection
OS=$(uname -s | tr '[:upper:]' '[:lower:]')
ARCH=$(uname -m)
case "$ARCH" in
    x86_64) AR_ARCH="x64" ;;
    aarch64|arm64) AR_ARCH="arm64" ;;
    *) echo "Unsupported arch: $ARCH" >&2; exit 1 ;;
esac
case "$OS" in
    linux) AR_OS="linux" ;;
    darwin) AR_OS="mac" ;;
    *) echo "Unsupported OS: $OS (this script targets Linux/macOS)" >&2; exit 1 ;;
esac

# 1. .NET 6 runtime + 8 SDK (sdk lets us install global tools)
if [[ ! -x "$DOTNET_INSTALL_DIR/dotnet" ]]; then
    echo "[setup] Installing .NET..."
    curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    /tmp/dotnet-install.sh --channel 8.0 --install-dir "$DOTNET_INSTALL_DIR" >/dev/null
    /tmp/dotnet-install.sh --channel 6.0 --runtime dotnet --install-dir "$DOTNET_INSTALL_DIR" >/dev/null
fi
export PATH="$DOTNET_INSTALL_DIR:$DOTNET_INSTALL_DIR/tools:$PATH"
export DOTNET_ROOT="$DOTNET_INSTALL_DIR"

# 2. ilspycmd at a version that ships a usable tool package
if ! command -v ilspycmd >/dev/null 2>&1; then
    echo "[setup] Installing ilspycmd $ILSPYCMD_VERSION..."
    dotnet tool install -g ilspycmd --version "$ILSPYCMD_VERSION" >/dev/null
fi

# 3. AssetRipperConsole (the last CLI build of AssetRipper)
if [[ ! -x "$TOOLS_DIR/AssetRipperConsole/AssetRipperConsole" ]]; then
    echo "[setup] Installing AssetRipperConsole $ASSETRIPPER_VERSION..."
    AR_URL="https://github.com/AssetRipper/AssetRipper/releases/download/$ASSETRIPPER_VERSION/AssetRipperConsole_${AR_OS}_${AR_ARCH}.zip"
    curl -sSL "$AR_URL" -o "$TOOLS_DIR/AssetRipperConsole.zip"
    rm -rf "$TOOLS_DIR/AssetRipperConsole"
    unzip -q "$TOOLS_DIR/AssetRipperConsole.zip" -d "$TOOLS_DIR/AssetRipperConsole"
    chmod +x "$TOOLS_DIR/AssetRipperConsole/AssetRipperConsole"
    rm -f "$TOOLS_DIR/AssetRipperConsole.zip"
fi

# 4. Sanity check
if ! command -v unzip >/dev/null 2>&1; then
    echo "[setup] WARN: 'unzip' not in PATH. Install it via your package manager." >&2
fi
if ! command -v python3 >/dev/null 2>&1; then
    echo "[setup] WARN: 'python3' not in PATH. The version-patching step needs it." >&2
fi

echo
echo "[setup] Done. Next:"
echo "  ./restore-unity-mono.sh path/to/game.xapk ./out"
