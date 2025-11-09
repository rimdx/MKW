// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.Loader
{
    public static class CryptographyLoader
    {
        public static ICryptographyProvider GetProvider()
        {
            if (SystemCryptographyLoader.Supported)
            {
                return SystemCryptographyLoader.GetProvider();
            }
            else if (BouncyCastleLoader.Supported)
            {
                return BouncyCastleLoader.GetProvider();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static ICryptographyProvider GetProvider(string name) => name switch
        {
            BouncyCastleLoader.Name => BouncyCastleLoader.GetProvider(),
            SystemCryptographyLoader.Name => SystemCryptographyLoader.GetProvider(),
            _ => throw new NotSupportedException(),
        };
    }
}
