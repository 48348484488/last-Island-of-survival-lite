import importlib.util
import struct
import unittest
from pathlib import Path
spec=importlib.util.spec_from_file_location(
    "elf",Path(__file__).resolve().parents[1]/"tools/map_original_loader_pcs.py")
elf=importlib.util.module_from_spec(spec)
spec.loader.exec_module(elf)

def fake_elf():
    data=bytearray(4096)
    data[:4]=b"\x7fELF"
    data[4],data[5]=2,1
    struct.pack_into("<H",data,18,183)
    struct.pack_into("<Q",data,32,64)
    struct.pack_into("<HH",data,54,56,1)
    struct.pack_into("<IIQQQQQQ",data,64,1,5,512,0x830000,0,512,512,4096)
    data[512+0x100:512+0x104]=b"\x00\x00\x80\xd2"
    return bytes(data)
class Mappings(unittest.TestCase):
    def test_maps_pc_inside_executable_segment(self):
        r=elf.inspect(fake_elf(),(0x830100,))
        self.assertEqual(r["target_pcs"][0]["instruction_at_pc"],"000080d2")
        self.assertEqual(r["target_pcs"][0]["file_offset"],hex(512+0x100))
    def test_non_file_backed_address_fails(self):
        with self.assertRaisesRegex(ValueError,"not in file-backed"):
            elf.inspect(fake_elf(),(0x830800,))
    def test_wrong_architecture_fails(self):
        b=bytearray(fake_elf())
        struct.pack_into("<H",b,18,62)
        with self.assertRaisesRegex(ValueError,"AArch64"):
            elf.inspect(bytes(b),(0x830100,))
    def test_alignment_enforced(self):
        with self.assertRaisesRegex(ValueError,"Unaligned"):
            elf.inspect(fake_elf(),(0x830101,))
if __name__=="__main__":
    unittest.main()
