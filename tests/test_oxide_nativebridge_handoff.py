"""Protect the native-bridge JNI to original-APK one-shot handoff."""
import subprocess
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
FIRST = ROOT / ".github/workflows/oxide-m3-x86-arm64-jni-smoke-once.yml"
SECOND = ROOT / ".github/workflows/oxide-m4-x86-arm64-nativebridge-original-on-success.yml"
RUNNER = ROOT / "tools/oxide_nativebridge_probe/original_on_verified_bridge.sh"


class NativeBridgeOneShotHandoffTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.first = FIRST.read_text()
        cls.second = SECOND.read_text()
        cls.script = RUNNER.read_text()

    def test_predecessor_is_pinned_and_trusted(self):
        self.assertIn("workflow_run:", self.second)
        self.assertIn("github.event.workflow_run.id == 38817025884", self.second)
        self.assertIn("github.event.workflow_run.conclusion == 'success'", self.second)
        self.assertIn("github.event.workflow_run.head_repository.full_name == github.repository", self.second)
        self.assertIn("gh run download 38817025884", self.second)
        self.assertIn("arm64_jni_executed", self.second)

    def test_proven_arm64_jni_precedes_expensive_oxide_download(self):
        self.assertLess(self.second.index("nativebridge-jni-verdict.json"),
                        self.second.index("reactivecircus/android-emulator-runner@v2"))
        self.assertLess(self.script.index('test -f "$REPORTS/nativebridge-prerequisite.json"'),
                        self.script.index("gh release download"))
        self.assertLess(self.script.index('assert d["arm64_jni_executed"] is True'),
                        self.script.index("gh release download"))
        self.assertLess(self.script.index("tools/oxide_original_hash_gate.py"),
                        self.script.index("install-multiple"))

    def test_no_periodic_repeat_or_new_provider_account(self):
        self.assertNotIn("  schedule:", self.first + self.second)
        self.assertNotIn("  schedule:", self.second)
        self.assertNotIn("gmsaas", self.first + self.second + self.script)
        self.assertNotIn("GENYMOTION_API_TOKEN", self.second)

    def test_probe_does_not_install_game(self):
        self.assertNotIn("gh release download", self.first)
        self.assertNotIn("split_UnityDataAssetPack.apk", self.first)
        self.assertIn("tools/oxide_nativebridge_probe/run_on_avd.sh", self.first)

    def test_original_game_is_unchanged_and_monotonic(self):
        for file in ("base.apk", "split_config.arm64_v8a.apk",
                     "split_config.xxhdpi.apk", "split_UnityDataAssetPack.apk"):
            self.assertIn(file, self.script)
        self.assertIn("--launch-activity", self.script)
        self.assertIn("tools/oxide_capture_samples.py", self.script)
        self.assertIn("tools/oxide_runtime_assess.py", self.script)
        self.assertNotIn("setprop", self.script)
        self.assertNotIn("pm grant", self.script)
        self.assertNotIn("resign", self.script.lower())

    def test_bash_runner_parses(self):
        p = subprocess.run(["bash", "-n", str(RUNNER)],
                           capture_output=True, text=True, timeout=6)
        self.assertEqual(p.returncode, 0, p.stderr)

    def test_supervisor_classifies_toy_jni_without_claiming_gameplay(self):
        supervisor = (ROOT / ".github/workflows/oxide-event-supervisor.yml").read_text()
        self.assertIn("Oxide M3 official x86_64 AVD executes independent ARM64 JNI - one shot", supervisor)
        self.assertIn("Oxide M4 original game on proven x86_64 ARM64 translator - one trial", supervisor)
        self.assertIn("nativebridge-jni-verdict.json", supervisor)
        self.assertIn("ABI/libndk_translation advertising alone is not sufficient", supervisor)
        self.assertIn("This is NOT game, menu, gameplay or ESP proof", supervisor)
        self.assertIn("no automatic repeat or bypass", supervisor)

    def test_evidence_preserved_even_on_failure(self):
        self.assertIn("if: always()", self.second)
        self.assertIn("oxide-x86-nativebridge-original-1-15-11966-one-shot", self.second)
        self.assertIn("trap collect_logcat EXIT", self.script)


if __name__ == "__main__":
    unittest.main()
