p = r'C:\Users\Admin\.gemini\antigravity\scratch\last-day-rules-survival\cpp_game\skinned_character.py'
with open(p, 'rb') as f:
    data = f.read()

target = b"glRotatef(-90, 0, 1, 0)"
repl = b"glRotatef(90, 0, 1, 0)"

if target in data:
    data = data.replace(target, repl)
    with open(p, 'wb') as f:
        f.write(data)
    print("Successfully changed axe rotation to glRotatef(90, 0, 1, 0)!")
else:
    print("Target not found or already changed")
