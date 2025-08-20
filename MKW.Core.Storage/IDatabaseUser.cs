namespace MKW.Core.Storage
{
    public interface IDatabaseUser : ISavable
    {
        Guid Id { get; }
        ReadOnlyMemory<byte> Salt { get; set; }
        ReadOnlyMemory<byte> PublicKey { get; set; }
        ReadOnlyMemory<byte> PrivateKey { get; set; }
    }
}
