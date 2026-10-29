#!/usr/bin/env python3
"""Build native craft icon atlas and source recipe data from verified EdgeAI output.

No guessed icons, item IDs, recipes or costs. Runtime locality, player
unlocks, 3D/placement support and Unity fonts remain a separate porting task.
"""
import argparse,json,math
from pathlib import Path
from PIL import Image

TILE = 128
COLUMNS = 12
ROWS = 12
PER_ATLAS = COLUMNS*ROWS

def cpp_text(value):
    return json.dumps(str(value or ""),ensure_ascii=False)

def generate(source,icon_index,icon_dir,out):
    info=json.loads(source.read_text(encoding="utf8"))
    atlas_info=json.loads(icon_index.read_text(encoding="utf8"))
    if info["record_count"]!=519:
        raise ValueError("Require verified 519 original drawing records")
    sprites={}
    for asset in atlas_info["sprites"]:
        icon=asset["icon"]
        if icon in sprites:raise ValueError("Ambiguous source icon "+icon)
        sprites[icon]=asset
    icon_names=sorted(sprites)
    icon_slots={}
    out.mkdir(parents=True,exist_ok=True)
    atlas_count=math.ceil(len(icon_names)/PER_ATLAS)
    for a in range(atlas_count):
        sheet=Image.new("RGBA",(TILE*COLUMNS,TILE*ROWS),(0,0,0,0))
        for index,icon in enumerate(icon_names[a*PER_ATLAS:(a+1)*PER_ATLAS]):
            art=sprites[icon]
            path=icon_dir/art["file"]
            if not path.is_file():raise FileNotFoundError(path)
            with Image.open(path) as original:
                img=original.convert("RGBA")
                max_side=TILE-6
                if img.width>max_side or img.height>max_side:
                    img.thumbnail((max_side,max_side),Image.Resampling.LANCZOS)
                x=index%COLUMNS*TILE+(TILE-img.width)//2
                y=index//COLUMNS*TILE+(TILE-img.height)//2
                sheet.alpha_composite(img,(x,y))
            icon_slots[icon]=[a,index]
        sheet.save(out/("craft_source_atlas_"+str(a)+".png"),optimize=True)
    recipes=sorted(enumerate(info["all_records"]),key=lambda item:(
        item[1]["order"],item[0]))
    materials=[]
    rows=[]
    for original_order,row in recipes:
        item=row.get("target") or {}
        icon=item.get("icon") or ""
        atlas,slot=icon_slots.get(icon,(-1,-1))
        start=len(materials)
        for m in row["materials"]:
            mat_icon=m.get("icon") or ""
            mat_atlas,mat_slot=icon_slots.get(mat_icon,(-1,-1))
            materials.append((int(m["itemId"]),int(m["num"]),
                              mat_atlas,mat_slot,m.get("name") or ""))
        rows.append((
            int(row["id"]),int(row["bigType"]),int(row["smallType"]),
            int(row["order"]),int(row["levelLimit"]),int(row["needTime"]),
            int(row["targetItemId"]),int(row["onceMaxCreate"]),
            int(row["productNum"]),bool(row["isNormal"]),start,len(row["materials"]),
            atlas,slot,row["name"],item.get("name") or "",icon
        ))
    lines=[
        "#pragma once",
        "// GENERATED: source LITE 7.3 cfg.DrawingCfg.oc and verified ItemCfg headers.",
        "// Unknown Unity runtime player state remains unimplemented.",
        "namespace LIOS_ORIGINAL_RECIPES {",
        "struct Ingredient {int itemId,amount,atlas,slot;const char* name;};",
        "struct Recipe {int id,bigType,smallType,order,level,seconds,targetId,onceMax,productNum;",
        " bool isNormal;int firstIngredient,ingredientCount,atlas,slot;",
        " const char* name;const char* itemName;const char* sourceIcon;};",
        "static constexpr int AtlasTile = 128;",
        "static constexpr int AtlasCols = 12;",
        "static constexpr int AtlasRows = 12;",
        "static constexpr int AtlasCount = "+str(atlas_count)+";",
        "static const Ingredient Ingredients[] = {"]
    for item in materials:
        lines.append(" {%d,%d,%d,%d,%s},"%(
            item[0],item[1],item[2],item[3],cpp_text(item[4])))
    lines.extend(["};","static const Recipe Recipes[] = {"])
    for r in rows:
        lines.append(" {%d,%d,%d,%d,%d,%d,%d,%d,%d,%s,%d,%d,%d,%d,%s,%s,%s},"%(
            *r[:9],"true" if r[9] else "false",*r[10:14],
            cpp_text(r[14]),cpp_text(r[15]),cpp_text(r[16])))
    lines.extend([
        "};",
        "static constexpr int RecipeCount = "+str(len(rows))+";",
        "static constexpr int IngredientCount = "+str(len(materials))+";",
        "inline const Recipe* find(int id) {",
        " for(int i=0;i<RecipeCount;i++)if(Recipes[i].id==id)return &Recipes[i];",
        " return nullptr;",
        "}",
        "inline bool inTab(const Recipe& r,int big,int small){",
        " if(small==1)return r.isNormal;",
        " if(small==4)return r.bigType==4 || r.smallType==4;",
        " return r.smallType==small;",
        "}",
        "} // namespace LIOS_ORIGINAL_RECIPES",
    ])
    (out/"original_craft_catalog_v73.hpp").write_text(
        "\n".join(lines)+"\n",encoding="utf8")
    proof={
        "schema":"edgeai.native-craft-atlas.v1",
        "source_config_sha256":info["sha256"],
        "original_records":len(rows),
        "original_material_requirements":len(materials),
        "matched_unique_source_sprites":len(icon_slots),
        "missing_target_icons":sum(1 for x in rows if x[12]<0),
        "atlas_count":atlas_count,
        "atlas_images":["craft_source_atlas_"+str(i)+".png" for i in range(atlas_count)],
        "not_ported":["original Unity drawing unlocks and workbench gates",
                      "ItemCfg full suffix and localized fonts",
                      "craft result inventory and server transport"]
    }
    (out/"native_craft_source_manifest.json").write_text(
        json.dumps(proof,indent=2,ensure_ascii=False)+"\n",encoding="utf8")
    print("EDGEAI_SOURCE_NATIVE_CRAFT_ATLAS_PASS",proof,flush=True)
    return proof

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("--config",type=Path,required=True)
    p.add_argument("--icons",type=Path,required=True)
    p.add_argument("--sprite-dir",type=Path,required=True)
    p.add_argument("--out",type=Path,required=True)
    a=p.parse_args()
    generate(a.config,a.icons,a.sprite_dir,a.out)
if __name__=="__main__":main()
