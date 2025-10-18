// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Security;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class BouncyCastleRandomGenerator : IRandomGenerator
    {
        private readonly SecureRandom random;

        public BouncyCastleRandomGenerator()
        {
            random = new SecureRandom();
        }

        public byte[] NextBytes(int length)
        {
            return SecureRandom.GetNextBytes(random, length);
        }
    }
}
