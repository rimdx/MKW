using MKW.Cryptography.Exceptions;

namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class InvalidKeyException : CryptographyException
    {
        public InvalidKeyException(string details)
            : base($"Invalid key: {details}", null)
        {
        }

        public InvalidKeyException(Exception ex)
            : base("Invalid key. See inner exception for details.", ex)
        {
        }
    }
}
