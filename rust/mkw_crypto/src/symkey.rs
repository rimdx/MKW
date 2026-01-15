use aes::cipher::{AsyncStreamCipher, KeyIvInit};

pub enum mkw_crypto_symkey_t {
    opengpg_aes_128 { key: [u8; 16] },
}

impl mkw_crypto_symkey_t {
    fn encrypt(&self, data: &[u8], output: &mut [u8]) {
        assert_eq!(output.len(), self.encrypt_align(output.len()));

        match self {
            Self::opengpg_aes_128 { key } => {
                type enc_t = cfb_mode::Encryptor<aes::Aes128>;

                // opengpg standard doesn't use AES IV. it's always all zeros
                //
                // it instead uses its own feedback mode that prepends some random data for better
                // security.
                //
                // in future we should stop hardcoding its size, by using some rust magic instead.
                let iv = &[0u8; 16];

                let alg = enc_t::new(key.into(), iv.into());

                // the only reason the error might occur is when data.len != output.len.
                //
                // we already asserted for invalid data so throw the error away (or die)
                alg.encrypt_b2b(data, output).unwrap();

                // todo: implement openpgp feedback mode
            }
        }
    }

    fn encrypt_align(&self, len: usize) -> usize {
        match self {
            Self::opengpg_aes_128 { key: _ } => {
                return len + len % 16;
            }
        }
    }
}
