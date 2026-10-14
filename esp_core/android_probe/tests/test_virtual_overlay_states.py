import sys
import tempfile
import unittest
from pathlib import Path
from PIL import Image,ImageDraw

sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
from check_virtual_overlay_states import evaluate

def write_image(root,name,green):
    img=Image.new("RGB",(720,1280),(11,16,24))
    if green:
        draw=ImageDraw.Draw(img)
        draw.rectangle((295,445,425,733),outline=(38,255,110),width=5)
    img.save(root/("overlay_"+name+".png"))

def populate(root,empty=False,stale=False,missinglog=False,with_game=False):
    for name in ("fresh","empty","stale","removed"):
        write_image(root,name,name=="fresh" or name=="empty" and empty or name=="stale" and stale)
    for name in ("fresh","empty","stale"):
        (root/("pid_"+name+".txt")).write_text("1234\n")
        (root/("launch_"+name+".txt")).write_text("Status: ok\n")
    (root/"game_packages.txt").write_text(
        "package:com.catsbit.oxidesurvivalisland\n" if with_game else "")
    log="" if missinglog else "\n".join(
        "10-14 10:10:01.111 1234 1234 I OXIDE_BRIDGE_OVERLAY: "
        "RESULT=PASS MODE="+mode+" RECT_COUNT="+str(1 if state=="fresh" else 0)
        +" SOURCE=CONTROLLED_SYNTHETIC_ONLY" for state,mode in (
            ("fresh","OVERLAY_DIAGNOSTIC"),
            ("empty","OVERLAY_NO_SAMPLES"),
            ("stale","OVERLAY_STALE_SAMPLES")))
    (root/"logcat.txt").write_text(log)

class VirtualOverlayStatesTests(unittest.TestCase):
    def check(self,**kw):
        with tempfile.TemporaryDirectory() as tmp:
            folder=Path(tmp);populate(folder,**kw);return evaluate(folder)
    def test_fresh_bright_green_and_empty_stale_cleared(self):
        r=self.check()
        self.assertEqual(r["result"],"VIRTUAL_ARM64_ISOLATED_OVERLAY_3_STATES_AND_STOP_PASS")
        self.assertGreater(r["green_center_region_pixels"]["fresh"],500)
        self.assertFalse(r["real_multiplayer_esp_verified"])
    def test_empty_marker_is_unsafe(self):
        self.assertFalse(self.check(empty=True)["checks"]["zero_sample_marker_cleared"])
    def test_stale_marker_is_unsafe(self):
        self.assertFalse(self.check(stale=True)["checks"]["stale_sample_marker_cleared"])
    def test_missing_service_success_logs_rejected(self):
        self.assertFalse(self.check(missinglog=True)["checks"]["service_log_marker_each_mode"])
    def test_original_game_installed_fails(self):
        self.assertFalse(self.check(with_game=True)["checks"]["original_game_absent"])
    def test_crash_from_our_app_is_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            folder=Path(tmp);populate(folder)
            with (folder/"logcat.txt").open("a") as f:
                f.write("\nAndroidRuntime: Process: com.oxide.bridgeprobe, PID: 1234\n")
            self.assertFalse(evaluate(folder)["checks"]["no_android_runtime_crash"])
    def test_service_pid_needed_after_activity_finishes(self):
        with tempfile.TemporaryDirectory() as tmp:
            folder=Path(tmp);populate(folder)
            (folder/"pid_fresh.txt").write_text("\n")
            self.assertFalse(evaluate(folder)["checks"]["service_pid_alive_fresh"])
if __name__=="__main__":
    unittest.main()
