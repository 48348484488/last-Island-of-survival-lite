"""Mock Linux /proc fixtures including legitimate and mismatched NSpid entries."""
import tempfile
import unittest
from pathlib import Path
import sys
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from prove_original_host_pid_namespace import inspect, crosscheck

GAME = "com.catsbit.oxidesurvivalisland"


def process(root, host_pid, nspids, ns="pid:[4024242]", command=GAME):
    path = Path(root) / str(host_pid)
    (path / "ns").mkdir(parents=True)
    (path / "status").write_text("Name:\toxide\nNSpid:\t" + "\t".join(map(str, nspids)) + "\n")
    (path / "ns" / "pid").symlink_to(ns)
    (path / "cmdline").write_bytes(command.encode() + b"\x00")
    (path / "stat").write_text(str(host_pid) + " (with spaces) S " + " ".join(
        str(i + 100) for i in range(32)) + "\n")


class Tests(unittest.TestCase):
    def setUp(self):
        t = tempfile.TemporaryDirectory()
        self.addCleanup(t.cleanup)
        self.proc = Path(t.name)
        process(self.proc, 7100, [7100, 1], command="/init")
        process(self.proc, 8123, [8123, 2211])
        process(self.proc, 8880, [8880, 2212], command=GAME + ":analytics")

    def test_exact_real_original_host_pid(self):
        result = inspect(self.proc, 7100, 2211)
        self.assertEqual(result["status"], "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN")
        self.assertEqual(result["host_pid"], 8123)
        self.assertEqual(result["nspid_chain"], [8123, 2211])
        self.assertEqual(result["container_init_nspid_chain"], [7100, 1])
        self.assertGreater(result["host_start_ticks"], 0)
        self.assertFalse(result["security_reason"] != "NOT_IDENTIFIED")

    def test_wrong_android_pid_fails(self):
        self.assertEqual(inspect(self.proc, 7100, 9999)["matches"], 0)

    def test_other_android_pid_namespace_rejected(self):
        process(self.proc, 8222, [8222, 2211], ns="pid:[123456]")
        self.assertEqual(inspect(self.proc, 7100, 2211)["host_pid"], 8123)

    def test_another_matching_original_process_makes_ambiguous(self):
        process(self.proc, 8222, [8222, 2211])
        self.assertEqual(inspect(self.proc, 7100, 2211)["matches"], 2)
        self.assertNotEqual(inspect(self.proc, 7100, 2211)["status"],
                            "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN")

    def test_same_guest_pid_wrong_command_rejected(self):
        process(self.proc, 8222, [8222, 2211], command="system_server")
        self.assertEqual(inspect(self.proc, 7100, 2211)["host_pid"], 8123)

    def test_fake_container_init_rejected(self):
        process(self.proc, 7300, [7300, 42], command="/init")
        self.assertEqual(inspect(self.proc, 7300, 2211)["status"],
                         "CONTAINER_GUEST_INIT_NOT_VERIFIED")

    def test_missing_container_root_refused(self):
        self.assertEqual(inspect(self.proc, 9000, 2211)["status"],
                         "CONTAINER_PID1_NOT_VERIFIED")

    def test_host_only_process_nspid_rejected(self):
        process(self.proc, 8222, [8222], command=GAME)
        self.assertEqual(inspect(self.proc, 7100, 8222)["matches"], 0)

    def test_valid_exit_crosscheck(self):
        result = inspect(self.proc, 7100, 2211)
        x = crosscheck(result, {
            "initial_activity_pid": 2211,
            "classification": "ORIGINAL_ACTIVITY_SELF_SIGKILL",
            "first_self_sigkill": "10-14 10:05:01.260",
        })
        self.assertEqual(x["status"], "HOST_AND_GUEST_PID_LINKED_TO_ORIGINAL_SELF_SIGKILL")
        self.assertFalse(x["host_kernel_signal_emitter_at_same_instant_observed"])
        self.assertFalse(x["finish_activity_requester_identified"])

    def test_exit_of_background_service_rejected(self):
        result = inspect(self.proc, 7100, 2211)
        x = crosscheck(result, {
            "initial_activity_pid": 2212,
            "classification": "ORIGINAL_ACTIVITY_SELF_SIGKILL",
            "first_self_sigkill": "10-14 10:05:01.260",
        })
        self.assertEqual(x["status"], "HOST_AND_GUEST_PID_EXIT_EVIDENCE_INCOMPLETE")

    def test_no_original_signal_rejected(self):
        result = inspect(self.proc, 7100, 2211)
        x = crosscheck(result, {
            "initial_activity_pid": 2211,
            "classification": "INITIAL_ACTIVITY_NO_SELF_SIGKILL_OBSERVED",
            "first_self_sigkill": None,
        })
        self.assertEqual(x["status"], "HOST_AND_GUEST_PID_EXIT_EVIDENCE_INCOMPLETE")

    def test_invalid_pid_type_rejected(self):
        result = inspect(self.proc, 7100, 2211)
        x = crosscheck(result, {
            "initial_activity_pid": True,
            "classification": "ORIGINAL_ACTIVITY_SELF_SIGKILL",
            "first_self_sigkill": "10-14 10:05:01.260",
        })
        self.assertEqual(x["status"], "HOST_AND_GUEST_PID_EXIT_EVIDENCE_INCOMPLETE")


if __name__ == "__main__":
    unittest.main()
