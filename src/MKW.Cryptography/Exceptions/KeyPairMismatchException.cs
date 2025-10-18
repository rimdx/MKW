// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class KeyPairMismatchException()
        : CryptographyException("The provided public and private keys do not form a valid key pair.", null)
    {
    }
}
