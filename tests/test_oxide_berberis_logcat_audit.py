"""Offline regression for original Oxide Berberis SIGILL chronology (no APK needed)."""
import re
import unittest

from tools.oxide_berberis_logcat_audit import audit_logcat

SAMPLE = """10-15 22:46:43.180  4426  4426 E berberis: Undefined instruction 0xd5380429 at 0x0000737d96904a3c
10-15 22:46:43.186  4426  4426 F libc    : Fatal signal 4 (SIGILL), code -6 (SI_TKILL) in tid 4426
"""


class BerberisLogcatAuditTests(unittest.TestCase):
    def test_original_pid_opcode_and_six_ms_delta(self):
        result = audit_logcat(SAMPLE)
        self.assertEqual(result["status"], "BERBERIS_UNDEFINED_INSTRUCTION_PRECEDES_SIGILL")
        self.assertEqual(result["pid"], 4426)
        self.assertEqual(result["opcode"], "0xd5380429")
        self.assertEqual(result["delta_ms"], 6)

    def test_unrelated_pid_is_not_correlated(self):
        result = audit_logcat(SAMPLE.replace("in tid 4426", "in tid 5555").replace(" 4426  4426 F", " 5555  5555 F"))
        self.assertNotEqual(result["status"], "BERBERIS_UNDEFINED_INSTRUCTION_PRECEDES_SIGILL")

    def test_missing_signal_is_not_success(self):
        result = audit_logcat(SAMPLE.splitlines()[0])
        self.assertNotEqual(result["status"], "BERBERIS_UNDEFINED_INSTRUCTION_PRECEDES_SIGILL")


if __name__ == "__main__":
    unittest.main()
