import copy
import json
import unittest
from pathlib import Path
from tools.oxide_external_arm64_provider_gate import assess

ROOT = Path(__file__).resolve().parents[1]
EVIDENCE = json.loads((ROOT / "evidence/oxide_external_arm64_providers_2026-10-16.json").read_text())


class ProviderGateTests(unittest.TestCase):
    def test_actual_documented_providers_blocked(self):
        result = assess(EVIDENCE)
        self.assertEqual(result["status"], "NO_VERIFIED_ELIGIBLE_EXTERNAL_ARM64_VIRTUAL_HOST")
        self.assertEqual(result["metadata_preflight_candidates"], [])
        self.assertFalse(result["heavy_android_or_apk_trial_authorized"])
        self.assertFalse(result["gameplay_verified"])

    def test_missing_capability_fails_closed(self):
        data = copy.deepcopy(EVIDENCE)
        del data["providers"][0]["play_store_app_supported"]
        self.assertEqual(assess(data)["status"], "INVALID_EXTERNAL_PROVIDER_EVIDENCE")

    def test_string_boolean_fails_closed(self):
        data = copy.deepcopy(EVIDENCE)
        data["providers"][0]["play_store_app_supported"] = "true"
        self.assertEqual(assess(data)["reason"], "NON_BOOLEAN_CAPABILITY")

    def test_unknown_source_fails_closed(self):
        data = copy.deepcopy(EVIDENCE)
        data["providers"][0]["official_source_url"] = "https://example.org/claim"
        self.assertEqual(assess(data)["reason"], "PROVIDER_SOURCE_MISMATCH")

    def test_duplicate_provider_fails_closed(self):
        data = copy.deepcopy(EVIDENCE)
        data["providers"].append(copy.deepcopy(data["providers"][0]))
        self.assertEqual(assess(data)["reason"], "DUPLICATE_PROVIDER")

    def test_no_claim_of_gameplay(self):
        data = copy.deepcopy(EVIDENCE)
        data["providers"][0]["original_gameplay_verified"] = True
        self.assertEqual(assess(data)["reason"], "UNSUPPORTED_GAMEPLAY_CLAIM")

    def test_documented_play_store_limitation_cannot_be_overridden(self):
        data = copy.deepcopy(EVIDENCE)
        data["providers"][0]["play_store_app_supported"] = True
        self.assertEqual(assess(data)["reason"], "CONTRADICTS_DOCUMENTED_PLAY_STORE_LIMITATION")

    def test_documented_payment_limitation_cannot_be_overridden(self):
        data = copy.deepcopy(EVIDENCE)
        data["providers"][1]["no_new_payment_required"] = True
        self.assertEqual(assess(data)["reason"], "CONTRADICTS_DOCUMENTED_ACCESS_LIMITATION")

    def test_provider_source_cannot_be_swapped(self):
        data = copy.deepcopy(EVIDENCE)
        data["providers"][0]["official_source_url"] = data["providers"][1]["official_source_url"]
        self.assertEqual(assess(data)["reason"], "PROVIDER_SOURCE_MISMATCH")

    def test_no_documented_provider_can_be_a_candidate(self):
        data = copy.deepcopy(EVIDENCE)
        for field in ("native_arm64_virtual", "authorized_access_verified",
                      "host_acceleration_verified", "legitimate_services_verified"):
            data["providers"][0][field] = True
        result = assess(data)
        self.assertEqual(result["metadata_preflight_candidates"], [])
        self.assertFalse(result["heavy_android_or_apk_trial_authorized"])

    def test_no_provider_fails_closed(self):
        self.assertEqual(assess({"schema_version": 1, "providers": []})["reason"], "NO_PROVIDERS")


if __name__ == "__main__":
    unittest.main()
