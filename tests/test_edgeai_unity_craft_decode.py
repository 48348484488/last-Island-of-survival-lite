#!/usr/bin/env python3
"""Deterministic EdgeAI crafted Octets fixtures; NOT original-game data."""
import struct
import sys
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from edgeai_unity_craft_decode import parse_drawing,parse_items,report_from_bytes

def i(v): return struct.pack("<i",v)
def s(v):
    v=v.encode("utf-8")
    return i(len(v))+v

def drawing(ident=1001):
    return (i(ident)+s("source label")+i(0)+i(1)+i(12)+i(2)+
            b"\x01\x01\x00"+i(1)+i(0)+i(0)+i(7)+i(2)+
            i(4001)+i(10)+i(4002)+i(3)+i(7001)+i(5)+i(3)+i(1)+i(1))

def item(ident=7001):
    return (i(ident)+s("mat item")+i(2)+i(0)+i(1)+i(0)+
            b"\x01"+i(50)+i(0)+b"\x00\x01"+
            s("tubiao1/abc")+s("item desc")+i(0)+i(50)+i(ident)+
            b"\x01"+i(0)+i(1)+s("")+s("")+s("")+
            i(100)+i(0)+s("")+struct.pack("<f",0.5)+i(1)+
            struct.pack("<f",1.25)+i(1)+s("x")+i(0))

class TestCraftDecoder(unittest.TestCase):
    def test_real_original_lite73_first_record(self):
        # First 128 bytes copied from own original XAPK under controlled
        # source audit; tail completes the byte length at the record boundary.
        prefix = bytes.fromhex("acde000018000000e9858de696b9efbc9ae794b5e7ab9ee78b82e6aca25355560e0000000f00000000010000010000010001050000000100000003000000010000000900000078000000040000005ac30000c800000082c3000001000000c8da00001e0000007fc300001e0000000a500100e703000096010000010000000100") + bytes([0,0]) + bytes.fromhex("000e000000080000000000000001000000")
        row=parse_drawing(prefix)[0]
        self.assertEqual(row["id"],57004)
        self.assertEqual((row["bigType"],row["smallType"]),(14,15))
        self.assertEqual(row["record_variant"],"lite73")
        self.assertEqual(row["needTime"],120)
        self.assertEqual(row["materials"],[
            {"itemId":50010,"num":200},{"itemId":50050,"num":1},
            {"itemId":56008,"num":30},{"itemId":50047,"num":30}])
        self.assertEqual(row["targetItemId"],86026)
        self.assertEqual(row["unmapped_suffix"]["integers"],[14,8,0,1])
        self.assertEqual(row["byte_span"],[0,147])
    def test_source_drawing_roundtrip(self):
        rows=parse_drawing(drawing()+drawing(1002))
        self.assertEqual(len(rows),2)
        self.assertEqual(rows[0]["materials"],[
            {"itemId":4001,"num":10},{"itemId":4002,"num":3}])
        self.assertEqual(rows[0]["needTime"],7)
        self.assertEqual(rows[0]["onceMaxCreate"],5)
        self.assertEqual(rows[0]["byte_span"][1],len(drawing()))
    def test_item_roundtrip(self):
        rows=parse_items(item())
        self.assertEqual(rows[0]["icon"],"tubiao1/abc")
        self.assertEqual(rows[0]["maxPileNum"],100)
        self.assertAlmostEqual(rows[0]["extras"][0],1.25)
    def test_material_link(self):
        r=report_from_bytes(drawing(),item())
        self.assertEqual(r["item_linked"],1)
        self.assertEqual(r["all_records"][0]["target"]["icon"],"tubiao1/abc")
    def test_rejects_duplicate(self):
        with self.assertRaises(ValueError):
            parse_drawing(drawing()+drawing())
    def test_rejects_truncated(self):
        for payload in (drawing()[:20],drawing()[:-1],drawing()[:4]):
            with self.assertRaises((ValueError,UnicodeError)):
                parse_drawing(payload)
    def test_rejects_corrupted_boolean(self):
        data=bytearray(drawing())
        offset=4+4+len("source label")+4*4
        data[offset]=255
        with self.assertRaises(ValueError):parse_drawing(bytes(data))
    def test_rejects_bad_count(self):
        data=bytearray(drawing())
        off=4+4+len("source label")+4*4+3+4*4
        data[off:off+4]=i(9000)
        with self.assertRaises(ValueError):parse_drawing(bytes(data))
if __name__=="__main__":
    unittest.main()
