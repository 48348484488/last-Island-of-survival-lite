/*
 * Execute REAL compiled AArch64 instructions from the unchanged public
 * 2048-cli ELF. This LD_PRELOAD probe intercepts __libc_start_main before
 * the interactive game's main runs, and calls only addresses contained in
 * the source-blind EdgeAI ABI manifest. No game source or symbol table.
 *
 * Under qemu-aarch64 user mode, these are the ORIGINAL executable
 * instructions, not the EdgeAI instruction interpreter or generated C++.
 * Linux AArch64 syscall write/exit provides deterministic output.
 *
 * This probe is an experimental verification harness, not a decompiler.
 */
#define _GNU_SOURCE
#include <stdint.h>
#include <stddef.h>
#include <stdlib.h>
#include <string.h>

#define ROUNDS 2000U
#define SLOTS 11U
#define FNV_BASIS UINT64_C(14695981039346656037)
#define FNV_PRIME UINT64_C(1099511628211)

static uint64_t digest;
static uint32_t rng;

static long direct_write(const char *buf, unsigned long len) {
    register long x0 __asm__("x0") = 1;
    register const char *x1 __asm__("x1") = buf;
    register unsigned long x2 __asm__("x2") = len;
    register long x8 __asm__("x8") = 64;
    __asm__ volatile ("svc #0" : "+r"(x0) : "r"(x1), "r"(x2), "r"(x8) : "memory");
    return x0;
}
__attribute__((noreturn)) static void direct_exit(int code) {
    register long x0 __asm__("x0") = code;
    register long x8 __asm__("x8") = 94;
    __asm__ volatile ("svc #0" : : "r"(x0), "r"(x8) : "memory");
    __builtin_unreachable();
}
static void put_u32(uint32_t v) {
    for (int i=0;i<4;i++) {
        digest ^= (uint8_t)(v >> (i*8));
        digest *= FNV_PRIME;
    }
}
static uint32_t next_rand(void) {
    rng ^= rng << 13;
    rng ^= rng >> 17;
    rng ^= rng << 5;
    return rng;
}
static uint32_t scalar(int i) {
    static const uint32_t special[]={
        0,1,2,4,8,16,32,64,128,256,512,1024,2048,0xffffffff,
        0x80000000,65535,10,100,1000,10000,4096,8192
    };
    if ((unsigned)i<sizeof(special)/sizeof(*special))
        return special[i];
    return next_rand();
}
static void make_board(uint32_t board[16], int i) {
    for (int j=0;j<16;j++) {
        uint32_t t=next_rand();
        if (i==0) board[j]=0;
        else if(i==1) board[j]=2;
        else if(i==2) board[j]=2048;
        else if(i==3) board[j]=(uint32_t)j;
        else if(i==4) board[j]=0xffffffff;
        else if(i==5) board[j]=(j%2)?0:2048;
        else if(i==6) board[j]=(uint32_t)(j*16);
        else {
            switch(t&7) {
              case 0:board[j]=0;break;
              case 1:board[j]=2;break;
              case 2:board[j]=4;break;
              case 3:board[j]=8;break;
              case 4:board[j]=2048;break;
              case 5:board[j]=0xffffffff;break;
              case 6:board[j]=t;break;
              default:board[j]=t&255;break;
            }
        }
    }
}
static size_t append_literal(char *dst,size_t n,const char *src){
    while(*src){dst[n++]=*src++;}
    return n;
}
static size_t append_unsigned(char *dst,size_t n,unsigned v){
    char tmp[16]; int len=0;
    do { tmp[len++]=(char)('0'+v%10);v/=10; } while(v);
    while(len)dst[n++]=tmp[--len];
    return n;
}
static size_t append_hex(char *dst,size_t n,uint64_t v){
    const char *hex="0123456789abcdef";
    for(int i=15;i>=0;i--)dst[n++]=hex[(v>>(i*4))&15];
    return n;
}

static void emit_case(unsigned slot, unsigned round, uint64_t hash) {
    char line[160];size_t n=0;
    n=append_literal(line,n,"ARM64_CASE slot=");
    n=append_unsigned(line,n,slot);
    n=append_literal(line,n," case=");
    n=append_unsigned(line,n,round);
    n=append_literal(line,n," fnv64=");
    n=append_hex(line,n,hash);
    line[n++]='\n';
    if(direct_write(line,n)!=(long)n)direct_exit(82);
}
static void execute_slot(unsigned k, uintptr_t addr, unsigned role) {
    digest=FNV_BASIS;
    rng=UINT32_C(0x31415926);
    for (unsigned i=0;i<ROUNDS;i++) {
        put_u32(i);
        if(role==2) {
            uint32_t in=scalar((int)i);
            put_u32(in);
            uint32_t (*f)(uint32_t)=(void *)(uintptr_t)addr;
            uint32_t ret=f(in);
            put_u32(ret);
        } else {
            uint32_t board[16];
            make_board(board,(int)i);
            for(int j=0;j<16;j++)put_u32(board[j]);
            if(role==1){
                void (*f)(uint32_t*)=(void *)(uintptr_t)addr;
                f(board);
            }else if(role==3 || role==4) {
                uint32_t score=i*73U+11U;
                put_u32(score);
                uint32_t result;
                if(role==3){
                    uint32_t (*f)(uint32_t*,uint32_t*)=(void *)(uintptr_t)addr;
                    result=f(board,&score);
                }else{
                    uint32_t column=i&3U;
                    put_u32(column);
                    uint32_t (*f)(uint32_t*,uint32_t,uint32_t*)=(void *)(uintptr_t)addr;
                    result=f(board,column,&score);
                }
                put_u32(result);
                put_u32(score);
            }else{
                uint32_t (*f)(uint32_t*)=(void *)(uintptr_t)addr;
                uint32_t ret=f(board);
                put_u32(ret);
            }
            for(int j=0;j<16;j++)put_u32(board[j]);
        }
        emit_case(k,i,digest);
    }
    char line[256]; size_t n=0;
    n=append_literal(line,n,"ARM64_REAL_ELF slot=");
    n=append_unsigned(line,n,k);
    n=append_literal(line,n," addr=0x");
    n=append_hex(line,n,(uint64_t)addr);
    n=append_literal(line,n," role=");
    n=append_unsigned(line,n,role);
    n=append_literal(line,n," calls=");
    n=append_unsigned(line,n,ROUNDS);
    n=append_literal(line,n," fnv64=");
    n=append_hex(line,n,digest);
    line[n++]='\n';
    if (direct_write(line,n)!=(long)n) direct_exit(81);
}
/*
 * Numeric addresses and role categories are passed as environment
 * variables populated from the ELF-only EdgeAI coverage manifest.
 * No function names, rules or source-derived constants in this harness.
 */
int __libc_start_main(int (*main_function)(int,char**,char**),
    int argc, char **argv, void (*init_fn)(void), void (*fini_fn)(void),
    void (*rtld_fini)(void), void *stack_end) {
    (void)main_function;(void)argc;(void)argv;(void)init_fn;
    (void)fini_fn;(void)rtld_fini;(void)stack_end;
    for (unsigned i=0;i<SLOTS;i++) {
        char k[24]="EDGEAI_ENTRY_";
        size_t kn=13; kn=append_unsigned(k,kn,i);k[kn]=0;
        char *entry=getenv(k);
        if(!entry)direct_exit(70);
        char *end=0;
        uintptr_t addr=(uintptr_t)strtoull(entry,&end,16);
        if (!end||*end||addr<0x400000||addr>0x600000||(addr&3)) direct_exit(71);
        char r[24]="EDGEAI_ROLE_";
        size_t rn=12;rn=append_unsigned(r,rn,i);r[rn]=0;
        char *role=getenv(r);
        if(!role || role[0]<'0' || role[0]>'4'||role[1]) direct_exit(72);
        execute_slot(i,addr,(unsigned)(role[0]-'0'));
    }
    direct_exit(0);
}
