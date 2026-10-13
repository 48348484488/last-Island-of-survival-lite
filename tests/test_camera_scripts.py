import importlib.util
from pathlib import Path
import unittest
spec=importlib.util.spec_from_file_location("scripts",Path(__file__).resolve().parents[1]/"tools/audit_camera_scripts.py")
mod=importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

class CameraScriptTests(unittest.TestCase):
    def test_complete_pptr(self):
        self.assertEqual(mod.pp({"m_FileID":3,"m_PathID":-77}),{"fileID":3,"pathID":-77})
    def test_external_component(self):
        self.assertEqual(mod.component_refs([{"m_FileID":0,"m_PathID":22},
                                             {"m_FileID":3,"m_PathID":5}]),
                          [{"fileID":0,"pathID":22},{"fileID":3,"pathID":5}])
    def test_unnamed_pointer_rejected(self):
        self.assertIsNone(mod.pp({"pathID":1}))
        self.assertIsNone(mod.pp({"m_FileID":0,"m_PathID":True}))
    def test_empty(self):
        self.assertEqual(mod.component_refs(None),[])
    def test_nested_component(self):
        self.assertEqual(mod.component_refs({"component":{"m_FileID":0,"m_PathID":-13}}),
                         [{"fileID":0,"pathID":-13}])
if __name__=="__main__":unittest.main()
