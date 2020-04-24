#ifndef BASE64_ENCODE__H
#define BASE64_ENCODE__H

#ifdef __cplusplus
extern "C"
{
#endif
#include <aos/kernel.h>

int base64_encode(uint8_t *in,uint32_t in_len,char **out);

#ifdef __cplusplus
}
#endif

#endif