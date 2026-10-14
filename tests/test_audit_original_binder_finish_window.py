import json
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from audit_original_binder_finish_window import inspect

GAME = "com.catsbit.oxidesurvivalisland"

class Tests(unittest.TestCase):
    def fixture(self):
        t = tempfile.TemporaryDirectory()
        self.addCleanup(t.cleanup)
        root = Path(t.name)
        common = {"status":"ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN",
                  "pid_namespace":"pid:[4024242]"}
        (root/"host_pid_namespace.json").write_text(json.dumps({
            **common,"host_pid":12351,"guest_pid":2120,
            "nspid_chain":[12351,2120],"original_process_cmdline":GAME,
            "host_start_ticks":12500,"observed_host_boot_ns":10000000000
        }))
        (root/"host_system_server_namespace.json").write_text(json.dumps({
            **common,"host_pid":930,"guest_pid":321,
            "nspid_chain":[930,321],"original_process_cmdline":"system_server",
            "host_start_ticks":700
        }))
        (root/"forensic_summary.json").write_text(json.dumps({
            "initial_activity_pid":2120,
            "classification":"ORIGINAL_ACTIVITY_SELF_SIGKILL",
            "activity_finish":"10-14 19:02:11.600",
            "first_self_sigkill":"10-14 19:02:11.602"
        }))
        (root/"exit_info.txt").write_text(
            "ApplicationExitInfo #0:\n pid=2120 process="+GAME+
            " reason=2 (SIGNALED) status=9\n")
        (root/"kernel_sigkill_trace.txt").write_text(
            "ANDROID_BINDER host_sender=12351 host_tid=12351 dest_proc=930 dest_thread=0 "
            "transaction=1234 code=27 reply=0 flags=0 comm=esurvivalisland boot_ns=14000000000\n"
            "KERNEL_SIGKILL syscall=kill host_sender=12351 host_tid=12351 "
            "target_pid=2120 signal=9 comm=esurvivalisland boot_ns=14500000000\n"
        )
        return root

    def test_original_binder_sender_and_server_target_matching(self):
        x=inspect(self.fixture())
        self.assertEqual(x["status"],"ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_OBSERVED")
        self.assertEqual(x["original_process_to_system_server_events_before_sigkill"],1)
        self.assertEqual(x["events"][0]["milliseconds_before_game_self_sigkill"],500)
        self.assertEqual(x["events"][0]["numeric_transaction_code"],27)
        self.assertFalse(x["events"][0]["is_finish_activity_call_proven"])
        self.assertFalse(x["activity_finish_requester_identified"])

    def test_wrong_sender_not_game(self):
        r=self.fixture()
        p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("ANDROID_BINDER host_sender=12351","ANDROID_BINDER host_sender=8888"))
        self.assertEqual(inspect(r)["status"],"NO_ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_CAPTURED")

    def test_wrong_system_server_namespace(self):
        r=self.fixture()
        p=r/"host_system_server_namespace.json"
        x=json.loads(p.read_text());x["pid_namespace"]="pid:[333]"
        p.write_text(json.dumps(x))
        self.assertEqual(inspect(r)["status"],"HOST_OR_GUEST_PID_ORIGINAL_KERNEL_PROOF_MISSING")

    def test_unmapped_server_fails(self):
        r=self.fixture()
        p=r/"host_system_server_namespace.json"
        x=json.loads(p.read_text());x["nspid_chain"]=[930,33]
        p.write_text(json.dumps(x))
        self.assertEqual(inspect(r)["status"],"HOST_OR_GUEST_PID_ORIGINAL_KERNEL_PROOF_MISSING")

    def test_wrong_target_not_system_server(self):
        r=self.fixture()
        p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("dest_proc=930","dest_proc=950"))
        self.assertEqual(inspect(r)["status"],"NO_ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_CAPTURED")

    def test_event_after_kernel_kill_not_counted(self):
        r=self.fixture()
        p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("boot_ns=14000000000","boot_ns=15000000000"))
        self.assertEqual(inspect(r)["status"],"NO_ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_CAPTURED")

    def test_unrelated_binder_reply_not_counted(self):
        r=self.fixture()
        p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("reply=0 flags=0", "reply=1 flags=0"))
        self.assertEqual(inspect(r)["status"],"NO_ORIGINAL_GAME_TO_SYSTEM_SERVER_BINDER_IPC_CAPTURED")

    def test_no_kernel_evidence_is_not_ipc_proof(self):
        r=self.fixture()
        p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("KERNEL_SIGKILL syscall=kill","OTHER syscall=kill"))
        self.assertEqual(inspect(r)["status"],"HOST_OR_GUEST_PID_ORIGINAL_KERNEL_PROOF_MISSING")

    def test_kernel_clock_missing_does_not_pass(self):
        r=self.fixture()
        p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("boot_ns=14500000000","boot_ns=notrecorded"))
        self.assertEqual(inspect(r)["status"],"HOST_OR_GUEST_PID_ORIGINAL_KERNEL_PROOF_MISSING")

    def test_numeric_code_does_not_prove_finish_method(self):
        x=inspect(self.fixture())
        self.assertFalse(x["interface_descriptor_identified"])
        self.assertFalse(x["root_cause_identified"])
        self.assertFalse(x["playable_gameplay_verified"])

if __name__ == "__main__":
    unittest.main()
