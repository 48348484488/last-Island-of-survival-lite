p = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\viewer3d.py'
with open(p, 'r', encoding='utf-8') as f:
    text = f.read()

target = "AXE_RUN_FWD  = 'zhan_run_front01'"
repl = "AXE_RUN_FWD  = 'dao_zhan_rush_front01'"

if target in text:
    text = text.replace(target, repl)
    with open(p, 'w', encoding='utf-8') as f:
        f.write(text)
    print("Successfully replaced AXE_RUN_FWD with dao_zhan_rush_front01")
else:
    print("Target already replaced or not found")
