pub struct mkw_bufreader<'a> {
    buf: &'a [u8],
}

impl<'a> mkw_bufreader<'a> {
    pub fn new(buf: &'a [u8]) -> Self {
        return mkw_bufreader {
            buf: buf,
        };
    }

    pub fn read_all(&mut self) -> &'a [u8] {
        let result = self.buf;
        self.buf = &[];
        return result;
    }

    pub fn read_buf(&mut self, len: usize) -> Option<&'a [u8]> {
        if (self.buf.len() < len) {
            return Option::None;
        } else {
            let (result, buf) = self.buf.split_at(len);
            self.buf = buf;
            return Option::Some(result);
        }
    }

    pub fn read_byte(&mut self) -> Option<u8> {
        if (self.buf.is_empty()) {
            return Option::None;
        } else {
            let result = self.buf[0];
            self.buf = &self.buf[1..];
            return Option::Some(result);
        }
    }
}

#[cfg(test)]
mod test {
    use crate::bufreader::mkw_bufreader;

    #[test]
    fn test_read_byte() {
        let mut reader = mkw_bufreader::new(&[1,2,3]);
        assert_eq!(reader.read_byte(), Option::Some(1));
        assert_eq!(reader.read_byte(), Option::Some(2));
        assert_eq!(reader.read_byte(), Option::Some(3));
        assert_eq!(reader.read_byte(), Option::None);
    }

    #[test]
    fn test_read_buf() {
        let mut reader = mkw_bufreader::new(&[1,2,3]);
        assert_eq!(reader.read_buf(2), Option::Some(&[1u8, 2u8][..]));
        assert_eq!(reader.read_buf(2), Option::None);
        assert_eq!(reader.read_buf(1), Option::Some(&[3u8][..]));
    }
}

