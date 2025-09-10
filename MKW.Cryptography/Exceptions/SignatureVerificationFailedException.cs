using MKW.Core.Cryptography.Exceptions;

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class SignatureVerificationFailedException(Exception ex)
        : CryptographyException("Signature verification failed. The data or signature may be corrupted or invalid.", ex)
    {
    }
}
