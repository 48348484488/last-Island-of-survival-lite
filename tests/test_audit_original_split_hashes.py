import importlib.util
import tempfile
import unittest
from pathlib import Path

p = Path(__file__).resolve().parents[1] / "tools/audit_original_split_hashes.py"
spec = importlib.util.spec_from_file_location("oxide_split_audit", p)
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

class AuditTests(unittest.TestCase):
    def test_missing_files(self):
        with tempfile.TemporaryDirectory() as d:
            result = mod.audit(Path(d))
            self.assertFalse(result["complete"])
            self.assertEqual(len(result["files"]), 4)
    def test_all_files_hashed(self):
        with tempfile.TemporaryDirectory() as d:
            root = Path(d)
            for name in mod.NAMES:
                (root / name).write_bytes(b"fixture")
            result = mod.audit(root)
            self.assertTrue(result["complete"])
            self.assertEqual(len({v["sha256"] for v in result["files"].values()}), 1)
    def test_no_mutation(self):
        with tempfile.TemporaryDirectory() as d:
            root = Path(d)
            (root / "base.apk").write_bytes(b"original")
            before = (root / "base.apk").read_bytes()
            mod.audit(root)
            self.assertEqual((root / "base.apk").read_bytes(), before)
    def test_no_runtime_claim(self):
        with tempfile.TemporaryDirectory() as d:
            self.assertFalse(mod.audit(Path(d))["runtime_tested"])

if __name__ == "__main__":
    unittest.main()
