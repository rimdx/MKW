// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;

namespace MKW.Core.Client
{
    internal sealed class GetSymmetricKeyVisitor : SymmetricKey.IVisitor<ReadOnlyMemory<byte>>
    {
        public ReadOnlyMemory<byte> VisitAesGcm(SymmetricKeyAesGcm key)
        {
            return key.KeyBytes;
        }

        public ReadOnlyMemory<byte> VisitAesOpenPgpCfb(SymmetricKeyAesOpenPgpCfb key)
        {
            return key.KeyBytes;
        }
    }
}
