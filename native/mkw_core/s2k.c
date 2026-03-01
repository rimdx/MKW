#include "mkw.h"
#include "mkw_crypto.h"

void
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

mkw_error_t
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

/* https://www.rfc-editor.org/rfc/rfc9580.html#section-3.7.1.3 */
#define EXPBIAS 6
#define UNPACK_S2K_ITERCOUNT(c) \
    ((uint32_t)16 + (c & 15)) << ((c >> 4) + EXPBIAS)

void
mkw_s2k_derive_key(const mkw_s2k_t *s2k,
                   const uint8_t *passwd, size_t passwdsize,
                   uint8_t *key, size_t keysize)
{
    struct sha256_ctx hash = { 0 };
    uint8_t digest[SHA256_DIGEST_SIZE];

    assert(keysize <= SHA256_DIGEST_SIZE);
    assert(s2k->hash == mkw_hash_tag_sha256);

    nettle_sha256_init(&hash);

    if (s2k->tag == mkw_s2k_tag_simple) {
        nettle_sha256_update(&hash, passwdsize, passwd);
    } else if (s2k->tag == mkw_s2k_tag_salted) {
        nettle_sha256_update(&hash, sizeof(s2k->salt), s2k->salt);
        nettle_sha256_update(&hash, passwdsize, passwd);
    } else if (s2k->tag == mkw_s2k_tag_salted_iterated) {
        size_t remaining = UNPACK_S2K_ITERCOUNT(s2k->count);
        size_t count;

        while (remaining > 0) {
            count = min(sizeof(s2k->salt), remaining);
            nettle_sha256_update(&hash, count, s2k->salt);
            remaining -= count;

            count = min(passwdsize, remaining);
            nettle_sha256_update(&hash, count, passwd);
            remaining -= count;
        }
    } else {
        abort();
    }

    nettle_sha256_digest(&hash, sizeof(digest), digest);
    memcpy(key, digest, keysize);

    /* security consideration */
    memset(&hash, 0, sizeof(hash));
    memset(digest, 0, sizeof(digest));
}

void
mkw_s2k_init(mkw_ctx_t *ctx, mkw_s2k_t *s2k)
{
    s2k->tag = mkw_s2k_tag_salted_iterated;
    s2k->hash = mkw_hash_tag_sha256;
    s2k->count = 0xff;
    nettle_yarrow256_random(&ctx->rng, sizeof(s2k->salt), s2k->salt);
}
