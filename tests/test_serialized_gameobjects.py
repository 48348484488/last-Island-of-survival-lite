import importlib.util
from pathlib import Path
import unittest

spec=importlib.util.spec_from_file_location("aud",Path(__file__).resolve().parents[1]/"tools/audit_serialized_gameobjects.py")
mod=importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

class TestPointers(unittest.TestCase):
    def test_pptr(self):
        self.assertEqual(mod.pointer({"m_FileID":0,"m_PathID":123}),{"fileID":0,"pathID":123})
    def test_not_memory_address(self):
        self.assertIsNone(mod.pointer({"address":123}))
    def test_noninteger(self):
        self.assertIsNone(mod.pointer({"m_FileID":0,"m_PathID":"123"}))
    def test_nested_refs(self):
        x={"m_Component":[{"component":{"m_FileID":0,"m_PathID":42}},{"component":{"m_FileID":0,"m_PathID":56}}]}
        self.assertEqual(mod.flatten_pointers(x),[{"fileID":0,"pathID":42},{"fileID":0,"pathID":56}])
    def test_bounded_serialization(self):
        r=mod.safe_extract({"m_PathID":10,"m_FileID":0})
        self.assertEqual(r,{"ptr":{"fileID":0,"pathID":10}})
    def test_ptr_without_fileid(self):
        self.assertEqual(mod.pointer({"path_id":123}),{"fileID":None,"pathID":123})
class TestTransformResolution(unittest.TestCase):
    def test_named_hierarchy(self):
        trans={10:{"m_GameObject":{"m_FileID":0,"m_PathID":101},
                   "m_Father":{"m_FileID":0,"m_PathID":20}},
               20:{"m_GameObject":{"m_FileID":0,"m_PathID":102},
                   "m_Father":{"m_FileID":0,"m_PathID":0}}}
        go={101:{"name":"Camera Root"},102:{"name":"player"}}
        chain=mod.resolve_transform({"fileID":0,"pathID":10},trans,go)
        self.assertEqual([x["gameObject"] for x in chain],["Camera Root","player"])
    def test_external_transform_unresolved(self):
        self.assertIsNone(mod.resolve_transform({"fileID":1,"pathID":10},{},{}))
    def test_cycle_bounded(self):
        trans={1:{"m_Father":{"m_FileID":0,"m_PathID":2}},
               2:{"m_Father":{"m_FileID":0,"m_PathID":1}}}
        chain=mod.resolve_transform({"fileID":0,"pathID":1},trans,{})
        self.assertLessEqual(len(chain),3)
        self.assertIn("cycle_at",chain[-1])

if __name__=="__main__":unittest.main()
