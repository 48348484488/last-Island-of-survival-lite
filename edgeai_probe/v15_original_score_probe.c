/* Test-only source-independent call shim for the stripped 2048 AArch64 ELF.
 * LD_PRELOAD interposes __libc_start_main before the game's main function.
 * The address and void(uint32_t) ABI come from the stripped ELF analysis.
 */
#define _GNU_SOURCE
#include <errno.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

__attribute__((noreturn)) static void done(int status) {
    fflush(stdout);
    _exit(status);
}

int __libc_start_main(int (*real_main)(int, char **, char **), int argc,
                      char **argv, void (*initfn)(void), void (*finifn)(void),
                      void (*rtldfn)(void), void *stack) {
    (void)real_main; (void)argc; (void)argv;
    (void)initfn; (void)finifn; (void)rtldfn; (void)stack;
    const char *address = getenv("EDGEAI_ENTRY");
    const char *sequence = getenv("EDGEAI_SEQUENCE");
    if (!address || strcmp(address, "0x4012d0") != 0 || !sequence || !*sequence)
        done(71);
    void (*candidate)(uint32_t) = (void *)(uintptr_t)0x4012d0UL;
    unsigned calls = 0;
    while (*sequence) {
        if (++calls > 256) done(72);
        if (*sequence < '0' || *sequence > '9') done(73);
        errno = 0;
        char *end = NULL;
        unsigned long long value = strtoull(sequence, &end, 10);
        if (errno || end == sequence || value > UINT32_MAX) done(74);
        if (*end != ':' && *end != '\0') done(75);
        candidate((uint32_t)value);
        if (*end == '\0') break;
        sequence = end + 1;
        if (!*sequence) done(76);
    }
    done(0);
}
