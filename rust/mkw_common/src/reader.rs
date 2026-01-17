use std::io::Read;

pub fn read_byte<R: Read>(reader: &mut R) -> std::io::Result<Option<u8>> {
    return reader.bytes().next().transpose();
}

#[derive(Debug)]
pub enum Error {
    IOError(std::io::Error),
    UnexpectedData,
}

type Result<T> = std::result::Result<T, Error>;

impl From<std::io::Error> for Error {
    fn from(value: std::io::Error) -> Self {
        return Self::IOError(value);
    }
}

pub fn eat_exact<R: Read, const SIZE: usize>(
    reader: &mut R,
    literal: &[u8; SIZE],
) -> Result<()> {
    let buf = &mut [0u8; SIZE];

    reader.read_exact(buf)?;

    match buf == literal {
        true => return Result::Ok(()),
        false => return Result::Err(Error::UnexpectedData),
    }
}
