"""Prevent legacy goal automation from re-downloading the already-failed original APK.

This is a static invariant, not a runtime or gameplay test.
"""
import re
import unittest
from pathlib import Path

WORKFLOW = (Path(__file__).resolve().parents[1]
            / ".github/workflows/oxide-goal-forensics.yml")


class NoExpensiveOriginalRetryTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.script = WORKFLOW.read_text(encoding="utf8")

    def test_manual_only_goal_dispatch(self):
        self.assertIn("  workflow_dispatch:", self.script)
        self.assertNotIn("  workflow_run:", self.script)
        self.assertNotIn("  schedule:", self.script)

    def test_actions_permission_is_read_only(self):
        self.assertRegex(self.script, r"(?m)^  actions: read$")
        self.assertNotIn("actions: write", self.script)

    def test_no_expensive_cross_workflow_dispatch(self):
        for forbidden in ("createWorkflowDispatch", "gh workflow run",
                          "actions/trigger-workflow", "workflow_id: 'oxide-115-arm64-runtime.yml'"):
            with self.subTest(forbidden=forbidden):
                self.assertNotIn(forbidden, self.script)

    def test_records_known_redroid_failures_without_retry(self):
        for marker in ("'FORENSIC_APP_EXIT'", "#38789434979", "#38787025289",
                       "No automatic Android/APK rerun", "REVIEW_REAL_MENU"):
            with self.subTest(marker=marker):
                self.assertIn(marker, self.script)

    def test_issue_comment_idempotency_remains(self):
        self.assertIn("oxide-goals-runtime:", self.script)
        self.assertIn("comments.some(", self.script)
        self.assertIn("github.rest.issues.createComment(", self.script)


if __name__ == "__main__":
    unittest.main()
