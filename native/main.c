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
               const char *passwd,
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
    mkw_pgp_seckeydata_encrypt(subbuf, &user->s2k, passwd,
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
              const char *passwd,
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
                                               passwd, &seckey, pool));
        } else {
            return MKW_ERROR_BAD_PACKET_TAG;
        }
    }

    return MKW_ERROR_NONE;
}

static uint8_t *
hex(const char *str, mkw_pool_t *pool);

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

    mkw_s2k_derive_key(&s2k, "password", actual, sizeof(actual));

    assert(memcmp(expected, actual, sizeof(expected)) == 0);
    return 0;
}

static mkw_error_t
test_user_round_trip(mkw_ctx_t *ctx, mkw_pool_t *pool)
{ 
    mkw_blobstore_t *store = mkw_blobstore_create_mem(pool);
    mkw_membuf_t *buf = mkw_membuf_create_empty(pool);
    mkw_user_t user1, user2;

    MKW_ERR(mkw_user_keygen(ctx, &user1));
    mkw_user_store(store, &user1, "password", ctx, pool);
    MKW_ERR(mkw_user_open(store, &user2, &user1.id, "password", pool));

#if 1
    mkw_blobstore_write(store, buf);
    fwrite(buf->data, buf->size, 1, stdout);
#endif

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_aescfb_testvectors(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    /*
     * COUNT = 0
     * KEY = 3d2013d183970f00d3551281f2543fbd
     * IV = 953eb9921a7ae4b9ec6d115eb720f7f0
     * PLAINTEXT = 2590ad2e5455a6a5fe61a09ea4033c81
     * CIPHERTEXT = 72beed95ea2239d3d087cba751e3769e
     */

    uint8_t *key = hex("085b8af6788fa6bc1a0b47dcf50fbd35", pool);
    uint8_t *iv = hex("58cb2b12bb52c6f14b56da9210524864", pool);
    uint8_t *plaintext = hex("4b5a872260293312eea1a570fd39c788", pool);
    uint8_t *ciphertext = hex("e92c80e0cfb6d8b1c27fd58bc3708b16", pool);

    {
        mkw_cfb_ctx_t cfb;
        mkw_membuf_t *out = mkw_membuf_create_empty(pool);
        mkw_cfb_ctx_init(&cfb, key, iv);
        mkw_cfb_ctx_encrypt_full(&cfb, out, plaintext, 16);
        assert(out->size == 16);
        assert(memcmp(ciphertext, out->data, 16) == 0);
    }

    {
        mkw_cfb_ctx_t cfb;
        mkw_membuf_t *out = mkw_membuf_create_empty(pool);
        mkw_cfb_ctx_init(&cfb, key, iv);
        mkw_cfb_ctx_decrypt_full(&cfb, out, ciphertext, 16);
        assert(out->size == 16);
        assert(memcmp(plaintext, out->data, 16) == 0);
    }

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_aescfb_testvectors_multiblock(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    /*
     * KEY = 0a8e8876c96cddf3223069002002c99f
     * IV = b125a20ecd79e8b5ae91af738037acf7
     * PLAINTEXT = 4fd0ecac65bfd321c88ebca0daea35d2b061205d696aab08bea68320db65451a6d6c3679fdf633f37cf8ebcf1fa94b91
     * CIPHERTEXT = cdd1ba252b2c009f34551a6a200602d71ffbf13e684a5e60478cdf74ffe61dfded344bdc7e8000c3b0b67552917f3e4c
     */

    size_t size = 96 / 2;
    uint8_t *key = hex("0a8e8876c96cddf3223069002002c99f", pool);
    uint8_t *iv = hex("b125a20ecd79e8b5ae91af738037acf7", pool);
    uint8_t *plaintext = hex("4fd0ecac65bfd321c88ebca0daea35d2b061205d696aab08bea68320db65451a6d6c3679fdf633f37cf8ebcf1fa94b91", pool);
    uint8_t *ciphertext = hex("cdd1ba252b2c009f34551a6a200602d71ffbf13e684a5e60478cdf74ffe61dfded344bdc7e8000c3b0b67552917f3e4c", pool);

    {
        mkw_cfb_ctx_t cfb;
        mkw_membuf_t *out = mkw_membuf_create_empty(pool);
        mkw_cfb_ctx_init(&cfb, key, iv);
        mkw_cfb_ctx_encrypt_full(&cfb, out, plaintext, size);
        assert(out->size == size);
        assert(memcmp(ciphertext, out->data, size) == 0);
    }

    {
        mkw_cfb_ctx_t cfb;
        mkw_membuf_t *out = mkw_membuf_create_empty(pool);
        mkw_cfb_ctx_init(&cfb, key, iv);
        mkw_cfb_ctx_decrypt_full(&cfb, out, ciphertext, size);
        assert(out->size == size);
        assert(memcmp(plaintext, out->data, size) == 0);
    }

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_aes_round_trip_full_blocks(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    mkw_symkey_aes_t symkey;
    uint8_t data[16 * 3];
    mkw_membuf_t *ciphertext = mkw_membuf_create_empty(pool);
    mkw_membuf_t *plaintext = mkw_membuf_create_empty(pool);

    nettle_yarrow256_random(&ctx->rng, sizeof(symkey.key), symkey.key);
    nettle_yarrow256_random(&ctx->rng, sizeof(data), data);

    mkw_symkey_encrypt(&symkey, ciphertext, data, sizeof(data));
    mkw_symkey_decrypt(&symkey, plaintext, ciphertext->data, ciphertext->size);

    assert(plaintext->size == sizeof(data));
    assert(memcmp(plaintext->data, data, plaintext->size) == 0);

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_aes_round_trip_unaligned(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    mkw_symkey_aes_t symkey;
    uint8_t data[123];
    mkw_membuf_t *ciphertext = mkw_membuf_create_empty(pool);
    mkw_membuf_t *plaintext = mkw_membuf_create_empty(pool);

    nettle_yarrow256_random(&ctx->rng, sizeof(symkey.key), symkey.key);
    nettle_yarrow256_random(&ctx->rng, sizeof(data), data);

    mkw_symkey_encrypt(&symkey, ciphertext, data, sizeof(data));
    mkw_symkey_decrypt(&symkey, plaintext, ciphertext->data, ciphertext->size);

    assert(plaintext->size == sizeof(data));
    assert(memcmp(plaintext->data, data, sizeof(data)) == 0);

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
    mkw_pgp_seckeydata_encrypt(ciphertext, &s2k, "password",
                               seckey1, pool);

    reader = mkw_memreader_create(ciphertext->data, ciphertext->size, pool);

    err = mkw_pgp_seckeydata_decrypt(reader, &s2k, "password",
                                     &seckey3, pool);

    assert(reader->remaining == 0);
    assert(err == 0);
    assert(0 == mpz_cmp(seckey1->p, seckey2.p));
    assert(0 == mpz_cmp(seckey1->q, seckey2.q));

    return MKW_ERROR_NONE;
}

static char *hex_enc = "0123456789abcdef";

static int
decrypt_one_hex(char ch)
{
    switch (ch) {
        case '0': return 0;
        case '1': return 1;
        case '2': return 2;
        case '3': return 3;
        case '4': return 4;
        case '5': return 5;
        case '6': return 6;
        case '7': return 7;
        case '8': return 8;
        case '9': return 9;
        case 'a': return 10;
        case 'b': return 11;
        case 'c': return 12;
        case 'd': return 13;
        case 'e': return 14;
        case 'f': return 15;
        default: abort();
    };
}

static uint8_t *
hex(const char *str, mkw_pool_t *pool)
{
    assert(strlen(str) % 2 == 0);
    uint8_t *result = mkw_pcalloc(pool, strlen(str) / 2);
    for (uint8_t *p = result; *str; str += 2, p++) {
        *p = decrypt_one_hex(str[0]) * 16 + decrypt_one_hex(str[1]);
    }
    return result;
}

static mkw_error_t
test_mdc_round_trip()
{
    return MKW_ERROR_NONE;
}

static mkw_error_t
test_payload(mkw_ctx_t *ctx, mkw_pool_t *pool)
{
    mkw_membuf_t *buf = mkw_membuf_create_empty(pool);
    mkw_vector_t *vec = mkw_vector_create_empty(pool);

    mkw_payload_entry_t username = {
        .key = "mkw:username",
        .content = mkw_membuf_create_from_cstr(pool, "tima"),
    };
    mkw_payload_entry_t passwd = {
        .key = "mkw:password",
        .content = mkw_membuf_create_from_cstr(pool, "secret123"),
    };
    mkw_payload_entry_t *items[] = { &username, &passwd };

    mkw_payload_write(buf, items, 2);
    MKW_ERR(mkw_payload_read(mkw_memreader_create(buf->data, buf->size, pool),
                             vec, pool));

#if 0
    fwrite(buf->data, buf->size, 1, stdout);
#endif

    return MKW_ERROR_NONE;
}

static mkw_error_t
test_bigint(mkw_pool_t *pool)
{
    int i;

    {
        mkw_bigint_t *a = mkw_bigint_from_num(UINT32_MAX, pool);
        mkw_bigint_add_n(a, 42);
        mkw_bigint_sub_n(a, 42);
        assert(0 == mkw_bigint_cmp(a, mkw_bigint_from_num(UINT32_MAX, pool)));
    }

    {
        mkw_bigint_t *a = mkw_bigint_from_num(UINT32_MAX, pool);
        mkw_limb_t data[] = { 1, UINT32_MAX - 1 };
        mkw_bigint_mul_n(a, 2);
        assert(0 == mkw_bigint_cmp(a, mkw_bigint_from_limbs(data, 2, pool)));
    }

    {
        mkw_limb_t data[] = { UINT32_MAX, UINT32_MAX };
        mkw_limb_t e[] = { 1, UINT32_MAX, UINT32_MAX - 1 };
        mkw_bigint_t *x = mkw_bigint_from_limbs(data, 2, pool);
        mkw_bigint_t *n = mkw_bigint_from_limbs(data, 2, pool);
        mkw_bigint_add(x, n);
        assert(0 == mkw_bigint_cmp(x, mkw_bigint_from_limbs(e, 3, pool)));
    }

    {
        mkw_bigint_t *x = mkw_bigint_create(0, pool);
        mkw_bigint_t *r = mkw_bigint_create(0, pool);
        mkw_bigint_t *tmp = mkw_bigint_create(0, pool);
        mkw_limb_t data[] = { UINT32_MAX, UINT32_MAX };
        mkw_limb_t e[] = { UINT32_MAX, UINT32_MAX - 1, 0, 1 };
        mkw_bigint_mul(x,
                       mkw_bigint_from_limbs(data, 2, pool),
                       mkw_bigint_from_limbs(data, 2, pool),
                       tmp);
        MKW_BIGINT_TRACE(x);
        MKW_BIGINT_TRACE(mkw_bigint_from_limbs(e, 4, pool));
        assert(0 == mkw_bigint_cmp(x, mkw_bigint_from_limbs(e, 4, pool)));

        mkw_bigint_div(x, r,
                       mkw_bigint_dup(x, pool),
                       mkw_bigint_from_limbs(data, 2, pool));
        MKW_BIGINT_TRACE(x);
        assert(0 == mkw_bigint_cmp(x, mkw_bigint_from_limbs(data, 2, pool)));
        // assert(0 == mkw_bigint_bitsize(r));
     }
 
    {
        mkw_bigint_t *tmp1 = mkw_bigint_create_empty(pool);
        mkw_bigint_t *tmp2 = mkw_bigint_create_empty(pool);
        mkw_bigint_t *tmp3 = mkw_bigint_create_empty(pool);
        mkw_bigint_t *a = mkw_bigint_create(2, pool);
        mkw_bigint_t *b = mkw_bigint_create(2, pool);
        mkw_bigint_t *x = mkw_bigint_create_empty(pool);
        mkw_bigint_t *r = mkw_bigint_create_empty(pool);

        for (i = 64969871; i < 100000000; i++) {
            if (i % 10000 == 0 || 1) {
                fprintf(stdout, "fuzzing: %d\n", i);
            }

            srand(i);

            mkw_limb_t a_data[] = { rand(), rand() };
            mkw_limb_t b_data[] = { rand(), rand() };

            a->digits[1] = rand();
            a->digits[0] = rand();
            b->digits[1] = rand();
            b->digits[0] = rand();
            mkw_bigint_zero(x);
            mkw_bigint_zero(r);

            mkw_bigint_mul(x, a, b, tmp1);

            MKW_BIGINT_TRACE(a);
            MKW_BIGINT_TRACE(b);
            MKW_BIGINT_TRACE(x);

            mkw_bigint_div(x, r, mkw_bigint_set(tmp3, x), a);
            MKW_BIGINT_TRACE(x);
            MKW_BIGINT_TRACE(r);
            assert(0 == mkw_bigint_cmp(x, b));
            // assert(0 == mkw_bigint_bitsize(r));

            mkw_bigint_mul(x, a, b, tmp1);
            mkw_bigint_div(x, r, mkw_bigint_set(tmp3, x), b);
            MKW_BIGINT_TRACE(x);
            MKW_BIGINT_TRACE(r);
            assert(0 == mkw_bigint_cmp(x, a));
            // assert(0 == mkw_bigint_bitsize(r));
        }
    }

    return MKW_ERROR_NONE;
}

static mkw_error_t
sub_main(mkw_pool_t *pool)
{
    mkw_blobstore_t *store = mkw_blobstore_create_mem(pool);
    mkw_user_t user;
    mkw_ctx_t ctx;

    MKW_ERR(mkw_ctx_create(&ctx, pool));

    MKW_ERR(test_bigint(pool));
    MKW_ERR(test_s2k());
    MKW_ERR(test_aescfb_testvectors(&ctx, pool));
    MKW_ERR(test_aescfb_testvectors_multiblock(&ctx, pool));
    MKW_ERR(test_aes_round_trip_full_blocks(&ctx, pool));
    MKW_ERR(test_aes_round_trip_unaligned(&ctx, pool));
    MKW_ERR(test_seckeydata_round_trip(&ctx, pool));
    MKW_ERR(test_user_round_trip(&ctx, pool));
    MKW_ERR(test_payload(&ctx, pool));

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
