import sys
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from classify_virtual_gralloc_probe import classify, EXPECTED

def row(pid,tag,message):
    return f"10-14 00:33:11.234 {pid} {pid} I {tag}: {message}"

def sample(pid=2337,fail43=True,fail56=True,rgba=True):
    lines=[row(pid,"OXIDE_GRALLOC_PROBE","STAGE=START API=33 ABIS=[arm64-v8a] ORIGINAL_GAME_INSTALLED=false")]
    for name,(fmt,usage) in EXPECTED.items():
        result=("FAIL" if (name=="RGBA1010102_EXACT" and fail43 or
                           name=="R8_EXACT" and fail56) else "PASS")
        if name=="RGBA8888_GPU_CONTROL" and not rgba:
            result="FAIL"
        lines.append(row(pid,"OXIDE_GRALLOC_PROBE",
            f"CASE={name} FORMAT={fmt} WIDTH=4 HEIGHT=4 LAYERS=1 USAGE=0x{usage:x}"
            f" SUPPORTED={'false' if result=='FAIL' else 'true'}"
            f" CREATE={result} FAILURE={'ILLEGAL_ARGUMENT' if result=='FAIL' else 'NONE'}"))
    lines.append(row(pid,"OXIDE_GRALLOC_PROBE","STAGE=COMPLETE CASES=6 ORIGINAL_GAME_TOUCHED=false"))
    return "\n".join(lines)
class GrallocProbeTest(unittest.TestCase):
    def test_two_real_formats_fail_but_normal_gpu_buffer_succeeds(self):
        d=classify(sample())
        self.assertEqual(d["status"],"VIRTUAL_ANDROID_FORMAT_LIMITATION_REPRODUCED_IN_SEPARATE_APP")
        self.assertTrue(d["exact_original_formats_43_and_56_fail"])
        self.assertTrue(d["rgba8888_gpu_control_success"])
        self.assertFalse(d["security_check_rejection_cause_proven"])
    def test_no_repro_can_be_reported(self):
        d=classify(sample(fail43=False,fail56=False))
        self.assertEqual(d["status"],"ORIGINAL_GRAPHICS_ERRORS_NOT_REPRODUCED_IN_SEPARATE_APP")
    def test_no_control_not_blind_success(self):
        d=classify(sample(rgba=False))
        self.assertEqual(d["status"],"EXACT_FORMATS_FAILED_BUT_RGBA_CONTROL_UNAVAILABLE")
    def test_another_process_cannot_inject_result(self):
        text=sample()+ "\n"+row(999,"OXIDE_GRALLOC_PROBE",
          "CASE=FAKE FORMAT=43 WIDTH=4 HEIGHT=4 LAYERS=1 USAGE=0xb00 SUPPORTED=true CREATE=PASS FAILURE=NONE")
        d=classify(text)
        self.assertTrue(d["probe_complete"])
        self.assertEqual(len(d["cases"]),6)
    def test_incomplete_probe_not_success(self):
        d=classify(sample().replace("STAGE=COMPLETE CASES=6 ORIGINAL_GAME_TOUCHED=false",""))
        self.assertFalse(d["probe_complete"])
        self.assertEqual(d["status"],"INDEPENDENT_GRAPHICS_PROBE_INCOMPLETE")
    def test_duplicate_case_fails_closed(self):
        s=sample()
        s+= "\n"+row(2337,"OXIDE_GRALLOC_PROBE",
          "CASE=R8_EXACT FORMAT=56 WIDTH=4 HEIGHT=4 LAYERS=1 USAGE=0xb00 SUPPORTED=false CREATE=FAIL FAILURE=ILLEGAL_ARGUMENT")
        self.assertFalse(classify(s)["probe_complete"])
if __name__=="__main__":
    unittest.main()
