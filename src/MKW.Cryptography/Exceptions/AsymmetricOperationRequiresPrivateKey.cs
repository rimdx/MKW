// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class AsymmetricOperationRequiresPrivateKey()
        : CryptographyException("The requested asymmetric cryptographic operation requires a private key, but none was provided.", null)
    {
    }
}
