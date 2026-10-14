import hashlib
import io
import sys
import tempfile
import unittest
import zipfile
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from scan_original_security_vendor import scan_stream,inspect
class VendorAttribution(unittest.TestCase):
    def test_explicit_brand_marker_is_inventory_only(self):
        count,g=scan_stream(io.BytesIO(b"Appdome ONEShield legacy"))
        self.assertEqual(count["appdome_brand_literal"],1)
        self.assertEqual(count["appdome_oneshield_literal"],1)
    def test_security_threat_phrase_not_brand_evidence(self):
        count,g=scan_stream(io.BytesIO(b"Security Threat Detected by App"))
        self.assertFalse(any(count.values()))
        self.assertEqual(g,1)
    def test_original_sha_check_fails_closed(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk";path.write_bytes(b"not apk")
            with self.assertRaisesRegex(ValueError,"SHA256 mismatch"):
                inspect([path],{"base.apk":"invalid"})
    def test_output_disclaims_vendor_on_known_marker(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            with zipfile.ZipFile(path,"w") as z:
                z.writestr("note.txt","appdome")
            digest=hashlib.sha256(path.read_bytes()).hexdigest()
            r=inspect([path],{"base.apk":digest})
            self.assertEqual(r["classification"],"BRANDING_INDICATORS_FOUND_NOT_VENDOR_PROOF")
            self.assertFalse(r["vendor_identity_confirmed"])
            self.assertFalse(r["specific_security_rule_identified"])
            self.assertFalse(r["original_game_modified"])
if __name__=="__main__":
    unittest.main()
