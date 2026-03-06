/*
 * Naming conventions:
 *   session - session key
 *   pubkey  - public key
 *   seckey  - private key (secret key)
 *   symkey  - symmetric key
 */

/* includes */
#include "mkw.h"
#include "mkw_crypto.h"
#include "mkw_pgp.h"
#include "mkw_storage.h"

#include <stdio.h>

#include <nettle/base16.h>
#include <nettle/base64.h>

void
mkw_base16_dump(FILE *file, const uint8_t *str, size_t len)
{
    static char *hex_table = "0123456789abcdef";

    for (int i = 0; i < len; i++) {
        if (i % 16 == 0) {
            putc('\n', file);
        }
        putc(hex_table[str[i] / 16], file);
        putc(hex_table[str[i] % 16], file);
        putc(' ', file);
    }

    putc('\n', file);
}

/* library context */
#define RNG_SEED_SIZE 256
#define RNG_DEVICE "/dev/urandom"

mkw_error_t
mkw_ctx_create(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    FILE *fdevice = NULL;
    uint8_t *buf = mkw_pcalloc(pool, RNG_SEED_SIZE);
    mkw_error_t err = MKW_ERROR_NONE;
    size_t bytes_read;

    nettle_yarrow256_init(&ctx->rng, 0, NULL);

    fdevice = fopen(RNG_DEVICE, "rb");
    if (! fdevice) {
        err = MKW_ERROR_IO;
        goto cleanup;
    }

    bytes_read = fread(buf, 1, RNG_SEED_SIZE, fdevice); 
    if (bytes_read < RNG_SEED_SIZE) {
        err = MKW_ERROR_IO;
        goto cleanup;
    }
    
    yarrow256_seed(&ctx->rng, RNG_SEED_SIZE, buf);

cleanup:
    if (fdevice) {
        fclose(fdevice);
    }
    return err;
}

typedef struct mkw_user_info_t {
    mkw_id_t id;
    mkw_pubkey_t pubkey;
} mkw_user_info_t;

typedef struct mkw_user_t {
    mkw_id_t id;
    mkw_s2k_t s2k;
    mkw_keypair_rsa_t key;
} mkw_user_t;


static mkw_error_t
mkw_user_keygen(mkw_ctx_t *ctx, mkw_user_t *user) {
    int status;

    mkw_s2k_init(ctx, &user->s2k);

    rsa_public_key_init(&user->key.pubkey);
    rsa_private_key_init(&user->key.seckey);

    status = nettle_rsa_generate_keypair(
            &user->key.pubkey,
            &user->key.seckey,
            &ctx->rng,
            (nettle_random_func *)yarrow256_random,
            NULL,   /* progress_ctx */
            NULL,   /* progress_fn */
            2048,   /* n_size */
            20      /* e_size */);

    if (! status) {
        return MKW_ERROR_RSA_KEYGEN;
    }

    return MKW_ERROR_NONE;
}

static void
mkw_user_store(mkw_blobstore_t *store,
               mkw_user_t *user,
               const uint8_t *passwd,
               size_t passwdsize,
               mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    mkw_membuf_t *buf = mkw_membuf_create_empty(pool);
    mkw_membuf_t *subbuf = mkw_membuf_create_empty(pool); 
    mkw_symkey_aes_t symkey = { 0 };
    mkw_blobstore_entry_t *entry;

    mkw_pubkey_t pubkey = {
        .time_created = 0,
        .expires_in_days = 0,
        .material = user->key.pubkey,
    };

    mkw_pgp_pubkey_serialize(subbuf, &pubkey);
    mkw_pgp_seckeydata_encrypt(subbuf, &user->s2k, passwd, passwdsize,
                               &user->key.seckey, pool);
    mkw_pgp_packet_serialize(buf, mkw_pgp_packet_seckey,
                             subbuf->data, subbuf->size);

    /* security consideration */
    memset(&symkey, 0, sizeof(symkey));

    entry = mkw_pcalloc(pool, sizeof(*entry));
    entry->type = mkw_blob_type_user;
    entry->id = &user->id;
    entry->data = buf;
    mkw_blobstore_create_entry(store, entry);
}

static mkw_error_t
mkw_user_open(mkw_blobstore_t *store,
              mkw_user_t *user,
              const mkw_id_t *id,
              const uint8_t *passwd,
              size_t passwdsize,
              mkw_pool_t *pool)
{
    mkw_blobstore_entry_t *entry = mkw_blobstore_get_entry(store, id);
    mkw_memreader_t *reader;

    if (entry == NULL || entry->type != mkw_blob_type_user) {
        return MKW_ERROR_USER_NOT_EXIST;
    }

    reader = mkw_memreader_create(entry->data->data, entry->data->size, pool);

    while (reader->remaining) {
        mkw_memreader_t bodyreader = { 0 };
        enum mkw_pgp_packet_tag_e tag;

        MKW_ERR(mkw_pgp_packet_deserialize(reader, &tag, &bodyreader));

        if (tag == mkw_pgp_packet_seckey) {
            mkw_pubkey_t pubkey = { 0 };
            mkw_seckey_rsa_t seckey = { 0 };

            MKW_ERR(mkw_pgp_pubkey_deserialize(&bodyreader, &pubkey));
            user->key.pubkey = pubkey.material;
            MKW_ERR(mkw_pgp_seckeydata_decrypt(&bodyreader, &user->s2k,
                                               passwd, passwdsize, &seckey,
                                               pool));
        } else {
            return MKW_ERROR_BAD_PACKET_TAG;
        }
    }

    return MKW_ERROR_NONE;
}

/* https://www.rfc-editor.org/rfc/rfc9580.html#appendix-A.9.1 */
static mkw_error_t
test_s2k() {
    mkw_s2k_t s2k = {
        .tag = mkw_s2k_tag_salted_iterated,
        .hash = mkw_hash_tag_sha256,
        .count = 0xff,
        .salt = { 0x56, 0xa2, 0x98, 0xd2, 0xf5, 0xe3, 0x64, 0x53 },
    };

    uint8_t expected[16] = {
        0xe8, 0x0d, 0xe2, 0x43, 0xa3, 0x62, 0xd9, 0x3b, 0x9d, 0xc6, 0x07, 0xed,
        0xe9, 0x6a, 0x73, 0x56,
    };
    uint8_t actual[16] = { 0 };

    mkw_s2k_derive_key(&s2k,
                       (const uint8_t *)"password", 8,
                       actual, sizeof(actual));

    assert(memcmp(expected, actual, sizeof(expected)) == 0);
    return 0;
}

static mkw_error_t
test_user_round_trip(mkw_ctx_t *ctx, mkw_pool_t *pool)
{ 
    mkw_blobstore_t *store = mkw_blobstore_create_mem(pool);
    mkw_user_t user1, user2;

    MKW_ERR(mkw_user_keygen(ctx, &user1));
    mkw_user_store(store, &user1,
                   (const uint8_t *)"password", 8,
                   ctx, pool);
    MKW_ERR(mkw_user_open(store, &user2, &user1.id,
                          (const uint8_t *)"password", 8,
                          pool));

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_aes_round_trip_full_blocks(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    mkw_symkey_aes_t symkey;
    uint8_t plaintext1[16 * 3];
    mkw_membuf_t *ciphertext = mkw_membuf_create_empty(pool);
    mkw_membuf_t *plaintext2 = mkw_membuf_create_empty(pool);

    nettle_yarrow256_random(&ctx->rng, sizeof(symkey.key), symkey.key);
    nettle_yarrow256_random(&ctx->rng, sizeof(plaintext1), plaintext1);

    mkw_symkey_encrypt(&symkey, ciphertext, plaintext1, sizeof(plaintext1));
    mkw_symkey_decrypt(&symkey, plaintext2, ciphertext->data, ciphertext->size);

    assert(plaintext2->size == sizeof(plaintext1));
    assert(memcmp(plaintext2->data, plaintext1, plaintext2->size));

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_aes_round_trip_unaligned(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    mkw_symkey_aes_t symkey;
    uint8_t plaintext1[123];
    mkw_membuf_t *ciphertext = mkw_membuf_create_empty(pool);
    mkw_membuf_t *plaintext2 = mkw_membuf_create_empty(pool);

    nettle_yarrow256_random(&ctx->rng, sizeof(symkey.key), symkey.key);
    nettle_yarrow256_random(&ctx->rng, sizeof(plaintext1), plaintext1);

    mkw_symkey_encrypt(&symkey, ciphertext, plaintext1, sizeof(plaintext1));
    mkw_symkey_decrypt(&symkey, plaintext2, ciphertext->data, ciphertext->size);

    assert(plaintext2->size >= sizeof(plaintext1));
    assert(memcmp(plaintext2->data, plaintext1, sizeof(plaintext1)));

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_seckeydata_round_trip(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    mkw_membuf_t *ciphertext;
    mkw_user_t user;
    mkw_s2k_t s2k;
    mkw_seckey_rsa_t *seckey1;
    mkw_seckey_rsa_t seckey2 = { 0 };
    mkw_seckey_rsa_t seckey3 = { 0 };
    mkw_error_t err;
    mkw_memreader_t *reader;

    mkw_user_keygen(ctx, &user);
    mkw_s2k_init(ctx, &s2k);
    seckey1 = &user.key.seckey;

    // using plaintext encode/decode
    ciphertext = mkw_membuf_create_empty(pool);
    mkw_pgp_seckeydata_encode(ciphertext, seckey1);
    reader = mkw_memreader_create(ciphertext->data, ciphertext->size, pool); 
    err = mkw_pgp_seckeydata_decode(reader, &seckey2);
    assert(reader->remaining == 0);
    assert(err == 0);
    assert(0 == mpz_cmp(seckey1->p, seckey2.p));
    assert(0 == mpz_cmp(seckey1->q, seckey2.q));

    /* */
    ciphertext = mkw_membuf_create_empty(pool);
    mkw_pgp_seckeydata_encrypt(ciphertext, &s2k,
                               (const uint8_t *)"password", 8,
                               seckey1, pool);

    reader = mkw_memreader_create(ciphertext->data, ciphertext->size, pool);

    err = mkw_pgp_seckeydata_decrypt(reader, &s2k,
                                     (const uint8_t *)"password", 8,
                                     &seckey3, pool);

    assert(reader->remaining == 0);
    assert(err == 0);
    assert(0 == mpz_cmp(seckey1->p, seckey2.p));
    assert(0 == mpz_cmp(seckey1->q, seckey2.q));

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_mdc_round_trip()
{
    return MKW_ERROR_NONE;
}

static mkw_error_t
sub_main(mkw_pool_t *pool)
{
    mkw_blobstore_t *store = mkw_blobstore_create_mem(pool);
    mkw_user_t user;
    mkw_ctx_t ctx;

    MKW_ERR(mkw_ctx_create(&ctx, pool));

    MKW_ERR(test_s2k());
    MKW_ERR(test_aes_round_trip_full_blocks(&ctx, pool));
    MKW_ERR(test_aes_round_trip_unaligned(&ctx, pool));
    MKW_ERR(test_seckeydata_round_trip(&ctx, pool));
    MKW_ERR(test_user_round_trip(&ctx, pool));

    MKW_ERR(mkw_id_create(&ctx, &user.id));
    MKW_ERR(mkw_user_keygen(&ctx, &user));

    mkw_pubkey_t pubkey = {
        .time_created = 0,
        .expires_in_days = 0,
        .material = user.key.pubkey, 
    };

    mkw_membuf_t *text = mkw_membuf_create_empty(pool);
    mkw_membuf_t *buf = mkw_membuf_create_empty(pool);

    mkw_blobstore_entry_t entry = {
        .type = mkw_blob_type_entry,
        .id = &user.id,
        .data = buf, 
    };

    mkw_pgp_armour_write(text, &entry);

    fwrite(text->data, 1, text->size, stdout);

    return MKW_ERROR_NONE;
}

int main()
{
    mkw_pool_t *pool = mkw_pool_create();

    mkw_error_t err = sub_main(pool);

    mkw_pool_nuke(pool);

    if (err == MKW_ERROR_NONE) {
        return 0;
    } else {
        fprintf(stderr, "Error: %d\n", err);
    }
}
