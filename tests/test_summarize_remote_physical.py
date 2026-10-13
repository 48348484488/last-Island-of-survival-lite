import tempfile
import json
import unittest
import sys
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from summarize_remote_physical import summarize

class ReportSafetyTests(unittest.TestCase):
    def test_no_raw_log_or_device_identifiers_leak(self):
        with tempfile.TemporaryDirectory() as d:
            f=Path(d)
            (f/"activity_verdict.json").write_text(json.dumps({
                "status":"INITIAL_ACTIVITY_TERMINATED","initial_activity_pid":9876}))
            (f/"forensic_summary.json").write_text(json.dumps({
                "classification":"ORIGINAL_ACTIVITY_SELF_SIGKILL",
                "evidence":{"start":"SECRET_DEVICE_SERIAL"}}))
            (f/"environment_inventory.json").write_text(json.dumps({
                "build_properties":{"ro.product.model":"SECRET"},
                "google_package_presence":{"play_services":"PRESENT"}}))
            r=summarize(f,1)
            self.assertEqual(r["original_activity_state"],"INITIAL_ACTIVITY_TERMINATED")
            self.assertNotIn("SECRET",json.dumps(r))
            self.assertNotIn("9876",json.dumps(r))
            self.assertFalse(r["real_players_esp_verified"])
    def test_missing_evidence_is_not_misreported_as_running_game(self):
        with tempfile.TemporaryDirectory() as d:
            r=summarize(d,69)
            self.assertEqual(r["original_activity_state"],"NOT_CLASSIFIED")
            self.assertFalse(r["gameplay_verified"])
if __name__=="__main__":
    unittest.main()
