# Original Oxide on authorized cloud-hosted physical Android

Goal: test four untouched signed APK splits on genuine remote Android hardware,
without needing the owner's phone or attempting to bypass security.

Google documents a remote Android device CLI which can reserve physical devices
and connect them to ADB:
https://developer.android.google.cn/tools/agents/android-cli/commands/device_remote?hl=pt

Requires an authorized and potentially billed Google Cloud project. Nothing
in this repository indicates such a project/account is currently connected.
The following commands are EXAMPLES, not actions completed:

    android auth login
    android device remote projects
    android device remote models
    android device remote create CODENAME/API --project=PROJECT_ID
    adb devices -l

By default, Google's `create` command connects the reserved device to ADB
automatically. If the reservation was created with `--connect=false`, run:

    android device remote connect RESERVATION_ID --project=PROJECT_ID

At the end of the authorized session:

    android device remote disconnect RESERVATION_ID --project=PROJECT_ID
    android device remote remove RESERVATION_ID --project=PROJECT_ID

Use actual model/reservation IDs from the service. Disconnect/remove the
reservation according to the provider documentation; never commit credentials.

After all FOUR original split APKs have been downloaded to the same directory
(the heavy original Unity asset APK is in this repo's release), and an
authorized remote device is reachable with ADB:

    bash tools/smoke_original_on_connected_device.sh ADB_SERIAL APK_DIRECTORY PRIVATE_OUTPUT_DIRECTORY

The script first checks the **full SHA-256 of every split against the
four original 1.14.11949 hashes**. If any split is missing or differs, the
script exits before even connecting to ADB. After that it checks ARM64,
writes environment diagnostics, installs all four unaltered splits with the
official Android install-multiple command, launches the genuine OxideActivity,
samples screen/process state at 3/10/35 seconds, and creates a forensic
report. It will only report a passing Activity smoke test when the **same
initial Activity PID** is present at both later checkpoints and the original
Activity is resumed at those points, with no original-process death logged.
Any missing evidence fails closed rather than counting an analytics service.

Only share the resulting logcat after redacting private information; this tool
does not publish the logs or screenshots. Nothing changes the game's security
controls, signing, code or multiplayer players.

Limits:
- No cloud project, authorized remote ADB reservation or provider credentials
  are connected. A GitHub-hosted Linux runner is not a physical device.
- A physical device is no guarantee that app/licensing/integrity verification
  passes. Do not assume the Oxide warning comes from Play Integrity specifically.
- Some device farms re-sign uploaded APKs; that is unsuitable for verifying an
  original signature-dependent binary.
- Surviving 35 seconds does NOT prove a playable multiplayer scene or an ESP.

## Automated checks already completed (no physical hardware)

[Oxide physical ADB smoke contract #38473888348](https://github.com/48348484488/oxide/actions/runs/38473888348)
passed **17/17 Python regression tests** plus the shell syntax check and a
command-line negative test proving that missing original APKs are rejected
before ADB is called. The CI workflow does not download proprietary APKs
or simulate actual gameplay.

The original APK Redroid boot workflow is now **manual-only**, because it
downloads roughly a gigabyte and consistently encounters the same built-in
security termination. See `.github/workflows/redroid-original-smoke.yml`.
The fast no-device contract workflow remains automatic on relevant code changes.

Official reference: https://developer.android.com/tools/agents/android-cli/commands/device_remote
