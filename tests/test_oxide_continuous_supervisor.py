import datetime as dt
import importlib.util
import unittest
from pathlib import Path

module_path = Path(__file__).resolve().parents[1] / "tools" / "oxide_continuous_supervisor.py"
spec = importlib.util.spec_from_file_location("oxide_supervisor", module_path)
m = importlib.util.module_from_spec(spec)
spec.loader.exec_module(m)

def run(status="completed", conclusion="success", when="2026-10-14T14:00:00Z"):
    return {
        "id": 12345, "run_attempt": 1, "head_branch": "main",
        "head_repository": {"full_name": m.REPO},
        "path": ".github/workflows/oxide-kernel-signal-replay.yml",
        "status": status, "conclusion": conclusion,
        "run_started_at": when, "created_at": when,
    }

class FakeAPI:
    def __init__(self):
        self.comments = []
        self.calls = []
    def record(self, issue, marker, body):
        self.comments.append((issue, marker, body))
        return True
    def api(self, endpoint, method="GET", payload=None):
        self.calls.append((endpoint, method, payload))
        if endpoint == "/issues?state=all&per_page=50":
            return [{"number": 1, "state": "closed"},
                    {"number": 2, "state": "closed"},
                    {"number": 3, "state": "open"}]
        if endpoint == "/issues/1":
            return {"state": getattr(self, "issue_state", "open")}
        if endpoint.endswith("/comments?per_page=100"):
            return []
        if endpoint.startswith("/actions/runs?"):
            return {"workflow_runs": []}
        if method == "POST":
            return {}
        raise AssertionError(endpoint)

class SupervisorTests(unittest.TestCase):
    def test_known_run(self):
        self.assertEqual(m.target(run()), 1)
    def test_unknown_branch_fails_closed(self):
        x = run()
        x["head_branch"] = "untrusted"
        self.assertIsNone(m.target(x))
    def test_unknown_repository_fails_closed(self):
        x = run()
        x["head_repository"]["full_name"] = "other/repository"
        self.assertIsNone(m.target(x))
    def test_duration_threshold(self):
        now = dt.datetime(2026, 10, 14, 14, 41, tzinfo=dt.timezone.utc)
        self.assertIsNotNone(m.old_run(run("in_progress"), now))
        self.assertIsNone(m.old_run(run("in_progress", when="2026-10-14T14:36:00Z"), now))
    def test_queued_threshold(self):
        now = dt.datetime(2026, 10, 14, 14, 26, tzinfo=dt.timezone.utc)
        self.assertIsNotNone(m.old_run(run("queued"), now))
        self.assertIsNone(m.old_run(run("completed"), now))
    def test_completed_result_not_claimed_as_gameplay(self):
        api = FakeAPI()
        m.complete(api, {"action": "completed", "workflow_run": run()})
        self.assertEqual(api.comments[0][0], 1)
        self.assertIn("nao prova gameplay", api.comments[0][2])
    def test_failed_workflow_no_automatic_retry(self):
        api = FakeAPI()
        m.complete(api, {"action": "completed", "workflow_run": run(conclusion="failure")})
        self.assertIn("Nao reiniciar automaticamente", api.comments[0][2])
        self.assertEqual(api.calls, [])
    def test_issues_closure_dispatches_next_existing_workflow(self):
        api = FakeAPI()
        event = {"action": "closed", "repository": {"full_name": m.REPO},
                 "issue": {"number": 1, "state": "closed"}}
        m.closed_issue(api, event)
        self.assertTrue(any("/dispatches" in x[0] and x[1] == "POST" for x in api.calls))
        self.assertEqual(api.comments[0][0], 3)
    def test_no_skipping_open_prerequisite(self):
        self.assertIsNone(m.plan_next(2, {1: "open", 2: "closed", 3: "open"}))
    def test_closed_workflow_not_stale(self):
        now = dt.datetime.now(dt.timezone.utc)
        self.assertIsNone(m.old_run(run("completed"), now))

    def test_lightweight_sequence_is_acyclic(self):
        self.assertEqual(
            m.lightweight_handoff("oxide-arm64-signal-observability.yml", "open"),
            "oxide-kernel-signal-replay.yml")
        self.assertEqual(
            m.lightweight_handoff("oxide-kernel-signal-replay.yml", "open"),
            "oxide-original-launch-milestones.yml")
        self.assertIsNone(m.lightweight_handoff("oxide-original-launch-milestones.yml", "open"))
        self.assertIsNone(m.lightweight_handoff("redroid-original-smoke.yml", "open"))

    def test_followup_starts_only_on_success(self):
        api = FakeAPI()
        x = run()
        m.complete(api, {"action": "completed", "workflow_run": x})
        calls = [c for c in api.calls if c[1] == "POST"]
        self.assertEqual(len(calls), 1)
        self.assertIn("oxide-original-launch-milestones.yml", calls[0][0])
        self.assertIn("oxide-auto:followup:", api.comments[-1][1])

    def test_failed_run_does_not_chain(self):
        api = FakeAPI()
        m.complete(api, {"action": "completed", "workflow_run": run(conclusion="failure")})
        self.assertFalse(any(c[1] == "POST" for c in api.calls))

    def test_handoff_blocks_closed_m1(self):
        api = FakeAPI()
        api.issue_state = "closed"
        m.complete(api, {"action": "completed", "workflow_run": run()})
        self.assertFalse(any(c[1] == "POST" for c in api.calls))

    def test_handoff_rejects_untrusted_source(self):
        api = FakeAPI()
        x = run()
        x["path"] = ".github/workflows/redroid-original-smoke.yml"
        m.complete(api, {"action": "completed", "workflow_run": x})
        self.assertFalse(any(c[1] == "POST" for c in api.calls))

    def test_handoff_idempotence_marker(self):
        api = FakeAPI()
        x = run()
        marker = "oxide-auto:followup:12345:1:oxide-original-launch-milestones.yml"
        original = api.api
        def custom(endpoint, method="GET", payload=None):
            if endpoint == "/issues/1/comments?per_page=100":
                return [{"body": "<!-- " + marker + " -->"}]
            return original(endpoint, method, payload)
        api.api = custom
        m.complete(api, {"action": "completed", "workflow_run": x})
        self.assertFalse(any(c[1] == "POST" for c in api.calls))

if __name__ == "__main__":
    unittest.main()
