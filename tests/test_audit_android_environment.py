import importlib.util
import unittest
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    "env_audit", Path(__file__).resolve().parents[1] / "tools" / "audit_android_environment.py")
audit = importlib.util.module_from_spec(spec)
spec.loader.exec_module(audit)

class FakeADB:
    def __init__(self, props=None, packages=None, failures=None):
        self.props = props or {}
        self.packages = set(packages or [])
        self.failures = set(failures or [])
        self.calls = []
    def __call__(self, args):
        self.calls.append(args)
        if " ".join(args) in self.failures:
            return 1, "", "failed"
        if args[0] == "getprop":
            return 0, self.props.get(args[1], "") + "\n", ""
        if args[:3] == ["pm", "list", "packages"]:
            return 0, ("package:" + args[3] + "\n") if args[3] in self.packages else "", ""
        if args[:2] == ["pm", "has-feature"]:
            return 0, "false\n", ""
        if args == ["dumpsys", "SurfaceFlinger"]:
            return 0, "RenderEngine: GLES: Mesa Test renderer\n", ""
        raise AssertionError(args)

class EnvironmentAuditTests(unittest.TestCase):
    def test_virtual_without_services_not_attested(self):
        fake = FakeADB(props={"ro.hardware": "redroid",
                              "ro.boot.qemu": "1",
                              "ro.product.cpu.abilist": "arm64-v8a"})
        r = audit.collect(fake)
        self.assertEqual(r["google_package_presence"]["play_services"], "NOT_FOUND")
        self.assertEqual(r["device_integrity_verdict"], "NOT_MEASURED")
        self.assertEqual(r["game_security_reason"], "NOT_IDENTIFIED")
        self.assertEqual(r["gpu_gles_description"], "Mesa Test renderer")
        self.assertIn("qemu_property_enabled", r["virtual_environment_hints"])
    def test_installed_google_packages_are_not_certification(self):
        fake = FakeADB(props={"ro.hardware": "qcom"},
            packages={"com.android.vending", "com.google.android.gms", "com.google.android.gsf"})
        r = audit.collect(fake)
        self.assertTrue(all(v == "PRESENT" for v in r["google_package_presence"].values()))
        self.assertEqual(r["device_integrity_verdict"], "NOT_MEASURED")
        self.assertEqual(r["game_start_success"], "NOT_TESTED_BY_THIS_SCRIPT")
        self.assertEqual(r["virtual_environment_hints"], [])
    def test_adb_failure_not_mistaken_for_missing_package(self):
        fake = FakeADB(failures={"pm list packages com.google.android.gms",
                                 "getprop ro.build.version.release"})
        r = audit.collect(fake)
        self.assertEqual(r["google_package_presence"]["play_services"], "UNKNOWN")
        self.assertEqual(r["build_properties"]["ro.build.version.release"], "UNKNOWN")
        self.assertIn("NOT IDENTIFIED", audit.markdown(r))
    def test_only_safe_queries(self):
        fake = FakeADB()
        audit.collect(fake)
        cmds = " ".join(" ".join(args) for args in fake.calls)
        for word in ("android_id", "imei", "serialno", "dumpsys account",
                     "setprop", "settings put", "pm install", "/data/user/"):
            self.assertNotIn(word, cmds)

if __name__ == "__main__":
    unittest.main()
