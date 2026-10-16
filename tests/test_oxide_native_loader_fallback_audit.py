"""Synthetic regressions for original native plugin loader naming chronology."""
import unittest
from tools.oxide_native_loader_fallback_audit import analyze_logcat, compare_logs


def fixture(pid, start, proc, audio_fail, audio_ok, generated, burst1, burst2, killed):
    activity = "com.catsbit.oxidesurvivalisland/.OxideActivity"
    return "\n".join((
        f"10-15 {start} 281 301 I ActivityTaskManager: START u0 {{cmp={activity}}}",
        f"10-15 {proc} 281 301 I ActivityManager: Start proc {pid}:com.catsbit.oxidesurvivalisland/u0a87",
        f'10-15 {audio_fail} {pid} 2129 D nativeloader: Load liblibAudioPluginDissonance.so using class loader ns clns-7 (caller=base.apk): dlopen failed: library "liblibAudioPluginDissonance.so" not found',
        f"10-15 {audio_ok} {pid} 2129 D nativeloader: Load /data/app/original/lib/arm64/libAudioPluginDissonance.so using class loader ns clns-7 (caller=base.apk): ok",
        f"10-15 {generated} {pid} 2129 D nativeloader: Load /data/app/original/lib/arm64/lib_burst_generated.so using class loader ns clns-7 (caller=base.apk): ok",
        f'10-15 {burst1} {pid} 2129 D nativeloader: Load liblib_burst_0_0.so using class loader ns clns-7 (caller=base.apk): dlopen failed: library "liblib_burst_0_0.so" not found',
        f'10-15 {burst2} {pid} 2129 D nativeloader: Load lib_burst_0_0.so using class loader ns clns-7 (caller=base.apk): dlopen failed: library "lib_burst_0_0.so" not found',
        f"10-15 {killed} {pid} {pid} I Process : Sending signal. PID: {pid} SIG: 9",
    )).encode()


A15 = fixture(2023, "18:55:51.207", "18:55:51.215", "18:55:52.053",
              "18:55:52.054", "18:55:55.297", "18:55:55.299",
              "18:55:55.299", "18:55:55.687")
A16 = fixture(2108, "18:38:25.470", "18:38:25.481", "18:38:26.419",
              "18:38:26.420", "18:38:30.025", "18:38:30.026",
              "18:38:30.027", "18:38:30.140")


class NativeLoaderFallbackTests(unittest.TestCase):
    def test_both_original_pids_and_same_thread_fallback(self):
        r = compare_logs(A15, A16)
        self.assertTrue(r["evidence_validated"])
        self.assertEqual(r["original_runs"]["android15"]["alternate_audio_name_gap_ms"], 1)
        self.assertEqual(r["original_runs"]["android16"]["alternate_audio_name_gap_ms"], 1)
        self.assertEqual(r["original_runs"]["android15"]["burst_name_failure_gap_ms"], 0)
        self.assertEqual(r["original_runs"]["android16"]["burst_name_failure_gap_ms"], 1)
        self.assertFalse(r["original_gameplay_verified"])

    def test_other_pid_dissonance_success_rejected(self):
        source = A15.replace(
            b"2023 2129 D nativeloader: Load /data/app/original/lib/arm64/libAudioPluginDissonance.so",
            b"5555 2129 D nativeloader: Load /data/app/original/lib/arm64/libAudioPluginDissonance.so")
        self.assertFalse(analyze_logcat(source, 2023)["verified_same_pid_sequence"])

    def test_other_thread_dissonance_success_rejected(self):
        source = A15.replace(
            b"2023 2129 D nativeloader: Load /data/app/original/lib/arm64/libAudioPluginDissonance.so",
            b"2023 9999 D nativeloader: Load /data/app/original/lib/arm64/libAudioPluginDissonance.so")
        self.assertFalse(analyze_logcat(source, 2023)["verified_same_pid_sequence"])

    def test_missing_initial_failure_rejected(self):
        self.assertFalse(analyze_logcat(
            A16.replace(b"liblibAudioPluginDissonance.so", b"other.so"), 2108
        )["verified_same_pid_sequence"])

    def test_missing_burst_second_failure_rejected(self):
        source = A16.replace(
            b"lib_burst_0_0.so using class loader",
            b"other_burst.so using class loader")
        self.assertFalse(analyze_logcat(source, 2108)["verified_same_pid_sequence"])

    def test_burst_name_success_changes_conclusion(self):
        source = A15.replace(
            b'Load lib_burst_0_0.so using class loader ns clns-7 (caller=base.apk): dlopen failed: library "lib_burst_0_0.so" not found',
            b"Load lib_burst_0_0.so using class loader ns clns-7 (caller=base.apk): ok")
        self.assertFalse(analyze_logcat(source, 2023)["verified_same_pid_sequence"])

    def test_events_after_sigkill_rejected(self):
        source = A16.replace(b"18:38:30.027 2108 2129", b"18:38:30.141 2108 2129")
        self.assertFalse(analyze_logcat(source, 2108)["verified_same_pid_sequence"])

    def test_other_activity_not_accepted(self):
        source = A15.replace(
            b"cmp=com.catsbit.oxidesurvivalisland/.OxideActivity",
            b"cmp=com.other/.Activity")
        self.assertFalse(analyze_logcat(source, 2023)["verified_same_pid_sequence"])


if __name__ == "__main__":
    unittest.main()
