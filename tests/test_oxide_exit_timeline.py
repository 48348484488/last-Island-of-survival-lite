"""Deterministic replays: synthetic excerpts reflecting real Android 15/16 timelines."""
import unittest
from tools.oxide_exit_timeline import assess, ACTIVITY

def fixture(start="18:55:51.207", draw="18:55:54.292", close="18:55:55.684",
            kill="18:55:55.687", death="18:55:55.728", proc=2023):
    return "\n".join([
        f"10-15 {start}   281  1748 I ActivityTaskManager: START u0 {{cmp={ACTIVITY}}} from uid 2000",
        f"10-15 18:55:51.215   281   314 I ActivityManager: Start proc {proc}:com.catsbit.oxidesurvivalisland/u0a87 for next-top-activity",
        "10-15 18:55:51.295  2023  2071 W GooglePlayServicesUtil: com.catsbit.oxidesurvivalisland requires the Google Play Store, but it is missing.",
        f"10-15 {draw}   281   302 I ActivityTaskManager: Fully drawn {ACTIVITY}: +3s91ms",
        f"10-15 {kill}  {proc}  {proc} I Process : Sending signal. PID: {proc} SIG: 9",
        f"10-15 {death}   281  1748 I ActivityManager: Process com.catsbit.oxidesurvivalisland (pid {proc}) has died: fg  TOP",
        f"10-15 18:55:55.871   281   302 V WindowManager: Sent Transition (#6) createdAt=10-15 {close} via request=TransitionRequestInfo {{ type = CLOSE, triggerTask = TaskInfo{{taskId=8 baseIntent=Intent {{ cmp={ACTIVITY} }} }} }}",
    ])

class ExitTimelineTests(unittest.TestCase):
    def test_android15(self):
        r = assess(fixture())
        self.assertEqual(r["status"], "TASK_CLOSE_PRECEDES_ORIGINAL_SELF_SIGKILL")
        self.assertEqual(r["evidence"]["start_to_self_sigkill_ms"], 4480)
        self.assertEqual(r["evidence"]["task_close_before_self_sigkill_ms"], 3)
        self.assertFalse(r["gameplay_verified"])
        self.assertEqual(r["security_trigger"], "UNKNOWN")

    def test_android16(self):
        r = assess(fixture(start="18:38:25.470", draw="18:38:28.642",
                           close="18:38:30.137", kill="18:38:30.140",
                           death="18:38:30.182", proc=2108))
        self.assertEqual(r["evidence"]["start_to_self_sigkill_ms"], 4670)
        self.assertEqual(r["evidence"]["task_close_before_self_sigkill_ms"], 3)

    def test_unrelated_sender_rejected(self):
        log = fixture().replace("2023  2023 I Process : Sending signal. PID: 2023",
                                "2222  2222 I Process : Sending signal. PID: 2023")
        self.assertEqual(assess(log)["status"], "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE")

    def test_other_app_close_rejected(self):
        log = fixture().replace("cmp=" + ACTIVITY + " } } }",
                                "cmp=com.example.other/.Activity } } }")
        self.assertEqual(assess(log)["status"], "ORIGINAL_SELF_SIGKILL_NO_MATCHED_CLOSE")

    def test_stale_close_rejected(self):
        self.assertEqual(assess(fixture(close="18:55:51.684"))["status"],
                         "ORIGINAL_SELF_SIGKILL_NO_MATCHED_CLOSE")

    def test_close_after_signal_rejected(self):
        self.assertEqual(assess(fixture(close="18:55:55.690"))["status"],
                         "ORIGINAL_SELF_SIGKILL_NO_MATCHED_CLOSE")

    def test_other_process_death_rejected(self):
        log = fixture().replace("(pid 2023) has died", "(pid 9999) has died")
        self.assertEqual(assess(log)["status"], "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE")

    def test_missing_records_fail_closed(self):
        r = assess("")
        self.assertEqual(r["status"], "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE")
        self.assertIsNone(r["evidence"]["original_pid"])

    def test_app_pid_permission_and_fingerprint_warnings_are_not_security_verdicts(self):
        warnings = "\n".join((
            '10-15 18:55:51.531  2023  2163 W System.err: Caused by: java.lang.SecurityException: Need to declare android.permission.REQUEST_INSTALL_PACKAGES to call this api',
            '10-15 18:55:51.561  2023  2158 E libc    : The property "ro.build.fingerprint" has a value with length 96 that is too large for __system_property_get()/__system_property_read(); use __system_property_read_callback() instead.',
        ))
        result = assess(fixture() + "\n" + warnings)
        self.assertTrue(result["evidence"]["undeclared_install_packages_permission_exception"])
        self.assertEqual(result["evidence"]["oversized_fingerprint_read_observed_length"], 96)
        self.assertEqual(result["security_trigger"], "UNKNOWN")
        self.assertEqual(result["play_integrity_verdict"], "NOT_OBSERVED")
        self.assertFalse(result["gameplay_verified"])

    def test_other_pid_warnings_are_not_attributed_to_game(self):
        warnings = "\n".join((
            '10-15 18:55:51.531  9999  9999 W System.err: Caused by: java.lang.SecurityException: Need to declare android.permission.REQUEST_INSTALL_PACKAGES to call this api',
            '10-15 18:55:51.561  9999  9999 E libc    : The property "ro.build.fingerprint" has a value with length 96 that is too large for __system_property_get()/__system_property_read(); use __system_property_read_callback() instead.',
        ))
        evidence = assess(fixture() + "\n" + warnings)["evidence"]
        self.assertFalse(evidence["undeclared_install_packages_permission_exception"])
        self.assertIsNone(evidence["oversized_fingerprint_read_observed_length"])

    def test_realistic_native_bridge_loader_sigill_not_called_unsupported_instruction(self):
        log = "\n".join((
            f"10-15 22:46:40.128   541  1109 I ActivityTaskManager: START u0 {{cmp={ACTIVITY}}} from uid 2000",
            "10-15 22:46:40.465   541   575 I ActivityManager: Start proc 4426:com.catsbit.oxidesurvivalisland/u0a209 for next-top-activity",
            "10-15 22:46:40.992  4426  4426 I berberis: Initialized Berberis (aarch64), version 0.2.3",
            "10-15 22:46:43.180  4426  4426 E berberis: Undefined instruction 0xd5380429 at 0x0000737d96904a3c",
            "10-15 22:46:43.186  4426  4426 F libc    : Fatal signal 4 (SIGILL), code -6 (SI_TKILL) in tid 4426 (esurvivalisland), pid 4426 (esurvivalisland)",
            "10-15 22:46:44.166  4495  4495 F DEBUG   : pid: 4426, tid: 4426, name: esurvivalisland  >>> com.catsbit.oxidesurvivalisland <<<",
            "10-15 22:46:44.166  4495  4495 F DEBUG   : signal 4 (SIGILL), code -6 (SI_TKILL), fault addr --------",
            "10-15 22:46:44.186  4495  4495 F DEBUG   :       #00 pc 0000000000700a3c  /data/app/synthetic/lib/arm64/libloader.so",
            "10-15 22:46:44.186  4495  4495 F DEBUG   :       #02 pc 000000000008b144  /system/bin/arm64/linker64 (__dl__ZN6soinfo17call_constructorsEv+604)",
            "10-15 22:46:44.327   350   350 I Zygote  : Process 4426 exited due to signal 4 (Illegal instruction)",
            "10-15 22:46:44.327   541   628 I ActivityManager: Process com.catsbit.oxidesurvivalisland (pid 4426) has died: fg TOP",
        ))
        result = assess(log)
        self.assertEqual(result["status"], "ORIGINAL_SIGILL_DURING_ARM64_LIBLOADER_CONSTRUCTORS")
        e = result["evidence"]
        self.assertEqual(e["original_pid"], 4426)
        self.assertTrue(e["original_fatal_sigill_observed"])
        self.assertEqual(e["original_fatal_sigill_code"], -6)
        self.assertEqual(e["original_fatal_sigill_origin"], "SI_TKILL")
        self.assertEqual(e["start_to_sigill_ms"], 3058)
        self.assertTrue(e["original_arm64_libloader_guest_frame"])
        self.assertTrue(e["arm64_dynamic_linker_constructors_frame"])
        self.assertTrue(e["original_zygote_signal4_exit_observed"])
        self.assertTrue(e["original_berberis_undefined_instruction_precedes_sigill"])
        self.assertEqual(e["original_berberis_reported_opcode"], "0xd5380429")
        self.assertEqual(e["original_berberis_to_sigill_ms"], 6)
        self.assertFalse(result["gameplay_verified"])
        self.assertEqual(result["security_trigger"], "UNKNOWN")

    def test_sigill_from_another_pid_does_not_count_as_original(self):
        log = "\n".join((
            f"10-15 22:46:40.128 541 1109 I ActivityTaskManager: START u0 {{cmp={ACTIVITY}}}",
            "10-15 22:46:40.465 541 575 I ActivityManager: Start proc 4426:com.catsbit.oxidesurvivalisland/u0a209",
            "10-15 22:46:43.186 5555 5555 F libc: Fatal signal 4 (SIGILL), code -6 (SI_TKILL) in tid 5555 (other), pid 5555 (other)",
            "10-15 22:46:44.327 350 350 I Zygote: Process 5555 exited due to signal 4 (Illegal instruction)",
        ))
        result = assess(log)
        self.assertEqual(result["status"], "INSUFFICIENT_ORIGINAL_EXIT_EVIDENCE")
        self.assertFalse(result["evidence"]["original_fatal_sigill_observed"])
        self.assertEqual(result["evidence"]["original_pid"], 4426)

    def test_sigill_without_native_stack_must_not_blame_libloader(self):
        log = "\n".join((
            f"10-15 22:46:40.128 541 1109 I ActivityTaskManager: START u0 {{cmp={ACTIVITY}}}",
            "10-15 22:46:40.465 541 575 I ActivityManager: Start proc 4426:com.catsbit.oxidesurvivalisland/u0a209",
            "10-15 22:46:43.186 4426 4426 F libc: Fatal signal 4 (SIGILL), code -6 (SI_TKILL) in tid 4426 (original), pid 4426 (original)",
            "10-15 22:46:44.327 350 350 I Zygote: Process 4426 exited due to signal 4 (Illegal instruction)",
        ))
        result = assess(log)
        self.assertEqual(result["status"], "ORIGINAL_SIGILL_NATIVE_BRIDGE_CAUSE_UNKNOWN")
        self.assertFalse(result["evidence"]["original_arm64_libloader_guest_frame"])
        self.assertFalse(result["evidence"]["original_berberis_undefined_instruction_precedes_sigill"])

    def test_invalid_year(self):
        with self.assertRaises(ValueError):
            assess(fixture(), 1999)

if __name__ == "__main__":
    unittest.main()
