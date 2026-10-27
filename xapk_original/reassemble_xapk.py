import os, sys

HERE = os.path.dirname(os.path.abspath(__file__))
OUTPUT_XAPK = os.path.join(HERE, 'Last_Island_of_Survival_LITE_7.3.xapk')

def reassemble():
    parts = sorted([f for f in os.listdir(HERE) if f.startswith('Last_Island_LITE_7.3.xapk.part')])
    if not parts:
        print('No part files found!')
        sys.exit(1)
    
    print(f'Reassembling {len(parts)} parts into {OUTPUT_XAPK}...')
    total_bytes = 0
    with open(OUTPUT_XAPK, 'wb') as f_out:
        for p in parts:
            part_path = os.path.join(HERE, p)
            with open(part_path, 'rb') as f_in:
                chunk = f_in.read()
                f_out.write(chunk)
                total_bytes += len(chunk)
            print(f'  Merged {p} ({len(chunk)/(1024*1024):.1f} MB)')

    print(f'Success! Reassembled file: {OUTPUT_XAPK} ({total_bytes / (1024*1024):.2f} MB)')

if __name__ == '__main__':
    reassemble()
