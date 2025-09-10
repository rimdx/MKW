namespace MKW.Core.Cryptography
{
    public interface IUserCredentials
    {
        ReadOnlyMemory<byte> ExportSalt();
        Memory<byte> GetSecretKey();
    }
}