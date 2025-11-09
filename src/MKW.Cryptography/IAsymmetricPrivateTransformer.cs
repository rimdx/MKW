// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public interface IAsymmetricPrivateTransformer : IAsymmetricPublicTransformer, IDisposable
    {
        ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data);
        ReadOnlyMemory<byte> ExportPrivateKey();
        ReadOnlyMemory<byte> Sign(ReadOnlySpan<byte> data);
    }
}
