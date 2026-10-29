#!/usr/bin/env python3
"""Recover exact ItemCfg.icon Sprite PNGs for source DrawingCfg recipes.

Only uses original LITE 7.3 XAPK and a verified EdgeAI Craft Config artifact.
Identifies icons by complete bundle path + exact Sprite name + source PathID.
Never substitutes lookalikes. Does not alter the original game.
"""
import argparse,collections,gc,hashlib,json,re,shutil,tempfile,zipfile
from pathlib import Path
import UnityPy

def export(parts_dir,source_manifest,destination):
    config=json.loads(source_manifest.read_text(encoding="utf8"))
    if config.get("record_count")!=519 or not config.get("item_headers_verified_only"):
        raise ValueError("Must use source-verified 519-recipe 7.3 catalog")
    wanted=collections.defaultdict(set)
    for r in config["all_records"]:
        for item in [r.get("target")]+r["materials"]:
            if not item: continue
            icon=item.get("icon") or ""
            if not icon: continue
            pair=icon.split("/",1)
            if len(pair)!=2 or not pair[0] or not pair[1]:continue
            wanted[pair[0]].add(pair[1])
    parts=sorted(parts_dir.glob("Last_Island_LITE_7.3.xapk.part*"))
    if len(parts)!=17: raise ValueError("Expected original 17-part LITE source XAPK")
    destination.mkdir(parents=True,exist_ok=True)
    resolved=[]
    missing=[]
    with tempfile.TemporaryDirectory() as folder:
        tmp=Path(folder)
        xapk=tmp/"original.xapk"
        with xapk.open("wb") as out:
            for part in parts:
                with part.open("rb") as src:shutil.copyfileobj(src,out,8*1024*1024)
        with zipfile.ZipFile(xapk) as outer:
            with outer.open("install_time_asset_pack.apk") as src,(tmp/"pack.apk").open("wb") as out:
                shutil.copyfileobj(src,out,8*1024*1024)
        with zipfile.ZipFile(tmp/"pack.apk") as pack:
            names=set(pack.namelist())
            for family,sp_names in sorted(wanted.items()):
                bundle="assets/icon/"+family+".ab"
                if bundle not in names:
                    missing.extend({"icon":family+"/"+n,"reason":"source_bundle_missing"}
                                   for n in sorted(sp_names))
                    print("EDGEAI_CRAFT_SPRITE_MISSING_BUNDLE",bundle,len(sp_names),flush=True)
                    continue
                # Use temporary file rather than keeping all icon bundles in RAM.
                binary=tmp/"icon.ab"
                with pack.open(bundle) as src,binary.open("wb") as out:
                    shutil.copyfileobj(src,out,8*1024*1024)
                try:
                    env=UnityPy.load(str(binary))
                    found=collections.defaultdict(list)
                    for obj in env.objects:
                        if obj.type.name!="Sprite":continue
                        try:
                            data=obj.read()
                            name=str(data.m_Name)
                            if name in sp_names:found[name].append(obj)
                        except Exception:continue
                    for name in sorted(sp_names):
                        matches=found.get(name,[])
                        if len(matches)!=1:
                            missing.append({"icon":family+"/"+name,
                                            "reason":"ambiguous" if matches else "no_sprite",
                                            "candidates":len(matches)})
                            continue
                        obj=matches[0]
                        try:
                            data=obj.read()
                            picture=data.image.convert("RGBA")
                            if min(picture.size)<4 or max(picture.size)>4096:
                                raise ValueError("Implausible source icon dimensions")
                            dst="craft_item_"+family+"_"+str(obj.path_id)+".png"
                            picture.save(destination/dst,optimize=True)
                            resolved.append({
                                "icon":family+"/"+name,
                                "bundle":bundle,
                                "pathid":str(obj.path_id),
                                "file":dst,"size":list(picture.size),
                                "sha256_pixels":hashlib.sha256(picture.tobytes()).hexdigest()})
                        except Exception as exc:
                            missing.append({"icon":family+"/"+name,
                                            "reason":"sprite_read_error",
                                            "detail":str(exc)[:180]})
                    print("EDGEAI_CRAFT_SOURCE_SPRITE_FAMILY",bundle,
                          "requested",len(sp_names),
                          "exported",sum(1 for x in resolved if x["bundle"]==bundle),
                          flush=True)
                finally:
                    del env
                    gc.collect()
                    binary.unlink(missing_ok=True)
    result={"schema":"edgeai.original-craft-sprites.v1",
            "config_source_sha256":config["sha256"],
            "requested":sum(len(x) for x in wanted.values()),
            "recovered":len(resolved),
            "missing_count":len(missing),
            "sprites":resolved,"unresolved":missing,
            "notes":["Each image is an exact original Sprite, with source AssetBundle and PathID.",
                     "An unresolved icon is not replaced by a guessed sprite.",
                     "No claim of 1:1 runtime UI or behavior."]}
    (destination/"craft_icons_manifest.json").write_text(
        json.dumps(result,indent=2,ensure_ascii=False)+"\n",encoding="utf8")
    print("EDGEAI_CRAFT_ICONS_RECOVERED",result["recovered"],
          "/",result["requested"],"missing",len(missing),flush=True)
    if result["recovered"]<30:
        raise ValueError("Source-original item icon recovery is too incomplete")
    return result

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--parts",type=Path,required=True)
    parser.add_argument("--config",type=Path,required=True)
    parser.add_argument("--out",type=Path,required=True)
    args=parser.parse_args()
    export(args.parts,args.config,args.out)
if __name__=="__main__":main()
