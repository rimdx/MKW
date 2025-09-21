namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class AsymmetricOperationRequiresPrivateKey()
        : CryptographyException("The requested asymmetric cryptographic operation requires a private key, but none was provided.", null)
    {
    }
}
