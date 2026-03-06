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
