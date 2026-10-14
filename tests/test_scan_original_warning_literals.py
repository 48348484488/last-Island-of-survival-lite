import io
import tempfile
import unittest
import zipfile
import hashlib
from pathlib import Path
import sys
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from scan_original_warning_literals import scan_entry,inspect

class SecurityWarningOrigin(unittest.TestCase):
    def test_across_chunk_boundary_ascii(self):
        txt=b"aaaaaSecurity Threat Detected by Appzzz"
        items=scan_entry(io.BytesIO(txt),len(txt),chunk_size=9)
        found=[x for x in items if x["term"]=="Security Threat Detected by App" and x["encoding"]=="utf-8"]
        self.assertEqual(found[0]["offsets"],[5])
    def test_utf16le_detection(self):
        txt=b"\x00"*10+"Security Threat".encode("utf-16le")
        items=scan_entry(io.BytesIO(txt),len(txt),chunk_size=7)
        found=[x for x in items if x["term"]=="Security Threat" and x["encoding"]=="utf-16le"]
        self.assertEqual(found[0]["offsets"],[10])
    def test_utf16be_across_chunk_boundary(self):
        txt=b"abc"+"Security Threat Detected by App".encode("utf-16be")+b"tail"
        items=scan_entry(io.BytesIO(txt),len(txt),chunk_size=11)
        matches=[m for m in items if m["term"]=="Security Threat Detected by App"
                 and m["encoding"]=="utf-16be"]
        self.assertEqual(matches[0]["offsets"],[3])
    def test_all_four_coverage_requires_all_four_files(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            with zipfile.ZipFile(path,"w") as z:z.writestr("asset",b"nothing")
            digest=hashlib.sha256(path.read_bytes()).hexdigest()
            r=inspect([path],{"base.apk":digest})
            self.assertFalse(r["scanned_all_four_original_splits"])
            self.assertFalse(r["full_plaintext_member_coverage"])
            self.assertEqual(r["uncompressed_bytes_not_scanned"],0)
    def test_hash_gate_and_zip_member(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            with zipfile.ZipFile(path,"w") as z:
                z.writestr("lib/arm64-v8a/example.so",b"REF: 5600:AA4C detected")
            digest=hashlib.sha256(path.read_bytes()).hexdigest()
            r=inspect([path],{"base.apk":digest})
            self.assertEqual(r["status"],"EXACT_SECURITY_WARNING_PATTERN_FOUND")
            self.assertEqual(r["matched_entries"][0]["entry"],"lib/arm64-v8a/example.so")
            self.assertFalse(r["specific_security_trigger_identified"])
    def test_mismatched_original_archive_fails_closed(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            path.write_bytes(b"invalid")
            with self.assertRaisesRegex(ValueError,"checksum mismatch"):
                inspect([path],{"base.apk":"00"*32})
    def test_no_match_not_proof_no_security(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            with zipfile.ZipFile(path,"w") as z:z.writestr("x",b"hello")
            digest=hashlib.sha256(path.read_bytes()).hexdigest()
            r=inspect([path],{"base.apk":digest})
            self.assertEqual(r["status"],"NO_PLAINTEXT_MATCHES_IN_SCANNED_APKS")
            self.assertFalse(r["specific_security_trigger_identified"])
if __name__=="__main__":
    unittest.main()
