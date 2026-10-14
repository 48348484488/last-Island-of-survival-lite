import hashlib
import io
import struct
import sys
import tempfile
import unittest
import zipfile
from pathlib import Path

sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from audit_original_unityfs_blocks import (
    audit, scan_bundle, parse_directory, MAX_BLOCK_BYTES
)

try:
    import lz4.block
except ImportError:
    lz4=None

def build_bundle(parts,*,meta_at_end=False,block_codecs=None):
    block_codecs=block_codecs or [3]*len(parts)
    packed=[]
    for part,codec in zip(parts,block_codecs):
        packed.append(part if codec==0 else
                      lz4.block.compress(part,mode="high_compression",
                                         store_size=False))
    raw_meta=b"\x00"*16+struct.pack(">I",len(parts))
    for part,bytes_on_disk,codec in zip(parts,packed,block_codecs):
        raw_meta+=struct.pack(">IIH",len(part),len(bytes_on_disk),codec)
    raw_meta+=struct.pack(">IQQI",1,0,sum(map(len,parts)),4)+b"asset\x00"
    compressed_meta=lz4.block.compress(raw_meta,mode="high_compression",
                                        store_size=False)
    prefix=(b"UnityFS\x00"+struct.pack(">I",6)+
            b"2021.3.11f1\x00"+b"2021.3.11f1\x00")
    flags=0x43 | (0x80 if meta_at_end else 0)
    hlen=len(prefix)+20
    aligned=(hlen+15)//16*16
    padding=b"\x00"*(aligned-hlen)
    after=(b"".join(packed)+compressed_meta if meta_at_end else
           compressed_meta+b"".join(packed))
    total=aligned+len(after)
    header=prefix+struct.pack(">QIII",total,len(compressed_meta),
                                len(raw_meta),flags)
    return header+padding+after

@unittest.skipIf(lz4 is None,"lz4 package not installed")
class UnityFSReader(unittest.TestCase):
    def test_lz4hc_metadata_and_data_two_block_cross_boundary(self):
        term=b"Security Threat Detected by App"
        payload=build_bundle([b"X"*7+term[:9],term[9:]+b"Z"*13])
        out=scan_bundle(io.BytesIO(payload),len(payload),100000)
        self.assertTrue(out["data_scan_complete"],out)
        self.assertEqual(out["block_count"],2)
        self.assertEqual(out["node_count"],1)
        self.assertEqual(out["index_compression"],"LZ4HC")
        self.assertEqual(out["data_block_codecs"],{"LZ4HC":2})
        self.assertEqual(out["pattern_offsets"]["Security Threat Detected by App [utf-8]"],[7])
    def test_metadata_at_end_and_uncompressed_data_block(self):
        payload=build_bundle([b"other data"],meta_at_end=True,block_codecs=[0])
        out=scan_bundle(io.BytesIO(payload),len(payload),100000)
        self.assertEqual(out["data_block_codecs"],{"NONE":1})
        self.assertTrue(out["data_scan_complete"],out)
        self.assertFalse(out["pattern_offsets"])
    def test_global_budget_rejected_before_allocating_bundle(self):
        payload=build_bundle([b"sample"],meta_at_end=True)
        with self.assertRaisesRegex(ValueError,"GLOBAL_DECOMPRESSION_LIMIT"):
            scan_bundle(io.BytesIO(payload),len(payload),3)
    def test_truncated_bundle_fails_closed(self):
        payload=build_bundle([b"hello"])
        with self.assertRaisesRegex(ValueError,"INVALID_UNITYFS_DECLARED_SIZE"):
            scan_bundle(io.BytesIO(payload[:-1]),len(payload)-1,100000)
    def test_apk_sha256_gate_and_bundle_count(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk"
            payload=build_bundle([b"ordinary Unity content"])
            with zipfile.ZipFile(path,"w") as z:
                z.writestr("assets/streamed/test.bundle",payload)
                z.writestr("hello.txt",b"ignored")
            digest=hashlib.sha256(path.read_bytes()).hexdigest()
            result=audit([path],{"base.apk":digest})
            self.assertEqual(result["unityfs_bundles_detected"],1)
            self.assertEqual(result["complete_bundle_scans"],1)
            self.assertFalse(result["all_original_apk_splits"])
            self.assertFalse(result["game_modified"])
    def test_reject_bad_original_digest(self):
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/"base.apk";path.write_bytes(b"invalid")
            with self.assertRaisesRegex(ValueError,"ORIGINAL_APK_SHA256_MISMATCH"):
                audit([path],{"base.apk":"bad"})

class UnityFSDirectoryValidation(unittest.TestCase):
    def test_too_short_directory(self):
        with self.assertRaisesRegex(ValueError,"DIRECTORY_TOO_SHORT"):
            parse_directory(b"short")
    def test_invalid_block_count(self):
        raw=b"\x00"*16+struct.pack(">I",10000000)+b"\x00"*4
        with self.assertRaisesRegex(ValueError,"INVALID_BLOCK_COUNT"):
            parse_directory(raw)
    def test_unexpected_node_range(self):
        raw=b"\x00"*16+struct.pack(">I",1)+struct.pack(">IIH",4,4,0)
        raw+=struct.pack(">IQQI",1,30,1,0)+b"asset\x00"
        with self.assertRaisesRegex(ValueError,"DIRECTORY_NODE_OUT_OF_RANGE"):
            parse_directory(raw)

if __name__=="__main__":
    unittest.main()
