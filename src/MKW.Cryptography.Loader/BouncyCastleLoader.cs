// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography.BouncyCastle;

namespace MKW.Cryptography.Loader
{
    public static class BouncyCastleLoader
    {
        public const string Name = "BouncyCastle.Cryptography";

        public const bool Supported = true;

        public static ICryptographyProvider GetProvider()
        {
            return new BouncyCastleCryptographyProvider();
        }
    }
}
