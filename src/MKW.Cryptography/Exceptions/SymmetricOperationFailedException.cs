// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class SymmetricOperationFailedException(Exception innerException)
        : CryptographyException("A symmetric cryptographic operation failed. See inner exception for details.", innerException)
    {
    }
}
