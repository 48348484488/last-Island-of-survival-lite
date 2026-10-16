#!/usr/bin/env python3
"""Capture a real QEMU VNC framebuffer using unencrypted local RFB.

Uses only Python stdlib. For localhost/QEMU test guests only. Does not provide
remote VNC credentials, expose a listening socket, modify Android, or infer gameplay.
"""
import argparse
import json
import socket
import struct
import zlib
from pathlib import Path

MAX_PIXELS = 4096 * 4096
MAX_RAW_BYTES = MAX_PIXELS * 4


def read_exact(sock, count):
    if count < 0 or count > MAX_RAW_BYTES:
        raise ValueError("RFB payload exceeds safety bound")
    chunks = []
    while count:
        chunk = sock.recv(min(count, 1 << 20))
        if not chunk:
            raise EOFError("Unexpected EOF from QEMU VNC")
        chunks.append(chunk)
        count -= len(chunk)
    return b"".join(chunks)


def png_chunk(tag, data):
    return (struct.pack(">I", len(data)) + tag + data
            + struct.pack(">I", zlib.crc32(tag + data) & 0xffffffff))


def save_bgra_as_png(path, width, height, pixels):
    # We explicitly request 32bpp little-endian true color with B,G,R,0 channels.
    if len(pixels) != width * height * 4:
        raise ValueError("Frame size mismatch")
    compressed = zlib.compressobj(6)
    output = bytearray()
    colors = set()
    for y in range(height):
        raw = pixels[y * width * 4: (y + 1) * width * 4]
        row = bytearray(width * 3 + 1)
        row[0] = 0  # PNG filter None
        row[1::3] = raw[2::4]
        row[2::3] = raw[1::4]
        row[3::3] = raw[0::4]
        output.extend(compressed.compress(row))
        for x in (0, width // 4, width // 2, width * 3 // 4, width - 1):
            pos = 1 + 3 * x
            colors.add(tuple(row[pos:pos + 3]))
    output.extend(compressed.flush())
    png = (
        b"\x89PNG\r\n\x1a\n"
        + png_chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 2, 0, 0, 0))
        + png_chunk(b"IDAT", bytes(output))
        + png_chunk(b"IEND", b"")
    )
    path.write_bytes(png)
    return {"sampled_unique_rgb": len(colors), "png_bytes": len(png)}


def capture(host, port, outfile):
    if host not in ("127.0.0.1", "::1", "localhost"):
        raise ValueError("Only localhost QEMU VNC is allowed")
    with socket.create_connection((host, port), timeout=8) as sock:
        sock.settimeout(15)
        version = read_exact(sock, 12)
        if not version.startswith(b"RFB 003."):
            raise ValueError("Not a VNC RFB protocol server")
        sock.sendall(b"RFB 003.008\n")
        count = read_exact(sock, 1)[0]
        if count == 0:
            n = struct.unpack(">I", read_exact(sock, 4))[0]
            detail = read_exact(sock, min(n, 1024))
            raise RuntimeError("No VNC security modes: " + detail.decode(errors="replace"))
        security = read_exact(sock, count)
        if 1 not in security:
            raise RuntimeError("QEMU VNC did not allow local no-auth mode")
        sock.sendall(bytes([1]))  # RFB security type None
        result = struct.unpack(">I", read_exact(sock, 4))[0]
        if result != 0:
            raise RuntimeError("QEMU VNC security negotiation failed")
        sock.sendall(bytes([1]))  # Shared connection
        width, height = struct.unpack(">HH", read_exact(sock, 4))
        original_pixel_format = read_exact(sock, 16)
        name_length = struct.unpack(">I", read_exact(sock, 4))[0]
        if name_length > 4096:
            raise ValueError("Oversized VNC desktop name")
        name = read_exact(sock, name_length).decode("utf8", "replace")
        if width == 0 or height == 0 or width * height > MAX_PIXELS:
            raise ValueError("Unsafe VNC framebuffer geometry")

        # Client message type 0, padding 3, explicit RGB888 in little-endian BGRA.
        fmt = struct.pack(">BBBBHHHBBBxxx", 32, 24, 0, 1, 255, 255, 255, 16, 8, 0)
        sock.sendall(bytes([0, 0, 0, 0]) + fmt)
        # SetEncodings (type 2) RAW only (encoding 0).
        sock.sendall(struct.pack(">BBHi", 2, 0, 1, 0))
        # Request full framebuffer.
        sock.sendall(struct.pack(">BBHHHH", 3, 0, 0, 0, width, height))

        for _ in range(20):
            message = read_exact(sock, 1)[0]
            if message == 2:  # Bell notification
                continue
            if message == 3:  # ServerCutText
                read_exact(sock, 3)
                n = struct.unpack(">I", read_exact(sock, 4))[0]
                if n > 1024 * 1024:
                    raise ValueError("Oversized clipboard reply")
                read_exact(sock, n)
                continue
            if message != 0:
                raise RuntimeError(f"Unsupported server message {message}")
            _padding = read_exact(sock, 1)
            count = struct.unpack(">H", read_exact(sock, 2))[0]
            framebuffer = bytearray(width * height * 4)
            for _rect in range(count):
                x, y, w, h, enc = struct.unpack(">HHHHi", read_exact(sock, 12))
                if not 0 < w <= width or not 0 < h <= height or x + w > width or y + h > height:
                    raise ValueError("Invalid framebuffer rectangle")
                if enc != 0:
                    raise ValueError(f"Unexpected encoding {enc}; only RAW requested")
                rect = read_exact(sock, w * h * 4)
                for ry in range(h):
                    dest = ((y + ry) * width + x) * 4
                    begin = ry * w * 4
                    framebuffer[dest:dest + w * 4] = rect[begin:begin + w * 4]
            if count:
                stats = save_bgra_as_png(outfile, width, height, bytes(framebuffer))
                return {
                    "vnc_server": name,
                    "server_version": version.decode("ascii", "replace").strip(),
                    "width": width,
                    "height": height,
                    "pixel_format_requested": "BGRA little-endian 32bpp",
                    "original_pixfmt_hex": original_pixel_format.hex(),
                    "framebuffer_captured": True,
                    **stats,
                    "is_android_home_or_gameplay": "UNVERIFIED",
                }
    raise RuntimeError("No framebuffer update received")


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--host", default="127.0.0.1")
    p.add_argument("--port", type=int, default=5909)
    p.add_argument("--output", type=Path, required=True)
    p.add_argument("--report", type=Path, required=True)
    args = p.parse_args()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    result = capture(args.host, args.port, args.output)
    args.report.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")
    print(json.dumps(result, sort_keys=True))


if __name__ == "__main__":
    main()
