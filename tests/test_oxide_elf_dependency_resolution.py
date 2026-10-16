"""Offline SHA-gated ELF DT_NEEDED graph reconciliation tests (no APK)."""
import json
import unittest

from tools.oxide_elf_dependency_resolution import summarize_dependency_edges
from tools.oxide_native_library_inventory import EXPECTED_PINNED_HASHES


def fixture(libraries=None, complete=True):
    if libraries is None:
        libraries = [
            {"path": "lib/arm64-v8a/libmain.so",
             "dt_needed": ["libc.so", "libOther.so"]},
            {"path": "lib/arm64-v8a/libOther.so", "dt_needed": ["liblog.so"]},
        ]
    return json.dumps({
        "status": "ORIGINAL_ARM64_DYNAMIC_DEPENDENCIES_READ_ONLY_VERIFIED",
        "all_packaged_arm64_libraries_examined": complete,
        "large_asset_pack_inspected": False,
        "large_libraries_not_examined": [],
        "original_apk_sha256_verified": {
            name: {"sha256": digest, "size_bytes": 10}
            for name, digest in EXPECTED_PINNED_HASHES.items()
        },
        "arm64_libraries_inspected": len(libraries),
        "libraries": libraries,
    }).encode()


class DependencyEdgeGraphTests(unittest.TestCase):
    def test_packaged_and_known_ndk_are_distinct(self):
        r = summarize_dependency_edges(fixture())
        self.assertEqual(r["status"], "ELF_DECLARED_DEPENDENCY_NAMES_ACCOUNTED_FOR")
        self.assertEqual(r["total_declared_dependency_edges"], 3)
        self.assertEqual(r["packaged_dependency_names"], {"libOther.so": 1})
        self.assertEqual(r["expected_ndk_system_dependency_edges"], 2)
        self.assertEqual(r["unaccounted_for_names"], {})
        self.assertEqual(r["security_trigger"], "UNKNOWN")
        self.assertFalse(r["original_gameplay_verified"])

    def test_unknown_nonplatform_name_not_silently_accepted(self):
        libs = [{"path": "lib/arm64-v8a/libmain.so",
                 "dt_needed": ["libUnknownCustom.so"]}]
        r = summarize_dependency_edges(fixture(libraries=libs))
        self.assertEqual(r["status"], "UNACCOUNTED_FOR_DECLARED_DEPENDENCY_NAMES")
        self.assertEqual(r["unaccounted_for_names"], {"libUnknownCustom.so": 1})

    def test_partial_coverage_is_rejected(self):
        r = summarize_dependency_edges(fixture(complete=False))
        self.assertEqual(r["status"], "BLOCKED_PARTIAL_OR_UNVERIFIED_ELF_COVERAGE")

    def test_altered_original_hash_is_rejected(self):
        original = json.loads(fixture())
        original["original_apk_sha256_verified"]["base.apk"]["sha256"] = "0" * 64
        r = summarize_dependency_edges(json.dumps(original).encode())
        self.assertEqual(r["status"], "BLOCKED_ORIGINAL_APK_SHA_MISMATCH")

    def test_duplicate_basename_rejected(self):
        libs = [{"path": "lib/arm64-v8a/libSame.so", "dt_needed": []},
                {"path": "lib/arm64-v8a/libSame.so", "dt_needed": []}]
        r = summarize_dependency_edges(fixture(libraries=libs))
        self.assertEqual(r["status"], "BLOCKED_DUPLICATE_LIBRARY_BASENAME")

    def test_invalid_needed_library_path_rejected(self):
        libs = [{"path": "lib/arm64-v8a/libmain.so",
                 "dt_needed": ["../bad.so"]}]
        r = summarize_dependency_edges(fixture(libraries=libs))
        self.assertEqual(r["status"], "BLOCKED_DYNAMIC_LIBRARY_NAMES")

    def test_invalid_json_rejected(self):
        r = summarize_dependency_edges(b"not-json")
        self.assertEqual(r["status"], "BLOCKED_INVALID_SOURCE_JSON")


if __name__ == "__main__":
    unittest.main()
