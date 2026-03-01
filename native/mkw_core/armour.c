#include "mkw.h"
#include "mkw_storage.h"

#include "nettle/base64.h"

/* PGP armour */
#define MKW_TYPE_STR_ENTRY "MKW ENTRY" 
#define MKW_TYPE_STR_USER "MKW USER" 

const char *
mkw_blob_type_encode(enum mkw_blob_type_e type) {
    if (type == mkw_blob_type_entry) {
        return MKW_TYPE_STR_ENTRY;
    } else if (type == mkw_blob_type_user) {
        return MKW_TYPE_STR_USER;
    } else {
        abort();
    }
}

const mkw_error_t
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

void
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

void
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

