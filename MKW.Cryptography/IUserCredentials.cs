namespace MKW.Cryptography
{
    public interface IUserCredentials
    {
        ReadOnlyMemory<byte> ExportSalt();
        Memory<byte> GetSecretKey();
    }
}