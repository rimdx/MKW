using MKW.Core.Cryptography.Exceptions;

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class KeyPairMismatchException()
        : CryptographyException("The provided public and private keys do not form a valid key pair.", null)
    {
    }
}
