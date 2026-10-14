import hashlib
import sys
import tempfile
import unittest
import zipfile
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from inventory_original_asset_containers import identify,inventory

class AssetContainerMetadata(unittest.TestCase):
    def test_unityfs_magic_only_a_format_hint(self):
        self.assertEqual(identify(b"UnityFS\x000\x00\x00\x00"),"UNITY_FS_ASSET_BUNDLE")
        self.assertEqual(identify(b"UnityWeb\x00"),"UNITY_WEB_ASSET_BUNDLE")
        self.assertEqual(identify(b"not unity fs"),"NOT_IDENTIFIED_BY_SIGNATURE")
    def test_sha256_gated_metadata_with_nested_sample(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            with zipfile.ZipFile(path,"w") as z:
                z.writestr("assets/one",b"UnityFS\x00"+b"test"*20)
                z.writestr("assets/two",b"random bytes")
            digest=hashlib.sha256(path.read_bytes()).hexdigest()
            result=inventory([path],{"base.apk":digest})
            self.assertEqual(result["members_total"],2)
            self.assertEqual(result["identified_header_kinds"]["UNITY_FS_ASSET_BUNDLE"],1)
            self.assertFalse(result["app_security_root_cause_proven"])
            self.assertFalse(result["all_original_splits_included"])
            self.assertFalse(result["original_game_modified"])
    def test_wrong_digest_rejected(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            path.write_bytes(b"fake")
            with self.assertRaisesRegex(ValueError,"checksum mismatch"):
                inventory([path],{"base.apk":"0"*64})
    def test_unknown_split_rejected(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"unknown.apk";path.write_bytes(b"fake")
            with self.assertRaisesRegex(ValueError,"Not an expected"):
                inventory([path],{"base.apk":"0"*64})
if __name__=="__main__":
    unittest.main()
