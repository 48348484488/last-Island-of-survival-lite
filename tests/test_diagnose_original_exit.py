import importlib.util
import tempfile
import unittest
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    'oxide_diag', Path(__file__).resolve().parents[1] / 'tools' / 'diagnose_original_exit.py')
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)


class ExitEvidenceTests(unittest.TestCase):
    def run_log(self, lines, oom=''):
        with tempfile.TemporaryDirectory() as tmp:
            p = Path(tmp)
            (p / 'all_buffers.txt').write_text('\n'.join(lines) + '\n')
            (p / 'host_oom_evidence.txt').write_text(oom)
            return module.diagnose(p)

    def test_app_self_kill_and_analytics_not_game(self):
        lines = [
            '10-13 21:25:32.961  225  254 I am_proc_start: [0,2088,10077,com.catsbit.oxidesurvivalisland,next-top-activity,{com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity}]',
            '10-13 21:25:37.589  225  883 I wm_finish_activity: [0,42,8,com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity,finish-activity]',
            '10-13 21:25:37.707 2088 2088 I Process : Sending signal. PID: 2088 SIG: 9',
            '10-13 21:25:37.768  225  947 I am_proc_died: [0,2088,com.catsbit.oxidesurvivalisland,0,2]',
            '10-13 21:25:44.970  225  254 I am_proc_start: [0,2523,10077,com.catsbit.oxidesurvivalisland,service,{com.catsbit.oxidesurvivalisland/com.google.android.gms.measurement.AppMeasurementJobService}]',
        ]
        r = self.run_log(lines)
        self.assertEqual(r['initial_activity_pid'], 2088)
        self.assertEqual(r['classification'], 'ORIGINAL_ACTIVITY_SELF_SIGKILL')
        self.assertEqual(r['finish_to_self_sigkill_ms'], 118)
        self.assertEqual(r['other_package_processes'][0]['pid'], 2523)
        self.assertTrue(r['other_package_processes'][0]['service'])
        self.assertTrue(r['exit_after_self_sigkill'])
        self.assertFalse(r['host_kernel_oom_keyword_match'])

    def test_external_signal_not_reported_as_self(self):
        lines = [
            '10-13 21:25:32.961  225  254 I am_proc_start: [0,2088,10077,com.catsbit.oxidesurvivalisland,next-top-activity,{com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity}]',
            '10-13 21:25:37.707  225  240 I Process : Sending signal. PID: 2088 SIG: 9',
        ]
        r = self.run_log(lines, 'Out of memory: killed process 2088\n')
        self.assertEqual(r['classification'], 'INITIAL_ACTIVITY_NO_SELF_SIGKILL_OBSERVED')
        self.assertIsNone(r['first_self_sigkill'])
        self.assertTrue(r['host_kernel_oom_keyword_match'])

    def test_second_launch_does_not_inherit_prior_pid(self):
        # Real Redroid A/B failure: second logcat still contains old
        # am_proc_start/finish/died events, yet a fresh process self-kills.
        lines = [
            '10-13 22:35:04.086  224  253 I am_proc_start: [0,2197,10077,com.catsbit.oxidesurvivalisland,next-top-activity,{com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity}]',
            '10-13 22:35:08.928  224  594 I wm_finish_activity: [0,1,8,com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity,finish-activity]',
            '10-13 22:35:09.026  224  594 I am_proc_died: [0,2197,com.catsbit.oxidesurvivalisland,0,2]',
            '10-13 22:35:15.776  224  253 I am_proc_start: [0,2578,10077,com.catsbit.oxidesurvivalisland,next-top-activity,{com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity}]',
            '10-13 22:35:20.380  224  594 I wm_finish_activity: [0,2,9,com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity,finish-activity]',
            '10-13 22:35:20.425 2578 2578 I Process : Sending signal. PID: 2578 SIG: 9',
            '10-13 22:35:20.483  224  594 I am_proc_died: [0,2578,com.catsbit.oxidesurvivalisland,0,2]',
        ]
        r = self.run_log(lines)
        self.assertEqual(r['initial_activity_pid'], 2578)
        self.assertEqual(r['activity_starts_observed'], 2)
        self.assertEqual(r['classification'], 'ORIGINAL_ACTIVITY_SELF_SIGKILL')
        self.assertEqual(r['finish_to_self_sigkill_ms'], 45)
        self.assertTrue(r['exit_after_self_sigkill'])
        self.assertEqual(r['activity_finish'], '10-13 22:35:20.380')

    def test_mixed_logs_with_background_service_not_a_new_activity(self):
        lines = [
            '10-13 22:35:15.776 224 253 I am_proc_start: [0,2578,10077,com.catsbit.oxidesurvivalisland,next-top-activity,{com.catsbit.oxidesurvivalisland/cTOOT3.abKmV1.tP0Gr1.OxideActivity}]',
            '10-13 22:35:20.425 2578 2578 I Process : Sending signal. PID: 2578 SIG: 9',
            '10-13 22:35:20.483 224 594 I am_proc_died: [0,2578,com.catsbit.oxidesurvivalisland,0,2]',
            '10-13 22:35:25.000 224 253 I am_proc_start: [0,3000,10077,com.catsbit.oxidesurvivalisland,service,{com.catsbit.oxidesurvivalisland/com.google.android.gms.measurement.AppMeasurementJobService}]',
        ]
        r = self.run_log(lines)
        self.assertEqual(r['initial_activity_pid'], 2578)
        self.assertEqual(r['classification'], 'ORIGINAL_ACTIVITY_SELF_SIGKILL')
        self.assertEqual(r['other_package_processes'][0]['pid'], 3000)

    def test_only_background_service_not_activity(self):
        line = '10-13 21:25:44.970  225  254 I am_proc_start: [0,2523,10077,com.catsbit.oxidesurvivalisland,service,{com.catsbit.oxidesurvivalisland/com.google.android.gms.measurement.AppMeasurementJobService}]'
        r = self.run_log([line])
        self.assertEqual(r['classification'], 'ORIGINAL_ACTIVITY_START_NOT_FOUND')
        self.assertIsNone(r['initial_activity_pid'])


if __name__ == '__main__':
    unittest.main()
