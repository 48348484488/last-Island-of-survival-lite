"""Fail-closed ActivityRecord-to-ProcessRecord PID binding for unchanged Oxide.

'pidof package' alone may return a surviving analytics service. Only the
resumed Activity's own ProcessRecord can prove which PID is responsible.
No app modifications, dynamic instrumentation or security bypass.
"""
import re

PACKAGE = "com.catsbit.oxidesurvivalisland"
ORIGINAL = PACKAGE + "/.OxideActivity"

_RESUMED = re.compile(
    r"^\s*(?:topResumedActivity=|mResumedActivity=|ResumedActivity:)"
    r"ActivityRecord\{(?P<id>[0-9A-Fa-f]+)\s+u\d+\s+"
    r"(?P<component>\S+)\s+t\d+\}", re.MULTILINE)
_HIST = re.compile(
    r"^\s*\*\s+Hist\s+#\d+:\s*ActivityRecord\{"
    r"(?P<id>[0-9A-Fa-f]+)\s+u\d+\s+(?P<component>\S+)\s+t\d+\}", re.MULTILINE)
_PROCESS = re.compile(
    r"^\s*app=ProcessRecord\{\S+\s+(?P<pid>[1-9]\d*):"
    + re.escape(PACKAGE) + r"/u\d+a\d+\}", re.MULTILINE)


def original_resumed_activity_pid(dump):
    """Return the original resumed Activity's OWN process PID; else None.

    Android may leave historical records referring to the original Activity
    after Launcher resumes. Prefer authoritative topResumedActivity. Bind its
    exact record identity to the matching Hist entry and app=ProcessRecord.
    """
    if not isinstance(dump, str):
        return None
    markers = list(_RESUMED.finditer(dump))
    if not markers:
        return None
    top = [m for m in markers
           if dump[m.start():m.end()].lstrip().startswith("topResumedActivity=")]
    selected = top[0] if top else markers[0]
    if selected["component"] != ORIGINAL:
        return None
    for record in _HIST.finditer(dump):
        if record["id"] != selected["id"] or record["component"] != ORIGINAL:
            continue
        # ProcessRecord is immediately below the Hist header in observed
        # Android 15/16 dumpsys. Never scan the entire dump for unrelated PIDs.
        subset = "\n".join(dump[record.end():].splitlines()[:12])
        subset = re.split(r"(?m)^\s*\*\s+Hist\s+#\d+:", subset)[0]
        proc = _PROCESS.search(subset)
        if proc:
            return proc["pid"]
    return None
