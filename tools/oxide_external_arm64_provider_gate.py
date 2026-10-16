#!/usr/bin/env python3
"""Read-only, fail-closed decision gate for documented external ARM64 virtual hosts.

A provider description is NOT an execution test. Even a passing provider
metadata check never authorizes an Android boot or original APK download.
"""
import argparse
import json
from pathlib import Path
from urllib.parse import urlparse

KNOWN_PROVIDER_CONSTRAINTS = {
    "firebase_test_lab_arm": ("firebase.google.com", "/docs/test-lab/android/avds"),
    "sauce_labs_arm": ("docs.saucelabs.com", "/mobile-apps/android-emulators/"),
}

REQUIRED = ("native_arm64_virtual", "play_store_app_supported",
            "no_new_payment_required", "authorized_access_verified",
            "host_acceleration_verified", "legitimate_services_verified")


def assess(data):
    def invalid(reason):
        return {"status": "INVALID_EXTERNAL_PROVIDER_EVIDENCE", "reason": reason,
                "metadata_preflight_candidates": [], "gameplay_verified": False,
                "heavy_android_or_apk_trial_authorized": False}

    if not isinstance(data, dict) or type(data.get("schema_version")) is not int or data["schema_version"] != 1:
        return invalid("INVALID_SCHEMA")
    providers = data.get("providers")
    if not isinstance(providers, list) or not providers:
        return invalid("NO_PROVIDERS")
    seen = set()
    decisions = []
    for item in providers:
        if not isinstance(item, dict) or not isinstance(item.get("name"), str) or not item["name"]:
            return invalid("INVALID_PROVIDER_NAME")
        if item["name"] in seen:
            return invalid("DUPLICATE_PROVIDER")
        seen.add(item["name"])
        if item.get("evidence_level") != "official_documentation_only":
            return invalid("UNVERIFIED_EVIDENCE_LEVEL")
        source = item.get("official_source_url")
        if not isinstance(source, str) or urlparse(source).scheme != "https":
            return invalid("MISSING_OFFICIAL_HTTPS_SOURCE")
        expected = KNOWN_PROVIDER_CONSTRAINTS.get(item["name"])
        if expected is None or (urlparse(source).hostname, urlparse(source).path) != expected:
            return invalid("PROVIDER_SOURCE_MISMATCH")
        if any(item.get(field) is not None and type(item[field]) is not bool for field in REQUIRED):
            return invalid("NON_BOOLEAN_CAPABILITY")
        if any(field not in item for field in REQUIRED):
            return invalid("MISSING_CAPABILITY")
        if item.get("play_store_app_supported") is not False:
            return invalid("CONTRADICTS_DOCUMENTED_PLAY_STORE_LIMITATION")
        if item.get("no_new_payment_required") is not False:
            return invalid("CONTRADICTS_DOCUMENTED_ACCESS_LIMITATION")
        if item.get("original_gameplay_verified") is not False:
            return invalid("UNSUPPORTED_GAMEPLAY_CLAIM")
        blockers = [key for key in REQUIRED if item[key] is not True]
        decisions.append({"name": item["name"],
                          "metadata_preflight_candidate": not blockers,
                          "unverified_or_failed_prerequisites": blockers})
    candidates = [d["name"] for d in decisions if d["metadata_preflight_candidate"]]
    return {
        "status": ("HOST_METADATA_PREFLIGHT_CANDIDATE_ONLY" if candidates
                   else "NO_VERIFIED_ELIGIBLE_EXTERNAL_ARM64_VIRTUAL_HOST"),
        "evidence_validated": True,
        "providers": decisions,
        "metadata_preflight_candidates": candidates,
        "same_activity_same_pid_35s_verified": False,
        "gameplay_verified": False,
        "heavy_android_or_apk_trial_authorized": False,
        "next_action": ("CHECK_HOST_METADATA_ONLY_NO_ANDROID_OR_APK" if candidates
                        else "STOP_WAIT_FOR_DISTINCT_DOCUMENTED_HOST_CAPABILITY"),
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--evidence", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    try:
        evidence = json.loads(args.evidence.read_text(encoding="utf-8"))
    except (OSError, ValueError):
        evidence = None
    verdict = assess(evidence)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(verdict, indent=2, sort_keys=True) + "\n",
                           encoding="utf-8")
    print(json.dumps(verdict, sort_keys=True))
    return 0 if verdict.get("evidence_validated") else 2


if __name__ == "__main__":
    raise SystemExit(main())
