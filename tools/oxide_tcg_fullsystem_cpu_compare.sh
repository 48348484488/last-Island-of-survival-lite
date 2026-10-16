#!/usr/bin/env bash
set -Eeuo pipefail
e="$PWD/evidence/oxide-tcg-performance"
mkdir -p "$e"
echo "RESULT=NOT_YET_MEASURED; SCOPE=Linux ARM64 guest only, no Android or Oxide" > "$e/result.txt"
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT
sudo apt-get update -qq
sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends qemu-system-arm gcc-aarch64-linux-gnu libc6-dev-arm64-cross cpio curl >/dev/null
curl --fail --silent --location --connect-timeout 10 --max-time 50 --max-filesize 16000000 https://dl-cdn.alpinelinux.org/alpine/v3.24/releases/aarch64/netboot/vmlinuz-virt -o "$tmp/vmlinuz"
bytes="$(stat -c %s "$tmp/vmlinuz")"
test "$bytes" -gt 4000000 && test "$bytes" -lt 16000000
sha256sum "$tmp/vmlinuz" > "$e/kernel.sha256"
mkdir -p "$tmp/fs/dev"
cat > "$tmp/guest.c" <<'EOF'
#include <pthread.h>
#include <stdint.h>
#include <stdio.h>
#include <time.h>
#include <unistd.h>
#include <sys/reboot.h>
static volatile uint64_t checksums[2];
static void* work(void* a) {
  uintptr_t id=(uintptr_t)a;
  volatile uint64_t value=0x123456abcdefULL+id;
  for (uint64_t i=0;i<22000000ULL;i++) {
    value ^= value << 13; value ^= value >> 7; value ^= value << 17;
  }
  checksums[id]=value;
  return 0;
}
static double now(void) {
  struct timespec ts;
  clock_gettime(CLOCK_MONOTONIC,&ts);
  return (double)ts.tv_sec + ts.tv_nsec/1e9;
}
int main(void) {
  printf("GUEST_ARM64_PID1_READY\n");fflush(stdout);
  pthread_t p,q;
  double t=now();
  if (pthread_create(&p,0,work,(void*)0) || pthread_create(&q,0,work,(void*)1)) return 3;
  pthread_join(p,0);pthread_join(q,0);
  printf("ARM64_BENCH_SECONDS=%.6f CHECK=%llu:%llu\n",now()-t,(unsigned long long)checksums[0],(unsigned long long)checksums[1]);
  fflush(stdout);sync();sleep(1);reboot(RB_POWER_OFF);
  for(;;)pause();
}
EOF
aarch64-linux-gnu-gcc -O2 -static -pthread "$tmp/guest.c" -o "$tmp/fs/init"
sudo mknod "$tmp/fs/dev/console" c 5 1
(cd "$tmp/fs"; find . -print0 | cpio --null -o -H newc 2>/dev/null | gzip -1 -n > "$tmp/ramdisk.gz")
for mode in default tuned; do
  if test "$mode" = tuned; then accel="tcg,tb-size=1024,thread=multi"; else accel="tcg"; fi
  set +e
  timeout --signal=TERM --kill-after=4s 55s qemu-system-aarch64 -machine virt -accel "$accel" -cpu max -smp 2 -m 768 -kernel "$tmp/vmlinuz" -initrd "$tmp/ramdisk.gz" -append 'console=ttyAMA0 rdinit=/init loglevel=4 panic=5' -nographic -monitor none -no-reboot -net none > "$e/$mode-console.log" 2>&1
  rc=$?
  set -e
  echo "MODE=$mode EXIT=$rc" | tee -a "$e/result.txt"
  grep 'ARM64_BENCH_SECONDS=' "$e/$mode-console.log" | tail -1 | tee -a "$e/result.txt" || true
done
python3 - "$e" <<'PY'
import json,re,sys
from pathlib import Path
p=Path(sys.argv[1]);times={};checks={}
for mode in ("default","tuned"):
 s=(p/(mode+"-console.log")).read_text(errors="replace")
 m=re.search(r"ARM64_BENCH_SECONDS=([0-9.]+) CHECK=([0-9]+:[0-9]+)",s)
 if m: times[mode]=float(m[1]);checks[mode]=m[2]
ok=len(times)==2 and checks["default"]==checks["tuned"]
data={"scope":"Actual full-system Linux ARM64 two-thread workload; not Android or Oxide","result":"PASS" if ok else "FAIL","guest_seconds":times,"checksum":checks,"tuned_speedup":round(times["default"]/times["tuned"],3) if ok else None}
(p/"comparison.json").write_text(json.dumps(data,indent=2,sort_keys=True)+"\n")
print(json.dumps(data,sort_keys=True))
if not ok:raise SystemExit(1)
PY
