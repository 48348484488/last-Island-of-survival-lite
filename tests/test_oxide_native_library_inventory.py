"""Synthetic ZIP archives test fail-closed original APK filename metadata checks."""
import hashlib
import tempfile
import unittest
from pathlib import Path
from zipfile import ZipFile

from tools.oxide_native_library_inventory import inspect_apk_index


class NativeOriginalSplitInventoryTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def archives(self, archive_entries):
        expected = {}
        for apk, paths in archive_entries.items():
            with ZipFile(self.root / apk, "w") as archive:
                for path in paths:
                    archive.writestr(path, b"synthetic test only")
            expected[apk] = hashlib.sha256((self.root / apk).read_bytes()).hexdigest()
        return expected

    def test_generated_present_but_lookup_names_absent(self):
        pinned = self.archives({
            "base.apk": ["assets/test"],
            "arm64.apk": ["lib/arm64-v8a/lib_burst_generated.so",
                          "lib/arm64-v8a/libloader.so"],
            "density.apk": ["res/drawable/x.png"],
        })
        result = inspect_apk_index(self.root, pinned)
        self.assertTrue(result["pinned_hashes_match"])
        self.assertEqual(result["status"], "THREE_PINNED_ORIGINAL_APK_ZIP_INDEXES_VERIFIED")
        self.assertTrue(result["both_lookup_names_absent_from_inspected_splits"])
        self.assertEqual(len(result["exact_library_matches"]["lib_burst_generated.so"]), 1)
        self.assertEqual(result["absence_in_large_uninspected_split"], "UNKNOWN")
        self.assertEqual(result["causal_link_to_original_exit"], "NOT_ESTABLISHED")

    def test_both_names_present_do_not_imply_loadability(self):
        pinned = self.archives({
            "base.apk": [],
            "arm64.apk": ["lib/arm64-v8a/lib_burst_0_0.so",
                          "lib/arm64-v8a/liblib_burst_0_0.so"],
            "density.apk": [],
        })
        result = inspect_apk_index(self.root, pinned)
        self.assertFalse(result["both_lookup_names_absent_from_inspected_splits"])
        self.assertEqual(result["loadable_at_runtime"], "NOT_ESTABLISHED")

    def test_bad_sha_rejects_before_zip_inspection(self):
        pinned = self.archives({"base.apk": ["abc"]})
        pinned["base.apk"] = "0" * 64
        result = inspect_apk_index(self.root, pinned)
        self.assertFalse(result["pinned_hashes_match"])
        self.assertEqual(result["status"], "UNVERIFIED_OR_MISSING_ORIGINAL_SPLITS")
        self.assertNotIn("exact_library_matches", result)

    def test_missing_split_rejected(self):
        self.archives({"base.apk": []})
        result = inspect_apk_index(
            self.root, {"base.apk": "0" * 64, "arm64.apk": "1" * 64})
        self.assertEqual(result["status"], "UNVERIFIED_OR_MISSING_ORIGINAL_SPLITS")

    def test_corrupted_zip_rejected_after_matching_sha(self):
        path = self.root / "base.apk"
        path.write_bytes(b"not an archive")
        result = inspect_apk_index(
            self.root, {"base.apk": hashlib.sha256(path.read_bytes()).hexdigest()})
        self.assertEqual(result["status"], "APK_CENTRAL_DIRECTORY_UNREADABLE")

    def test_duplicate_zip_names_rejected(self):
        import warnings
        path = self.root / "base.apk"
        with warnings.catch_warnings():
            warnings.simplefilter("ignore", UserWarning)
            with ZipFile(path, "w") as archive:
                archive.writestr("lib/a.so", "a")
                archive.writestr("lib/a.so", "b")
        result = inspect_apk_index(
            self.root, {"base.apk": hashlib.sha256(path.read_bytes()).hexdigest()})
        self.assertEqual(result["status"], "AMBIGUOUS_DUPLICATE_ZIP_ENTRIES")


if __name__ == "__main__":
    unittest.main()
