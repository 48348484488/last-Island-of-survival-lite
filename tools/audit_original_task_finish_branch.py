#!/usr/bin/env python3
"""Check original Oxide task-removal finish branch from archived stock Android logs.

The framework event reason describes branch behavior, never proves which
public Java method or game security routine selected it.
"""
import argparse
import json
import re
from pathlib import Path
from audit_controlled_activity_finish_node import assess as assess_control
from diagnose_original_exit import log_lines, event_ms, PACKAGE, ACTIVITY

FINISH_RE = re.compile(r'^\[(\d+),(\d+),(\d+),([^,\]]+),([^,\]]+)\]$')


def analyze(folder):
    folder = Path(folder)
    control = assess_control(folder)
    forensic = json.loads((folder/'forensic_summary.json').read_text())
    guest_pid = forensic.get('initial_activity_pid')
    start = forensic.get('initial_activity_start')
    killed = forensic.get('first_self_sigkill')
    if type(guest_pid) is not int or guest_pid <= 0 or not start or not killed:
        raise ValueError('Original Activity PID, start or self SIGKILL absent')
    rows = list(log_lines(folder/'all_buffers.txt'))
    finishes = []
    for row in rows:
        if row['tag'] != 'wm_finish_activity':
            continue
        m = FINISH_RE.fullmatch(row['msg'])
        if m:
            user, token, task, component, reason = m.groups()
            finishes.append({'timestamp': row['timestamp'], 'user': int(user),
                             'token': int(token), 'task': int(task),
                             'component': component, 'reason': reason})
    games = [x for x in finishes if PACKAGE+'/' in x['component']
             and ACTIVITY in x['component'] and start <= x['timestamp'] <= killed]
    settings = [x for x in finishes if x['component'] == 'com.android.settings/.Settings'
                and x['timestamp'] < start]
    game = min(games,key=lambda x:x['timestamp']) if games else None
    setting = max(settings,key=lambda x:x['timestamp']) if settings else None
    pause = None
    removed = None
    if game:
        task, token = game['task'], game['token']
        for row in rows:
            if row['timestamp'] < game['timestamp']:
                continue
            if row['tag'] == 'wm_pause_activity' and pause is None:
                if re.match(r'^\[\d+,'+str(token)+r',',row['msg']) and (
                    PACKAGE+'/' in row['msg'] and ACTIVITY in row['msg']
                    and 'finish' in row['msg']
                ):
                    pause = row
            if row['tag'] == 'wm_task_removed' and removed is None:
                if re.match(r'^\['+str(task)+','+str(task)+r',',row['msg']) and (
                    'ActivityRecord{'+str(token)+' ' in row['msg']
                    and PACKAGE+'/' in row['msg'] and ACTIVITY in row['msg']
                ):
                    removed = row
    finish_delta = event_ms(game['timestamp'],killed) if game else None
    removal_delta = event_ms(game['timestamp'],removed['timestamp']) if game and removed else None
    binder_ok = (control.get('status') ==
                 'CONTROLLED_SETTINGS_FINISH_BINDER_NODE_MATCHES_ORIGINAL_FINAL_CODE15'
                 and control.get('shared_binder_endpoint_node_correlated') is True
                 and control.get('original_guest_pid') == guest_pid
                 and control.get('original_kernel_sigkill_verified') is True
                 and control.get('settings_activity_finish_logged') is True)
    checks = {
        'stock_settings_and_original_same_binder_node_and_code': binder_ok,
        'settings_ordinary_app_request_reason': bool(setting and setting['reason']=='app-request'),
        'original_task_removal_finish_reason': bool(game and game['reason']=='finish-activity'),
        'finish_precedes_self_sigkill': bool(finish_delta is not None and 0 <= finish_delta <= 1000),
        'same_token_activity_paused_for_finish': bool(pause and pause['timestamp'] <= killed),
        'same_token_and_task_removed_after_finish': bool(removed and removal_delta is not None and 0 <= removal_delta <= 3000),
        'control_happened_before_original_launch': bool(setting and setting['timestamp'] < start),
        'distinct_control_and_game_tokens': bool(game and setting and game['token'] != setting['token']),
    }
    return {
        'status': ('ORIGINAL_TASK_REMOVAL_FINISH_BRANCH_CORROBORATED' if all(checks.values())
                   else 'TASK_REMOVAL_BRANCH_NOT_PROVEN'),
        'original_activity_pid': guest_pid,
        'original_finish': game,
        'settings_control_finish': setting,
        'finish_to_self_sigkill_ms': finish_delta,
        'finish_to_task_removed_ms': removal_delta,
        'checks': checks,
        'shared_binder_node': control['original_last20ms_binder_code15_nodes'][0] if binder_ok else None,
        'aosp_semantics': ('finish-activity is the removeTask branch in '
            'IActivityClientController.finishActivity(), selected for finishTask=2 '
            'or finishTask=1 when the Activity is root. Ordinary finish uses app-request.'),
        'finish_task_argument_value_observed': False,
        'public_api_finish_and_remove_task_proven': False,
        'app_internal_caller_or_security_reason_proven': False,
        'gameplay_or_esp_proven': False,
        'original_apks_modified': False,
        'limitations': [
            'Source-code branch interpretation is not an observation of Binder payload.',
            'Different Redroid framework versions could have different branches.',
            'A framework task-removal request does not reveal the internal game decision.',
        ],
    }


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('evidence_dir',type=Path)
    args=p.parse_args()
    verdict=analyze(args.evidence_dir)
    (args.evidence_dir/'original_task_removal_branch_verdict.json').write_text(
        json.dumps(verdict,indent=2)+'\n')
    print(json.dumps(verdict,indent=2))
    return 0 if verdict['status']=='ORIGINAL_TASK_REMOVAL_FINISH_BRANCH_CORROBORATED' else 1

if __name__=='__main__':
    raise SystemExit(main())
