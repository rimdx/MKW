// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class AsymmetricOperationFailedException(Exception innerException)
        : CryptographyException("An asymmetric cryptographic operation failed. See inner exception for details.", innerException)
    {
    }
}
