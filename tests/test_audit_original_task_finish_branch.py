import json
import sys
import unittest
from pathlib import Path

sys.path.insert(0,str(Path(__file__).resolve().parents[1]/'tools'))
from audit_original_task_finish_branch import analyze
from test_audit_controlled_activity_finish_node import Tests as ExistingFixture

GAME = 'com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity'


class TaskFinishTests(unittest.TestCase):
    def case(self):
        root=ExistingFixture.fixture(self)
        f=root/'forensic_summary.json'
        data=json.loads(f.read_text())
        data['initial_activity_start']='10-14 22:05:45.000'
        f.write_text(json.dumps(data))
        p=root/'all_buffers.txt'
        p.write_text(p.read_text()+
          '10-14 22:05:50.371 320 700 I wm_finish_activity: [0,249525552,9,'+GAME+',finish-activity]\n'+
          '10-14 22:05:50.372 320 700 I wm_pause_activity: [0,249525552,'+GAME+',userLeaving=false,finish]\n'+
          '10-14 22:05:50.900 320 700 I wm_task_removed: [9,9,0,removeChild, last child = ActivityRecord{249525552 u0 '+GAME+' t-1 f}} in Task{aaa #9}\n')
        return root

    def change(self,folder,a,b):
        p=folder/'all_buffers.txt'
        assert a in p.read_text(),a
        p.write_text(p.read_text().replace(a,b))

    def test_same_run_real_structure_proven(self):
        x=analyze(self.case())
        self.assertEqual(x['status'],'ORIGINAL_TASK_REMOVAL_FINISH_BRANCH_CORROBORATED')
        self.assertEqual(x['finish_to_self_sigkill_ms'],2)
        self.assertEqual(x['finish_to_task_removed_ms'],529)
        self.assertFalse(x['public_api_finish_and_remove_task_proven'])
        self.assertFalse(x['app_internal_caller_or_security_reason_proven'])

    def test_regular_app_request_does_not_count_as_task_removal(self):
        r=self.case();self.change(r,GAME+',finish-activity]',GAME+',app-request]')
        self.assertFalse(analyze(r)['checks']['original_task_removal_finish_reason'])

    def test_settings_control_must_use_app_request(self):
        r=self.case();self.change(r,'com.android.settings/.Settings,app-request]',
                                    'com.android.settings/.Settings,finish-activity]')
        self.assertFalse(analyze(r)['checks']['settings_ordinary_app_request_reason'])

    def test_wrong_task_id_rejected(self):
        r=self.case();self.change(r,'wm_task_removed: [9,9,0,','wm_task_removed: [8,8,0,')
        self.assertFalse(analyze(r)['checks']['same_token_and_task_removed_after_finish'])

    def test_wrong_activity_token_in_removal_rejected(self):
        r=self.case();self.change(r,'ActivityRecord{249525552 u0','ActivityRecord{555 u0')
        self.assertFalse(analyze(r)['checks']['same_token_and_task_removed_after_finish'])

    def test_unrelated_component_does_not_prove_game_finish(self):
        r=self.case();self.change(r,GAME+',finish-activity]', 'fake.pkg/.Main,finish-activity]')
        self.assertFalse(analyze(r)['checks']['original_task_removal_finish_reason'])

    def test_wrong_original_guest_pid_rejected(self):
        r=self.case();p=r/'forensic_summary.json';d=json.loads(p.read_text())
        d['initial_activity_pid']=3212;p.write_text(json.dumps(d))
        self.assertFalse(analyze(r)['checks']['stock_settings_and_original_same_binder_node_and_code'])

    def test_other_pause_token_rejected(self):
        r=self.case();self.change(r,'wm_pause_activity: [0,249525552,',
                                   'wm_pause_activity: [0,999999,')
        self.assertFalse(analyze(r)['checks']['same_token_activity_paused_for_finish'])

    def test_game_finish_after_sigkill_rejected(self):
        r=self.case();self.change(r,'22:05:50.371 320 700 I wm_finish_activity:',
                                   '22:05:51.371 320 700 I wm_finish_activity:')
        self.assertFalse(analyze(r)['checks']['original_task_removal_finish_reason'])

    def test_missing_finish_event_rejected(self):
        r=self.case();p=r/'all_buffers.txt'
        p.write_text('\n'.join(x for x in p.read_text().splitlines()
                if 'wm_finish_activity:' not in x or GAME not in x)+'\n')
        self.assertEqual(analyze(r)['status'],'TASK_REMOVAL_BRANCH_NOT_PROVEN')

    def test_control_and_game_tokens_distinct(self):
        x=analyze(self.case())
        self.assertEqual(x['settings_control_finish']['token'],236818407)
        self.assertEqual(x['original_finish']['token'],249525552)


if __name__=='__main__':
    unittest.main()
