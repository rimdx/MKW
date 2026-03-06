#include "mkw.h"
#include "mkw_crypto.h"

#include <nettle/memxor.h>
#include <nettle/sha1.h>

void
mkw_sha1(uint8_t *digest, const uint8_t *data, size_t size)
{
    struct sha1_ctx checksum;

    nettle_sha1_init(&checksum);
    nettle_sha1_update(&checksum, size, data);
    nettle_sha1_digest(&checksum, SHA1_DIGEST_SIZE, digest);
}

/* 
 * Streamly CFB feedback implementation.
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
 * We don't really care about non-aes implications. Hence hardcode the
 * blocksize for convenience.
 *
 * There is no function to finalise context when decrypting because ciphertext
 * must be aligned to blocksize.
 */
void
mkw_cfb_ctx_encrypt_block(mkw_cfb_ctx_t *ctx, 
                          const uint8_t plaintext[MKW_CFB_BLOCK_SIZE],
                          uint8_t ciphertext[MKW_CFB_BLOCK_SIZE])
{
    ctx->cipher_fn(ctx->cipher_ctx, MKW_CFB_BLOCK_SIZE,
                   ciphertext /* dst */, ctx->p /* src */);
    nettle_memxor(ciphertext, plaintext, MKW_CFB_BLOCK_SIZE);
    memcpy(ctx->p, ciphertext, MKW_CFB_BLOCK_SIZE);
}

void
mkw_cfb_ctx_encrypt_final(mkw_cfb_ctx_t *ctx, 
                          size_t length,
                          const uint8_t plaintext[length],
                          uint8_t ciphertext[MKW_CFB_BLOCK_SIZE])
{
    assert(length <= MKW_CFB_BLOCK_SIZE);
    if (length > 0) {
        ctx->cipher_fn(ctx->cipher_ctx, MKW_CFB_BLOCK_SIZE,
                       ciphertext /* dst */, ctx->p /* src */);
        nettle_memxor(ciphertext, plaintext, MKW_CFB_BLOCK_SIZE);
    }
    /* nuke ctx because it should never be used after finalised */
    memset(ctx, 0, sizeof(*ctx));
}

void
mkw_cfb_ctx_decrypt_block(mkw_cfb_ctx_t *ctx, 
                          const uint8_t ciphertext[MKW_CFB_BLOCK_SIZE],
                          uint8_t plaintext[MKW_CFB_BLOCK_SIZE])
{
    ctx->cipher_fn(ctx->cipher_ctx, MKW_CFB_BLOCK_SIZE,
                   plaintext /* dst */, ctx->p /* src */);
    nettle_memxor(plaintext, ciphertext, MKW_CFB_BLOCK_SIZE);
    memcpy(ctx->p, ciphertext, MKW_CFB_BLOCK_SIZE);
}

/* symkey encryption primitives */
void
mkw_symkey_encrypt(const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   size_t size)
{
    struct aes128_ctx aesctx = { 0 };
    mkw_cfb_ctx_t cfb = {
        .p = { 0 },
        .cipher_fn = (nettle_cipher_func *)nettle_aes128_encrypt,
        .cipher_ctx = &aesctx,
    };
    uint8_t *buf;

    nettle_aes128_set_encrypt_key(&aesctx, key->key);

    while (size > MKW_CFB_BLOCK_SIZE)
    {
        buf = mkw_membuf_write_buf(out, MKW_CFB_BLOCK_SIZE);
        mkw_cfb_ctx_encrypt_block(&cfb, data, buf);

        size -= MKW_CFB_BLOCK_SIZE;
        data += MKW_CFB_BLOCK_SIZE;
    }

    assert(size > 0);
    buf = mkw_membuf_write_buf(out, MKW_CFB_BLOCK_SIZE);
    mkw_cfb_ctx_encrypt_final(&cfb, size, data, buf);
}

mkw_error_t
mkw_symkey_decrypt(const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   size_t size) 
{
    struct aes128_ctx aesctx = { 0 };
    mkw_cfb_ctx_t cfb = {
        .p = { 0 },
        .cipher_fn = (nettle_cipher_func *)nettle_aes128_decrypt,
        .cipher_ctx = &aesctx,
    };
    uint8_t *buf;

    if (size % MKW_CFB_BLOCK_SIZE != 0) {
        return MKW_ERROR_BAD_BLOCK_SIZE;
    }

    nettle_aes128_set_decrypt_key(&aesctx, key->key);

    while (size >= MKW_CFB_BLOCK_SIZE)
    {
        buf = mkw_membuf_write_buf(out, MKW_CFB_BLOCK_SIZE);
        mkw_cfb_ctx_decrypt_block(&cfb, data, buf);

        size -= MKW_CFB_BLOCK_SIZE;
        data += MKW_CFB_BLOCK_SIZE;
    }

    assert(size == 0);
    return MKW_ERROR_NONE;
}

void
mkw_symkey_protected_encrypt(const mkw_symkey_aes_t *key,
                             mkw_membuf_t *out,
                             const uint8_t *data,
                             size_t size,
                             mkw_ctx_t *ctx, mkw_pool_t *pool) 
{
    mkw_membuf_t *mdp = mkw_membuf_create_empty(pool);

    mkw_mdp_write(ctx, mdp, data, size);
    mkw_symkey_encrypt(key, out, mdp->data, mdp->size);
}

mkw_error_t
mkw_symkey_protected_decrypt(const mkw_symkey_aes_t *key,
                             mkw_membuf_t *plaintext,
                             const uint8_t *data,
                             size_t size,
                             mkw_ctx_t *ctx, mkw_pool_t *pool) 
{
    mkw_membuf_t *mdp = mkw_membuf_create_empty(pool);
    mkw_memreader_t *reader;

    mkw_symkey_decrypt(key, mdp, data, size);
    reader = mkw_memreader_create(mdp->data, mdp->size, pool);
    MKW_ERR(mkw_mdp_read(reader, plaintext));

    return MKW_ERROR_NONE;
}

/*
 * The secret key data packet is what we store encrypted for each key/user.
 * It consists of the follwing parts:
 *
 * 1) MPIs that represents private key. The mkw_pgp_seckeydata_encode and
 * mkw_pgp_seckeydata_decode methods are responsible to deal with it. Sound
 * easy? It will turn into hell soon. We're just starting.
 *
 * 2) Append SHA1 checksum of that data. There are more variations like new
 * AEAD and deprecated V3 packets that we don't really care about so we stick
 * with that one only.
 *
 * 3) When decrypting, we must first eat all MPIs utilising their lengths. Then
 * a checksum must follow. 20 bytes are read and compared to what the data
 * actually was.
 *
 * 4) The symkey is derived using s2k (string to key). It's usage is described
 * as first octect of seckey packet. In our case, it's always 254. Then finally
 * goes s2k description.
 */

void
mkw_pgp_seckeydata_encode(mkw_membuf_t *buf,
                          const mkw_seckey_rsa_t *seckey)
{
    mkw_membuf_write_mpi(buf, seckey->d);
    mkw_membuf_write_mpi(buf, seckey->p);
    mkw_membuf_write_mpi(buf, seckey->q);
    mkw_membuf_write_mpi(buf, seckey->c);
}

mkw_error_t
mkw_pgp_seckeydata_decode(mkw_memreader_t *reader,
                          mkw_seckey_rsa_t *seckey)
{
    mpz_t p1, q1;

    MKW_ERR(mkw_memreader_read_mpi(reader, seckey->d));
    MKW_ERR(mkw_memreader_read_mpi(reader, seckey->p));
    MKW_ERR(mkw_memreader_read_mpi(reader, seckey->q));
    MKW_ERR(mkw_memreader_read_mpi(reader, seckey->c));

    mpz_init(p1);
    mpz_init(q1); 

    mpz_sub_ui(p1, seckey->p, 1);
    mpz_sub_ui(q1, seckey->q, 1);

    /* a = d % (p-1) */
    mpz_fdiv_r(seckey->a, seckey->d, p1);

    /* b = d % (q-1) */
    mpz_fdiv_r(seckey->b, seckey->d, q1);

    return MKW_ERROR_NONE;
}

void
mkw_pgp_seckeydata_encrypt(mkw_membuf_t *buf,
                           const mkw_s2k_t *s2k,
                           const uint8_t *passwd,
                           size_t passwdsize,
                           const mkw_seckey_rsa_t *seckey,
                           mkw_pool_t *pool)
{
    uint8_t sha1[SHA1_DIGEST_SIZE];
    mkw_membuf_t *data = mkw_membuf_create_empty(pool);
    mkw_symkey_aes_t symkey;

    /* prepare data to encrypt */
    mkw_pgp_seckeydata_encode(data, seckey);
    mkw_sha1(sha1, data->data, data->size);
    mkw_membuf_write_str(data, sha1, sizeof(sha1));
    
    mkw_base16_dump(stderr, data->data, data->size);

    mkw_s2k_derive_key(s2k, passwd, passwdsize,
                       symkey.key, sizeof(symkey.key));

    /* write everything to the output */
    mkw_membuf_write_uint8(buf, MKW_S2K_USAGE_SOME_SHA1);
    mkw_pgp_s2k_serialize(buf, s2k);
    mkw_symkey_encrypt(&symkey, buf, data->data, data->size);
}

mkw_error_t
mkw_pgp_seckeydata_decrypt(mkw_memreader_t *reader,
                           mkw_s2k_t *s2k,
                           const uint8_t *passwd,
                           size_t passwdsize,
                           mkw_seckey_rsa_t *seckey,
                           mkw_pool_t *pool)
{
    uint8_t sha1_computed[SHA1_DIGEST_SIZE];
    uint8_t sha1_packet[SHA1_DIGEST_SIZE];
    uint8_t s2k_usage;
    mkw_symkey_aes_t symkey;
    mkw_membuf_t *plaintext = mkw_membuf_create_empty(pool);

    mkw_memreader_t payload_reader = { 0 };
    const uint8_t *payload_start;
    size_t payload_size;

    /* read encrypted things as they are */
    MKW_ERR(mkw_memreader_read_uint8(reader, &s2k_usage));
    MKW_ERR(mkw_pgp_s2k_deserialize(reader, s2k));

    if (s2k_usage != MKW_S2K_USAGE_SOME_SHA1) {
        return MKW_ERROR_BAD_S2K_USAGE;
    }

    mkw_s2k_derive_key(s2k, passwd, passwdsize,
                       symkey.key, sizeof(symkey.key));

    /* decrypt and consume everything from the reader */
    MKW_ERR(mkw_symkey_decrypt(&symkey, plaintext,
                               reader->data, reader->remaining));
    reader->data += reader->remaining;
    reader->remaining = 0;

    mkw_base16_dump(stderr, plaintext->data, plaintext->size);

    payload_start = plaintext->data;
    payload_reader.data = plaintext->data;
    payload_reader.remaining = plaintext->size;

    MKW_ERR(mkw_pgp_seckeydata_decode(&payload_reader, seckey));

    mkw_base16_dump(stderr, plaintext->data, plaintext->size);

    payload_size = payload_reader.data - payload_start;

    /* verify checksum before decoding plaintext payload body */
    MKW_ERR(mkw_memreader_read_buf(&payload_reader,
                                   sha1_packet, sizeof(sha1_packet)));
    mkw_sha1(sha1_computed, payload_start, payload_size);
    if (memcmp(sha1_computed, sha1_packet, SHA1_DIGEST_SIZE) != 0) {
        return MKW_ERROR_MDP_BAD_CHECKSUM;
    }

    return MKW_ERROR_NONE;
}

/* modification detection code */
#define MKW_MDP_SALT_SIZE 16
#define MKW_MDP_TAG (uint16_t)0xd314

void
mkw_mdp_write(mkw_ctx_t *ctx, mkw_membuf_t *mdp,
              const uint8_t *data, const uint8_t len)
{
    /* prepends random salt+two last bytes */
    nettle_yarrow256_random(&ctx->rng, MKW_MDP_SALT_SIZE,
                            mkw_membuf_write_buf(mdp, MKW_MDP_SALT_SIZE));
    mkw_membuf_write_uint8(mdp, mdp->data[MKW_MDP_SALT_SIZE - 2]);
    mkw_membuf_write_uint8(mdp, mdp->data[MKW_MDP_SALT_SIZE - 1]);

    /* the dat itself */
    mkw_membuf_write_str(mdp, data, len);

    /* and the SHA1 checksum with a tag */
    mkw_membuf_write_uint16(mdp, MKW_MDP_TAG);
    mkw_sha1(mkw_membuf_write_buf(mdp, SHA1_DIGEST_SIZE), data, len);
}
 
mkw_error_t
mkw_mdp_read(mkw_memreader_t *reader, mkw_membuf_t *plaintext)
{
    uint8_t salt[MKW_MDP_SALT_SIZE];
    uint8_t checksum_computed[SHA1_DIGEST_SIZE];
    uint8_t checksum_mdc[SHA1_DIGEST_SIZE];
    uint8_t b0, b1;
    uint16_t tag;
    uint8_t *plaintext_start = plaintext->data;
    size_t plaintext_size;

    MKW_ERR(mkw_memreader_read_buf(reader, salt, sizeof(salt)));
    MKW_ERR(mkw_memreader_read_uint8(reader, &b0));
    MKW_ERR(mkw_memreader_read_uint8(reader, &b1));

    if (b0 != salt[sizeof(salt) - 2] ||
        b1 != salt[sizeof(salt) - 1]) {
        return MKW_ERROR_MDP_BAD_QUICK_CHECK;
    }

    plaintext_size = reader->remaining - SHA1_DIGEST_SIZE - sizeof(MKW_MDP_TAG);
    MKW_ERR(mkw_memreader_read_buf(
                reader,
                mkw_membuf_write_buf(plaintext, plaintext_size),
                plaintext_size));

    MKW_ERR(mkw_memreader_read_uint16(reader, &tag));
    if (tag != MKW_MDP_TAG) {
        return MKW_ERROR_MDP_BAD_TAG;
    }

    MKW_ERR(mkw_memreader_read_buf(reader, checksum_mdc,
                                   sizeof(checksum_computed)));
    mkw_sha1(checksum_computed, plaintext_start, plaintext_size);
    if (memcmp(checksum_computed, checksum_mdc, SHA1_DIGEST_SIZE)) {
        return MKW_ERROR_MDP_BAD_CHECKSUM;
    }

    assert(reader->remaining == 0);

    return MKW_ERROR_NONE;
}
