#!/usr/bin/env python3
"""Read-only original Oxide Android API permission contract and chronology.

This is a public Android SDK API mismatch, not a security-policy diagnosis.
Consumes only already-captured original logcats and aapt badging text.
Never edits, launches, or repackages original APKs.
"""
import argparse
import hashlib
import json
import re
from datetime import datetime
from pathlib import Path

PACKAGE = 'com.catsbit.oxidesurvivalisland'
ACTIVITY = PACKAGE + '/.OxideActivity'
LOG_SHA256 = {
    'android15': 'bcc57f460584553fe860fab3995f4d930b88ce9e8e6eb370ff116a9932d70c4c',
    'android16': '776a2394656e5cc802c1f267aae7b36fd4c90b295f7b9a0527ef86ec96b8f120',
}
EXPECTED_ORIGINAL_PID = {'android15': 2023, 'android16': 2108}
PERMISSION = 'android.permission.REQUEST_INSTALL_PACKAGES'
API = 'android.content.pm.PackageManager.canRequestPackageInstalls()'
LOG_ROW = re.compile(
    r'^(?P<monthday>\d\d-\d\d) (?P<clock>\d\d:\d\d:\d\d\.\d+)\s+'
    r'(?P<pid>\d+)\s+(?P<tid>\d+)\s+(?P<level>[A-Z])\s+'
    r'(?P<tag>[^:]+):\s?(?P<message>.*)$')
PROC = re.compile(r'\bStart proc (?P<pid>\d+):' + re.escape(PACKAGE) + r'/')
KILL = re.compile(r'\bSending signal\. PID: (?P<pid>\d+) SIG: 9(?:\D|$)')
EXCEPTION = 'Caused by: java.lang.SecurityException: Need to declare ' + PERMISSION + ' to call this api'
STACK_API = 'at android.app.ApplicationPackageManager.canRequestPackageInstalls('


def _time(row):
    return datetime.strptime('2026-' + row['monthday'] + ' ' + row['clock'],
                             '%Y-%m-%d %H:%M:%S.%f')


def _milliseconds(a, b):
    return round((b - a).total_seconds() * 1000, 3)


def _manifest(badging):
    """Match exact original package/SDK fields and parse permission declarations only."""
    lines = badging.decode('utf8', errors='replace').splitlines()
    if not any(re.match(r"^package: name='" + re.escape(PACKAGE)
                        + r"' versionCode='11966' versionName='1\.15\.11966'", l)
               for l in lines):
        return None
    target = [int(m.group(1)) for line in lines
              if (m := re.fullmatch(r"targetSdkVersion:'(\d+)'", line))]
    if target != [36]:
        return None
    declared = [m.group(1) for line in lines if
                (m := re.match(r"^uses-permission(?:-sdk-\d+)?: name='([^']+)'", line))]
    return {'target_sdk': target[0], 'requested_install_permission_declared':
            PERMISSION in declared}


def analyze(log_bytes, badging_bytes, label):
    digest = hashlib.sha256(log_bytes).hexdigest()
    if label not in LOG_SHA256 or digest != LOG_SHA256[label]:
        return {'status': 'BLOCKED_LOGCAT_PROVENANCE', 'verified': False}
    manifest = _manifest(badging_bytes)
    if manifest is None:
        return {'status': 'BLOCKED_MANIFEST_METADATA', 'verified': False}
    expected = EXPECTED_ORIGINAL_PID[label]
    started = None
    pid = None
    event_times = {}
    errors = []
    stack_calls = []
    job_pids = set()
    for line in log_bytes.decode('utf8', errors='replace').splitlines():
        row = LOG_ROW.match(line)
        if not row:
            continue
        message, at = row['message'], _time(row)
        row_pid, row_tid = int(row['pid']), int(row['tid'])
        if started is None and 'START u0' in message and 'cmp=' + ACTIVITY in message:
            started = at
            event_times['original_activity_started'] = at
        process = PROC.search(message)
        if started is not None and process:
            proc_id = int(process['pid'])
            if pid is None:
                pid = proc_id
                event_times['original_process_started'] = at
            elif (proc_id != pid and 'for service' in message and
                  'AppMeasurementJobService' in message):
                job_pids.add(proc_id)
        if row_pid != expected or pid != expected or started is None:
            continue
        if row['tag'].strip() == 'System.err':
            if message == EXCEPTION:
                errors.append({'at': at, 'tid': row_tid})
            elif STACK_API in message:
                stack_calls.append({'at': at, 'tid': row_tid})
        if row['tag'].strip() == 'Unity':
            if 'Scripting Backend' in message and "'il2cpp'" in message:
                event_times.setdefault('unity_il2cpp_identified', at)
            if 'Product Name: OXIDE' in message:
                event_times.setdefault('unity_product_confirmed', at)
        if (row['tag'].strip() == 'Process' and row_pid == row_tid == expected
                and (kill := KILL.search(message)) and int(kill['pid']) == expected):
            event_times.setdefault('self_sigkill_recorded', at)
    matched = next((e for e in errors for stack in stack_calls
                    if e['tid'] == stack['tid'] and abs(_milliseconds(e['at'], stack['at'])) <= 5), None)
    chronological = bool(pid == expected and matched and all(
        name in event_times for name in (
            'original_activity_started', 'unity_il2cpp_identified',
            'unity_product_confirmed', 'self_sigkill_recorded'))
        and event_times['original_activity_started'] <= matched['at']
        < event_times['unity_il2cpp_identified']
        < event_times['unity_product_confirmed']
        < event_times['self_sigkill_recorded'])
    verified = bool(chronological and not manifest['requested_install_permission_declared'])
    secondary_exceptions = []
    for line in log_bytes.decode('utf8', errors='replace').splitlines():
        row = LOG_ROW.match(line)
        if (row and row['tag'].strip() == 'System.err' and row['message'] == EXCEPTION
                and int(row['pid']) in job_pids and
                'self_sigkill_recorded' in event_times and
                _time(row) > event_times['self_sigkill_recorded']):
            secondary_exceptions.append(int(row['pid']))
    return {
        'status': ('ORIGINAL_UNDECLARED_INSTALL_PERMISSION_API_EXCEPTION_CONFIRMED'
                   if verified else 'ORIGINAL_PERMISSION_API_EVIDENCE_INCOMPLETE'),
        'verified': verified,
        'original_logcat_sha256': digest,
        'original_pid': pid,
        'target_sdk': manifest['target_sdk'],
        'request_install_packages_declared': manifest['requested_install_permission_declared'],
        'observed_api': API if matched else None,
        'exception_observed_in_original_pid': matched is not None,
        'exception_to_unity_il2cpp_ms': _milliseconds(
            matched['at'], event_times['unity_il2cpp_identified'])
            if matched and 'unity_il2cpp_identified' in event_times else None,
        'exception_to_original_sigkill_ms': _milliseconds(
            matched['at'], event_times['self_sigkill_recorded'])
            if matched and 'self_sigkill_recorded' in event_times else None,
        'same_permission_exception_repeated_in_later_service_process':
            bool(secondary_exceptions),
        'secondary_process_ids': sorted(set(secondary_exceptions)),
        'java_uncaught_exception_proven': False,
        'exception_caused_original_sigkill': 'NOT_ESTABLISHED',
        'security_threat_alert_caused_by_exception': 'NOT_ESTABLISHED',
        'original_apk_modified': False,
        'original_gameplay_verified': False,
    }


def compare(a15_log, a15_badging, a16_log, a16_badging):
    runs = {
        'android15': analyze(a15_log, a15_badging, 'android15'),
        'android16': analyze(a16_log, a16_badging, 'android16'),
    }
    verified = all(value['verified'] for value in runs.values())
    return {'status': ('SHARED_ORIGINAL_PERMISSION_API_MISMATCH_OBSERVED'
                       if verified else 'ORIGINAL_PERMISSION_API_COMPARISON_INCOMPLETE'),
            'verified': verified, 'runs': runs,
            'permission_grant_or_apk_edit_proposed': False,
            'cause_of_exit': 'UNKNOWN', 'gameplay_verified': False}


def main():
    p = argparse.ArgumentParser()
    p.add_argument('--a15-log', type=Path, required=True)
    p.add_argument('--a15-badging', type=Path, required=True)
    p.add_argument('--a16-log', type=Path, required=True)
    p.add_argument('--a16-badging', type=Path, required=True)
    p.add_argument('--output', type=Path, required=True)
    args = p.parse_args()
    report = compare(args.a15_log.read_bytes(), args.a15_badging.read_bytes(),
                     args.a16_log.read_bytes(), args.a16_badging.read_bytes())
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2, sort_keys=True) + '\n')
    print(json.dumps(report, sort_keys=True))
    return 0 if report['verified'] else 2


if __name__ == '__main__':
    raise SystemExit(main())
