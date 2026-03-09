#ifndef MKW_H
#define MKW_H

#include <assert.h>
#include <stddef.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>

#include <gmp.h> /* for mpz_t */
#include <nettle/yarrow.h> /* for yarrow256_ctx */

#include <stdlib.h>

#define DEBUG
#define max(a, b) (((a) > (b)) ? (a) : (b))
#define min(a, b) (((a) < (b)) ? (a) : (b))

typedef int mkw_error_t;

#define MKW_ERR(expr) do { \
    mkw_error_t __err = (expr); \
    if (__err != MKW_ERROR_NONE) \
        return __err; \
} while(0);

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
#define MKW_ERROR_BAD_BLOCK_SIZE                20
#define MKW_ERROR_BAD_S2K_USAGE                 21 
#define MKW_ERROR_PAYLOAD_BAD_LENGTH            22
#define MKW_ERROR_PAYLOAD_MALFORMED             23
#define MKW_ERROR_PAYLOAD_BAD_KEY               24

/* memory allocations */
typedef struct mkw_pool_t mkw_pool_t; 

mkw_pool_t *mkw_pool_create();
void mkw_pool_nuke(mkw_pool_t *pool);

void *mkw_palloc(mkw_pool_t *pool, size_t size);
char *mkw_pstrndup(mkw_pool_t *pool, const char *str, size_t len);
char *mkw_pstrdup(mkw_pool_t *pool, const char *cstr);

#define mkw_pcalloc(pool, size) memset(mkw_palloc(pool, size), 0, size)

/* growable collection of pointers */
typedef struct mkw_vector_t {
    void **data;
    size_t size;
    size_t capacity;
    mkw_pool_t *pool;
} mkw_vector_t;

mkw_vector_t *mkw_vector_create_empty(mkw_pool_t *pool);
void mkw_vector_resize(mkw_vector_t *vec, size_t new_capacity);
void mkw_vector_ensure(mkw_vector_t *vec, size_t size);
void mkw_vector_push(mkw_vector_t *vec, void *elem);

/* growable byte buffer */
typedef struct mkw_membuf_t {
    uint8_t *data;
    size_t size;
    size_t capacity;
    mkw_pool_t *pool;
} mkw_membuf_t;

mkw_membuf_t *
mkw_membuf_create(mkw_pool_t *pool, size_t capacity);

#define mkw_membuf_create_empty(pool) mkw_membuf_create(pool, 64)

mkw_membuf_t *
mkw_membuf_create_from_cstr(mkw_pool_t *pool, const char *str);

mkw_membuf_t *
mkw_membuf_create_from_nstr(mkw_pool_t *pool,
                            const char *str, size_t size);

void mkw_membuf_resize(mkw_membuf_t *buf, size_t new_capacity);
void mkw_membuf_ensure(mkw_membuf_t *buf, size_t size);

void mkw_membuf_write_str(mkw_membuf_t *buf, const uint8_t *str, size_t len);
void mkw_membuf_write_cstr(mkw_membuf_t *buf, const char *str);
void mkw_membuf_write_uint8(mkw_membuf_t *buf, uint8_t num);
void mkw_membuf_write_uint16(mkw_membuf_t *buf, uint16_t num);
void mkw_membuf_write_uint32(mkw_membuf_t *buf, uint32_t num);
void mkw_membuf_write_mpi(mkw_membuf_t *buf, const mpz_t num);
void mkw_membuf_write_mpi_str(mkw_membuf_t *buf, mkw_membuf_t *mpi);
void mkw_membuf_write_base16(mkw_membuf_t *buf, uint8_t *data, size_t len);
uint8_t *mkw_membuf_write_buf(mkw_membuf_t *buf, size_t len);

/* memory reader */
typedef struct mkw_memreader_t {
    const uint8_t *data;
    size_t remaining;
} mkw_memreader_t;

mkw_memreader_t *
mkw_memreader_create(const uint8_t *data, size_t size, mkw_pool_t *pool);

mkw_error_t
mkw_memreader_read_uint8(mkw_memreader_t *reader, uint8_t *result);

mkw_error_t
mkw_memreader_read_uint16(mkw_memreader_t *reader, uint16_t *result);

mkw_error_t
mkw_memreader_read_uint32(mkw_memreader_t *reader, uint32_t *result);

mkw_error_t
mkw_memreader_read_mpi(mkw_memreader_t *reader, mpz_t mpi);

mkw_error_t
mkw_memreader_readline(mkw_memreader_t *reader,
                       mkw_membuf_t *buf,
                       const char *eol);

mkw_error_t
mkw_memreader_eat_cstr(mkw_memreader_t *reader, const char *str);

mkw_error_t
mkw_memreader_read_buf(mkw_memreader_t *reader,
                       uint8_t *buf, size_t size);

mkw_error_t
mkw_memreader_subreader(mkw_memreader_t *reader,
                        mkw_memreader_t *subreader,
                        size_t len);

/* library context */
typedef struct mkw_ctx_t {
    struct yarrow256_ctx rng;
} mkw_ctx_t;

mkw_error_t
mkw_ctx_create(mkw_ctx_t *ctx, mkw_pool_t *pool);

#include <stdio.h>
void mkw_base16_dump(FILE *file, const uint8_t *str, size_t len);

#endif
