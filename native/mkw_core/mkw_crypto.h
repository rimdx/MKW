#ifndef MKW_CRYPTO_H
#define MKW_CRYPTO_H

#include "mkw.h"

#include <nettle/rsa.h> /* for rsa_public_key and rsa_private_key */

/* out big-integer implementation */

typedef uint32_t mkw_limb_t;

#define MKW_BIGINT_BITS 512 
#define MKW_BIGINT_LIMBS (MKW_BIGINT_BITS / bitsize(mkw_limb_t))

/* 
 * a representation of a large unsigned integer number with support of
 * essential cryptographic math functions that we need to implement public key
 * cryptosystem.
 *
 * please note: the digits are in little-endian. this simplifies backward
 * indexing (what we need more often) and gives more significance to higher
 * components (limbs).
 * */
typedef struct mkw_bigint_t {
    mkw_limb_t digits[MKW_BIGINT_LIMBS];
} mkw_bigint_t;

mkw_bigint_t *mkw_bigint_create(int limbs, mkw_pool_t *pool);
mkw_bigint_t *mkw_bigint_create_empty(mkw_pool_t *pool);
mkw_bigint_t *mkw_bigint_dup(const mkw_bigint_t *n, mkw_pool_t *pool);
mkw_bigint_t *mkw_bigint_from_num(mkw_limb_t num, mkw_pool_t *pool);
mkw_bigint_t *mkw_bigint_from_limbs(const mkw_limb_t *data, mkw_limb_t size,
                                    mkw_pool_t *pool);

mkw_bigint_t *
mkw_bigint_set(mkw_bigint_t *x, const mkw_bigint_t *n);
void mkw_bigint_zero(mkw_bigint_t *x);

/*
 * Compares bigints a and b, returning zero if they are equal, positive value
 * if a is greater than b and negative in opposite scenario.
 */
int mkw_bigint_cmp(const mkw_bigint_t *a, const mkw_bigint_t *b);

void mkw_bigint_limbshift(mkw_bigint_t *x, int n);

void mkw_bigint_print(const mkw_bigint_t *num, FILE *file);

void mkw_bigint_add_n(mkw_bigint_t *x, mkw_limb_t n);
void mkw_bigint_add(mkw_bigint_t *x, const mkw_bigint_t *n);

void mkw_bigint_sub_n(mkw_bigint_t *x, mkw_limb_t n);
void mkw_bigint_sub(mkw_bigint_t *x, const mkw_bigint_t *n);

void mkw_bigint_mul_n(mkw_bigint_t *x, mkw_limb_t n);

void mkw_bigint_mul(mkw_bigint_t *x, const mkw_bigint_t *a,
                    const mkw_bigint_t *b, mkw_bigint_t *tmp);

void mkw_bigint_div(mkw_bigint_t *result, mkw_bigint_t *remainder,
                    const mkw_bigint_t *x, const mkw_bigint_t *n);

#if 1
#define MKW_BIGINT_TRACE(x)                                                   \
    fprintf(stderr,                                                           \
            "BIGINT TRACING %s:%s\t" #x "\t",                                \
            (strrchr(__FILE__, '/') ? strrchr(__FILE__, '/') + 1 : __FILE__), \
            __FUNCTION__);                                                    \
    mkw_bigint_print(x, stderr);
#else
#define MKW_BIGINT_TRACE(x)
#endif

/* Elliptic Curves Cryptosystem. */
typedef struct mkw_ecc_point_t {
    /* a ecc curve could be stuck at something often called zero or infinity
     * point. it's just a special state that should be handled. in most cases
     * infinity points are just banned and are illegal for most transformations
     * to function properly. */
    int is_infinity;
    mkw_bigint_t *x;
    mkw_bigint_t *y;
} mkw_ecc_point_t;

/* 
 * Represents the curve parameters.
 * https://en.wikipedia.org/wiki/Elliptic-curve_cryptography#Domain_parameters 
 */
typedef struct mkw_ecc_curve_t {
    /*
     * a and b constants from the curve equation.
     * y^2 (mod p) = x^3+x^a+b (mod p)
     * https://en.wikipedia.org/wiki/Elliptic-curve_cryptography#Elliptic_curve_theory
     */
    mkw_bigint_t *a;
    mkw_bigint_t *b;

    /* the starting (generator) point. it must be on the curve. */
    mkw_ecc_point_t *g;

    /* prime p (or non-prime for binray curves but we're not binray to care to
     * support them) that represents modulus of elliptic curve field. */
    mkw_bigint_t *p;
} mkw_ecc_curve;

/* https://en.wikipedia.org/wiki/Elliptic_curve_point_multiplication#Point_addition */
void
mkw_ecc_point_add(mkw_ecc_point_t *x, const mkw_ecc_point_t *n);

/* https://en.wikipedia.org/wiki/Elliptic_curve_point_multiplication#Point_doubling  */
void
mkw_ecc_point_double(mkw_ecc_point_t *x);

/*
 * point multiplication is built on repeated doubling it and adding of a point
 * to itself. similar to square-and-multiply exponentiation of large numbers
 * (which is used in RSA and not only), the same is applicable and is the basic
 * operation which the entire ECC cryptosystem relies on.
 *
 * https://en.wikipedia.org/wiki/Elliptic_curve_point_multiplication#Double-and-add
 */
void mkw_ecc_point_mul(mkw_ecc_point_t *x, const mkw_bigint_t *n);

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
