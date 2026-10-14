import tempfile
import unittest
from pathlib import Path
from PIL import Image,ImageDraw
from test_virtual_overlay_sequence import sample
from check_virtual_feed_control import check

def fixture(root,replay_visible=False,omitted=None,bad_sequence=False):
    sample(root)
    original=(root/"sequence_logcat.txt").read_text()
    records=[
        "10-14 10:00:01.100 1234 1234 I OXIDE_BRIDGE_OVERLAY: RESULT=PASS FEED_READY SYNTHETIC_SHELL_ONLY",
        "10-14 10:00:02.100 1234 1234 I OXIDE_BRIDGE_OVERLAY: RESULT=PASS FEED_SEQ=1 AGE_MS=5 RECT_COUNT=1 SYNTHETIC_ONLY",
        "10-14 10:00:04.100 1234 1234 I OXIDE_BRIDGE_OVERLAY: RESULT=PASS FEED_SEQ=2 AGE_MS=600 RECT_COUNT=0 SYNTHETIC_ONLY",
        "10-14 10:00:04.500 1234 1234 W OXIDE_BRIDGE_OVERLAY: RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=2",
        "10-14 10:00:06.100 1234 1234 I OXIDE_BRIDGE_OVERLAY: RESULT=PASS FEED_SEQ=3 AGE_MS=5 RECT_COUNT=1 SYNTHETIC_ONLY",
        "10-14 10:00:07.100 1234 1234 I OXIDE_BRIDGE_OVERLAY: RESULT=PASS FEED_STOP_REQUEST SEQ=4"
    ]
    if bad_sequence:
        records[4]=records[4].replace("FEED_SEQ=3","FEED_SEQ=2")
    if omitted:
        records=[line for line in records if omitted not in line]
    (root/"sequence_logcat.txt").write_text(original+"\n"+"\n".join(records)+"\n")
    im=Image.new("RGB",(720,1280),(12,16,23))
    if replay_visible:
        ImageDraw.Draw(im).rectangle((330,450,480,750),
                                      outline=(38,255,110),width=6)
    im.save(root/"sequence_replay.png")

class ExternalSyntheticFeedTests(unittest.TestCase):
    def run_case(self,**kw):
        with tempfile.TemporaryDirectory() as tmp:
            root=Path(tmp);fixture(root,**kw)
            return check(root)
    def test_external_monotonic_feed_passes_while_original_absent(self):
        result=self.run_case()
        self.assertEqual(result["status"],"NATIVE_ARM64_ADB_FEED_REPLAY_SAFE_PASS")
        self.assertTrue(all(result["checks"].values()))
        self.assertFalse(result["multiplayer_esp_verified"])
    def test_replayed_snapshot_with_marker_is_failure(self):
        self.assertFalse(self.run_case(replay_visible=True)
                         ["checks"]["replayed_input_did_not_restore_expired_box"])
    def test_missing_privileged_feed_marker_fails(self):
        self.assertFalse(self.run_case(omitted="FEED_READY")
                         ["checks"]["shell_feed_receiver_ready"])
    def test_missing_replay_rejection_fails(self):
        self.assertFalse(self.run_case(omitted="REPLAY_OR_OUT_OF_ORDER")
                         ["checks"]["out_of_order_input_explicitly_rejected"])
    def test_older_accepted_seq_fails(self):
        self.assertFalse(self.run_case(bad_sequence=True)
                         ["checks"]["exact_external_sequences_accepted"])
    def test_missing_shutdown_rejected(self):
        self.assertFalse(self.run_case(omitted="FEED_STOP_REQUEST")
                         ["checks"]["service_stopped_via_next_accepted_sequence"])

if __name__=="__main__":
    unittest.main()
