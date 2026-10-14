#!/usr/bin/env python3
"""Non-invasive vendor attribution from original, SHA-verified APK members.

Tests a narrow set of public branding strings, not protection logic or
internal detection checks. The toast "Security Threat Detected by App"
also occurs in third-party documentation and is NOT proof of any vendor.
The scanner deliberately does not search for / modify anti-cheat rules.
"""
import argparse
import json
import zipfile
from pathlib import Path
from verify_original_splits import EXPECTED_SHA256, sha256_file

CANDIDATES = {
    "appdome_brand_literal": (b"appdome",),
    "appdome_oneshield_literal": (b"oneshield",),
    "appdome_fusion_set_literal": (b"fusion set", b"fusionset"),
}
NOT_VENDOR_PROOF = (b"security threat detected by app",)
MAX_ENTRY = 1024*1024*1024
READ_BLOCK = 512*1024
OVERLAP = 64

def scan_stream(source):
    counts={key:0 for key in CANDIDATES}
    generic_count=0
    tail=b""
    while block:=source.read(READ_BLOCK):
        merged=(tail+block).lower()
        for key,patterns in CANDIDATES.items():
            for needle in patterns:
                counts[key]+=merged.count(needle)
        generic_count+=sum(merged.count(pattern) for pattern in NOT_VENDOR_PROOF)
        tail=merged[-OVERLAP:]
    return counts,generic_count

def inspect(paths,expected=None):
    expected=expected or EXPECTED_SHA256
    matches=[]
    scanned=0
    skipped=0
    digest_map={}
    total_counts={key:0 for key in CANDIDATES}
    generic=0
    for path in paths:
        if path.name not in expected:
            raise ValueError("Unknown original APK: "+path.name)
        sha=sha256_file(path)
        if sha!=expected[path.name]:
            raise ValueError("SHA256 mismatch: "+path.name)
        digest_map[path.name]=sha
        with zipfile.ZipFile(path) as z:
            for item in z.infolist():
                if item.is_dir():continue
                if item.file_size>MAX_ENTRY:
                    skipped+=1
                    continue
                with z.open(item) as stream:
                    counts,g=scan_stream(stream)
                scanned+=1
                generic+=g
                for k,n in counts.items():
                    total_counts[k]+=n
                    if n:matches.append({"apk":path.name,"member":item.filename,
                                         "indicator":k,"count":n})
    return {
        "classification":("BRANDING_INDICATORS_FOUND_NOT_VENDOR_PROOF" if matches
                          else "NO_EXPLICIT_BRANDING_INDICATORS_FOUND"),
        "original_apks_verified_sha256":digest_map,
        "members_scanned":scanned,
        "oversize_members_not_scanned":skipped,
        "brand_indicator_counts":total_counts,
        "brand_indicator_matches":matches[:75],
        "generic_security_threat_string_count_not_vendor_evidence":generic,
        "specific_security_rule_identified":False,
        "original_game_modified":False,
        "vendor_identity_confirmed":False,
        "notes":[
            "Only positive explicit vendor brand strings are inventoried, not security code or checks.",
            "Toast wording alone cannot establish which vendor, component or rule produced it.",
            "No branding string can be caused by packing, encryption, custom integration or absent metadata.",
            "An explicit brand string could be bundled by another SDK, and is still not proof of active protection.",
            "Exact original APK SHA256 verification is a prerequisite before scanning.",
        ],
    }

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument("apk",nargs="+",type=Path)
    p.add_argument("--out",type=Path,required=True)
    opt=p.parse_args()
    data=inspect(opt.apk)
    opt.out.mkdir(parents=True,exist_ok=True)
    (opt.out/"vendor_footprint.json").write_text(json.dumps(data,indent=2)+"\n")
    lines=["# Original Oxide — public branding footprint audit","",
           "Classification: "+data["classification"],
           "SHA-256 verified APK splits: "+str(len(data["original_apks_verified_sha256"])),
           "APK members scanned: "+str(data["members_scanned"]),
           "Generic warning is NOT vendor evidence.", "",
           "| Indicator | Total occurrences |","|---|---:|"]
    for key,count in data["brand_indicator_counts"].items():
        lines.append("| "+key+" | "+str(count)+" |")
    lines+=["","Branding evidence is insufficient to infer the trigger of the game's protection."]
    (opt.out/"vendor_footprint.md").write_text("\n".join(lines)+"\n")
    print("\n".join(lines))
if __name__=="__main__":
    main()
