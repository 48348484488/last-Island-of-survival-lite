#!/usr/bin/env bash
# Compare two unmodified native ARM64 Redroid virtual Android images.
# No physical device, no x86 translation, no app/security modifications.
set -euo pipefail
if [[ $# -ne 2 ]]; then
  echo "Usage: $0 ORIGINAL_APK_DIRECTORY OUTPUT_DIRECTORY" >&2; exit 64
fi
APK="$1"; OUT="$2"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
GAME=com.catsbit.oxidesurvivalisland
ACTIVITY=cTOOT3.abKmV1.tP0Gr1.OxideActivity
SERIAL=127.0.0.1:5555
mkdir -p "$OUT"
python3 "$ROOT/tools/verify_original_splits.py" "$APK" \
  --output "$OUT/original_hashes.json" >/dev/null
test "$(uname -m)" = aarch64

cleanup() {
  docker rm -f oxide-virtual-version >/dev/null 2>&1 || true
  adb disconnect "$SERIAL" >/dev/null 2>&1 || true
}
trap cleanup EXIT

for VERSION in ${OXIDE_ANDROID_VERSIONS:-12 14}; do
  case "$VERSION" in 12|13|14|15|16) ;; *) echo "Unsupported virtual Android: $VERSION" >&2; exit 64;; esac
  DIR="$OUT/android_$VERSION"; mkdir -p "$DIR"
  IMAGE="redroid/redroid:$VERSION.0.0_64only-latest"
  cleanup
  echo "$IMAGE" > "$DIR/image.txt"
  echo STARTED > "$DIR/experiment_phase.txt"
  if ! timeout 600 docker pull "$IMAGE" > "$DIR/docker_pull.txt" 2>&1; then
    echo IMAGE_PULL_FAILED > "$DIR/experiment_phase.txt"
    continue
  fi
  DOCKER_DEVICE_ARGS=()
  if [[ "${OXIDE_ATTACH_HOST_ASHMEM:-0}" == "1" ]]; then
    test -c /dev/ashmem || { echo HOST_ASHMEM_DEVICE_MISSING >&2; exit 70; }
    DOCKER_DEVICE_ARGS=(--device /dev/ashmem:/dev/ashmem)
  fi
  docker run -d --rm --privileged --name oxide-virtual-version \
    "${DOCKER_DEVICE_ARGS[@]}" \
    -p 127.0.0.1:5555:5555 -v /dev/binderfs:/dev/binderfs \
    -v "/tmp/oxide-virtual-$VERSION-data:/data" \
    "$IMAGE" \
    androidboot.redroid_gpu_mode=guest \
    androidboot.redroid_width=720 androidboot.redroid_height=1280 \
    androidboot.redroid_dpi=320 \
    > "$DIR/container.txt" 2>&1 || {
      echo CONTAINER_START_FAILED > "$DIR/experiment_phase.txt"
      continue
    }
  READY=0
  for attempt in $(seq 1 60); do
    adb connect "$SERIAL" >/dev/null 2>&1 || true
    if timeout 6 adb -s "$SERIAL" shell getprop sys.boot_completed \
      2>/dev/null | grep -q '^1'; then READY=1; break; fi
    sleep 3
  done
  if [[ "$READY" != 1 ]]; then
    echo ANDROID_BOOT_FAILED > "$DIR/experiment_phase.txt"
    docker logs oxide-virtual-version > "$DIR/container_log.txt" 2>&1 || true
    continue
  fi
  echo BOOT_PASS > "$DIR/experiment_phase.txt"
  adb -s "$SERIAL" shell getprop ro.product.cpu.abilist \
    > "$DIR/abi.txt" 2>&1 || true
  adb -s "$SERIAL" shell getprop ro.build.version.release \
    > "$DIR/release.txt" 2>&1 || true
  adb -s "$SERIAL" shell getprop ro.build.version.sdk \
    > "$DIR/sdk.txt" 2>&1 || true
  if ! grep -q arm64-v8a "$DIR/abi.txt"; then
    echo NON_ARM64_GUEST > "$DIR/experiment_phase.txt"
    continue
  fi
  adb -s "$SERIAL" shell pm list packages com.google.android.gms \
    > "$DIR/google_packages.txt" 2>&1 || true
  if ! adb -s "$SERIAL" install-multiple -r \
    "$APK/base.apk" "$APK/split_config.arm64_v8a.apk" \
    "$APK/split_config.xxhdpi.apk" "$APK/split_UnityDataAssetPack.apk" \
    > "$DIR/install.txt" 2>&1; then
    echo ORIGINAL_INSTALL_FAILED > "$DIR/experiment_phase.txt"
    continue
  fi
  if ! grep -q '^Success' "$DIR/install.txt"; then
    echo ORIGINAL_INSTALL_NOT_CONFIRMED > "$DIR/experiment_phase.txt"
    continue
  fi
  echo ORIGINAL_INSTALL_SUCCESS > "$DIR/experiment_phase.txt"
  adb -s "$SERIAL" logcat -b all -c > "$DIR/logcat_clear.txt" 2>&1 || true
  adb -s "$SERIAL" shell am force-stop "$GAME" > "$DIR/force_stop.txt" 2>&1 || true
  adb -s "$SERIAL" logcat -b all -v threadtime > "$DIR/all_buffers.txt" 2>&1 &
  LOG_PID=$!
  # Optional kernel syscall trace, entirely outside the unmodified Android app.
  # Start it BEFORE launching the Activity and keep output per experiment.
  if [[ "${OXIDE_CAPTURE_KERNEL_SIGKILL:-0}" == "1" ]]; then
    command -v bpftrace >/dev/null || { echo HOST_BPFTRACE_MISSING >&2; exit 71; }
    cat > "$DIR/original_kernel_sigkill.bt" <<'BT'
BEGIN
{
  printf("OXIDE_KERNEL_TRACE_READY\n");
}
tracepoint:syscalls:sys_enter_kill
/args->sig == 9/
{
  printf("KERNEL_SIGKILL syscall=kill host_sender=%d host_tid=%d target_pid=%d signal=%d comm=%s boot_ns=%llu\n",
         pid, tid, args->pid, args->sig, comm, nsecs);
}
tracepoint:syscalls:sys_enter_tgkill
/args->sig == 9/
{
  printf("KERNEL_SIGKILL syscall=tgkill host_sender=%d host_tid=%d target_tgid=%d target_tid=%d signal=%d comm=%s boot_ns=%llu\n",
         pid, tid, args->tgid, args->pid, args->sig, comm, nsecs);
}
tracepoint:syscalls:sys_enter_rt_sigqueueinfo
/args->sig == 9/
{
  printf("KERNEL_SIGKILL syscall=rt_sigqueueinfo host_sender=%d host_tid=%d target_pid=%d signal=%d comm=%s boot_ns=%llu\n",
         pid, tid, args->pid, args->sig, comm, nsecs);
}
BT
    if [[ "${OXIDE_CAPTURE_BINDER_FINISH_WINDOW:-0}" == "1" ]]; then
      # Capture only the stock Android system_server destination; generic
      # Binder numeric code is NOT proof of a particular AIDL method.
      if ! sudo -n test -r /sys/kernel/tracing/events/binder/binder_transaction/id; then
        echo BINDER_TRANSACTION_TRACEPOINT_NOT_PRESENT > "$DIR/binder_preflight.txt"
        exit 74
      fi
      SERVER_GUEST_PID="$(timeout 8 adb -s "$SERIAL" shell pidof system_server | tr -d '\r')"
      [[ "$SERVER_GUEST_PID" =~ ^[0-9]+$ ]] || { echo INVALID_ANDROID_SYSTEM_SERVER_PID >&2; exit 75; }
      CONTAINER_SERVER_INIT="$(docker inspect -f '{{.State.Pid}}' oxide-virtual-version)"
      # Verify system_server is in the SAME container PID namespace.
      sudo -n python3 - "$ROOT" "$CONTAINER_SERVER_INIT" "$SERVER_GUEST_PID" \
        "$DIR/host_system_server_namespace.json" <<'PY'
import json,sys
from pathlib import Path
sys.path.insert(0,str(Path(sys.argv[1])/"tools"))
from prove_original_host_pid_namespace import inspect
result=inspect(Path("/proc"),int(sys.argv[2]),int(sys.argv[3]),package="system_server")
Path(sys.argv[4]).write_text(json.dumps(result,indent=2)+"\n")
print(result)
if result.get("status") != "ORIGINAL_ANDROID_HOST_NSPID_MAPPING_PROVEN":
    raise SystemExit(1)
PY
      SERVER_HOST_PID="$(python3 - "$DIR/host_system_server_namespace.json" <<'PY'
import json,sys
x=json.load(open(sys.argv[1]))
print(x["host_pid"])
PY
)"
      [[ "$SERVER_HOST_PID" =~ ^[0-9]+$ ]] || exit 76
      cat >> "$DIR/original_kernel_sigkill.bt" <<'BTB'
tracepoint:binder:binder_transaction
/args->to_proc == __ANDROID_SYSTEM_SERVER_HOST_PID__ && args->reply == 0/
{
  printf("ANDROID_BINDER host_sender=%d host_tid=%d dest_proc=%d dest_thread=%d transaction=%d code=%d reply=%d flags=%d comm=%s boot_ns=%llu\n",
    pid,tid,args->to_proc,args->to_thread,args->debug_id,args->code,args->reply,args->flags,comm,nsecs);
}
BTB
      sed -i "s/__ANDROID_SYSTEM_SERVER_HOST_PID__/$SERVER_HOST_PID/g" \
        "$DIR/original_kernel_sigkill.bt"
      echo "SYSTEM_SERVER_HOST_PID=$SERVER_HOST_PID GUEST_PID=$SERVER_GUEST_PID" > "$DIR/binder_preflight.txt"
    fi
    # bpftrace can take >5s to compile probes on GitHub ARM runners.
    # BEGIN prints a startup marker; the controlled SIGKILL below independently
    # verifies probes are actually active BEFORE launching the original game.
    sudo -n stdbuf -oL -eL timeout --signal=INT 95s \
      bpftrace "$DIR/original_kernel_sigkill.bt" \
      > "$DIR/kernel_sigkill_trace.txt" 2>&1 &
    KERNEL_TRACE_PID=$!
    KERNEL_READY=0
    for poll in $(seq 1 55); do
      if grep -Fq OXIDE_KERNEL_TRACE_READY "$DIR/kernel_sigkill_trace.txt"; then
        KERNEL_READY=1
        break
      fi
      if ! kill -0 "$KERNEL_TRACE_PID" 2>/dev/null; then break; fi
      sleep 1
    done
    if [[ "$KERNEL_READY" != 1 ]]; then
      echo KERNEL_TRACE_READY_BEFORE_GAME_NOT_CONFIRMED > "$DIR/kernel_trace_preflight.txt"
      echo "No original game launched: kernel tracer not attached" >&2
      exit 72
    fi
    # Prove that the same live probe captures a CONTROLLED shell kill syscall.
    # Only a disposable sleep child is terminated; the Oxide process is untouched.
    sleep 90 &
    KERNEL_CONTROL_PID=$!
    kill -9 "$KERNEL_CONTROL_PID" 2>/dev/null || true
    wait "$KERNEL_CONTROL_PID" 2>/dev/null || true
    KERNEL_CONTROL_PASS=0
    for poll in $(seq 1 12); do
      if grep -Fq "target_pid=$KERNEL_CONTROL_PID signal=9" "$DIR/kernel_sigkill_trace.txt"; then
        KERNEL_CONTROL_PASS=1
        break
      fi
      sleep 0.25
    done
    if [[ "$KERNEL_CONTROL_PASS" != 1 ]]; then
      echo KERNEL_SIGNAL_CONTROL_NOT_CAPTURED > "$DIR/kernel_trace_preflight.txt"
      echo "No original game launched: controlled kernel signal missing" >&2
      exit 73
    fi
    echo KERNEL_TRACE_ATTACHED_BEFORE_ORIGINAL_ACTIVITY > "$DIR/kernel_trace_preflight.txt"
  fi
  # Start rapid read-only PID observation BEFORE launching the original Activity.
  # This avoids relying only on am_proc_start events, which Android 12 can lose.
  (
    for sample in $(seq 1 70); do
      pids="$(timeout 3 adb -s "$SERIAL" shell pidof "$GAME" 2>/dev/null | tr -d '\r' || true)"
      printf 'sample=%s utc=%s pid=%s\n' "$sample" "$(date -u +%Y-%m-%dT%H:%M:%SZ)" "${pids:-none}"
      sleep 0.12
    done
  ) > "$DIR/pid_trace.txt" 2>&1 &
  TRACE_PID=$!
  # Extra host/guest PID-namespace mapping runs only when explicitly requested.
  # It observes actual Linux /proc while the unchanged Activity is alive.
  # No ptrace, app modifications, signal manipulation, or identity spoofing.
  if [[ "${OXIDE_CAPTURE_HOST_PID_NAMESPACE:-0}" == "1" ]]; then
    CONTAINER_INIT_HOST_PID="$(docker inspect -f '{{.State.Pid}}' oxide-virtual-version)"
    test "$CONTAINER_INIT_HOST_PID" -gt 1
    echo "$CONTAINER_INIT_HOST_PID" > "$DIR/container_init_host_pid.txt"
    (
      for attempt in $(seq 1 100); do
        test -s "$DIR/host_pid_namespace.json" && break
        original_pid="$(timeout 2 adb -s "$SERIAL" shell pidof "$GAME" 2>/dev/null | tr -d '\r' || true)"
        if [[ "$original_pid" =~ ^[0-9]+$ ]]; then
          if sudo -n python3 "$ROOT/tools/prove_original_host_pid_namespace.py" \
            --container-init-host-pid "$CONTAINER_INIT_HOST_PID" \
            --guest-pid "$original_pid" --attempts 2 --interval-ms 50 \
            --out "$DIR/host_pid_namespace_probe.json" \
            > "$DIR/host_pid_namespace_probe_stdout.txt" 2>&1; then
            cp "$DIR/host_pid_namespace_probe.json" "$DIR/host_pid_namespace.json"
            echo LIVE_ORIGINAL_NSPID_HOST_GUEST_MAPPING_OBSERVED
            break
          fi
        fi
        sleep 0.08
      done
    ) > "$DIR/host_pid_namespace_watcher_stdout.txt" 2>&1 &
    HOSTMAP_PID=$!
  fi
  adb -s "$SERIAL" shell am start -W -n "$GAME/$ACTIVITY" \
    > "$DIR/activity_start.txt" 2>&1 || true
  adb -s "$SERIAL" shell dumpsys activity activities \
    > "$DIR/activities_early.txt" 2>&1 || true
  adb -s "$SERIAL" exec-out screencap -p \
    > "$DIR/screen_early.png" 2>/dev/null || true
  sleep 2
  adb -s "$SERIAL" exec-out screencap -p \
    > "$DIR/screen_2s.png" 2>/dev/null || true
  sleep 8
  wait "$TRACE_PID" 2>/dev/null || true
  if [[ "${OXIDE_CAPTURE_HOST_PID_NAMESPACE:-0}" == "1" ]]; then
    wait "$HOSTMAP_PID" 2>/dev/null || true
  fi
  adb -s "$SERIAL" shell pidof "$GAME" > "$DIR/pid_10s.txt" 2>&1 || true
  adb -s "$SERIAL" shell dumpsys activity activities \
    > "$DIR/activities_10s.txt" 2>&1 || true
  if [[ "${OXIDE_CAPTURE_35S:-0}" == "1" ]]; then
    # Read-only continuity proof: package PID alone is not enough, also
    # require the ORIGINAL Activity to remain resumed at 35 seconds.
    sleep 25
    adb -s "$SERIAL" shell pidof "$GAME" > "$DIR/pid_35s.txt" 2>&1 || true
    adb -s "$SERIAL" shell dumpsys activity activities \
      > "$DIR/activities_35s.txt" 2>&1 || true
  fi
  adb -s "$SERIAL" shell dumpsys activity exit-info "$GAME" \
    > "$DIR/exit_info.txt" 2>&1 || true
  adb -s "$SERIAL" logcat -b crash -d -v threadtime \
    > "$DIR/crash_buffer.txt" 2>&1 || true
  kill "$LOG_PID" 2>/dev/null || true
  wait "$LOG_PID" 2>/dev/null || true
  if [[ "${OXIDE_CAPTURE_KERNEL_SIGKILL:-0}" == "1" ]]; then
    wait "$KERNEL_TRACE_PID" 2>/dev/null || true
  fi
  echo OBSERVATION_COMPLETE > "$DIR/experiment_phase.txt"
  python3 "$ROOT/tools/diagnose_original_exit.py" "$DIR" \
    > "$DIR/forensic_stdout.txt" 2>&1 || true
  if [[ "${OXIDE_CAPTURE_HOST_PID_NAMESPACE:-0}" == "1" ]]; then
    PYTHONPATH="$ROOT/tools" python3 - "$DIR" <<'PY' \
      > "$DIR/host_pid_exit_chain_stdout.txt" 2>&1 || true
import json,sys
from pathlib import Path
from prove_original_host_pid_namespace import crosscheck
root=Path(sys.argv[1])
mapping_path=root/"host_pid_namespace.json"
mapping=json.loads(mapping_path.read_text()) if mapping_path.is_file() else {"status":"NOT_OBSERVED"}
forensic_path=root/"forensic_summary.json"
forensic=json.loads(forensic_path.read_text()) if forensic_path.is_file() else {}
verdict=crosscheck(mapping,forensic)
(root/"host_pid_exit_chain.json").write_text(json.dumps(verdict,indent=2)+"\n")
print(json.dumps(verdict,indent=2))
PY
  fi
  if [[ "${OXIDE_CAPTURE_KERNEL_SIGKILL:-0}" == "1" ]]; then
    PYTHONPATH="$ROOT/tools" python3 \
      "$ROOT/tools/corroborate_original_kernel_and_namespace.py" "$DIR" \
      > "$DIR/same_run_kernel_namespace_stdout.txt" 2>&1 || true
  fi
  if [[ "${OXIDE_CAPTURE_BINDER_FINISH_WINDOW:-0}" == "1" ]]; then
    PYTHONPATH="$ROOT/tools" python3 \
      "$ROOT/tools/audit_original_binder_finish_window.py" "$DIR" \
      > "$DIR/original_binder_finish_window_stdout.txt" 2>&1 || true
  fi
  python3 "$ROOT/tools/diagnose_virtual_activity_pid.py" "$DIR" \
    > "$DIR/lifecycle_recovery_stdout.txt" 2>&1 || true
  docker logs oxide-virtual-version > "$DIR/container_log.txt" 2>&1 || true
done

python3 - "$OUT" "${OXIDE_ANDROID_VERSIONS:-12 14}" <<'PY'
import json,sys
from pathlib import Path
root=Path(sys.argv[1])
result={}
for version in sys.argv[2].split():
    folder=root/("android_"+version)
    def read(name):
        p=folder/name
        return p.read_text(errors="replace").strip() if p.is_file() else ""
    def load(name):
        try: return json.loads(read(name))
        except (ValueError,TypeError): return {}
    foren=load("forensic_summary.json")
    lifecycle=load("lifecycle_recovery.json")
    pids={x for x in read("pid_10s.txt").split() if x.isdecimal()}
    pid=foren.get("initial_activity_pid")
    if (not isinstance(pid,int) or isinstance(pid,bool) or pid <= 0) and lifecycle.get("pid_confirms_activity_process") is True:
        pid=lifecycle.get("selected_pid")
    pids35={x for x in read("pid_35s.txt").split() if x.isdecimal()}
    activities35=read("activities_35s.txt")
    resumed35=any(
        "OxideActivity" in x and "com.catsbit.oxidesurvivalisland/" in x
        and ("topResumedActivity" in x or "mResumedActivity" in x)
        for x in activities35.splitlines())
    activities=read("activities_10s.txt")
    resumed=any(
        "OxideActivity" in x and "com.catsbit.oxidesurvivalisland/" in x
        and ("topResumedActivity" in x or "mResumedActivity" in x)
        for x in activities.splitlines())
    gms=read("google_packages.txt")
    result[version]={
        "host_architecture":"aarch64",
        "abi":read("abi.txt"),
        "boot":"PASS" if read("experiment_phase.txt") in
            ("BOOT_PASS","ORIGINAL_INSTALL_FAILED","ORIGINAL_INSTALL_NOT_CONFIRMED",
             "ORIGINAL_INSTALL_SUCCESS","OBSERVATION_COMPLETE") else "FAIL",
        "install":"SUCCESS" if read("install.txt").startswith("Success") else "NOT_CONFIRMED",
        "initial_activity_pid_selected":pid,
        "initial_pid_alive_10s":pid is not None and str(pid) in pids,
        "activity_resumed_10s":resumed,
        "initial_pid_alive_35s":pid is not None and str(pid) in pids35,
        "activity_resumed_35s":resumed35,
        "forensic":foren,
        "lifecycle_recovery":lifecycle,
        "play_services":"PRESENT" if "package:com.google.android.gms" in gms.splitlines() else
                        "NOT_FOUND" if gms or read("experiment_phase.txt")=="OBSERVATION_COMPLETE" else "NOT_MEASURED",
        "experiment_phase":read("experiment_phase.txt"),
    }
(root/"raw_version_states.json").write_text(json.dumps(result,indent=2)+"\n")
PY
if [[ "${OXIDE_ANDROID_VERSIONS:-12 14}" == "12 14" ]]; then
  python3 "$ROOT/tools/compare_virtual_redroid_versions.py" \
    "$OUT/raw_version_states.json" --out "$OUT"
else
  python3 - "$OUT" <<'PY'
import json,sys
from pathlib import Path
out=Path(sys.argv[1])
data=json.loads((out/"raw_version_states.json").read_text())
for version,item in data.items():
    lifecycle=item.get("lifecycle_recovery",{})
    lines=["# Virtual-only Oxide "+version+" process recovery","",
           "Android guest boot: "+item["boot"],
           "Original APK installation: "+item["install"],
           "am_proc_start baseline: "+item["forensic"].get("classification","NOT_MEASURED"),
           "PID recovery: "+lifecycle.get("status","NOT_MEASURED"),
           "Recovered PID source: "+lifecycle.get("pid_source","NOT_MEASURED"),
           "App self-SIGKILL: "+str(lifecycle.get("android_process_self_sigkill_logged_for_selected_pid",False)),
           "Activity PID attribution: "+str(lifecycle.get("pid_confirms_activity_process",False)),
           "", "No game protection modification or multiplayer/ESP verification."]
    (out/"virtual_lifecycle_retest.md").write_text("\n".join(lines)+"\n")
    print("\n".join(lines))
PY
fi
