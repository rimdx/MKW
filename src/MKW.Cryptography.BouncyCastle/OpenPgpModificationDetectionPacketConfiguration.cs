// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.BouncyCastle
{
    internal static class OpenPgpModificationDetectionPacketConfiguration
    {
        public const int BlockSize = 16;
        public const int Sha1Length = 20;

        public static ReadOnlySpan<byte> MDPTag => [0xD3, 0x14];
    }
}
