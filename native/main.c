/*
 * Naming conventions:
 *   seskey - session key
 *   pubkey - public key
 *   seckey - private key (secret key)
 *   symkey - symmetric key
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
#define MKW_ERROR_NONE                  0
#define MKW_ERROR_EOF                   1 
#define MKW_ERROR_MAFORMED_PACKET       2 
#define MKW_ERROR_BAD_PACKET_TAG        3 
#define MKW_ERROR_BAD_PUBKEY_TAG        4 
#define MKW_ERROR_BAD_VERSION           5 
#define MKW_ERROR_BAD_TYPE              6 
#define MKW_ERROR_RSA_KEYGEN            7
#define MKW_ERROR_IO                    8
#define MKW_ERROR_BAD_CHAR              9
#define MKW_ERROR_MDP_BAD_CHECKSUM      10
#define MKW_ERROR_MDP_BAD_QUICK_CHECK   11
#define MKW_ERROR_MDP_MALFORMED         12

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
mkw_membuf_write_mpi(mkw_membuf_t *buf, mpz_t num)
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
    mkw_pgp_packet_seskey_pubkey    = 1,
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

typedef struct mkw_pgp_packet_t {
    enum mkw_pgp_packet_tag_e tag;
    void *packet;
} mkw_pgp_packet_t;

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

static void
mkw_pgp_seskey_pubkey_serialize(mkw_membuf_t *buf,
                                mkw_enckey_t *key)
{
    mkw_membuf_write_uint8(buf, 3 /* version */);
    mkw_membuf_write_str(buf, key->id.data, MKW_ID_SIZE);

    mkw_membuf_write_uint8(buf, mkw_pubkey_tag_rsa);
    mkw_membuf_write_mpi(buf, key->m_to_e_mod_n);
}

static mkw_error_t
mkw_pgp_seskey_pubkey_deserialize(mkw_memreader_t *reader,
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
                         mkw_pubkey_t *pubkey)
{
    mkw_membuf_write_uint8(buf, 4 /* version */);
    mkw_membuf_write_uint32(buf, pubkey->time_created);
    mkw_membuf_write_uint16(buf, pubkey->expires_in_days);
    mkw_membuf_write_uint8(buf, pubkey->tag);

    if (pubkey->tag == mkw_pubkey_tag_rsa) {
        mkw_pubkey_rsa_t *rsa = &pubkey->material.rsa;
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

static void
mkw_pgp_packet_serialize(mkw_membuf_t *buf, mkw_pgp_packet_t *packet)
{
    assert(packet->tag <= 0x3f);
    mkw_membuf_write_uint8(buf, 
                           (0x80) | /* always one*/
                           (0x40) | /* new format */
                           (0x3f & packet->tag));

    mkw_membuf_t *innerbuf = mkw_membuf_create_empty();

    if (packet->tag == mkw_pgp_packet_pubkey) {
        mkw_pgp_pubkey_serialize(innerbuf, packet->packet);
    } else {
        abort();
    }

    write_new_len(buf, innerbuf->size);
    mkw_membuf_write_str(buf, innerbuf->data, innerbuf->size);
}

static mkw_error_t
mkw_pgp_packet_deserialize(mkw_memreader_t *reader,
                           mkw_pgp_packet_t *packet_p)
{
    uint8_t header;

    MKW_ERR(mkw_memreader_read_uint8(reader, &header));
    int just_one    = 0x80 & header;
    int new_format  = 0x40 & header;
    int tag         = 0x3f & header;

    if (! (just_one && new_format)) {
        return MKW_ERROR_MAFORMED_PACKET;
    }

    packet_p->tag = tag;
    if (tag == mkw_pgp_packet_pubkey) {
        MKW_ERR(mkw_pgp_pubkey_deserialize(reader, packet_p->packet));
    } else {
        return MKW_ERROR_BAD_PACKET_TAG;
    }

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

#define MKW_MDP_SIZE 16

static void
mkw_mdp_write(mkw_ctx_t *ctx, mkw_membuf_t *mdp,
              const uint8_t *data, const uint8_t len)
{
    struct sha1_ctx checksum;

    /* prepends random salt+two last bytes */
    nettle_yarrow256_random(&ctx->rng, MKW_MDP_SIZE,
                            mkw_membuf_write_buf(mdp, MKW_MDP_SIZE));
    mkw_membuf_write_uint8(mdp, mdp->data[MKW_MDP_SIZE - 2]);
    mkw_membuf_write_uint8(mdp, mdp->data[MKW_MDP_SIZE - 1]);

    /* the dat itself */
    mkw_membuf_write_str(mdp, data, len);

    /* and the SHA1 checksum */
    nettle_sha1_init(&checksum);
    nettle_sha1_update(&checksum, len, data);
    nettle_sha1_digest(&checksum, SHA1_DIGEST_SIZE,
                       mkw_membuf_write_buf(mdp, SHA1_DIGEST_SIZE));
}
 
static void
mkw_symkey_encrypt(mkw_ctx_t *ctx,
                   const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   const uint8_t len) 
{
#define BLOCK_SIZE sizeof(key->key)

    uint8_t iv[BLOCK_SIZE] = { 0 };
    struct aes128_ctx aesctx = { 0 };
    mkw_membuf_t *mdp = mkw_membuf_create_empty();
    size_t output_size;


    output_size = (mdp->size + BLOCK_SIZE - 1) / BLOCK_SIZE * BLOCK_SIZE;

    nettle_aes128_set_encrypt_key(&aesctx, key->key);
    nettle_cfb_encrypt(&aesctx,
                       (nettle_cipher_func *)nettle_aes128_encrypt,
                       BLOCK_SIZE, iv,
                       mdp->size,
                       mkw_membuf_write_buf(out, output_size),
                       mdp->data);
}

static mkw_error_t
mkw_symkey_decrypt(const mkw_symkey_aes_t *key,
                   mkw_membuf_t *out,
                   const uint8_t *data,
                   const uint8_t len) 
{
#define BLOCK_SIZE sizeof(key->key)

    uint8_t iv[BLOCK_SIZE] = { 0 };
    struct aes128_ctx aesctx = { 0 };
    struct sha1_ctx checksum;
    mkw_membuf_t *mdp = mkw_membuf_create_empty();
    size_t output_size;

    nettle_aes128_set_encrypt_key(&aesctx, key->key);
    nettle_cfb_decrypt(
            &aesctx, (nettle_cipher_func *)nettle_aes128_decrypt,
            BLOCK_SIZE, iv, mdp->size,
            mkw_membuf_write_buf(out, output_size),
            mdp->data);


}

static mkw_error_t
mkw_user_create(mkw_ctx_t *ctx, mkw_user_t *user_p)
{
    int status;

    MKW_ERR(mkw_id_create(ctx, &user_p->id));
    user_p->key.tag = mkw_pubkey_tag_rsa;

    rsa_public_key_init(&user_p->key.material.rsa.pubkey);
    rsa_private_key_init(&user_p->key.material.rsa.seckey);

    status = nettle_rsa_generate_keypair(
            &user_p->key.material.rsa.pubkey,
            &user_p->key.material.rsa.seckey,
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

static mkw_error_t
sub_main()
{
    mkw_blobstore_t *store = mkw_blobstore_create_mem();
    mkw_user_t user;
    mkw_ctx_t ctx;

    MKW_ERR(mkw_ctx_create(&ctx));
    MKW_ERR(mkw_user_create(&ctx, &user));

    mkw_pubkey_t pubkey = {
        .time_created = 0,
        .expires_in_days = 0,
        .tag = mkw_pubkey_tag_rsa,
        .material.rsa = user.key.material.rsa.pubkey, 
    };

    mkw_pgp_packet_t packet = {
        .tag = mkw_pgp_packet_pubkey,
        .packet = &pubkey,
    };

    mkw_membuf_t *text = mkw_membuf_create_empty();
    mkw_membuf_t *buf = mkw_membuf_create_empty();

    mkw_pgp_packet_serialize(buf, &packet);
    mkw_pgp_packet_deserialize(mkw_memreader_create(buf->data, buf->size), &packet);
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

