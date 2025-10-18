// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MKW.Cryptography.BouncyCastle
{
    public sealed class UserCredentials : IUserCredentials, IDisposable
    {
        private readonly ReadOnlyMemory<byte> password;
        private readonly ReadOnlyMemory<byte> salt;
        private readonly PasswordDerivationConfiguration config;
        private readonly Pkcs5S2ParametersGenerator generator;

        public UserCredentials(ReadOnlyMemory<byte> password,
                               ReadOnlyMemory<byte> salt,
                               PasswordDerivationConfiguration config)
        {
            this.password = password;
            this.salt = salt;
            this.config = config;

            IDigest digest = config.HashEngine switch
            {
                HashAlgorithmEngine.Sha256 => DigestUtilities.GetDigest(NistObjectIdentifiers.IdSha256),
            };

            generator = new Pkcs5S2ParametersGenerator(digest);
        }

        public ReadOnlyMemory<byte> ExportSalt()
        {
            return salt.ToArray();
        }

        public Memory<byte> GetSecretKey()
        {
            generator.Init(password.Span, salt.Span, config.Iterations);

            KeyParameter key = (KeyParameter)generator.GenerateDerivedMacParameters(128);

            return key.GetKey();
        }

        public void Dispose()
        {
        }
    }
}
