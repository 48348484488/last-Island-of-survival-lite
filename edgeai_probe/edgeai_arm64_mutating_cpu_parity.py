#!/usr/bin/env python3
"""Source-blind AArch64 word-memory CPU-state parity against real QEMU.

No game C or function names are read. A bounded interpreter executes only a
small explicit subset of decoded ARM64 instructions. Every instruction-state
comparison is grounded in snapshots from QEMU executing the original ELF.

The input ABI is an explicit hypothesis: void f(uint32_t board[16]). It does
NOT infer the original prototype, emulate syscalls, or reconstruct a whole game.
"""
import argparse
import hashlib
import json
import re
from pathlib import Path

from edgeai_qemu_cpu_state_differential import disassemble, VBASE

RE_REG = re.compile(r'^[wx]([0-9]|[12][0-9]|30)$')
RE_INDEXED = re.compile(r'^(w\d+), \[(x\d+), (x\d+), lsl #(\d+)\]$')
RE_IMMEDIATE = re.compile(r'^(w\d+), \[(x\d+)(?:, #(0x[0-9a-fA-F]+|\d+))?\](!?)$')
RE_TARGET = re.compile(r'^0x([0-9a-fA-F]+)$')
MASK32 = (1 << 32) - 1
MASK64 = (1 << 64) - 1
FNV_OFFSET = 14695981039346656037
FNV_PRIME = 1099511628211


def fnv_board(words):
    result = FNV_OFFSET
    for word in words:
        if not 0 <= word <= MASK32:
            raise ValueError('board word outside unsigned 32-bit')
        for k in range(4):
            result = ((result ^ ((word >> (8 * k)) & 0xff)) * FNV_PRIME) & MASK64
    return format(result, '016x')


def run_memory_interpreter(code, start, board, *, invert_condition=False, budget=4096):
    if len(board) != 16 or any(not isinstance(v, int) or not 0 <= v <= MASK32 for v in board):
        raise ValueError('expected exactly sixteen uint32 board words')
    state = list(board)
    regs = [0] * 31
    regs[0] = VBASE
    defined = {0}
    flags = None
    pc = start
    trace = []

    def get(v):
        if v.startswith('#'):
            return int(v[1:], 0)
        m = RE_REG.fullmatch(v)
        if not m:
            raise ValueError('unsupported operand '+v)
        i = int(m[1])
        if i not in defined:
            raise ValueError('read of undefined register '+v)
        return regs[i] & (MASK32 if v[0] == 'w' else MASK64)

    def put(v, value):
        m = RE_REG.fullmatch(v)
        if not m:
            raise ValueError('unsupported destination '+v)
        i = int(m[1]); regs[i] = value & (MASK32 if v[0] == 'w' else MASK64)
        defined.add(i)

    def checked_word(address):
        if address < VBASE or address > VBASE+60 or (address - VBASE) % 4:
            raise ValueError('memory access outside bounded board at '+hex(address))
        return (address - VBASE) // 4

    for _ in range(budget):
        if pc not in code:
            raise ValueError('PC outside fully decoded function '+hex(pc))
        op, args = code[pc]
        parts = [x.strip() for x in args.split(',')]
        trace.append({'pc':hex(pc),
                      'defined':{f'X{i:02d}':regs[i] for i in sorted(defined)},
                      'nzcv':flags})
        nxt = pc + 4
        if op == 'mov' and len(parts) == 2:
            put(parts[0], get(parts[1]))
        elif op in ('add', 'and') and len(parts) == 3:
            a, b = get(parts[1]), get(parts[2])
            put(parts[0], a+b if op == 'add' else a & b)
        elif op == 'cmp' and len(parts) == 2:
            width = 32 if parts[0].startswith('w') else 64
            mask = (1 << width) - 1
            a, b = get(parts[0]) & mask, get(parts[1]) & mask
            diff = (a - b) & mask
            n = (diff >> (width - 1)) & 1
            z = int(diff == 0)
            c = int(a >= b)
            v = int(bool(((a ^ b) & (a ^ diff)) & (1 << (width - 1))))
            flags = (n << 3) | (z << 2) | (c << 1) | v
        elif op in ('ldr', 'str'):
            indexed = RE_INDEXED.fullmatch(args)
            imm = RE_IMMEDIATE.fullmatch(args)
            if indexed:
                if int(indexed[4]) not in (0, 2):
                    raise ValueError('unsupported scaled memory index')
                address = get(indexed[2]) + (get(indexed[3]) << int(indexed[4]))
                dst_or_value = indexed[1]
            elif imm:
                address = get(imm[2]) + (int(imm[3], 0) if imm[3] else 0)
                dst_or_value = imm[1]
            else:
                raise ValueError('unmodeled load/store addressing '+args)
            index = checked_word(address)
            if op == 'ldr':
                put(dst_or_value, state[index])
            else:
                state[index] = get(dst_or_value) & MASK32
            if imm and imm[4] == '!':
                put(imm[2], address)
        elif op in ('b','b.eq','b.ne','b.hi','b.ls') and len(parts) == 1:
            m = RE_TARGET.fullmatch(parts[0])
            if not m:
                raise ValueError('nonliteral branch '+args)
            cond = {'b':True}
            if op != 'b':
                if flags is None:
                    raise ValueError('conditional branch without CMP')
                z, c = bool(flags & 4), bool(flags & 2)
                cond = {'b.eq':z,'b.ne':not z,'b.hi':c and not z,'b.ls':not c or z}
            take = cond[op]
            if invert_condition and op != 'b':
                take = not take
            if take:
                nxt = int(m[1], 16)
        elif op == 'ret' and args in ('', 'x30'):
            return trace, state
        else:
            raise ValueError(f'unsupported ARM64 instruction {pc:#x}: {op} {args}')
        pc = nxt
    raise ValueError('bounded interpreter instruction budget exceeded')


def compare_memory(elf, cpu_json, qemu_raw, start, stop, case, *, invert_condition=False):
    record = json.loads(Path(cpu_json).read_text())
    if hashlib.sha256(Path(qemu_raw).read_bytes()).hexdigest() != record['qemu_raw_trace_sha256']:
        raise ValueError('QEMU trace SHA256 mismatch')
    io = record['original_machine_io']
    if io['case'] != case or len(io['input_board']) != 16:
        raise ValueError('QEMU input mismatch')
    actual = record['snapshot_sequence']
    if not actual or int(actual[0]['pc'],16) != start:
        raise ValueError('QEMU did not enter selected ELF function')
    decoded = disassemble(elf, start, stop)
    sim, result_board = run_memory_interpreter(decoded,start,io['input_board'],
                                               invert_condition=invert_condition)
    qemu_ptr = int(actual[0]['regs']['X00'],16)
    checks = {'pc':0,'registers':0,'nzcv':0,'final_board_digest':0}
    mismatch = None
    for ix in range(max(len(actual),len(sim))):
        if ix >= len(actual) or ix >= len(sim):
            mismatch={'step':ix,'kind':'instruction_count','real':len(actual),'simulated':len(sim)}
            break
        a,b=actual[ix],sim[ix]
        if a['pc'] != b['pc']:
            mismatch={'step':ix,'kind':'PC','real':a['pc'],'simulated':b['pc']}
            break
        checks['pc']+=1
        for reg, expected in b['defined'].items():
            if reg not in a['regs']:
                raise ValueError('QEMU missing register '+reg)
            observed = int(a['regs'][reg], 16)
            if qemu_ptr <= observed <= qemu_ptr + 64:
                observed = VBASE + (observed-qemu_ptr)
            if observed != expected:
                mismatch={'step':ix,'kind':'register','pc':a['pc'],'register':reg,
                          'real':hex(observed),'simulated':hex(expected)}
                break
            checks['registers']+=1
        if mismatch:
            break
        if b['nzcv'] is not None:
            if 'pstate' not in a:
                raise ValueError('QEMU missing processor status')
            observed_flags=(int(a['pstate'],16)>>28)&15
            if observed_flags != b['nzcv']:
                mismatch={'step':ix,'kind':'NZCV','pc':a['pc'],
                          'real':hex(observed_flags),'simulated':hex(b['nzcv'])}
                break
            checks['nzcv']+=1
    expected_board_hash=io['board_after_fnv64']
    simulated_board_hash=fnv_board(result_board)
    if mismatch is None and expected_board_hash != simulated_board_hash:
        mismatch={'step':len(sim),'kind':'memory_hash',
                  'real':expected_board_hash,'simulated':simulated_board_hash}
    elif mismatch is None:
        checks['final_board_digest']+=1
    return {'case':case,'entry':hex(start),'binary_sha256':hashlib.sha256(Path(elf).read_bytes()).hexdigest(),
            'original_arm64_executed_in_qemu':True,'source_used':False,'game_symbols_used':False,
            'abi_hypothesis':'void(uint32_t[16])',
            'qemu_instructions':len(actual),'simulated_instructions':len(sim),
            'counts':checks,'total_checks':sum(checks.values()),
            'original_board_fnv64':expected_board_hash,
            'interpreter_board_fnv64':simulated_board_hash,
            'passed':mismatch is None,'first_mismatch':mismatch,
            'injected_bad_branch':invert_condition,
            'limitations':['QEMU instruction-state parity on finite cases, not full-program reverse engineering',
                           'FNV64 board digest can collide; sampled cases are not universal proof',
                           'Only an explicitly bounded and fail-closed ARM64 instruction subset']}


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--elf',required=True,type=Path)
    p.add_argument('--cpu',required=True,type=Path)
    p.add_argument('--raw',required=True,type=Path)
    p.add_argument('--entry',required=True,type=lambda x:int(x,0))
    p.add_argument('--stop',required=True,type=lambda x:int(x,0))
    p.add_argument('--case',required=True,type=int)
    p.add_argument('--output',required=True,type=Path)
    p.add_argument('--invert-condition',action='store_true')
    a=p.parse_args()
    report=compare_memory(a.elf,a.cpu,a.raw,a.entry,a.stop,a.case,
                          invert_condition=a.invert_condition)
    a.output.parent.mkdir(parents=True,exist_ok=True)
    a.output.write_text(json.dumps(report,indent=2)+'\n')
    print(json.dumps(report,indent=2))
    if a.invert_condition and report['passed']:
        raise SystemExit('negative control unexpectedly passed')
    if not a.invert_condition and not report['passed']:
        raise SystemExit('QEMU actual CPU state and bounded memory interpreter differ')


if __name__=='__main__':
    main()
