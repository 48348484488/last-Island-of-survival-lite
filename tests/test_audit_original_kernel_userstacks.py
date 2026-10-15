import sys
import unittest
from pathlib import Path

sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from audit_original_kernel_userstacks import analyze, resolve_frame, mappings
from test_audit_controlled_activity_finish_node import Tests as OriginalControlFixture


BINDER = (
    "OXIDE_USERSTACK_BEGIN kind=binder host_sender=12335 host_tid=12335 "
    "transaction=901 target_node=777 code=15 boot_ns=14997000000\n"
    "0x400100\n0x5000a4\nOXIDE_USERSTACK_END kind=binder\n"
)
KILL = (
    "OXIDE_USERSTACK_BEGIN kind=kill host_sender=12335 host_tid=12335 "
    "target_pid=2120 signal=9 boot_ns=15000000000\n"
    "0x500120\n0x400200\nOXIDE_USERSTACK_END kind=kill\n"
)
MAPS = (
    "00400000-00410000 r-xp 00000000 08:01 123 /apex/lib64/libc.so\n"
    "00500000-00590000 r-xp 00008000 08:01 456 /data/app/base.apk!/lib/arm64-v8a/libunity.so\n"
    "00600000-00601000 rw-p 00000000 00:00 0 [anon:unrelated]\n"
)


class RealOriginalUserspaceCallsiteTests(unittest.TestCase):
    def fixture(self):
        root=OriginalControlFixture.fixture(self)
        with (root/"kernel_sigkill_trace.txt").open("a") as writer:
            writer.write(BINDER+KILL)
        (root/"original_host_exec_maps.txt").write_text(MAPS)
        return root

    def mutate(self,root,original,replacement,filename="kernel_sigkill_trace.txt"):
        f=root/filename
        self.assertIn(original,f.read_text())
        f.write_text(f.read_text().replace(original,replacement))

    def test_same_original_pid_binder_event_and_kernel_sigkill_mapped(self):
        result=analyze(self.fixture())
        self.assertEqual(result["status"],
                         "ORIGINAL_BINDER_AND_SIGKILL_CALLSITE_MODULES_CORRELATED")
        self.assertEqual(result["binder_stack_events_attributed"],1)
        self.assertEqual(result["sigkill_stack_events_attributed"],1)
        self.assertTrue(result["both_stacks_contain_mapped_executable_pc"])
        self.assertEqual(result["attributed_stacks"][0]["frames"][0]["file_offset_hex"],"0x100")
        self.assertEqual(result["attributed_stacks"][0]["frames"][1]["file_offset_hex"],"0x80a4")
        self.assertFalse(result["source_of_security_decision_identified"])
        self.assertFalse(result["original_apks_modified"])

    def test_wrong_binder_node_not_attributed(self):
        x=self.fixture()
        self.mutate(x,"transaction=901 target_node=777 code=15 boot_ns=14997000000",
                    "transaction=901 target_node=999 code=15 boot_ns=14997000000")
        self.assertEqual(analyze(x)["binder_stack_events_attributed"],0)

    def test_wrong_binder_transaction_not_attributed(self):
        x=self.fixture()
        self.mutate(x,"transaction=901 target_node=777 code=15 boot_ns=14997000000",
                    "transaction=991 target_node=777 code=15 boot_ns=14997000000")
        self.assertEqual(analyze(x)["binder_stack_events_attributed"],0)

    def test_wrong_binder_thread_pid_not_attributed(self):
        x=self.fixture()
        self.mutate(x,"kind=binder host_sender=12335","kind=binder host_sender=8888")
        self.assertEqual(analyze(x)["binder_stack_events_attributed"],0)

    def test_wrong_kill_target_not_attributed(self):
        x=self.fixture()
        self.mutate(x,"target_pid=2120 signal=9 boot_ns=15000000000",
                    "target_pid=2121 signal=9 boot_ns=15000000000")
        self.assertEqual(analyze(x)["sigkill_stack_events_attributed"],0)

    def test_wrong_kill_kernel_clock_not_attributed(self):
        x=self.fixture()
        self.mutate(x,"kind=kill host_sender=12335 host_tid=12335 target_pid=2120 signal=9 boot_ns=15000000000",
                    "kind=kill host_sender=12335 host_tid=12335 target_pid=2120 signal=9 boot_ns=15100000000")
        self.assertEqual(analyze(x)["sigkill_stack_events_attributed"],0)

    def test_zero_frames_rejected(self):
        x=self.fixture()
        self.mutate(x,"0x400100\n0x5000a4","0x0\n0x0")
        self.assertFalse(analyze(x)["both_stacks_contain_mapped_executable_pc"])

    def test_unmapped_user_pcs_rejected(self):
        x=self.fixture()
        (x/"original_host_exec_maps.txt").write_text(
            "01000000-01010000 r-xp 00000000 08:01 456 /system/other.so\n")
        self.assertEqual(analyze(x)["status"],"ORIGINAL_USERSPACE_STACK_INCOMPLETE_OR_UNMAPPED")

    def test_missing_host_executable_maps_fails_closed(self):
        x=self.fixture()
        (x/"original_host_exec_maps.txt").unlink()
        self.assertFalse(analyze(x)["both_stacks_contain_mapped_executable_pc"])

    def test_original_control_not_proven_fails_closed(self):
        x=self.fixture()
        self.mutate(x,"transaction=901 target_node=777 code=15 reply=0",
                    "transaction=901 target_node=123 code=15 reply=0")
        self.assertFalse(analyze(x)["real_original_pid_and_kernel_binder_control_verified"])

    def test_nonexecutable_map_cannot_resolve_pc(self):
        x=self.fixture()
        self.mutate(x,"r-xp 00008000","rw-p 00008000",filename="original_host_exec_maps.txt")
        result=analyze(x)
        self.assertEqual(result["attributed_stacks"][0]["frames"][1]["mapped_image"],None)

    def test_unrelated_map_file_offsets_are_not_guessed(self):
        x=self.fixture()
        result=resolve_frame(0x700100,mappings(x/"original_host_exec_maps.txt"))
        self.assertFalse(result["executable_mapping"])
        self.assertIsNone(result["file_offset_hex"])

    def test_missing_binder_stack_is_not_success(self):
        x=self.fixture()
        self.mutate(x,BINDER,"")
        self.assertEqual(analyze(x)["status"],"ORIGINAL_USERSPACE_STACK_INCOMPLETE_OR_UNMAPPED")


if __name__=="__main__":
    unittest.main()
