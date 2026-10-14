import importlib.util
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
def load(name, path):
    spec = importlib.util.spec_from_file_location(name, ROOT / "tools" / path)
    obj = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(obj)
    return obj

m = load("sigkill", "correlate_kernel_sigkill.py")
TRACE = """
Attaching 3 probes...
KERNEL_SIGKILL syscall=kill host_sender=12895 host_tid=12895 target_pid=2232 signal=9 comm=esurvivalisland\\n
   kill+8
KERNEL_SIGKILL syscall=kill host_sender=9883 host_tid=9898 target_pid=2232 signal=9 comm=binder:223_2\\n
KERNEL_SIGKILL syscall=kill host_sender=9883 host_tid=9913 target_pid=-2232 signal=9 comm=ActivityManager\\n
"""
FORENSIC = dict(initial_activity_pid=2232, classification="ORIGINAL_ACTIVITY_SELF_SIGKILL",
                first_self_sigkill="10-14 16:37:45.146")
MAP = dict(samples=[
  dict(host_pid=12895, guest_pid=2232, name="esurvivalisland", nspid=[12895,2232]),
  dict(host_pid=9883, guest_pid=223, name="system_server", nspid=[9883,223])])

class KernelTest(unittest.TestCase):
    def test_unmapped_fails_closed(self):
        r = m.audit(TRACE, FORENSIC)
        self.assertEqual(r["status"], "GAME_SELF_SIGKILL_LOGCAT_ONLY")
        self.assertFalse(r["host_guest_pid_mapping_confirmed"])
        self.assertEqual(r["captured_kernel_sigkill_event_count"],3)

    def test_real_mapping(self):
        r = m.audit(TRACE, FORENSIC, MAP)
        self.assertEqual(r["status"], "GAME_SELF_SIGNAL_HOST_PID_CORROBORATED")
        self.assertEqual(r["game_self_kill_kernel_event_count"], 1)
        self.assertEqual(r["events"][1]["classification"], "OTHER_PROCESS")
        self.assertFalse(r["specific_security_cause_proven"])
        self.assertFalse(r["gameplay_verified"])

    def test_wrong_game_namespace(self):
        bad = dict(samples=[dict(host_pid=12895, guest_pid=1222,
                                 name="esurvivalisland", nspid=[12895,1222])])
        self.assertFalse(m.audit(TRACE, FORENSIC, bad)["host_guest_pid_mapping_confirmed"])

    def test_pid_reuse_detected(self):
        rows = MAP["samples"] + [dict(host_pid=12895,guest_pid=11,name="other",nspid=[12895,11])]
        r = m.audit(TRACE, FORENSIC, dict(samples=rows))
        self.assertIn(12895, r["host_pid_mapping_ambiguous"])
        self.assertFalse(r["host_guest_pid_mapping_confirmed"])

    def test_without_logcat_no_causes(self):
        r = m.audit(TRACE, {"initial_activity_pid":2232}, MAP)
        self.assertEqual(r["status"], "GAME_SELF_SIGKILL_NOT_CONFIRMED")

    def test_tgkill(self):
        s="KERNEL_SIGKILL syscall=tgkill host_sender=12895 host_tid=12896 target_tgid=2232 target_tid=2233 signal=9 comm=game\\n"
        self.assertEqual(m.extract_events(s)[0]["target_tid"],2233)


    def test_truncated_kernel_event_fails_closed(self):
        broken = "KERNEL_SIGKILL syscall=kill host_sender=12895 host_tid=12895 target_pid="
        self.assertEqual(m.extract_events(broken), [])
        self.assertEqual(m.audit(broken, FORENSIC)["game_self_kill_kernel_event_count"], 0)

    def test_whitespace_and_unrelated_process(self):
        unrelated = "KERNEL_SIGKILL   syscall=kill   host_sender=100   host_tid=100   target_pid=200 signal=9 comm=init\\n"
        self.assertEqual(len(m.extract_events(unrelated)), 1)
        self.assertEqual(m.audit(unrelated, FORENSIC, MAP)["game_self_kill_kernel_event_count"], 0)

    def test_malformed_namespace_chain_is_ignored(self):
        invalid = dict(samples=[dict(host_pid=12895, guest_pid=2232,
                                     nspid=[12895,88], name="esurvivalisland")])
        self.assertFalse(m.audit(TRACE, FORENSIC, invalid)["host_guest_pid_mapping_confirmed"])

    def test_no_activity_pid_is_not_guessed(self):
        result = m.audit(TRACE, {"classification":"UNKNOWN"}, MAP)
        self.assertIsNone(result["game_activity_guest_pid"])
        self.assertFalse(result["host_guest_pid_mapping_confirmed"])


if __name__ == "__main__":
    unittest.main()
