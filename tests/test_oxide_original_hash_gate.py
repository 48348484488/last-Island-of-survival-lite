import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from tools.oxide_original_hash_gate import EXPECTED, verify_inventory


def original_inventory():
    return {"complete": True, "files": {
        name: {"present": True, "size_bytes": 1234, "sha256": digest}
        for name, digest in EXPECTED.items()
    }}


class OriginalHashGateTests(unittest.TestCase):
    def test_exact_four_pinned_hashes(self):
        out = verify_inventory(original_inventory())
        self.assertTrue(out["all_hashes_match"])
        self.assertFalse(out["gameplay_verified"])
        self.assertFalse(out["provenance_certified"])

    def test_mutated_split_must_be_rejected(self):
        inventory = original_inventory()
        inventory["files"]["split_UnityDataAssetPack.apk"]["sha256"] = "0" * 64
        self.assertEqual(verify_inventory(inventory)["status"], "BLOCKED_UNVERIFIED_APK_SPLITS")

    def test_missing_split_rejected(self):
        inventory = original_inventory()
        del inventory["files"]["split_config.arm64_v8a.apk"]
        self.assertFalse(verify_inventory(inventory)["all_hashes_match"])

    def test_incomplete_inventory_rejected(self):
        inventory = original_inventory()
        inventory["complete"] = False
        self.assertFalse(verify_inventory(inventory)["all_hashes_match"])

    def test_unexpected_duplicate_entry_rejected(self):
        inventory = original_inventory()
        inventory["files"]["wrong.apk"] = inventory["files"]["base.apk"]
        self.assertFalse(verify_inventory(inventory)["all_hashes_match"])

    def test_blank_digest_rejected(self):
        inventory = original_inventory()
        inventory["files"]["base.apk"]["sha256"] = ""
        self.assertFalse(verify_inventory(inventory)["all_hashes_match"])

    def test_fake_boolean_size_rejected(self):
        inventory = original_inventory()
        inventory["files"]["base.apk"]["size_bytes"] = True
        self.assertFalse(verify_inventory(inventory)["all_hashes_match"])

    def test_cli_fails_closed_on_invalid_file(self):
        script = Path(__file__).resolve().parents[1] / "tools" / "oxide_original_hash_gate.py"
        with tempfile.TemporaryDirectory() as d:
            output = Path(d) / "report.json"
            p = subprocess.run([sys.executable, str(script), "--inventory", str(Path(d) / "missing.json"),
                                "--output", str(output)], capture_output=True, text=True, cwd=d, timeout=10)
            self.assertEqual(p.returncode, 2, p.stderr)
            self.assertFalse(json.loads(output.read_text())["gameplay_verified"])

    def test_cli_uses_observed_inventory(self):
        script = Path(__file__).resolve().parents[1] / "tools" / "oxide_original_hash_gate.py"
        with tempfile.TemporaryDirectory() as d:
            src = Path(d) / "inventory.json"
            src.write_text(json.dumps(original_inventory()))
            p = subprocess.run([sys.executable, str(script), "--inventory", str(src),
                                "--output", str(Path(d) / "verified.json")],
                               capture_output=True, text=True, cwd=d, timeout=10)
            self.assertEqual(p.returncode, 0, p.stderr)
            self.assertTrue(json.loads(p.stdout)["all_hashes_match"])


if __name__ == "__main__":
    unittest.main()
