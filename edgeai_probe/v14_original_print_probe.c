/* Source-independent original-ELF AArch64 machine-output probe.
 * Injects at __libc_start_main; invokes the actual machine code at
 * address from ELF-derived ABI manifest; does not consult game symbols,
 * original C source, or generated C++. Used only as a test oracle.
 */
#define _GNU_SOURCE
#include <stdint.h>
#include <stddef.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>

__attribute__((noreturn)) static void done(int result){
  fflush(stdout);
  register long x0 __asm__("x0")=result;
  register long x8 __asm__("x8")=94;
  __asm__ volatile("svc #0" :: "r"(x0),"r"(x8):"memory");
  __builtin_unreachable();
}
static uint32_t number(const char*text, uint32_t max){
  if(!text || !*text)done(71);
  char*end=0; errno=0;
  unsigned long long n=strtoull(text,&end,10);
  if(errno||!end||*end||n>max)done(72);
  return (uint32_t)n;
}
int __libc_start_main(int(*real_main)(int,char**,char**),int argc,char**argv,
  void(*initfn)(void),void(*finifn)(void),void(*rtldfn)(void),void*stack){
  (void)real_main;(void)argc;(void)argv;(void)initfn;(void)finifn;(void)rtldfn;(void)stack;
  const char* address=getenv("EDGEAI_ENTRY");
  if(!address)done(73);
  char*end=0;
  unsigned long long entry=strtoull(address,&end,16);
  if(!end||*end||entry<0x400000ULL||entry>0x600000ULL||(entry&3))done(74);
  const char*mode=getenv("EDGEAI_MODE");
  if(!mode)done(75);
  if(!strcmp(mode,"CELL")){
    uint32_t value=number(getenv("EDGEAI_VALUE"),9999);
    uint8_t row=(uint8_t)number(getenv("EDGEAI_ROW"),3);
    uint8_t col=(uint8_t)number(getenv("EDGEAI_COL"),3);
    void(*fn)(uint32_t,uint8_t,uint8_t)=(void*)(uintptr_t)entry;
    fn(value,row,col);
  }else if(!strcmp(mode,"BOARD")){
    const char*next=getenv("EDGEAI_BOARD");
    if(!next)done(76);
    uint32_t board[16];
    for(int i=0;i<16;i++){
      char *tail=0;errno=0;
      unsigned long long num=strtoull(next,&tail,10);
      if(errno||!tail||tail==next||num>9999)done(77);
      board[i]=(uint32_t)num;
      if(i<15){if(*tail!=',')done(78);next=tail+1;}
      else if(*tail)done(79);
    }
    void(*fn)(uint32_t*)=(void*)(uintptr_t)entry;
    fn(board);
  }else done(80);
  done(0);
}