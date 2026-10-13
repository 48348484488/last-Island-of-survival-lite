import importlib.util
from pathlib import Path
import unittest
spec=importlib.util.spec_from_file_location("cam",Path(__file__).resolve().parents[1]/"tools/audit_all_cameras.py")
cam=importlib.util.module_from_spec(spec)
spec.loader.exec_module(cam)

class CameraLookupTests(unittest.TestCase):
    def test_local_pointer(self):
        self.assertEqual(cam.ptr({"m_FileID":0,"m_PathID":-123}),-123)
    def test_null_pointer(self):
        self.assertIsNone(cam.ptr({"m_FileID":0,"m_PathID":0}))
    def test_external_pointer(self):
        self.assertIsNone(cam.ptr({"m_FileID":1,"m_PathID":123}))
    def test_invalid_pointer(self):
        self.assertIsNone(cam.ptr({"m_FileID":0,"m_PathID":"123"}))
        self.assertIsNone(cam.ptr({"m_FileID":0,"m_PathID":True}))
    def test_empty_object(self):
        self.assertIsNone(cam.ptr(None))
if __name__=="__main__":unittest.main()
