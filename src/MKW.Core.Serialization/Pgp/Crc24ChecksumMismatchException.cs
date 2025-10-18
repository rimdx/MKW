namespace MKW.Core.Serialization.Pgp
{
    public sealed class Crc24ChecksumMismatchException(int expected, int actual)
        : Exception($"CRC checksum mismatch: {expected:X6} - {actual:X6}")
    {
    }
}
