extern crate rand;

const USER_ID_SIZE: usize = 8;
const ENTRY_ID_SIZE: usize = 8;

#[derive(Clone, Debug, PartialEq, Eq)]
pub struct mkw_user_id_t {
    data: [u8; USER_ID_SIZE],
}

#[derive(Clone, Debug, PartialEq, Eq)]
pub struct mkw_entry_id_t {
    data: [u8; ENTRY_ID_SIZE],
}

impl mkw_user_id_t {
    pub fn create() -> Self {
        let mut buf = [0u8; USER_ID_SIZE];
        let mut rng = rand::rng();

        rand::RngCore::fill_bytes(&mut rng, &mut buf[..]);

        return Self { data: buf };
    }
}

impl mkw_entry_id_t {
    pub fn create() -> Self {
        let mut buf = [0u8; ENTRY_ID_SIZE];
        let mut rng = rand::rng();

        rand::RngCore::fill_bytes(&mut rng, &mut buf[..]);

        return Self { data: buf };
    }
}

#[cfg(test)]
mod tests {
    use crate::id::mkw_user_id_t;

    #[test]
    fn create_user_id_test() {
        let id = mkw_user_id_t::create();
        println!("{:?}", id);
    }
}
