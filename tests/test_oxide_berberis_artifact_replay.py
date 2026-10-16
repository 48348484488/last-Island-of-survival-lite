"""Offline, synthetic regression for the unchanged Oxide x86_64 logcat replay.

The original GitHub Actions evidence is separately replayed by the lightweight
workflow; these fixtures never claim to be a live Android execution.
"""
import unittest

from tools.oxide_berberis_artifact_replay import verify_existing_logcat

ORIGINAL_LIKE = "\n".join((
    "10-15 22:46:40.128   541  1109 I ActivityTaskManager: START u0 {cmp=com.catsbit.oxidesurvivalisland/.OxideActivity} from uid 2000",
    "10-15 22:46:40.465   541   575 I ActivityManager: Start proc 4426:com.catsbit.oxidesurvivalisland/u0a209 for next-top-activity",
    "10-15 22:46:40.992  4426  4426 I berberis: Initialized Berberis (aarch64), version 0.2.3",
    "10-15 22:46:43.180  4426  4426 E berberis: Undefined instruction 0xd5380429 at 0x0000737d96904a3c",
    "10-15 22:46:43.186  4426  4426 F libc    : Fatal signal 4 (SIGILL), code -6 (SI_TKILL) in tid 4426 (esurvivalisland), pid 4426 (esurvivalisland)",
    "10-15 22:46:44.166  4495  4495 F DEBUG   : pid: 4426, tid: 4426, name: esurvivalisland  >>> com.catsbit.oxidesurvivalisland <<<",
    "10-15 22:46:44.186  4495  4495 F DEBUG   :       #00 pc 0000000000700a3c  /data/app/synthetic/lib/arm64/libloader.so",
    "10-15 22:46:44.186  4495  4495 F DEBUG   :       #02 pc 000000000008b144  /system/bin/arm64/linker64 (__dl__ZN6soinfo17call_constructorsEv+604)",
    "10-15 22:46:44.327   350   350 I Zygote  : Process 4426 exited due to signal 4 (Illegal instruction)",
))


class OriginalBerberisArtifactReplayTests(unittest.TestCase):
    def read(self, text=ORIGINAL_LIKE):
        return verify_existing_logcat(text.encode("utf-8"))

    def test_matching_original_like_evidence_is_accepted_with_limits(self):
        result = self.read()
        self.assertEqual(result["status"], "ORIGINAL_BERBERIS_OPCODE_SIGILL_EVIDENCE_CONFIRMED")
        self.assertTrue(result["all_checks_passed"])
        self.assertEqual(result["observed_signal_delta_ms"], 6)
        self.assertEqual(result["original_pid"], 4426)
        self.assertEqual(result["reported_opcode"], "0xd5380429")
        self.assertFalse(result["original_gameplay_verified"])
        self.assertEqual(result["security_trigger"], "UNKNOWN")
        self.assertEqual(result["native_arm64_compatibility"], "NOT_ESTABLISHED")

    def test_wrong_instruction_address_fails_closed(self):
        result = self.read(ORIGINAL_LIKE.replace("0x0000737d96904a3c", "0x0000737d96904a40"))
        self.assertFalse(result["checks"]["address"])
        self.assertFalse(result["all_checks_passed"])

    def test_wrong_opcode_fails_closed(self):
        result = self.read(ORIGINAL_LIKE.replace("0xd5380429", "0xd503201f"))
        self.assertFalse(result["checks"]["opcode"])
        self.assertFalse(result["all_checks_passed"])

    def test_other_pid_for_undefined_instruction_fails_closed(self):
        result = self.read(ORIGINAL_LIKE.replace("4426  4426 E berberis:", "9999  9999 E berberis:"))
        self.assertFalse(result["all_checks_passed"])

    def test_missing_loader_constructors_fails_closed(self):
        result = self.read(ORIGINAL_LIKE.replace("call_constructors", "other_method"))
        self.assertFalse(result["checks"]["constructor_stack"])
        self.assertFalse(result["all_checks_passed"])

    def test_wrong_start_to_signal_fails_closed(self):
        result = self.read(ORIGINAL_LIKE.replace("22:46:40.128", "22:46:39.128"))
        self.assertFalse(result["checks"]["launch_to_signal"])
        self.assertFalse(result["all_checks_passed"])

    def test_missing_signal_fails_closed(self):
        result = self.read("\n".join(line for line in ORIGINAL_LIKE.splitlines()
                          if "Fatal signal 4" not in line))
        self.assertFalse(result["all_checks_passed"])


if __name__ == "__main__":
    unittest.main()
