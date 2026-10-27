# restore-unity-mono

Reproducible toolchain that takes a Unity 2018-era **Mono-backend** Android game (`.apk` or `.xapk`) and reconstructs an openable Unity project from it — scenes, prefabs, materials, meshes, textures, animations, AnimatorControllers, full original C# source (with intact namespaces, class names, member names) and project settings.

This is a reverse-engineering / preservation tool. Use it on builds you legally own a copy of, for personal study, archival, or single-player modding. It does not strip DRM and does not let you bypass live-service backends.

## What it works on

- Unity Mono backend builds (look for `assets/bin/Data/Managed/Assembly-CSharp.dll` inside the apk)
- Unity 5.x – 2019.x roughly. Tested on **2018.3.8f1**.
- Both pure `.apk` and Google Play `.xapk` (apk + obb) are supported.

## What it does NOT work on

- **IL2CPP** builds (`libil2cpp.so` instead of Mono dlls). Those need a different pipeline (`Il2CppDumper` + reverse the AOT machine code), which this tool does not do.
- Encrypted / packed AssetBundles (some publishers wrap `.ab` in custom AES). Run `head -c 8 some.ab` — it must start with `UnityFS` for AssetRipper to read it.
- Online-only games — you get back the **client**, not a server. Multiplayer / live ops won't function without the original backend.

## What you get out

A real Unity 2018.3.8f1 project (`out/ExportedProject/`) with:

- `Assets/Scene/*.unity` — scenes
- `Assets/MonoScript/Assembly-CSharp/*.cs` — decompiled C# (uses ICSharpCode.Decompiler internally; namespaces, types, fields, methods preserved when not obfuscated)
- `Assets/Material/`, `Assets/Mesh/`, `Assets/Texture2D/`, `Assets/Sprite/`, `Assets/AnimationClip/`, `Assets/AnimatorController/`, `Assets/Avatar/`, `Assets/Shader/`, `Assets/ComputeShader/`
- `Assets/Asset_Bundles/<bundle>.ab/...` — every loose AssetBundle from the obb expanded into a folder mirroring its internal asset paths
- `ProjectSettings/` — full project settings, including the correct Unity version pin

## Limitations of the result

The output is editable, not "press Play and it runs". Expect:
- Pink materials (built-in / mobile shaders need re-binding)
- `DllNotFoundException` for native plugins that aren't shipped (Firebase, Google Billing, custom native net code)
- Some decompiled scripts won't compile straight away (compiler-generated names, anonymous-type quirks, unsafe blocks)
- No live-service backend → online gameplay isn't restorable

You're getting a research-grade snapshot of the build, not a magic re-buildable retail copy.

## Usage

```bash
# 1. Set up dependencies (one-time)
./setup.sh

# 2. Run on an apk or xapk
./restore-unity-mono.sh /path/to/game.xapk ./out

# Output:
#   ./out/work/apk_extracted/   - apk contents
#   ./out/work/obb_extracted/   - obb contents (only for xapk)
#   ./out/ExportedProject/      - the Unity project to open in Unity Hub
```

## How it works

1. **Unzip** xapk → apk (+ obb if present)
2. **Unzip** apk → `assets/bin/Data/` (Mono managed DLLs + Unity asset files)
3. **Unzip** obb → `assets/<category>/*.ab`
4. **Detect Unity version** from `globalgamemanagers` (first ASCII hit after the header)
5. **Patch version drift** — older `.ab` files sometimes report a slightly older patch version (e.g. 2018.3.7f1 vs 2018.3.8f1). Same-length ASCII swap so AssetRipper treats them as one collection
6. **AssetRipperConsole** reconstructs the Unity project, decompiling Mono assemblies inline
7. (Optional) Re-decompile via `ilspycmd` for higher-fidelity reference C# in `out/ilspy/`

## Why these specific tools

- **AssetRipper 0.2.1.1** — last release with a real CLI build (`AssetRipperConsole_linux_x64`). Newer 1.x is GUI / web-server only.
- **ilspycmd 8.2.0.7535** — last version with a working `DotnetToolSettings.xml` in its NuGet package. Newer ilspycmd packages on NuGet are broken (`Tool 'ilspycmd' failed to install`).
- **.NET 6 runtime** — required by ilspycmd 8.2.0.

## License

MIT for this toolchain. The output files are derived from the input game and remain copyright of their original authors. Don't redistribute reconstructed projects of games you don't own.
