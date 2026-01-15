use crate::blobstore::{
    entry::mkw_blobstore_entry_t, mkw_blobstore_txn_t, mkw_txn_result_t, IBlobStore,
};

pub struct mkw_blobstore_memory_t {
    entries: Vec<mkw_blobstore_entry_t>,
}

impl IBlobStore for mkw_blobstore_memory_t {
    fn with_transation<T, E, F>(&mut self, func: F) -> Result<T, E>
    where
        F: Fn(&mut mkw_blobstore_txn_t) -> mkw_txn_result_t<T, E>,
    {
        let mut entries = self.entries.to_vec();

        let result: mkw_txn_result_t<T, E> = {
            let mut txn = mkw_blobstore_txn_t::new(&mut entries);
            func(&mut txn)
        };

        match result {
            mkw_txn_result_t::commit(data) => {
                self.entries = entries;
                return Result::Ok(data);
            }
            mkw_txn_result_t::rollback(err) => {
                return Result::Err(err);
            }
        };
    }
}

impl mkw_blobstore_memory_t {
    pub fn new() -> Self {
        return Self {
            entries: Vec::new(),
        };
    }
}
