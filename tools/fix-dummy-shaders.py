#!/usr/bin/env python3
"""
fix-dummy-shaders.py

AssetRipper exports shader source files as "DummyShaderTextExporter" stubs
because the original GLSL/HLSL is no longer in the build (only compiled GPU
bytecode is). Those stubs frequently crash the Unity Editor on Linux during
shader compilation.

This script walks the project, finds every dummy shader, and rewrites it as
a minimal valid Unity Shader that *only* declares the original Properties
and falls back to a Unity built-in shader. Unity's renderer follows the
Fallback chain when a shader has no compilable SubShader, which is exactly
what we want here.

Usage:
    python3 fix-dummy-shaders.py <ExportedProject_path>

Idempotent. Run multiple times safely.
"""
import os
import re
import sys
import argparse


# Heuristic: shader name → Unity built-in fallback that's most visually similar.
# Order matters; first matching pattern wins.
FALLBACK_RULES = [
    # Particles
    (r"particles?/?\s*additive",                 "Particles/Additive"),
    (r"particles?/?\s*alpha\s*blended",          "Particles/Alpha Blended"),
    (r"particles?/?\s*multiply",                 "Particles/Multiply"),
    (r"particles?",                              "Particles/Alpha Blended"),

    # UI
    (r"^ui[/_]|sprites?/?\s*default|/sprite",    "UI/Default"),
    (r"^tmp[_/]|textmesh\s*pro",                 "TextMeshPro/Distance Field"),

    # Skybox
    (r"skybox",                                  "Skybox/Procedural"),

    # Water (no built-in water; closest is Standard with transparency, but
    # Standard isn't a great fallback for water — use Mobile/Diffuse so it at
    # least renders solid instead of crashing).
    (r"water|reflective",                        "Mobile/Diffuse"),

    # Glow / bloom / post-process — these are Hidden/* in built-ins; no clean
    # fallback. Use Hidden/InternalErrorShader so they fail gracefully without
    # crashing the editor.
    (r"^hidden[/_]|post[\s_-]*process|bloom|blur|flare|glow|halo|distort", "Hidden/InternalErrorShader"),
    (r"outline",                                 "Hidden/InternalErrorShader"),

    # Vegetation / grass / leaves
    (r"grass|leaf|leaves|tree",                  "Mobile/Diffuse"),

    # 3D text
    (r"3d\s*text|text\s*shader",                 "GUI/Text Shader"),

    # Shader Forge custom — no good fallback; use Standard
    (r"shader\s*forge",                          "Standard"),

    # Toon / cel
    (r"toon|cel",                                "Mobile/Diffuse"),

    # Bumped / normal mapped
    (r"bump|normal\s*map",                       "Mobile/Bumped Diffuse"),

    # Diffuse (catch-all)
    (r"diffuse|albedo",                          "Mobile/Diffuse"),

    # Specular
    (r"specular|gloss|metallic",                 "Standard"),
]
DEFAULT_FALLBACK = "Mobile/Diffuse"


SHADER_RE = re.compile(r'^\s*Shader\s+"([^"]+)"\s*\{', re.M)
DUMMY_RE = re.compile(r'//\s*DummyShaderTextExporter')

# Properties block — keep it intact so material .mat files continue to find
# their texture / color / float references.
PROPERTIES_RE = re.compile(r'(Properties\s*\{[\s\S]*?\n\s*\})', re.M)


def pick_fallback(shader_name: str) -> str:
    name = shader_name.lower()
    for pat, fb in FALLBACK_RULES:
        if re.search(pat, name):
            return fb
    return DEFAULT_FALLBACK


def rewrite_shader(text: str) -> tuple[str | None, str, str]:
    m = SHADER_RE.search(text)
    if not m:
        return None, "", ""
    shader_name = m.group(1)

    props_m = PROPERTIES_RE.search(text)
    properties_block = props_m.group(1) if props_m else "Properties {\n\t}"

    fb = pick_fallback(shader_name)

    new_text = (
        f'Shader "{shader_name}" {{\n'
        f'\t{properties_block}\n'
        f'\tFallback "{fb}"\n'
        f'}}\n'
    )
    return new_text, shader_name, fb


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("project", help="Path to ExportedProject root")
    ap.add_argument("--dry-run", action="store_true", help="Don't write, just print what would change")
    args = ap.parse_args()

    assets_dir = os.path.join(args.project, "Assets")
    if not os.path.isdir(assets_dir):
        print(f"ERROR: {assets_dir} not found", file=sys.stderr)
        sys.exit(2)

    rewritten = 0
    skipped_real = 0
    failed = 0
    bytes_saved = 0

    for root, _, files in os.walk(assets_dir):
        for fname in files:
            if not fname.endswith(".shader"):
                continue
            path = os.path.join(root, fname)
            try:
                with open(path, "r", encoding="utf-8") as fh:
                    text = fh.read()
            except Exception as e:
                print(f"  {path}: read failed ({e})")
                failed += 1
                continue

            if not DUMMY_RE.search(text):
                # Already real / not a dummy — leave alone.
                skipped_real += 1
                continue

            new_text, shader_name, fb = rewrite_shader(text)
            if new_text is None:
                print(f"  {path}: no Shader \"...\" header, skipping")
                failed += 1
                continue

            old_size = len(text.encode("utf-8"))
            new_size = len(new_text.encode("utf-8"))
            bytes_saved += old_size - new_size

            rel = os.path.relpath(path, args.project)
            print(f"  [{shader_name}] -> Fallback \"{fb}\"  ({rel})")

            if not args.dry_run:
                with open(path, "w", encoding="utf-8") as fh:
                    fh.write(new_text)
            rewritten += 1

    print()
    print(f"Rewrote {rewritten} dummy shaders.")
    print(f"Skipped {skipped_real} non-dummy shaders.")
    if failed:
        print(f"Failed   {failed} files.")
    print(f"Bytes saved: {bytes_saved}")


if __name__ == "__main__":
    main()
