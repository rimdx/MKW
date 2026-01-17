use mkw_common::reader::{eat_exact, read_byte};
use std::{
    arch::x86_64::_mm512_mask_dpbf16_ps,
    io::{BufRead, Read},
};

use crate::message::{
    armour::read_armour,
    header::{self, mkw_pgp_armour_header_t},
};

pub struct mkw_pgp_message_t<'a> {
    type_header: &'a str,
}

#[derive(Debug)]
pub enum mkw_pgp_message_serialization_error_t {
    IOError(std::io::Error),
    UnexpectedEndOfFile,
    MalformedHeader,
    MalformedArmourHeaderLine,
}

impl From<std::io::Error> for mkw_pgp_message_serialization_error_t {
    fn from(value: std::io::Error) -> Self {
        return mkw_pgp_message_serialization_error_t::IOError(value);
    }
}

type Result<T> = std::result::Result<T, mkw_pgp_message_serialization_error_t>;

fn read_message<StreamT: BufRead>(
    reader: &mut StreamT,
    data: &mut Vec<u8>,
) -> Result<()> {
    let mut line_header: Vec<u8> = Vec::new();
    reader.read_until(b'\n', &mut line_header)?;

    match read_armour(&mut line_header) {
        Ok(type_header) => {}
        Err(_err) => return Result::Err(
            mkw_pgp_message_serialization_error_t::MalformedArmourHeaderLine,
        ),
    }

    loop {
        let mut line: Vec<u8> = Vec::new();
        reader.read_until(b'\n', &mut line)?;

        match mkw_pgp_armour_header_t::parse(&mut line) {
            Ok(opt) => match opt {
                Some(header) => {}
                None => break,
            },
            Err(_err) => {
                return Result::Err(
                    mkw_pgp_message_serialization_error_t::MalformedHeader,
                )
            }
        }
    }

    return Result::Ok(());
}

#[cfg(test)]
mod test {
    use crate::message::serializer::*;
    use std::{io::Cursor, vec};

    // https://www.rfc-editor.org/rfc/rfc9580.html#name-sample-cleartext-signed-mes
    const TESTVECTOR: &[u8] = b"-----BEGIN PGP SIGNATURE-----\n\
        Version: 6.7\n\
        customkey: value\n\
        \n\
        wpgGARsKAAAAKQWCY5ijYyIhBssYbE8GCaaX5NUt+mxyKwwfHifBilZwj2Ul7Ce6\n\
        2azJAAAAAGk2IHZJX1AhiJD39eLuPBgiUU9wUA9VHYblySHkBONKU/usJ9BvuAqo\n\
        /FvLFuGWMbKAdA+epq7V4HOtAPlBWmU8QOd6aud+aSunHQaaEJ+iTFjP2OMW0KBr\n\
        NK2ay45cX1IVAQ==\n\
        -----END PGP SIGNATURE-----";

    #[test]
    fn simple_test() {
        let mut buf = Cursor::new(TESTVECTOR);
        let mut data = Vec::new();

        read_message(&mut buf, &mut data).unwrap();

        // only verify first few and last few bytes with sample from web for
        // simplicity and because I really don't care.
        assert_eq!(&data[..2], [0xc2, 0x98]);
        assert_eq!(&data[data.len() - 2..], [0xc2, 0x98]);
    }
}
