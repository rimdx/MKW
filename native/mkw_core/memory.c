#include "mkw.h"
#include <stdio.h>
#include <nettle/base16.h>

/* memory primitives */
void *
mkw_alloc(size_t size)
{
#ifdef DEBUG
    fprintf(stderr, "malloc(%ld)\n", size);
#endif
    void *ptr = malloc(size);
    if (! ptr) {
        fputs("out of memory\n", stderr);
        abort();
    }
    return ptr;
}

void
mkw_free(void *ptr)
{
    free(ptr);
#ifdef DEBUG
    fprintf(stderr, "free()\n");
#endif
}


/* growable vector of pointers */
#define MKW_VECTOR_ELEMENT_SIZE sizeof(void *)

mkw_vector_t *
mkw_vector_create_empty()
{ 
    mkw_vector_t *vec = mkw_calloc(sizeof(*vec));
    vec->size = 0;
    vec->capacity = 8;
    vec->data = mkw_calloc(vec->capacity * MKW_VECTOR_ELEMENT_SIZE);
    return vec;
}

void
mkw_vector_resize(mkw_vector_t *vec, size_t new_capacity)
{
    void **new_data = mkw_calloc(new_capacity * MKW_VECTOR_ELEMENT_SIZE); 

#ifdef DEBUG
    fprintf(stderr, "vector.resize(%ld -> %ld)\n",
            vec->capacity, new_capacity);
#endif

    memcpy(new_data, vec->data, vec->size);
    mkw_free(vec->data);

    vec->data = new_data;
    vec->capacity = new_capacity;
}

void
mkw_vector_ensure(mkw_vector_t *vec, size_t size)
{
    if (size > vec->capacity) {
        mkw_vector_resize(vec, max(size, vec->capacity * 2));
    }
}

void
mkw_vector_push(mkw_vector_t *vec, void *elem)
{
    mkw_vector_ensure(vec, vec->size + 1);
    vec->data[vec->size] = elem;
    vec->size++;
}

/* growable memory buffer of bytes */
mkw_membuf_t *
mkw_membuf_create_empty()
{ 
    mkw_membuf_t *buf = mkw_calloc(sizeof(*buf));
    buf->size = 0;
    buf->capacity = 64;
    buf->data = mkw_calloc(buf->capacity);
    return buf;
}

void
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

void
mkw_membuf_ensure(mkw_membuf_t *buf, size_t size)
{
    if (size > buf->capacity) {
        mkw_membuf_resize(buf, max(size, buf->capacity * 2));
    }
}

void
mkw_membuf_write_str(mkw_membuf_t *buf, const uint8_t *str, size_t len)
{
    mkw_membuf_ensure(buf, buf->size + len);
    memcpy(buf->data + buf->size, str, len);
    buf->size += len;
}

void
mkw_membuf_write_cstr(mkw_membuf_t *buf, const char *str)
{
    mkw_membuf_write_str(buf, (uint8_t *)str, strlen(str));
}

void
mkw_membuf_write_uint8(mkw_membuf_t *buf, uint8_t num)
{
    mkw_membuf_ensure(buf, buf->size + 1);
    buf->data[buf->size++] = num;
}

void
mkw_membuf_write_uint16(mkw_membuf_t *buf, uint16_t num)
{
    mkw_membuf_ensure(buf, buf->size + 2);
    buf->data[buf->size++] = (uint8_t)((num & 0xff00) >> 8);
    buf->data[buf->size++] = (uint8_t)((num & 0x00ff) >> 0);
}

void
mkw_membuf_write_uint32(mkw_membuf_t *buf, uint32_t num)
{
    mkw_membuf_ensure(buf, buf->size + 4);
    buf->data[buf->size++] = (uint8_t)((num & 0xff000000) >> 24);
    buf->data[buf->size++] = (uint8_t)((num & 0x00ff0000) >> 16);
    buf->data[buf->size++] = (uint8_t)((num & 0x0000ff00) >> 8);
    buf->data[buf->size++] = (uint8_t)((num & 0x000000ff) >> 0);
}

void
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

void
mkw_membuf_write_mpi_str(mkw_membuf_t *buf, mkw_membuf_t *mpi)
{
    /* TODO: properly calculate amout of bits */
    mkw_membuf_write_uint16(buf, mpi->size * 8);
    mkw_membuf_write_str(buf, mpi->data, mpi->size);
}

void
mkw_membuf_write_base16(mkw_membuf_t *buf, uint8_t *data, size_t len) 
{
    mkw_membuf_ensure(buf, buf->size + BASE16_ENCODE_LENGTH(len));
    nettle_base16_encode_update((char *)(buf->data + buf->size),
                                len, data);
    buf->size += BASE16_ENCODE_LENGTH(len);
}

uint8_t *
mkw_membuf_write_buf(mkw_membuf_t *buf, size_t len)
{
    mkw_membuf_ensure(buf, buf->size + len);
    uint8_t *result = &buf->data[buf->size + len];
    buf->size += len;
    return result;
}

/* memory reader */
mkw_memreader_t *
mkw_memreader_create(const uint8_t *data, size_t size)
{
    mkw_memreader_t *result = mkw_calloc(sizeof(*result));
    result->data = data;
    result->remaining = size;
    return result;
}

mkw_error_t
mkw_memreader_read_uint8(mkw_memreader_t *reader, uint8_t *result)
{
    if (reader->remaining > 0) {
        *result = *reader->data; 
        reader->data++;
        reader->remaining--;
        return MKW_ERROR_NONE;
    } else {
        return MKW_ERROR_EOF;
    }
}

mkw_error_t 
mkw_memreader_read_uint16(mkw_memreader_t *reader, uint16_t *result)
{
    uint8_t b0, b1;
    MKW_ERR(mkw_memreader_read_uint8(reader, &b0));
    MKW_ERR(mkw_memreader_read_uint8(reader, &b1));
    *result = (b0 << 8) | (b1 << 0); 
    return MKW_ERROR_NONE;
}

mkw_error_t 
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

mkw_error_t
mkw_memreader_read_mpi(mkw_memreader_t *reader, mpz_t mpi)
{
    uint16_t bits;
    size_t bytes;

    MKW_ERR(mkw_memreader_read_uint16(reader, &bits));
    bytes = (bits + 7) / 8;

    if (reader->remaining >= bytes) {
        mpz_import(mpi,
                   bytes,   /* count    */
                   1,       /* order    */
                   1,       /* size     */
                   1,       /* endian   */
                   0,       /* nails    */
                   reader->data);
        reader->data += bytes;
        reader->remaining -= bytes;
        return MKW_ERROR_NONE;
    } else {
        return MKW_ERROR_EOF;
    }
}

mkw_error_t
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

mkw_error_t
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

mkw_error_t
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

mkw_error_t
mkw_memreader_subreader(mkw_memreader_t *reader,
                        mkw_memreader_t *subreader,
                        size_t len)
{
    if (reader->remaining >= len ) {
        subreader->data = reader->data;
        subreader->remaining = len;
        reader->data += len;
        reader->remaining -= len;
        return MKW_ERROR_NONE;
    } else {
        reader->data += reader->remaining;
        reader->remaining = 0;
        return MKW_ERROR_EOF;
    }
}
