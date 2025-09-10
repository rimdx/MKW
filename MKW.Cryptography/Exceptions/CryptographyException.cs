namespace MKW.Cryptography.Exceptions
{
    [Serializable]
    public class CryptographyException(string message, Exception? innerException)
        : Exception(message, innerException)
    {
    }
}
