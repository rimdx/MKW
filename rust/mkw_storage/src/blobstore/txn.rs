use mkw_common::id::{mkw_entry_id_t, mkw_user_id_t};

use crate::blobstore::entry::{mkw_blobstore_entry_t, mkw_blobstore_id_t};

#[derive(Debug)]
pub struct mkw_blobstore_txn_t<'a> {
    entries: &'a mut Vec<mkw_blobstore_entry_t>,
}

impl<'a> mkw_blobstore_txn_t<'a> {
    pub fn new(entries: &'a mut Vec<mkw_blobstore_entry_t>) -> Self {
        return Self { entries };
    }

    pub fn get(&self, id: &mkw_blobstore_id_t) -> Option<&mkw_blobstore_entry_t> {
        for item in self.entries.iter() {
            if item.get_id() == *id {
                return Option::Some(item);
            }
        }

        return Option::None;
    }

    pub fn entries(&self) -> &[mkw_blobstore_entry_t] {
        return &self.entries[..];
    }

    pub fn create(&mut self, entry: mkw_blobstore_entry_t) -> bool {
        if self.get(&entry.get_id()).is_some() {
            return false;
        }

        self.entries.push(entry);

        return true;
    }
}

