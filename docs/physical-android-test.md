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
    android device remote connect RESERVATION_ID

Use actual model/reservation IDs from the service. Disconnect/remove the
reservation according to the provider documentation; never commit credentials.

After all FOUR original split APKs have been downloaded to the same directory
(the heavy original Unity asset APK is in this repo's release), and an
authorized remote device is reachable with ADB:

    bash tools/smoke_original_on_connected_device.sh ADB_SERIAL APK_DIRECTORY PRIVATE_OUTPUT_DIRECTORY

The script checks ARM64, writes environment diagnostics, installs all four
unaltered splits with the official Android install-multiple command, launches
the genuine OxideActivity, samples screen/process state at 3/10/35 seconds,
and creates a forensic report.

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
