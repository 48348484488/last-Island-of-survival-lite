"""Resolve source-proven original Unity HUD sprites, not similarly named composites.

Source mapping: assets/hud_original_xapk/manifest_original.json.
Some names are duplicated in multiple AssetBundles and have different pixels;
prefer the Sprite from its owning original gameplay panel over common.ab.
This does not recover the exact original Unity Canvas layout.
"""
import json
from functools import lru_cache
from pathlib import Path

PREFERRED_BUNDLES = (
    "battlepanel.ab", "battlepanel2.ab", "battlecustompanel.ab",
    "common.ab", "tubiao.ab", "tubiao1.ab",
    "battlepanel_kedu_yw.ab", "map_map.ab"
)

@lru_cache(maxsize=2)
def _original_index(root_str):
    root=Path(root_str)
    manifest=root/"hud_original_xapk"/"manifest_original.json"
    if not manifest.exists():
        return {}
    data=json.loads(manifest.read_text(encoding="utf8"))
    grouped={}
    for s in data["sprites"]:
        grouped.setdefault(s["name"],[]).append(s)
    for name in grouped:
        grouped[name].sort(key=lambda r:(PREFERRED_BUNDLES.index(r["bundle"])
              if r["bundle"] in PREFERRED_BUNDLES else 999,
              r["file"]))
    return grouped

def resolve_original_hud_image(asset_dir, filename, *, strict=False):
    """Return a decoded source Unity Sprite when present; legacy fallback otherwise.

    Args:
        asset_dir: game assets directory
        filename: requested sprite filename, e.g. '1BattlePanel_cz_t.png'
        strict: fail rather than select a legacy file when no source Sprite exists
    """
    asset_dir=Path(asset_dir)
    root=asset_dir/"hud_original_xapk"
    name=Path(filename).stem
    for source in _original_index(str(asset_dir)).get(name,[]):
        found=root/source["file"]
        if found.is_file():return str(found)
    if strict:raise FileNotFoundError(f"Original Unity Sprite missing: {name}")
    return str(asset_dir/"ui_hud"/filename)

def source_sprite_evidence(asset_dir, filename):
    """Find original Unity Sprite metadata for reproducible hashing and attribution."""
    asset_dir=Path(asset_dir)
    return tuple(_original_index(str(asset_dir)).get(Path(filename).stem,()))
