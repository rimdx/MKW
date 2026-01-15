#[cfg(test)]
mod tests {
    use mkw_common::id::{mkw_entry_id_t, mkw_user_id_t};
    use mkw_storage::blobstore::IBlobStore;
    use mkw_storage::blobstore::memory::*;
    use mkw_storage::blobstore::entry::*;
    use mkw_storage::blobstore::txn::*;

    #[test]
    fn simple_test() {
        let mut storage = mkw_blobstore_memory_t::new();

        // we should have no elements at the begining.
        storage.with_transation(|tx| -> Result<(), ()> {
            assert_eq!(tx.entries().len(), 0);
            return Result::Ok(());
        }).unwrap();

        let id1 = mkw_entry_id_t::create();

        // create some entries
        storage.with_transation(|tx| -> Result<(), ()> {
            tx.create(mkw_blobstore_entry_t::entry
                {
                    id: id1.clone(),
                    data: Box::new([1, 2, 3]),
                });

            tx.create(mkw_blobstore_entry_t::user
                {
                    id: mkw_user_id_t::create(),
                    data: Box::new([4, 5, 6]),
                });

            return Result::Ok(());
        }).unwrap();

        // verify that the transaction successfully committed
        storage.with_transation(|tx| -> Result<(), ()> {
            assert_eq!(tx.entries().len(), 2);
            return Result::Ok(());
        }).unwrap();

        // reject transaction
        let result = storage.with_transation(|tx| -> Result<(), ()> {
            tx.create(mkw_blobstore_entry_t::entry
                {
                    id: mkw_entry_id_t::create(),
                    data: Box::new([1, 2, 3]),
                });

            tx.create(mkw_blobstore_entry_t::user
                {
                    id: mkw_user_id_t::create(),
                    data: Box::new([4, 5, 6]),
                });

            return Result::Err(());
        });

        assert_eq!(result, Result::Err(()));

        // there shuold be no changes
        storage.with_transation(|tx| -> Result<(), ()> {
            assert_eq!(tx.entries().len(), 2);
            return Result::Ok(());
        }).unwrap();


    }
}

