// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public static class CommonCryptographyAlgorithms
    {
        public static readonly SymmetricAlgorithmConfiguration Aes128Gcm =
            new SymmetricAlgorithmConfiguration
            {
                Engine = SymmetricAlgorithmEngine.AesGcm,
                KeySizeBits = 128,
                IVSizeBits = 128,
            };

        public static readonly SymmetricAlgorithmConfiguration Aes128OpenPgpCfb =
            new SymmetricAlgorithmConfiguration
            {
                Engine = SymmetricAlgorithmEngine.AesOpenPgpCfb,
                KeySizeBits = 128,
                IVSizeBits = 128,
            };

        public static readonly AsymmetricAlgorithmConfiguration Rsa2048 =
            new AsymmetricAlgorithmConfiguration
            {
                Engine = AsymmetricAlgorithmEngine.Rsa,
                HashEngine = HashAlgorithms.Sha256,
                StrengthBits = 2048,
            };

        public static readonly PasswordDerivationConfiguration Pbkdf2 =
            new PasswordDerivationConfiguration
            {
                Engine = PasswordDerivationEngine.Pbkdf2,
                HashEngine = HashAlgorithms.Sha256,
                Iterations = 100_000,
                KeySizeBits = 128,
                SaltSizeBits = 128,
            };

        public static readonly PasswordDerivationConfiguration OpenPgpStringToKey =
            new PasswordDerivationConfiguration
            {
                Engine = PasswordDerivationEngine.Pbkdf2,
                HashEngine = HashAlgorithms.Sha256,
                Iterations = 100_000,
                KeySizeBits = 128,
                SaltSizeBits = 64,
            };

        public static class HashAlgorithms
        {
            public static readonly HashAlgorithm Sha1 = new HashAlgorithmSha1();
            public static readonly HashAlgorithm Sha256 = new HashAlgorithmSha256();
        }
    }
}
