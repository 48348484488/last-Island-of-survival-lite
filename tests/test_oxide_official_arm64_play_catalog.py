import unittest
from tools.oxide_official_arm64_play_catalog import parse_catalog, probe

SAMPLE = b"""<?xml version="1.0"?>
<sdk:sdk-repository xmlns:sdk="http://schemas.android.com/sdk/android/repository/12">
  <remotePackage path="system-images;android-36;google_apis_playstore;arm64-v8a">
    <revision><major>4</major></revision><channelRef ref="channel-0"/>
    <archives><archive><complete><size>1000000</size></complete></archive></archives>
  </remotePackage>
  <remotePackage path="system-images;android-34;google_apis_playstore;x86_64">
    <revision><major>12</major></revision></remotePackage>
  <remotePackage path="system-images;android-35;google_apis;arm64-v8a">
    <revision><major>2</major></revision></remotePackage>
</sdk:sdk-repository>
"""


class CatalogTests(unittest.TestCase):
    def test_parses_only_arm64_playstore(self):
        image = parse_catalog(SAMPLE)
        self.assertEqual(len(image), 1)
        self.assertEqual(image[0]["api_level"], 36)
        self.assertEqual(image[0]["revision"], 4)
        self.assertEqual(image[0]["min_archive_size_bytes"], 1000000)

    def test_does_not_count_beta_as_stable(self):
        xml = SAMPLE.replace(b"channel-0", b"channel-2")
        self.assertEqual(parse_catalog(xml), [])

    def test_selects_recent_and_never_claims_boot(self):
        r = probe(opener=lambda url: SAMPLE)
        self.assertEqual(r["status"], "OFFICIAL_ARM64_PLAY_IMAGE_CATALOGED_BOOT_UNTESTED")
        self.assertFalse(r["android_booted"])
        self.assertFalse(r["gameplay_verified"])
        self.assertEqual(len(r["recent_api_35_or_newer"]), 1)

    def test_only_old_image_does_not_clear_gate(self):
        r = probe(opener=lambda url: SAMPLE.replace(b"android-36", b"android-31"))
        self.assertEqual(r["status"], "NO_RECENT_ARM64_PLAY_IMAGE_IN_THIS_CATALOG")

    def test_all_catalog_errors_fail_closed(self):
        def offline(url):
            raise OSError("network unavailable")
        self.assertEqual(probe(opener=offline)["status"], "OFFICIAL_CATALOG_UNAVAILABLE")


if __name__ == "__main__":
    unittest.main()
