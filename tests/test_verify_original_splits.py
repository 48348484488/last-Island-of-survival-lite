import hashlib
import importlib.util
import tempfile
import unittest
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    "original_hashes",
    Path(__file__).resolve().parents[1] / "tools" / "verify_original_splits.py"
)
m = importlib.util.module_from_spec(spec)
spec.loader.exec_module(m)


class OriginalSplitIntegrityTests(unittest.TestCase):
    def test_complete_fixture_exactly_matches(self):
        with tempfile.TemporaryDirectory() as tmp:
            folder = Path(tmp)
            expected = {}
            for name, data in {"base.apk": b"alpha", "split.apk": b"beta"}.items():
                (folder / name).write_bytes(data)
                expected[name] = hashlib.sha256(data).hexdigest()
            r = m.verify(folder, expected)
            self.assertEqual(r["status"], "PASS")
            self.assertTrue(all(x["status"] == "PASS" for x in r["entries"]))

    def test_changed_bytes_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            folder = Path(tmp)
            (folder / "base.apk").write_bytes(b"tampered")
            expected = {"base.apk": hashlib.sha256(b"original").hexdigest()}
            r = m.verify(folder, expected)
            self.assertEqual(r["status"], "FAIL")
            self.assertEqual(r["entries"][0]["status"], "MISMATCH")

    def test_missing_split_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            r = m.verify(tmp, {"base.apk": hashlib.sha256(b"x").hexdigest()})
            self.assertEqual(r["status"], "FAIL")
            self.assertEqual(r["entries"][0]["status"], "MISSING")

    def test_reference_digests_cover_only_four_original_names(self):
        self.assertEqual(set(m.EXPECTED_SHA256), {
            "base.apk", "split_config.arm64_v8a.apk", "split_config.xxhdpi.apk",
            "split_UnityDataAssetPack.apk",
        })
        self.assertTrue(all(len(v) == 64 for v in m.EXPECTED_SHA256.values()))


if __name__ == "__main__":
    unittest.main()
