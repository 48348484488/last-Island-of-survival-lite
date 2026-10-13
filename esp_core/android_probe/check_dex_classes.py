#!/usr/bin/env python3
"""Reject Android APKs missing executable DEX class definitions.

Checking raw strings is insufficient: a class can be referenced but absent.
No APK modification or game interaction is performed.
"""
import struct
import sys
import zipfile

REQUIRED = (
    "Lcom/oxide/bridgeprobe/MainActivity;",
    "Lcom/oxide/bridgeprobe/MainActivity$NativeRectView;",
)


def class_definitions(data):
    if len(data) < 112 or not data.startswith(b"dex\n"):
        raise ValueError("not a DEX file")
    file_size = struct.unpack_from("<I", data, 32)[0]
    if file_size != len(data):
        raise ValueError("invalid DEX length")
    str_count, str_off, typ_count, typ_off = struct.unpack_from("<4I", data, 56)
    class_count, class_off = struct.unpack_from("<2I", data, 96)
    if str_count > 1000000 or typ_count > 1000000 or class_count > 100000:
        raise ValueError("unreasonable DEX collection size")
    if (str_off + 4 * str_count > len(data) or
        typ_off + 4 * typ_count > len(data) or
        class_off + 32 * class_count > len(data)):
        raise ValueError("DEX table out of bounds")
    strings = []
    for i in range(str_count):
        pos = struct.unpack_from("<I", data, str_off + 4*i)[0]
        if pos >= len(data):
            raise ValueError("string data offset out of bounds")
        while True:
            if pos >= len(data):
                raise ValueError("truncated string prefix")
            ch = data[pos]
            pos += 1
            if not ch & 0x80:
                break
        end = data.find(b"\0", pos)
        if end < 0:
            raise ValueError("unterminated DEX string")
        strings.append(data[pos:end].decode("utf-8", "replace"))
    types = []
    for i in range(typ_count):
        idx = struct.unpack_from("<I", data, typ_off+4*i)[0]
        if idx >= len(strings):
            raise ValueError("type index out of bounds")
        types.append(strings[idx])
    result = set()
    for i in range(class_count):
        idx = struct.unpack_from("<I", data, class_off+32*i)[0]
        if idx >= len(types):
            raise ValueError("class index out of bounds")
        result.add(types[idx])
    return result


def verify(apk):
    with zipfile.ZipFile(apk) as z:
        definitions = class_definitions(z.read("classes.dex"))
    missing = set(REQUIRED) - definitions
    print("DEX class definitions:", len(definitions))
    for cls in REQUIRED:
        print(cls, "PRESENT" if cls in definitions else "MISSING")
    if missing:
        raise ValueError("APK missing Java class definitions")


if __name__ == "__main__":
    verify(sys.argv[1])
