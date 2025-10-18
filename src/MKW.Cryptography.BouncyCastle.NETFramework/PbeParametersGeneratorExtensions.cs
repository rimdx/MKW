// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Crypto.Generators;

namespace MKW.Cryptography.BouncyCastle
{
    public static class PbeParametersGeneratorExtensions
    {
        public static void Init(this Pkcs5S2ParametersGenerator generator,
                                ReadOnlySpan<byte> password,
                                ReadOnlySpan<byte> salt,
                                int iterationCount)
        {
            generator.Init(password.ToArray(), salt.ToArray(), iterationCount);
        }
    }
}
