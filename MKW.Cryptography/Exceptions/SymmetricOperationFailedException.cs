using MKW.Cryptography.Exceptions;

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class SymmetricOperationFailedException(Exception innerException)
        : CryptographyException("A symmetric cryptographic operation failed. See inner exception for details.", innerException)
    {
    }
}
