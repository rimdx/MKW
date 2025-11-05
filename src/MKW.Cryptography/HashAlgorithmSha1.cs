// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public sealed class HashAlgorithmSha1 : HashAlgorithm
    {
        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitSha1(this);
        }
    }
}
