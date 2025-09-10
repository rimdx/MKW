using MKW.Core.Cryptography.Exceptions;

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class AsymmetricOperationFailedException(Exception innerException)
        : CryptographyException("An asymmetric cryptographic operation failed. See inner exception for details.", innerException)
    {
    }
}
