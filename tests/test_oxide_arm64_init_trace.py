"""Synthetic regression guards for original Redroid ARM64 init timeline."""
import unittest
from tools.oxide_arm64_init_trace import inspect_native_logcat, compare_native_logs


def fixture(pid, start, proc, built, product, burst, lookup_a, lookup_b, close, killed):
    activity = "com.catsbit.oxidesurvivalisland/.OxideActivity"
    return "\n".join((
        f"10-15 {start} 281 1748 I ActivityTaskManager: START u0 {{cmp={activity}}}",
        f"10-15 {proc} 281 314 I ActivityManager: Start proc {pid}:com.catsbit.oxidesurvivalisland/u0a87",
        f"10-15 {built} {pid} 2129 I Unity   : Built from '6000.3/staging' branch, Scripting Backend 'il2cpp'",
        f"10-15 {product} {pid} 2129 I Unity   : Product Name: OXIDE",
        f"10-15 {burst} {pid} 2129 D nativeloader: Load /data/app/original/lib/arm64/lib_burst_generated.so using class loader ns clns-7 (caller=/data/app/base.apk): ok",
        f'10-15 {lookup_a} {pid} 2129 D nativeloader: Load liblib_burst_0_0.so using class loader ns clns-7 (caller=/data/app/base.apk): dlopen failed: library "liblib_burst_0_0.so" not found',
        f'10-15 {lookup_b} {pid} 2129 D nativeloader: Load lib_burst_0_0.so using class loader ns clns-7 (caller=/data/app/base.apk): dlopen failed: library "lib_burst_0_0.so" not found',
        f"10-15 {killed} {pid} {pid} I Process : Sending signal. PID: {pid} SIG: 9",
        f"10-15 {killed} 281 302 V WindowManager: Sent Transition (#6) createdAt=10-15 {close} via request=TransitionRequestInfo {{ type = CLOSE, triggerTask = TaskInfo{{taskId=8 baseIntent=Intent {{ cmp={activity} }} }} }}",
    )).encode("utf-8")


A15 = fixture(2023, "18:55:51.207", "18:55:51.215",
              "18:55:51.756", "18:55:52.055",
              "18:55:55.297", "18:55:55.299", "18:55:55.299",
              "18:55:55.684", "18:55:55.687")
A16 = fixture(2108, "18:38:25.470", "18:38:25.481",
              "18:38:26.023", "18:38:26.420",
              "18:38:30.025", "18:38:30.026", "18:38:30.027",
              "18:38:30.137", "18:38:30.140")


class OriginalNativeInitTests(unittest.TestCase):
    def test_native_runs_share_an_ordered_sequence_not_a_cause(self):
        report = compare_native_logs(A15, A16)
        self.assertTrue(report["replay_verified"])
        self.assertEqual(report["status"], "SHARED_OPTIONAL_BURST_LOOKUP_BEFORE_EXIT_OBSERVED")
        self.assertEqual(report["executions"]["android15"]["burst_last_lookup_to_self_sigkill_ms"], 388)
        self.assertEqual(report["executions"]["android16"]["burst_last_lookup_to_self_sigkill_ms"], 113)
        self.assertEqual(report["executions"]["android15"]["task_close_to_self_sigkill_ms"], 3)
        self.assertEqual(report["executions"]["android16"]["task_close_to_self_sigkill_ms"], 3)
        self.assertEqual(report["native_security_trigger"], "UNKNOWN")
        self.assertEqual(report["causal_link_from_burst_lookup_to_exit"], "NOT_ESTABLISHED")
        self.assertFalse(report["gameplay_verified"])

    def test_missing_lookup_is_not_two_confirmed_failures(self):
        changed = A15.replace(b"liblib_burst_0_0.so", b"otherlib.so")
        self.assertFalse(compare_native_logs(changed, A16)["replay_verified"])

    def test_other_pid_signal_is_rejected(self):
        changed = A15.replace(b"2023 2023 I Process", b"9999 9999 I Process")
        report = inspect_native_logcat(changed)
        self.assertFalse(report["all_required_milestones_in_order"])
        self.assertNotIn("self_sigkill", report["observations"])

    def test_other_pid_lookup_is_rejected(self):
        changed = A16.replace(b"2108 2129 D nativeloader: Load lib_burst_0_0.so",
                              b"2222 2129 D nativeloader: Load lib_burst_0_0.so")
        self.assertFalse(inspect_native_logcat(changed)["all_required_milestones_in_order"])

    def test_lookup_after_exit_is_rejected(self):
        changed = A16.replace(b"18:38:30.027 2108", b"18:38:30.141 2108")
        self.assertFalse(inspect_native_logcat(changed)["all_required_milestones_in_order"])

    def test_wrong_activity_does_not_count(self):
        changed = A15.replace(b"cmp=com.catsbit.oxidesurvivalisland/.OxideActivity",
                              b"cmp=org.other/.Main")
        report = inspect_native_logcat(changed)
        self.assertIsNone(report["original_pid"])
        self.assertFalse(report["all_required_milestones_in_order"])

    def test_other_unity_product_does_not_count(self):
        changed = A16.replace(b"Product Name: OXIDE", b"Product Name: OTHER")
        self.assertFalse(inspect_native_logcat(changed)["all_required_milestones_in_order"])

    def test_generated_burst_loaded_before_optional_failed_lookups(self):
        report = inspect_native_logcat(A15)
        self.assertEqual(report["observations"]["burst_generated_loaded"]["elapsed_from_activity_start_ms"], 4090)
        self.assertEqual(report["optional_burst_lookup_missing_library_is_crash_cause"], "NOT_ESTABLISHED")


if __name__ == "__main__":
    unittest.main()
