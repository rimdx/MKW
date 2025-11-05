// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class HashAlgorithmDigestFactoryVisitor : HashAlgorithm.IVisitor<IDigest>
    {
        public IDigest VisitSha1(HashAlgorithmSha1 algorithm)
        {
            return new Sha1Digest();
        }

        public IDigest VisitSha256(HashAlgorithmSha256 algorithm)
        {
            return new Sha256Digest();
        }
    }
}
