namespace MKW.Core.Storage
{
    public interface IDatabaseUser : ISavable
    {
        UserId Id { get; }

        ReadOnlyMemory<byte> Salt { get; set; }
        ReadOnlyMemory<byte> PublicKey { get; set; }
        ReadOnlyMemory<byte> PrivateKey { get; set; }

        void AddTrust(ReadOnlyMemory<byte> data);
        void DeleteTrust(ReadOnlyMemory<byte> data);
        IEnumerable<ReadOnlyMemory<byte>> EnumerateTrust();
    }
}
