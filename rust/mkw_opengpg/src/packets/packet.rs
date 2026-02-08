use mkw_common::bufreader::mkw_bufreader;

const PUBLIC_KEY_ENCRYPTED_SESSION: u8 = 1;
const SIGNATURE: u8 = 2;
const SYMMETRIC_KEY_ENCRYPTED_SESSION_KEY: u8 = 3;
const ONE_PASS_SIGNATURE: u8 = 4;
const SECRET_KEY: u8 = 5;
const PUBLIC_KEY: u8 = 6;
const SECRET_SUBKEY: u8 = 7;
const COMPRESSED_DATA: u8 = 8;
const SYMMETRIC_KEY_ENCRYPTED: u8 = 9;
const MARKER: u8 = 10;
const LITERAL_DATA: u8 = 11;
const TRUST: u8 = 12;
const USER_ID: u8 = 13;
const PUBLIC_SUBKEY: u8 = 14;
const USER_ATTRIBUTE: u8 = 17;
const SYMMETRIC_ENCRYPTED_INTEGRITY_PROTECTED: u8 = 18;
const MODIFICATION_DETECTION_CODE: u8 = 19;

#[derive(Debug, Eq, PartialEq)]
pub enum mkw_pgp_packet<'a> {
    PublicKey(&'a [u8]),
}

impl<'a> mkw_pgp_packet<'a> {
    pub fn serialize(&self, writer: &mut Vec<u8>) {
        let (tag, content) = match self {
            Self::PublicKey(data) => (PUBLIC_KEY, data),
        };

        let new_format = 1;
        let header: u8 =
            0b1000_0000 |
            0b0100_0000 * new_format |
            0b0011_1111 & tag;

        writer.push(header);
        write_length(writer, content.len() as u64);
        writer.extend_from_slice(content);
    }

    pub fn deserialize(reader: &mut mkw_bufreader<'a>) -> Option<Self> {
        let byte = reader.read_byte()?;

        // unpack things from type
        let just_one = byte & 0b1000_0000;
        let new_format = byte & 0b0100_0000;
        let tag = byte & 0b0011_1111;

        assert_eq!(just_one, 1);
        assert_eq!(new_format, 1);

        let len = read_length(reader)? as usize;
        let content = reader.read_buf(len)?;

        return match tag {
            PUBLIC_KEY => Option::Some(mkw_pgp_packet::PublicKey(content)),
            _ => Option::None,
        }
    }
} 

fn read_length<'a>(reader: &mut mkw_bufreader<'a>) -> Option<u64> {
    let b0 = reader.read_byte()? as u64;
    
    if (b0 < 192) {
        return Option::Some(b0);
    } else if (b0 < 224) {
        let b1 = reader.read_byte()? as u64;
        return Option::Some((((b0 - 192) << 8) + b1) + 192);
    } else if (b0 == 255) {
        return Option::Some(
            (reader.read_byte()? as u64) << 24 |
            (reader.read_byte()? as u64) << 16 |
            (reader.read_byte()? as u64) << 8 |
            (reader.read_byte()? as u64)
        );
    } else {
        return Option::None;
    }
}

fn write_length(writer: &mut Vec<u8>, len: u64) {
    if (len < 192) {
        writer.push(len as u8);
    } else if (len <= ((223 - 192) << 8) + 0xFF + 192) {
        let len = len - 192;
        writer.push(((len >> 8 & 0xff) + 192) as u8);
        writer.push((len & 0xff) as u8);
    } else {
        writer.push(0xff);
        writer.push(((len & 0xff00_0000) >> 24) as u8);
        writer.push(((len & 0x00ff_0000) >> 16) as u8);
        writer.push(((len & 0x0000_ff00) >> 8) as u8);
        writer.push(((len & 0x0000_00ff) >> 0) as u8);
    }
}

#[cfg(test)]
mod test {

    use crate::packets::packet::*;

    #[test]
    fn test_read_length() {
        assert_eq!(
            read_length(&mut mkw_bufreader::new(&[42])),
            Option::Some(42)
        );

        assert_eq!(
            read_length(&mut mkw_bufreader::new(&[200, 10])),
            Option::Some(2250)
        );
    }

    #[test]
    fn test_serializer() {
        let original = mkw_pgp_packet::PublicKey(&[0, 1, 2]);
        let mut writer = Vec::new();
        original.serialize(&mut writer);
        let decoded = mkw_pgp_packet::deserialize(&mut mkw_bufreader::new(&writer.as_slice()));
        assert_eq!(Option::Some(original), decoded);
    }
}
