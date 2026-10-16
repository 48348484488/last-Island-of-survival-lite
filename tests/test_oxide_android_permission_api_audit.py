"""Synthetic, fail-closed tests; full original logs are replayed separately."""
import hashlib
import unittest
from unittest.mock import patch
from tools import oxide_android_permission_api_audit as app

BADGING = b"\n".join([
    b"package: name='com.catsbit.oxidesurvivalisland' versionCode='11966' versionName='1.15.11966' platformBuildVersionName='16'",
    b"sdkVersion:'25'", b"targetSdkVersion:'36'",
    b"uses-permission: name='android.permission.INTERNET'",
])


def fixture(pid=2023, secondary=2296):
    return '\n'.join([
        '10-15 18:55:51.207 281 1748 I ActivityTaskManager: START u0 {cmp=com.catsbit.oxidesurvivalisland/.OxideActivity}',
        f'10-15 18:55:51.215 281 314 I ActivityManager: Start proc {pid}:com.catsbit.oxidesurvivalisland/u0a87 for next-top-activity',
        f'10-15 18:55:51.531 {pid} 2163 W System.err: Caused by: java.lang.SecurityException: Need to declare android.permission.REQUEST_INSTALL_PACKAGES to call this api',
        f'10-15 18:55:51.531 {pid} 2163 W System.err: at android.app.ApplicationPackageManager.canRequestPackageInstalls(ApplicationPackageManager.java:3608)',
        f"10-15 18:55:51.756 {pid} 2129 I Unity: Built from staging, Scripting Backend 'il2cpp', CPU 'arm64-v8a'",
        f'10-15 18:55:52.055 {pid} 2129 I Unity: Product Name: OXIDE',
        f'10-15 18:55:55.687 {pid} {pid} I Process: Sending signal. PID: {pid} SIG: 9',
        f'10-15 18:56:01.449 281 314 I ActivityManager: Start proc {secondary}:com.catsbit.oxidesurvivalisland/u0a87 for service {{com.catsbit.oxidesurvivalisland/com.google.android.gms.measurement.AppMeasurementJobService}}',
        f'10-15 18:56:13.773 {secondary} 2403 W System.err: Caused by: java.lang.SecurityException: Need to declare android.permission.REQUEST_INSTALL_PACKAGES to call this api',
    ]).encode()


class PermissionContractTests(unittest.TestCase):
    def analyze(self, data=None, manifest=BADGING, expected=2023):
        data = fixture() if data is None else data
        with patch.dict(app.LOG_SHA256, {'android15': hashlib.sha256(data).hexdigest()}), patch.dict(app.EXPECTED_ORIGINAL_PID, {'android15': expected}):
            return app.analyze(data, manifest, 'android15')

    def test_original_like_exception_precedes_unity_and_exit(self):
        r = self.analyze()
        self.assertTrue(r['verified'])
        self.assertEqual(r['status'], 'ORIGINAL_UNDECLARED_INSTALL_PERMISSION_API_EXCEPTION_CONFIRMED')
        self.assertEqual(r['exception_to_unity_il2cpp_ms'], 225)
        self.assertEqual(r['exception_to_original_sigkill_ms'], 4156)
        self.assertEqual(r['secondary_process_ids'], [2296])
        self.assertFalse(r['original_gameplay_verified'])
        self.assertEqual(r['exception_caused_original_sigkill'], 'NOT_ESTABLISHED')

    def test_pinned_original_logcat_checksum_blocks_altered_records(self):
        data = fixture()
        with patch.dict(app.LOG_SHA256, {'android15': '0' * 64}):
            self.assertEqual(app.analyze(data, BADGING, 'android15')['status'], 'BLOCKED_LOGCAT_PROVENANCE')

    def test_permission_present_in_manifest_does_not_qualify(self):
        manifest = BADGING + b"\nuses-permission: name='android.permission.REQUEST_INSTALL_PACKAGES'\n"
        self.assertFalse(self.analyze(manifest=manifest)['verified'])

    def test_missing_stack_method_not_correlated(self):
        data = fixture().replace(b'ApplicationPackageManager.canRequestPackageInstalls', b'ApplicationPackageManager.other')
        self.assertFalse(self.analyze(data)['verified'])

    def test_other_thread_stack_not_correlated(self):
        data = fixture().replace(b'2023 2163 W System.err: at android.app', b'2023 7777 W System.err: at android.app')
        self.assertFalse(self.analyze(data)['verified'])

    def test_other_process_exception_not_original(self):
        data = fixture().replace(b'2023 2163 W System.err: Caused by', b'1234 2163 W System.err: Caused by')
        self.assertFalse(self.analyze(data)['verified'])

    def test_late_exception_after_unity_invalid(self):
        data = fixture().replace(b'18:55:51.531 2023', b'18:55:53.531 2023')
        self.assertFalse(self.analyze(data)['verified'])

    def test_missing_target_sdk_or_different_package_rejected(self):
        self.assertEqual(self.analyze(manifest=BADGING.replace(b"targetSdkVersion:'36'", b"targetSdkVersion:'28'"))['status'], 'BLOCKED_MANIFEST_METADATA')
        self.assertEqual(self.analyze(manifest=BADGING.replace(b'com.catsbit.oxidesurvivalisland', b'other.game'))['status'], 'BLOCKED_MANIFEST_METADATA')

    def test_later_service_not_required_or_causal(self):
        data = fixture().replace(b'AppMeasurementJobService', b'SomeOtherService')
        result = self.analyze(data)
        self.assertTrue(result['verified'])
        self.assertFalse(result['same_permission_exception_repeated_in_later_service_process'])
        self.assertEqual(result['security_threat_alert_caused_by_exception'], 'NOT_ESTABLISHED')


if __name__ == '__main__':
    unittest.main()
