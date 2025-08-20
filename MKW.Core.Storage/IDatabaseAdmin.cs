namespace MKW.Core.Storage
{
    public interface IDatabaseAdmin : IDatabaseUser, ISavable
    {
        IEnumerable<ReadOnlyMemory<byte>> EnumerateTrust();

        void AddTrust(ReadOnlyMemory<byte> data);
        void DeleteTrust(ReadOnlyMemory<byte> data);
    }
}
