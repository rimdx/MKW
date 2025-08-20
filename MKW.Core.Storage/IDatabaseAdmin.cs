namespace MKW.Core.Storage
{
    public interface IDatabaseAdmin : IDatabaseUser, ISavable
    {
        HashSet<ReadOnlyMemory<byte>> Trust { get; set; }
    }
}
