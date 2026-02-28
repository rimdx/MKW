#ifndef MKW_PGP_H
#define MKW_PGP_H

#include "mkw.h"
#include "mkw_crypto.h"

/* id */
#define MKW_ID_SIZE 8
typedef struct mkw_id_t {
    uint8_t data[MKW_ID_SIZE];
} mkw_id_t;

mkw_error_t
mkw_id_create(mkw_ctx_t *ctx, mkw_id_t *id);

int mkw_id_compare(const mkw_id_t *left, const mkw_id_t *right); 

/* pgp packets */
typedef uint32_t mkw_pgp_time_t;

enum mkw_pgp_packet_tag_e {
    mkw_pgp_packet_session_pubkey   = 1,
    mkw_pgp_packet_seckey           = 5,
    mkw_pgp_packet_pubkey           = 6,
};

enum mkw_pubkey_tag_e {
    mkw_pubkey_tag_rsa          = 1,
};

void
mkw_pgp_packet_serialize(mkw_membuf_t *buf,
                         enum mkw_pgp_packet_tag_e tag,
                         uint8_t *body, size_t bodylen);

mkw_error_t
mkw_pgp_packet_deserialize(mkw_memreader_t *reader,
                           enum mkw_pgp_packet_tag_e *tag,
                           mkw_memreader_t *body);

enum mkw_symkey_tag_e {
    mkw_symkey_tag_plaintext    = 0,
    mkw_symkey_tag_aes128       = 7,
    mkw_symkey_tag_aes192       = 8,
    mkw_symkey_tag_aes256       = 9,
};

/* public key packet */
typedef struct mkw_pubkey_t {
    mkw_pgp_time_t time_created;
    uint16_t expires_in_days;

    enum mkw_pubkey_tag_e tag;
    union {
        mkw_pubkey_rsa_t rsa;
    } material;
} mkw_pubkey_t;

void
mkw_pgp_pubkey_serialize(mkw_membuf_t *buf,
                         const mkw_pubkey_t *pubkey);

mkw_error_t
mkw_pgp_pubkey_deserialize(mkw_memreader_t *reader,
                           mkw_pubkey_t *pubkey_p);

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

/* public key encrypted sesssion key packet */
typedef struct mkw_enckey_t {
    /* who we encrypt for */
    mkw_id_t id;
    mpz_t m_to_e_mod_n;
} mkw_enckey_t;

void
mkw_pgp_session_pubkey_serialize(mkw_membuf_t *buf,
                                 mkw_enckey_t *key);

mkw_error_t
mkw_pgp_session_pubkey_deserialize(mkw_memreader_t *reader,
                                   mkw_enckey_t *key);

/* encrypted secret key */
typedef struct mkw_encseckey_t {
    mkw_membuf_t key;
    mkw_s2k_t s2k;
} mkw_encseckey_t;

void
mkw_pgp_seckey_serialize(mkw_membuf_t *buf,
                         const mkw_encseckey_t *seckey);

#endif
