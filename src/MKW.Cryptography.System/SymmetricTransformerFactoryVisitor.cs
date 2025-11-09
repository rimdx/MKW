// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.System
{
    internal sealed class SymmetricTransformerFactoryVisitor : SymmetricKey.IVisitor<ISymmetricTransformer>
    {
        public ISymmetricTransformer VisitAesGcm(SymmetricKeyAesGcm key)
        {
            return AesGcmSymmetricTransformer.Open(key);
        }

        public ISymmetricTransformer VisitAesOpenPgpCfb(SymmetricKeyAesOpenPgpCfb key)
        {
            throw new NotSupportedException();
        }
    }
}