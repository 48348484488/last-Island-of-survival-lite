import importlib.util
from pathlib import Path
import unittest
from types import SimpleNamespace

spec=importlib.util.spec_from_file_location("aud",Path(__file__).resolve().parents[1]/"tools/audit_camera_contract.py")
a=importlib.util.module_from_spec(spec)
spec.loader.exec_module(a)

class CameraContractTests(unittest.TestCase):
    def test_monoscript_pointer(self):
        self.assertEqual(a.pointer({"m_FileID":1,"m_PathID":-94}),{"fileID":1,"pathID":-94})
    def test_boolean_is_not_pathid(self):
        self.assertIsNone(a.pointer({"m_FileID":0,"m_PathID":True}))
    def test_local_nested_pointers(self):
        self.assertEqual(a.safe_value({"m_FileID":0,"m_PathID":834}),{"fileID":0,"pathID":834})
    def test_list_bounds(self):
        self.assertEqual(len(a.safe_value(list(range(400)))),24)
    def test_external_list_1_based(self):
        externals=[SimpleNamespace(path_name="archive:/monoscripts",name="main"),SimpleNamespace(path_name="archive:/shaders")]
        r=a.ext_summary(externals)
        self.assertEqual([x["fileID"] for x in r],[1,2])
        self.assertEqual(r[0]["info"]["path_name"],"archive:/monoscripts")
    def test_external_missing(self):
        self.assertEqual(a.ext_summary(None),[])
    def test_serialized_file_metadata(self):
        obj=SimpleNamespace(name="CAB-1",externals=[SimpleNamespace(path_name="X")])
        info=a.file_description(obj)
        self.assertEqual(info["externalReferences"][0]["fileID"],1)
        self.assertEqual(info["name"],"CAB-1")
if __name__=="__main__":
    unittest.main()
