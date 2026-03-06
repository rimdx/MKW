#include "mkw.h"
#include "mkw_pgp.h"

#include <nettle/yarrow.h>

/* the ID */
mkw_error_t
mkw_id_create(mkw_ctx_t *ctx, mkw_id_t *id)
{
    nettle_yarrow256_random(&ctx->rng, sizeof(id->data), id->data);
    return MKW_ERROR_NONE;
}

int
mkw_id_compare(const mkw_id_t *left, const mkw_id_t *right)
{
    return memcmp(left->data, right->data, MKW_ID_SIZE);
}

/* public key encrypted sesssion key packet */
void
mkw_pgp_session_pubkey_serialize(mkw_membuf_t *buf,
                                 mkw_enckey_t *key)
{
    mkw_membuf_write_uint8(buf, 3 /* version */);
    mkw_membuf_write_str(buf, key->id.data, MKW_ID_SIZE);

    mkw_membuf_write_uint8(buf, mkw_pubkey_tag_rsa);
    mkw_membuf_write_mpi(buf, key->m_to_e_mod_n);
}

mkw_error_t
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

/* public key packet */
void
mkw_pgp_pubkey_serialize(mkw_membuf_t *buf,
                         const mkw_pubkey_t *pubkey)
{
    mkw_membuf_write_uint8(buf, 4 /* version */);
    mkw_membuf_write_uint32(buf, pubkey->time_created);
    mkw_membuf_write_uint16(buf, pubkey->expires_in_days);
    mkw_membuf_write_uint8(buf, mkw_pubkey_tag_rsa);

    /* RSA material */
    mkw_membuf_write_mpi(buf, pubkey->material.n);
    mkw_membuf_write_mpi(buf, pubkey->material.e);
}

mkw_error_t
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

    if (tag == mkw_pubkey_tag_rsa) {
        mkw_memreader_read_mpi(reader, pubkey_p->material.n);
        mkw_memreader_read_mpi(reader, pubkey_p->material.e);
    } else {
        return MKW_ERROR_BAD_PUBKEY_TAG;
    }

    return MKW_ERROR_NONE;
}

/* encrypted secret key */
void
mkw_pgp_seckey_serialize(mkw_membuf_t *buf,
                         const mkw_encseckey_t *seckey)
{
    mkw_membuf_write_uint8(buf, MKW_S2K_USAGE_SOME_SHA1);
    mkw_pgp_s2k_serialize(buf, &seckey->s2k);
}
