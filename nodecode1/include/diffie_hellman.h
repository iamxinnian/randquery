#ifndef _DIFFIE_HELLMAN_H
#define _DIFFIE_HELLMAN_H

#include "app_main.h"
#include "miracl.h"
#include "mirdef.h"

int generate_privkey(uint32_t random_t,char *privkey);
int generate_key(const char *prime_num,const char *orig_root,const char *privkey,char *pubkey);
#endif