#include "aes_encrypt_ecb.h"

int encrypt_aes_ecb(aes_confg *aes_cfg,
                const uint8_t* in, uint32_t in_len,
                uint8_t* out, uint32_t* out_len)
{
    int ret = 0;
    size_t ctx_size;
    void* aes_ctx = NULL;
    ali_crypto_result result;

    result = ali_aes_get_ctx_size(AES_ECB, &ctx_size);
    if (result != ALI_CRYPTO_SUCCESS) {
        return -1;
    }

    aes_ctx = ls_osa_malloc(ctx_size);
    if (aes_ctx == NULL) {
        printf("out of mem, %d\n", (int)ctx_size);
        ret = -1;
        goto _out;
    }

    result = ali_aes_init(AES_ECB, aes_cfg->is_enc, aes_cfg->key, NULL, aes_cfg->key_len, NULL, aes_ctx);
    if (result != ALI_CRYPTO_SUCCESS) {
        printf("aes init fail, 0x%x\n", result);
        ret = -1;
        goto _out;
    }

    result = ali_aes_process(in, out, in_len, aes_ctx);
    if (result != ALI_CRYPTO_SUCCESS) {
        printf("aes process fail, 0x%x\n", result);
        ret = -1;
        goto _out;
    }

    result = ali_aes_finish(NULL, 0, NULL, NULL, SYM_NOPAD, aes_ctx);
    if (result != ALI_CRYPTO_SUCCESS) {
        printf("aes finish fail, 0x%x\n", result);
        ret = -1;
        goto _out;
    }

    *out_len = in_len;

_out:
    ls_osa_free(aes_ctx);

    return ret;
}