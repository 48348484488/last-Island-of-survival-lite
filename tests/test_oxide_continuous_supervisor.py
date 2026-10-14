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
        self.assertIsNone(m.old_run(run("in_progress", when="2026-10-14T14:30:00Z"), now))
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

if __name__ == "__main__":
    unittest.main()
