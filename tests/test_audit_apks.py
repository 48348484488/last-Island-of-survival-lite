#!/usr/bin/env python3
import json
from pathlib import Path
import sys
import tempfile
import unittest
import zipfile

sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
import audit_apks as a

ANDROID="http://schemas.android.com/apk/res/android"

def apk(path,version="12000",extra=None):
    xml=('<manifest xmlns:android="'+ANDROID+'" package="com.catsbit.oxidesurvivalisland" '
         'android:versionName="1.14.test" android:versionCode="'+version+'"/>')
    with zipfile.ZipFile(path,"w",compression=zipfile.ZIP_STORED) as z:
        z.writestr("AndroidManifest.xml",xml)
        for k,v in (extra or {}).items():z.writestr(k,v)

class TestAudit(unittest.TestCase):
    def setUp(self):
        self.tmp=tempfile.TemporaryDirectory()
        self.d=Path(self.tmp.name)
        for name in a.NAMES:apk(self.d/name)
    def tearDown(self):self.tmp.cleanup()
    def test_manifest(self):
        with zipfile.ZipFile(self.d/"base.apk") as z:
            m=a.manifest(z)
        self.assertEqual(m["package"],"com.catsbit.oxidesurvivalisland")
        self.assertEqual(m["versionCode"],"12000")
    def test_read_only_success(self):
        r=a.audit(self.d,self.d/"out")
        self.assertEqual(r["status"],"PASS")
        self.assertEqual(len(r["files"]),4)
        self.assertTrue((self.d/"out/REPORT.md").is_file())
        self.assertTrue((self.d/"out/audit.json").is_file())
    def test_different_versions_fail(self):
        apk(self.d/"split_config.xxhdpi.apk","11000")
        r=a.audit(self.d,self.d/"out")
        self.assertEqual(r["status"],"FAIL")
        self.assertTrue(any("versionCode mismatch" in e for e in r["errors"]))
    def test_missing_part_fail(self):
        (self.d/"split_config.arm64_v8a.apk").unlink()
        r=a.audit(self.d,self.d/"out")
        self.assertEqual(r["status"],"FAIL")
        self.assertIn("missing split_config.arm64_v8a.apk",r["errors"])
    def test_hash_pin(self):
        r=a.audit(self.d,self.d/"out",expected_sha="0"*64)
        self.assertEqual(r["status"],"FAIL")
        self.assertTrue(any("SHA-256 mismatch" in e for e in r["errors"]))
    def test_binary_axml_manifest(self):
        import struct
        strs=["manifest","package","com.catsbit.oxidesurvivalisland","versionCode","versionName","1.14.9"]
        chunks=[]; offsets=[]; idx=0
        for x in strs:
            raw=x.encode("utf-8")
            offsets.append(idx)
            encoded=bytes([len(x),len(raw)])+raw+b"\x00"
            chunks.append(encoded);idx+=len(encoded)
        string_bytes=b"".join(chunks)
        pool_size=28+4*len(strs)+len(string_bytes)
        pool=(struct.pack("<HHI",1,28,pool_size)+
              struct.pack("<IIIII",len(strs),0,0x100,28+4*len(strs),0)+
              struct.pack("<"+"I"*len(strs),*offsets)+string_bytes)
        def attr(k,raw,kind,v):
            return struct.pack("<IIIHBBI",0xffffffff,k,raw,8,0,kind,v)
        attrs=(attr(1,2,3,2)+attr(3,0xffffffff,0x10,12345)+attr(4,5,3,5))
        ext=struct.pack("<IIHHHHHH",0xffffffff,0,20,20,3,0,0,0)
        tag=(struct.pack("<HHI",0x102,16,16+len(ext)+len(attrs))+
             struct.pack("<II",1,0xffffffff)+ext+attrs)
        manifest=struct.pack("<HHI",3,8,8+len(pool)+len(tag))+pool+tag
        with zipfile.ZipFile(self.d/"base.apk","w") as z:
            z.writestr("AndroidManifest.xml",manifest)
        with zipfile.ZipFile(self.d/"base.apk") as z:
            got=a.manifest(z)
        self.assertEqual(got["package"],"com.catsbit.oxidesurvivalisland")
        self.assertEqual(got["versionCode"],12345)
        self.assertEqual(got["versionName"],"1.14.9")
    def test_bad_manifest_fails_audit(self):
        with zipfile.ZipFile(self.d/"base.apk","w") as z:
            z.writestr("AndroidManifest.xml",b"\x03\x00\x08\x00\x08\x00\x00\x00")
        r=a.audit(self.d,self.d/"out")
        self.assertEqual(r["status"],"FAIL")
        self.assertTrue(any("AXML parse" in x for x in r["errors"]))
    def test_native_metadata(self):
        elf=bytearray(32);elf[:4]=b"\x7fELF";elf[4]=2;elf[18:20]=(183).to_bytes(2,"little")
        apk(self.d/"split_config.arm64_v8a.apk",extra={
            "lib/arm64-v8a/libil2cpp.so":bytes(elf)+b"Oxide.PlayerManager",
            "assets/bin/Data/Managed/Metadata/global-metadata.dat":bytes.fromhex("af1bb1fa")+int(270).to_bytes(4,"little")
        })
        item=a.inspect(self.d/"split_config.arm64_v8a.apk")
        lib=next(x for x in item["interesting"] if x["path"].endswith("libil2cpp.so"))
        self.assertEqual(lib["elf_machine"],183)
        self.assertEqual(lib["string_markers"]["Oxide.PlayerManager"],1)
        meta=next(x for x in item["interesting"] if x["path"].endswith("global-metadata.dat"))
        self.assertEqual(meta["version_field"],270)
    def test_corrupt_zip(self):
        (self.d/"base.apk").write_bytes(b"not a zip")
        r=a.audit(self.d,self.d/"out")
        self.assertEqual(r["status"],"FAIL")

if __name__=="__main__":unittest.main()
