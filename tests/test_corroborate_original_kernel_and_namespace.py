import sys
import unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from corroborate_original_kernel_and_namespace import prove

G = "com.catsbit.oxidesurvivalisland"
MAP = {
    "status": "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN",
    "guest_pid": 2092, "host_pid": 12360, "nspid_chain": [12360, 2092],
    "original_process_cmdline": G, "host_start_ticks": 17500,
    "observed_host_boot_ns": 10000000000,
}
FORENSIC = {
    "initial_activity_pid": 2092, "classification": "ORIGINAL_ACTIVITY_SELF_SIGKILL",
    "activity_finish": "10-14 18:18:11.201",
    "first_self_sigkill": "10-14 18:18:11.204",
}
EXIT = ("ApplicationExitInfo #1:\n  timestamp=2026-10-14 18:18:11.254 "
        "pid=2092 process=" + G + " reason=2 (SIGNALED) "
        "subreason=0 (UNKNOWN) status=9\n")
TRACE = (
    "Attaching 3 probes...\n"
    "KERNEL_SIGKILL syscall=kill host_sender=12360 host_tid=12360 "
    "target_pid=2092 signal=9 comm=esurvivalisland boot_ns=14300000000\\n\n"
    "KERNEL_SIGKILL syscall=kill host_sender=9576 host_tid=9576 "
    "target_pid=-2092 signal=9 comm=ActivityManager boot_ns=14600000000\\n\n"
)


class Tests(unittest.TestCase):
    def test_independent_kernel_original_host_android_guest_same_run(self):
        r = prove(MAP, TRACE, FORENSIC, EXIT)
        self.assertEqual(r["status"], "SAME_RUN_TIMESTAMPED_HOST_KERNEL_GUEST_SELF_SIGKILL_PROVEN")
        self.assertEqual(r["linux_host_pid"], 12360)
        self.assertTrue(r["kernel_clock_temporal_window_verified"])
        self.assertEqual(r["first_matched_event_after_host_pid_sample_ms"], 4300.0)
        self.assertEqual(r["matching_real_original_kernel_kill_syscalls"], 1)
        self.assertFalse(r["activity_finish_requester_identified"])
        self.assertFalse(r["playable_game_verified"])

    def test_missing_kernel_monotonic_timestamp_fails(self):
        self.assertEqual(
            prove(MAP, TRACE.replace(" boot_ns=14300000000", ""), FORENSIC, EXIT)["status"],
            "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_signal_before_pid_observation_fails(self):
        self.assertEqual(
            prove(MAP, TRACE.replace("boot_ns=14300000000", "boot_ns=9000000000"), FORENSIC, EXIT)["status"],
            "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_signal_beyond_temporal_window_fails(self):
        self.assertEqual(
            prove(MAP, TRACE.replace("boot_ns=14300000000", "boot_ns=30000000000"), FORENSIC, EXIT)["status"],
            "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_host_clock_missing_fails(self):
        m=dict(MAP)
        del m["observed_host_boot_ns"]
        self.assertEqual(prove(m, TRACE, FORENSIC, EXIT)["status"],
                         "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_host_sender_other_process_is_not_original(self):
        self.assertEqual(prove(MAP, TRACE.replace("host_sender=12360", "host_sender=9576"),
                               FORENSIC, EXIT)["matching_real_original_kernel_kill_syscalls"], 0)

    def test_target_another_android_pid_fails(self):
        self.assertEqual(prove(MAP, TRACE.replace("target_pid=2092", "target_pid=9999"),
                               FORENSIC, EXIT)["matching_real_original_kernel_kill_syscalls"], 0)

    def test_non_original_command_fails(self):
        bad = dict(MAP, original_process_cmdline=G + ":analytics")
        self.assertFalse(prove(bad, TRACE, FORENSIC, EXIT)["live_namespace_mapping_verified"])

    def test_mismatched_host_namespace_fails(self):
        bad = dict(MAP, nspid_chain=[12360, 1111])
        self.assertFalse(prove(bad, TRACE, FORENSIC, EXIT)["live_namespace_mapping_verified"])

    def test_missing_system_exit_status9_fails(self):
        self.assertEqual(prove(MAP, TRACE, FORENSIC, EXIT.replace("status=9", "status=0"))["status"],
                         "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_no_activity_finish_fails(self):
        bad = dict(FORENSIC, activity_finish=None)
        self.assertEqual(prove(MAP, TRACE, bad, EXIT)["status"], "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_finish_after_self_sigkill_fails(self):
        bad = dict(FORENSIC, activity_finish="10-14 18:18:11.999")
        self.assertEqual(prove(MAP, TRACE, bad, EXIT)["status"], "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_wrong_exit_pid_fails(self):
        self.assertEqual(prove(MAP, TRACE, FORENSIC, EXIT.replace("pid=2092", "pid=2001"))["status"],
                         "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_tgkill_not_falsely_kill(self):
        tr = TRACE.replace("syscall=kill", "syscall=tgkill").replace("target_pid=", "target_tgid=")
        self.assertEqual(prove(MAP, tr, FORENSIC, EXIT)["matching_real_original_kernel_kill_syscalls"], 0)

    def test_truncated_signal_line_fails(self):
        self.assertEqual(prove(MAP, "KERNEL_SIGKILL syscall=kill host_sender=", FORENSIC, EXIT)["status"],
                         "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")

    def test_kernel_comm_unrelated_rejected(self):
        self.assertEqual(prove(MAP, TRACE.replace("comm=esurvivalisland", "comm=system_server"),
                               FORENSIC, EXIT)["status"], "SAME_RUN_TIMESTAMPED_KERNEL_SENDER_NOT_PROVEN")


if __name__ == "__main__":
    unittest.main()
