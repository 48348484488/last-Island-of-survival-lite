import sys
import unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "tools"))
from remote_physical_preflight import evaluate, FIELDS

GOOD = {
    "OXIDE_REMOTE_HOST": "lab.example.net",
    "OXIDE_REMOTE_USER": "androidlab",
    "OXIDE_REMOTE_SSH_KEY": "-----BEGIN OPENSSH PRIVATE KEY-----\ntest\n-----END OPENSSH PRIVATE KEY-----",
    "OXIDE_REMOTE_KNOWN_HOSTS": "lab.example.net ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAExamplePublicKey",
    "OXIDE_REMOTE_ADB_SERIAL": "R5C1234FA",
}
class RemotePhysicalTests(unittest.TestCase):
    def test_default_no_device_no_download(self):
        v=evaluate({})
        self.assertEqual(v["status"],"REMOTE_PHYSICAL_NOT_CONFIGURED")
        self.assertEqual(set(v["missing_secret_names"]),set(FIELDS))
        self.assertFalse(v["game_installed"])
        self.assertFalse(v["cloud_device_billing_triggered"])
    def test_secrets_presence_not_device_proof(self):
        v=evaluate(GOOD)
        self.assertEqual(v["status"],"REMOTE_PHYSICAL_CONNECTION_CONFIGURED")
        self.assertFalse(v["device_connected"])
        self.assertEqual(v["device_type"],"UNVERIFIED")
    def test_injection_in_serial_rejected(self):
        env={**GOOD,"OXIDE_REMOTE_ADB_SERIAL":"1234;curl bad"}
        self.assertIn("OXIDE_REMOTE_ADB_SERIAL",evaluate(env)["invalid_format_secret_names"])
    def test_ssh_host_user_validation(self):
        env={**GOOD,"OXIDE_REMOTE_HOST":"$(touch /tmp/example)","OXIDE_REMOTE_USER":"root;id"}
        out=evaluate(env)
        self.assertIn("OXIDE_REMOTE_HOST",out["invalid_format_secret_names"])
        self.assertIn("OXIDE_REMOTE_USER",out["invalid_format_secret_names"])
    def test_requires_pinned_ssh_key_not_auto_scan(self):
        env={**GOOD,"OXIDE_REMOTE_KNOWN_HOSTS":"just_hostname"}
        v=evaluate(env)
        self.assertEqual(v["status"],"REMOTE_PHYSICAL_NOT_CONFIGURED")
        self.assertIn("OXIDE_REMOTE_KNOWN_HOSTS",v["invalid_format_secret_names"])
    def test_key_not_exposed(self):
        v=evaluate(GOOD)
        self.assertNotIn(GOOD["OXIDE_REMOTE_SSH_KEY"],str(v))
if __name__ == "__main__":
    unittest.main()
