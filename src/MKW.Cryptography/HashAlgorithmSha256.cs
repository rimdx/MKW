// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public sealed class HashAlgorithmSha256 : HashAlgorithm
    {
        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitSha256(this);
        }
    }
}
