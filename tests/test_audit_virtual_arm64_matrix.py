import importlib.util
import unittest
from pathlib import Path

source = Path(__file__).resolve().parents[1] / "tools" / "audit_virtual_arm64_matrix.py"
spec = importlib.util.spec_from_file_location("oxide_matrix", source)
audit = importlib.util.module_from_spec(spec)
spec.loader.exec_module(audit)

def android12():
    return dict(status="ORIGINAL_ACTIVITY_PID_AND_SIGNAL_CONFIRMED",
                activity_snapshot_pid=1829, snapshot_activity_resumed=True,
                main_package_pid_sample_count=31,
                prior_pid_scoped_logcat_self_sigkill=True,
                android_exit_info_sigkill_status9=True,
                multiplayer_gameplay_verified=False)

def forensic(pid):
    return dict(classification="ORIGINAL_ACTIVITY_SELF_SIGKILL",
                initial_activity_pid=pid, initial_activity_start="10-14 16:37:40.166",
                first_self_sigkill="10-14 16:37:45.146",
                evidence={"sigkill": "10-14 16:37:45.146  %d  %d I Process : Sending signal. PID: %d SIG: 9" % (pid,pid,pid)})

class VirtualArm64Matrix(unittest.TestCase):
    def test_three_original_early_exits(self):
        d = audit.summarize(android12(), forensic(2232), forensic(1876))
        self.assertTrue(d["all_three_self_sigkill_confirmed"])
        self.assertEqual([row["original_activity_pid"] for row in d["cases"]], [1829,2232,1876])
        self.assertFalse(d["gameplay_verified"])
        self.assertEqual(d["security_root_cause"], "UNKNOWN")

    def test_android12_unattributed_pid(self):
        a = android12()
        a["snapshot_activity_resumed"] = False
        self.assertFalse(audit.summarize(a, forensic(2232), forensic(1876))["all_three_self_sigkill_confirmed"])

    def test_android12_no_logcat_self_signal(self):
        a = android12()
        a["prior_pid_scoped_logcat_self_sigkill"] = False
        self.assertEqual(audit.check_version("12", a)["classification"], "INSUFFICIENT_EVIDENCE")

    def test_android13_other_emitter(self):
        a = forensic(2232)
        a["evidence"]["sigkill"] = "10-14 16:37:45.146  2604  2604 I Process : Sending signal. PID: 2232 SIG: 9"
        self.assertEqual(audit.check_version("13", a)["classification"], "INSUFFICIENT_EVIDENCE")

    def test_android14_missing_sigkill_timestamp(self):
        a = forensic(1876)
        del a["first_self_sigkill"]
        self.assertEqual(audit.check_version("14", a)["classification"], "INSUFFICIENT_EVIDENCE")

    def test_bool_pid_invalid(self):
        a = forensic(2232)
        a["initial_activity_pid"] = True
        self.assertFalse(audit.check_version("13", a)["checks"]["valid_activity_pid"])

    def test_all_empty_fail_closed(self):
        d = audit.summarize({}, {}, {})
        self.assertFalse(d["all_three_self_sigkill_confirmed"])
        self.assertEqual(len(d["cases"]), 3)

    def test_report_never_claims_playable(self):
        text = audit.markdown(audit.summarize(android12(), forensic(2232), forensic(1876)))
        self.assertIn("NOT PROVEN", text)
        self.assertIn("UNKNOWN", text)
        self.assertIn("38599474610", text)

if __name__ == "__main__":
    unittest.main()
