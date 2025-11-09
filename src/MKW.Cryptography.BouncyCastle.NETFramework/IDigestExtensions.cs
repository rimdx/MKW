// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Crypto;
using System.Buffers;

namespace MKW.Cryptography.BouncyCastle
{
    internal static class IDigestExtensions
    {
        public static void BlockUpdate(this IDigest digest, ReadOnlySpan<byte> input)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(input.Length);

            try
            {
                input.CopyTo(sharedBuffer);
                digest.BlockUpdate(sharedBuffer, 0, input.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
    }
}
