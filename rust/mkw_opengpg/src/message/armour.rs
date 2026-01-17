#[derive(Debug)]
pub struct mkw_pgp_armour_invalid_t;

pub fn read_armour<'a>(
    line: &'a [u8],
) -> Result<&'a [u8], mkw_pgp_armour_invalid_t> {
    // -----BEGIN PGP SIGNED MESSAGE-----

    const PREFIX: &[u8] = &[b'-'; 5];

    let result = line
        .strip_prefix(PREFIX)
        .ok_or_else(|| mkw_pgp_armour_invalid_t)?
        .strip_suffix(PREFIX)
        .ok_or_else(|| mkw_pgp_armour_invalid_t)?;

    return Result::Ok(result);
}
