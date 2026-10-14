import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from audit_original_exit_order import analyze, app_exit_entry, compare

PKG = "com.catsbit.oxidesurvivalisland"
ACT = PKG + "/cTOOT3.abKmV1.tP0Gr1.OxideActivity"


def log(ms, pid, tag, message):
    return f"10-14 13:49:{ms} {pid:5} {pid:5} I {tag:<8}: {message}"


def valid_rows(pid=1234):
    return [
        log("30.000", 200, "am_proc_start",
            f"[0,{pid},10088,{PKG},next-top-activity,{{{ACT}}}]"),
        log("31.400", pid, "Unity", "Product Name: OXIDE"),
        log("34.550", 200, "wm_finish_activity",
            f"[0,77,8,{ACT},finish-activity]"),
        log("34.551", 200, "wm_pause_activity",
            f"[0,77,{ACT},userLeaving=false,finish]"),
        log("34.553", pid, "Process", f"Sending signal. PID: {pid} SIG: 9"),
        log("34.602", 200, "am_proc_died", f"[0,{pid},{PKG},0,2]"),
        log("35.200", 200, "am_proc_start",
            f"[0,{pid+500},10088,{PKG},service,{{{PKG}/AppMeasurementJobService}}]"),
    ]


def valid_exit(pid=1234):
    return ("\n    ApplicationExitInfo #0:\n"
            f"      timestamp=2026-10-14 13:50:00.000 pid={pid+500}\n"
            f"      process={PKG} reason=1 (EXIT_SELF) status=0\n"
            "    ApplicationExitInfo #1:\n"
            f"      timestamp=2026-10-14 13:49:34.602 pid={pid}\n"
            f"      process={PKG} reason=2 (SIGNALED) subreason=0 (UNKNOWN) status=9\n")


class OriginalExitOrderProof(unittest.TestCase):
    def analyze_case(self, rows=None, exit_info=None):
        tmp = tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        folder = Path(tmp.name)
        (folder / "all_buffers.txt").write_text(
            "\n".join(valid_rows() if rows is None else rows) + "\n", encoding="utf-8")
        if exit_info is not False:
            (folder / "exit_info.txt").write_text(
                valid_exit() if exit_info is None else exit_info, encoding="utf-8")
        return analyze(folder, "16")

    def test_three_independent_pid_correlated_markers(self):
        x = self.analyze_case()
        self.assertEqual(x["classification"], "OBSERVED_FINISH_THEN_SELF_SIGKILL_STATUS9")
        self.assertEqual(x["finish_to_self_sigkill_ms"], 3)
        self.assertEqual(x["original_activity_pid"], 1234)
        self.assertEqual(x["events_ms_after_start"]["activity_paused_for_finish_ms"], 4551)
        self.assertEqual(x["events_ms_after_start"]["original_process_death_ms"], 4602)
        self.assertFalse(x["gameplay_verified"])
        self.assertEqual(x["activity_finish_requester"], "NOT_IDENTIFIED")
        self.assertFalse(x["original_apk_modified"])

    def test_later_analytics_service_exit_self_not_selected(self):
        x = self.analyze_case()
        self.assertEqual(x["application_exit_info"]["matching_entries"], 1)
        self.assertTrue(x["application_exit_info"]["signal_9_recorded"])

    def test_system_exit_wrong_original_pid_fails_closed(self):
        result = self.analyze_case(exit_info=valid_exit(pid=4444))
        self.assertEqual(result["classification"], "INCOMPLETE_OR_NONMATCHING_EXIT_EVIDENCE")

    def test_application_exit_self_not_signal_cannot_pass(self):
        result = self.analyze_case(exit_info=(
            "ApplicationExitInfo #0:\n"
            f"pid=1234 process={PKG} reason=1 (EXIT_SELF) status=0\n"))
        self.assertFalse(result["application_exit_info"]["signal_9_recorded"])
        self.assertEqual(result["classification"], "INCOMPLETE_OR_NONMATCHING_EXIT_EVIDENCE")

    def test_other_process_signalling_game_not_self_signal(self):
        rows = [r.replace(" 1234  1234 I Process", "  200   200 I Process")
                for r in valid_rows()]
        # More resilient than relying on fixed space padding.
        rows = [
            log("34.553", 200, "Process", "Sending signal. PID: 1234 SIG: 9")
            if "Sending signal." in r else r for r in rows
        ]
        result = self.analyze_case(rows=rows)
        self.assertEqual(result["classification"], "INCOMPLETE_OR_NONMATCHING_EXIT_EVIDENCE")

    def test_finish_after_signal_not_correct_order(self):
        rows = [
            r.replace("13:49:34.550", "13:49:34.570")
            if "wm_finish_activity" in r else r for r in valid_rows()
        ]
        result = self.analyze_case(rows=rows)
        self.assertFalse(result["finish_before_self_signal"])

    def test_other_activity_finish_does_not_count(self):
        rows = [
            r.replace(ACT, "com.example.other/.FakeActivity")
            if "wm_finish_activity" in r else r for r in valid_rows()
        ]
        result = self.analyze_case(rows=rows)
        self.assertEqual(result["classification"], "INCOMPLETE_OR_NONMATCHING_EXIT_EVIDENCE")

    def test_missing_exit_info_refused(self):
        with self.assertRaisesRegex(ValueError, "ApplicationExitInfo"):
            self.analyze_case(exit_info=False)

    def test_no_original_activity_launch_refused(self):
        rows = [r for r in valid_rows() if "am_proc_start" not in r]
        with self.assertRaisesRegex(ValueError, "PID/start"):
            self.analyze_case(rows=rows)

    def test_java_crash_other_pid_not_attributed_to_game(self):
        rows = valid_rows() + [log("33.000", 567, "AndroidRuntime",
                                   "FATAL EXCEPTION: unrelated service")]
        result = self.analyze_case(rows=rows)
        self.assertEqual(result["original_pid_java_fatal_markers"], 0)

    def test_actual_original_pid_java_fatal_is_visible(self):
        rows = valid_rows() + [log("33.000", 1234, "AndroidRuntime", "FATAL EXCEPTION: game")]
        result = self.analyze_case(rows=rows)
        self.assertEqual(result["original_pid_java_fatal_markers"], 1)

    def test_native_sigsegv_not_mistaken_for_own_sigkill(self):
        rows = valid_rows() + [log("33.000", 1234, "libc",
                                   "Fatal signal 11 (SIGSEGV), code 1")]
        result = self.analyze_case(rows=rows)
        self.assertEqual(result["original_pid_native_fatal_6_or_11_markers"], 1)

    def test_only_one_android_version_not_three_version_proof(self):
        result = compare({"16": self.analyze_case()})
        self.assertEqual(result["status"], "UNVERIFIED")
        self.assertFalse(result["root_cause_proven"])

    def test_three_distinct_android_versions_count(self):
        case = self.analyze_case()
        result = compare({"13": case, "15": case, "16": case})
        self.assertEqual(result["status"], "THREE_REAL_NATIVE_ANDROID_VERSIONS_CONFIRMED")
        self.assertFalse(result["requester_identified"])

    def test_wrong_exit_info_signal_status_rejected(self):
        item = app_exit_entry(valid_exit().replace("status=9", "status=11"), 1234)
        self.assertFalse(item["signal_9_recorded"])


if __name__ == "__main__":
    unittest.main()
