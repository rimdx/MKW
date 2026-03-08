#include "mkw.h"
#include "mkw_crypto.h"

/* 
 * Streamly CFB feedback implementation.
 *
 * RFC: https://www.rfc-editor.org/rfc/rfc3826#section-3.1.3
 *
 * The nettle implementation does not support streaming and I don't really like
 * it in general.
 *
 * The CFB cipher has some kind of context p which is initialized with the
 * initilal vector. It's updated on every block. To encrypt a block, take that
 * p and buink it though block cipher algorithm. The xor its output with new
 * plaintext block. This will produce a block of ciphertext which is then saved
 * into p.
 *
 * Decryption is done in basically the same way except that after block
 * encrytion of internal p bufer is xored with plaintext (instead of
 * ciphertext). This (the ciphertext) then becomes a new p (same as in
 * encryption).
 *
 * Important note: for both decryption and encryption, no matter where it
 * transforms the data, we still use AES encrypt primitive. It does the job
 * into both sides.
 *
 * We don't really care about non-aes implications. Hence hardcode the
 * blocksize for convenience.
 *
 * There is no function to finalise context when decrypting because ciphertext
 * must be aligned to blocksize.
 */

void
mkw_cfb_ctx_encrypt_block(mkw_cfb_ctx_t *ctx, 
                          uint8_t block[MKW_CFB_BLOCK_SIZE])
{
    uint8_t buf[MKW_CFB_BLOCK_SIZE];
    mkw_aes_ctx_t aes;

    /* buf is what we want to encrypt. it's filled with the 'p'.
     * mkw_aes_encrypt_block() does encryption in-places so it nukes the given
     * block and writes the result there. this variables also what we need to
     * return from the function and save to the context */

    memcpy(buf, ctx->p, MKW_CFB_BLOCK_SIZE);

    mkw_aes_init(&aes, ctx->key);
    mkw_aes_encrypt_block(&aes, buf);

    for (size_t i = 0; i < MKW_CFB_BLOCK_SIZE; i++)
        block[i] ^= buf[i];

    memcpy(ctx->p, block, MKW_CFB_BLOCK_SIZE);
}

void
mkw_cfb_ctx_encrypt_final(mkw_cfb_ctx_t *ctx, 
                          uint8_t *block, size_t length)
{
    assert(length <= MKW_CFB_BLOCK_SIZE);

    if (length > 0) {
        uint8_t buf[MKW_CFB_BLOCK_SIZE];
        mkw_aes_ctx_t aes;

        memcpy(buf, ctx->p, MKW_CFB_BLOCK_SIZE);

        mkw_aes_init(&aes, ctx->key);
        mkw_aes_encrypt_block(&aes, buf);

        for (size_t i = 0; i < length; i++)
            block[i] ^= buf[i];
    }

    /* nuke ctx because it should never be used after finalised */
    memset(ctx, 0, sizeof(*ctx));
}

void
mkw_cfb_ctx_encrypt_full(mkw_cfb_ctx_t *cfb, mkw_membuf_t *out,
                         const uint8_t *data, size_t size)
{
    uint8_t buf[MKW_CFB_BLOCK_SIZE];

    while (size > MKW_CFB_BLOCK_SIZE)
    {
        memcpy(buf, data, sizeof(buf));
        mkw_cfb_ctx_encrypt_block(cfb, buf);
        mkw_membuf_write_str(out, buf, sizeof(buf));

        size -= MKW_CFB_BLOCK_SIZE;
        data += MKW_CFB_BLOCK_SIZE;
    }

    assert(size > 0);

    memcpy(buf, data, size);
    mkw_cfb_ctx_encrypt_final(cfb, buf, size);
    mkw_membuf_write_str(out, buf, size);
}

void
mkw_cfb_ctx_decrypt_block(mkw_cfb_ctx_t *ctx, 
                          uint8_t block[MKW_CFB_BLOCK_SIZE])
{
    uint8_t buf[MKW_CFB_BLOCK_SIZE];
    mkw_aes_ctx_t aes;

    memcpy(buf, ctx->p, MKW_CFB_BLOCK_SIZE);

    /* it's not a mistake. we should use "encrypt" even when decrypting */
    mkw_aes_init(&aes, ctx->key);
    mkw_aes_encrypt_block(&aes, buf);

    memcpy(ctx->p, block, MKW_CFB_BLOCK_SIZE);

    for (size_t i = 0; i < MKW_CFB_BLOCK_SIZE; i++)
        block[i] ^= buf[i];
}

void
mkw_cfb_ctx_decrypt_final(mkw_cfb_ctx_t *ctx,
                          uint8_t block[], size_t size)
{
    assert(size <= MKW_CFB_BLOCK_SIZE);

    if (size > 0) {
        uint8_t buf[MKW_CFB_BLOCK_SIZE];
        mkw_aes_ctx_t aes;

        memcpy(buf, ctx->p, MKW_CFB_BLOCK_SIZE);

        mkw_aes_init(&aes, ctx->key);
        mkw_aes_encrypt_block(&aes, buf);

        for (size_t i = 0; i < size; i++)
            block[i] ^= buf[i];
    }

    /* nuke ctx because it should never be used after finalised */
    memset(ctx, 0, sizeof(*ctx));
}

void
mkw_cfb_ctx_decrypt_full(mkw_cfb_ctx_t *cfb, mkw_membuf_t *out,
                         const uint8_t *data, size_t size)
{
    uint8_t buf[MKW_CFB_BLOCK_SIZE];

    while (size > MKW_CFB_BLOCK_SIZE)
    {
        memcpy(buf, data, sizeof(buf));
        mkw_cfb_ctx_decrypt_block(cfb, buf);
        mkw_membuf_write_str(out, buf, sizeof(buf));

        size -= MKW_CFB_BLOCK_SIZE;
        data += MKW_CFB_BLOCK_SIZE;
    }

    memcpy(buf, data, size);
    mkw_cfb_ctx_decrypt_final(cfb, buf, size);
    mkw_membuf_write_str(out, buf, size);
}

void
mkw_cfb_ctx_init(mkw_cfb_ctx_t *cfb,
                 const uint8_t key[AES128_KEY_SIZE],
                 const uint8_t iv[MKW_CFB_BLOCK_SIZE])
{
    memcpy(cfb->key, key, sizeof(cfb->key));
    memcpy(cfb->p, iv, sizeof(cfb->p));
}
