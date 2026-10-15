"""Check Android's resumed ActivityRecord belongs to the asserted original PID.

Fixtures reproduce the syntax of real Android 15/16 dumpsys without shipping
APK payloads or changing a protected game. Presence of package text alone is
not sufficient to validate the foreground game process.
"""
import unittest
from tools.oxide_activity_binding import (
    PACKAGE, ORIGINAL, original_resumed_activity_pid,
)


def dump(pid="2023", token="41083833", component=ORIGINAL,
         process_package=PACKAGE):
    return f"""ACTIVITY MANAGER ACTIVITIES (dumpsys activity activities)
Display #0:
  * Task{{66383fe #8}}
    topResumedActivity=ActivityRecord{{{token} u0 {component} t8}}
    * Hist  #0: ActivityRecord{{{token} u0 {component} t8}}
      packageName={PACKAGE} processName={PACKAGE}
      launchedFromUid=2000
      app=ProcessRecord{{2b72d29 {pid}:{process_package}/u0a87}}
      Intent {{ flg=0x10000000 cmp={component} }}
      state=RESUMED
  * Task{{other #7}}
    * Hist  #0: ActivityRecord{{4768167 u0 com.android.launcher3/.Home t7}}
      app=ProcessRecord{{f032c4 1150:com.android.launcher3/u0a60}}
"""


class TestActivityProcessBinding(unittest.TestCase):
    def test_original_android15_pid(self):
        self.assertEqual(original_resumed_activity_pid(dump()), "2023")

    def test_original_android16_pid(self):
        self.assertEqual(original_resumed_activity_pid(dump(pid="2108")), "2108")

    def test_analytics_pid_cannot_replace_original_pid(self):
        self.assertNotEqual(original_resumed_activity_pid(dump(pid="2023")), "2296")

    def test_resumed_launcher_with_stale_original_history(self):
        s = dump().replace(
            f"topResumedActivity=ActivityRecord{{41083833 u0 {ORIGINAL} t8}}",
            "topResumedActivity=ActivityRecord{4768167 u0 com.android.launcher3/.Home t7}"
        )
        self.assertIsNone(original_resumed_activity_pid(s))

    def test_mismatched_activity_record_identity(self):
        s = dump().replace("* Hist  #0: ActivityRecord{41083833 ",
                           "* Hist  #0: ActivityRecord{ffffff ", 1)
        self.assertIsNone(original_resumed_activity_pid(s))

    def test_background_process_suffix_rejected(self):
        self.assertIsNone(original_resumed_activity_pid(
            dump(process_package=PACKAGE + ":analytics")))

    def test_activity_without_app_record_rejected(self):
        self.assertIsNone(original_resumed_activity_pid(
            dump().replace("app=ProcessRecord{", "notProcess={")))

    def test_multiple_pid_tokens_rejected(self):
        self.assertIsNone(original_resumed_activity_pid(dump(pid="2023 2296")))

    def test_historical_activity_without_resumed_state(self):
        self.assertIsNone(original_resumed_activity_pid(
            dump().replace("topResumedActivity=", "mLastPausedActivity=")))

    def test_different_activity_or_package_rejected(self):
        self.assertIsNone(original_resumed_activity_pid(
            dump(component=PACKAGE + "/.OtherActivity")))
        self.assertIsNone(original_resumed_activity_pid(
            dump(process_package="com.example.other")))

    def test_never_infers_missing_process_pid(self):
        self.assertIsNone(original_resumed_activity_pid(""))
        self.assertIsNone(original_resumed_activity_pid(None))
        self.assertIsNone(original_resumed_activity_pid("ActivityRecord ..."))


if __name__ == "__main__":
    unittest.main()
