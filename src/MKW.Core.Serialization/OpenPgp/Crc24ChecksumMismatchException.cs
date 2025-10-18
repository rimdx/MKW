// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp
{
    public sealed class Crc24ChecksumMismatchException(int expected, int actual)
        : Exception($"CRC checksum mismatch: {expected:X6} - {actual:X6}")
    {
    }
}
