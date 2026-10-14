import json
import sys
import tempfile
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from audit_controlled_activity_finish_node import assess

PKG="com.catsbit.oxidesurvivalisland"
COMMON={"status":"ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN","pid_namespace":"pid:[4122]"}
class Tests(unittest.TestCase):
    def fixture(self):
        tmp=tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        r=Path(tmp.name)
        (r/"host_pid_namespace.json").write_text(json.dumps({
            **COMMON,"host_pid":12335,"guest_pid":2120,"nspid_chain":[12335,2120],
            "original_process_cmdline":PKG,"host_start_ticks":1000,"observed_host_boot_ns":10000000000
        }))
        (r/"host_system_server_namespace.json").write_text(json.dumps({
            **COMMON,"host_pid":10423,"guest_pid":321,"nspid_chain":[10423,321],
            "original_process_cmdline":"system_server"
        }))
        (r/"host_settings_namespace.json").write_text(json.dumps({
            **COMMON,"host_pid":12500,"guest_pid":2250,"nspid_chain":[12500,2250],
            "original_process_cmdline":"com.android.settings"
        }))
        (r/"settings_back_window.json").write_text(json.dumps({
            "before_host_boot_ns":11100000000,"after_host_boot_ns":11300000000,
            "input_keyevent_exit_code":0
        }))
        (r/"forensic_summary.json").write_text(json.dumps({
            "initial_activity_pid":2120,
            "classification":"ORIGINAL_ACTIVITY_SELF_SIGKILL",
            "activity_finish":"10-14 22:05:50.371",
            "first_self_sigkill":"10-14 22:05:50.373"
        }))
        (r/"exit_info.txt").write_text("ApplicationExitInfo #0:\n "
           "pid=2120 process="+PKG+" reason=2 (SIGNALED) status=9\n")
        (r/"all_buffers.txt").write_text(
            "10-14 22:05:40.100 321 300 I wm_finish_activity: "
            "[0,15,7,com.android.settings/.Settings,finish-activity]\n"
        )
        (r/"kernel_sigkill_trace.txt").write_text(
            "ANDROID_BINDER host_sender=12500 host_tid=12500 dest_proc=10423 dest_thread=0 "
            "transaction=501 target_node=777 code=15 reply=0 flags=18 comm=Settings boot_ns=11180000000\n"
            "ANDROID_BINDER host_sender=12335 host_tid=12335 dest_proc=10423 dest_thread=0 "
            "transaction=901 target_node=777 code=15 reply=0 flags=18 comm=esurvivalisland boot_ns=14997000000\n"
            "KERNEL_SIGKILL syscall=kill host_sender=12335 host_tid=12335 "
            "target_pid=2120 signal=9 comm=esurvivalisland boot_ns=15000000000\n"
        )
        return r
    def test_same_node_with_android_settings_control(self):
        x=assess(self.fixture())
        self.assertEqual(x["status"],"CONTROLLED_SETTINGS_FINISH_BINDER_NODE_MATCHES_ORIGINAL_FINAL_CODE15")
        self.assertTrue(x["shared_binder_endpoint_node_correlated"])
        self.assertEqual(x["original_last20ms_code15_events"][0]["milliseconds_before_game_sigkill"],3.0)
        self.assertFalse(x["interface_descriptor_read_from_transaction"])
        self.assertFalse(x["reason_for_activity_finish"]!="UNKNOWN")

    def test_different_binder_node_rejected(self):
        r=self.fixture();p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("transaction=901 target_node=777","transaction=901 target_node=900"))
        self.assertFalse(assess(r)["shared_binder_endpoint_node_correlated"])

    def test_missing_settings_finish_log_rejected(self):
        r=self.fixture();(r/"all_buffers.txt").write_text("")
        self.assertFalse(assess(r)["shared_binder_endpoint_node_correlated"])

    def test_wrong_settings_host_process_rejected(self):
        r=self.fixture();p=r/"host_settings_namespace.json"
        d=json.loads(p.read_text());d["original_process_cmdline"]="com.android.launcher"
        p.write_text(json.dumps(d))
        self.assertFalse(assess(r)["shared_binder_endpoint_node_correlated"])

    def test_settings_code_after_calibration_window_rejected(self):
        r=self.fixture();p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("boot_ns=11180000000","boot_ns=16000000000"))
        self.assertFalse(assess(r)["shared_binder_endpoint_node_correlated"])

    def test_original_code_too_early_rejected(self):
        r=self.fixture();p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("boot_ns=14997000000","boot_ns=14700000000"))
        self.assertFalse(assess(r)["shared_binder_endpoint_node_correlated"])

    def test_wrong_interface_code_rejected(self):
        r=self.fixture();p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text().replace("target_node=777 code=15 reply=0 flags=18 comm=esurvivalisland",
             "target_node=777 code=20 reply=0 flags=18 comm=esurvivalisland"))
        self.assertFalse(assess(r)["shared_binder_endpoint_node_correlated"])

    def test_multiple_controller_nodes_ambiguous(self):
        r=self.fixture();p=r/"kernel_sigkill_trace.txt"
        p.write_text(p.read_text()+
        "ANDROID_BINDER host_sender=12500 host_tid=12500 dest_proc=10423 dest_thread=0 "
        "transaction=502 target_node=555 code=15 reply=0 flags=18 comm=Settings boot_ns=11181000000\n")
        self.assertFalse(assess(r)["shared_binder_endpoint_node_correlated"])

if __name__=="__main__":
    unittest.main()
