// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public abstract record class SymmetricKey
    {
        public abstract T Visit<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitAesGcm(SymmetricKeyAesGcm key);
            T VisitAesOpenPgpCfb(SymmetricKeyAesOpenPgpCfb key);
        }
    }
}
