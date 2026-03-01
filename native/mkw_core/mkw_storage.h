#ifndef MKW_STORAGE_H
#define MKW_STORAGE_H

#include "mkw.h"
#include "mkw_pgp.h"

/* blobstore */
typedef struct mkw_blobstore_t {
    mkw_vector_t *entries;
} mkw_blobstore_t;

enum mkw_blob_type_e {
    mkw_blob_type_entry,
    mkw_blob_type_user,
};

typedef struct mkw_blobstore_entry_t {
    enum mkw_blob_type_e type;
    mkw_id_t *id;
    mkw_membuf_t *data;
} mkw_blobstore_entry_t;

mkw_blobstore_t *
mkw_blobstore_create_mem();

mkw_blobstore_entry_t *
mkw_blobstore_get_entry(mkw_blobstore_t *store,
                        const mkw_id_t *id);

void
mkw_blobstore_create_entry(mkw_blobstore_t *store,
                           mkw_blobstore_entry_t *entry);


/* armour */
void
mkw_pgp_armour_encode_body(mkw_membuf_t *dst,
                           const uint8_t *data, size_t size);

void
mkw_pgp_armour_write(mkw_membuf_t *dst,
                     mkw_blobstore_entry_t *entry);

#endif
