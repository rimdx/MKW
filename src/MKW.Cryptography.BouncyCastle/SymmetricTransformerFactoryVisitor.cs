// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class SymmetricTransformerFactoryVisitor : SymmetricKey.IVisitor<ISymmetricTransformer>
    {
        public ISymmetricTransformer VisitAesGcm(SymmetricKeyAesGcm key)
        {
            return new AesGcmSymmetricTransformer(key);
        }

        public ISymmetricTransformer VisitAesOpenPgpCfb(SymmetricKeyAesOpenPgpCfb key)
        {
            return new AesOpenPgpTransformer(key);
        }
    }
}
