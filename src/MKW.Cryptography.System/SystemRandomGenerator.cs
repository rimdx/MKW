// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Security.Cryptography;

namespace MKW.Cryptography.System
{
    internal sealed class SystemRandomGenerator : IRandomGenerator
    {
        public SystemRandomGenerator()
        {
        }

        public byte[] NextBytes(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }
    }
}