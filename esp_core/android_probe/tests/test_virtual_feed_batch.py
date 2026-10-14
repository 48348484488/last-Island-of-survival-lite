import sys
import tempfile
import unittest
from pathlib import Path
from PIL import Image, ImageDraw

sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
from check_virtual_feed_batch import evaluate, X, STAGES

def make_screen(path, stage, leaked=False):
    im=Image.new("RGB",(720,1280),(12,16,23))
    draw=ImageDraw.Draw(im)
    for i,x in enumerate(X):
        if stage in ("idle","stopped"): continue
        if stage in ("half","replay") and i%2==1 and not leaked: continue
        cx=360+72*x*(0.8 if stage=="changed_camera" else 1)
        draw.rectangle((cx-19,595,cx+19,685),outline=(38,255,110),width=4)
    im.save(path)

def setup(root, *, leaked=False, skip_replay=False, changing_pid=False):
    for stage in STAGES:
        make_screen(root/f"batch_{stage}.png",stage,leaked)
        (root/f"batch_pid_{stage}.txt").write_text(
            "1415" if changing_pid and stage=="idle" else "1414")
    logs=[
        "RESULT=PASS FEED_BATCH_SEQ=100 INPUT_COUNT=8 RECT_COUNT=8",
        "RESULT=PASS FEED_BATCH_SEQ=101 INPUT_COUNT=8 RECT_COUNT=4",
        "RESULT=DROP REPLAY_OR_OUT_OF_ORDER SEQ=101",
        "RESULT=DROP INVALID_BATCH_SHAPE SEQ=102",
        "RESULT=DROP INVALID_BATCH_PROJECTION SEQ=102",
        "RESULT=PASS FEED_BATCH_SEQ=102 INPUT_COUNT=8 RECT_COUNT=8",
        "RESULT=PASS FEED_IDLE_TIMEOUT RECT_COUNT=0",
        "RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE",
    ]
    if skip_replay: logs=[s for s in logs if "REPLAY_OR_OUT_OF_ORDER" not in s]
    (root/"batch_logcat.txt").write_text("\n".join("10-14 12:00:00.000 1414 1414 I OXIDE_BRIDGE_OVERLAY: "+s for s in logs)+"\n")
    (root/"batch_game_packages.txt").write_text("")

class BatchVerdictRegressions(unittest.TestCase):
    def verdict(self,**kwargs):
        with tempfile.TemporaryDirectory() as d:
            root=Path(d)
            setup(root,**kwargs)
            return evaluate(root)
    def test_valid_eight_object_batch(self):
        r=self.verdict()
        self.assertEqual(r["status"],"ARM64_VIRTUAL_8_ENTITY_BATCH_PASS",r)
        self.assertTrue(all(r["checks"].values()))
        self.assertFalse(r["real_player_data_verified"])
    def test_stale_entity_must_disappear(self):
        r=self.verdict(leaked=True)
        self.assertFalse(r["checks"]["four_independently_stale"])
    def test_replay_denial_mandatory(self):
        r=self.verdict(skip_replay=True)
        self.assertFalse(r["checks"]["replay_blocked"])
    def test_pid_must_remain_constant(self):
        r=self.verdict(changing_pid=True)
        self.assertFalse(r["checks"]["stable_service_pid"])

if __name__=="__main__": unittest.main()
