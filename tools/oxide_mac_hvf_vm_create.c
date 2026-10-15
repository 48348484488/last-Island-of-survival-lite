/* Native macOS ARM64 Hypervisor.Framework capability gate.
 * Exercise hv_vm_create, not just 'emulator -accel-check' availability.
 * Creates no guest code and downloads no Android or APK files.
 */
#include <Hypervisor/Hypervisor.h>
#include <stdio.h>

int main(void) {
    hv_return_t result = hv_vm_create(NULL);
    if (result == HV_UNSUPPORTED) {
        fprintf(stderr, "BLOCKED_HVF_UNSUPPORTED rc=0x%x\n", (unsigned)result);
        return 2;
    }
    if (result != HV_SUCCESS) {
        fprintf(stderr, "BLOCKED_HVF_VM_CREATE_FAILED rc=0x%x\n", (unsigned)result);
        return 3;
    }

    hv_return_t destroyed = hv_vm_destroy();
    if (destroyed != HV_SUCCESS) {
        fprintf(stderr, "BLOCKED_HVF_VM_DESTROY_FAILED rc=0x%x\n", (unsigned)destroyed);
        return 4;
    }
    puts("HVF_VM_CREATE_SUCCESS_ANDROID_BOOT_UNTESTED");
    return 0;
}
