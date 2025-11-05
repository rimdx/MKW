// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public abstract class HashAlgorithm
    {
        public abstract T Visit<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitSha1(HashAlgorithmSha1 hashAlgorithmSha1);
            T VisitSha256(HashAlgorithmSha256 algorithm);
        }
    }
}
