"""Synthetic regression for the existing-evidence environment comparison.

Real evidence is separately reprocessed in GitHub Actions; synthetic tests
ensure we reject missing, mismatched, or causally ambiguous logcat sequences.
"""
import unittest
from tools.oxide_original_environment_compare import compare_original_environments


def native_log(pid, start, proc_started, close, sigkill, died):
    package = "com.catsbit.oxidesurvivalisland"
    activity = package + "/.OxideActivity"
    return "\n".join((
        f"10-15 {start}  281 1748 I ActivityTaskManager: START u0 {{cmp={activity}}} from uid 2000",
        f"10-15 {proc_started} 281 314 I ActivityManager: Start proc {pid}:{package}/u0a87 for next-top-activity",
        f"10-15 {sigkill} {pid} {pid} I Process : Sending signal. PID: {pid} SIG: 9",
        f"10-15 {died} 281 1748 I ActivityManager: Process {package} (pid {pid}) has died: fg TOP",
        f"10-15 {died} 281 302 V WindowManager: Sent Transition (#6) createdAt=10-15 {close} "
        f"via request=TransitionRequestInfo {{ type = CLOSE, triggerTask = "
        f"TaskInfo{{taskId=8 baseIntent=Intent {{ cmp={activity} }} }} }}",
    )).encode("utf-8")


ARM15 = native_log(2023, "18:55:51.207", "18:55:51.215", "18:55:55.684",
                   "18:55:55.687", "18:55:55.728")
ARM16 = native_log(2108, "18:38:25.470", "18:38:25.478", "18:38:30.137",
                   "18:38:30.140", "18:38:30.182")
X86 = "\n".join((
    "10-15 22:46:40.128 541 1109 I ActivityTaskManager: START u0 {cmp=com.catsbit.oxidesurvivalisland/.OxideActivity} from uid 2000",
    "10-15 22:46:40.465 541 575 I ActivityManager: Start proc 4426:com.catsbit.oxidesurvivalisland/u0a209 for next-top-activity",
    "10-15 22:46:43.180 4426 4426 E berberis: Undefined instruction 0xd5380429 at 0x0000737d96904a3c",
    "10-15 22:46:43.186 4426 4426 F libc: Fatal signal 4 (SIGILL), code -6 (SI_TKILL) in tid 4426 (esurvivalisland), pid 4426 (esurvivalisland)",
    "10-15 22:46:44.166 4495 4495 F DEBUG: pid: 4426, tid: 4426, name: esurvivalisland  >>> com.catsbit.oxidesurvivalisland <<<",
    "10-15 22:46:44.186 4495 4495 F DEBUG:       #00 pc 0000000000700a3c  /data/app/synthetic/lib/arm64/libloader.so",
    "10-15 22:46:44.186 4495 4495 F DEBUG:       #02 pc 000000000008b144  /system/bin/arm64/linker64 (__dl__ZN6soinfo17call_constructorsEv+604)",
    "10-15 22:46:44.327 350 350 I Zygote: Process 4426 exited due to signal 4 (Illegal instruction)",
)).encode("utf-8")


class OriginalEnvironmentComparisonTests(unittest.TestCase):
    def test_distinct_original_exit_paths_with_no_causality_claim(self):
        report = compare_original_environments(ARM15, ARM16, X86)
        self.assertEqual(report["status"], "DISTINCT_ORIGINAL_TERMINATION_PATHS_VERIFIED")
        self.assertTrue(report["all_checks_passed"])
        self.assertEqual(report["executions"]["android15"]["start_to_signal_ms"], 4480)
        self.assertEqual(report["executions"]["android16"]["start_to_signal_ms"], 4670)
        self.assertEqual(report["executions"]["x86_translated"]["berberis_to_signal_ms"], 6)
        self.assertEqual(report["native_redroid_cause"], "UNKNOWN")
        self.assertEqual(report["shared_causality"], "NOT_ESTABLISHED")
        self.assertFalse(report["original_gameplay_verified"])

    def test_native_missing_sigkill_must_fail_closed(self):
        report = compare_original_environments(ARM15.replace(b"SIG: 9", b"SIG: 6"), ARM16, X86)
        self.assertFalse(report["all_checks_passed"])
        self.assertFalse(report["checks"]["android15"]["original_self_sigkill"])

    def test_native_wrong_pid_must_fail_closed(self):
        report = compare_original_environments(ARM15.replace(b"Start proc 2023:", b"Start proc 9999:"), ARM16, X86)
        self.assertFalse(report["all_checks_passed"])
        self.assertFalse(report["checks"]["android15"]["expected_original_pid"])

    def test_translated_missing_opcode_must_fail_closed(self):
        report = compare_original_environments(ARM15, ARM16,
            X86.replace(b"0xd5380429", b"0xd503201f"))
        self.assertFalse(report["all_checks_passed"])
        self.assertFalse(report["checks"]["x86_translated"]["original_berberis_sigill_replay"])


if __name__ == "__main__":
    unittest.main()
