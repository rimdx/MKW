// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public interface ISymmetricTransformer : IDisposable
    {
        ReadOnlyMemory<byte> Encrypt(ReadOnlySpan<byte> data);
        ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data);
    }
}