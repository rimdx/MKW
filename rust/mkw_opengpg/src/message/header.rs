#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct mkw_pgp_armour_header_t<'a> {
    key: &'a [u8],
    value: &'a [u8],
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct mkw_pgp_armour_header_invalid_t;

impl<'a> mkw_pgp_armour_header_t<'a> {
    pub fn new(key: &'a [u8], value: &'a [u8]) -> mkw_pgp_armour_header_t<'a> {
        return mkw_pgp_armour_header_t { key, value };
    }

    pub fn parse(
        line: &'a [u8],
    ) -> Result<
        Option<mkw_pgp_armour_header_t<'a>>,
        mkw_pgp_armour_header_invalid_t,
    > {
        // key: value [whitespaces]

        let line = line.trim_ascii_end();

        if line.is_empty() {
            return Result::Err(mkw_pgp_armour_header_invalid_t);
        }

        let separator_idx = line
            .iter()
            .position(|b| b == &b':')
            .ok_or_else(|| mkw_pgp_armour_header_invalid_t)?;

        let (key, value) = line.split_at(separator_idx);

        let value = value
            .strip_prefix(b": ")
            .ok_or_else(|| mkw_pgp_armour_header_invalid_t)?;

        return Result::Ok(Option::Some(Self::new(key, value)));
    }
}

#[cfg(test)]
mod test {
    use crate::message::header::*;

    #[test]
    fn simple_test() {
        assert_eq!(
            mkw_pgp_armour_header_t::parse(b"key: value"),
            Result::Ok(Option::Some(mkw_pgp_armour_header_t::new(
                b"key", b"value"
            ))),
        );

        assert_eq!(
            mkw_pgp_armour_header_t::parse(b"key: value    "),
            Result::Ok(Option::Some(mkw_pgp_armour_header_t::new(
                b"key", b"value"
            ))),
        );

        assert_eq!(
            mkw_pgp_armour_header_t::parse(b""),
            Result::Err(mkw_pgp_armour_header_invalid_t),
        );

        assert_eq!(
            mkw_pgp_armour_header_t::parse(b"    "),
            Result::Err(mkw_pgp_armour_header_invalid_t)
        );
    }
}
