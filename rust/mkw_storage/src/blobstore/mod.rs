use crate::blobstore::txn::mkw_blobstore_txn_t;

pub mod entry;
pub mod memory;
pub mod txn;

pub enum mkw_txn_result_t<T, E> {
    commit(T),
    rollback(E),
}

impl<T, E> Into<Result<T, E>> for mkw_txn_result_t<T, E> {
    fn into(self) -> Result<T, E> {
        return match self {
            Self::commit(data) => Result::Ok(data),
            Self::rollback(data) => Result::Err(data),
        };
    }
}

pub trait IBlobStore {
    fn with_transation<T, E, F>(&mut self, func: F) -> Result<T, E>
    where
        F: Fn(&mut mkw_blobstore_txn_t) -> mkw_txn_result_t<T, E>;
}
