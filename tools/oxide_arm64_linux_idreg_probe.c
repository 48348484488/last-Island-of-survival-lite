/*
 * Independent ARM64 Linux EL0 opcode probe.
 * Executes precisely 0xd5380429 (MRS X9, ID_AA64PFR1_EL1) in this small
 * standalone process. Never links to, loads, patches, or executes the game.
 * Its result does NOT establish Android behavior or a security-policy cause.
 */
#define _POSIX_C_SOURCE 200809L
#include <setjmp.h>
#include <signal.h>
#include <stdint.h>
#include <stdio.h>
#include <string.h>
#include <sys/utsname.h>

static sigjmp_buf resume_after_sigill;
static volatile sig_atomic_t signal_code;

static void handle_sigill(int signal_number, siginfo_t *information, void *context) {
    (void)signal_number;
    (void)context;
    signal_code = information ? information->si_code : 0;
    siglongjmp(resume_after_sigill, 1);
}

int main(void) {
#if !defined(__aarch64__)
    fputs("This probe must be compiled natively for AArch64.\n", stderr);
    return 2;
#else
    struct utsname machine;
    if (uname(&machine) != 0 || strcmp(machine.machine, "aarch64") != 0) {
        fputs("Not running on native Linux AArch64.\n", stderr);
        return 2;
    }

    struct sigaction action;
    memset(&action, 0, sizeof(action));
    action.sa_sigaction = handle_sigill;
    action.sa_flags = SA_SIGINFO;
    sigemptyset(&action.sa_mask);
    if (sigaction(SIGILL, &action, NULL) != 0) {
        perror("sigaction");
        return 2;
    }

    if (sigsetjmp(resume_after_sigill, 1) == 0) {
        unsigned long long id_register;
        /* Disambiguates the exact opcode from compiler-chosen alternatives. */
        __asm__ volatile(".inst 0xd5380429\n\tmov %0, x9"
                         : "=r"(id_register) : : "x9", "memory");
        printf("{\"status\":\"EL0_MRS_READ_SUCCEEDED\","
               "\"opcode\":\"0xd5380429\",\"os\":\"linux\","
               "\"architecture\":\"aarch64\","
               "\"register\":\"ID_AA64PFR1_EL1\","
               "\"value_hex\":\"0x%016llx\","
               "\"android_runtime_tested\":false,"
               "\"original_game_tested\":false}\n", id_register);
    } else {
        printf("{\"status\":\"EL0_MRS_TRAPPED_SIGILL\","
               "\"opcode\":\"0xd5380429\",\"os\":\"linux\","
               "\"architecture\":\"aarch64\","
               "\"register\":\"ID_AA64PFR1_EL1\","
               "\"sigill_si_code\":%d,"
               "\"android_runtime_tested\":false,"
               "\"original_game_tested\":false}\n", (int)signal_code);
    }
    return 0;
#endif
}
