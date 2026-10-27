p = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\viewer3d.py'
with open(p, 'rb') as f:
    data = f.read()

target = b"AXE_RUN_FWD  = 'dao_zhan_rush_front01'"
repl = b"AXE_RUN_FWD  = 'zhan_run_front01'"

if target in data:
    data = data.replace(target, repl)
    with open(p, 'wb') as f:
        f.write(data)
    print("Reverted AXE_RUN_FWD to zhan_run_front01")
else:
    print("Not found")
