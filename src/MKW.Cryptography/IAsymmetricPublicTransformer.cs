// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public interface IAsymmetricPublicTransformer : IDisposable
    {
        Memory<byte> Encrypt(ReadOnlySpan<byte> data);
        Memory<byte> ExportPublicKey();
        bool Verify(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature);
    }
}
