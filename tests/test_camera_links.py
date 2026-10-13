import importlib.util
from pathlib import Path
import unittest
spec=importlib.util.spec_from_file_location("camera_links",Path(__file__).resolve().parents[1]/"tools/audit_camera_links.py")
cam=importlib.util.module_from_spec(spec)
spec.loader.exec_module(cam)

class TestCameraLinks(unittest.TestCase):
    def test_positive_and_negative_pathid(self):
        self.assertEqual(cam.local_ptr({"m_FileID":0,"m_PathID":-44}),-44)
        self.assertEqual(cam.local_ptr({"m_FileID":0,"m_PathID":13}),13)
    def test_no_external_or_null(self):
        self.assertIsNone(cam.local_ptr({"m_FileID":1,"m_PathID":44}))
        self.assertIsNone(cam.local_ptr({"m_FileID":0,"m_PathID":0}))
    def test_reject_bool_non_int(self):
        self.assertIsNone(cam.local_ptr({"m_FileID":0,"m_PathID":True}))
        self.assertIsNone(cam.local_ptr({"m_FileID":0,"m_PathID":"44"}))
    def test_nested_refs(self):
        obj={"a":{"m_FileID":0,"m_PathID":-2},"b":[{"m_FileID":0,"m_PathID":51},
                {"m_FileID":2,"m_PathID":4}]}
        self.assertEqual(cam.refs(obj),[-2,51])
    def test_bounded_metadata(self):
        self.assertEqual(cam.simple({"m_FileID":0,"m_PathID":33}),{"localPathID":33})
        self.assertEqual(cam.simple([{"m_FileID":0,"m_PathID":3}])["listLength"],1)
    def test_invalid(self):
        self.assertEqual(cam.refs(None),[])
        self.assertEqual(cam.refs(1),[])
        self.assertEqual(cam.refs([{"m_FileID":1,"m_PathID":1}]),[])
if __name__=="__main__":unittest.main()
