import importlib.util
from pathlib import Path
import unittest

spec = importlib.util.spec_from_file_location(
    "grants", Path(__file__).resolve().parents[1] / "tools/compare_android_grants.py")
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)

def item(p, s):
    return {"name": p, "grant": s}

class GrantTests(unittest.TestCase):
    def test_permissions_really_changed_but_original_still_exits(self):
        b = {"requested_permission_status": [item(p, "DENIED") for p in module.PERMS],
             "game_termination": "ORIGINAL_ACTIVITY_SELF_SIGKILL"}
        a = {"requested_permission_status": [item(p, "GRANTED") for p in module.PERMS],
             "game_termination": "ORIGINAL_ACTIVITY_SELF_SIGKILL"}
        r = module.compare(b,a,{p:"OK" for p in module.PERMS})
        self.assertEqual(r["status"],"PERMISSIONS_GRANTED_BUT_SELF_SIGKILL_PERSISTS")
        self.assertEqual(r["grant_changes"],4)
        self.assertFalse(r["gameplay_proven"])
    def test_unaccepted_grants_are_not_inferred(self):
        b = {"requested_permission_status": [item(p, "DENIED") for p in module.PERMS]}
        a = {"requested_permission_status": [item(p, "DENIED") for p in module.PERMS]}
        r=module.compare(b,a,{p:"ERROR" for p in module.PERMS})
        self.assertEqual(r["status"],"NO_NEW_EFFECTIVE_PERMISSIONS")
        self.assertEqual(r["grant_changes"],0)
    def test_changed_exit_does_not_prove_gameplay(self):
        p=module.PERMS[0]
        b={"requested_permission_status":[item(p,"DENIED")],"game_termination":"ORIGINAL_ACTIVITY_SELF_SIGKILL"}
        a={"requested_permission_status":[item(p,"GRANTED")],"game_termination":"INITIAL_ACTIVITY_NO_SELF_SIGKILL_OBSERVED"}
        r=module.compare(b,a,{p:"OK"})
        self.assertEqual(r["status"],"TERMINATION_CHANGED_REQUIRES_GAMEPLAY_VALIDATION")
        self.assertFalse(r["player_positions_proven"])
    def test_unknown_grants_not_treated_as_success(self):
        r=module.compare({"requested_permission_status":[]},
                         {"requested_permission_status":[]}, {})
        self.assertEqual(r["grant_changes"],0)

if __name__ == "__main__":
    unittest.main()
