import importlib.util
from pathlib import Path
import struct
import unittest

spec=importlib.util.spec_from_file_location("unity_scan",Path(__file__).resolve().parents[1]/"tools/audit_unityfs.py")
a=importlib.util.module_from_spec(spec)
spec.loader.exec_module(a)

class TestUnityFS(unittest.TestCase):
    def fixture(self,payload,compressed):
        import lz4.block
        body=lz4.block.compress(payload,store_size=False) if compressed else payload
        info=bytes(16)+struct.pack(">I",1)+struct.pack(">IIH",len(payload),len(body),2 if compressed else 0)+struct.pack(">I",0)
        meta=lz4.block.compress(info,store_size=False)
        head=b"UnityFS\0"+struct.pack(">I",7)+b"6000.3.18f1\0"+b"test\0"
        pad=b"\0"*((-(len(head)+20))%16)
        total=len(head)+20+len(pad)+len(meta)+len(body)
        return head+struct.pack(">QIII",total,len(meta),len(info),2)+pad+meta+body
    def test_plain_body(self):
        result=a.inspect_bundle(self.fixture(b"PlayerManager\0visRange",False))
        self.assertEqual(result["terms"].get("PlayerManager"),1)
        self.assertEqual(result["terms"].get("visRange"),1)
        self.assertEqual(result["unity_version"],"6000.3.18f1")
    def test_compressed_body(self):
        result=a.inspect_bundle(self.fixture(b"networkPositionObserver"*5,True))
        self.assertEqual(result["terms"].get("networkPositionObserver"),5)
    def test_truncated(self):
        raw=self.fixture(b"PlayerManager",True)[:-1]
        with self.assertRaises(ValueError):a.inspect_bundle(raw)

if __name__=="__main__":
    unittest.main()
