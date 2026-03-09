#include "mkw.h"
#include "mkw_pgp.h"
#include "mkw_storage.h"

mkw_blobstore_t *
mkw_blobstore_create_mem(mkw_pool_t *pool)
{
    mkw_blobstore_t *result = mkw_pcalloc(pool, sizeof(*result));
    result->entries = mkw_vector_create_empty(pool);
    return result;
}

mkw_blobstore_entry_t *
mkw_blobstore_get_entry(mkw_blobstore_t *store,
                        const mkw_id_t *id)
{
    for (size_t i = 0; i < store->entries->size; i++) {
        mkw_blobstore_entry_t *current = store->entries->data[i];

        if (mkw_id_compare(id, current->id) == 0) {
            return current;
        }
    }

    return NULL;
}

void
mkw_blobstore_create_entry(mkw_blobstore_t *store,
                           mkw_blobstore_entry_t *entry)
{
    mkw_vector_push(store->entries, entry);
}

mkw_error_t
mkw_blobstore_read(mkw_blobstore_t *store,
                   mkw_memreader_t *reader,
                   mkw_pool_t *pool)
{
    abort();
    return MKW_ERROR_NONE;
}

mkw_error_t
mkw_blobstore_write(mkw_blobstore_t *store,
                    mkw_membuf_t *out)
{
    size_t i;
    /* this allows to avoid traling double newline. mkw_pgp_armour_write()
     * writes one newline after the armour. the extra newline makes blocks look
     * more separated.
     *
     * like this:
     *
     * [[[
     * -----BEGIN MKW ENTRY-----
     * ....
     * -----END MKW ENTRY-----
     * (\n)
     * -----BEGIN MKW ENTRY-----
     * ....
     * -----END MKW ENTRY-----
     * */
    int first = 1;

    /* we want to write users before entries.
     * in my opinion it's better to keep things more orgonised like that.
     * there is no real reasoning for that tho.
     * if there was something like a metadata entry (which we will have in
     * future) it would be the first one even before users. and obviously only
     * one of these entries would be allowed.
     */

    for (i = 0; i < store->entries->size; i++) {
        mkw_blobstore_entry_t *entry = store->entries->data[i];
        if (entry->type == mkw_blob_type_user) {
            if (! first) {
                mkw_membuf_write_cstr(out, "\n");
                first = 0;
            }

            mkw_pgp_armour_write(out, entry);
        }
    }

    for (i = 0; i < store->entries->size; i++) {
        mkw_blobstore_entry_t *entry = store->entries->data[i];
        if (entry->type == mkw_blob_type_entry) {
            if (! first) {
                mkw_membuf_write_cstr(out, "\n");
                first = 0;
            }

            mkw_membuf_write_cstr(out, "\n");
        }
    }

    return MKW_ERROR_NONE;
}
