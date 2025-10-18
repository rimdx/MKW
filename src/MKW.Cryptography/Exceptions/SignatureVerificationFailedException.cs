// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class SignatureVerificationFailedException(Exception ex)
        : CryptographyException("Signature verification failed. The data or signature may be corrupted or invalid.", ex)
    {
    }
}
