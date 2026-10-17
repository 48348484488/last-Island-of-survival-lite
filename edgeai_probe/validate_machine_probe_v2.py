#!/usr/bin/env python3
"""Validate actual AArch64 original-ELF call records, not generated C++.

Passes only if every one of 11 SHA-pinned entrypoint/ABI roles produced
2000 per-case rolling digests, in order, plus one identical final digest.
No original game source is accessed or used.
"""
import argparse, hashlib, json, re
from pathlib import Path
CASE=re.compile(r'^ARM64_CASE slot=(\d+) case=(\d+) fnv64=([0-9a-f]{16})$')
FINAL=re.compile(r'^ARM64_REAL_ELF slot=(\d+) addr=0x([0-9a-f]{16}) role=(\d+) calls=(\d+) fnv64=([0-9a-f]{16})$')


def verify(elf, manifest, records, output, rounds=2000):
    original=Path(elf).read_bytes()
    cfg=json.loads(Path(manifest).read_text())
    if hashlib.sha256(original).hexdigest()!=cfg['elf_sha256']:
        raise ValueError('ELF differs from manifest')
    lines=Path(records).read_text().splitlines()
    entries=cfg['entries']
    expected=len(entries)*(rounds+1)
    if len(lines)!=expected:
        raise ValueError(f'expected {expected} records, found {len(lines)}')
    summaries=[]
    last_offsets=[]
    at=0
    for slot,desc in enumerate(entries):
        end_digest=None
        for case in range(rounds):
            match=CASE.fullmatch(lines[at])
            if not match:raise ValueError(f'bad/unsupported record {at}: {lines[at][:100]}')
            s,i,h=match.groups()
            if int(s)!=slot or int(i)!=case:
                raise ValueError(f'case index disagreement slot={slot} case={case}')
            end_digest=h
            at+=1
        match=FINAL.fullmatch(lines[at])
        if not match:raise ValueError('missing actual machine final line')
        s,addr,role,cases,h=match.groups()
        if int(s)!=slot or int(addr,16)!=int(desc['address'],16):
            raise ValueError('wrong physical machine entry executed')
        if int(role)!=cfg['roles'][desc['role']] or int(cases)!=rounds or h!=end_digest:
            raise ValueError('cumulative digest discrepancy')
        summaries.append({'address':desc['address'],'role':desc['role'],'cases':rounds,'fnv64':h})
        last_offsets.append(at);at+=1
    report={'test':'Actual ARM64 original game machine instructions under QEMU',
        'unmodified_elf_sha256':cfg['elf_sha256'], 'emulator':'QEMU user-mode ARM64',
        'source_used_in_runtime_reference':False,'original_game_symbols_used':False,
        'game_elf_executed':True,'abi_roles_from_blind_lifter':True,
        'functions_executed':len(summaries),'cases_executed':len(summaries)*rounds,
        'individual_case_digests_saved':len(summaries)*rounds,
        'machine_reference':summaries,
        'strict_case_order_verified':True,
        'run_data_sha256':hashlib.sha256(Path(records).read_bytes()).hexdigest(),
        'edgeai_cpp_comparison_completed':False,
        'notes':'Only ARM64-original outputs recorded; C++ comparison is a separate step'}
    Path(output).write_text(json.dumps(report,indent=2)+'\n')
    return report


def main():
    p=argparse.ArgumentParser(description=__doc__)
    for name in ('elf','manifest','records','output'):
        p.add_argument('--'+name,type=Path,required=True)
    args=p.parse_args()
    out=verify(args.elf,args.manifest,args.records,args.output)
    print(json.dumps({k:out[k] for k in ('functions_executed','cases_executed','individual_case_digests_saved','run_data_sha256')},indent=2))


if __name__=='__main__':main()
