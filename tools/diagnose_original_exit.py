#!/usr/bin/env python3
"""Non-invasive evidence summary for unmodified Oxide Android smoke artifacts.

No game injection, patching, security manipulation, or private-data collection.
"""
import argparse
import json
import re
from datetime import datetime
from pathlib import Path

PACKAGE = 'com.catsbit.oxidesurvivalisland'
ACTIVITY = 'OxideActivity'
LOG_RE = re.compile(r'^(?P<date>\d\d-\d\d)\s+(?P<clock>\d\d:\d\d:\d\d\.\d{3})\s+(?P<pid>\d+)\s+(?P<tid>\d+)\s+(?P<level>\w)\s+(?P<tag>[^:]+?):\s*(?P<msg>.*)$')
SIG_RE = re.compile(r'\bSending signal\. PID:\s*(\d+)\s+SIG:\s*(\d+)')
PID_RE = re.compile(r'^\[\d+,(\d+),')


def log_lines(path):
    for line in path.read_text(encoding='utf-8', errors='replace').splitlines():
        match = LOG_RE.match(line)
        if match:
            row = match.groupdict()
            row['pid'] = int(row['pid'])
            row['tid'] = int(row['tid'])
            row['tag'] = row['tag'].strip()
            row['timestamp'] = row['date'] + ' ' + row['clock']
            row['raw'] = line
            yield row


def event_ms(a, b):
    fmt = '%Y-%m-%d %H:%M:%S.%f'
    return round((datetime.strptime('2000-'+b, fmt) - datetime.strptime('2000-'+a, fmt)).total_seconds() * 1000)


def diagnose(directory):
    directory = Path(directory)
    logfile = directory / 'all_buffers.txt'
    if not logfile.is_file():
        raise FileNotFoundError(f'Missing evidence: {logfile}')
    rows = list(log_lines(logfile))
    starts = []
    for x in rows:
        if x['tag'] == 'am_proc_start' and PACKAGE in x['msg'] and f'{PACKAGE},next-top-activity' in x['msg'] and ACTIVITY in x['msg']:
            m = PID_RE.search(x['msg'])
            if m:
                starts.append({**x, 'started_pid': int(m.group(1))})
    initial = starts[0] if starts else None
    pid = initial['started_pid'] if initial else None
    signals, finishes, deaths, other_processes = [], [], [], []
    for x in rows:
        if x['tag'] == 'Process':
            m = SIG_RE.search(x['msg'])
            if m and int(m.group(2)) == 9 and pid is not None and int(m.group(1)) == pid:
                signals.append({'timestamp': x['timestamp'], 'emitter_pid': x['pid'], 'target_pid': pid,
                                'self_directed': x['pid'] == pid, 'raw': x['raw']})
        if x['tag'] == 'wm_finish_activity' and PACKAGE in x['msg'] and ACTIVITY in x['msg']:
            finishes.append({'timestamp': x['timestamp'], 'raw': x['raw']})
        if x['tag'] == 'am_proc_died' and pid is not None and re.search(r'^\[\d+,'+str(pid)+r',', x['msg']):
            deaths.append({'timestamp': x['timestamp'], 'raw': x['raw']})
        if x['tag'] == 'am_proc_start' and pid is not None and PACKAGE in x['msg']:
            m = PID_RE.search(x['msg'])
            if m and int(m.group(1)) != pid:
                other_processes.append({'pid': int(m.group(1)), 'timestamp': x['timestamp'],
                                        'service': 'AppMeasurementJobService' in x['msg'], 'raw': x['raw']})
    self_kills = [s for s in signals if s['self_directed']]
    delta = None
    if finishes and self_kills:
        delta = event_ms(finishes[0]['timestamp'], self_kills[0]['timestamp'])
    oom = directory / 'host_oom_evidence.txt'
    oom_observed = bool(oom.is_file() and oom.read_text(errors='replace').strip())
    # Negative findings are scoped to the collected kernel tail, not the entire system.
    result = {
        'classification': ('ORIGINAL_ACTIVITY_SELF_SIGKILL' if self_kills else
                           'INITIAL_ACTIVITY_NO_SELF_SIGKILL_OBSERVED' if initial else
                           'ORIGINAL_ACTIVITY_START_NOT_FOUND'),
        'initial_activity_pid': pid,
        'initial_activity_start': initial['timestamp'] if initial else None,
        'activity_finish': finishes[0]['timestamp'] if finishes else None,
        'first_self_sigkill': self_kills[0]['timestamp'] if self_kills else None,
        'finish_to_self_sigkill_ms': delta,
        'exit_after_self_sigkill': bool(deaths and self_kills and event_ms(self_kills[0]['timestamp'], deaths[0]['timestamp']) >= 0),
        'death_event': deaths[0]['timestamp'] if deaths else None,
        'other_package_processes': other_processes,
        'host_kernel_oom_keyword_match': oom_observed,
        'evidence': {'start': initial['raw'] if initial else None,
                     'finish': finishes[0]['raw'] if finishes else None,
                     'sigkill': self_kills[0]['raw'] if self_kills else None,
                     'death': deaths[0]['raw'] if deaths else None},
        'limitations': [
            'Matching emitter and target PID supports self-directed SIGKILL, not the specific security check that triggered it.',
            'No OOM matches in a kernel-log tail cannot rule out every OOM condition.',
            'Process existence or an analytics service cannot establish a playable multiplayer game or working ESP.',
            'Screenshot contents and game-security reason are not inferred from logcat.',
        ],
    }
    return result


def markdown(r):
    def t(v):
        return 'not observed' if v is None else str(v)
    lines = ['# Original Oxide ARM64 process exit: evidence audit', '',
             f"- Classification: **{r['classification']}**",
             f"- Initial Activity PID: {t(r['initial_activity_pid'])}",
             f"- Activity process start: {t(r['initial_activity_start'])}",
             f"- Activity finish event: {t(r['activity_finish'])}",
             f"- First self-directed SIGKILL log: {t(r['first_self_sigkill'])}",
             f"- Finish-to-signal interval: {t(r['finish_to_self_sigkill_ms'])} ms",
             f"- Activity process death: {t(r['death_event'])}",
             f"- Host kernel OOM keyword match: {r['host_kernel_oom_keyword_match']}",
             f"- Later package processes (NOT proof of gameplay): {len(r['other_package_processes'])}", '',
             '## Evidence lines', '']
    for v in r['evidence'].values():
        if v:
            lines.append('    ' + v.strip())
    lines += ['', '## Interpretation boundaries', '']
    lines += ['- ' + s for s in r['limitations']]
    return '\n'.join(lines) + '\n'


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('artifact_dir', type=Path)
    args = p.parse_args()
    result = diagnose(args.artifact_dir)
    (args.artifact_dir / 'forensic_summary.json').write_text(json.dumps(result, indent=2, ensure_ascii=False) + '\n')
    (args.artifact_dir / 'forensic_summary.md').write_text(markdown(result))
    print(markdown(result))


if __name__ == '__main__':
    main()
