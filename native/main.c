/*
 * Naming conventions:
 *   session - session key
 *   pubkey  - public key
 *   seckey  - private key (secret key)
 *   symkey  - symmetric key
 */

/* includes */
#include <stdio.h>
#include <stdlib.h>
#include <stddef.h>
#include <assert.h>
#include <string.h>

#include <nettle/yarrow.h>
#include <nettle/rsa.h>
#include <nettle/cfb.h>
#include <nettle/aes.h>
#include <nettle/sha1.h>
#include <nettle/base16.h>
#include <nettle/base64.h>

#define DEBUG

#define max(a, b) (((a) > (b)) ? (a) : (b))
#define min(a, b) (((a) < (b)) ? (a) : (b))

#define ALIGN_SIZE sizeof(void *)
#define ALIGN_UP(n) ((n + ALIGN_SIZE - 1) / ALIGN_SIZE * ALIGN_SIZE)

static void
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

/* memory primitives */
static void *
mkw_alloc(size_t size)
{
    void *ptr = malloc(size);
    if (! ptr) {
        fputs("out of memory\n", stderr);
        abort();
    }
#ifdef DEBUG
    fprintf(stderr, "malloc(%ld)\n", size);
#endif
    return ptr;
}

#define mkw_calloc(size) memset(mkw_alloc(size), 0, size)

static void
mkw_free(void *ptr)
{
    free(ptr);
#ifdef DEBUG
    fprintf(stderr, "free()\n");
#endif
}

/* error handling */
#define MKW_ERROR_NONE                          0
#define MKW_ERROR_EOF                           1 
#define MKW_ERROR_MAFORMED_PACKET               2 
#define MKW_ERROR_BAD_PACKET_TAG                3 
#define MKW_ERROR_BAD_PUBKEY_TAG                4 
#define MKW_ERROR_BAD_HASH_TAG                  5 
#define MKW_ERROR_BAD_S2K_TAG                   6 
#define MKW_ERROR_BAD_VERSION                   7 
#define MKW_ERROR_BAD_TYPE                      8 
#define MKW_ERROR_RSA_KEYGEN                    9
#define MKW_ERROR_IO                            10
#define MKW_ERROR_BAD_CHAR                      11
#define MKW_ERROR_MDP_BAD_CHECKSUM              13
#define MKW_ERROR_MDP_BAD_QUICK_CHECK           14
#define MKW_ERROR_MDP_MALFORMED                 15
#define MKW_ERROR_MDP_BAD_TAG                   16
#define MKW_ERROR_USER_NOT_EXIST                17
#define MKW_ERROR_ENTRY_NOT_EXIST               18
#define MKW_ERROR_PARTIAL_LENGTH_NOT_SUPPORTED  19

typedef int mkw_error_t;

#define MKW_ERR(expr) do { \
    mkw_error_t __err = (expr); \
    if (__err != MKW_ERROR_NONE) \
        return __err; \
} while(0);

/* growable vector data structure */
typedef struct mkw_vector_t
{
    void **data;
    size_t size;
    size_t capacity;
} mkw_vector_t;

#define MKW_VECTOR_ELEMENT_SIZE sizeof(void *)

static mkw_vector_t *
mkw_vector_create_empty()
{ 
    mkw_vector_t *vec = mkw_calloc(sizeof(*vec));
    vec->data = NULL;
    vec->size = 0;
    vec->capacity = 0;
    return vec;
}

static void
mkw_vector_resize(mkw_vector_t *vec, size_t new_capacity)
{
    void **new_data = mkw_calloc(new_capacity * MKW_VECTOR_ELEMENT_SIZE); 

    if (vec->data) {
        memcpy(new_data, vec->data, vec->size * MKW_VECTOR_ELEMENT_SIZE);
        mkw_free(vec->data);
    }

    vec->data = new_data;
}

static void
mkw_vector_ensure(mkw_vector_t *vec, size_t size)
{
    if (size > vec->capacity) {
        // Chose the largest of 8, twice as current capacity, or requested
        // capacity.
        mkw_vector_resize(vec, max(max(size, vec->capacity * 2), 8));
    }
}

static void
mkw_vector_push(mkw_vector_t *vec, void *elem)
{
    mkw_vector_ensure(vec, vec->capacity + 1);
    vec->data[vec->size] = elem;
    vec->size++;
}

/* growable buffer */
typedef struct mkw_membuf_t
{
    uint8_t *data;
    size_t size;
    size_t capacity;
} mkw_membuf_t;

static mkw_membuf_t *
mkw_membuf_create_empty()
{ 
    mkw_membuf_t *buf = mkw_calloc(sizeof(*buf));
    buf->size = 0;
    buf->capacity = 64;
    buf->data = mkw_calloc(buf->capacity);
    return buf;
}

static void
mkw_membuf_resize(mkw_membuf_t *buf, size_t new_capacity)
{
    uint8_t *new_data = mkw_calloc(new_capacity); 

#ifdef DEBUG
    fprintf(stderr, "membuf.resize(%ld -> %ld)\n",
            buf->capacity, new_capacity);
#endif

    memcpy(new_data, buf->data, buf->size);
    mkw_free(buf->data);

    buf->data = new_data;
    buf->capacity = new_capacity;
}

static void
mkw_membuf_ensure(mkw_membuf_t *buf, size_t size)
{
    if (size > buf->capacity) {
        mkw_membuf_resize(buf, max(size, buf->capacity * 2));
    }
}

static void
mkw_membuf_write_str(mkw_membuf_t *buf, const uint8_t *str, size_t len)
{
    mkw_membuf_ensure(buf, buf->size + len);
    memcpy(buf->data + buf->size, str, len);
    buf->size += len;
}

static void
mkw_membuf_write_cstr(mkw_membuf_t *buf, const char *str)
{
    mkw_membuf_write_str(buf, (uint8_t *)str, strlen(str));
}

static void
mkw_membuf_write_uint8(mkw_membuf_t *buf, uint8_t num)
{
    mkw_membuf_ensure(buf, buf->size + 1);
    buf->data[buf->size++] = num;
}

static void
mkw_membuf_write_uint16(mkw_membuf_t *buf, uint16_t num)
{
    mkw_membuf_ensure(buf, buf->size + 2);
    buf->data[buf->size++] = (uint8_t)((num & 0xff00) >> 8);
    buf->data[buf->size++] = (uint8_t)((num & 0x00ff) >> 0);
}

static void
mkw_membuf_write_uint32(mkw_membuf_t *buf, uint32_t num)
{
    mkw_membuf_ensure(buf, buf->size + 4);
    buf->data[buf->size++] = (uint8_t)((num & 0xff000000) >> 24);
    buf->data[buf->size++] = (uint8_t)((num & 0x00ff0000) >> 16);
    buf->data[buf->size++] = (uint8_t)((num & 0x0000ff00) >> 8);
    buf->data[buf->size++] = (uint8_t)((num & 0x000000ff) >> 0);
}

static void
mkw_membuf_write_mpi(mkw_membuf_t *buf, const mpz_t num)
{
    size_t bits = mpz_sizeinbase(num, 2);
    size_t bytes = (bits + 7) / 8;
    assert(bytes <= UINT16_MAX);
    mkw_membuf_write_uint16(buf, bits);
    mkw_membuf_ensure(buf, buf->size + bytes);
    mpz_export(buf->data + buf->size,
               NULL,     /* countp */ 
               bytes,    /* size */
               1,        /* order */
               1,        /* endian */
               0,        /* nails */
               num);
    buf->size += bytes;
}

static void
mkw_membuf_write_mpi_str(mkw_membuf_t *buf, mkw_membuf_t *mpi)
{
    /* TODO: properly calculate amout of bits */
    mkw_membuf_write_uint16(buf, mpi->size * 8);
    mkw_membuf_write_str(buf, mpi->data, mpi->size);
}

static void
mkw_membuf_write_base16(mkw_membuf_t *buf, uint8_t *data, size_t len) 
{
    mkw_membuf_ensure(buf, buf->size + BASE16_ENCODE_LENGTH(len));
    nettle_base16_encode_update((char *)(buf->data + buf->size),
                                len, data);
    buf->size += BASE16_ENCODE_LENGTH(len);
}

static uint8_t *
mkw_membuf_write_buf(mkw_membuf_t *buf, size_t len)
{
    mkw_membuf_ensure(buf, buf->size + len);
    uint8_t *result = &buf->data[buf->size + len];
    buf->size += len;
    return result;
}

/* memory reader */
typedef struct mkw_memreader_t
{
    const uint8_t *data;
    size_t size;
    size_t offset;
} mkw_memreader_t;

static mkw_memreader_t *
mkw_memreader_create(const uint8_t *data, size_t size)
{
    mkw_memreader_t *result = mkw_calloc(size);
    result->data = data;
    result->size = size;
    result->offset = 0;
    return result;
}

static mkw_error_t
mkw_memreader_read_uint8(mkw_memreader_t *reader, uint8_t *result)
{
    if (reader->offset < reader->size) {
        *result = reader->data[reader->offset++]; 
        return MKW_ERROR_NONE;
    } else {
        return MKW_ERROR_EOF;
    }
}

static mkw_error_t 
mkw_memreader_read_uint16(mkw_memreader_t *reader, uint16_t *result)
{
    uint8_t b0, b1;
    MKW_ERR(mkw_memreader_read_uint8(reader, &b0));
    MKW_ERR(mkw_memreader_read_uint8(reader, &b1));
    *result = (b0 << 8) | (b1 << 0); 
    return MKW_ERROR_NONE;
}

static mkw_error_t 
mkw_memreader_read_uint32(mkw_memreader_t *reader, uint32_t *result)
{
    uint8_t b0, b1, b2, b3;

    MKW_ERR(mkw_memreader_read_uint8(reader, &b0));
    MKW_ERR(mkw_memreader_read_uint8(reader, &b1));
    MKW_ERR(mkw_memreader_read_uint8(reader, &b2));
    MKW_ERR(mkw_memreader_read_uint8(reader, &b3));

    *result = (b0 << 24) | (b1 << 16) | (b1 << 8) | (b1 << 0); 
    return MKW_ERROR_NONE;
}

static mkw_error_t
mkw_memreader_read_mpi(mkw_memreader_t *reader, mpz_t mpi)
{
    uint16_t bits;
    size_t bytes;

    MKW_ERR(mkw_memreader_read_uint16(reader, &bits));
    bytes = (bits + 7) / 8;

    if (reader->offset + bytes <= reader->size) {
        mpz_import(mpi,
                   bytes,   /* count    */
                   1,       /* order    */
                   1,       /* size     */
                   1,       /* endian   */
                   0,       /* nails    */
                   reader->data + reader->offset);
        reader->offset += bytes;
        return MKW_ERROR_NONE;
    } else {
        return MKW_ERROR_EOF;
    }
}

static mkw_error_t
mkw_memreader_readline(mkw_memreader_t *reader,
                       mkw_membuf_t *buf,
                       const char *eol) 
{
    uint8_t ch;

    while (1) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &ch));
        if (strchr(eol, ch) == NULL) {
            mkw_membuf_write_uint8(buf, ch);
        } else {
            return MKW_ERROR_NONE;
        }
    }
}

static mkw_error_t
mkw_memreader_eat_cstr(mkw_memreader_t *reader,
                       const char *str)
{
    uint8_t ch;
    for (; str; str++) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &ch));
        if (ch != *str) {
            return MKW_ERROR_BAD_CHAR;
        }
    }
    return MKW_ERROR_NONE;
}

static mkw_error_t
mkw_memreader_read_buf(mkw_memreader_t *reader,
                       uint8_t *buf, size_t size)
{
    size_t i;
    uint8_t ch;

    for (i = 0; i < size; i++) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &ch));
        buf[i] = ch;
    }

    return MKW_ERROR_NONE;
}

static mkw_error_t
mkw_memreader_subreader(mkw_memreader_t *reader,
                        mkw_memreader_t *subreader,
                        size_t len)
{
    if (len <= subreader->size - subreader->offset) {
        subreader->offset = 0;
        subreader->size = len;
        subreader->data = reader->data + reader->offset;
        reader->offset += len;
        return MKW_ERROR_NONE;
    } else {
        reader->offset = reader->size;
        return MKW_ERROR_EOF;
    }
}

/* library context */
typedef struct mkw_ctx_t {
    struct yarrow256_ctx rng;
} mkw_ctx_t;

#define RNG_SEED_SIZE 256
#define RNG_DEVICE "/dev/urandom"

static mkw_error_t
mkw_ctx_create(mkw_ctx_t *ctx)
{
    FILE *fdevice = NULL;
    uint8_t *buf = mkw_calloc(RNG_SEED_SIZE);
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
    
    mkw_base16_dump(stderr, buf, RNG_SEED_SIZE);
    yarrow256_seed(&ctx->rng, RNG_SEED_SIZE, buf);

cleanup:
    mkw_free(buf);
    if (fdevice) {
        fclose(fdevice);
    }
    return err;
}

/* id */
#define MKW_ID_SIZE 8

typedef struct mkw_id_t
{
    uint8_t data[MKW_ID_SIZE];
} mkw_id_t;

static mkw_error_t
mkw_id_create(mkw_ctx_t *ctx, mkw_id_t *id) {
    nettle_yarrow256_random(&ctx->rng, sizeof(id->data), id->data);
    return MKW_ERROR_NONE;
}

static int
mkw_id_compare(mkw_id_t *left, mkw_id_t *right) {
    return memcmp(left->data, right->data, MKW_ID_SIZE);
}

/* blobstore */
typedef struct mkw_blobstore_t {
    mkw_vector_t *entries;
} mkw_blobstore_t;

enum mkw_blob_type_e {
    mkw_blob_type_entry,
    mkw_blob_type_user,
} mkw_blob_type;

typedef struct mkw_blobstore_entry_t {
    enum mkw_blob_type_e type;
    mkw_id_t *id;
    mkw_membuf_t *data;
} mkw_blobstore_entry_t;

/* PGP armour */
#define MKW_TYPE_STR_ENTRY "MKW ENTRY" 
#define MKW_TYPE_STR_USER "MKW USER" 

static const char *
mkw_blob_type_encode(enum mkw_blob_type_e type) {
    if (type == mkw_blob_type_entry) {
        return MKW_TYPE_STR_ENTRY;
    } else if (type == mkw_blob_type_user) {
        return MKW_TYPE_STR_USER;
    } else {
        abort();
    }
}

static const mkw_error_t
mkw_blob_type_decode(const char *str, enum mkw_blob_type_e *type) {
    if (strcmp(str, MKW_TYPE_STR_ENTRY)) {
        *type = mkw_blob_type_entry;
        return MKW_ERROR_NONE;
    } else if (strcmp(str, MKW_TYPE_STR_USER)) {
        *type = mkw_blob_type_user;
        return MKW_ERROR_NONE;
    } else {
        return MKW_ERROR_BAD_TYPE;
    }
}

#define MKW_PGP_ARMOUR_LINE_SIZE    76
#define MKW_PGP_ARMOUR_BLOCK_SIZE   BASE64_DECODE_LENGTH(MKW_PGP_ARMOUR_LINE_SIZE)

static void
mkw_pgp_armour_encode_body(mkw_membuf_t *dst,
                           const uint8_t *data,
                           size_t size)
{
    struct base64_encode_ctx ectx = { 0 };
    char buf[BASE64_ENCODE_LENGTH(MKW_PGP_ARMOUR_BLOCK_SIZE)];
    size_t offset, final_count;

    nettle_base64_encode_init(&ectx);

    for (offset = 0;
         offset < size;
         offset += MKW_PGP_ARMOUR_BLOCK_SIZE)
    {
        int is_final = (offset + MKW_PGP_ARMOUR_BLOCK_SIZE) >= size;
        size_t this_block = is_final ? size - offset
                                     : MKW_PGP_ARMOUR_BLOCK_SIZE;

        nettle_base64_encode_update(&ectx, buf, this_block,
                                    data + offset);
        mkw_membuf_write_str(dst, (uint8_t *)buf, this_block);

        if (is_final) {
            final_count = nettle_base64_encode_final(&ectx, buf);
            mkw_membuf_write_str(dst, (uint8_t *)buf, final_count);
        }

        mkw_membuf_write_cstr(dst, "\n");
    }
}

static void
mkw_pgp_armour_write(mkw_membuf_t *dst,
                     mkw_blobstore_entry_t *entry)
{
    /* -----BEGIN MKW ENTRY----- */
    mkw_membuf_write_cstr(dst, "-----BEGIN ");
    mkw_membuf_write_cstr(dst, mkw_blob_type_encode(entry->type));
    mkw_membuf_write_cstr(dst, "-----\n");

    /* ID: dead0a55b16b00b5 */
    mkw_membuf_write_cstr(dst, "ID: ");
    mkw_membuf_write_base16(dst, entry->id->data, MKW_ID_SIZE);
    mkw_membuf_write_cstr(dst, "\n");

    /* separator */
    mkw_membuf_write_cstr(dst, "\n");

    /* body */
    mkw_pgp_armour_encode_body(dst, entry->data->data, entry->data->size);
    mkw_membuf_write_cstr(dst, "\n");

    /* -----END MKW ENTRY----- */
    mkw_membuf_write_cstr(dst, "-----END ");
    mkw_membuf_write_cstr(dst, mkw_blob_type_encode(entry->type));
    mkw_membuf_write_cstr(dst, "-----\n");
}

static mkw_blobstore_t *
mkw_blobstore_create_mem()
{
    mkw_blobstore_t *result = mkw_calloc(sizeof(*result));
    result->entries = mkw_vector_create_empty();
    return result;
}

static mkw_blobstore_entry_t *
mkw_blobstore_get_entry(mkw_blobstore_t *store,
                        mkw_id_t *id)
{
    for (size_t i = 0; i < store->entries->size; i++) {
        mkw_blobstore_entry_t *current = store->entries->data[i];

        if (mkw_id_compare(id, current->id) == 0) {
            return current;
        }
    }

    return NULL;
}

static void
mkw_blobstore_create_entry(mkw_blobstore_t *store,
                           mkw_blobstore_entry_t *entry)
{
    mkw_vector_push(store->entries, entry);
}

/* pgp packets */
typedef uint32_t mkw_pgp_time_t;

enum mkw_pgp_packet_tag_e {
    mkw_pgp_packet_session_pubkey   = 1,
    mkw_pgp_packet_seckey           = 5,
    mkw_pgp_packet_pubkey           = 6,
};

typedef struct rsa_public_key mkw_pubkey_rsa_t;
typedef struct rsa_private_key mkw_seckey_rsa_t;

typedef struct mkw_symkey_aes128_t {
    uint8_t key[128 / 8];
} mkw_symkey_aes_t;

enum mkw_pubkey_tag_e {
    mkw_pubkey_tag_rsa          = 1,
};

enum mkw_symkey_tag_e {
    mkw_symkey_tag_plaintext    = 0,
    mkw_symkey_tag_aes128       = 7,
    mkw_symkey_tag_aes192       = 8,
    mkw_symkey_tag_aes256       = 9,
};

typedef struct mkw_pubkey_t {
    mkw_pgp_time_t time_created;
    uint16_t expires_in_days;

    enum mkw_pubkey_tag_e tag;
    union {
        mkw_pubkey_rsa_t rsa;
    } material;
} mkw_pubkey_t;

typedef struct mkw_keypair_rsa_t {
    mkw_pubkey_rsa_t pubkey;
    mkw_seckey_rsa_t seckey;
} mkw_keypair_rsa_t;

typedef struct mkw_keypair_t {
    enum mkw_pubkey_tag_e tag;
    union {
        mkw_keypair_rsa_t rsa;
    } material;
} mkw_keypair_t; 

typedef struct mkw_enckey_t {
    /* who we encrypt for */
    mkw_id_t id;
    mpz_t m_to_e_mod_n;
} mkw_enckey_t;

enum mkw_hash_tag_e {
    mkw_hash_tag_md5 = 1,
    mkw_hash_tag_sha1 = 2,
    mkw_hash_tag_sha256 = 8,
    mkw_hash_tag_sha384 = 9,
    mkw_hash_tag_sha512 = 10,
    mkw_hash_tag_sha224 = 11,
};

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

typedef struct mkw_encseckey_t {
    mkw_membuf_t key;
    mkw_s2k_t s2k;
} mkw_encseckey_t;

/* TODO: */
typedef struct mkw_passwd_t mkw_passwd_t;

static void
mkw_pgp_session_pubkey_serialize(mkw_membuf_t *buf,
                                 mkw_enckey_t *key)
{
    mkw_membuf_write_uint8(buf, 3 /* version */);
    mkw_membuf_write_str(buf, key->id.data, MKW_ID_SIZE);

    mkw_membuf_write_uint8(buf, mkw_pubkey_tag_rsa);
    mkw_membuf_write_mpi(buf, key->m_to_e_mod_n);
}

static mkw_error_t
mkw_pgp_session_pubkey_deserialize(mkw_memreader_t *reader,
                                   mkw_enckey_t *key)
{
    uint8_t version, pubkey_tag;

    MKW_ERR(mkw_memreader_read_uint8(reader, &version));
    MKW_ERR(mkw_memreader_read_buf(reader, key->id.data, MKW_ID_SIZE));

    MKW_ERR(mkw_memreader_read_uint8(reader, &pubkey_tag));
    if (pubkey_tag == mkw_pubkey_tag_rsa) { 
        MKW_ERR(mkw_memreader_read_mpi(reader, key->m_to_e_mod_n));
    } else {
        return MKW_ERROR_BAD_PUBKEY_TAG;
    }

    return MKW_ERROR_NONE;
}

static void
mkw_pgp_pubkey_serialize(mkw_membuf_t *buf,
                         const mkw_pubkey_t *pubkey)
{
    mkw_membuf_write_uint8(buf, 4 /* version */);
    mkw_membuf_write_uint32(buf, pubkey->time_created);
    mkw_membuf_write_uint16(buf, pubkey->expires_in_days);
    mkw_membuf_write_uint8(buf, pubkey->tag);

    if (pubkey->tag == mkw_pubkey_tag_rsa) {
        const mkw_pubkey_rsa_t *rsa = &pubkey->material.rsa;
        mkw_membuf_write_mpi(buf, rsa->n);
        mkw_membuf_write_mpi(buf, rsa->e);
    } else {
        abort();
    }
}

static mkw_error_t
mkw_pgp_pubkey_deserialize(mkw_memreader_t *reader,
                           mkw_pubkey_t *pubkey_p)
{
    uint8_t version;
    uint8_t tag;

    mkw_memreader_read_uint8(reader, &version);
    if (version != 4) {
        return MKW_ERROR_BAD_VERSION;
    }

    mkw_memreader_read_uint32(reader, &pubkey_p->time_created);
    mkw_memreader_read_uint16(reader, &pubkey_p->expires_in_days);
    mkw_memreader_read_uint8(reader, &tag);

    pubkey_p->tag = tag;
    if (tag == mkw_pubkey_tag_rsa) {
        mkw_pubkey_rsa_t *rsa = &pubkey_p->material.rsa;
        mkw_memreader_read_mpi(reader, rsa->n);
        mkw_memreader_read_mpi(reader, rsa->e);
    } else {
        return MKW_ERROR_BAD_PUBKEY_TAG;
    }

    return MKW_ERROR_NONE;
}

#define EXPBIAS 6

static void
mkw_pgp_s2k_serialize(mkw_membuf_t *buf,
                      const mkw_s2k_t *s2k)
{
    mkw_membuf_write_uint8(buf, s2k->tag);

    if (s2k->tag == mkw_s2k_tag_plaintext) {
        /* no-op */
    } else if (s2k->tag == mkw_s2k_tag_simple) {
        mkw_membuf_write_uint8(buf, s2k->hash);
    } else if (s2k->tag == mkw_s2k_tag_salted) {
        mkw_membuf_write_uint8(buf, s2k->hash);
        mkw_membuf_write_str(buf, s2k->salt, sizeof(s2k->salt));
    } else if (s2k->tag == mkw_s2k_tag_salted_iterated) {
        mkw_membuf_write_uint8(buf, s2k->hash);
        mkw_membuf_write_str(buf, s2k->salt, sizeof(s2k->salt));
        mkw_membuf_write_uint8(buf, s2k->count);
    } else {
        abort();
    }
}

static mkw_error_t
mkw_pgp_s2k_deserialize(mkw_memreader_t *reader,
                        mkw_s2k_t *s2k)
{
    uint8_t tag, hash;

    MKW_ERR(mkw_memreader_read_uint8(reader, &tag));
    s2k->tag = tag;

    if (s2k->tag == mkw_s2k_tag_plaintext) {
        /* no-op */
    } else if (s2k->tag == mkw_s2k_tag_simple) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &hash));
    } else if (s2k->tag == mkw_s2k_tag_salted) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &hash));
        MKW_ERR(mkw_memreader_read_buf(reader, s2k->salt, sizeof(s2k->salt)));
    } else if (s2k->tag == mkw_s2k_tag_salted_iterated) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &hash));
        MKW_ERR(mkw_memreader_read_buf(reader, s2k->salt, sizeof(s2k->salt)));
        MKW_ERR(mkw_memreader_read_uint8(reader, &s2k->count));
    } else {
        return MKW_ERROR_BAD_S2K_TAG;
    }

    s2k->hash = hash;
    return MKW_ERROR_NONE;
}

/* https://www.rfc-editor.org/rfc/rfc4880#section-5.5.3 */
#define MKW_S2K_USAGE_PLAINTEXT (uint8_t)0
#define MKW_S2K_USAGE_SOME_SHA1 (uint8_t)254
#define MKW_S2K_USAGE_SOME_HASH (uint8_t)255

static void
mkw_pgp_seckey_serialize(mkw_membuf_t *buf,
                         const mkw_encseckey_t *seckey)
{
    mkw_membuf_write_uint8(buf, MKW_S2K_USAGE_SOME_SHA1);
    mkw_pgp_s2k_serialize(buf, &seckey->s2k);
}

static void 
write_new_len(mkw_membuf_t *buf, uint64_t len)
{
    if (len < 192) {
        mkw_membuf_write_uint8(buf, (uint8_t)len);
    } else if (len <= ((223 - 192) << 8) + 0xFF + 192) {
        len -= 192;
        mkw_membuf_write_uint8(buf, (uint8_t)((len >> 8 & 0xff) + 192));
        mkw_membuf_write_uint8(buf, (uint8_t)(len & 0xff));
    } else {
        mkw_membuf_write_uint8(buf, 0xff);
        mkw_membuf_write_uint32(buf, len);
    }
}

static mkw_error_t 
read_new_len(mkw_memreader_t *reader, uint64_t *len)
{
    uint8_t b0, b1;
    MKW_ERR(mkw_memreader_read_uint8(reader, &b0));

    if (b0 < 192) {
        *len = b0;
        return MKW_ERROR_NONE;
    } else if (b0 < 224) {
        MKW_ERR(mkw_memreader_read_uint8(reader, &b1));
        *len = (((b0 - 192) << 8) | b1) + 192;
        return MKW_ERROR_NONE;
    } else if (b0 == 255) {
        uint32_t u32_len;
        MKW_ERR(mkw_memreader_read_uint32(reader, &u32_len));
        *len = u32_len;
        return MKW_ERROR_NONE;
    } else {
        return MKW_ERROR_PARTIAL_LENGTH_NOT_SUPPORTED;
    }
}

static void
mkw_pgp_packet_serialize(mkw_membuf_t *buf,
                         enum mkw_pgp_packet_tag_e tag,
                         uint8_t *body, size_t bodylen)
{
    assert(tag <= 0x3f);
    mkw_membuf_write_uint8(buf, 
                           (0x80) | /* always one*/
                           (0x40) | /* new format */
                           (0x3f & tag));

    write_new_len(buf, bodylen);
    mkw_membuf_write_str(buf, body, bodylen);
}

static mkw_error_t
mkw_pgp_packet_deserialize(mkw_memreader_t *reader,
                           enum mkw_pgp_packet_tag_e *tag,
                           mkw_memreader_t *body)
{
    uint8_t header;
    size_t len;

    MKW_ERR(mkw_memreader_read_uint8(reader, &header));
    int just_one    = 0x80 & header;
    int new_format  = 0x40 & header;
    *tag            = 0x3f & header;

    if (! (just_one && new_format)) {
        return MKW_ERROR_MAFORMED_PACKET;
    }

    MKW_ERR(read_new_len(reader, &len));
    MKW_ERR(mkw_memreader_subreader(reader, body, len));

    return MKW_ERROR_NONE;
}

typedef struct mkw_user_info_t {
    mkw_id_t id;
    mkw_pubkey_t pubkey;
} mkw_user_info_t;

typedef struct mkw_user_t {
    mkw_id_t id;
    mkw_keypair_t key;
} mkw_user_t;

static void
mkw_sha1(uint8_t *digest, const uint8_t *data, size_t size) {
    struct sha1_ctx checksum;

    nettle_sha1_init(&checksum);
    nettle_sha1_update(&checksum, size, data);
    nettle_sha1_digest(&checksum, SHA1_DIGEST_SIZE, digest);
}

#define MKW_MDP_SALT_SIZE 16
#define MKW_MDP_TAG (uint16_t)0xd314

static void
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
 
static mkw_error_t
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

    plaintext_size = reader->size - SHA1_DIGEST_SIZE - sizeof(MKW_MDP_TAG);
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

    assert((reader->offset) == (reader->size - 1));

    return MKW_ERROR_NONE;
}

#define ROUND_UP(num, block) ((num + block - 1) / block * block);

static void
mkw_symkey_encrypt(const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   size_t size)
{
    uint8_t iv[AES128_KEY_SIZE] = { 0 };
    struct aes128_ctx aesctx = { 0 };
    mkw_membuf_t *mdp = mkw_membuf_create_empty();
    size_t output_size = ROUND_UP(mdp->size, AES128_KEY_SIZE);

    nettle_aes128_set_encrypt_key(&aesctx, key->key);
    nettle_cfb_encrypt(&aesctx,
                       (nettle_cipher_func *)nettle_aes128_encrypt,
                       AES128_KEY_SIZE, iv, size,
                       mkw_membuf_write_buf(out, output_size),
                       data);
}

static void
mkw_symkey_decrypt(const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   const uint8_t size) 
{
    uint8_t iv[AES128_KEY_SIZE] = { 0 };
    struct aes128_ctx aesctx = { 0 };
    size_t output_size;

    nettle_aes128_set_encrypt_key(&aesctx, key->key);
    nettle_cfb_decrypt(
            &aesctx, (nettle_cipher_func *)nettle_aes128_decrypt,
            AES128_KEY_SIZE, iv, size,
            mkw_membuf_write_buf(out, output_size),
            data);
}

static void
mkw_symkey_protected_encrypt(mkw_ctx_t *ctx,
                             const mkw_symkey_aes_t *key,
                             mkw_membuf_t *out,
                             const uint8_t *data,
                             size_t size) 
{
    mkw_membuf_t *mdp = mkw_membuf_create_empty();

    mkw_mdp_write(ctx, mdp, data, size);
    mkw_symkey_encrypt(key, out, mdp->data, mdp->size);
}

static mkw_error_t
mkw_symkey_protected_decrypt(mkw_ctx_t *ctx,
                             const mkw_symkey_aes_t *key,
                             mkw_membuf_t *plaintext,
                             const uint8_t *data,
                             size_t size) 
{
    mkw_membuf_t *mdp = mkw_membuf_create_empty();
    mkw_memreader_t *reader;

    mkw_symkey_decrypt(key, mdp, data, size);
    reader = mkw_memreader_create(mdp->data, mdp->size);
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
 * 3) The symkey is derived using s2k (string to key). It's usage is described
 * as first octect of seckey packet. In our case, it's always 254. Then finally
 * goes s2k description.
 */

static void
mkw_pgp_seckeydata_encode(mkw_membuf_t *buf,
                          const mkw_seckey_rsa_t *seckey)
{
    mkw_membuf_write_mpi(buf, seckey->d);
    mkw_membuf_write_mpi(buf, seckey->p);
    mkw_membuf_write_mpi(buf, seckey->q);
    mkw_membuf_write_mpi(buf, seckey->c);
}

static mkw_error_t
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

static void
mkw_pgp_seckeydata_encrypt(mkw_membuf_t *buf,
                           const mkw_s2k_t *s2k,
                           const mkw_symkey_aes_t *symkey,
                           const mkw_seckey_rsa_t *seckey)
{
    uint8_t sha1[SHA1_DIGEST_SIZE];
    mkw_membuf_t *data = mkw_membuf_create_empty();

    /* prepare data to encrypt */
    mkw_pgp_seckeydata_encode(data, seckey);
    mkw_sha1(sha1, data->data, data->size);

    /* write everything to the output */
    mkw_membuf_write_uint8(buf, MKW_S2K_USAGE_SOME_SHA1);
    mkw_pgp_s2k_serialize(buf, s2k);
    mkw_symkey_encrypt(symkey, buf, data->data, data->size);
}

static mkw_error_t
mkw_pgp_seckeydata_decrypt(mkw_memreader_t *reader,
                           mkw_s2k_t *s2k,
                           const mkw_symkey_aes_t *symkey,
                           mkw_seckey_rsa_t *seckey)
{
    uint8_t sha1_computed[SHA1_DIGEST_SIZE], *sha1_packet;
    uint8_t s2k_usage;
    mkw_membuf_t *plaintext = mkw_membuf_create_empty();
    mkw_memreader_t *plaintext_reader, payload_reader;

    /* read encrypted things as they are */
    MKW_ERR(mkw_memreader_read_uint8(reader, &s2k_usage));
    MKW_ERR(mkw_pgp_s2k_deserialize(reader, s2k));
    mkw_symkey_decrypt(symkey, plaintext,
                       &reader->data[reader->offset],
                       reader->size - reader->offset);

    /* split packet onto two components; sha1 at the end and the rest is
     * the payload */
    reader = mkw_memreader_create(plaintext->data,
                                  plaintext->size - SHA1_DIGEST_SIZE);
    sha1_packet = &plaintext->data[plaintext->size - SHA1_DIGEST_SIZE];

    /* verify checksum before decoding plaintext payload body */
    mkw_sha1(sha1_computed, plaintext->data, plaintext->size);
    if (memcmp(sha1_computed, sha1_packet, SHA1_DIGEST_SIZE) != 0) {
        return MKW_ERROR_MDP_BAD_CHECKSUM;
    }

    /* let the decoder cool for the rest */
    MKW_ERR(mkw_pgp_seckeydata_decode(reader, seckey));

    return MKW_ERROR_NONE;
}

static mkw_error_t
mkw_user_keygen(mkw_ctx_t *ctx, mkw_user_t *user) {
    int status;

    user->key.tag = mkw_pubkey_tag_rsa;

    rsa_public_key_init(&user->key.material.rsa.pubkey);
    rsa_private_key_init(&user->key.material.rsa.seckey);

    status = nettle_rsa_generate_keypair(
            &user->key.material.rsa.pubkey,
            &user->key.material.rsa.seckey,
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
mkw_user_store(mkw_user_t *user, mkw_blobstore_t *store) {
    mkw_membuf_t *buf = mkw_membuf_create_empty();
    mkw_membuf_t *subbuf = mkw_membuf_create_empty(); 

    mkw_pubkey_t pubkey = {
        .time_created = 0,
        .expires_in_days = 0,
        .tag = user->key.tag,
        .material = user->key.material.rsa.pubkey,
    };
    mkw_s2k_t s2k = {
        .tag = mkw_s2k_tag_salted_iterated,
        .hash = mkw_hash_tag_sha256,
        .count = 100,
    };

    mkw_pgp_pubkey_serialize(subbuf, &pubkey);
    mkw_pgp_seckeydata_encrypt(subbuf, &s2k, NULL,
                               &user->key.material.rsa.seckey);
    mkw_pgp_packet_serialize(buf, mkw_pgp_packet_pubkey,
                             subbuf->data, subbuf->size);
}

static void
mkw_user_open(mkw_user_t *user,
              mkw_blobstore_t *store,
              mkw_id_t *id,
              mkw_passwd_t *passwd)
{
}

static mkw_error_t
sub_main()
{
    mkw_blobstore_t *store = mkw_blobstore_create_mem();
    mkw_user_t user;
    mkw_ctx_t ctx;

    MKW_ERR(mkw_ctx_create(&ctx));

    MKW_ERR(mkw_id_create(&ctx, &user.id));
    MKW_ERR(mkw_user_keygen(&ctx, &user));

    mkw_pubkey_t pubkey = {
        .time_created = 0,
        .expires_in_days = 0,
        .tag = mkw_pubkey_tag_rsa,
        .material.rsa = user.key.material.rsa.pubkey, 
    };

    mkw_membuf_t *text = mkw_membuf_create_empty();
    mkw_membuf_t *buf = mkw_membuf_create_empty();

    mkw_base16_dump(stdout, buf->data, buf->size);

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
    mkw_error_t err = sub_main();
    if (err == MKW_ERROR_NONE) {
        return 0;
    } else {
        fprintf(stderr, "Error: %d\n", err);
    }
}

