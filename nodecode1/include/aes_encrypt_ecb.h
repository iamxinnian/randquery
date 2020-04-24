#ifndef AES_ENCRYPT_ECB__H
#define AES_ENCRYPT_ECB__H

#ifdef __cplusplus
extern "C"
{
#endif
#include "ali_crypto.h"

struct aes_config
{
    uint8_t* key;
    uint32_t key_len;
    bool is_enc;
};
typedef struct aes_config aes_confg;
int encrypt_aes_ecb(aes_confg *aes_cfg,const uint8_t* in, uint32_t in_len,uint8_t* out, uint32_t* out_len);
#ifdef __cplusplus
}
#endif

#endif