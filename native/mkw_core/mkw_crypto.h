#ifndef MKW_CRYPTO_H
#define MKW_CRYPTO_H

#include "mkw.h"

#include <nettle/rsa.h> /* for rsa_public_key and rsa_private_key */

/* digest utilities */
enum mkw_hash_tag_e {
    mkw_hash_tag_md5 = 1,
    mkw_hash_tag_sha1 = 2,
    mkw_hash_tag_sha256 = 8,
    mkw_hash_tag_sha384 = 9,
    mkw_hash_tag_sha512 = 10,
    mkw_hash_tag_sha224 = 11,
};

void
mkw_sha1(uint8_t *digest, const uint8_t *data, size_t size);

/* AES context */
#define MKW_AES_BLOCK_SIZE 16 
#define MKW_AES_KEY_SIZE 16   

typedef struct mkw_aes_ctx_t {
  uint8_t RoundKey[176];
} mkw_aes_ctx_t;

void
mkw_aes_init(mkw_aes_ctx_t *ctx,
             const uint8_t key[MKW_AES_KEY_SIZE]);

void
mkw_aes_encrypt_block(mkw_aes_ctx_t *ctx, 
                      uint8_t block[MKW_AES_BLOCK_SIZE]);

void
mkw_aes_decrypt_block(mkw_aes_ctx_t *ctx,
                      uint8_t block[MKW_AES_BLOCK_SIZE]);

/*
 * string to key idendifier 
 *
 * RFC: https://www.rfc-editor.org/rfc/rfc4880#section-3.7
 * - https://www.rfc-editor.org/rfc/rfc4880#section-5.5.3 (Secret-Key Packet)
 */
enum mkw_s2k_tag_e {
    mkw_s2k_tag_plaintext = -1,
    mkw_s2k_tag_simple = 0,
    mkw_s2k_tag_salted = 1,
    mkw_s2k_tag_salted_iterated = 3,
};

typedef struct mkw_s2k_t {
    enum mkw_s2k_tag_e tag;
    enum mkw_hash_tag_e hash;
    uint8_t salt[8];
    uint8_t count;
} mkw_s2k_t;

#define MKW_S2K_USAGE_PLAINTEXT (uint8_t)0
#define MKW_S2K_USAGE_SOME_SHA1 (uint8_t)254
#define MKW_S2K_USAGE_SOME_HASH (uint8_t)255

void
mkw_s2k_init(mkw_ctx_t *ctx, mkw_s2k_t *s2k);

void
mkw_s2k_derive_key(const mkw_s2k_t *s2k,
                   const char *passwd,
                   uint8_t *key, size_t keysize);

void
mkw_pgp_s2k_serialize(mkw_membuf_t *buf,
                      const mkw_s2k_t *s2k);

mkw_error_t
mkw_pgp_s2k_deserialize(mkw_memreader_t *reader,
                        mkw_s2k_t *s2k);

/* symkey */
typedef struct mkw_symkey_aes128_t {
    uint8_t key[128 / 8];
} mkw_symkey_aes_t;

void
mkw_symkey_encrypt(const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   size_t size);

void
mkw_symkey_decrypt(const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   size_t size);

void
mkw_symkey_protected_encrypt(const mkw_symkey_aes_t *key,
                             mkw_membuf_t *out,
                             const uint8_t *data,
                             size_t size,
                             mkw_ctx_t *ctx, mkw_pool_t *pool);

mkw_error_t
mkw_symkey_protected_decrypt(const mkw_symkey_aes_t *key,
                             mkw_membuf_t *plaintext,
                             const uint8_t *data,
                             size_t size,
                             mkw_ctx_t *ctx, mkw_pool_t *pool);

/* pubkey cryptography */
typedef struct rsa_public_key mkw_pubkey_rsa_t;
typedef struct rsa_private_key mkw_seckey_rsa_t;

/* CFB feedback mode */
#define MKW_CFB_BLOCK_SIZE AES_BLOCK_SIZE

typedef struct mkw_cfb_ctx_t {
    uint8_t p[MKW_CFB_BLOCK_SIZE];
    uint8_t key[MKW_AES_KEY_SIZE];
} mkw_cfb_ctx_t;

void
mkw_cfb_ctx_encrypt_block(mkw_cfb_ctx_t *ctx, 
                          uint8_t block[MKW_CFB_BLOCK_SIZE]);

void
mkw_cfb_ctx_encrypt_final(mkw_cfb_ctx_t *ctx, 
                          uint8_t *block, size_t length);

void
mkw_cfb_ctx_encrypt_full(mkw_cfb_ctx_t *cfb, mkw_membuf_t *out,
                         const uint8_t *data, size_t size);

void
mkw_cfb_ctx_decrypt_block(mkw_cfb_ctx_t *ctx, 
                          uint8_t block[MKW_CFB_BLOCK_SIZE]);

void
mkw_cfb_ctx_decrypt_final(mkw_cfb_ctx_t *ctx, 
                          uint8_t block[], size_t size);


void
mkw_cfb_ctx_decrypt_full(mkw_cfb_ctx_t *cfb, mkw_membuf_t *out,
                         const uint8_t *data, size_t size);

void
mkw_cfb_ctx_init(mkw_cfb_ctx_t *cfb,
                 const uint8_t key[AES128_KEY_SIZE],
                 const uint8_t iv[MKW_CFB_BLOCK_SIZE]);

/* Modification Detection Code (MDC). */

void
mkw_mdp_write(mkw_ctx_t *ctx, mkw_membuf_t *mdp,
              const uint8_t *data, const uint8_t len);
 
mkw_error_t
mkw_mdp_read(mkw_memreader_t *reader, mkw_membuf_t *plaintext);


void
mkw_pgp_seckeydata_encode(mkw_membuf_t *buf,
                          const mkw_seckey_rsa_t *seckey);

mkw_error_t
mkw_pgp_seckeydata_decode(mkw_memreader_t *reader,
                          mkw_seckey_rsa_t *seckey);

void
mkw_pgp_seckeydata_encrypt(mkw_membuf_t *buf,
                           const mkw_s2k_t *s2k,
                           const char *passwd,
                           const mkw_seckey_rsa_t *seckey,
                           mkw_pool_t *pool);

mkw_error_t
mkw_pgp_seckeydata_decrypt(mkw_memreader_t *reader,
                           mkw_s2k_t *s2k,
                           const char *passwd,
                           mkw_seckey_rsa_t *seckey,
                           mkw_pool_t *pool);

#endif
