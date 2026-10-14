import tempfile
import unittest
from pathlib import Path
import sys
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from compare_original_preexit import analyze,compare

APP="com.catsbit.oxidesurvivalisland"
ACT="cTOOT3.abKmV1.tP0Gr1.OxideActivity"
def row(pid,tag,msg,time):
    return f"10-13 {time} {pid} {pid} E {tag}: {msg}"
def start(pid,time):
    return row(224,"am_proc_start",
      f"[0,{pid},10077,{APP},next-top-activity,{{{APP}/{ACT}}}]",time)

class PreExitAudit(unittest.TestCase):
    def analyze(self,logs):
        with tempfile.TemporaryDirectory() as tmp:
            p=Path(tmp)
            (p/"all_buffers.txt").write_text("\n".join(logs)+"\n")
            return analyze(p)
    def test_stale_first_process_is_ignored(self):
        rows=[
            start(2197,"22:35:04.086"),
            row(2197,"libc",'The property "ro.build.fingerprint" has a value with length 111 that is too large',"22:35:04.535"),
            row(2197,"Process","Sending signal. PID: 2197 SIG: 9","22:35:08.981"),
            start(2578,"22:35:15.776"),
            row(2578,"libc",'The property "ro.build.fingerprint" has a value with length 113 that is too large',"22:35:16.211"),
            row(2578,"GooglePlayServicesUtil","Google Play services is missing.","22:35:16.400"),
            row(2578,"Process","Sending signal. PID: 2578 SIG: 9","22:35:20.425"),
        ]
        r=self.analyze(rows)
        self.assertEqual(r["original_activity_pid"],2578)
        self.assertEqual(r["fingerprint_reported_lengths"],{"113":1})
        self.assertEqual(r["events"]["native_build_fingerprint_api_overlength"]["count"],1)
        self.assertEqual(r["events"]["google_play_services_missing"]["count"],1)
        self.assertEqual(r["self_sigkill_ms_after_activity_start"],4649)
    def test_other_android_process_never_counted(self):
        r=self.analyze([
            start(1234,"22:35:15.776"),
            row(7777,"libc",'The property "ro.build.fingerprint" has a value with length 113 that is too large',"22:35:16.200"),
            row(1234,"Process","Sending signal. PID: 1234 SIG: 9","22:35:20.425")])
        self.assertFalse(r["fingerprint_reported_lengths"])
    def test_event_after_kill_not_associated_with_game(self):
        r=self.analyze([
            start(1234,"22:35:15.776"),
            row(1234,"Process","Sending signal. PID: 1234 SIG: 9","22:35:20.425"),
            row(1234,"Sensors","Failed to get Sensor list","22:35:20.930")])
        self.assertEqual(r["events"]["sensor_enumeration_failed"]["count"],0)
    def test_before_after_not_evidence_causation(self):
        a=self.analyze([
            start(1234,"22:35:04.086"),
            row(1234,"Sensors","Failed to get Sensor list","22:35:04.500"),
            row(1234,"Process","Sending signal. PID: 1234 SIG: 9","22:35:08.981")])
        b=self.analyze([
            start(9999,"22:35:15.776"),
            row(9999,"Sensors","Failed to get Sensor list","22:35:16.200"),
            row(9999,"Process","Sending signal. PID: 9999 SIG: 9","22:35:20.425")])
        c=compare(a,b)
        item=next(x for x in c["process_scoped_event_comparison"]
                  if x["event"]=="sensor_enumeration_failed")
        self.assertEqual(item["classification"],"PERSISTENT_BOTH")
        self.assertFalse(c["can_identify_specific_security_check"])
    def test_missing_original_pid_fails_closed(self):
        with self.assertRaisesRegex(ValueError,"No exact"):
            self.analyze([row(1234,"Sensors","Failed to get Sensor list","22:35:15.776")])
if __name__=="__main__":
    unittest.main()
