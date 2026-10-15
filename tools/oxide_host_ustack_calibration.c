/* Host-only synthetic probe. Does not interact with Android or any game.
 * Invoke kill(SIGKILL) from a symbolized, frame-pointer-enabled function;
 * the child being killed is a disposable process made by this binary.
 */
#define _POSIX_C_SOURCE 200809L
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <unistd.h>

__attribute__((noinline))
static int synthetic_send_sigkill(pid_t child) {
    fprintf(stderr, "HOST_ONLY_STACK_CALIBRATION sender=%ld target=%ld\n",
            (long)getpid(), (long)child);
    fflush(stderr);
    return kill(child, SIGKILL);
}
int main(void) {
    pid_t child = fork();
    if (child < 0) { perror("fork"); return 2; }
    if (child == 0) {
        for (;;) pause();
    }
    usleep(100000);
    int rc = synthetic_send_sigkill(child);
    int status = 0;
    waitpid(child, &status, 0);
    if (rc < 0 || !WIFSIGNALED(status) || WTERMSIG(status) != SIGKILL) {
        fprintf(stderr, "SYNTHETIC_ONLY_STACK_PROBE_FAILED\n");
        return 3;
    }
    puts("SYNTHETIC_ONLY_STACK_PROBE_PASS");
    return 0;
}
