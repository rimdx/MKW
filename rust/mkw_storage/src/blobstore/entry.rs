use mkw_common::id::{mkw_entry_id_t, mkw_user_id_t};

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum mkw_blobstore_id_t {
    entry(mkw_entry_id_t),
    user(mkw_user_id_t),
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum mkw_blobstore_entry_t {
    entry { id: mkw_entry_id_t, data: Box<[u8]> },
    user { id: mkw_user_id_t, data: Box<[u8]> },
}

impl mkw_blobstore_entry_t {
    pub fn get_id(&self) -> mkw_blobstore_id_t {
        return match self {
            Self::entry { id, data: _ } => mkw_blobstore_id_t::entry(id.clone()),
            Self::user { id, data: _ } => mkw_blobstore_id_t::user(id.clone()),
        };
    }
}
