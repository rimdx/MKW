namespace MKW.Core.Cryptography.Exceptions
{
    [Serializable]
    public class AsymmetricOperationFailedException(Exception innerException)
        : CryptographyException("An asymmetric cryptographic operation failed. See inner exception for details.", innerException)
    {
    }
}
