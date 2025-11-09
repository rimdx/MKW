// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.OpenPgp
{
    public sealed class SessionKeyChecksumMismatchException(int expected, int actual)
        : Exception($"Session key checksum mismatch: {expected:X4} - {actual:X4}")
    {
    }
}
