import tempfile
import unittest
import sys
from pathlib import Path
from PIL import Image, ImageDraw
sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
from check_virtual_feed_stress import evaluate, STAGES, COUNT

def snapshot(path, center=360, visible=True):
    im=Image.new("RGB",(720,1280),(12,16,23))
    if visible:
        ImageDraw.Draw(im).rectangle(
            (center-75,450,center+75,750),
            outline=(38,255,110),width=6)
    im.save(path)

def populate(root, *, omit=None, replay_box=False, pid_change=False,
             malformed_missing=False, old_game=False, no_idle=False):
    for stage in STAGES:
        visible=stage in ("FIRST","LAST","RESTORED")
        if stage=="REPLAY" and replay_box: visible=True
        center={"FIRST":300,"LAST":420,"RESTORED":305}.get(stage,360)
        snapshot(root/("stress_"+stage.lower()+".png"),center,visible)
        p="1337"
        if pid_change and stage in ("LAST","STALE","REPLAY","RESTORED","IDLE"):
            p="1888"
        (root/("stress_pid_"+stage.lower()+".txt")).write_text(
            p if stage!="STOPPED" else "")
    (root/"stress_game_packages.txt").write_text(
        "package:com.catsbit.oxidesurvivalisland" if old_game else "")
    events=["10-14 11:20:05.000 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
            "RESULT=PASS FEED_SEQ="+str(seq)+" AGE_MS=5 RECT_COUNT=1 SYNTHETIC_ONLY"
            for seq in range(1,COUNT+1)]
    events+=[
        "10-14 11:21:05.000 1337 1337 I OXIDE_BRIDGE_FEED: "
        "RESULT=DROP MALFORMED_FIELDS",
        "10-14 11:21:05.100 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
        "RESULT=PASS FEED_SEQ=65 AGE_MS=5 RECT_COUNT=1 SYNTHETIC_ONLY",
        "10-14 11:21:06.100 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
        "RESULT=PASS FEED_SEQ=66 AGE_MS=600 RECT_COUNT=0 SYNTHETIC_ONLY",
        "10-14 11:21:07.100 1337 1337 W OXIDE_BRIDGE_OVERLAY: "
        "RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=66",
        "10-14 11:21:08.100 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
        "RESULT=PASS FEED_SEQ=67 AGE_MS=5 RECT_COUNT=1 SYNTHETIC_ONLY",
        "10-14 11:21:12.100 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
        "RESULT=PASS FEED_IDLE_TIMEOUT RECT_COUNT=0",
        "10-14 11:21:12.200 1337 1337 W OXIDE_BRIDGE_FEED: "
        "RESULT=DROP MISSING_REQUIRED_FIELDS SEQ=68",
        "10-14 11:21:13.000 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
        "RESULT=PASS FEED_STOP_REQUEST SEQ=68",
        "10-14 11:21:13.100 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
        "RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE",
    ]
    if omit:
        events=[x for x in events if omit not in x]
    if malformed_missing:
        events.append("10-14 11:21:14.100 1337 1337 I OXIDE_BRIDGE_OVERLAY: "
                      "RESULT=PASS FEED_SEQ=65 AGE_MS=5 RECT_COUNT=1 SYNTHETIC_ONLY")
    if no_idle:
        events=[x for x in events if "FEED_IDLE_TIMEOUT" not in x]
    (root/"stress_logcat.txt").write_text("\n".join(events)+"\n")

class ExternalFeedStressRegression(unittest.TestCase):
    def analyze(self,**kwargs):
        with tempfile.TemporaryDirectory() as tmp:
            root=Path(tmp)
            populate(root,**kwargs)
            return evaluate(root)

    def test_all_64_plus_new_fresh_stale_and_recovered(self):
        r=self.analyze()
        self.assertEqual(r["status"],"NATIVE_ARM64_VIRTUAL_64_SNAPSHOT_FEED_STRESS_PASS")
        self.assertTrue(all(r["checks"].values()))
        self.assertEqual(r["accepted_snapshot_count"],67)
        self.assertFalse(r["real_player_coordinates_proven"])

    def test_duplicate_malformed_message_fails_closed(self):
        r=self.analyze(malformed_missing=True)
        self.assertFalse(r["checks"]["all_64_consecutive_native_snapshots_accepted_exactly_once"])

    def test_replay_cannot_restore_old_box(self):
        r=self.analyze(replay_box=True)
        self.assertFalse(r["checks"]["replay_did_not_restore_box"])

    def test_same_process_must_persist(self):
        r=self.analyze(pid_change=True)
        self.assertFalse(r["checks"]["all_live_screens_same_pid"])

    def test_missing_idle_expiry_not_accepted(self):
        r=self.analyze(no_idle=True)
        self.assertFalse(r["checks"]["timeout_cleared_once"])

    def test_missing_rejection_of_bad_type_is_failure(self):
        r=self.analyze(omit="RESULT=DROP MALFORMED_FIELDS")
        self.assertFalse(r["checks"]["wrong_type_rejected_without_consuming_sequence"])

    def test_original_game_must_be_absent(self):
        r=self.analyze(old_game=True)
        self.assertFalse(r["checks"]["original_oxide_not_installed"])

if __name__=="__main__":
    unittest.main()
