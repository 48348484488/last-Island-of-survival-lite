/*
 * Minimal runtime hook for targeted source-blind ARM64 machine trace:
 * the original ELF's code at address EDGEAI_DEBUG_ENTRY is called exactly
 * once with the 16-word input corresponding to EDGEAI_DEBUG_CASE.
 *
 * The original executable is unchanged. The QEMU -singlestep/-d cpu,in_asm
 * command captures actual PC, X registers, SP and PSTATE at the *machine*
 * instruction level, independent of EdgeAI's C++ reconstruction.
 */
#define _GNU_SOURCE
#include <stdint.h>
#include <stdlib.h>
#include <stddef.h>

static long sys_write(const char *buf, size_t len) {
    register long x0 __asm__("x0") = 1;
    register const char *x1 __asm__("x1") = buf;
    register unsigned long x2 __asm__("x2") = len;
    register long x8 __asm__("x8") = 64;
    __asm__ volatile ("svc #0" : "+r"(x0) : "r"(x1), "r"(x2), "r"(x8) : "memory");
    return x0;
}
__attribute__((noreturn)) static void sys_exit(int result) {
    register long x0 __asm__("x0") = result;
    register long x8 __asm__("x8") = 94;
    __asm__ volatile ("svc #0" : : "r"(x0), "r"(x8) : "memory");
    __builtin_unreachable();
}
static void send(const char *str) {
    size_t n=0; while(str[n])n++;
    if(sys_write(str,n)!=(long)n)sys_exit(92);
}
static void send_uint(unsigned v) {
    char tmp[15];int i=0;
    do {tmp[i++]=(char)('0'+v%10);v/=10;}while(v);
    char out[15];for(int j=0;j<i;j++)out[j]=tmp[i-1-j];
    if(sys_write(out,(size_t)i)!=(long)i)sys_exit(93);
}
static void send_hex(uint64_t n) {
    char out[16];const char *digits="0123456789abcdef";
    for(unsigned i=0;i<16;i++)out[i]=digits[(n>>((15-i)*4))&15];
    if(sys_write(out,16)!=16)sys_exit(94);
}
int __libc_start_main(int (*real_main)(int,char**,char**),
    int argc, char **argv, void (*initfn)(void),void (*finifn)(void),
    void (*rtldfn)(void),void *stack) {
    (void)real_main;(void)argc;(void)argv;(void)initfn;(void)finifn;
    (void)rtldfn;(void)stack;
    char *str=getenv("EDGEAI_DEBUG_ENTRY");
    if(!str)sys_exit(70);
    char *end=0;
    uintptr_t entry=(uintptr_t)strtoull(str,&end,16);
    if(!end||*end||entry<0x400000||entry>0x600000||(entry&3))sys_exit(71);
    str=getenv("EDGEAI_DEBUG_CASE");
    if(!str)sys_exit(72);
    unsigned long selected=strtoul(str,&end,10);
    if(!end||*end||selected>100000UL)sys_exit(73);
    uint32_t board[16];
    if(selected==0){for(unsigned i=0;i<16;i++)board[i]=0;}
    else if(selected==1){for(unsigned i=0;i<16;i++)board[i]=2;}
    else if(selected==2){for(unsigned i=0;i<16;i++)board[i]=2048;}
    else if(selected==3){for(unsigned i=0;i<16;i++)board[i]=i;}
    else if(selected==4){for(unsigned i=0;i<16;i++)board[i]=UINT32_MAX;}
    else sys_exit(74); /* Target only known, fully bounded edge inputs. */
    send("EDGEAI_MACHINE_DEBUG case=");send_uint((unsigned)selected);
    send(" entry=0x");send_hex(entry);send(" board=[");
    for(unsigned i=0;i<16;i++){
        if(i)send(",");
        send_uint(board[i]);
    }
    send("]\n");
    uint32_t (*target)(uint32_t*)=(void *)(uintptr_t)entry;
    uint32_t return_value=target(board);
    send("EDGEAI_MACHINE_RESULT case=");send_uint((unsigned)selected);
    send(" return=");send_uint(return_value);
    send(" board_sha_fnv64=");
    uint64_t digest=UINT64_C(14695981039346656037);
    for(unsigned i=0;i<16;i++) {
        for(unsigned b=0;b<4;b++){
            digest^=(uint8_t)(board[i]>>(8*b));
            digest*=UINT64_C(1099511628211);
        }
    }
    send_hex(digest);
    send("\n");
    sys_exit(0);
}
