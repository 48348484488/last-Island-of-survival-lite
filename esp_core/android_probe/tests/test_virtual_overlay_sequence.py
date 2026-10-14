import sys
import tempfile
import unittest
from pathlib import Path
from PIL import Image,ImageDraw
sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
from check_virtual_overlay_sequence import evaluate,PHASES

def screen(path,center,visible):
    im=Image.new("RGB",(720,1280),(12,16,23))
    if visible:
        ImageDraw.Draw(im).rectangle((center-75,450,center+75,750),
                                     outline=(38,255,110),width=6)
    im.save(path)

def sample(root,*,stale_visible=False,no_move=False,extra_pid=False,missing_phase=False):
    centers={"INITIAL":360,"MOVED":360 if no_move else 432,
             "STALE":432,"RESTORED":288,"STOPPED":360}
    for phase in PHASES:
        screen(root/("sequence_"+phase.lower()+".png"),centers[phase],
               phase in ("INITIAL","MOVED","RESTORED") or
               phase=="STALE" and stale_visible)
        (root/("sequence_pid_"+phase.lower()+".txt")).write_text(
            "" if phase=="STOPPED" else "1234" if not extra_pid or phase=="INITIAL" else "7777")
    (root/"sequence_game_packages.txt").write_text("")
    phases=[phase for phase in PHASES[:-1] if phase != ("MOVED" if missing_phase else "")]
    log="\n".join(
        "10-14 11:23:04.100 1234 1234 I OXIDE_BRIDGE_OVERLAY: "
        f"RESULT=PASS PHASE={phase} RECT_COUNT={0 if phase=='STALE' else 1}"
        " SOURCE=SYNTHETIC_JNI" for phase in phases)
    log+="\n10-14 11:23:15.100 1234 1234 I OXIDE_BRIDGE_OVERLAY: "
    log+="RESULT=PASS PHASE=STOPPED RECT_COUNT=0 WINDOW_REMOVED=TRUE\n"
    (root/"sequence_logcat.txt").write_text(log)

class SyntheticOverlaySequence(unittest.TestCase):
    def examine(self,**kwargs):
        with tempfile.TemporaryDirectory() as tmp:
            root=Path(tmp)
            sample(root,**kwargs)
            return evaluate(root)
    def test_complete_movement_stale_restore_and_stop(self):
        r=self.examine()
        self.assertEqual(r["status"],"NATIVE_VIRTUAL_ARM64_CONTINUOUS_JNI_OVERLAY_PASS")
        self.assertTrue(all(r["checks"].values()))
        self.assertFalse(r["live_multiplayer_or_real_player_esp_verified"])
    def test_stale_frame_not_cleared(self):
        self.assertFalse(self.examine(stale_visible=True)["checks"]["stale_cleared"])
    def test_movement_must_really_change_pixels(self):
        self.assertFalse(self.examine(no_move=True)["checks"]["moves_right"])
    def test_relaunch_of_service_not_treated_as_one(self):
        self.assertFalse(self.examine(extra_pid=True)["checks"]["same_service_pid_during_updates"])
    def test_missing_phase_not_reported_pass(self):
        self.assertFalse(self.examine(missing_phase=True)["checks"]["native_four_phase_transitions_exactly_once"])
    def test_game_installed_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            root=Path(tmp);sample(root)
            (root/"sequence_game_packages.txt").write_text(
                "package:com.catsbit.oxidesurvivalisland\n")
            self.assertFalse(evaluate(root)["checks"]["game_not_installed"])
if __name__=="__main__":
    unittest.main()
